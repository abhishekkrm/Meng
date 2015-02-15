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
    public sealed class PhysicalFolder : FilesystemObject, QS.Fx.Filesystem.IFolder
    {
        public PhysicalFolder(QS._core_c_.Core.IFileController fileController, string physical_path, string name) : base(name)
        {
            this.fileController = fileController;
            this.physical_path = physical_path;
        }

        private const int DefaultBufferSizeForCreateFile = 8192;

        private QS._core_c_.Core.IFileController fileController;
        private string physical_path;

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
                string[] filepaths = System.IO.Directory.GetDirectories(physical_path);
                string[] files = new string[filepaths.Length];
                for (int ind = 0; ind < filepaths.Length; ind++)
                    files[ind] = filepaths[ind].Substring(filepaths[ind].LastIndexOf('\\') + 1);
                return files;
            }
        }

        IEnumerable<string> QS.Fx.Filesystem.IFolder.Files
        {
            get 
            {
                string[] filepaths = System.IO.Directory.GetFiles(physical_path);
                string[] files = new string[filepaths.Length];
                for (int ind = 0; ind < filepaths.Length; ind++)
                    files[ind] = filepaths[ind].Substring(filepaths[ind].LastIndexOf('\\') + 1);
                return files;
            }
        }

        QS.Fx.Filesystem.IFolder QS.Fx.Filesystem.IFolder.OpenFolder(string name)
        {
            string subfolder_path = physical_path + "\\" + name;
            if (System.IO.Directory.Exists(subfolder_path))
                return new PhysicalFolder(fileController, subfolder_path, name);
            else
                throw new Exception("Cannot open folder \"" + name + "\" because it does not exist.");
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
            string file_path = physical_path + "\\" + name;
            try
            {
                QS._core_c_.Core.FileFlagsAndAttributes coreflags = QS._core_c_.Core.FileFlagsAndAttributes.None;
                if ((flags & QS.Fx.Filesystem.FileFlags.WriteThrough) == QS.Fx.Filesystem.FileFlags.WriteThrough)
                    coreflags |= QS._core_c_.Core.FileFlagsAndAttributes.WriteThrough;

                return new PhysicalFile(fileController.OpenFile(file_path, mode, access, share, coreflags), file_path, name);
            }
            catch (Exception exc)
            {
                throw new Exception("Cannot open file \"" + name + "\", physical operation failed.", exc);
            }
        }

        void QS.Fx.Filesystem.IFolder.CreateFolder(string name)
        {
            string subfolder_path = physical_path + "\\" + name;
            if (!System.IO.Directory.Exists(subfolder_path))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(subfolder_path);
                }
                catch (Exception exc)
                {
                    throw new Exception("Cannot create folder \"" + name + "\", the physical operation failed.", exc);
                }
            }
            else
                throw new Exception("Cannot create folder \"" + name + "\" because a folder with this name already exists.");
        }

        void QS.Fx.Filesystem.IFolder.CreateFile(string name)
        {
            this._CreateFile(name); // , System.IO.FileOptions.None);
        }

        private void _CreateFile(string name) // , System.IO.FileOptions options)
        {
            string file_path = physical_path + "\\" + name;
            if (!System.IO.File.Exists(file_path))
            {
                try
                {
                    System.IO.File.Create(file_path, DefaultBufferSizeForCreateFile); // , options);
                }
                catch (Exception exc)
                {
                    throw new Exception("Cannot create file \"" + name + "\", the physical operation failed.", exc);
                }
            }
            else
                throw new Exception("Cannot create file \"" + name + "\" because a file with this name already exists.");
        }

        bool QS.Fx.Filesystem.IFolder.FolderExists(string name)
        {
            string subfolder_path = physical_path + "\\" + name;
            return System.IO.Directory.Exists(subfolder_path);
        }

        bool QS.Fx.Filesystem.IFolder.FileExists(string name)
        {
            string file_path = physical_path + "\\" + name;
            return System.IO.File.Exists(file_path);
        }

        void QS.Fx.Filesystem.IFolder.DeleteFolder(string name)
        {
            ((QS.Fx.Filesystem.IFolder)this).DeleteFolder(name, false);
        }

        void QS.Fx.Filesystem.IFolder.DeleteFolder(string name, bool recursive)
        {
            string subfolder_path = physical_path + "\\" + name;
            if (System.IO.Directory.Exists(subfolder_path))
            {
                try
                {
                    System.IO.Directory.Delete(subfolder_path, recursive);
                }
                catch (Exception exc)
                {
                    throw new Exception("Cannot delete folder \"" + name + "\", the physical operation failed.", exc);
                }
            }
            else
                throw new Exception("Cannot delete folder \"" + name + "\" because it does not exist.");
        }

        void QS.Fx.Filesystem.IFolder.DeleteFile(string name)
        {
            string file_path = physical_path + "\\" + name;
            if (System.IO.File.Exists(file_path))
            {
                try
                {
                    System.IO.File.Delete(file_path);
                }
                catch (Exception exc)
                {
                    throw new Exception("Cannot delete file \"" + name + "\", the physical operation failed.", exc);
                }
            }
            else
                throw new Exception("Cannot delete file \"" + name + "\" because it does not exist.");
        }

        void QS.Fx.Filesystem.IFolder.RenameFolder(string oldname, string newname)
        {
            string old_subfolder_path = physical_path + "\\" + oldname;
            if (System.IO.Directory.Exists(old_subfolder_path))
            {
                string new_subfolder_path = physical_path + "\\" + newname;
                if (!System.IO.Directory.Exists(new_subfolder_path))
                {
                    try
                    {
                        System.IO.Directory.Move(old_subfolder_path, new_subfolder_path);
                    }
                    catch (Exception exc)
                    {
                        throw new Exception("Cannot rename folder \"" + oldname + "\" to \"" + newname + "\", the physical operation failed.", exc);
                    }
                }
                else
                    throw new Exception(
                        "Cannot assign folder \"" + oldname + "\" a new name \"" + newname + "\" because a folder with such name already exists.");
            }
            else
                throw new Exception("Cannot rename folder \"" + oldname + "\" because it does not exist.");
        }

        void QS.Fx.Filesystem.IFolder.RenameFile(string oldname, string newname)
        {
            string old_file_path = physical_path + "\\" + oldname;
            if (System.IO.File.Exists(old_file_path))
            {
                string new_file_path = physical_path + "\\" + newname;
                if (!System.IO.File.Exists(new_file_path))
                {
                    try
                    {
                        System.IO.File.Move(old_file_path, new_file_path);
                    }
                    catch (Exception exc)
                    {
                        throw new Exception("Cannot rename file \"" + oldname + "\" to \"" + newname + "\", the physical operation failed.", exc);
                    }
                }
                else
                    throw new Exception(
                        "Cannot assign file \"" + oldname + "\" a new name \"" + newname +  "\" because a file with such name already exists.");
            }
            else
                throw new Exception("Cannot rename file \"" + oldname + "\" because it does not exist.");
        }

        #endregion
    }
}
