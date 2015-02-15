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

// #define DEBUG_GenericRemoteNode

using System;
using System.Net;

namespace QS._qss_e_.Runtime_
{
	public interface IRemoteNode : IEnvironment, INodeRef, Management_.IManagedComponent
	{
		void connect();
	}

	/// <summary>
	/// Summary description for GenericRemoteNode.
	/// </summary>
	public abstract class GenericRemoteNode : QS.Fx.Inspection.Inspectable, IRemoteNode, QS._qss_e_.Base_1_.IApplicationController
	{	
		public GenericRemoteNode(QS._core_c_.Base.IReadableLogger logger, IPAddress localAddress, uint consolePortNo, uint controllerPortNo, 
			IPAddress nodeAddress, TimeSpan defaultTimeoutOnRemoteOperations)
		{
#if DEBUG_GenericRemoteNode
			logger.Log(this, "SSHControlledNode(" + localAddress.ToString() + ", " + consolePortNo.ToString() + ", " + 
				controllerPortNo.ToString() + ", " + nodeAddress.ToString() + ", " + remotePathToAppLauncher + ")");
#endif

			this.defaultTimeoutOnRemoteOperations = defaultTimeoutOnRemoteOperations;
			this.logger = logger;
			this.localAddress = localAddress;
			this.nodeAddress = nodeAddress;
			tcpDevice = new QS._qss_c_.Devices_1_.TCPCommunicationsDevice(
                "RemoteNode_TCP", localAddress, logger, true, (int) consolePortNo, 2);
			this.consolePortNo = (uint) tcpDevice.PortNumber;
			tcpDevice.registerOnReceiveCallback(new QS._qss_c_.Devices_1_.OnReceiveCallback(this.receiveCallback));
			controller = new QS._qss_c_.Components_1_.Controller(logger, localAddress, controllerPortNo);
			this.controllerPortNo = (uint) controller.PortNumber;

			// this.startupRemoteAgent();

            controller.ApplicationController = this;

			inspectableApps = new QS._qss_e_.Inspection_.DictionaryWrapper1<uint, ApplicationRef>("Active Applications", launched_apps,
				new QS._qss_e_.Inspection_.DictionaryWrapper1<uint, ApplicationRef>.ConversionCallback(ConvertString2UInt32));
		}

		private static uint ConvertString2UInt32(string s)
		{
			return Convert.ToUInt32(s);
		}

		#region IManagedComponent Members

		public object Component
		{
			get
			{
				return this;
			}
		}

		public string Name
		{
			get
			{
				return nodeAddress.ToString();
			}
		}

		public Management_.IManagedComponent[] Subcomponents
		{
			get
			{
				return null;
			}
		}

		public QS._core_c_.Base.IOutputReader Log
		{
			get
			{
				return logger;
			}
		}

		#endregion

		public void connect()
		{
			this.startupRemoteAgent();
			this.synchronize();
		}

		private void synchronize()
		{
#if DEBUG_GenericRemoteNode
			logger.Log(null, "Synchronizing...");
#endif

			controller.synchronize();

#if DEBUG_GenericRemoteNode
			logger.Log(null, "...synchronized.");
#endif
		}

		protected abstract void startupRemoteAgent();
		protected abstract void shutdownRemoteAgent();

		protected QS._core_c_.Base.IReadableLogger logger;
		protected IPAddress localAddress, nodeAddress;
		protected uint consolePortNo, controllerPortNo;
		protected QS._qss_c_.Devices_1_.TCPCommunicationsDevice tcpDevice;
		protected QS._qss_c_.Components_1_.Controller controller;
		protected TimeSpan defaultTimeoutOnRemoteOperations;

		private void receiveCallback(IPAddress senderAddress, int senderPortNo, byte[] buffer, uint bufferSize)
		{
			logger.Log(null, "REMOTE APPLICATION : " + System.Text.Encoding.ASCII.GetString(buffer, 0, (int) bufferSize));
        }

        #region Shutdown

        protected void shutdown()
		{
            try
            {
                lock (this)
                {
                    if (controller != null)
                    {
                        try
                        {
                            controller.shutdownClient();
                        }
                        catch (Exception exc)
                        {
                            logger.Log(this, exc.ToString());
                        }
                    }

                    try
                    {
                        this.shutdownRemoteAgent();
                    }
                    catch (Exception exc)
                    {
                        logger.Log(this, exc.ToString());
                    }

                    if (controller != null)
                    {
                        try
                        {
                            controller.Dispose();
                        }
                        catch (Exception exc)
                        {
                            logger.Log(this, exc.ToString());
                        }

                        controller = null;
                    }

                    if (tcpDevice != null)
                    {
                        try
                        {
                            tcpDevice.shutdown();
                        }
                        catch (Exception exc)
                        {
                            logger.Log(this, exc.ToString());
                        }

                        tcpDevice = null;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

		#region IDisposable Members

		public void Dispose()
		{
			this.shutdown();
		}

		#endregion

		#region ApplicationRef

		private class ApplicationRef : QS._qss_e_.Inspection_.RemotingProxy, IApplicationRef
		{
			public ApplicationRef(GenericRemoteNode encapsulatingRemoteNode, uint applicationSeqNo, ulong appid)
			{
				this.encapsulatingRemoteNode = encapsulatingRemoteNode;
				this.applicationSeqNo = applicationSeqNo;
                this.appid = appid;
			}

			#region Overrides for Remote Proxy

			protected override object MakeCall(QS._core_c_.Components.AttributeSet arguments)
			{
				System.Reflection.MethodInfo methodToCall = typeof(RemoteAgent).GetMethod("inspectionProxyCall",	
					new System.Type[] { typeof(QS._core_c_.Components.AttributeSet) });
				if (methodToCall == null)
					throw new Exception("__MakeCall: Cannot bind to method");
				arguments["applicationSeqNo"] = applicationSeqNo;
				QS._qss_c_.Components_1_.CallRequest request = 
					new QS._qss_c_.Components_1_.CallRequest(methodToCall, new object[] { arguments });

				return encapsulatingRemoteNode.controller.invoke(request);
			}

			#endregion

			private GenericRemoteNode encapsulatingRemoteNode;
			private uint applicationSeqNo;
            private Base_1_.IApplicationController applicationController = null;
            private ulong appid;

            public string Address
            {
                get { return encapsulatingRemoteNode.nodeAddress.ToString() + "!" + appid.ToString(); }
            }

            public ulong AppID
            {
                get { return appid; }
            }

            private object invoke(System.Reflection.MethodInfo methodInfo, object[] arguments, 
				bool withTimeout, TimeSpan timeout)
			{
				QS._core_c_.Components.AttributeSet invokeArgs = new QS._core_c_.Components.AttributeSet(3);
				invokeArgs["applicationSeqNo"] = applicationSeqNo;
				invokeArgs["methodInfo"] = methodInfo;
				invokeArgs["arguments"] = arguments;
				
				QS._qss_c_.Components_1_.CallRequest request = new QS._qss_c_.Components_1_.CallRequest(
					typeof(RemoteAgent).GetMethod("invoke", new System.Type[] { typeof(QS._core_c_.Components.AttributeSet) }), 
					new object[] { invokeArgs });

				object result = withTimeout ? encapsulatingRemoteNode.controller.invoke(request, timeout) :
					encapsulatingRemoteNode.controller.invoke(request);				
				// encapsulatingRemoteNode.defaultTimeoutOnRemoteOperations);							

				return result;			
			}

			#region IApplicationRef Members

            public Base_1_.IApplicationController Controller
            {
                get { return applicationController; }
                set { applicationController = value; }
            }

            public object invoke(System.Reflection.MethodInfo methodInfo, object[] arguments, TimeSpan timeout)
			{
				return this.invoke(methodInfo, arguments, true, timeout);
			}

			public object invoke(System.Reflection.MethodInfo methodInfo, object[] arguments)
			{
				return this.invoke(methodInfo, arguments, false, TimeSpan.Zero);
			}

			public IAsyncResult BeginInvoke(
				System.Reflection.MethodInfo methodInfo, object[] arguments, AsyncCallback callback, object asynchronousState)
			{
				return new QS._qss_c_.Base3_.AsynchronousCall2<object>(new QS._qss_e_.Runtime_.InvokeMethodCallback(invoke),
					new object[] { methodInfo, arguments }, callback, asynchronousState, true);
			}

			public object EndInvoke(IAsyncResult asynchronousResult)
			{
				return ((QS._qss_c_.Base3_.AsynchronousCall2<object>)asynchronousResult).OperationResult;
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				QS._core_c_.Components.AttributeSet removeArgs = new QS._core_c_.Components.AttributeSet(1);
				removeArgs["applicationSeqNo"] = applicationSeqNo;
				
				encapsulatingRemoteNode.controller.invoke(new QS._qss_c_.Components_1_.CallRequest(
					typeof(RemoteAgent).GetMethod("remove", new System.Type[] { typeof(QS._core_c_.Components.AttributeSet) }), new object[] { removeArgs }));
			}

			#endregion
		}

		#endregion

		#region IEnvironment Members

		public INodeRef[] Nodes
		{
			get
			{
				return new INodeRef[] { this };
			}
		}

		public QS.Fx.Clock.IAlarmClock AlarmClock
		{
			get
			{
				throw new Exception("not implemented!");
			}
		}

		public QS.Fx.Clock.IClock Clock
		{
			get
			{
				throw new Exception("not implemented!");
			}
		}

		#endregion

		#region INodeRef Members

        public void ReleaseResources()
        {
            // TODO: Should implement............................................................
        }

		public IAsyncResult BeginLaunch(string fullyQualifiedClassName, QS._core_c_.Components.AttributeSet arguments,
			AsyncCallback callback, object asynchronousState)
		{
			return new QS._qss_c_.Base3_.AsynchronousCall2<IApplicationRef>(new LaunchApplicationAsyncCallback(launch),
				new object[] { fullyQualifiedClassName, arguments }, callback, asynchronousState, true);
		}

		public IApplicationRef EndLaunch(IAsyncResult asynchronousResult)
		{
			return ((QS._qss_c_.Base3_.AsynchronousCall2<IApplicationRef>)asynchronousResult).OperationResult;
		}

		public IApplicationRef launch(string fullyQualifiedClassName, QS._core_c_.Components.AttributeSet arguments)
		{
            try
            {
                QS._core_c_.Components.AttributeSet launchArgs = new QS._core_c_.Components.AttributeSet(2);
                launchArgs["fullyQualifiedClassName"] = fullyQualifiedClassName;
                launchArgs["arguments"] = arguments;

                object responseObject = controller.invoke(new QS._qss_c_.Components_1_.CallRequest(
                    typeof(RemoteAgent).GetMethod("launch", new System.Type[] { typeof(QS._core_c_.Components.AttributeSet) }),
                    new object[] { launchArgs }), this.defaultTimeoutOnRemoteOperations);

#if DEBUG_GenericRemoteNode
			logger.Log(this, "...responseObject = " + 
				((responseObject != null) ? (responseObject.ToString() + ":" + responseObject.GetType().ToString()) : "null"));
#endif

                if (!(responseObject is System.UInt32))
                    throw new Exception("Controller received a wrong object type in response, expected System.UInt32, received " +
                        ((responseObject != null) ? (responseObject.GetType().FullName + " : \"" + responseObject.ToString() + "\"") : "(null)"));

                uint applicationSeqNo = (System.UInt32)responseObject;

                ApplicationRef appRef = new ApplicationRef(this, applicationSeqNo, 0);
                launched_apps[applicationSeqNo] = appRef;
                return appRef;
            }
            catch (Exception exc)
            {
                throw new Exception("Could not launch class \"" + fullyQualifiedClassName + "\".", exc);
            }
        }

		public IApplicationRef launch(System.Reflection.ConstructorInfo constructorInfo, object[] arguments)
		{
			QS._core_c_.Components.AttributeSet launchArgs = new QS._core_c_.Components.AttributeSet(2);
			launchArgs["constructorInfo"] = constructorInfo;
			launchArgs["arguments"] = arguments;

			object responseObject = controller.invoke(new QS._qss_c_.Components_1_.CallRequest(
				typeof(RemoteAgent).GetMethod("launch", new System.Type[] { typeof(QS._core_c_.Components.AttributeSet) }), 
				new object[] { launchArgs }), this.defaultTimeoutOnRemoteOperations);

#if DEBUG_GenericRemoteNode
			logger.Log(this, "...responseObject = " + 
				((responseObject != null) ? (responseObject.ToString() + ":" + responseObject.GetType().ToString()) : "null"));
#endif

			uint applicationSeqNo = (System.UInt32) responseObject;
			
			ApplicationRef appRef = new ApplicationRef(this, applicationSeqNo, 0);
            launched_apps[applicationSeqNo] = appRef;
            return appRef;
		}

        private System.Collections.Generic.IDictionary<uint, ApplicationRef> launched_apps = 
            new System.Collections.Generic.Dictionary<uint, ApplicationRef>();
		[QS.Fx.Base.Inspectable("Active Applications", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private QS._qss_e_.Inspection_.DictionaryWrapper1<uint, ApplicationRef> inspectableApps;

		public System.Net.IPAddress[] NICs
		{
			get
			{
				return new IPAddress[] { nodeAddress }; // for now this is succifient, whatever...
			}
		}

		//		public IApplicationRef[] Apps
		//		{
		//			get
		//			{
		//				return null;
		//			}
		//		}

		#endregion

        #region IApplicationController Members

        QS._core_c_.Components.AttributeSet QS._qss_e_.Base_1_.IApplicationController.upcall(string operation, QS._core_c_.Components.AttributeSet arguments)
        {
            uint appseqno = (uint) arguments["application"];

            launched_apps[appseqno].Controller.upcall(
                (string)arguments["operation"], (QS._core_c_.Components.AttributeSet)arguments["arguments"]);

            return QS._core_c_.Components.AttributeSet.None;
        }

        #endregion
	}
}
