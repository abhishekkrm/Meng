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

namespace liveobjects_8.Components_
{
    [QS.Fx.Reflection.InterfaceClass("2109CB865A3E4204A5037F31E124EC28`1", "Incoming")]
    public interface IIncoming<[QS.Fx.Reflection.Parameter("ObjectClass", QS.Fx.Reflection.ParameterClass.ObjectClass)] ObjectClass>
        : QS.Fx.Interface.Classes.IInterface
        where ObjectClass : class, QS.Fx.Object.Classes.IObject
    {
        [QS.Fx.Reflection.Operation("Ready")]
        bool Ready();

        [QS.Fx.Reflection.Operation("Channels")]
        Channels Channels(string path);

        [QS.Fx.Reflection.Operation("Add")]
        void Add(string path, string _key, QS.Fx.Object.IReference<ObjectClass> _object, int op);

        [QS.Fx.Reflection.Operation("Delete")]
        void Delete(string path, string channelID, int op);

        [QS.Fx.Reflection.Operation("Rename")]
        void Rename(string path, string channelID, string newID, int op);

        [QS.Fx.Reflection.Operation("AddNewFolder")]
        void AddNewFolder(string path, string folderID, int op);

        [QS.Fx.Reflection.Operation("GetXML")]
        string GetXML(string path, string _key);

        [QS.Fx.Reflection.Operation("getChildren")]
        IDictionary<String, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>
           getChildren(QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _key);
        //[QS.Fx.Reflection.Operation("GetObject")]
        //QS.Fx.Object.IReference<ObjectClass> GetObject(String _key);

        //[QS.Fx.Reflection.Operation("TryGetObject")]
        //bool TryGetObject(String _key, out QS.Fx.Object.IReference<ObjectClass> _object);
    }
}
