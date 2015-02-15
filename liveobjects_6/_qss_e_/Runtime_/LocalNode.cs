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

// #define DEBUG_LocalNode

using System;

namespace QS._qss_e_.Runtime_
{
	public delegate IApplicationRef LaunchApplicationAsyncCallback(
		string fullyQualifiedClassName, QS._core_c_.Components.AttributeSet arguments);

	public delegate object InvokeMethodCallback(System.Reflection.MethodInfo methodInfo, object[] arguments);

	/// <summary>
	/// Summary description for PhysicalEnvironment.
	/// </summary>
	public class LocalNode : QS.Fx.Inspection.Inspectable, IEnvironment, IDisposable, INodeRef, Management_.IManagedComponent
	{
//		public LocalNode(CMS.Platform.IPlatform platform) : this(platform, false)
//		{
//		}

		private const uint defaultAnticipatedNumberOfApplications = 10;

        public LocalNode(QS._qss_c_.Platform_.IPlatform platform, bool responsibleForDisposingOfPlatform)
		{
			this.responsibleForDisposingOfPlatform = responsibleForDisposingOfPlatform;
			this.platform = platform;
			this.applications = new QS._qss_c_.Collections_1_.LinkableHashSet(defaultAnticipatedNumberOfApplications);

			applicationCollectionInspectableProxy = new AppCollectionInspectableProxy(this);
		}

		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
        private QS._qss_c_.Platform_.IPlatform platform;
		private bool responsibleForDisposingOfPlatform;
		private QS._qss_c_.Collections_1_.ILinkableHashSet applications;
		private bool indirectApplicationCalls = false;

		public bool IndirectApplicationCalls
		{
			get { return indirectApplicationCalls; }
			set { indirectApplicationCalls = value; }
		}
		
		#region Inspection into Applications

		[QS.Fx.Base.Inspectable("Applications", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private AppCollectionInspectableProxy applicationCollectionInspectableProxy;

		private class AppCollectionInspectableProxy : QS.Fx.Inspection.IAttributeCollection
		{
			public AppCollectionInspectableProxy(LocalNode node)
			{
				this.node = node;
			}

			LocalNode node;

			#region IAttributeCollection Members

			System.Collections.Generic.IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
			{
				get 
				{
					foreach (ApplicationRef applicationRef in node.applications.Elements)
						yield return applicationRef.AppID.ToString();
				}
			}

			QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
			{
				get { return node.applications.lookup(Convert.ToUInt64(attributeName)) as QS.Fx.Inspection.IAttribute; }
			}

			#endregion

			#region IAttribute Members

			string QS.Fx.Inspection.IAttribute.Name
			{
				get { return "Applications"; }
			}

			QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
			{
				get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
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

		#region IDisposable Members

		public void Dispose()
		{
            try
            {
                lock (this)
                {
                    if (responsibleForDisposingOfPlatform)
                        platform.Dispose();
                    platform = null;
                }

                foreach (ApplicationRef appRef in applications.Elements)
                {
                    try
                    {
                        appRef.Dispose();
                    }
                    catch (Exception exc)
                    {
                        platform.Logger.Log(this, exc.ToString());
                    }
                }
            }
            catch (Exception exc)
            {
                platform.Logger.Log(this, exc.ToString());
            }
        }

		#endregion

		#region Class ApplicationRef

		private class ApplicationRef : QS._qss_c_.Collections_1_.GenericLinkable, IApplicationRef, QS.Fx.Inspection.IScalarAttribute
		{
			public ApplicationRef(LocalNode encapsulatingEnvironment, IDisposable applicationObject, ulong appid)
			{
				if (applicationObject == null)
					throw new ArgumentException("Application object is NULL.");

				this.appid = appid;
				this.encapsulatingEnvironment = encapsulatingEnvironment;
				this.applicationObject = applicationObject;
			}

			private LocalNode encapsulatingEnvironment;
			private IDisposable applicationObject;
            // private Base.IApplicationController applicationController = null;
            private ulong appid;

#pragma warning disable 618
            public string Address
            {
                get { return encapsulatingEnvironment.NICs[0].Address.ToString() + "!" + appid.ToString(); }
            }
#pragma warning restore 618

            #region IInspectable Members

            QS.Fx.Inspection.IAttributeCollection QS.Fx.Inspection.IInspectable.Attributes
			{
				get 
				{ 
					return (applicationObject is QS.Fx.Inspection.IInspectable) ? ((applicationObject as QS.Fx.Inspection.IInspectable).Attributes) : QS.Fx.Inspection.AttributeCollection.NoAttributes;
				}
			}

			#endregion

			public override int GetHashCode()
			{
				return applicationObject.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				return (obj != null) ? applicationObject.Equals(obj) : false;
			}

			#region IApplicationRef Members

            public ulong AppID
            {
                get { return appid; }
            }

            public Base_1_.IApplicationController Controller
            {
                get { return (applicationObject as Base_1_.IControlledApplication).Controller; }
                set 
                {
                    Base_1_.IControlledApplication controlledApplication = applicationObject as Base_1_.IControlledApplication;
                    if (controlledApplication != null)
                        controlledApplication.Controller = value;
                    else
                        throw new Exception("Cannot attach controller, the controlled application is not of type \"QS.TMS.Base.IControlledApplication\".");
                }
            }

//			public object invoke(string method, System.Type[] argumentTypes, object[] arguments)
//			{
//				return this.invoke(applicationObject.GetType().GetMethod(method, argumentTypes), arguments));
//			}

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

			public object invoke(System.Reflection.MethodInfo methodInfo, object[] arguments, TimeSpan timeout)
			{
				return this.invoke(methodInfo, arguments);
			}
			
			public object invoke(System.Reflection.MethodInfo methodInfo, object[] arguments)
			{
#if DEBUG_LocalNode
				encapsulatingEnvironment.platform.Logger.Log(this, "invoke_enter : " + methodInfo.ToString() +
					" for type " + methodInfo.ReflectedType.ToString() + " for object " + applicationObject.GetType().ToString());
#endif

                if (methodInfo == null)
                    throw new Exception("Method is NULL!");

				if (encapsulatingEnvironment.indirectApplicationCalls)
				{
//					encapsulatingEnvironment.platform.AlarmClock.Schedule(0,
//						new QS.Fx.QS.Fx.Clock.AlarmCallback(indirectCall), 

					return null;



				}
				else
					return methodInfo.Invoke(applicationObject, arguments);
			}



			#endregion

			private void disposeOfApplicationObject()
			{
				lock (this)
				{
					if (applicationObject != null)
						applicationObject.Dispose();
					// applicationObject = null;
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				lock (encapsulatingEnvironment)
				{
					try
					{
						encapsulatingEnvironment.applications.remove(this);
					}
					catch (Exception)
					{
					}
				}

				this.disposeOfApplicationObject();
			}

			#endregion

			#region IScalarAttribute Members

			object QS.Fx.Inspection.IScalarAttribute.Value
			{
				get { return applicationObject; }
			}

			#endregion

			#region IAttribute Members

			string QS.Fx.Inspection.IAttribute.Name
			{
				get { return this.AppID.ToString(); }
			}

			QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
			{
				get { return QS.Fx.Inspection.AttributeClass.SCALAR; }
			}

			#endregion
		}

		#endregion

		#region INodeRef Members

        public void ReleaseResources()
        {
            platform.ReleaseResources();
        }

		public System.Net.IPAddress[] NICs
		{
			get
			{
				return platform.NICs;
			}
		}

		public IApplicationRef[] Apps
		{
			get
			{
				IApplicationRef[] result = null;
				lock (this)
				{
					result = (IApplicationRef[]) applications.ToArray(typeof(IApplicationRef));
				}

				return result;
			}
		}

//		public IApplicationRef launch(System.Type objectType, System.Type[] argumentTypes, object[] arguments)
//		{
//			Type[] completeArgumentTypes = new Type[argumentTypes.Length + 1];
//			argumentTypes.CopyTo(completeArgumentTypes, 1);
//			completeArgumentTypes[0] = typeof(Platform.IPlatform);
//
//			return this.launch(objectType.GetConstructor(completeArgumentTypes), arguments);
//		}

		public IAsyncResult BeginLaunch(string fullyQualifiedClassName, QS._core_c_.Components.AttributeSet arguments,
			AsyncCallback callback, object asynchronousState)
		{
			return new QS._qss_c_.Base3_.AsynchronousCall2<IApplicationRef>(
				new QS._qss_e_.Runtime_.LaunchApplicationAsyncCallback(launch), 
				new object[] { fullyQualifiedClassName, arguments }, callback, asynchronousState, true);
		}

		public IApplicationRef EndLaunch(IAsyncResult asynchronousResult)
		{
			return ((QS._qss_c_.Base3_.AsynchronousCall2<IApplicationRef>)asynchronousResult).OperationResult;
		}

		public IApplicationRef launch(string fullyQualifiedClassName, QS._core_c_.Components.AttributeSet arguments)
		{
            System.Type type = null;
            foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(fullyQualifiedClassName);
                if (type != null)
                    break;
            }

            if (type == null)
                throw new Exception("Unknown type \"" + fullyQualifiedClassName + "\".");

			System.Reflection.ConstructorInfo constructorInfo = type.GetConstructor(
				new System.Type[] { typeof(QS.Fx.Platform.IPlatform), typeof(QS._core_c_.Components.AttributeSet) });
			return launch(constructorInfo, new object[] { arguments });
		}

        private long lastused_appid = 0;
        public IApplicationRef launch(System.Reflection.ConstructorInfo constructorInfo, object[] arguments)
		{
			ApplicationRef appRef = null;

			lock (this)
			{
				if (platform == null)
					throw new Exception("shutting down, cannot launch applications");

				if (constructorInfo == null)
				throw new Exception("could not locate the appropriate constructor");

				object[] completeArguments = new object[arguments.Length + 1];
				arguments.CopyTo(completeArguments, 1);
				completeArguments[0] = this.platform;

				IDisposable applicationObject = (IDisposable) constructorInfo.Invoke(completeArguments);
				if (applicationObject == null)
					throw new Exception("the created application object is null");

				appRef = new ApplicationRef(this, applicationObject, (ulong) System.Threading.Interlocked.Increment(ref lastused_appid));
				applications.insert(appRef);
			}

			return appRef;
		}

		#endregion

		#region IManagedComponent Members

		public string Name
		{
			get
			{
				throw new Exception("not supported");
			}
		}

		public QS._qss_e_.Management_.IManagedComponent[] Subcomponents
		{
			get
			{
				throw new Exception("not nsupported");
			}
		}

		public QS._core_c_.Base.IOutputReader Log
		{
			get
			{
				throw new Exception("not supported");
			}
		}

		public object Component
		{
			get
			{
				throw new Exception("not supported");
			}
		}

		#endregion
	}
}
