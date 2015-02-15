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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Simulations_2_
{
	public class SimulatedNode : QS.Fx.Inspection.Inspectable, ISimulatedNode
	{
		public SimulatedNode(
            QS.Fx.Logging.IEventLogger eventLogger, QS.Fx.Clock.IClock simulatedClock, QS.Fx.Clock.IAlarmClock simulatedAlarmClock, 
			Virtualization_.INetwork[] networks, int incomingQueueSize) 
            : this(new SimulatedPlatform(eventLogger, 
                new SimulatedCPU(eventLogger, simulatedClock, simulatedAlarmClock), networks, incomingQueueSize))
		{
		}

		public SimulatedNode(ISimulatedPlatform platform)
		{
			this.simulatedCPU = platform.SimulatedCPU;
			simulatedCPU.Name = platform.NICs[0].ToString();

			this.platform = platform;
			logger = new QS._qss_c_.Base3_.Logger(simulatedCPU, true, null);

			applications = new System.Collections.Generic.SortedDictionary<ulong, ApplicationRef>();
			inspectableWrapper_applications = new QS._qss_e_.Inspection_.DictionaryWrapper1<ulong, ApplicationRef>(
				"Applications", applications, new QS._qss_e_.Inspection_.DictionaryWrapper1<ulong, ApplicationRef>.ConversionCallback(
				Convert.ToUInt64));
		}

		[QS.Fx.Base.Inspectable("Platform", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private ISimulatedPlatform platform;
		[QS.Fx.Base.Inspectable("CPU", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private ISimulatedCPU simulatedCPU;
		private ulong lastused_appid;
		private System.Collections.Generic.IDictionary<ulong, ApplicationRef> applications;
		[QS.Fx.Base.Inspectable("Log", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private Base3_.Logger logger;

		[QS.Fx.Base.Inspectable("Applications", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private QS._qss_e_.Inspection_.DictionaryWrapper1<ulong, ApplicationRef> inspectableWrapper_applications;

		#region Class ApplicationRef

		private class ApplicationRef : QS.Fx.Inspection.Inspectable, QS._qss_e_.Runtime_.IApplicationRef
		{
			public ApplicationRef(SimulatedNode owner, ulong appid, System.IDisposable applicationObject)
			{
				this.owner = owner;
				this.appid = appid;
				this.applicationObject = applicationObject;
			}

			private SimulatedNode owner;
			[QS.Fx.Base.Inspectable("Controlled Object", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private System.IDisposable applicationObject;
			private ulong appid;

#pragma warning disable 618
            public string Address
            {
                get { return owner.platform.NICs[0].Address.ToString() + "!" + appid.ToString(); }
            }
#pragma warning restore 618

            #region Class CallRequest

            private class CallRequest
			{
				public CallRequest(System.Reflection.MethodInfo methodInfo, object[] arguments)
				{
					this.MethodInfo = methodInfo;
					this.Arguments = arguments;
				}

				public System.Reflection.MethodInfo MethodInfo;
				public object[] Arguments;
			}

			#endregion

			#region IApplicationRef Members

			ulong QS._qss_e_.Runtime_.IApplicationRef.AppID
			{
				get { return appid; }
			}

			public IAsyncResult BeginInvoke(System.Reflection.MethodInfo methodInfo, object[] arguments,
				AsyncCallback callback, object asynchronousState)
			{
				IAsyncResult asyncResult = new QS._qss_c_.Base3_.AsynchronousCall2<object>(
					new InvokeMethodCallback(invokeMethod), new object[] { new CallRequest(methodInfo, arguments) },
					callback, asynchronousState, false);

                ((QS.Fx.Scheduling.IScheduler) owner.simulatedCPU).BeginExecute(new AsyncCallback(invokeMethodCallback), asyncResult);

				return asyncResult;
			}

			private void invokeMethodCallback(IAsyncResult asynchronousResult)
			{
				try
				{
					((QS._qss_c_.Base3_.AsynchronousCall2<object>) asynchronousResult.AsyncState).Invoke();
				}
				catch (Exception exc)
				{
					((QS.Fx.Logging.ILogger) owner.logger).Log(this, "__invokeMethodCallback : " + exc.ToString());
				}
			}

			private delegate object InvokeMethodCallback(CallRequest request);
			private object invokeMethod(CallRequest request)
			{
				return request.MethodInfo.Invoke(applicationObject, request.Arguments);
			}

			public object EndInvoke(IAsyncResult asynchronousResult)
			{
				return ((QS._qss_c_.Base3_.AsynchronousCall2<object>)asynchronousResult).OperationResult;
			}

			object QS._qss_e_.Runtime_.IApplicationRef.invoke(System.Reflection.MethodInfo methodInfo, object[] arguments)
			{
				IAsyncResult asynchronousResult = BeginInvoke(methodInfo, arguments, null, null);
				asynchronousResult.AsyncWaitHandle.WaitOne();
				return EndInvoke(asynchronousResult);
			}

			object QS._qss_e_.Runtime_.IApplicationRef.invoke(System.Reflection.MethodInfo methodInfo, object[] arguments, TimeSpan timeout)
			{
				throw new NotSupportedException();
/*
				if (timeout.Equals(TimeSpan.MaxValue))
				else
				{
					if (!asynchronousResult.AsyncWaitHandle.WaitOne(timeout, false))
						throw new Exception("Call timeout expired, method \"" + methodInfo.Name + "\" did not complete within " + timeout.ToString() + ".");
				}

				if (request.Result is System.Exception)
					throw new Exception("Could not invoke method.", (System.Exception)request.Result);
				else
					return request.Result;
*/
			}

			public QS._qss_e_.Base_1_.IApplicationController Controller
			{
				get { return (applicationObject as QS._qss_e_.Base_1_.IControlledApplication).Controller; }
				set
				{
					QS._qss_e_.Base_1_.IControlledApplication controlledApplication = applicationObject as QS._qss_e_.Base_1_.IControlledApplication;
					if (controlledApplication != null)
						controlledApplication.Controller = value;
					else
						throw new Exception("Cannot attach controller, the controlled application is not of type \"QS.TMS.Base.IControlledApplication\".");
				}
			}

			#endregion

			#region IDisposable Members

			void IDisposable.Dispose()
			{
				lock (owner)
				{
					owner.applications.Remove(appid);
				}

                try
                {
                    applicationObject.Dispose();
                }
                catch (Exception)
                {
                }
			}

			#endregion
		}

		#endregion

		#region Class LaunchRequest

		private class LaunchRequest
		{
			public LaunchRequest(System.Reflection.ConstructorInfo constructorInfo, object[] arguments)
			{
				this.ConstructorInfo = constructorInfo;
				this.Arguments = arguments;
			}

			public System.Reflection.ConstructorInfo ConstructorInfo;
			public object[] Arguments;
		}

		#endregion

		#region INodeRef Members

        public void ReleaseResources()
        {
            platform.ReleaseResources();
        }

		private static System.Reflection.ConstructorInfo GetConstructor(string fullyQualifiedClassName)
		{
			System.Reflection.ConstructorInfo constructorInfo = 
				System.Type.GetType(fullyQualifiedClassName).GetConstructor(
					new System.Type[] { typeof(QS.Fx.Platform.IPlatform), typeof(QS._core_c_.Components.AttributeSet) });
			if (constructorInfo == null)
				throw new Exception("Class " + fullyQualifiedClassName + " does not have the appropriate constructor.");
			return constructorInfo;
		}

		private static void CheckConstructor(System.Reflection.ConstructorInfo constructorInfo)
		{
			if (constructorInfo == null)
				throw new ArgumentException("Constructor is NULL.");
			if (!typeof(IDisposable).IsAssignableFrom(constructorInfo.ReflectedType))
				throw new ArgumentException("The requested type does not implement System.IDisposable.");
			if (constructorInfo.GetParameters().Length < 1 || !constructorInfo.GetParameters()[0].ParameterType.Equals(typeof(QS.Fx.Platform.IPlatform)))
				throw new ArgumentException("Constructor does not take platform as the first argument.");
		}

		public IAsyncResult BeginLaunch(string fullyQualifiedClassName, QS._core_c_.Components.AttributeSet arguments,
			AsyncCallback callback, object asynchronousState)
		{
			return BeginLaunch(GetConstructor(fullyQualifiedClassName), new object[] { arguments }, callback, asynchronousState);
		}

		public IAsyncResult BeginLaunch(System.Reflection.ConstructorInfo constructorInfo, object[] arguments, 
			AsyncCallback callback, object asynchronousState)
		{
			lock (this)
			{
				CheckConstructor(constructorInfo);

				object[] completeArguments = new object[arguments.Length + 1];
				completeArguments[0] = platform;
				arguments.CopyTo(completeArguments, 1);

				IAsyncResult asyncResult = new QS._qss_c_.Base3_.AsynchronousCall2<QS._qss_e_.Runtime_.IApplicationRef>(
					new LaunchApplicationCallback(launchApplication),
					new object[] { new LaunchRequest(constructorInfo, completeArguments) },
					callback, asynchronousState, false);

                ((QS.Fx.Scheduling.IScheduler)simulatedCPU).BeginExecute(new AsyncCallback(launchApplicationCallback), asyncResult);

				return asyncResult;
			}
		}

		private void launchApplicationCallback(IAsyncResult asynchronousResult)
		{
			try
			{
				((QS._qss_c_.Base3_.AsynchronousCall2<QS._qss_e_.Runtime_.IApplicationRef>)asynchronousResult.AsyncState).Invoke();
			}
			catch (Exception exc)
			{
				((QS.Fx.Logging.ILogger)logger).Log(this, "__launchApplicationCallback : " + exc.ToString());
			}
		}

		private delegate QS._qss_e_.Runtime_.IApplicationRef LaunchApplicationCallback(LaunchRequest request);
		private QS._qss_e_.Runtime_.IApplicationRef launchApplication(LaunchRequest request)
		{
			object obj = request.ConstructorInfo.Invoke(request.Arguments);
			if (obj == null)
				throw new Exception("Could not create application object.");

			ApplicationRef applicationRef;
			lock (this)
			{
				ulong appid = ++lastused_appid;
				applicationRef = new ApplicationRef(this, appid, (IDisposable) obj);
				applications[appid] = applicationRef;
			}

			return applicationRef;
		}

		public QS._qss_e_.Runtime_.IApplicationRef EndLaunch(IAsyncResult asynchronousResult)
		{
			return ((QS._qss_c_.Base3_.AsynchronousCall2<QS._qss_e_.Runtime_.IApplicationRef>)asynchronousResult).OperationResult;
		}

		QS._qss_e_.Runtime_.IApplicationRef QS._qss_e_.Runtime_.INodeRef.launch(string fullyQualifiedClassName, QS._core_c_.Components.AttributeSet arguments)
		{
			// return ((QS.TMS.Runtime.INodeRef)this).launch(constructorInfo, new object[] { arguments });
			return ((QS._qss_e_.Runtime_.INodeRef) this).launch(GetConstructor(fullyQualifiedClassName), new object[] { arguments });
		}

		QS._qss_e_.Runtime_.IApplicationRef QS._qss_e_.Runtime_.INodeRef.launch(System.Reflection.ConstructorInfo constructorInfo, object[] arguments)
		{
			IAsyncResult asynchronousResult = BeginLaunch(constructorInfo, arguments, null, null);
			asynchronousResult.AsyncWaitHandle.WaitOne();
			return EndLaunch(asynchronousResult);
		}

		System.Net.IPAddress[] QS._qss_e_.Runtime_.INodeRef.NICs
		{
			get { return platform.NICs; }
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			platform.Dispose();
		}

		#endregion

		#region IManagedComponent Members

		string QS._qss_e_.Management_.IManagedComponent.Name
		{
			get { return "Node " + platform.NICs[0].ToString(); }
		}

		QS._qss_e_.Management_.IManagedComponent[] QS._qss_e_.Management_.IManagedComponent.Subcomponents
		{
			get { return new QS._qss_e_.Management_.IManagedComponent[] { (QS._qss_e_.Management_.IManagedComponent) platform }; }
		}

		QS._core_c_.Base.IOutputReader QS._qss_e_.Management_.IManagedComponent.Log
		{
			get { return logger; }
		}

		object QS._qss_e_.Management_.IManagedComponent.Component
		{
			get { return this; }
		}

		#endregion

		#region ISimulatedNode Members

		ISimulatedPlatform ISimulatedNode.SimulatedPlatform
		{
			get { return platform; }
		}

		#endregion

		public override string ToString()
		{
			return platform.NICs[0].ToString();
		}
	}
}
