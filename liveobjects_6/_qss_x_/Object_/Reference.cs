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

namespace QS._qss_x_.Object_
{
    public static class Reference<ObjectClass> where ObjectClass : class, QS.Fx.Object.Classes.IObject
    {
        #region Create

        public static QS.Fx.Object.IReference<ObjectClass> Create(
            QS.Fx.Object.Classes.IObject _object, string _id, QS.Fx.Reflection.IObjectClass _objectclass)
        {
            return new Reference1(_object, _id, new QS.Fx.Attributes.Attributes(_object), _objectclass);
        }

        public static QS.Fx.Object.IReference<ObjectClass> Create(
            QS.Fx.Object.Classes.IObject _object, string _id, QS.Fx.Attributes.IAttributes _attributes, 
            QS.Fx.Reflection.IObjectClass _objectclass)
        {
            return new Reference1(_object, _id, _attributes, _objectclass);
        }

        public static QS.Fx.Object.IReference<ObjectClass> Create(QS.Fx.Reflection.IObject _object)
        {
            return new Reference2(_object);
        }

        public static QS.Fx.Object.IReference<ObjectClass> Create(
            QS.Fx.Reflection.IObject _object, QS.Fx.Attributes.IAttributes _attributes)
        {
            return new Reference2(_object, _attributes);
        }

        public static QS.Fx.Object.IReference<ObjectClass> Create(
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _folder,
            string _id, QS.Fx.Reflection.IObjectClass _objectclass)
        {
            return new Reference3(_folder, _id, _objectclass);
        }

        public static QS.Fx.Object.IReference<ObjectClass> Create(
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _folder,
            string _id, QS.Fx.Reflection.IObjectClass _objectclass, QS.Fx.Attributes.IAttributes _attributes)
        {
            return new Reference3(_folder, _id, _objectclass, _attributes);
        }

        public static QS.Fx.Object.IReference<ObjectClass> Create(
            QS._qss_c_.Base3_.Constructor<QS.Fx.Object.Classes.IObject> _constructorcallback, 
            Type _underlyingtype, string _id, QS.Fx.Reflection.IObjectClass _objectclass, QS.Fx.Attributes.IAttributes _attributes)
        {
            return new Reference4(_constructorcallback, _underlyingtype, _id, _objectclass, _attributes);
        }

        public static QS.Fx.Object.IReference<ObjectClass> Create
        (
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _authenticatedobject,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IAuthenticating1<QS.Fx.Object.Classes.IObject>> _authenticatingobject
        )
        {
            return new Reference5(_authenticatedobject, _authenticatingobject);
        }

        public static QS.Fx.Object.IReference<ObjectClass> Create
        (
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object,
            QS.Fx.Reflection.IObjectClass _objectclass
        )
        {
            return new Reference6(_object, _objectclass);
        }

        public static QS.Fx.Object.IReference<ObjectClass> Create
        (
            string _id,
            QS.Fx.Attributes.IAttributes _attributes, 
            QS.Fx.Reflection.IObjectClass _objectclass,
            IDictionary<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _components
        )
        {
            return new Reference7(_id, _attributes, _objectclass, _components);
        }

        #endregion

        #region Class Reference1

        [QS.Fx.Printing.Printable("Reference", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        private sealed class Reference1 : QS.Fx.Inspection.Inspectable, QS.Fx.Object.IReference<ObjectClass>
        {
            #region Constructor

            public Reference1(
                QS.Fx.Object.Classes.IObject _object, 
                string _id, 
                QS.Fx.Attributes.IAttributes _attributes, 
                QS.Fx.Reflection.IObjectClass _objectclass)
            {
                this._object = _object;
                this._id = _id;
                this._attributes = _attributes;
                this._objectclass = _objectclass;

                QS.Fx.Reflection.IObjectClass _c1 = this._objectclass;
                QS.Fx.Reflection.IObjectClass _c2 = QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(ObjectClass));
                if (!_c1.IsSubtypeOf(_c2))
                    throw new Exception("Cannot cast a reference to an object of type \"" + _c1.ID + "\" into a reference of type \"" + _c2.ID +
                        "\" because the former is not a subtype of the latter.");
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable("object")]
            [QS.Fx.Printing.Printable("object")]
            private QS.Fx.Object.Classes.IObject _object;

            [QS.Fx.Base.Inspectable("id")]
            [QS.Fx.Printing.Printable("id")]
            private string _id;

            [QS.Fx.Base.Inspectable("attributes")]
            [QS.Fx.Printing.Printable("attributes")]
            private QS.Fx.Attributes.IAttributes _attributes;

            [QS.Fx.Base.Inspectable("objectclass")]
            [QS.Fx.Printing.Printable("objectclass")]
            private QS.Fx.Reflection.IObjectClass _objectclass;

            #endregion

            #region IReference<ObjectClass> Members

            ObjectClass QS.Fx.Object.IReference<ObjectClass>.Dereference(QS.Fx.Object.IContext _mycontext)
            {
                return QS._qss_x_.Reflection_.Internal_._internal._cast_object<ObjectClass>(_object, false);

/*
                    if (_object != null)
                    {
                        if (!(_object is ObjectClass))
                            throw new Exception("The type \"" + _object.GetType().ToString() + "\" of the supplied object does not match the expected type \"" +
                                typeof(ObjectClass).ToString() + "\".");

                        return (ObjectClass) _object;
                    }
                    else
                        return null;
*/
            }

            QS.Fx.Object.IReference<AnotherObjectClass> QS.Fx.Object.IReference<ObjectClass>.CastTo<AnotherObjectClass>()
            {
                return new Reference<AnotherObjectClass>.Reference1(this._object, this._id, this._attributes, this._objectclass);
            }

            #endregion

            #region IClass<IObject> Members

            QS.Fx.Base.ID QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ID
            {
                get { return null; }
            }

            ulong QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Incarnation
            {
                get { throw new NotImplementedException(); }
            }

            QS.Fx.Attributes.IAttributes QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Attributes
            {
                get { return this._attributes; }
            }

            IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ClassParameters
            {
                get { return new Dictionary<string, QS.Fx.Reflection.IParameter>(); }
            }

            IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.OpenParameters
            {
                get { return new Dictionary<string, QS.Fx.Reflection.IParameter>(); }
            }

            QS.Fx.Reflection.IObject QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
            {
                return this;
            }

            Type QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.UnderlyingType
            {
                get { return this._object.GetType(); }
            }

            #endregion

            #region IObject Members

            string QS.Fx.Reflection.IObject.ID
            {
                get { return _id; }
            }

            QS.Fx.Reflection.IObject QS.Fx.Reflection.IObject.From
            {
                get { return null; }
            }

            QS.Fx.Reflection.IObjectClass QS.Fx.Reflection.IObject.ObjectClass
            {
                get { return _objectclass; }
            }

            QS.Fx.Object.Classes.IObject QS.Fx.Reflection.IObject.Dereference(QS.Fx.Object.IContext _mycontext)
            {
                return _object;
            }

            QS.Fx.Reflection.Xml.Object QS.Fx.Reflection.IObject.Serialize
            {
                get
                {
                    throw new Exception("Object \"" + this._id + "\" of class \"" + this._objectclass.ID.ToString() + 
                        "\" cannot be serialized because the medatadata is missing.");
//                    return new QS.Fx.Reflection.Xml.ReferenceObject(this._id, this._objectclass.Serialize, null, null);
                }
            }

            #endregion
        }

        #endregion

        #region Class Reference2

        [QS.Fx.Printing.Printable("Reference", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        private sealed class Reference2 : QS.Fx.Inspection.Inspectable, QS.Fx.Object.IReference<ObjectClass>
        {
            #region Constructor

            public Reference2(QS.Fx.Reflection.IObject _object, QS.Fx.Attributes.IAttributes _attributes)
            {
                this._object = _object;
                this._attributes = _attributes;

                QS.Fx.Reflection.IObjectClass _c1 = this._object.ObjectClass;
                QS.Fx.Reflection.IObjectClass _c2 = QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(ObjectClass));
                if (!_c1.IsSubtypeOf(_c2))
                    throw new Exception("Cannot cast a reference to an object of type \"" + _c1.ID + "\" into a reference of type \"" + _c2.ID +
                        "\" because the former is not a subtype of the latter.");
            }

            public Reference2(QS.Fx.Reflection.IObject _object) : this(_object, null)
            {
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable("object")]
            [QS.Fx.Printing.Printable("object")]
            private QS.Fx.Reflection.IObject _object;

            private QS.Fx.Attributes.IAttributes _attributes;

            #endregion

            #region IReference<ObjectClass> Members

            ObjectClass QS.Fx.Object.IReference<ObjectClass>.Dereference(QS.Fx.Object.IContext _mycontext)
            {
                QS.Fx.Object.Classes.IObject _actualobject = _object.Dereference(_mycontext);
                return (ObjectClass) 
                    QS._qss_x_.Reflection_.Internal_._internal._cast_object(
                        _actualobject, _object.ObjectClass.UnderlyingType, typeof(ObjectClass), false);

/*
                    if (_actualobject != null)
                    {
                        if (!(_actualobject is ObjectClass))
                            throw new Exception("The type \"" + _actualobject.GetType().ToString() + "\" of the supplied object does not match the expected type \"" +
                                typeof(ObjectClass).ToString() + "\".");

                        return (ObjectClass)_actualobject;
                    }
                    else
                        return null;
*/
            }

            QS.Fx.Object.IReference<AnotherObjectClass> QS.Fx.Object.IReference<ObjectClass>.CastTo<AnotherObjectClass>()
            {
                return new Reference<AnotherObjectClass>.Reference2(this._object, this._attributes);                
            }

            #endregion

            #region IClass<IObject> Members

            QS.Fx.Base.ID QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ID
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>) this._object).ID; }
            }

            ulong QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Incarnation
            {
                get { throw new NotImplementedException(); }
            }

            QS.Fx.Attributes.IAttributes QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Attributes
            {
                get { return (this._attributes != null) ? this._attributes : ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>) this._object).Attributes; }
            }

            IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ClassParameters
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>) this._object).ClassParameters; }
            }

            IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.OpenParameters
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>) this._object).OpenParameters; }
            }

            QS.Fx.Reflection.IObject QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
            {
                return new Reference2(((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>)this._object).Instantiate(_parameters), this._attributes);
            }

            Type QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.UnderlyingType
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>) this._object).UnderlyingType; }
            }

            #endregion

            #region IObject Members

            string QS.Fx.Reflection.IObject.ID
            {
                get { return _object.ID; }
            }

            QS.Fx.Reflection.IObject QS.Fx.Reflection.IObject.From
            {
                get { return _object.From; }
            }

            QS.Fx.Reflection.IObjectClass QS.Fx.Reflection.IObject.ObjectClass
            {
                get { return _object.ObjectClass; }
            }

            QS.Fx.Object.Classes.IObject QS.Fx.Reflection.IObject.Dereference(QS.Fx.Object.IContext _mycontext)
            {
                return _object.Dereference(_mycontext);
            }

            QS.Fx.Reflection.Xml.Object QS.Fx.Reflection.IObject.Serialize
            {
                get
                {
                    return this._object.Serialize;
                }
            }

            #endregion
        }

        #endregion

        #region Class Reference3

        [QS.Fx.Printing.Printable("Reference", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        private sealed class Reference3 : QS.Fx.Inspection.Inspectable, QS.Fx.Object.IReference<ObjectClass>
        {
            #region Constructor

            public Reference3(
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _folder,
                string _id, QS.Fx.Reflection.IObjectClass _objectclass)
                : this(_folder, _id, _objectclass, null)
            {
            }

            public Reference3(
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _folder,
                string _id, QS.Fx.Reflection.IObjectClass _objectclass, QS.Fx.Attributes.IAttributes _attributes)
            {
                this._folder = _folder;
                this._id = _id;
                this._objectclass = _objectclass;
                this._attributes = _attributes;

                QS.Fx.Reflection.IObjectClass _c1 = this._objectclass;
                QS.Fx.Reflection.IObjectClass _c2 = QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(ObjectClass));
                if (!_c1.IsSubtypeOf(_c2))
                    throw new Exception("Cannot cast a reference to an object of type \"" + _c1.ID + "\" into a reference of type \"" + _c2.ID +
                        "\" because the former is not a subtype of the latter.");
            }

            #endregion

            #region Fields

            private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _folder;
            private string _id;
            private QS.Fx.Reflection.IObjectClass _objectclass;
            private QS.Fx.Attributes.IAttributes _attributes;

            #endregion

            #region _Load

            private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _Load(QS.Fx.Object.IContext _mycontext)
            {
                QS._qss_x_.Component_.Classes_.DictionaryClient<string, QS.Fx.Object.Classes.IObject> _folderclient =
                    new QS._qss_x_.Component_.Classes_.DictionaryClient<string, QS.Fx.Object.Classes.IObject>(_mycontext, _folder);
                QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject> _folderinterface;
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref_1;
                using (QS._qss_x_.Component_.Classes_.Service<
                    QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>.Connect(_mycontext, _folderclient, out _folderinterface))
                {
                    _objectref_1 = _folderinterface.GetObject(this._id);
                }
                return _objectref_1;
            }

            #endregion

            #region IReference<ObjectClass> Members

            ObjectClass QS.Fx.Object.IReference<ObjectClass>.Dereference(QS.Fx.Object.IContext _mycontext)
            {
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref_1 = this._Load(_mycontext);
                QS.Fx.Object.IReference<ObjectClass> _objectref_2 = _objectref_1.CastTo<ObjectClass>();
                ObjectClass _actualobject = _objectref_2.Dereference(_mycontext);
                return _actualobject;
            }

            QS.Fx.Object.IReference<AnotherObjectClass> QS.Fx.Object.IReference<ObjectClass>.CastTo<AnotherObjectClass>()
            {
                return new Reference<AnotherObjectClass>.Reference3(this._folder, this._id, this._objectclass);
            }

            #endregion

            #region IClass<IObject> Members

            QS.Fx.Base.ID QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ID
            {
                get { return null; }
            }

            ulong QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Incarnation
            {
                get { throw new NotImplementedException(); }
            }

            QS.Fx.Attributes.IAttributes QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Attributes
            {
                get { return (this._attributes != null) ? this._attributes : new QS.Fx.Attributes.Attributes(new QS.Fx.Attributes.IAttribute[0]); }
            }

            IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ClassParameters
            {
                get { return new Dictionary<string, QS.Fx.Reflection.IParameter>(); }
            }

            IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.OpenParameters
            {
                get { return new Dictionary<string, QS.Fx.Reflection.IParameter>(); }
            }

            QS.Fx.Reflection.IObject QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
            {
                throw new Exception("Referencing objects in folders does not currently support templates.");
            }

            Type QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.UnderlyingType
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            #region IObject Members

            string QS.Fx.Reflection.IObject.ID
            {
                get { return this._id; }
            }

            QS.Fx.Reflection.IObject QS.Fx.Reflection.IObject.From
            {
                get { return this._folder; }
            }

            QS.Fx.Reflection.IObjectClass QS.Fx.Reflection.IObject.ObjectClass
            {
                get { return this._objectclass; }
            }

            QS.Fx.Object.Classes.IObject QS.Fx.Reflection.IObject.Dereference(QS.Fx.Object.IContext _mycontext)
            {
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref_1 = this._Load(_mycontext);
                QS.Fx.Object.Classes.IObject _actualobject = _objectref_1.Dereference(_mycontext);
                return _actualobject;
            }

            QS.Fx.Reflection.Xml.Object QS.Fx.Reflection.IObject.Serialize
            {
                get
                {
                    return new QS.Fx.Reflection.Xml.ReferenceObject(
                        this._id, QS.Fx.Attributes.Attributes.Serialize(this._attributes), this._objectclass.Serialize, null, null, _folder.Serialize);
                }
            }

            #endregion
        }

        #endregion

        #region Class Reference4

        [QS.Fx.Printing.Printable("Reference", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        private sealed class Reference4 : QS.Fx.Inspection.Inspectable, QS.Fx.Object.IReference<ObjectClass>
        {
            #region Constructor

            public Reference4(QS._qss_c_.Base3_.Constructor<QS.Fx.Object.Classes.IObject> _constructorcallback, 
                Type _underlyingtype, string _id, QS.Fx.Reflection.IObjectClass _objectclass, QS.Fx.Attributes.IAttributes _attributes)
            {
//                if (!typeof(ObjectClass).IsAssignableFrom(_underlyingtype))
//                    throw new Exception("Cannot create a reference, type mismatch.");

                this._constructorcallback = _constructorcallback;
                this._underlyingtype = _underlyingtype;
                this._id = _id;
                this._objectclass = _objectclass;
                this._attributes = _attributes;

                QS.Fx.Reflection.IObjectClass _c1 = this._objectclass;
                QS.Fx.Reflection.IObjectClass _c2 = QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(ObjectClass));
                if (!_c1.IsSubtypeOf(_c2))
                    throw new Exception("Cannot cast a reference to an object of type \"" + _c1.ID + "\" into a reference of type \"" + _c2.ID +
                        "\" because the former is not a subtype of the latter.");
            }

            #endregion

            #region Fields

            private QS._qss_c_.Base3_.Constructor<QS.Fx.Object.Classes.IObject> _constructorcallback;
            private Type _underlyingtype;
            private string _id;
            private QS.Fx.Reflection.IObjectClass _objectclass;
            private QS.Fx.Attributes.IAttributes _attributes;

            #endregion

            #region IReference<ObjectClass> Members

            ObjectClass QS.Fx.Object.IReference<ObjectClass>.Dereference(QS.Fx.Object.IContext _mycontext)
            {
                QS.Fx.Object.Classes.IObject _component = _constructorcallback();
                if (!_component.GetType().Equals(_underlyingtype))
                    throw new Exception("Constructor returned an obejct of a wrong type, expecting " + 
                        _underlyingtype.ToString() + ", but got " + _component.GetType().ToString() + ".");

                return QS._qss_x_.Reflection_.Internal_._internal._cast_object<ObjectClass>(_component, false);

/*
                    if (_component is ObjectClass)
                        return (ObjectClass)_component;
                    else
                        throw new Exception("Type mismatch.");
*/
            }

            QS.Fx.Object.IReference<AnotherObjectClass> QS.Fx.Object.IReference<ObjectClass>.CastTo<AnotherObjectClass>()
            {
                return new Reference<AnotherObjectClass>.Reference4(this._constructorcallback, this._underlyingtype, this._id, this._objectclass, this._attributes);
            }

            #endregion

            #region IObject Members

            string QS.Fx.Reflection.IObject.ID
            {
                get { return _id; }
            }

            ulong QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Incarnation
            {
                get { throw new NotImplementedException(); }
            }

            QS.Fx.Reflection.IObject QS.Fx.Reflection.IObject.From
            {
                get { return null; }
            }

            QS.Fx.Reflection.IObjectClass QS.Fx.Reflection.IObject.ObjectClass
            {
                get { return _objectclass; }
            }

            QS.Fx.Object.Classes.IObject QS.Fx.Reflection.IObject.Dereference(QS.Fx.Object.IContext _mycontext)
            {
                QS.Fx.Object.Classes.IObject _component = _constructorcallback();
                if (!_component.GetType().Equals(_underlyingtype))
                    throw new Exception("Constructor returned an obejct of a wrong type, expecting " +
                        _underlyingtype.ToString() + ", but got " + _component.GetType().ToString() + ".");
                return _component;
            }

            QS.Fx.Reflection.Xml.Object QS.Fx.Reflection.IObject.Serialize
            {
                get { throw new NotImplementedException("Sorry, teferences of this type currently cannot be serialized."); }
            }

            #endregion

            #region IClass<IObject> Members

            QS.Fx.Base.ID QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ID
            {
                get { return null; }
            }

            QS.Fx.Attributes.IAttributes QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Attributes
            {
                get { return this._attributes; }
            }

            IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ClassParameters
            {
                get { return new Dictionary<string, QS.Fx.Reflection.IParameter>(); }
            }

            IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.OpenParameters
            {
                get { return new Dictionary<string, QS.Fx.Reflection.IParameter>(); }
            }

            QS.Fx.Reflection.IObject QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
            {
                return this;
            }

            Type QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.UnderlyingType
            {
                get { return this._underlyingtype; }
            }

            #endregion
        }

        #endregion

        #region Class Reference5

        [QS.Fx.Printing.Printable("Reference", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        private sealed class Reference5 : QS.Fx.Inspection.Inspectable, QS.Fx.Object.IReference<ObjectClass>
        {
            #region Constructor

            public Reference5
            (
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _authenticatedobject,
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IAuthenticating1<QS.Fx.Object.Classes.IObject>> _authenticatingobject
            )
            {
                this._authenticatedobject = _authenticatedobject;
                this._authenticatingobject = _authenticatingobject;

                QS.Fx.Reflection.IObjectClass _c1 = this._authenticatedobject.ObjectClass;
                QS.Fx.Reflection.IObjectClass _c2 = QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(ObjectClass));
                if (!_c1.IsSubtypeOf(_c2))
                    throw new Exception("Cannot cast a reference to an object of type \"" + _c1.ID + "\" into a reference of type \"" + _c2.ID +
                        "\" because the former is not a subtype of the latter.");
            }

            #endregion

            #region Fields

            private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _authenticatedobject;
            private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IAuthenticating1<QS.Fx.Object.Classes.IObject>> _authenticatingobject;

            #endregion

            #region _CreateObject

            private ObjectClass _CreateObject(QS.Fx.Object.IContext _mycontext)
            {
                // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                QS.Fx.Object.Classes.IObject _proxyobject = 
                    (QS.Fx.Object.Classes.IObject) 
                        (new AuthenticatedObject_(_mycontext, _authenticatedobject, _authenticatingobject)).GetTransparentProxy();

                return (ObjectClass) 
                    QS._qss_x_.Reflection_.Internal_._internal._cast_object(
                        _proxyobject, _authenticatedobject.ObjectClass.UnderlyingType, typeof(ObjectClass), false);
            }

            #endregion

            #region IReference<ObjectClass> Members

            ObjectClass QS.Fx.Object.IReference<ObjectClass>.Dereference(QS.Fx.Object.IContext _mycontext)
            {
                return this._CreateObject(_mycontext); 
            }

            QS.Fx.Object.IReference<AnotherObjectClass> QS.Fx.Object.IReference<ObjectClass>.CastTo<AnotherObjectClass>()
            {
                return new Reference<AnotherObjectClass>.Reference5(this._authenticatedobject, this._authenticatingobject);
            }
            
            #endregion

            #region IObject Members

            string QS.Fx.Reflection.IObject.ID
            {
                get { return ((QS.Fx.Reflection.IObject)this._authenticatedobject).ID; }
            }

            QS.Fx.Reflection.IObject QS.Fx.Reflection.IObject.From
            {
                get { return ((QS.Fx.Reflection.IObject)this._authenticatedobject).From; }
            }

            QS.Fx.Reflection.IObjectClass QS.Fx.Reflection.IObject.ObjectClass
            {
                get { return ((QS.Fx.Reflection.IObject)this._authenticatedobject).ObjectClass; }
            }

            QS.Fx.Object.Classes.IObject QS.Fx.Reflection.IObject.Dereference(QS.Fx.Object.IContext _mycontext)
            {
                return ((QS.Fx.Object.IReference<ObjectClass>)this).Dereference(_mycontext);
            }

            QS.Fx.Reflection.Xml.Object QS.Fx.Reflection.IObject.Serialize
            {
                get 
                {
                    QS.Fx.Reflection.Xml.Object _xmlobject = this._authenticatedobject.Serialize;
                    _xmlobject.Authority = this._authenticatingobject.Serialize;
                    return _xmlobject;
                }
            }

            #endregion

            #region IClass<IObject> Members

            QS.Fx.Base.ID QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ID
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>) this._authenticatedobject).ID; }
            }

            ulong QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Incarnation
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>)this._authenticatedobject).Incarnation; }
            }

            QS.Fx.Attributes.IAttributes QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Attributes
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>)this._authenticatedobject).Attributes; }
            }

            IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ClassParameters
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>)this._authenticatedobject).ClassParameters; }
            }

            IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.OpenParameters
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>)this._authenticatedobject).OpenParameters; }
            }

            QS.Fx.Reflection.IObject QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
            {
                return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>) this._authenticatedobject).Instantiate(_parameters);
            }

            Type QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.UnderlyingType
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>)this._authenticatedobject).UnderlyingType; }
            }

            #endregion
        }

        #endregion

        #region Class Reference6

        [QS.Fx.Printing.Printable("Reference", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        private sealed class Reference6 : QS.Fx.Inspection.Inspectable, QS.Fx.Object.IReference<ObjectClass>
        {
            #region Constructor

            public Reference6(
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object, QS.Fx.Reflection.IObjectClass _objectclass)
            {
                this._object = _object;
                this._objectclass = _objectclass;
                QS.Fx.Reflection.IObjectClass _c1 = this._object.ObjectClass;
                QS.Fx.Reflection.IObjectClass _c2 = this._objectclass;
                QS.Fx.Reflection.IObjectClass _c3 = QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(ObjectClass));
                if (!_c1.IsSubtypeOf(_c2))
                    throw new Exception("Cannot cast a reference to an object of type \"" + _c1.ID + "\" into a reference of type \"" + _c2.ID +
                        "\" because the former is not a subtype of the latter.");
                if (!_c2.IsSubtypeOf(_c3))
                    throw new Exception("Cannot cast a reference to an object of type \"" + _c2.ID + "\" into a reference of type \"" + _c3.ID +
                        "\" because the former is not a subtype of the latter.");
            }

            #endregion

            #region Fields

            private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object;
            private QS.Fx.Reflection.IObjectClass _objectclass;

            #endregion

            #region IReference<ObjectClass> Members

            ObjectClass QS.Fx.Object.IReference<ObjectClass>.Dereference(QS.Fx.Object.IContext _mycontext)
            {
                return (ObjectClass)_object.Dereference(_mycontext);
            }

            QS.Fx.Object.IReference<AnotherObjectClass> QS.Fx.Object.IReference<ObjectClass>.CastTo<AnotherObjectClass>()
            {
                return new Reference<AnotherObjectClass>.Reference6(this._object, this._objectclass);
            }

            #endregion

            #region IObject Members

            string QS.Fx.Reflection.IObject.ID
            {
                get { return ((QS.Fx.Reflection.IObject) this._object).ID; }
            }

            QS.Fx.Reflection.IObject QS.Fx.Reflection.IObject.From
            {
                get { return ((QS.Fx.Reflection.IObject) this._object).From; }
            }

            QS.Fx.Reflection.IObjectClass QS.Fx.Reflection.IObject.ObjectClass
            {
                get { return this._objectclass; }
            }

            QS.Fx.Object.Classes.IObject QS.Fx.Reflection.IObject.Dereference(QS.Fx.Object.IContext _mycontext)
            {
                return ((QS.Fx.Reflection.IObject) this._object).Dereference(_mycontext);
            }

            QS.Fx.Reflection.Xml.Object QS.Fx.Reflection.IObject.Serialize
            {
                get
                {
                    QS.Fx.Reflection.Xml.Object _xmlobject = this._object.Serialize;
                    _xmlobject.ObjectClass = this._objectclass.Serialize;
                    return _xmlobject;
                }
            }

            #endregion

            #region IClass<IObject> Members

            QS.Fx.Base.ID QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ID
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>) this._object).ID; }
            }

            ulong QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Incarnation
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>) this._object).Incarnation; }
            }

            QS.Fx.Attributes.IAttributes QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Attributes
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>) this._object).Attributes; }
            }

            IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ClassParameters
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>) this._object).ClassParameters; }
            }

            IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.OpenParameters
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>) this._object).OpenParameters; }
            }

            QS.Fx.Reflection.IObject QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
            {
                return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>) this._object).Instantiate(_parameters);
            }

            Type QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.UnderlyingType
            {
                get { return ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>) this._objectclass).UnderlyingType; }
            }

            #endregion
        }

        #endregion

        #region Class Reference7

        [QS.Fx.Printing.Printable("Reference", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        private sealed class Reference7 : QS.Fx.Inspection.Inspectable, QS.Fx.Object.IReference<ObjectClass>
        {
            #region Constructor

            public Reference7
            (
                string _id,
                QS.Fx.Attributes.IAttributes _attributes, 
                QS.Fx.Reflection.IObjectClass _objectclass,
                IDictionary<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _components
            )
            {
                this._id = _id;
                this._attributes = _attributes;
                this._objectclass = _objectclass;
                this._components = _components;

                QS.Fx.Reflection.IObjectClass _c1 = this._objectclass;
                QS.Fx.Reflection.IObjectClass _c2 = QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(ObjectClass));
                if (!_c1.IsSubtypeOf(_c2))
                    throw new Exception("Cannot cast a reference to an object of type \"" + _c1.ID + "\" into a reference of type \"" + _c2.ID +
                        "\" because the former is not a subtype of the latter.");
            }

            #endregion

            #region Fields

            private string _id;
            private QS.Fx.Attributes.IAttributes _attributes;
            private QS.Fx.Reflection.IObjectClass _objectclass;
            private IDictionary<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _components;

            #endregion

            #region _Dereference

            private ObjectClass _Dereference(QS.Fx.Object.IContext _mycontext)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IReference<ObjectClass> Members

            ObjectClass QS.Fx.Object.IReference<ObjectClass>.Dereference(QS.Fx.Object.IContext _mycontext)
            {
                return this._Dereference(_mycontext);
            }

            QS.Fx.Object.IReference<AnotherObjectClass> QS.Fx.Object.IReference<ObjectClass>.CastTo<AnotherObjectClass>()
            {
                return new Reference<AnotherObjectClass>.Reference7(this._id, this._attributes, this._objectclass, this._components);
            }

            #endregion

            #region IClass<IObject> Members

            QS.Fx.Base.ID QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ID
            {
                get { return null; }
            }

            ulong QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Incarnation
            {
                get { throw new NotImplementedException(); }
            }

            QS.Fx.Attributes.IAttributes QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Attributes
            {
                get { return (this._attributes != null) ? this._attributes : new QS.Fx.Attributes.Attributes(new QS.Fx.Attributes.IAttribute[0]); }
            }

            IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.ClassParameters
            {
                get { return new Dictionary<string, QS.Fx.Reflection.IParameter>(); }
            }

            IDictionary<string, QS.Fx.Reflection.IParameter> QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.OpenParameters
            {
                get { return new Dictionary<string, QS.Fx.Reflection.IParameter>(); }
            }

            QS.Fx.Reflection.IObject QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
            {
                throw new Exception("Composite objects do not currently support templates.");
            }

            Type QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObject>.UnderlyingType
            {
                get { throw new NotImplementedException(); }
            }

            #endregion

            #region IObject Members

            string QS.Fx.Reflection.IObject.ID
            {
                get { return this._id; }
            }

            QS.Fx.Reflection.IObject QS.Fx.Reflection.IObject.From
            {
                get { return null; }
            }

            QS.Fx.Reflection.IObjectClass QS.Fx.Reflection.IObject.ObjectClass
            {
                get { return this._objectclass; }
            }

            QS.Fx.Object.Classes.IObject QS.Fx.Reflection.IObject.Dereference(QS.Fx.Object.IContext _mycontext)
            {
                return this._Dereference(_mycontext);
            }

            QS.Fx.Reflection.Xml.Object QS.Fx.Reflection.IObject.Serialize
            {
                get
                {
                    List<QS.Fx.Reflection.Xml.Attribute> _attributes = new List<QS.Fx.Reflection.Xml.Attribute>();
                    foreach (QS.Fx.Attributes.IAttribute _attribute in this._attributes)
                        _attributes.Add(new QS.Fx.Reflection.Xml.Attribute(_attribute.Class.ID.ToString(), _attribute.Value));
                    List<QS.Fx.Reflection.Xml.CompositeObject.Component> _components = new List<QS.Fx.Reflection.Xml.CompositeObject.Component>();
                    foreach (KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _component in this._components)
                        _components.Add(new QS.Fx.Reflection.Xml.CompositeObject.Component(_component.Key, _component.Value.Serialize));
                    return new QS.Fx.Reflection.Xml.CompositeObject
                    (
                        this._id,
                        _attributes.ToArray(),
                        this._objectclass.Serialize,
                        null,
                        null,
                        _components.ToArray(),
                        null,
                        null
                    );
                }
            }

            #endregion
        }

        #endregion
    }
}
