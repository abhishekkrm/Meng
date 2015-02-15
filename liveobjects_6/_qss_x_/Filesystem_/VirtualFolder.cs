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
    [Base1_.SynchronizationClass(Base1_.SynchronizationOption.Reentrant | Base1_.SynchronizationOption.Asynchronous)]
    public sealed class VirtualFolder : FilesystemObject, QS.Fx.Filesystem.IFolder, IVirtualFolder
    {
        public VirtualFolder(string name, QS.Fx.Logging.ILogger logger, IVirtualDisk disk) : base(name)
        {
            this.logger = logger;
            this.disk = disk;
        }

        private IDictionary<string, IVirtualFolder> folders = new Dictionary<string, IVirtualFolder>();
        private IDictionary<string, IVirtualFile> files = new Dictionary<string, IVirtualFile>();
        private QS.Fx.Logging.ILogger logger;
        private IVirtualDisk disk;

        #region Inspection

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Inspection.IAttributeCollection _subfolders
        {
            get 
            {
                QS.Fx.Inspection.AttributeCollection result = new QS.Fx.Inspection.AttributeCollection("_subfolders");
                foreach (IVirtualFolder subfolder in folders.Values)
                {
                    if (subfolder is QS.Fx.Inspection.IAttribute)
                        result.Add((QS.Fx.Inspection.IAttribute) subfolder);
                }
                return result;
            }
        }

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Inspection.IAttributeCollection _files
        {
            get
            {
                QS.Fx.Inspection.AttributeCollection result = new QS.Fx.Inspection.AttributeCollection("_files");
                foreach (IVirtualFile file in files.Values)
                {
                    if (file is QS.Fx.Inspection.IAttribute)
                        result.Add((QS.Fx.Inspection.IAttribute) file);

                }                
                return result;
            }
        }

        #endregion

        #region IFilesystemObject Members

        QS.Fx.Filesystem.FilesystemObjectType QS.Fx.Filesystem.IFilesystemObject.Type
        {
            get { return QS.Fx.Filesystem.FilesystemObjectType.Folder; }
        }

        #endregion

        #region IFolder Members

        IEnumerable<string> QS.Fx.Filesystem.IFolder.Folders
        {
            get 
            {
                lock (this)
                {
                    return new List<string>(folders.Keys);
                }
            }
        }

        IEnumerable<string> QS.Fx.Filesystem.IFolder.Files
        {
            get 
            {
                lock (this)
                {
                    return new List<string>(files.Keys);
                }
            }
        }

        QS.Fx.Filesystem.IFolder QS.Fx.Filesystem.IFolder.OpenFolder(string name)
        {
            IVirtualFolder folder;
            lock (this)
            {
                if (!folders.TryGetValue(name, out folder))
                    throw new Exception("Cannot open folder \"" + name + "\" because it does not exist.");
            }
            return folder;
        }

        QS.Fx.Filesystem.IFile QS.Fx.Filesystem.IFolder.OpenFile(string name, System.IO.FileMode mode)
        {
            return ((QS.Fx.Filesystem.IFolder)this).OpenFile(name, mode, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None, QS.Fx.Filesystem.FileFlags.None);
        }

        QS.Fx.Filesystem.IFile QS.Fx.Filesystem.IFolder.OpenFile(string name, System.IO.FileMode mode, System.IO.FileAccess access)
        {
            return ((QS.Fx.Filesystem.IFolder)this).OpenFile(name, mode, access, System.IO.FileShare.None, QS.Fx.Filesystem.FileFlags.None);
        }

        QS.Fx.Filesystem.IFile QS.Fx.Filesystem.IFolder.OpenFile(string name, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share)
        {
            return ((QS.Fx.Filesystem.IFolder)this).OpenFile(name, mode, access, share, QS.Fx.Filesystem.FileFlags.None);
        }

        QS.Fx.Filesystem.IFile QS.Fx.Filesystem.IFolder.OpenFile(string name, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share, QS.Fx.Filesystem.FileFlags flags)
        {
            IVirtualFile file;
            lock (this)
            {
                bool already_exists = files.TryGetValue(name, out file);

                switch (mode)
                {
                    case System.IO.FileMode.CreateNew:
                    {
                        if (already_exists)
                            throw new Exception("Cannot create file \"" + name + "\" because it already exists.");

                        file = new VirtualFile(name, logger, disk);
                        files.Add(name, file);
                    }
                    break;

                    case System.IO.FileMode.Create:
                    {
                        if (already_exists)
                            file.Truncate();
                        else
                        {
                            file = new VirtualFile(name, logger, disk);
                            files.Add(name, file);
                        }
                    }
                    break;

                    case System.IO.FileMode.Open:
                    {
                        if (!already_exists)
                            throw new Exception("Cannot open file \"" + name + "\" because it does not exist.");              
                    }
                    break;

                    case System.IO.FileMode.OpenOrCreate:
                    {
                        if (!already_exists)
                        {
                            file = new VirtualFile(name, logger, disk);
                            files.Add(name, file);
                        }                        
                    }
                    break;

                    case System.IO.FileMode.Truncate:
                    {
                        if (!already_exists)
                            throw new Exception("Cannot open file \"" + name + "\" because it does not exist.");
                        file.Truncate();
                    }
                    break;
                    
                    case System.IO.FileMode.Append:
                        throw new NotImplementedException();
                }
            }

            return file.OpenRef(access, share, flags);
        }

        void QS.Fx.Filesystem.IFolder.CreateFolder(string name)
        {
            lock (this)
            {
                if (folders.ContainsKey(name))
                    throw new Exception("Cannot create folder \"" + name + "\" because a folder with this name already exists.");
                else
                    folders.Add(name, new VirtualFolder(name, logger, disk));
            }
        }

        void QS.Fx.Filesystem.IFolder.CreateFile(string name)
        {
            this._CreateFile(name); // , System.IO.FileOptions.None);
        }

        private void _CreateFile(string name) // , System.IO.FileOptions options)
        {
            lock (this)
            {
                if (files.ContainsKey(name))
                    throw new Exception("Cannot create file \"" + name + "\" because a file with this name already exists.");
                else
                    files.Add(name, new VirtualFile(name, logger, disk)); // , options));
            }
        }

        bool QS.Fx.Filesystem.IFolder.FolderExists(string name)
        {
            lock (this)
            {
                return folders.ContainsKey(name);
            }
        }

        bool QS.Fx.Filesystem.IFolder.FileExists(string name)
        {
            lock (this)
            {
                return files.ContainsKey(name);
            }
        }

        void QS.Fx.Filesystem.IFolder.DeleteFolder(string name)
        {
            ((QS.Fx.Filesystem.IFolder)this).DeleteFolder(name, false);
        }

        void QS.Fx.Filesystem.IFolder.DeleteFolder(string name, bool recursive)
        {
            lock (this)
            {
                IVirtualFolder folder;
                if (!folders.TryGetValue(name, out folder))
                    throw new Exception("Cannot delete folder \"" + name + "\" because it does not exist.");

                if (!folder.IsEmpty)
                    throw new Exception("Cannot delete folder \"" + name + "\" because it is not empty.");

                folders.Remove(name);
            }
        }

        void QS.Fx.Filesystem.IFolder.DeleteFile(string name)
        {
            lock (this)
            {
                IVirtualFile file;
                if (!files.TryGetValue(name, out file))
                    throw new Exception("Cannot delete file \"" + name + "\" because it does not exist.");

                if (file.IsOpened)
                    throw new Exception("Cannot delete file \"" + name + "\" because it is currently opened.");

                files.Remove(name);
            }
        }

        void QS.Fx.Filesystem.IFolder.RenameFolder(string oldname, string newname)
        {
            lock (this)
            {
                IVirtualFolder folder;
                if (!folders.TryGetValue(oldname, out folder))
                    throw new Exception("Cannot rename folder \"" + oldname + "\" because it does not exist.");

                if (folders.ContainsKey(newname))
                    throw new Exception(
                        "Cannot rename folder \"" + oldname + "\" to \"" + newname + "\" because a folder with such name already exists.");

                folder.Rename(newname);

                folders.Remove(oldname);
                folders.Add(newname, folder);
            }
        }

        void QS.Fx.Filesystem.IFolder.RenameFile(string oldname, string newname)
        {
            lock (this)
            {
                IVirtualFile file;
                if (!files.TryGetValue(oldname, out file))
                    throw new Exception("Cannot rename file \"" + oldname + "\" because it does not exist.");

                if (files.ContainsKey(newname))
                    throw new Exception(
                        "Cannot rename file \"" + oldname + "\" to \"" + newname + "\" because a file with such name already exists.");

                file.Rename(newname);

                files.Remove(oldname);
                files.Add(newname, file);
            }
        }

        #endregion

        #region IVirtualFolder Members

        bool IVirtualFolder.IsEmpty
        {
            get { return (folders.Count + files.Count) == 0; }
        }

        void IVirtualFolder.Rename(string newname)
        {
            this.name = newname;
        }

        void IVirtualFolder.Reset()
        {
            lock (this)
            {
                foreach (IVirtualFile file in files.Values)
                    file.Reset();

                foreach (IVirtualFolder folder in folders.Values)
                    folder.Reset();
            }
        }

        #endregion
    }
}
