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

#if XNA

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;

namespace QS._qss_x_.Content_
{
    public sealed class Controller_ : IController_
    {
        #region Static

        // Optional components to load should be in folder ..\content, relative to the executable.
        private static readonly string _CONTENT_ROOT =
            QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_ + Path.DirectorySeparatorChar + "content";

        static Controller_()
        {
            _localcontroller = new Controller_(_CONTENT_ROOT);
        }

        public static IController_ _Local
        {
            get
            {
                return _localcontroller;
            }
        }

        private static Controller_ _localcontroller;

        #endregion

        #region Constructor

        private Controller_(string _root)
        {
            this._root = _root;
            this._namespaces = new Dictionary<QS.Fx.Base.ID, Namespace_>();
        }

        #endregion

        #region Fields

        private string _root;
        private IDictionary<QS.Fx.Base.ID, Namespace_> _namespaces = new Dictionary<QS.Fx.Base.ID, Namespace_>();

        #endregion

        #region Class Namespace_

        public sealed class Namespace_
        {
            #region Constructor

            public Namespace_(Controller_ _library, QS.Fx.Base.ID _namespace_id)
            {
                this._library = _library;
                this._namespace_id = _namespace_id;
                this._incarnations = new Dictionary<ulong, Incarnation_>();
                this._root = Path.GetFullPath(Path.Combine(this._library._root, this._namespace_id.ToString()));
            }

            #endregion

            #region Fields

            private Controller_ _library;
            private QS.Fx.Base.ID _namespace_id;
            private IDictionary<ulong, Incarnation_> _incarnations;
            private string _root;
            private Incarnation_ _newestincarnation;

            #endregion

            #region Accessors

            public QS.Fx.Base.ID _ID
            {
                get { return this._namespace_id; }
            }

            #endregion

            #region Class Incarnation_

            public sealed class Incarnation_ : INamespace_
            {
                #region Constructor

                public Incarnation_(Namespace_ _namespace, ulong _namespace_incarnation)
                {
                    this._namespace = _namespace;
                    this._namespace_incarnation = _namespace_incarnation;
                    this._root = Path.GetFullPath(Path.Combine(this._namespace._root, this._namespace_incarnation.ToString()));
                    this._dataroot = Path.GetFullPath(Path.Combine(this._root, "data"));
                    if (!Directory.Exists(this._root))
                    {
                        // TODO: might download...............................................................................................................................
                        throw new Exception("Content library \"" +
                            this._namespace._namespace_id.ToString() + "`" + this._namespace_incarnation.ToString() + "\" is not installed.");
                    }
                    string _metadatafile = Path.Combine(this._root, "metadata.xml");
                    if (!File.Exists(_metadatafile))
                        throw new Exception("Missing metadata for content library \"" + 
                            this._namespace._namespace_id.ToString() + "`" + this._namespace_incarnation.ToString() + "\".");
                    Metadata_ _metadata;
                    using (StreamReader _reader = new StreamReader(_metadatafile))
                    {
                        _metadata = (Metadata_) (new XmlSerializer(typeof(Metadata_))).Deserialize(_reader);
                    }
                    string _metadata_fqid = _metadata._ID;
                    QS.Fx.Base.ID _metadata_id;
                    ulong _metadata_incarnation;
                    int p = _metadata_fqid.IndexOf('`');
                    if (p >= 0 && p < _metadata_fqid.Length)
                    {
                        _metadata_id = (p > 0) ? new QS.Fx.Base.ID(_metadata_fqid.Substring(0, p)) : QS.Fx.Base.ID._0;
                        _metadata_incarnation = (p < (_metadata_fqid.Length - 1)) ? Convert.ToUInt64(_metadata_fqid.Substring(p + 1)) : 0;
                    }
                    else
                    {
                        _metadata_id = new QS.Fx.Base.ID(_metadata_fqid);
                        _metadata_incarnation = 0;
                    }
                    if (!_metadata_id.Equals(_namespace._namespace_id) || !_metadata_incarnation.Equals(_namespace_incarnation))
                        throw new Exception("Corrupt metadata in content library \"" +
                            this._namespace._namespace_id.ToString() + "`" + this._namespace_incarnation.ToString() + "\".");
                }

                #endregion

                #region Fields

                private Namespace_ _namespace;
                private ulong _namespace_incarnation;
                private string _root, _dataroot;

                #endregion

                #region Accessors

                public ulong _Incarnation
                {
                    get { return this._namespace_incarnation; }
                }

                #endregion

                #region Class Metadata_

                [XmlType("Library")]
                public sealed class Metadata_
                {
                    public Metadata_()
                    {
                    }

                    private string _id;

                    [XmlAttribute("id")]
                    public string _ID
                    {
                        get { return this._id; }
                        set { this._id = value; }
                    }
                }

                #endregion

                #region Class Content_

                private sealed class Content_ : IContent_
                {
                    #region Constructor

                    public Content_(Incarnation_ _incarnation, string _id, string _filename)
                    {
                        this._incarnation = _incarnation;
                        this._id = _id;
                        this._filename = _filename;
                    }

                    #endregion

                    #region Fields

                    private Incarnation_ _incarnation;
                    private string _id, _filename;

                    #endregion

                    #region IContent_ Members

                    string IContent_.ID
                    {
                        get 
                        {
                            return this._incarnation._namespace._namespace_id.ToString() + "`" +
                                this._incarnation._namespace_incarnation.ToString() + ":" + this._id;
                        }
                    }

                    Stream IContent_.Stream
                    {
                        get { return new FileStream(this._filename, FileMode.Open, FileAccess.Read, FileShare.Read); }
                    }

                    #endregion

                    #region IDisposable Members

                    void IDisposable.Dispose()
                    {
                    }

                    #endregion
                }

                #endregion

                #region INamespace_ Members

                QS.Fx.Base.ID INamespace_._ID
                {
                    get { return this._namespace._namespace_id; }
                }

                ulong INamespace_._Incarnation
                {
                    get { return this._namespace_incarnation; }
                }

                IContent_ INamespace_._Content(string _id)
                {
                    lock (this)
                    {
                        string _filename = Path.GetFullPath(Path.Combine(this._dataroot, _id));
                        if (_filename.Contains("..") || !_filename.StartsWith(this._dataroot))
                            throw new Exception("Content identifier \"" + _id + "\" is illegal because it leads outside of the content library folder.");
                        if (!File.Exists(_filename))
                            throw new Exception("Content library \"" + this._namespace._namespace_id.ToString() + "`" +
                                this._namespace_incarnation.ToString() + "\" does not contain any content named \"" + _id + "\".");
                        return new Content_(this, _id, _filename);
                    }
                }

                #endregion

                #region IDisposable Members

                void IDisposable.Dispose()
                {
                }

                #endregion
            }

            #endregion

            #region _Namespace

            public INamespace_ _Namespace(ulong _namespace_incarnation)
            {
                lock (this)
                {
                    Incarnation_ _incarnation;
                    if ((_newestincarnation != null) && (_newestincarnation._Incarnation >= _namespace_incarnation))
                        _incarnation = _newestincarnation;
                    else
                    {
                        if (!_incarnations.TryGetValue(_namespace_incarnation, out _incarnation))
                        {
                            ulong _new_namespace_incarnation = 0;
                            bool _found = false;
                            foreach (string _foldername in Directory.GetDirectories(this._root, "*", SearchOption.TopDirectoryOnly))
                            {
                                string _s = _foldername.Substring(_foldername.LastIndexOfAny(new char[] { '\\', '/' }) + 1);
                                try
                                {
                                    _new_namespace_incarnation = Convert.ToUInt64(_s);
                                    _found = true;
                                }
                                catch (Exception)
                                {
                                }
                            }
                            if (!_found)
                            {
                                // TODO: ........might need to load here......
                                throw new Exception("Content library \"" + _namespace_id.ToString() + "\" is not installed on this machine.");
                            }
                            _incarnation = new Incarnation_(this, _new_namespace_incarnation);
                            _incarnations.Add(_new_namespace_incarnation, _incarnation);
                            _newestincarnation = _incarnation;
                        }
                    }
                    return _incarnation;
                }
            }

            #endregion
        }

        #endregion

        #region IController_ Members

        INamespace_ IController_._Namespace(QS.Fx.Base.ID _namespace_id, ulong _namespace_incarnation)
        {
            lock (this)
            {
                Namespace_ _namespace;
                if (!_namespaces.TryGetValue(_namespace_id, out _namespace))
                {
                    string _foldername = Path.GetFullPath(Path.Combine(this._root, _namespace_id.ToString()));
                    if (!Directory.Exists(_foldername))
                    {
                        // ......should downlod or something
                        throw new Exception("Content library \"" + _namespace_id.ToString() + "\" is not installed on this machine.");
                    }
                    _namespace = new Namespace_(this, _namespace_id);
                    _namespaces.Add(_namespace_id, _namespace);
                }
                return _namespace._Namespace(_namespace_incarnation);
            }
        }

        #endregion
    }
}

#endif
