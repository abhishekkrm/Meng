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

namespace QS._qss_x_.Filesystem_
{
    public abstract class FilesystemObject : QS.Fx.Inspection.Inspectable, QS.Fx.Filesystem.IFilesystemObject
    {
        protected FilesystemObject(string name)
        {
            this.name = name;
        }

/*
        protected IFilesystem filesystem;
        protected IFolder container;
*/

        [QS.Fx.Base.Inspectable]
        protected string name;

        #region IFilesystemObject Members

        string QS.Fx.Filesystem.IFilesystemObject.Name
        {
            get { return name; }
        }

        QS.Fx.Filesystem.FilesystemObjectType QS.Fx.Filesystem.IFilesystemObject.Type
        {
            get { throw new NotSupportedException(); }
        }

/*
        IFolder IFilesystemObject.Container
        {
            get { return container; }
        }

        string IFilesystemObject.Path
        {
            get
            {
                string containerpath = (container != null) ? ((IFilesystemObject)container).Path : "\\";
                StringBuilder builder = new StringBuilder(containerpath.Length + 1 + name.Length);
                builder.Append(containerpath);
                builder.Append("\\");
                builder.Append(name);
                return builder.ToString();
            }
        }
*/

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
