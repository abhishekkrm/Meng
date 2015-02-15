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

// #define DEBUG_RemoteAgent

using System;

namespace QS._qss_e_.Runtime_
{
	/// <summary>
	/// Summary description for RemoteAgent.
	/// </summary>
	public class RemoteAgent : Base_1_.ControlledApplication
	{
        public RemoteAgent(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
		{
			this.platform = platform;
			this.localNode = new LocalNode(platform, false);
			this.applications = new System.Collections.Generic.Dictionary<uint, IApplicationRef>();

#if DEBUG_RemoteAgent
			platform.Logger.Log(this, "RemoteAgent Started.");
#endif

            inspectableWrapper_Applications = new QS._qss_e_.Inspection_.DictionaryWrapper1<uint, IApplicationRef>(
                "Applications", this.applications, 
                new QS._qss_e_.Inspection_.DictionaryWrapper1<uint, IApplicationRef>.ConversionCallback(
                    delegate(string s) { return Convert.ToUInt32(s); }));

            inspectionAgent = new QS._qss_c_.Connections_.Inspector.Agent(this);

            inspectionServer = new QS._qss_c_.Connections_.TCPServer(
                platform, new QS.Fx.Network.NetworkAddress(QS._qss_c_.Connections_.Inspector.MulticastAddress),
                new QS._qss_c_.Connections_.CreateCallback(
                    delegate(QS.Fx.Network.NetworkAddress address, QS._qss_c_.Connections_.IAsynchronousRef asynchronousRef)
                    {
                        return new QS._qss_c_.Connections_.ServiceObject(platform.Logger, inspectionAgent);
                    }));
		}

        private QS._qss_c_.Connections_.TCPServer inspectionServer;
        private QS._qss_c_.Connections_.Inspector.Agent inspectionAgent;

        private QS._qss_c_.Platform_.IPlatform platform;
		private LocalNode localNode;
		private System.Collections.Generic.IDictionary<uint, IApplicationRef> applications;
		private uint firstUnusedApplicationSeqNo = 1;

		public object inspectionProxyCall(QS._core_c_.Components.AttributeSet args)
		{
			IApplicationRef applicationRef = (IApplicationRef)applications[(uint)args["applicationSeqNo"]];
			return QS._qss_e_.Inspection_.RemotingProxy.DispatchCall(applicationRef, args);
		}

        [QS.Fx.Base.Inspectable("Applications")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<uint, IApplicationRef> inspectableWrapper_Applications;

		#region IControlledApplication Members

		public class ProxyController : Base_1_.IApplicationController
		{
            public ProxyController(RemoteAgent owner, IApplicationRef controlledApplication, uint appseqno)
            {
                this.owner = owner;
                this.controlledApplication = controlledApplication;
                this.appseqno = appseqno;
            }

            private RemoteAgent owner;
            private IApplicationRef controlledApplication;
            private uint appseqno;

            #region IApplicationController Members

            QS._core_c_.Components.AttributeSet QS._qss_e_.Base_1_.IApplicationController.upcall(string operation, QS._core_c_.Components.AttributeSet arguments)
            {
                if (owner.applicationController != null)
                {
                    QS._core_c_.Components.AttributeSet myargs = new QS._core_c_.Components.AttributeSet(3);
                    myargs["application"] = appseqno;
                    myargs["operation"] = operation;
                    myargs["arguments"] = arguments;

#if DEBUG_RemoteAgent
                    owner.platform.Logger.Log(this, "__upcall: " + operation + "(" + arguments.ToString() + ")");
#endif

                    owner.applicationController.upcall("process_upcall", myargs);

                    return QS._core_c_.Components.AttributeSet.None;
                }
                else
                    throw new Exception("Application controller not present.");
            }

            #endregion
        }

        public delegate QS._core_c_.Components.AttributeSet ProxyCallback(object referenceObject, QS._core_c_.Components.AttributeSet arguments);

        #endregion

        public object launch(QS._core_c_.Components.AttributeSet args)
		{
#if DEBUG_RemoteAgent
			platform.Logger.Log(null, "RemoteAgent : launch_enter");
#endif

			IApplicationRef appref = null;

			if (args.contains("constructorInfo"))
			{
				System.Reflection.ConstructorInfo constructorInfo = (System.Reflection.ConstructorInfo) args["constructorInfo"];
				object[] arguments = (object[]) args["arguments"];

				appref = localNode.launch(constructorInfo, arguments);
			}
			else
			{
				string fullyQualifiedClassName = (string) args["fullyQualifiedClassName"];
				QS._core_c_.Components.AttributeSet arguments = (QS._core_c_.Components.AttributeSet) args["arguments"];

				appref = localNode.launch(fullyQualifiedClassName, arguments);
			}

			uint applicationSeqNo = firstUnusedApplicationSeqNo++;

			applications[applicationSeqNo] = appref;

            try
            {
                appref.Controller = new ProxyController(this, appref, applicationSeqNo);
            }
            catch (Exception exc)
            {
                platform.Logger.Log(this, "Cannot attach controller.\n" + exc.ToString());
            }

            return applicationSeqNo;
		}

		public object invoke(QS._core_c_.Components.AttributeSet args)
		{
			uint applicationSeqNo = (uint) args["applicationSeqNo"];
			System.Reflection.MethodInfo methodInfo = (System.Reflection.MethodInfo) args["methodInfo"];
			object[] arguments = (object[]) args["arguments"];

			IApplicationRef appRef = (IApplicationRef) applications[applicationSeqNo];

			object result = appRef.invoke(methodInfo, arguments);

			return result;
		}

		public bool remove(QS._core_c_.Components.AttributeSet args)
		{
			try
			{
				uint applicationSeqNo = (uint) args["applicationSeqNo"];
				IApplicationRef appRef = (IApplicationRef) applications[applicationSeqNo];
                applications.Remove(applicationSeqNo);
				appRef.Dispose();
				return true;
			}
			catch (Exception exc)
			{
				platform.Logger.Log(null, exc.ToString());
			}

			return false;
		}

/*
        public void inspect(CMS.Components.AttributeSet args)
        {
            uint applicationSeqNo = (uint)args["applicationSeqNo"];
            IApplicationRef appRef = (IApplicationRef)applications[applicationSeqNo];
            QS.GUI.
            System.Reflection.MethodInfo methodInfo = (System.Reflection.MethodInfo)args["methodInfo"];
            object[] arguments = (object[])args["arguments"];

            IApplicationRef appRef = (IApplicationRef)applications[applicationSeqNo];

            object result = appRef.invoke(methodInfo, arguments);

            return result;
        }
*/

		#region IDisposable Members

		public override void Dispose()
		{
			localNode.Dispose();
		}

		#endregion
    }
}
