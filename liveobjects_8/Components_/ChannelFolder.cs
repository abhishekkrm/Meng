/*

Copyright (c) 2009 Mihir Patel, Umang Kaul. All rights reserved.

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
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace liveobjects_8.Components_
{
    [QS.Fx.Reflection.ComponentClass("FC22950D19BA48b7B3A3C1B969D710E9`1", "ChannelFolder")]
    public sealed class ChannelFolder
        :
        IChannelFolder,
        IIncoming<QS.Fx.Object.Classes.IObject>,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<Folder_, Channels>,
        QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>
    {
        public ChannelFolder(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<Folder_, Channels>> channel)            
        {
            this._mycontext = _mycontext;
            this.folderendpoint = _mycontext.DualInterface<IOutgoing, IIncoming<QS.Fx.Object.Classes.IObject>>(this);
            this.channelendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<Folder_, Channels>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<Folder_, Channels>>(this);
            this.channelconnection = this.channelendpoint.Connect(channel.Dereference(_mycontext).Channel);
            //this.parentfolder = "";
            //this.currentfolder = "root";
            root.parent = "";
            root.mychannels = new Dictionary<string, Channel>();
            root.foldername = "root";
            this.path = "root";
            this.allfolders.Add("root", root);
            
            //for sharedfolder connections
            endPoints = new Dictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
            QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>>>();

            connections = new Dictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
                QS.Fx.Endpoint.IConnection>();
        }

        //private IDictionary<string, Channel> mychannels = new Dictionary<string, Channel>();
        private bool ready;
        //public string parentfolder;
        //public string currentfolder;
        public string path;
        QS.Fx.Object.IContext _mycontext;
        private QS.Fx.Endpoint.Internal.IDualInterface<IOutgoing, IIncoming<QS.Fx.Object.Classes.IObject>> folderendpoint;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<Folder_, Channels>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<Folder_, Channels>> channelendpoint;
        private QS.Fx.Endpoint.IConnection channelconnection;
        // Stores the endpoints of the shared folder objects
        private IDictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
             QS.Fx.Endpoint.Internal.IDualInterface<
             QS.Fx.Interface.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>,
             QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>>> endPoints;
        //stores connections to shared folder object endpoints
        private IDictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
           QS.Fx.Endpoint.IConnection> connections;
        //shared folder type
        private static readonly QS.Fx.Reflection.IObjectClass sharedFolderObject =
           QS.Fx.Reflection.Library.LocalLibrary.ObjectClass<QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>>();
        #region MultipleFolders

        struct foldercontents
        {
            public string foldername;
            public string parent;
            public IDictionary<string, Channel> mychannels;
        };
        private IDictionary<string, foldercontents> allfolders = new Dictionary<string, foldercontents>();
        foldercontents root;        
        #endregion

        QS.Fx.Endpoint.Classes.IDualInterface<IOutgoing, IIncoming<QS.Fx.Object.Classes.IObject>> IChannelFolder.ChannelFolder
        {
            get { return this.folderendpoint; }
        }

        Channels QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<Folder_, Channels>.Checkpoint()
        {
            lock (this)
            {
                foldercontents currfolder;
                this.allfolders.TryGetValue(this.path, out currfolder);
                return new Channels((new List<Channel>(currfolder.mychannels.Values)).ToArray());
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<Folder_, Channels>.Initialize(Channels _checkpoint)
        {
            lock (this)
            {
                foldercontents currfolder;
                string keyfolder = this.path;
                this.allfolders.TryGetValue(keyfolder, out currfolder);

                currfolder.mychannels.Clear();
                if ((_checkpoint != null) && (_checkpoint.channels != null))
                    foreach (Channel channel in _checkpoint.channels)
                        currfolder.mychannels.Add(channel.id,channel);
                this.ready = true;
                if (this.folderendpoint.IsConnected)
                    this.folderendpoint.Interface.Ready();
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<Folder_, Channels>.Receive(Folder_ _message)
        {
            lock (this)
            {

                List<Channel> alerts = new List<Channel>();
                //String keyfolder = _message.parent + "/" + _message.current;
                String keyfolder = _message.path;
                setpath(_message.path);
                foldercontents currfolder;
                
                this.allfolders.TryGetValue(keyfolder, out currfolder);

                Channel channel;
                if ((_message._id != null) && (_message._id.Length > 0) && _message.op == 1) //Add new channel
                {
                    if (currfolder.mychannels.TryGetValue(_message._id, out channel))
                    {
                        channel = new Channel("-1", _message._objectxml);
                        alerts.Add(channel);
                    }
                    else
                    {
                        if (_message._id.Contains(";sharedfolder"))
                        {
                            string notify = "\n"+ "<!-- thisisasharedfolder -->";
                            _message._objectxml = _message._objectxml + notify;
                            //remove the ;sharedfolder marker from ID
                            _message._id = _message._id.Substring(0, _message._id.LastIndexOf(";"));
                        }
                        channel = new Channel(_message._id, _message._objectxml);
                        currfolder.mychannels.Add(_message._id, channel);
                        alerts.Add(channel);
                    }
                }
                else if ((_message._id != null) && (_message._id.Length > 0) && _message.op == 2) //Delete
                {
                    currfolder.mychannels.Remove(_message._id);
                    this.folderendpoint.Interface.Alert(_message._id, _message._objectxml);
                }

                else if ((_message._id != null) && (_message._id.Length > 0) && _message.op == 3) //Rename
                {

                    if (currfolder.mychannels.TryGetValue(_message.newID, out channel))
                    {
                        channel = new Channel("-1", _message._objectxml);
                        alerts.Add(channel);
                    }
                    else
                    {
                        if (currfolder.mychannels.TryGetValue(_message._id, out channel))
                        {
                            channel.id = _message.newID;
                            currfolder.mychannels.Remove(_message._id);
                            currfolder.mychannels.Add(_message.newID, channel);
                            //this.folderendpoint.Interface.Alert(_message._id, channel.objectxml);
                            alerts.Add(channel);
                        }
                    }
                }
                else if ((_message._id != null) && (_message._id.Length > 0) && _message.op == 4) //Add New Folder
                {
                    if (currfolder.mychannels.TryGetValue(_message._id, out channel))
                    {
                        channel = new Channel("-1", _message._id);                        
                        alerts.Add(channel);
                    }
                    else
                    {
                        channel = new Channel(_message._id, "thisisafolder");
                        currfolder.mychannels.Add(_message._id, channel);

                        foldercontents f;
                        f.foldername = _message._id;
                        f.parent = _message.path;
                        f.mychannels = new Dictionary<string, Channel>();
                        string newKey = _message.path + "/" + _message._id;
                        this.allfolders.Add(newKey, f);
                        
                        alerts.Add(channel);
                    }
                }

                foreach (Channel chnl in alerts)
                    this.folderendpoint.Interface.Alert(chnl.id, chnl.objectxml);
            }
        }

        bool IIncoming<QS.Fx.Object.Classes.IObject>.Ready()
        {
            return this.ready;
        }
        /*
        bool IIncoming<QS.Fx.Object.Classes.IObject>.TryGetObject(string _key, out QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            try
            {
                _object = ((IIncoming<QS.Fx.Object.Classes.IObject>)this).GetObject(_key);
                return true;
            }
            catch (Exception)
            {
                _object = null;
                return false;
            }
        }*/

        /*
        private void _Synchronize()
        {
            if (!this.ready)
                this._ready_event.WaitOne();
        }*/
        /*
        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> IIncoming<QS.Fx.Object.Classes.IObject>.GetObject(string _key)
        {
            this._Synchronize();
            lock (this)
            {
                
                //QS.Fx.Base.ID _id;
                //try
                //{
                //    _id = new QS.Fx.Base.ID(_key);
                //}
                //catch (Exception)
                //{
                //    _id = null;
                //}
                Channel _channel;
                if ((_key == null) || !this.mychannels.TryGetValue(_key, out _channel))
                    throw new Exception("No channel named \"" + _key + "\" has been defined.");
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref = this._loaderendpoint.Interface.Load(_channel.objectxml);
                return _objectref;
            }
        }
        *
        */
        
        Channels IIncoming<QS.Fx.Object.Classes.IObject>.Channels(string path)
        {
            lock (this)
            {
                
                foldercontents currfolder;
                string keyfolder = path;
                this.allfolders.TryGetValue(keyfolder, out currfolder);
                return new Channels((new List<Channel>(currfolder.mychannels.Values)).ToArray());
            }
        }

        void IIncoming<QS.Fx.Object.Classes.IObject>.Add(string path, String _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object, int op)
        {
            lock (this)
            {
                string _objectxml;
                foldercontents currfolder;
                this.allfolders.TryGetValue(path, out currfolder);

                if (currfolder.mychannels.ContainsKey(_key))
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

                this.channelendpoint.Interface.Send(new Folder_(path, _key, _objectxml, op));
            }
        }

        void IIncoming<QS.Fx.Object.Classes.IObject>.Delete(string path, string channelID, int op)
        {
            this.channelendpoint.Interface.Send(new Folder_(path, channelID, op));
        }

        void IIncoming<QS.Fx.Object.Classes.IObject>.Rename(string path, string channelID, string newID, int op)
        {            
            this.channelendpoint.Interface.Send(new Folder_(path, channelID, newID, op));
        }
        void IIncoming<QS.Fx.Object.Classes.IObject>.AddNewFolder(string path, string folderName, int op)
        {            
            this.channelendpoint.Interface.Send(new Folder_(path, folderName, op));                        
        }
        string IIncoming<QS.Fx.Object.Classes.IObject>.GetXML(string path, string _key)        
        {            
            Channel channel;
            foldercontents currfolder;
            this.allfolders.TryGetValue(path, out currfolder);
            if (currfolder.mychannels.TryGetValue(_key, out channel))
            {
                return channel.objectxml;
            }
            else
                return "Could not find object";
        }
        IDictionary<String, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> IIncoming<QS.Fx.Object.Classes.IObject>.getChildren(QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _key)
        {
            lock (this)
            {
                IDictionary<String, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> returnMap =
                    new Dictionary<String, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>();

                //if (!_key.ObjectClass.IsSubtypeOf(sharedFolderObject)) return returnMap;

                QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>,
                QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>> endPoint;

                if (!endPoints.ContainsKey(_key))
                {
                    //cast it to a folder object  
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>> folderRef;
                    lock (_key)
                    {
                        folderRef = _key.CastTo<QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>>();
                    }
                    QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject> folder;
                    lock (folderRef)
                    {
                        folder = folderRef.Dereference(this._mycontext);
                    }

                    //Get the endPoint
                    //Tells the endPoint that this class is the second end of the connection
                    lock (folderRef)
                    {
                        endPoint =
                       this._mycontext.DualInterface<
                       QS.Fx.Interface.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>,
                       QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>>(this);
                    }
                    System.Threading.Thread.Sleep(500);
                    lock (endPoint)
                    {
                        endPoints.Add(_key, endPoint);
                    }
                    System.Threading.Thread.Sleep(500);
                    lock (endPoint)
                    {
                        connections.Add(_key, endPoint.Connect(folder.Endpoint));
                    }


                }
                else
                {
                    endPoints.TryGetValue(_key, out endPoint);
                }



                foreach (String _name in endPoint.Interface.Keys())
                {

                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> objRef;

                    if (endPoint.Interface.TryGetObject(_name, out objRef))
                    {
                        /*
                        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> objReference =
                            objRef.CastTo<QS.Fx.Object.Classes.IObject>();
                    
                        returnMap.Add(_name, objReference);
                         * */
                        returnMap.Add(_name, null);
                    }
                }
                return returnMap;
            }
        }

        void setpath(string path)
        {
            this.path = path;
        }

        #region IDictionaryClient<String,IObject> Members

        void QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>.Ready()
        {
            return;
        }

        void QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>.Added(String _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            //TODO
            return;
        }

        void QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>.Removed(String _key)
        {
            //TODO
            return;
        }

        #endregion
        
    }
}
