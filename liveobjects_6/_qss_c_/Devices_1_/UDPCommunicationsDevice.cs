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
	public class UDPCommunicationsDevice 
		: CommunicationsDevice, IUnicastingDevice, IMulticastingDevice
	{
		public UDPCommunicationsDevice(QS.Fx.Logging.ILogger logger,
			bool workingAsServer, int portno, uint anticipatedNumberOfGroups) : base(portno)
		{
			this.logger = logger;
			this.workingAsServer = workingAsServer;

			myIPAddress = Dns.GetHostAddresses(Dns.GetHostName())[0];

			ccsock = new Socket(AddressFamily.InterNetwork, 
				SocketType.Dgram, ProtocolType.Udp);

			myGroups = new QS._core_c_.Collections.Hashtable(anticipatedNumberOfGroups);

			mysock = new Socket(AddressFamily.InterNetwork, 
				SocketType.Dgram, ProtocolType.Udp);
			IPEndPoint localEndPoint = new IPEndPoint(myIPAddress, workingAsServer ? portno : 0);
			mysock.Bind(localEndPoint);

			if (workingAsServer)
			{
				groupSockets = new ArrayList();			
			}
			else
			{
				this.portno = ((IPEndPoint) mysock.LocalEndPoint).Port;
			}

			this.doShutdown = false;

			mythread = new Thread(new ThreadStart(mainloop));
			mythread.Start();
		}

		public override void shutdown()
		{
			if (ccsock != null)
				ccsock.Close();				

			doShutdown = true;

			if (mysock != null)
				mysock.Close();
		
			mythread.Join();
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

		private class MulticastingGroup
		{
			public MulticastingGroup(
				bool workingAsServer, IPAddress localAddress, IPAddress ipAddress, int portno)
			{
				this.workingAsServer = workingAsServer;
				this.ipAddress = ipAddress;
				this.portno = portno;
				this.localAddress = localAddress;
			}

			public void initalize()
			{				
				cSocket = new Socket(AddressFamily.InterNetwork, 
					SocketType.Dgram, ProtocolType.Udp);

				if (workingAsServer)
				{
					sSocket = new Socket(AddressFamily.InterNetwork, 
						SocketType.Dgram, ProtocolType.Udp);
					IPEndPoint sipep = new IPEndPoint(IPAddress.Any, portno);
					sSocket.Bind(sipep);
					sSocket.SetSocketOption(SocketOptionLevel.IP,
						SocketOptionName.AddMembership,
						new MulticastOption(ipAddress, localAddress));
				}

				cSocket.SetSocketOption(SocketOptionLevel.IP, 
					SocketOptionName.MulticastTimeToLive, 1);					
			}			

			public void send(
				int receiverPortNo, byte[] buffer, int offset, int bufferSize)
			{
				IPEndPoint cipep = new IPEndPoint(ipAddress, receiverPortNo);
				cSocket.SendTo(buffer, offset, bufferSize, SocketFlags.None, cipep);
			}

			public void cleanup()
			{
				sSocket.Close();
				cSocket.Close();
			}

			public void P()
			{
				mylock.WaitOne();
			}

			public void V()
			{
				mylock.ReleaseMutex();
			}

			public Socket sSocket, cSocket;

			private bool workingAsServer;
			private IPAddress ipAddress, localAddress;
			private int portno;
			private Mutex mylock = new Mutex();
		}

		private bool workingAsServer;
		private Thread mythread;
		private IPAddress myIPAddress;
		private Socket mysock, ccsock;
		private bool doShutdown;
		private QS._core_c_.Collections.Hashtable myGroups;
		private System.Collections.ArrayList groupSockets;
		private QS.Fx.Logging.ILogger logger;

		private void mainloop()
		{
			try
			{
				IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
				EndPoint senderRemote = (EndPoint) sender;

				ArrayList socklist;
				for (socklist = new ArrayList(); !doShutdown; socklist.Clear())
				{
					socklist.Add(mysock);

					if (workingAsServer)
					{
						lock (groupSockets)
						{
							socklist.AddRange(groupSockets);
						}
					}

					Socket.Select(socklist, null, null, 10000); 

					foreach (Socket someSocket in socklist)
					{
						int bytesAvailable = someSocket.Available;
						if (bytesAvailable > 0)
						{
							byte[] buffer = new byte[bytesAvailable];
							int bytesReceived = someSocket.ReceiveFrom(buffer, bytesAvailable, SocketFlags.None, ref senderRemote);

							// Debug.Assert(bytesReceived == bytesAvailable);

							if (senderRemote is IPEndPoint)						
							{
								dispatch(((IPEndPoint) senderRemote).Address, ((IPEndPoint) senderRemote).Port, buffer, (uint) bytesReceived);
							}
						}
					}
				}  
			}
			catch (Exception exc)
			{
				if (!doShutdown)
					logger.Log(this, "listener failed : " + exc.ToString());
			}
		}

		#region IUnicastingDevice Members

		public void unicast(IPAddress receiverAddress, int receiverPortNo, 
			byte[] buffer, int offset, int bufferSize)
		{
			IPEndPoint endPoint = new IPEndPoint(receiverAddress, receiverPortNo);
			lock (ccsock)
			{
				ccsock.SendTo(
					buffer, offset, bufferSize, SocketFlags.None, endPoint);
			}
		}

		#endregion

		#region IMulticastingDevice Members

		public void multicast(IPAddress receiverGroupAddress, int receiversPortNo,
			byte[] buffer, int offset, int bufferSize)
		{
			bool multicasting;
			MulticastingGroup theGroup;

			lock (myGroups)
			{
				theGroup = (MulticastingGroup) myGroups[receiverGroupAddress];

				multicasting = theGroup != null;
				if (multicasting)
					theGroup.P();
			}

			if (multicasting)
			{
				theGroup.send(receiversPortNo, buffer, offset, bufferSize);
				theGroup.V();
			}			
		}

		public void join(IPAddress groupAddress, int listeningPortNo)
		{
			bool creating_new;
			MulticastingGroup newGroup = null;

			lock (myGroups)
			{
				QS._core_c_.Collections.IDictionaryEntry entry = 
					myGroups.lookupOrCreate(groupAddress);
				
				creating_new = entry.Value == null;
				if (creating_new)
				{								
					newGroup = new MulticastingGroup(
						workingAsServer, this.myIPAddress, groupAddress, listeningPortNo);
					entry.Value = newGroup;

					newGroup.P();
				}
			}

			if (creating_new)
			{
				newGroup.initalize();		
		
				if (workingAsServer)
				{
					lock (groupSockets)
					{
						groupSockets.Add(newGroup.sSocket);
					}
				}

				newGroup.V();
			}
		}

		public void leave(IPAddress groupAddress)
		{
			bool removing_old;
			MulticastingGroup oldGroup = null;

			lock (myGroups)
			{
				QS._core_c_.Collections.IDictionaryEntry entry = 
					myGroups.remove(groupAddress);

				removing_old = entry != null;
				if (removing_old)
				{
					oldGroup = (MulticastingGroup) entry.Value;
					oldGroup.P();
				}
			}

			if (removing_old)
			{
				if (workingAsServer)
				{
					lock (groupSockets)
					{
						try
						{
							groupSockets.Remove(oldGroup.sSocket);
						}
						catch (Exception)
						{
						}
					}
				}

				oldGroup.cleanup();
				oldGroup.V();
			}	
		}

		#endregion
	}
}
