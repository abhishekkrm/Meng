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
    public sealed class Folder : Object, IFolder, IFolderControl
    {
        public Folder(ulong identifier, string name) : base(identifier, name)
        {
        }

        private IDictionary<ulong, IObject> objects = new Dictionary<ulong, IObject>();

        #region Operations on the folder

        public bool Add(IObject obj)
        {
            if (objects.ContainsKey(obj.Identifier))
                return false;
            else
            {
                objects.Add(obj.Identifier, obj);
                return true;
            }
        }

        public bool Remove(ulong identifier)
        {
            return objects.Remove(identifier);
        }

        public bool Get(ulong identifier, out IObject obj)
        {
            if (objects.TryGetValue(identifier, out obj))
                return true;
            else
            {
                obj = null;
                return false;
            }
        }

        #endregion

        #region IObject Members

        bool IObject.IsFolder
        {
            get { return true; }
        }

        Category IObject.Category
        {
            get { return Category.Folder; }
        }

        #endregion

        #region IFolder Members

        bool IFolder.HasObjects
        {
            get
            {
                return objects.Count > 0;
            }
        }

        IEnumerable<IObject> IFolder.Objects
        {
            get 
            {
                return new List<IObject>(objects.Values);
            }
        }

        #endregion

        #region IFolderControl Members

        bool IFolderControl.Add(IObject obj)
        {
            return Add(obj);
        }

        bool IFolderControl.Remove(ulong identifier)
        {
            return Remove(identifier);
        }

        bool IFolderControl.Get(ulong identifier, out IObject obj)
        {
            return Get(identifier, out obj);
        }

        #endregion
    }
}
