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

namespace QS._qss_x_.Reflection_
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Classes<C> : QS.Fx.Inspection.Inspectable, IClasses<C> where C : QS.Fx.Reflection.IClass<C>
    {
        #region Constructor

        public Classes()
        {
        }

        #endregion

        #region Fields

        private IDictionary<QS.Fx.Base.ID, C> _classes_1 = new Dictionary<QS.Fx.Base.ID, C>();

        private IDictionary<Type, C> _classes_2 = new Dictionary<Type, C>();

        #endregion

        #region Inspection

        [QS.Fx.Base.Inspectable("classes")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, C> __inspectable_classes
        {
            get
            {
                return new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, C>("_classes", _classes_1,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, C>.ConversionCallback(QS.Fx.Base.ID.FromString));
            }
        }

        #endregion

        #region Members

        public C GetClass(QS.Fx.Base.ID _id)
        {
            lock (this)
            {
                C _class;
                if (!_classes_1.TryGetValue(_id, out _class))
                    throw new Exception("Cannot find a class with id = " + _id.ToString() + ".");
                return _class;
            }
        }

        public C GetClass(Type _type)
        {
            lock (this)
            {
                C _class;
                if (!_classes_2.TryGetValue(_type, out _class))
                    throw new Exception("Cannot find a class for type \"" + _type.ToString() + "\".");
                return _class;
            }
        }

        public bool TryGetClass(QS.Fx.Base.ID _id, out C _class)
        {
            lock (this)
            {
                return _classes_1.TryGetValue(_id, out _class);
            }
        }

        public bool TryGetClass(Type _type, out C _class)
        {
            lock (this)
            {
                return _classes_2.TryGetValue(_type, out _class);
            }
        }

        public void RegisterClass(QS.Fx.Base.ID _id, C _class)
        {
            lock (this)
            {
                if (_classes_1.ContainsKey(_id))
                    throw new Exception("Class with id = \"" + _id.ToString() + "\" has already been registered.");
                _classes_1.Add(_id, _class);
            }
        }

        public bool TryRegisterClass(QS.Fx.Base.ID _id, C _class)
        {
            lock (this)
            {
                if (!_classes_1.ContainsKey(_id))
                {
                    _classes_1.Add(_id, _class);
                    return true;
                }
                else
                    return false;
            }
        }

        public void RegisterClass(Type _type, C _class)
        {
            lock (this)
            {
                if (_classes_2.ContainsKey(_type))
                    throw new Exception("Class for type \"" + _type.ToString() + "\" has already been registered.");
                _classes_2.Add(_type, _class);
            }
        }

        public bool TryRegisterClass(Type _type, C _class)
        {
            lock (this)
            {
                if (!_classes_2.ContainsKey(_type))
                {
                    _classes_2.Add(_type, _class);
                    return true;
                }
                else
                    return false;
            }
        }

        public void RegisterClass(QS.Fx.Base.ID _id, Type _type, C _class)
        {
            lock (this)
            {
                if (!_classes_1.ContainsKey(_id))
                {
                    if (!_classes_2.ContainsKey(_type))
                    {
                        _classes_1.Add(_id, _class);
                        _classes_2.Add(_type, _class);
                    }
                    else
                        throw new Exception("Class for type \"" + _type.ToString() + "\" has already been registered.");
                }
                else
                    throw new Exception("Class with id = \"" + _id.ToString() + "\" has already been registered.");
            }
        }

        public bool TryRegisterClass(QS.Fx.Base.ID _id, Type _type, C _class)
        {
            lock (this)
            {
                if (!_classes_1.ContainsKey(_id))
                {
                    if (!_classes_2.ContainsKey(_type))
                    {
                        _classes_1.Add(_id, _class); 
                        _classes_2.Add(_type, _class);
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
        }

        #endregion
    }
}
