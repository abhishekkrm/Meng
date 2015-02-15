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

// #define DEBUG_AttachDebuggerOnStart
// #define DEBUG_RunApp

using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace QS._qss_e_.Runtime_
{
/*
    public interface IAppController
    {
        bool IsRunning
        {
            get;
        }

        void Start();
        void Stop();
    }
*/

	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class RunApp
	{
/*
        private class AppController : IAppController
        {
            public AppController()
            {
            }

            private TMS.Runtime.IApplicationRef applicationRef;

            public TMS.Runtime.IApplicationRef ApplicationRef
            {
                get { return applicationRef; }
                set { applicationRef = value; }
            }

            #region IAppController Members

            bool IAppController.IsRunning
            {
                get { throw new Exception("The method or operation is not implemented."); }
            }

            void IAppController.Start()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            void IAppController.Stop()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }
*/

		private static TimeSpan defaultTimeoutForSSHControlledRemoteOperations = TimeSpan.FromSeconds(5);
		private static TimeSpan defaultTimeoutForServiceControlledRemoteOperations = TimeSpan.FromSeconds(5);

        public static void Run(string[] args, QS.Fx.Logging.IConsole console, ManualResetEvent toAbort)
        {
            Run(args, console, null, toAbort);
        }

		public static void Run(string[] args, QS.Fx.Logging.IConsole console, QS._qss_e_.Inspection_.IInspector inspector, ManualResetEvent toAbort)
		{
#if DEBUG_AttachDebuggerOnStart
            System.Diagnostics.Debugger.Launch();
            System.Diagnostics.Debugger.Break();
#endif

            QS._core_c_.Base.Logger.DefaultClock = QS._core_c_.Core.Clock.SharedClock;

			console.Log("QS.TMS.Runtime.RunApp.Run: CLR version is " + System.Environment.Version.ToString());
            console.Log("Platform: " + System.Environment.OSVersion.ToString());
            console.Log("Processors detected: " + System.Environment.ProcessorCount.ToString());
            if (System.Environment.ProcessorCount > 1)
            {
                System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(1);
                console.Log("Affinity: " + System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity.ToString());

/*
                if (System.Environment.OSVersion.Platform == PlatformID.Win32NT
                    && System.Environment.OSVersion.Version.Major == 5
                    && System.Environment.OSVersion.Version.Minor == 2)
                {
                    ulong processMask, systemMask;
                    if (QS.CMS.Native.Win32.GetProcessAffinityMask(
                        System.Diagnostics.Process.GetCurrentProcess().Handle,
                        out processMask, out systemMask) != 0)
                    {
                        console.writeLine("Affinity Mask:\t" + processMask.ToString() + " / " + systemMask.ToString());
                    }
                }
*/
            }

//			QS.Fx.Logging.IConsole console = ;

// #if DEBUG_RunTest
//			using (System.IO.StreamWriter w = new System.IO.StreamWriter("runtest-error.txt"))
//			{
//				string s = null;
//				for (uint ind = 0; ind < args.Length; ind++)
//					s = ((s != null) ? (s + ", ") : "") + args[ind];
//				w.WriteLine("Arguments: " + s);
//				w.Flush();
//			}
// #endif

			try
			{
				if (args.Length < 1)
					throw new Exception("Need to first specify the type of the test.");
				string testClass = args[0];

				System.Type type = null;
                foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = assembly.GetType(testClass);
                    if (type != null)
                    {
                        console.Log("Type : " + type.AssemblyQualifiedName);
                        break;
                    }
                }
                                
				if (type == null)
					throw new Exception("unknown type");
				
				QS._core_c_.Components.AttributeSet argumentSet = new QS._core_c_.Components.AttributeSet(args.Length);
				for (uint ind = 1; ind < args.Length; ind++)
				{
					string argument = args[ind];

					if (!argument.StartsWith("-"))
						throw new Exception("Argument with an incorrect syntaxt detected : " + argument);

					int separator_position = argument.IndexOf(":");
					if (separator_position < 0)
						argumentSet[argument.Substring(1)] = null;
					else
						argumentSet[argument.Substring(1, separator_position - 1)] = argument.Substring(separator_position + 1);
				}		

				QS._qss_e_.Runtime_.RunApp.Run(type, argumentSet, console, inspector, toAbort);
			}
			catch (Exception exc)
			{
				Console.WriteLine("Exception : " + exc.ToString());			
			}
		}	

		public static void Run(Type testType, QS._core_c_.Components.AttributeSet argumentSet,
            QS.Fx.Logging.IConsole console, QS._qss_e_.Inspection_.IInspector inspector, ManualResetEvent toAbort)
		{
#if DEBUG_RunApp
			using (System.IO.StreamWriter runapp_logfileWriter = new System.IO.StreamWriter("runapp-error.txt"))
			{
#endif

				System.IO.StreamWriter logfileWriter = null;

				try
				{
#if DEBUG_RunApp
					runapp_logfileWriter.WriteLine("starting");
					runapp_logfileWriter.Flush();
#endif

					if (argumentSet.contains("logfile"))
					{
						logfileWriter = new System.IO.StreamWriter((string) argumentSet["logfile"]);
						argumentSet.remove("logfile");

						logfileWriter.WriteLine("Run(" + ((testType != null) ? testType.ToString() : "null") + ", " + argumentSet.ToString() + ")\n");
						logfileWriter.Flush();
					}

					if (testType == null)
						throw new Exception("type is null");
				
					// if (!testType.IsSubclassOf(typeof(System.IDisposable)))
					//	throw new Exception("Test type " + testType.ToString() + " does not implement System.IDisposable.");
				
					System.Reflection.ConstructorInfo typeConstructor = testType.GetConstructor(
						new System.Type[] { typeof(QS.Fx.Platform.IPlatform), typeof(QS._core_c_.Components.AttributeSet) });
					if (typeConstructor == null)
						throw new Exception("could not locate the appropriate constructor");

					if (argumentSet.contains("sendlog"))
					{				
						QS.Fx.Network.NetworkAddress networkAddress = new QS.Fx.Network.NetworkAddress((string) argumentSet["sendlog"]);
						argumentSet.remove("sendlog");

						using (QS._qss_c_.Components_1_.NetworkConsole networkConsole = new QS._qss_c_.Components_1_.NetworkConsole(
								   new QS._core_c_.Base.Logger(null, false, console), IPAddress.Parse((string) argumentSet["base"]), networkAddress))
						{                            
							launch(typeConstructor, argumentSet, (argumentSet.contains("copylog") ?
                                ((QS.Fx.Logging.IConsole) (new QS._qss_c_.Base1_.Consoles(
                                    new QS.Fx.Logging.IConsole[] { networkConsole, console }))) 
                                    : ((QS.Fx.Logging.IConsole) networkConsole)),                                 
                                logfileWriter, inspector, toAbort);
						}
					}
					else
                        launch(typeConstructor, argumentSet, console, logfileWriter, inspector, toAbort);

					Console.WriteLine("\nCompleted.");
				}
				catch (Exception exc)
				{
					if (logfileWriter != null)
					{
						try
						{
							logfileWriter.WriteLine(exc.ToString() + "\n");
							logfileWriter.Flush();
						}
						catch (Exception)
						{
						}
					}

#if DEBUG_RunApp
					runapp_logfileWriter.WriteLine(exc.ToString());
					runapp_logfileWriter.Flush();
#endif

					Console.WriteLine(exc.ToString());
				}		
#if DEBUG_RunApp
			}
#endif
		}

		private class OurConsole : QS.Fx.Logging.IConsole
		{
			public OurConsole(System.IO.StreamWriter writer, QS.Fx.Logging.IConsole console)
			{
				this.writer = writer;
				this.console = console;
			}

			private System.IO.StreamWriter writer;
			private QS.Fx.Logging.IConsole console;

			#region IConsole Members

			public void Log(string s)
			{
				try
				{
					writer.WriteLine(s + "\n");
					writer.Flush();
				}
				catch (Exception)
				{
				}

				try
				{
					console.Log(s);
				}
				catch (Exception)
				{
				}
			}

			#endregion
		}

		private static void launch(System.Reflection.ConstructorInfo typeConstructor, 
			QS._core_c_.Components.AttributeSet argumentSet, QS.Fx.Logging.IConsole console, 
            System.IO.StreamWriter logfileWriter, QS._qss_e_.Inspection_.IInspector inspector, ManualResetEvent toAbort)
		{
			QS.Fx.Network.NetworkAddress controllerAddress = null;
			if (argumentSet.contains("rsync"))
			{
				controllerAddress = new QS.Fx.Network.NetworkAddress((string) argumentSet["rsync"]);
				argumentSet.remove("rsync");

                QS.GUI.Components.RepositorySubmit1.DefaultRepositoryServerName =
                    System.Net.Dns.GetHostEntry(controllerAddress.HostIPAddress).HostName + ":" +
                    QS._qss_d_.Service_2_.Service.DefaultTcpChannelPort.ToString();
			}

			bool waitForKey = argumentSet.contains("wait");
			if (waitForKey)
				argumentSet.remove("wait");

			QS._core_c_.Base.IReadableLogger logger = new QS._core_c_.Base.Logger(null, false, 
				((logfileWriter != null) ? new OurConsole(logfileWriter, console) : console));

			if (argumentSet.contains("here"))
			{
				argumentSet.remove("here");

				using (QS._qss_e_.Runtime_.IEnvironment environment = new QS._qss_e_.Runtime_.LocalNode(new QS._qss_c_.Platform_.PhysicalPlatform(logger), true))
				{
					execute(environment.Nodes[0], logger, typeConstructor, argumentSet, controllerAddress, waitForKey, inspector, toAbort);
				}				
			} 
			else if (argumentSet.contains("ssh"))
			{
				string sshSequence = (string) argumentSet["ssh"]; 
				argumentSet.remove("ssh");

				int semi_ind = sshSequence.IndexOf(";");
				string localSSHSeq = sshSequence.Substring(0, semi_ind);
				string remoteSSHSeq = sshSequence.Substring(semi_ind + 1);
				
				int comma_ind = localSSHSeq.IndexOf(",");
				QS.Fx.Network.NetworkAddress partialLocalAddr = new QS.Fx.Network.NetworkAddress(localSSHSeq.Substring(0, comma_ind));
				IPAddress localBaseAddr = partialLocalAddr.HostIPAddress;
				uint consolePortNo = (uint) partialLocalAddr.PortNumber;
				uint controllerPortNo = Convert.ToUInt32(localSSHSeq.Substring(comma_ind + 1));

				int colon_ind = remoteSSHSeq.IndexOf(":");
				IPAddress remoteNodeAddr = IPAddress.Parse(remoteSSHSeq.Substring(0, colon_ind));
				string launchPath = remoteSSHSeq.Substring(colon_ind + 1);
				
				argumentSet["base"] = remoteNodeAddr.ToString();

				using (QS._qss_e_.Runtime_.SSHControlledNode node = new QS._qss_e_.Runtime_.SSHControlledNode(logger, localBaseAddr, 
						   consolePortNo, controllerPortNo, remoteNodeAddr, launchPath, defaultTimeoutForSSHControlledRemoteOperations))
				{
					node.connect();
					execute(node, logger, typeConstructor, argumentSet, controllerAddress, true, inspector, toAbort);
				}					
			}
			else if (argumentSet.contains("node"))
			{
				string addressSequence = (string) argumentSet["node"]; 
				argumentSet.remove("node");

				int semi_ind = addressSequence.IndexOf(";");
				string localAddrSeq = addressSequence.Substring(0, semi_ind);
				string remoteAddrSeq = addressSequence.Substring(semi_ind + 1);

				int comma_ind = localAddrSeq.IndexOf(",");
				QS.Fx.Network.NetworkAddress partialLocalAddr = new QS.Fx.Network.NetworkAddress(localAddrSeq.Substring(0, comma_ind));
				IPAddress localBaseAddr = partialLocalAddr.HostIPAddress;
				uint consolePortNo = (uint) partialLocalAddr.PortNumber;
				uint controllerPortNo = Convert.ToUInt32(localAddrSeq.Substring(comma_ind + 1));

				int colon_ind = remoteAddrSeq.IndexOf(":", remoteAddrSeq.IndexOf(":") + 1);
				QS.Fx.Network.NetworkAddress serviceAddr = new QS.Fx.Network.NetworkAddress(remoteAddrSeq.Substring(0, colon_ind));
				string launchPath = remoteAddrSeq.Substring(colon_ind + 1);
				
				argumentSet["base"] = serviceAddr.HostIPAddress.ToString();

				QS._qss_d_.Service_2_.IClient serviceClient = new QS._qss_d_.Service_2_.Client(localBaseAddr, new QS.Fx.Network.NetworkAddress(IPAddress.Any, 0),
					QS._qss_d_.Base_.Win32Config.DefaultCryptographicKeyFile);
				using (QS._qss_e_.Runtime_.ServiceControlledNode node = new QS._qss_e_.Runtime_.ServiceControlledNode(logger, localBaseAddr, consolePortNo,
					controllerPortNo, serviceClient, serviceAddr, defaultTimeoutForServiceControlledRemoteOperations, launchPath))
				{
					node.connect();
					execute(node, logger, typeConstructor, argumentSet, controllerAddress, true, inspector, toAbort);
				}
			}
			else
				throw new Exception("execution environment has not been specified");
		}

		private static void execute(QS._qss_e_.Runtime_.INodeRef nodeRef, QS.Fx.Logging.ILogger logger, System.Reflection.ConstructorInfo typeConstructor, 
			QS._core_c_.Components.AttributeSet argumentSet, QS.Fx.Network.NetworkAddress controllerAddress, bool waitForKey,
            QS._qss_e_.Inspection_.IInspector inspector, ManualResetEvent toAbort)
		{
// #if DEBUG_RunApp
//			logger.Log(null, "execute_enter");
// #endif

			try
			{
//				using (TMS.Runtime.IApplicationRef applicationRef = nodeRef.launch(typeConstructor, new object[] { argumentSet }))
				using (QS._qss_e_.Runtime_.IApplicationRef applicationRef = nodeRef.launch(typeConstructor.ReflectedType.FullName, argumentSet))
				{
                    if (inspector != null)
                        inspector.Add(applicationRef);

					// #if DEBUG_RunApp
					//				logger.Log(null, "application created");
					// #endif

					if (controllerAddress != null)
					{
						using (QS._qss_c_.Components_1_.ControllerClient controllerClient = new QS._qss_c_.Components_1_.ControllerClient(
								   logger, IPAddress.Parse((string) argumentSet["base"]), controllerAddress))
						{
							controllerClient.synchronize();  

                            try
                            {
                                applicationRef.Controller = controllerClient;
                            }
                            catch (Exception exc)
                            {
                                logger.Log(null, exc.ToString());
                            }

                            for (QS._qss_c_.Components_1_.CallRequest callRequest = controllerClient.NextRequest(toAbort); 
								callRequest != null; callRequest = controllerClient.NextRequest(toAbort))
							{
                                if (toAbort.WaitOne(0, false))
                                {
                                    Console.WriteLine("Abort signal received!");
                                    break;
                                }

#if DEBUG_RunApp
								logger.Log(null, "application call received : " + callRequest.ToString());
#endif

								try
								{
									object response = applicationRef.invoke(callRequest.MethodToCall, callRequest.ArgumentObjects);

#if DEBUG_RunApp
									logger.Log(null, "responding with " + 
										((response != null) ? (response.ToString() + ":" + response.GetType().ToString()) : "null"));
#endif
								
									controllerClient.respond(callRequest, response);
								}
								catch (Exception exc)
								{
									logger.Log(null, exc.ToString());
									controllerClient.respond(callRequest, exc.ToString());
								}
							}
						}
					}

					if (waitForKey)
						Console.ReadLine();
				}
			}
			catch (Exception exc)
			{
				logger.Log(null, "Could not launch application : " + exc.ToString());				
			}
		}
    }
}
