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
using System.Xml.Serialization;
using System.Threading;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.SharedFolder, "Shared Folder", "A folder object based on a multicast protocol.")]
    public sealed class SharedFolder : QS.Fx.Inspection.Inspectable,
        QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>,
        QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS._qss_x_.Channel_.Message_.Folder.IOperation, QS._qss_x_.Channel_.Message_.Folder.IState>
    {
        #region Constructor

        public SharedFolder
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS._qss_x_.Channel_.Message_.Folder.IOperation, QS._qss_x_.Channel_.Message_.Folder.IState>> 
                        _channel,
            [QS.Fx.Reflection.Parameter("loader", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> 
                        _loader
        )
        {
            this._channelendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS._qss_x_.Channel_.Message_.Folder.IOperation, QS._qss_x_.Channel_.Message_.Folder.IState>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS._qss_x_.Channel_.Message_.Folder.IOperation, QS._qss_x_.Channel_.Message_.Folder.IState>>(this);

            this._folderendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
                QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>(this);

            this._folderendpoint.OnConnect += new QS.Fx.Base.Callback(this._FolderConnectCallback);
            this._folderendpoint.OnConnected += new QS.Fx.Base.Callback(this._FolderConnectedCallback);
            this._folderendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._FolderDisconnectCallback);

            this._loaderendpoint = _mycontext.ImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>();

            if (_loader != null)
                this._loaderconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._loaderendpoint).Connect(_loader.Dereference(_mycontext).Endpoint);
            else
                throw new Exception("Cannot run without a loader attached.");

            if (_channel != null)
            {
                this._channelobject = _channel.Dereference(_mycontext);
                this._channelconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._channelendpoint).Connect(_channelobject.Channel);
            }

            this._InitializeInspection();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("channelobject")]
        private QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
            QS._qss_x_.Channel_.Message_.Folder.IOperation, QS._qss_x_.Channel_.Message_.Folder.IState> _channelobject;

        [QS.Fx.Base.Inspectable("channelendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS._qss_x_.Channel_.Message_.Folder.IOperation, QS._qss_x_.Channel_.Message_.Folder.IState>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS._qss_x_.Channel_.Message_.Folder.IOperation, QS._qss_x_.Channel_.Message_.Folder.IState>>
                _channelendpoint;

        [QS.Fx.Base.Inspectable("channelconnection")]
        private QS.Fx.Endpoint.IConnection _channelconnection;

        [QS.Fx.Base.Inspectable("folderendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _folderendpoint;

        [QS.Fx.Base.Inspectable("loaderendpoint")]
        private QS.Fx.Endpoint.Internal.IImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>> _loaderendpoint;

        [QS.Fx.Base.Inspectable("loaderconnection")]
        private QS.Fx.Endpoint.IConnection _loaderconnection;

        private IDictionary<string, _Object> _objects = new Dictionary<string, _Object>();

        #endregion

        #region Inspection

        [QS.Fx.Base.Inspectable("objects")]
        private QS._qss_e_.Inspection_.DictionaryWrapper0<_Object> __inspectable_objects;

        private void _InitializeInspection()
        {
            __inspectable_objects =
                new QS._qss_e_.Inspection_.DictionaryWrapper0<_Object>("__inspectable_objects", _objects);
        }

        #endregion

        #region IDictionary<string,IObject> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>
            QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Endpoint
        {
            get { return this._folderendpoint; }
        }

        #endregion

        #region _FolderConnectCallback

        private void _FolderConnectCallback()
        {
        }

        #endregion

        #region _FolderConnectedCallback

        private void _FolderConnectedCallback()
        {
        }

        #endregion

        #region _FolderDisconnectCallback

        private void _FolderDisconnectCallback()
        {
            //if (!System.Diagnostics.Debugger.IsAttached)
            //    System.Diagnostics.Debugger.Launch();
            //System.Diagnostics.Debugger.Break();
        }

        #endregion

        #region Class _Object

        private sealed class _Object : QS.Fx.Inspection.Inspectable
        {
            #region Constructor

            public _Object(string _id, string _objectxml)
            {
                this._id = _id;
                this._objectxml = _objectxml;
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            private string _id;
            [QS.Fx.Base.Inspectable]
            private string _objectxml;

            #endregion

            #region Accessors

            public string ID
            {
                get { return this._id; }
            }

            public string ObjectXml
            {
                get { return this._objectxml; }
            }

            #endregion
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IDictionary<string,IObject> Members

        #region IDictionary<string,Object>.Keys

        IEnumerable<string> QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Keys()
        {
            lock (this)
            {
                return new List<string>(this._objects.Keys);
            }
        }

        #endregion

        #region IDictionary<string,Object>.Objects

        IEnumerable<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> 
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Objects()
        {
            lock (this)
            {
                List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _objects = new List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>();
                foreach (_Object _object in this._objects.Values)
                {
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref;
                    if (((QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>) this).TryGetObject(_object.ID, out _objectref))
                        _objects.Add(_objectref);
                }
                return _objects;
            }
        }

        #endregion

        #region IDictionary<string,Object>.ContainsKey

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.ContainsKey(string _key)
        {
            lock (this)
            {
                return this._objects.ContainsKey(_key);
            }
        }

        #endregion

        #region IDictionary<string,Object>.TryGetObject

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.TryGetObject(
            string _key, out QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            try
            {
                _object = ((QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>) this).GetObject(_key);
                return true;
            }
            catch (Exception)
            {
                _object = null;
                return false;
            }
        }

        #endregion

        #region IDictionary<string,Object>.GetObject

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> 
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.GetObject(string _key)
        {
            lock (this)
            {
                _Object _object;
                if (!this._objects.TryGetValue(_key, out _object))
                    throw new Exception("There is no object named \"" + _key + "\" in the folder.");
                return this._loaderendpoint.Interface.Load(_object.ObjectXml);
            }
        }

        #endregion

        #region IDictionary<string,Object>.Add

        void QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Add(
            string _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            string _objectxml;
            lock (this)
            {
                if (this._objects.ContainsKey(_key))
                    throw new Exception("Cannot add object with id \"" + _key + "\" to the folder because an object with such id already exists.");
                StringBuilder sb = new StringBuilder();
                using (StringWriter writer = new StringWriter(sb))
                {
                    try
                    {
                        (new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Root))).Serialize(writer, new QS.Fx.Reflection.Xml.Root(_object.Serialize));
                    }
                    catch (Exception _exc)
                    {
                        throw new Exception("Could not add object to the folder because the object could not be serialized.\n", _exc);
                    }
                }
                _objectxml = sb.ToString();
            }

            this._channelendpoint.Interface.Send(
                new QS._qss_x_.Channel_.Message_.Folder.Operation_Add(_key, _objectxml));
        }

        #endregion

        #region IDictionary<string>.Remove

        void QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Remove(
            string _key)
        {
            lock (this)
            {
                if (this._objects.ContainsKey(_key))
                    this._objects.Remove(_key);
            }
            this._channelendpoint.Interface.Send(
                new QS._qss_x_.Channel_.Message_.Folder.Operation_Remove(_key));
            lock (_folderendpoint)
            {
                if (_folderendpoint.IsConnected)
                    _folderendpoint.Interface.Removed(_key);
            }
        }

        #endregion

        #region IDictionary<string,Object>.Count

        int QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Count()
        {
            lock (this)
            {
                return this._objects.Count;
            }
        }

        #endregion

        #region IDictionary<string,Object>.IsReadOnly

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.IsReadOnly()
        {
            return false;
        }

        #endregion

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ICheckpointedCommunicationChannelClient<IOperation,IState> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
            QS._qss_x_.Channel_.Message_.Folder.IOperation, QS._qss_x_.Channel_.Message_.Folder.IState>.Receive(
                QS._qss_x_.Channel_.Message_.Folder.IOperation _message)
        {
            Monitor.Enter(this);
            bool _this_locked = true;
            try
            {
                switch (_message.OperationType)
                {
                    case QS._qss_x_.Channel_.Message_.Folder.OperationType.Add:
                        {
                            QS._qss_x_.Channel_.Message_.Folder.Operation_Add _operation_add = (QS._qss_x_.Channel_.Message_.Folder.Operation_Add)_message;
                            if (this._objects.ContainsKey(_operation_add.ID))
                                throw new Exception("Cannot add object with id \"" + _operation_add.ID + "\" to the folder because an object with such id already exists.");
                            _Object _object = new _Object(_operation_add.ID, _operation_add.ObjectXml);
                            this._objects.Add(_object.ID, _object);

                            lock (_folderendpoint)
                            {
                                if (_folderendpoint.IsConnected)
                                {
                                    _this_locked = false;
                                    Monitor.Exit(this);
                                    _folderendpoint.Interface.Added(_object.ID, this._loaderendpoint.Interface.Load(_object.ObjectXml));
                                }
                            }
                        }
                        break;

                    case QS._qss_x_.Channel_.Message_.Folder.OperationType.Remove:
                        {
                            QS._qss_x_.Channel_.Message_.Folder.Operation_Remove _operation_remove = (QS._qss_x_.Channel_.Message_.Folder.Operation_Remove)_message;
                            string _key = _operation_remove.ID;

                            if (this._objects.ContainsKey(_key))
                                this._objects.Remove(_key);

                            lock (_folderendpoint)
                            {
                                if (_folderendpoint.IsConnected)
                                {
                                    _this_locked = false;
                                    Monitor.Exit(this);
                                    _folderendpoint.Interface.Removed(_key);
                                }
                            }
 
                        }
                        break;

                    case QS._qss_x_.Channel_.Message_.Folder.OperationType.Rename:
                    default:
                        throw new NotImplementedException();
                }
            }
            finally
            {
                if (_this_locked)
                    Monitor.Exit(this);
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
            QS._qss_x_.Channel_.Message_.Folder.IOperation, QS._qss_x_.Channel_.Message_.Folder.IState>.Initialize(
                QS._qss_x_.Channel_.Message_.Folder.IState _checkpoint)
        {
            Monitor.Enter(this);
            bool _this_locked = true;
            try
            {
                this._objects.Clear();

                if (_checkpoint != null)
                {
                    foreach (QS._qss_x_.Channel_.Message_.Folder.Object _o in _checkpoint.Objects)
                    {
                        if (this._objects.ContainsKey(((QS._qss_x_.Channel_.Message_.Folder.IObject)_o).ID))
                            throw new Exception("Cannot add object with id \"" + ((QS._qss_x_.Channel_.Message_.Folder.IObject)_o).ID +
                                "\" to the folder because an object with such id already exists.");
                        _Object _object = new _Object(((QS._qss_x_.Channel_.Message_.Folder.IObject)_o).ID, ((QS._qss_x_.Channel_.Message_.Folder.IObject)_o).ObjectXml);
                        this._objects.Add(_object.ID, _object);
                    }
                }

                lock (_folderendpoint)
                {
                    if (_folderendpoint.IsConnected)
                    {
                        _this_locked = false;
                        Monitor.Exit(this);
                        _folderendpoint.Interface.Ready();
                    }
                }
            }
            finally
            {
                if (_this_locked)
                    Monitor.Exit(this);
            }
        }

        QS._qss_x_.Channel_.Message_.Folder.IState
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                QS._qss_x_.Channel_.Message_.Folder.IOperation, QS._qss_x_.Channel_.Message_.Folder.IState>.Checkpoint()
        {
            lock (this)
            {
                List<QS._qss_x_.Channel_.Message_.Folder.Object> _o = new List<QS._qss_x_.Channel_.Message_.Folder.Object>();
                foreach (_Object _object in this._objects.Values)
                    _o.Add(new QS._qss_x_.Channel_.Message_.Folder.Object(_object.ID, _object.ObjectXml));
                return new QS._qss_x_.Channel_.Message_.Folder.State(_o);
            }
        }

        #endregion
    }
}
