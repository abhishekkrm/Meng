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

namespace QS._qss_x_.Uplink_2_
{
/*
    public sealed class Object_
        : QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>,
        QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>
    {
        #region Constants

        private const string _c_key_QsmChannel = "QsmChannel";

        #endregion

        #region Constructor

        private Object_(Uplink_  _myuplink)
        {
            this._myuplink = _myuplink;
            this._myendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
                QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>(this);
            this._myconstructors =
                new Dictionary<string, QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>>();
            this._myconstructors.Add
            (
                _c_key_QsmChannel,
                ChannelConstructor_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>._CreateConstructor
                (
                    this._myuplink,
                    _c_channel_qsm,
                    "QsmChannel",
                    "QsmChannel",
                    "QsmChannel"
                )
            );
        }

        #endregion

        #region Fields

        private Uplink_ _myuplink;
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _myendpoint;
        private IDictionary<string, QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>> _myconstructors;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class ChannelConstructor_

        private sealed class ChannelConstructor_<MessageClass>
            where MessageClass : class, QS.Fx.Serialization.ISerializable
        {
            public static QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> 
                _CreateConstructor(Uplink_ _uplink, int _channel, string _id, string _name, string _comment)
            {
                return new QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>
                (
                    (new ChannelConstructor_<MessageClass>(_uplink, _channel, _id, _name, _comment))._CreateObjectReference
                );
            }

            private ChannelConstructor_(Uplink_ _uplink, int _channel, string _id, string _name, string _comment)
            {
                this._uplink = _uplink;
                this._channel = _channel;
                this._id = _id;
                this._name = _name;
                this._comment = _comment;
            }

            private Uplink_ _uplink;
            private int _channel;
            private string _id, _name, _comment;

            private QS.Fx.Object.Classes.IObject _CreateObject()
            {
                return new Channel_<MessageClass>(this._uplink, this._channel);
            }

            private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _CreateObjectReference()
            {
                return QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create
                (
                    new QS._qss_c_.Base3_.Constructor<QS.Fx.Object.Classes.IObject>(this._CreateObject),
                    typeof(Channel_<MessageClass>),
                    this._id,
                    QS._qss_x_.Reflection_.Library.ObjectClassOf
                    (
                        typeof(QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<MessageClass>)
                    ),
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

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IDictionary<string,IObject> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>
            QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Endpoint
        {
            get { return this._myendpoint; }
        }

        #endregion

        #region IDictionary<string,IObject> Members

        IEnumerable<string> QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Keys()
        {
            lock (this)
            {
                return this._myconstructors.Keys;
            }
        }

        IEnumerable<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Objects()
        {
            List<QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>> _constructors =
                new List<QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>>();
            lock (this)
            {
                _constructors.AddRange(this._myconstructors.Values);
            }
            List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _objects =
                new List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>();
            foreach (QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _constructor in _constructors)
                _objects.Add(_constructor());
            return _objects;
        }

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.ContainsKey(string _key)
        {
            lock (this)
            {
                return this._myconstructors.ContainsKey(_key);
            }
        }

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.GetObject(string _key)
        {
            QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _constructor;
            lock (this)
            {
                if (!this._myconstructors.TryGetValue(_key, out _constructor))
                    throw new Exception("No object with id \"" + _key + "\" could be found in this folder.");
            }
            return _constructor();
        }

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.TryGetObject(
            string _key, out QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _constructor;
            lock (this)
            {
                if (!this._myconstructors.TryGetValue(_key, out _constructor))
                {
                    _object = null;
                    return false;
                }
            }
            _object = _constructor();
            return true;
        }

        int QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Count()
        {
            lock (this)
            {
                return this._myconstructors.Count;
            }
        }

        void QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Add(
            string _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            throw new NotSupportedException();
        }

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.IsReadOnly()
        {
            return true;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class Factory_

        [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Uplink_, "Uplink", "Uplink")]
        public sealed class Factory_
            : QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>,
            QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>
        {
            #region Constructor

            public Factory_(
                [QS.Fx.Reflection.Parameter("Configuration", QS.Fx.Reflection.ParameterClass.Value)] 
                    QS.Fx.Configuration.IConfiguration _configuration)
            {
                this._myendpoint =
                    _mycontext.ExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>>(this);
                this._configuration = _configuration;
            }

            #endregion

            #region Fields

            private QS.Fx.Endpoint.Internal.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>> _myendpoint;
            private QS.Fx.Configuration.IConfiguration _configuration;
            private Uplink_ _myuplink;

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region IFactory<IObject> Members

            QS.Fx.Endpoint.Classes.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>>
                QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>.Endpoint
            {
                get { return this._myendpoint; }
            }

            #endregion

            #region IFactory<IObject> Members

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>
                QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>.Create()
            {
                lock (this)
                {
                    return QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create
                    (
                        new QS._qss_c_.Base3_.Constructor<QS.Fx.Object.Classes.IObject>(this._Object_CreateCallback_),
                        typeof(Object_),
                        "Uplink",
                        QS._qss_x_.Reflection_.Library.ObjectClassOf
                        (
                            typeof(QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>)
                        ),
                        new QS.Fx.Attributes.Attributes(
                            new QS.Fx.Attributes.IAttribute[] 
                            {
                                new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_name, "Uplink"),
                            }
                        )
                    );
                }
            }

            #endregion

            #region _Object_CreateCallback_

            private QS.Fx.Object.Classes.IObject _Object_CreateCallback_()
            {
                lock (this)
                {
                    if (this._myuplink == null)
                        this._myuplink = new Uplink_(this._configuration);
                    return (QS.Fx.Object.Classes.IObject) new Object_(this._myuplink);
                }
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        #endregion
    }
*/
}
