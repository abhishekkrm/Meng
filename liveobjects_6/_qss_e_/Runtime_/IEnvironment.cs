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

namespace QS._qss_e_.Runtime_
{
	/// <summary>
	/// Summary description for IEnvironment.
	/// </summary>
	public interface IEnvironment : IDisposable, Management_.IManagedComponent, QS.Fx.Inspection.IInspectable
	{
		INodeRef[] Nodes
		{
			get;
		}

		QS.Fx.Clock.IAlarmClock AlarmClock
		{
			get;
		}

		QS.Fx.Clock.IClock Clock
		{
			get;
		}
	}

	public interface INodeRef : IDisposable
	{
		IPAddress[] NICs
		{
			get;
		}

		IApplicationRef launch(string fullyQualifiedClassName, QS._core_c_.Components.AttributeSet arguments);
		IApplicationRef launch(System.Reflection.ConstructorInfo constructorInfo, object[] arguments);

		IAsyncResult BeginLaunch(string fullyQualifiedClassName, QS._core_c_.Components.AttributeSet arguments, 
			AsyncCallback callback, object asynchronousState);
		IApplicationRef EndLaunch(IAsyncResult asynchronousResult);

//		IApplicationRef[] Apps
//		{
//			get;
//		}

        void ReleaseResources();
	}

	public interface IApplicationRef : QS.Fx.Inspection.IInspectable, System.IDisposable	
	{
        string Address
        {
            get;
        }

        ulong AppID
        {
            get;
        }

        object invoke(System.Reflection.MethodInfo methodInfo, object[] arguments);
		object invoke(System.Reflection.MethodInfo methodInfo, object[] arguments, TimeSpan timeout);

		IAsyncResult BeginInvoke(System.Reflection.MethodInfo methodInfo, object[] arguments,
			AsyncCallback callback, object asynchronousState);
		object EndInvoke(IAsyncResult asynchronousResult);

		Base_1_.IApplicationController Controller
        {
            get;
            set;
        }
    }

    // public delegate CMS.Components.AttributeSet ApplicationUpcallCallback(CMS.Components.AttributeSet arguments)
}
