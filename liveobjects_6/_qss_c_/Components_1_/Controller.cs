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

// #define Controller_TalksXML

// #define DEBUG_Controller

using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Net;

#if Controller_TalksXML
using System.Xml.Serialization;
#else
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace QS._qss_c_.Components_1_
{
	/// <summary>
	/// Summary description for Controller.
	/// </summary>
	public class Controller : System.IDisposable
	{
		public Controller(QS.Fx.Logging.ILogger logger, IPAddress localIPAddress, uint listenerPortNo)
		{
			this.logger = logger;
			tcpDevice = new QS._qss_c_.Devices_1_.TCPCommunicationsDevice(
                "Controller_TCP", localIPAddress, logger, true, (int) listenerPortNo, 2);
			tcpDevice.registerOnReceiveCallback(new QS._qss_c_.Devices_1_.OnReceiveCallback(this.receiveCallback));

			callRequests = new System.Collections.Generic.Dictionary<int, CallRequest>();
		}

		public int PortNumber
		{
			get
			{
				return tcpDevice.PortNumber;
			}
		}

		public void synchronize()
		{
			if (!synchronizationComplete)
			{
				clientConnected.WaitOne();
			}
		}

		public bool synchronize(TimeSpan timeout)
		{
			return synchronizationComplete || clientConnected.WaitOne(timeout, false);
		}

		private QS.Fx.Logging.ILogger logger;
		private QS.Fx.Network.NetworkAddress controlledAddress = null;
		private QS._qss_c_.Devices_1_.TCPCommunicationsDevice tcpDevice;
		private ManualResetEvent clientConnected = new ManualResetEvent(false);
		private bool synchronizationComplete = false;
		private bool alreadyAskedForClientShutdown = false;

		// private AutoResetEvent responseArrived = new AutoResetEvent(false);
		// private object receivedObject;

		private int lastused_seqno = 0;
		private System.Collections.Generic.IDictionary<int, CallRequest> callRequests;

		public object invoke(CallRequest callRequest)
		{
			return this.invoke(callRequest, false, TimeSpan.Zero);
		}

		public object invoke(CallRequest callRequest, TimeSpan timeout)
		{
			return this.invoke(callRequest, true, timeout);
		}

		private object invoke(CallRequest callRequest, bool applyTimeout, TimeSpan timeout)
		{
			if (!synchronizationComplete)
				throw new Exception("client not connected");

			object result = null;

			lock (this)
			{
				lastused_seqno++;
				callRequest.SeqNo = lastused_seqno;
				callRequests[lastused_seqno] = callRequest;
				sendObject(callRequest);				
			}

			if (applyTimeout)
			{
				if (!callRequest.CompletionEvent.WaitOne(timeout, false))
					throw new Exception("timeout expired");
			}
			else
				callRequest.CompletionEvent.WaitOne();
			result = callRequest.ReceivedObject;

			return result;
		}

		private void sendObject(object obj)
		{
			try
			{
				if (!synchronizationComplete)
				{
					throw new Exception(
						"Cannot sent object because the handshaking protocol not completed and address of the remote process is not known.");
				}

                if (obj == null)
                    obj = new System.Object();

				MemoryStream memoryStream = new MemoryStream();
#if Controller_TalksXML
                Base.StringSerializer.SaveObject(memoryStream, obj);
#else
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(memoryStream, obj);
#endif				

				tcpDevice.unicast(controlledAddress.HostIPAddress, controlledAddress.PortNumber, 
					memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
			}
			catch (Exception exc)
			{
				logger.Log(this, exc.ToString());
			}
		}

        public QS._qss_e_.Base_1_.IApplicationController ApplicationController = null;

        private void upcall(object obj)
        {
            try
            {
                if (ApplicationController == null)
                    throw new Exception("Cannot process upcall: application controller not present.");

                QS._core_c_.Components.AttributeSet request = (QS._core_c_.Components.AttributeSet) obj;

#if DEBUG_Controller
                logger.Log(this, "__upcall: " + request.ToString());
#endif

                QS._core_c_.Components.AttributeSet response = ApplicationController.upcall((string)request["operation"], (QS._core_c_.Components.AttributeSet)request["argument"]);
                
                // cannot send response                
            }
            catch (Exception exc)
            {
                logger.Log(this, "__upcall: " + exc.ToString());
            }
        }

        private void receiveCallback(IPAddress senderAddress, int senderPortNo, byte[] buffer, uint bufferSize)
		{
			QS.Fx.Network.NetworkAddress networkAddress = new QS.Fx.Network.NetworkAddress(senderAddress, senderPortNo);			
			if (controlledAddress == null)
				controlledAddress = networkAddress;
			else
				Debug.Assert(controlledAddress.Equals(networkAddress));

			object receivedObject = null;

			MemoryStream memoryStream = new MemoryStream(buffer, 0, (int) bufferSize);
#if Controller_TalksXML
            receivedObject = Base.StringSerializer.LoadObject(memoryStream);
#else
			BinaryFormatter formatter = new BinaryFormatter();
			receivedObject = formatter.Deserialize(memoryStream);
#endif

            if (receivedObject is AsynchronousUpcall)
            {
                this.upcall(((AsynchronousUpcall)receivedObject).Object);
            }
            else if (receivedObject is Response)
            {
				try
				{
					object responseObject = ((Response)receivedObject).Object;

					if (responseObject.GetType().Equals(typeof(System.Object)))
						responseObject = null;

					if (!synchronizationComplete)
					{
						synchronizationComplete = true;
						logger.Log(null, "Controller.Synchronization : " + ((responseObject != null) ? receivedObject.ToString() : "(null)"));

						clientConnected.Set();
					}
					else
					{
						int seqno = ((Response)receivedObject).SeqNo;
						CallRequest callRequest = null;
						lock (this)
						{
							callRequest = callRequests[seqno];
							callRequests.Remove(seqno);
							callRequest.ReceivedObject = responseObject;
							callRequest.CompletionEvent.Set();
						}
					}
				}
				catch (Exception exc)
				{
					logger.Log(this, "Cannot handle the received response: " + QS._core_c_.Helpers.ToString.ObjectRef(receivedObject) + ", " + exc.ToString());
				}
			}
            else
            {
                try
                {
                    logger.Log(this, "Received object of unknown type: " +
                        QS._core_c_.Helpers.ToString.ReceivedObject(new QS.Fx.Network.NetworkAddress(senderAddress, senderPortNo), receivedObject));
                }
                catch (Exception)
                {
                }
            }
        }

		public void shutdownClient()
		{
			if (!alreadyAskedForClientShutdown)
			{
				this.sendObject("shutdown");
				alreadyAskedForClientShutdown = true;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
            try
            {
                shutdownClient();
            }
            catch (Exception)
            {
            }

            try
            {
                foreach (CallRequest request in callRequests.Values)
                {
                    try
                    {
                        request.ReceivedObject = null;
                        request.CompletionEvent.Set();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }

            try
            {
                clientConnected.Set();
            }
            catch (Exception)
            {
            }

            try
            {
                tcpDevice.shutdown();
            }
            catch (Exception)
            {
            }
		}

		#endregion
    }


	#region CallRequest Class

	[Serializable]
	public class CallRequest
	{
		[System.NonSerialized] 
		[System.Xml.Serialization.XmlIgnore]
		public System.Threading.ManualResetEvent CompletionEvent;

		[System.NonSerialized]
		[System.Xml.Serialization.XmlIgnore]
		public object ReceivedObject;

		public CallRequest()
		{
		}

		public CallRequest(System.Reflection.MethodInfo methodToCall, object[] argumentObjects)
		{
            if (methodToCall == null)
                throw new ArgumentException("Method is null.");
            if (argumentObjects == null)
                throw new ArgumentException("Arguments are null.");
            for (int ind = 0; ind < argumentObjects.Length; ind++)
                if (argumentObjects[ind] == null)
                    throw new ArgumentException("Argument #" + (ind + 1).ToString() + " is null.");

//			this.methodToCall = methodToCall;
			this.methodInfoWrapper = new MethodInfoWrapper(methodToCall);

			this.argumentObjects = argumentObjects;

			this.seqno = 0;
			this.CompletionEvent = new ManualResetEvent(false);
		}

//		private System.Reflection.MethodInfo methodToCall;
		private MethodInfoWrapper methodInfoWrapper;

		private object[] argumentObjects;

		private int seqno;

		public int SeqNo
		{
			get { return seqno; }
			set { seqno = value; }
		}

		[System.Xml.Serialization.XmlIgnore]
		public System.Reflection.MethodInfo MethodToCall
		{
			get
			{
				// return methodToCall;
				return methodInfoWrapper.WrappedMethodInfo;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public object[] ArgumentObjects
		{
			get
			{
				return argumentObjects;
			}
		}

		public QS._core_c_.Base.XmlObject XmlMethod
		{
			get
			{
				return new QS._core_c_.Base.XmlObject(methodInfoWrapper);
			}

			set
			{
				methodInfoWrapper = (MethodInfoWrapper) value.Contents;
			}
		}

		[System.Xml.Serialization.XmlElement("argument")]
		public QS._core_c_.Base.XmlObject[] XmlArguments
		{
			get
			{
				QS._core_c_.Base.XmlObject[] xmlObjects = new QS._core_c_.Base.XmlObject[argumentObjects.Length];
				for (uint ind = 0; ind < argumentObjects.Length; ind++)
					xmlObjects[ind] = new QS._core_c_.Base.XmlObject(argumentObjects[ind]);
				return xmlObjects;
			}

			set
			{
				argumentObjects = new object[value.Length];
				for (uint ind = 0; ind < value.Length; ind++)
					argumentObjects[ind] = value[ind].Contents;
			}
		}

		public override string ToString()
		{
			string s = null;
			for (uint ind = 0; ind < argumentObjects.Length; ind++)
				s = ((s != null) ? (s + ", ") : "") + argumentObjects[ind].ToString();

			return "CallRequest(" + 
				methodInfoWrapper.WrappedMethodInfo.ToString() + //methodToCall.ToString()
				"; " + ((s != null) ? s : "-") + ")";
		}
	}

	#endregion

    #region Responses and Upcalls

    [Serializable]
    public class Response
    {
        public Response(object obj, int seqno)
        {
            this.Object = obj;
			this.SeqNo = seqno;
		}

        public Response()
        {
        }

        public object Object;
		public int SeqNo;
    }

    [Serializable]
    public class AsynchronousUpcall
    {
        public AsynchronousUpcall(object obj)
        {
            this.Object = obj;
        }

        public AsynchronousUpcall()
        {
        }

        public object Object;
    }

    #endregion

    public class ControllerClient : System.IDisposable, QS._qss_e_.Base_1_.IApplicationController
    {
		public ControllerClient(QS.Fx.Logging.ILogger logger, IPAddress localIPAddress, QS.Fx.Network.NetworkAddress controllerAddress)
		{
			this.logger = logger;
			this.controllerAddress = controllerAddress;
			tcpDevice = new QS._qss_c_.Devices_1_.TCPCommunicationsDevice("ControllerClient_TCP", localIPAddress, logger, false, 0, 2);
			tcpDevice.registerOnReceiveCallback(new QS._qss_c_.Devices_1_.OnReceiveCallback(this.receiveCallback));
		}

		public void synchronize()
		{
#if DEBUG_Controller
			logger.Log(null, "sending acknowledgement");
#endif

			this.respond(null, "ready");
		}

		private QS.Fx.Logging.ILogger logger;
		private QS._qss_c_.Devices_1_.TCPCommunicationsDevice tcpDevice;
		private QS.Fx.Network.NetworkAddress controllerAddress;
		private AutoResetEvent requestArrived = new AutoResetEvent(false);

		private System.Collections.Generic.Queue<CallRequest> pendingRequests = new System.Collections.Generic.Queue<CallRequest>();
		// private CallRequest callRequest = null;

		#region IDisposable Members

		public void Dispose()
		{
#if DEBUG_Controller
			logger.Log(null, "ControllerClient.Dispose_enter");
#endif

            try
            {
                tcpDevice.shutdown();
            }
            catch (Exception)
            {
            }

#if DEBUG_Controller
			logger.Log(null, "ControllerClient.Dispose_leave");
#endif

            requestArrived.Set();
		}

		#endregion

		private void receiveCallback(IPAddress senderAddress, int senderPortNo, byte[] buffer, uint bufferSize)
		{
#if DEBUG_Controller
			logger.Log(null, "ControllerClient.receiveCallback_enter");
#endif

			try
			{
				MemoryStream memoryStream = new MemoryStream(buffer, 0, (int) bufferSize);
#if Controller_TalksXML
                object receivedObject = Base.StringSerializer.LoadObject(memoryStream);						
#else				
				BinaryFormatter formatter = new BinaryFormatter();
				object receivedObject = formatter.Deserialize(memoryStream);
#endif

                if (receivedObject.GetType().Equals(typeof(System.Object)))
                    receivedObject = null;

				CallRequest callRequest;

				if (receivedObject is CallRequest)
				{
#if DEBUG_Controller
					logger.Log(null, "ControllerClient.receiveCallback : " + receivedObject.ToString());
#endif

					callRequest = (CallRequest) receivedObject;
				}
				else
				{
					logger.Log(null, "terminating");					
					callRequest = null;
				}

				lock (this)
				{
					pendingRequests.Enqueue(callRequest);
					requestArrived.Set();
				}
			}
			catch (Exception exc)
			{
				logger.Log(this, "ReceiveCallback, Exception : " + exc.ToString());

				logger.Log(null, "terminating the client automatically");

				lock (this)
				{
					pendingRequests.Enqueue(null);
					requestArrived.Set();
				}
			}

#if DEBUG_Controller
			logger.Log(null, "ControllerClient.receiveCallback_leave");
#endif
		}

		public CallRequest NextRequest()
		{
			return NextRequest(null);
		}

        public CallRequest NextRequest(System.Threading.WaitHandle handle)
        {
            if (handle == null)
                requestArrived.WaitOne();
            else
                System.Threading.WaitHandle.WaitAny(new WaitHandle[] { requestArrived, handle });

            lock (this)
            {
                CallRequest callRequest = pendingRequests.Dequeue();
                return callRequest;
            }
        }

		public void respond(CallRequest callRequest, object responseObject)
		{
			if (responseObject == null)
				responseObject = new System.Object();

            int_respond(new Response(responseObject, ((callRequest != null) ? callRequest.SeqNo : 0)));
        }

        private void int_respond(object responseObject)
        {
            MemoryStream memoryStream = new MemoryStream();
#if Controller_TalksXML
            Base.StringSerializer.SaveObject(memoryStream, responseObject);
#else
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
			formatter.Serialize(memoryStream, responseObject);
#endif

            lock (this)
            {
                tcpDevice.unicast(controllerAddress.HostIPAddress, controllerAddress.PortNumber,
                    memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            }
        }

        #region IApplicationController Members

        QS._core_c_.Components.AttributeSet QS._qss_e_.Base_1_.IApplicationController.upcall(string operation, QS._core_c_.Components.AttributeSet arguments)
        {
            QS._core_c_.Components.AttributeSet request = new QS._core_c_.Components.AttributeSet(2);
            request["operation"] = operation;
            request["argument"] = arguments;
            int_respond(new AsynchronousUpcall(request));
            return QS._core_c_.Components.AttributeSet.None;
        }

        #endregion
    }
}
