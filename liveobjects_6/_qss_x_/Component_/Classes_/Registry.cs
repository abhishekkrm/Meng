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
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Registry, "Registry",
        "A collection of services with known names, but with a local customized configuration that could be based on system registry or on configuration files.")]
    public sealed class Registry
        : QS.Fx.Inspection.Inspectable,
        QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>
    {
        #region Objects

        public static class Objects
        {
            public const string _quicksilver_c_ = "010BA5DB9C7F48B0A242A0AD2EF73C54";
            public const string _quicksilver_s_ = "571A2D935613476B9E82D16E089EA596";

            public const string Centralized_CC = "011E83FA6A9F426993BB8EF26FF7D7B8";
            public const string Centralized_CC_SVR = "5356357BBA5244009D3094BEBBB3A8B4";
            public const string QSM_C = "3959133F87E84D909DF70C5C2B4F2CB7";
            public const string QSM_S = "571A2D935613476B9E82D16E089EA596";
            public const string Uplink = "957FFCB9E86E48E6B0B297C6CA7CFB3C";
            public const string UplinkController = "EE959E9734974365B4DE82D433BD38AB";
        }

        #endregion

        #region Constructor

        public Registry(QS.Fx.Object.IContext _mycontext)
        {
            lock (this)
            {
                QS._qss_x_.Object_.Context_ _mycontext_internal = (QS._qss_x_.Object_.Context_)_mycontext;
                QS._qss_x_.Registry_._Registry _registry = _mycontext_internal._Registry;
                this._endpoint = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
                    QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>(_registry);
            }
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _endpoint;

        #endregion

        #region IDictionary<string,IObject> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> 
            QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Endpoint
        {
            get { return this._endpoint; }
        }

        #endregion
    }
}
