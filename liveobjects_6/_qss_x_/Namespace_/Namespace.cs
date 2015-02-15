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

namespace QS._qss_x_.Namespace_
{
    public sealed class Namespace : INamespace, INamespaceControl, IDisposable
    {
        public const ulong RootID = 1;
        public const string RootName = "My Channels";

        #region Constructor

        public Namespace(QS.Fx.Platform.IPlatform platform)
        {
            this.platform = platform;
            root = new Folder(RootID, RootName);
            objects.Add(RootID, root);
            lastid = RootID;

            // some bogus initialization code

/*
            Folder f, f2, f3;
            TestObject o;

            f = new Folder(++lastid, "Cornell University");
            objects.Add(((IObject) f).Identifier, f);
            root.Add(f);

            o = new TestObject(++lastid, "News", Category.Topic, new string[] { "View", "Save" }, platform.Logger);
            objects.Add(((IObject) o).Identifier, o);
            f.Add(o);

            o = new TestObject(++lastid, "Violent Game", Category.Video,
                new string[] { "Play" }, platform.Logger);
            objects.Add(((IObject)o).Identifier, o);
            f.Add(o);

            f2 = new Folder(++lastid, "Cornell University Computer Science");
            objects.Add(((IObject) f2).Identifier, f2);
            f.Add(f2);

            o = new TestObject(++lastid, "Espresso Machine Training Channel", Category.Video,
                new string[] { "Watch", "Record" }, platform.Logger);
            objects.Add(((IObject)o).Identifier, o);
            f2.Add(o);

            o = new TestObject(++lastid, "Visual Studio 2005", Category.File, new string[] { "Download", "Mount" }, platform.Logger);
            objects.Add(((IObject)o).Identifier, o);
            f2.Add(o);

            f3 = new Folder(++lastid, "Krzys's Offerings");
            objects.Add(((IObject) f3).Identifier, f3);
            f2.Add(f3);

            o = new TestObject(++lastid, "Krzys's Music Compilation for Today", Category.Music, 
                new string[] { "Listen", "Record" }, platform.Logger);
            objects.Add(((IObject)o).Identifier, o);
            f3.Add(o);

            o = new TestObject(++lastid, "Experiment Results", Category.Topic,
                new string[] { "Listen", "Record" }, platform.Logger);
            objects.Add(((IObject)o).Identifier, o);
            f3.Add(o);

            o = new TestObject(++lastid, "Krzys's Weblog", Category.Document, new string[] { "View", "Edit" }, platform.Logger);
            objects.Add(((IObject)o).Identifier, o);
            f3.Add(o);

            o = new TestObject(++lastid, "Cluster Administration", Category.Connection, new string[] { "Administer" }, platform.Logger);
            objects.Add(((IObject)o).Identifier, o);
            f3.Add(o);
*/ 
        }

        #endregion

        private QS.Fx.Platform.IPlatform platform;
        private Folder root;
        private ulong lastid;
        private IDictionary<ulong, IObject> objects = new Dictionary<ulong, IObject>();

        #region INamespace Members

        IFolder INamespace.Root
        {
            get { return root; }
        }

        bool INamespace.Lookup(ulong identifier, out IObject obj)
        {
            lock (this)
            {
                if (objects.TryGetValue(identifier, out obj))
                    return true;
                else
                {
                    obj = null;
                    return false;
                }
            }
        }

        ulong INamespace.NewIdentifier
        {
            get
            {
                lock (this)
                {
                    return ++lastid;
                }
            }
        }

        NewIdentifierCallback INamespace.NewIdentifierCallback
        {
            get { return new NewIdentifierCallback(this._NewIdentifier); }
        }

        private ulong _NewIdentifier()
        {
            lock (this)
            {
                return ++lastid;
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region INamespaceControl Members

        IFolderControl INamespaceControl.RootControl
        {
            get { return root; }
        }

        void INamespaceControl.Register(IObject obj)
        {
            lock (this)
            {
                objects.Add(obj.Identifier, obj);
            }
        }

        void INamespaceControl.Unregister(IObject obj)
        {
            lock (this)
            {
                objects.Remove(obj.Identifier);
            }
        }

        #endregion
    }
}
