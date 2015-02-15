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
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace QS._qss_x_.Object_
{
    public sealed class Constructor_<ImplementingClass, ObjectClass> : IConstructor_
        where ObjectClass : class, QS.Fx.Object.Classes.IObject
    {
        #region Constructor

        public Constructor_(QS.Fx.Object.IContext _mycontext, string _id, string _name, string _comment, 
            QS._qss_c_.Base3_.Constructor<QS.Fx.Object.Classes.IObject> _constructor)
        {
            this._mycontext = _mycontext;
            this._id = _id;
            this._name = _name;
            this._comment = _comment;
            this._constructor = _constructor;
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        private string _id, _name, _comment;
        private QS._qss_c_.Base3_.Constructor<QS.Fx.Object.Classes.IObject> _constructor;

        #endregion

        #region IConstructor_ Members

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> IConstructor_.Create
        {
            get 
            {
                lock (this)
                {
                    return QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create
                    (
                        this._constructor,
                        typeof(ImplementingClass),
                        this._id,
                        QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(ObjectClass)),
                        new QS.Fx.Attributes.Attributes
                        (
                            new QS.Fx.Attributes.IAttribute[] 
                            {
                                new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_name, this._name),
                                new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_comment, this._comment)
                            }
                        )
                    );
                }
            }
        }

        #endregion
    }
}
