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

namespace QS.Fx.Filesystem
{
    /// <summary>
    /// This interface represents a folder in a virtualized filesystem.
    /// </summary>
    public interface IFolder : IFilesystemObject
    {
        /// <summary>
        /// Returns the names of subfolders in this folder.
        /// </summary>
        IEnumerable<string> Folders { get; }

        /// <summary>
        /// Returns the names of files in this folder.
        /// </summary>
        IEnumerable<string> Files { get; }

        /// <summary>
        /// Opens the subfolder with the given name.
        /// </summary>
        /// <param name="name">The name of the subfolder to open.</param>
        /// <returns>The subfolder.</returns>
        IFolder OpenFolder(string name);

        /// <summary>
        /// Opens the file with the given name.
        /// </summary>
        /// <param name="name">The name of the file to open.</param>
        /// <param name="mode">The opening mode.</param>
        /// <returns>The file.</returns>
        IFile OpenFile(string name, System.IO.FileMode mode);

        /// <summary>
        /// Opens the file with the given name, and with the given access.
        /// </summary>
        /// <param name="name">The name of the file to open.</param>
        /// <param name="mode">The opening mode.</param>
        /// <param name="access">The access mode.</param>
        /// <returns>The file.</returns>
        IFile OpenFile(string name, System.IO.FileMode mode, System.IO.FileAccess access);

        /// <summary>
        /// Opens the file with the given name, access and sharing strategy.
        /// </summary>
        /// <param name="name">The name of the file to open.</param>
        /// <param name="mode">The opening mode.</param>
        /// <param name="access">The access mode.</param>
        /// <param name="share">The sharing strategy.</param>
        /// <returns>The file.</returns>
        IFile OpenFile(string name, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share);

        /// <summary>
        /// Opens the file with the given name, access, sharing strategy and flags.
        /// </summary>
        /// <param name="name">The name of the file to open.</param>
        /// <param name="mode">The opening mode.</param>
        /// <param name="access">The access mode.</param>
        /// <param name="share">The sharing strategy.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>The file.</returns>
        IFile OpenFile(string name, System.IO.FileMode mode, System.IO.FileAccess access, System.IO.FileShare share, FileFlags flags);

        /// <summary>
        /// Create a subfolder with the given name.
        /// </summary>
        /// <param name="name">The name of the subfolder to create.</param>
        void CreateFolder(string name);

        /// <summary>
        /// Create a file with the given name.
        /// </summary>
        /// <param name="name">The name of the file to create.</param>
        void CreateFile(string name);
        // void CreateFile(string name, System.IO.FileOptions options);

        /// <summary>
        /// Test whether a subfolder with the given name exists in the folder.
        /// </summary>
        /// <param name="name">The name of the subfolder.</param>
        /// <returns>True if a subfolder with the given name exists in this folder, and false otherwise.</returns>
        bool FolderExists(string name);

        /// <summary>
        /// Test whether a file with the given name exists in the folder.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <returns>True if a file with the given name exists in this folder, and false otherwise.</returns>
        bool FileExists(string name);

        /// <summary>
        /// Delete the subfolder with the given name.
        /// </summary>
        /// <param name="name">The name of the subfolder to delete.</param>
        void DeleteFolder(string name);

        /// <summary>
        /// Delete the subfolder with the given name and, optionally, all of its subfolders and files in it recursively.
        /// </summary>
        /// <param name="name">The name of the subfolder to delete.</param>
        /// <param name="recursive">Indicates whether files and subfolders should be deleted recursively.</param>
        void DeleteFolder(string name, bool recursive);

        /// <summary>
        /// Delete the file with the given name in this folder.
        /// </summary>
        /// <param name="name">The name of the file to delete.</param>
        void DeleteFile(string name);

        /// <summary>
        /// Rename a subfolder.
        /// </summary>
        /// <param name="oldname">Old subfolder name.</param>
        /// <param name="newname">New subfolder name.</param>
        void RenameFolder(string oldname, string newname);

        /// <summary>
        /// Rename a file in this folder.
        /// </summary>
        /// <param name="oldname">Old file name.</param>
        /// <param name="newname">New file name.</param>
        void RenameFile(string oldname, string newname);
    }
}
