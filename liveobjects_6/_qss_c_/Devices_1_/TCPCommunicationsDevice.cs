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

// #define DEBUG_TCPCommunicationsDevice

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace QS._qss_c_.Devices_1_
{
	/// <summary>
	/// Aaa... 
	/// </summary>
	public class TCPCommunicationsDevice 
		: CommunicationsDevice, IUnicastingDevice, ICommunicationsDevice
	{
		private static TimeSpan mainLoopKillingTimeout = TimeSpan.FromMilliseconds(200);
		private static TimeSpan connectionKillingTimeout = TimeSpan.FromMilliseconds(100);

        public TCPCommunicationsDevice(string name, IPAddress localAddress, QS.Fx.Logging.ILogger logger, bool workingAsServer,
            int portno, uint anticipatedMaximumNumberOfConcurrentConnections)
            : this(name, localAddress, logger, workingAsServer, portno, anticipatedMaximumNumberOfConcurrentConnections, false)
        {
        }

		public TCPCommunicationsDevice(string name, IPAddress localAddress, QS.Fx.Logging.ILogger logger, bool workingAsServer, 
			int portno, uint anticipatedMaximumNumberOfConcurrentConnections, bool reuseSocket) : base(portno)
		{
			this.logger = logger;
			this.workingAsServer = workingAsServer;
            this.name = name;

			// IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
			myIPAddress = localAddress; // ipHostInfo.AddressList[0];

			this.portno = workingAsServer ? portno : 0;

			bool randomize_port = workingAsServer && this.portno == 0;
			System.Random random = new System.Random();

			uint countdown = 5000;
			bool success = false;
			while (!success)
			{
				int port_touse = randomize_port ? (55000 + random.Next(5000)) : this.portno;
				try
				{
					IPEndPoint localEndPoint = new IPEndPoint(myIPAddress, port_touse);

					mysock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    mysock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                    if (reuseSocket)
                        mysock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

					mysock.Bind(localEndPoint);

					success = true;

					if (randomize_port)
					{
						this.portno = port_touse;
						logger.Log(this, "Selected a random port: " + port_touse.ToString() + ".");
					}
				}
				catch (Exception exc)
				{
					success = false;

					if (!randomize_port)
						throw new Exception("Could not bind socket.", exc);
					else
					{
						countdown--;
						if (countdown == 0)
							throw new Exception("Cound not bind socket to any address in the available range.", exc);
					}
				}
			}

			mysock.Listen((int) anticipatedMaximumNumberOfConcurrentConnections);

			listOfConnections = new System.Collections.ArrayList((int) anticipatedMaximumNumberOfConcurrentConnections);
			connections = new QS._core_c_.Collections.Hashtable(anticipatedMaximumNumberOfConcurrentConnections);

			this.doShutdown = false;

			mythread = new Thread(new ThreadStart(mainloop));
            mythread.Name = "Listener thread for TCP device {" + name + "}"; 
			mythread.Start();
		}

        private string name;

		public override void shutdown()
		{
			doShutdown = true;
			
			try
			{
				if (mysock != null)
					mysock.Close();
			}
			catch (Exception exc)
			{
				logger.Log(this, exc.ToString());
			}

			lock (listOfConnections)
			{
				foreach (Connection connection in listOfConnections)
				{
                    try
                    {
                        connection.P();
                        connection.disconnect();
                    }
                    catch (Exception exc)
                    {
                        logger.Log(this, exc.ToString());
                    }
                }
			}

			try
			{
				if (mythread != null)
				{
					mythread.Join(mainLoopKillingTimeout); 
					if (mythread.IsAlive)
						mythread.Abort();
					mythread = null;
				}
			}
			catch (Exception exc)
			{
				logger.Log(this, exc.ToString());
			}
		}

		private void mainloop()
		{
			while (!doShutdown)
			{
				try
				{
					Socket receivingSock = mysock.Accept();

					if (receivingSock == null)
						throw new Exception("Receiving socket is NULL.");

					if (!(receivingSock.RemoteEndPoint is IPEndPoint))
						throw new Exception("Remote endpoint isn't IP.");

					IPEndPoint remoteEndPoint = (IPEndPoint) receivingSock.RemoteEndPoint;

					Connection connection = null;
					lock (listOfConnections)
					{
						QS.Fx.Network.NetworkAddress incomingConnecitonNetAddr = 
							new QS.Fx.Network.NetworkAddress(remoteEndPoint.Address, remoteEndPoint.Port);
#if DEBUG_TCPCommunicationsDevice
						logger.Log(this, "Creating new INCOMING connection for address " + 
							incomingConnecitonNetAddr.ToString());
#endif
						connection = new Connection(this, incomingConnecitonNetAddr, receivingSock);								

						connections[connection.Addr] = connection;
						listOfConnections.Add(connection);

						connection.P();
					}

					connection.startup();
					connection.V();
				}
				catch (Exception exc)
				{
					if (!doShutdown)
					{
						logger.Log(this, "Listener failed : Exception = " + exc.ToString() + 
							", LastWin32Error = " + System.Runtime.InteropServices.Marshal.GetLastWin32Error());
					}
					else
					{
#if DEBUG_TCPCommunicationsDevice
						logger.Log(this, "Shutting down.");
#endif
					}
				}
			}
		}

		public IPAddress IPAddress
		{
			get
			{
				return this.myIPAddress;
			}
		}

		public int PortNumber
		{
			get
			{
				return portno;
			}
		}

		private bool workingAsServer;
		private QS.Fx.Logging.ILogger logger;
		private IPAddress myIPAddress;
		private bool doShutdown;
		private Thread mythread;
		private System.Collections.ArrayList listOfConnections;
		private QS._core_c_.Collections.IDictionary connections;
		private Socket mysock;

		#region IUnicastingDevice Members

		public void unicast(
			IPAddress nodeAddress, int receiverPortNo, byte[] buffer, int offset, int bufferSize)
		{
			QS.Fx.Network.NetworkAddress address = new QS.Fx.Network.NetworkAddress(nodeAddress, receiverPortNo);

			bool needsToConnect = false;
			Connection connection = null;

			lock (listOfConnections)
			{
				while (connection == null)
				{
					QS._core_c_.Collections.IDictionaryEntry dic_en = connections.lookupOrCreate(address);
					if (dic_en.Value == null)
					{
						needsToConnect = true;

						connection = new Connection(this, address);
						dic_en.Value = connection;

						listOfConnections.Add(connection);

#if DEBUG_TCPCommunicationsDevice
						logger.Log(this, "Sending TCP unicast via some new connection to " + address.ToString());		
#endif			
					}
					else
					{
						connection = (Connection) dic_en.Value;
						if (!connection.Connected)
						{
#if DEBUG_TCPCommunicationsDevice
							logger.Log(this, "Connection no longer connected : " + address.ToString());		
#endif

							listOfConnections.Remove(connection);
							connections.remove(connection.Addr);

							connection = null;
						}
						else
						{
#if DEBUG_TCPCommunicationsDevice
							logger.Log(this, "Sending TCP unicast via existing connection to " + address.ToString());		
#endif
						}
					}
				}

				connection.P();
			}

			try
			{
				if (needsToConnect)
				{
					connection.connect();

					if (!connection.Connected)
						throw new Exception("Cannot send, could not connect to " + connection.Addr.ToString());

#if DEBUG_TCPCommunicationsDevice
					logger.Log(this, "Connection established for destination " + address.ToString());		
#endif

					connection.startup();
				}

				connection.Sock.Send(System.BitConverter.GetBytes((uint) bufferSize));
				connection.Sock.Send(buffer, offset, bufferSize, SocketFlags.None);			

#if DEBUG_TCPCommunicationsDevice
				logger.Log(this, "Successfully sent data to " + address.ToString());		
#endif
			}
			catch (Exception exc)
			{
                logger.Log(this, "Cannot send to " + connection.Addr.ToString() + ", " + exc.ToString());
                connection.disconnect();
            }
            finally
			{
				connection.V();			
			}
		}

		#endregion

		private class Connection
		{
			private void mainloop()
			{
#if DEBUG_TCPCommunicationsDevice
				tcpDevice.logger.Log(this, "mainloop_enter : " + address.ToString());
#endif

				try
				{
					while (Connected && receiveObject())
						;
				}
				catch (Exception exc)
				{
					tcpDevice.logger.Log(
						this, "mainloop " + address.ToString() + ", exception : " + exc.ToString());

                    try
                    {
                        this.disconnect();
                    }
                    catch (Exception)
                    {
                    }
				}

#if DEBUG_TCPCommunicationsDevice
				tcpDevice.logger.Log(this, "mainloop_leave : " + address.ToString());
#endif
			}

			private bool receiveObject()
			{
#if DEBUG_TCPCommunicationsDevice
				tcpDevice.logger.Log(this, "receiveObject : " + address.ToString());
#endif

				try
				{
					P();
					Socket sock = this.socket;
					V();

					if (sock == null)
						throw new Exception("Socket is NULL");

					byte[] buffer = new byte[4];
					
					int offset = 0;
					while ( offset < 4)
					{
						int received_bytes = sock.Receive(buffer, offset, 4 - offset, SocketFlags.None);
						if (received_bytes <=0)
							throw new Exception("received bytes <= 0, something is wrong with this socket");

						offset += received_bytes;
					}

					uint bufferSize = System.BitConverter.ToUInt32(buffer, 0);
					
					if (bufferSize == 0)
						throw new Exception("bufferSize == 0");
					
					buffer = new byte[bufferSize];

					offset = 0; 
					while (offset < bufferSize)
					{
						int received_bytes = sock.Receive(buffer, offset, (int) bufferSize - offset, SocketFlags.None);
						if (received_bytes <=0)
							throw new Exception("received bytes <= 0, something is wrong with this socket");

						offset+= received_bytes;
					}

#if DEBUG_TCPCommunicationsDevice
					tcpDevice.logger.Log(this, 
						"successfully received " + bufferSize + " bytes from " + address.ToString());
#endif

					tcpDevice.dispatch(address.HostIPAddress, address.PortNumber, buffer, bufferSize);
					return true;
				}
#if DEBUG_TCPCommunicationsDevice
				catch (Exception exc)
				{
					tcpDevice.logger.Log(this, "receiveObject failed : " + exc.ToString());
#else
				catch (Exception)
				{
#endif
					return false;
				}
			}

			public Connection(TCPCommunicationsDevice tcpDevice, QS.Fx.Network.NetworkAddress address)
			{
				initialize(tcpDevice, address, true, null);
			}

			public Connection(TCPCommunicationsDevice tcpDevice, QS.Fx.Network.NetworkAddress address, Socket socket)
			{
				initialize(tcpDevice, address, false, socket);
			}

			private void initialize(
				TCPCommunicationsDevice tcpDevice, QS.Fx.Network.NetworkAddress address, bool outgoing, Socket socket)
			{
				this.tcpDevice = tcpDevice;

				this.address = address;
				this.outgoing = outgoing;
				mutex = new Mutex();
				if (socket != null)
					this.socket = socket;
				else
				{
					this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					IPEndPoint localEndPoint = new IPEndPoint(tcpDevice.myIPAddress, 0);
					this.socket.Bind(localEndPoint);
				}

				listenerThread = new Thread(new ThreadStart(this.mainloop));
			}

			public void startup()
			{
				listenerThread.Start();
			}

			public void connect()
			{
				if (!outgoing)
					throw new Exception("This is an incoming socket!");

				IPEndPoint destination = new IPEndPoint(address.HostIPAddress, address.PortNumber);
				socket.Connect(destination);
				if (!socket.Connected)
					throw new Exception("Cannot connect to " + address.ToString());
			}

			public void disconnect()
			{
                try
                {
                    if (socket != null)
                    {
                        try
                        {
                            socket.Shutdown(SocketShutdown.Both);
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            socket.Close();
                        }
                        catch (Exception)
                        {
                        }
                    }

                    socket = null;

                    if (listenerThread != null)
                    {
                        listenerThread.Join(connectionKillingTimeout);
                        if (listenerThread.IsAlive)
                            listenerThread.Abort();
                        listenerThread = null;
                    }
                }
                catch (Exception exc)
                {
                    tcpDevice.logger.Log(this, "__disconnect: " + exc.ToString());
                }
            }

			public bool Connected
			{
				get
				{
					return socket != null && (socket.Connected || !outgoing);
				}
			}

			public void P()
			{
				mutex.WaitOne();
			}

			public void V()
			{
				mutex.ReleaseMutex();
			}

			public Socket Sock
			{
				get
				{
					return socket;
				}
			}

			public QS.Fx.Network.NetworkAddress Addr
			{
				get
				{
					return address;
				}
			}

			private Socket socket = null;
			private Mutex mutex;
			private bool outgoing;
			private QS.Fx.Network.NetworkAddress address;
			private TCPCommunicationsDevice tcpDevice;
			private Thread listenerThread;
		}
	}	
}
