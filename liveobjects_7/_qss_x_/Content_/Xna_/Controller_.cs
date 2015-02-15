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

#if XNA

using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.Xml;
using System.Xml.Serialization;

namespace QS._qss_x_.Content_.Xna_
{
    public sealed class Controller_ : IController_
    {
        #region Constructor

        public Controller_(QS._qss_x_.Content_.IController_ _library, IServiceProvider _services)
        {
            this._library = _library;
            this._services = _services;
            this._namespaces = new Dictionary<QS.Fx.Base.ID, Namespace_>();
            this._graphicsservice = (IGraphicsDeviceService)this._services.GetService(typeof(IGraphicsDeviceService));
        }

        #endregion

        #region Fields

        private QS._qss_x_.Content_.IController_ _library;
        private IServiceProvider _services;
        private IDictionary<QS.Fx.Base.ID, Namespace_> _namespaces;
        private IGraphicsDeviceService _graphicsservice;

        #endregion

        #region Class Namespace_

        private class Namespace_
        {
            #region Constructor

            public Namespace_(Controller_ _controller, QS.Fx.Base.ID _id) 
            {
                this._controller = _controller;
                this._id = _id;
                this._incarnations = new Dictionary<ulong, Incarnation_>();
            }

            #endregion

            #region Fields

            private Controller_ _controller;
            private QS.Fx.Base.ID _id;
            private IDictionary<ulong, Incarnation_> _incarnations;
            private Incarnation_ _newestincarnation;

            #endregion

            #region Accessors

            public QS.Fx.Base.ID _ID
            {
                get { return this._id; }
            }

            #endregion

            #region Class Incarnation_

            private class Incarnation_ : Microsoft.Xna.Framework.Content.ContentManager
            {
                #region Constructor

                public Incarnation_(Namespace_ _namespace, QS._qss_x_.Content_.INamespace_ _underlyingnamespace) 
                    : base(_namespace._controller._services)
                {
                    this._namespace = _namespace;
                    this._incarnation = _underlyingnamespace._Incarnation;
                    this._underlyingnamespace = _underlyingnamespace;
                }

                #endregion
                
                #region Fields

                private Namespace_ _namespace;
                private ulong _incarnation;
                private QS._qss_x_.Content_.INamespace_ _underlyingnamespace;

                #endregion

                #region Accessors

                public ulong _Incarnation
                {
                    get { return this._incarnation; }
                }

                #endregion

                #region Content_

                private sealed class Content_ : QS.Fx.Xna.IContent
                {
                    #region Constructor

                    public Content_(QS.Fx.Xna.IContentRef _reference, object _content)
                    {
                        this._reference = _reference;
                        this._content = _content;
                    }

                    #endregion

                    #region Fields

                    private QS.Fx.Xna.IContentRef _reference;
                    private object _content;

                    #endregion

                    #region IContent Members

                    QS.Fx.Xna.IContentRef QS.Fx.Xna.IContent.Reference
                    {
                        get { return this._reference; }
                    }

                    object QS.Fx.Xna.IContent.Content
                    {
                        get { return this._content; }
                    }

                    #endregion

                    #region IDisposable Members

                    void IDisposable.Dispose()
                    {
                    }

                    #endregion
                }

                #endregion

                #region _GetContent

                private const string _c_xnb_extension = ".xnb";
                private const string _c_spritefont_extension = ".xnb";

                public QS.Fx.Xna.IContent _GetContent(QS.Fx.Xna.IContentRef _contentref, string _id)
                {
                    switch (_contentref.ContentClass)
                    {
                        case QS.Fx.Xna.ContentClass.Texture2D:
                            {
                                Stream _s = this._underlyingnamespace._Content(_id).Stream;
                                object _o = Texture2D.FromStream(this._namespace._controller._graphicsservice.GraphicsDevice, _s);
                                
                                return new Content_(_contentref, _o);
                            }

                        case QS.Fx.Xna.ContentClass.Model:
                        case QS.Fx.Xna.ContentClass.Texture3D:
                            {
                                if (_id.EndsWith(_c_xnb_extension))
                                {
                                    string _m_id_ = _id.Substring(0, _id.Length - _c_xnb_extension.Length);
                                    object _m_o;
                                    switch (_contentref.ContentClass)
                                    {
                                        case QS.Fx.Xna.ContentClass.Model:
                                            _m_o = this.Load<Model>(_m_id_);
                                            break;

                                        case QS.Fx.Xna.ContentClass.Texture3D:
                                            _m_o = this.Load<Texture3D>(_m_id_);
                                            break;

                                        default:
                                            throw new NotImplementedException();
                                    }
                                    return new Content_(_contentref, _m_o);
                                }
                                else
                                    throw new Exception("Content of this type must be stored in files with the \"" + _c_xnb_extension + "\" extension.");
                            }
                        case QS.Fx.Xna.ContentClass.SpriteFont:
                            {
                                if (_id.EndsWith(_c_spritefont_extension))
                                {
                                    string _sf_id_ = _id.Substring(0, _id.Length - _c_spritefont_extension.Length);
                                    object _sf_o = this.Load<SpriteFont>(_sf_id_);
                                    return new Content_(_contentref, _sf_o);
                                }
                                else
                                    throw new Exception("Content of this type must be stored in files with the \"" + _c_spritefont_extension + "\" extension.");
                            }
                        default:
                            throw new NotImplementedException();
                    }
                }

                #endregion

                #region Internals

                protected override Stream OpenStream(string _assetname)
                {
                    return this._underlyingnamespace._Content(_assetname + ".xnb").Stream;
                }

                #endregion
            }

            #endregion

            #region _GetContent

            public QS.Fx.Xna.IContent _GetContent(QS.Fx.Xna.IContentRef _contentref, ulong _incarnation, string _id)
            {
                lock (this)
                {
                    Incarnation_ _o;
                    if ((this._newestincarnation != null) && (this._newestincarnation._Incarnation >= _incarnation))
                        _o = this._newestincarnation;
                    else
                    {
                        QS._qss_x_.Content_.INamespace_ _ns = this._controller._library._Namespace(this._id, _incarnation);
                        _o = new Incarnation_(this, _ns);
                        this._incarnations.Add(_o._Incarnation, _o);
                        this._newestincarnation = _o;
                        if (_o._Incarnation < _incarnation)
                            throw new Exception("The newest version of content library \"" + this._id.ToString() +
                                "\" that could be found is " + _o._Incarnation.ToString() + 
                                ", which is a lower version than the requested " + _incarnation.ToString() + ".");
                    }
                    return _o._GetContent(_contentref, _id);
                }
            }

            #endregion
        }

        #endregion

        #region IController_ Members

        QS.Fx.Xna.IContent IController_._GetContent(QS.Fx.Xna.IContentRef _contentref)
        {
            try
            {
                string _fqid = _contentref.ID;
                int _i = _fqid.IndexOf(':');
                QS.Fx.Base.ID _namespace_id = QS.Fx.Base.ID._0;
                ulong _namespace_incarnation = 0;
                if (_i > 0)
                {
                    int _j = _fqid.IndexOf('`', 0, _i);
                    if (_j > 0)
                        _namespace_id = new QS.Fx.Base.ID(_fqid.Substring(0, _j));
                    if (_j < (_i - 1))
                        _namespace_incarnation = Convert.ToUInt64(_fqid.Substring(_j + 1, _i - _j - 1));
                }
                string _id = _fqid.Substring(_i + 1);
                lock (this)
                {
                    Namespace_ _namespace;
                    if (!this._namespaces.TryGetValue(_namespace_id, out _namespace))
                    {
                        _namespace = new Namespace_(this, _namespace_id);
                        _namespaces.Add(_namespace_id, _namespace);
                    }
                    return _namespace._GetContent(_contentref, _namespace_incarnation, _id);
                }
            }
            catch (Exception _exc)
            {
                throw new Exception("Could not load content \"" + _contentref.ID + "\" of type \"" + 
                    _contentref.ContentClass.ToString() + "\".", _exc);
            }
        }

        #endregion
    }
}

#endif

/*
        QS.Fx.Xna.IContent IContentLibrary_._GetContent(QS.Fx.Xna.IContentRef _contentref)
        {
            lock (this)
            {
                    string _namespace_root = this._root + Path.DirectorySeparatorChar + _namespace_id.ToString();
                    string _namespace_metadata = _namespace_root + Path.DirectorySeparatorChar + "metadata.xml";
                    if (Directory.Exists(_namespace_root) && File.Exists(_namespace_metadata))
                    {
                        Metadata_ _metadata;
                        using (StreamReader _reader = new StreamReader(_namespace_metadata))
                        {
                            _metadata = (Metadata_)(new XmlSerializer(typeof(Metadata_))).Deserialize(_reader);
                        }
                        if (!_metadata._ID.Equals(_namespace_id))
                            throw new Exception("Corrupt metadata in content library \"" + _namespace_id.ToString() + "\".");
                        _namespace = new Namespace_(_metadata);
                        _namespaces.Add(_namespace);
                    }
                    else
                        throw new Exception("Could not locate content library \"" + _namespace_id.ToString() + "\".");
                }
                if (_namespace._Metadata._Incarnation < _namespace_incarnation)
                    throw new Exception("The installed version of content library \"" + _namespace_id.ToString() + "\" is " +
                        _namespace._Metadata._Incarnation.ToString() + ", which is older than the version " + 
                        _namespace_incarnation + " that has been requested."); 
                Metadata_._Item _item;
                if (!_namespace._Metadata.Lookup(_id, out _item))
                    throw new Exception("Could not locate component \"" + _id.ToString() + "\" in content library \"" +
                        _namespace_id.ToString() + "\".");
                if (_item._Incarnation < _incarnation)
                    throw new Exception("The installed version of component \"" _id.ToString() + "\" in library \"" + 
                        _namespace_id.ToString() + "\" is " + _item._Incarnation.ToString() + ", which is older than the version " +
                        _incarnation.ToString() + " that has been requested."); 
                if (_item.Class != _contentref.ContentClass)
                    throw new Exception("Content \"" + _contentref.ID + "\" installed on this machine is of class \"" + _item.Class.ToString() +
                        "\", but it was expected to be of class \"" + _contentref.ContentClass.ToString() + "\".");
                string _content_filename = _namespace_root + Path.DirectorySeparatorChar + _item.Filename;
                object _content;
                string _filename_extension = Path.GetExtension(_content_filename);
                if (_filename_extension.Equals(".xnb"))
                {
                    _content_filename = _content_filename.Substring(0, _content_filename.LastIndexOf(".xnb"));
                    switch (_item.Class)
                    {
                        case QS.Fx.Xna.ContentClass.Model:
                            _content = _content_manager.Load<Model>(_content_filename);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
                else if (_filename_extension.Equals(".jpg"))
                {
                    switch (_item.Class)
                    {
                        case QS.Fx.Xna.ContentClass.Texture2D:
                            _content = Texture2D.FromFile(this._graphics.GraphicsDevice, _content_filename);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                    throw new Exception("Unknown file type \"" + _filename_extension + "\".");
                return new QS._qss_x_.Xna_.Content(_contentref.ContentClass, _contentref.ID, _content, null);


            }
*/
