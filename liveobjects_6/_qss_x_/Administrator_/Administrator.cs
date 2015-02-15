/*

Copyright (c) 2004-2009 Krzysztof Ostrowski. All rights reserved.

Redistribution and use in source and binary forms,
with or without modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above
   copyright notice, this list of conditions and the following
   disclaimer in the documentation and/or other materials provided
   with the distribution.

THIS SOFTWARE IS PROVIDED "AS IS" BY THE ABOVE COPYRIGHT HOLDER(S)
AND ALL OTHER CONTRIBUTORS AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE ABOVE COPYRIGHT HOLDER(S) OR ANY OTHER
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
SUCH DAMAGE.

*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.ServiceModel;
using System.IO;
using System.Security.Cryptography;
using System.ServiceModel.Description;
using System.Diagnostics;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;

namespace QS._qss_x_.Administrator_
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public sealed class Administrator : IDisposable, IAdministrator
    {
        #region Constructors

        public Administrator(string subnet, int port, QS.Fx.Logging.ILogger logger, bool verbose, string rootfolder, bool executable, string authentication, string certificate)
        {
            this.rootfolder = rootfolder;
            this.logger = logger;
            this.executable = executable;
            this.verbose = verbose;
            QS._qss_c_.Base1_.Subnet _subnet = new QS._qss_c_.Base1_.Subnet(subnet);
            bool found = false;
            foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && _subnet.contains(address))
                {
                    this.localaddress = address;
                    found = true;
                    break;
                }
            }
            if (!found)
                throw new Exception("Cannot find any local ip address on the requested subnet " + _subnet.ToString() + ".");
            this.port = port;
            this.symmetricalgorithm = SymmetricAlgorithm.Create();
            this.random = new Random();
            this.connections = new System.Collections.ObjectModel.Collection<Connection>();
            this.servicehost = new ServiceHost(this);
            string endpointaddress = "http://" + localaddress.ToString() + ":" + port.ToString() + "/Administrator";
            if (verbose)
                logger.Log("Hosting the administrator service at " + endpointaddress + ".");
            WSHttpBinding binding = new WSHttpBinding();
            binding.Security.Mode = SecurityMode.Message;
            if ((authentication == null) || authentication.Equals("windows"))
                binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            else if (authentication.Equals("username"))
                binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            else
                throw new Exception("Unknown authentication type: \"" + authentication + "\"");
            this.servicehost.AddServiceEndpoint(typeof(IAdministrator), binding, endpointaddress);
            if (certificate != null)
                this.servicehost.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, certificate);
            this.servicehost.Open();
        }        

        #endregion

        #region Fields

        private IPAddress localaddress;
        private int port;
        private SymmetricAlgorithm symmetricalgorithm;
        private ServiceHost servicehost;
        private Random random;
        private ICollection<Connection> connections;
        private QS.Fx.Logging.ILogger logger;
        private bool verbose, executable;
        private string rootfolder;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                if (verbose)
                    logger.Log("Closing the administrator service.");

                if (this.servicehost != null)
                    this.servicehost.Close();

                foreach (Connection connection in connections)
                    ((IDisposable)connection).Dispose();
            }
        }

        #endregion

        #region IAdministrator Members

        void IAdministrator.Connect(string client_ipaddress, int client_portno,
            out string server_ipaddress, out int server_portno, out int connection_id, out byte[] iv, out byte[] key)
        {
            _UserCheck();
            lock (this)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, false);
                socket.Bind(new IPEndPoint(localaddress, 0));
                int newport = ((IPEndPoint)socket.LocalEndPoint).Port;
                socket.Listen(1);
                server_ipaddress = localaddress.ToString();
                server_portno = newport;
                connection_id = random.Next();
                ICryptoTransform encryptor, decryptor;
                lock (symmetricalgorithm)
                {
                    symmetricalgorithm.GenerateIV();
                    symmetricalgorithm.GenerateKey();
                    iv = symmetricalgorithm.IV;
                    key = symmetricalgorithm.Key;
                    encryptor = symmetricalgorithm.CreateEncryptor(key, iv);
                    decryptor = symmetricalgorithm.CreateDecryptor(key, iv);
                }
                if (verbose)
                    logger.Log("Negotiated a secure connection [ " + connection_id.ToString() + " ] from [ " + 
                        client_ipaddress + " : " + client_portno.ToString() + " ] to [ " + localaddress.ToString() + " : " + newport.ToString() + " ].");
                connections.Add(new Connection(connection_id, IPAddress.Parse(client_ipaddress), client_portno, socket, encryptor, decryptor,
                    new QS.Fx.Base.ContextCallback<Connection>(this._DisconnectCallback), logger, verbose, rootfolder));
            }
        }

        void IAdministrator.Execute(string target, string folder, string arguments, TimeSpan timeout, out string output)
        {
            _UserCheck();
            if (verbose)
                logger.Log("Executing \"" + target + "\" with arguments \"" + arguments + "\" in folder \"" + folder + "\", with timeout " +
                    timeout.ToString() + ".");
            try
            {
                if (!executable)
                    throw new Exception("Execution of processes has been disabled.");
                _RootCheck(target);
                if (folder != null)
                    _RootCheck(folder);
                Process process = new Process();
                process.StartInfo.FileName = target;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WorkingDirectory = folder;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
                process.WaitForExit((int)Math.Round(timeout.TotalMilliseconds));
                StringBuilder _output = new StringBuilder();
                bool failed = false;
                if (!process.HasExited)
                {
                    failed = true;
                    _output.AppendLine("Timeout expired, terminating the process forcifully.");
                    process.Kill();
                    while (!process.HasExited)
                    {
                        process.WaitForExit(100);
                        process.Kill();
                    }
                }
                string _s;
                _s = process.StandardOutput.ReadToEnd().Trim(' ', '\n');
                if (_s.Length > 0)
                    _output.AppendLine(_s);
                _s = process.StandardError.ReadToEnd().Trim(' ', '\n');
                if (_s.Length > 0)
                    _output.AppendLine(_s);
                if (process.ExitCode != 0)
                {
                    failed = true;
                    _output.AppendLine("The process exit code is nonzero.");
                }
                output = _output.ToString();
                if (verbose)
                    logger.Log("Output:\n" + output);
                if (failed)
                    throw new Exception("The process execution has failed.");
                if (verbose)
                    logger.Log("Execution completed successfully.\n");
            }
            catch (Exception exc)
            {
                if (verbose)
                    logger.Log("Execution failed.\n" + exc.ToString());
                throw exc;
            }
        }

        #endregion

        #region _DisconnectCallback

        private void _DisconnectCallback(Connection connection)
        {
            lock (this)
            {
                connections.Remove(connection);
            }
        }

        #endregion

        #region _RootCheck

        private void _RootCheck(string _path)
        {
            Administrator._RootCheck(this.rootfolder, _path);
        }

        public static void _RootCheck(string _rootfolder, string _path)
        {
            if (_rootfolder != null)
            {
                string _fullpath = Path.GetFullPath(_path);
                bool isok = false;
                if (_fullpath.StartsWith(_rootfolder))
                {
                    string _subpath = _fullpath.Substring(_rootfolder.Length);
                    if (!_subpath.Contains("\\..\\") && !_subpath.Contains("/../") && !_subpath.Contains("/..\\") && !_subpath.Contains("\\../"))
                        isok = true;
                }
                if (!isok)
                    throw new Exception("Cannot read from or write to \"" + _fullpath + "\" because it is not in \"" + _rootfolder + "\".");
            }
        }

        #endregion

        #region _UserCheck

        private void _UserCheck()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (!identity.IsAuthenticated)
                throw new Exception("The user has not been authenticated.");
            if (identity.IsGuest || identity.IsAnonymous)
                throw new Exception("the user is a guest or anonymous.");
            logger.Log("Connection request from an authenticated user \"" + identity.Name + "\".");
        }

        #endregion
    }
}
