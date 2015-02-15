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

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.ComponentClass_02)]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class ComponentClass_02_ : QS._qss_x_.Experiment_.Object_.IObjectClass_02_, QS._qss_x_.Experiment_.Interface_.IInterfaceClass_02_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public ComponentClass_02_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._fooendpoint = _mycontext.ExportedInterface<QS._qss_x_.Experiment_.Interface_.IInterfaceClass_02_>(this);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IExportedInterface<QS._qss_x_.Experiment_.Interface_.IInterfaceClass_02_> _fooendpoint;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IObjectClass_02_ Members

        QS.Fx.Endpoint.Classes.IExportedInterface<QS._qss_x_.Experiment_.Interface_.IInterfaceClass_02_> QS._qss_x_.Experiment_.Object_.IObjectClass_02_._Foo
        {
            get { return this._fooendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IInterfaceClass_02_ Members

        string QS._qss_x_.Experiment_.Interface_.IInterfaceClass_02_._Hello(string _message)
        {
            return "Response from ComponentClass_02_ to (\"" + _message + "\")";
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
