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
using System.Xml.Serialization;

namespace liveobjects_8.Components_
{
    [QS.Fx.Reflection.ValueClass("FEDA595A2B51420991B7D87C5B64A681`1", "Folder_")]
    public sealed class Folder_
    {
        public Folder_(string path, string _id, string _objectxml, int op) //add
        {
            this.path = path;
            this._id = _id;
            if (op == 3) //rename operation overloaded into this constructor
                this.newID = _objectxml;
            else if (op == 1 ) //add operation
                this._objectxml = _objectxml;
            this.op = op;
        }
        /* //Same as previous constructor
        public Folder_(string channelID, string newID, int op) //rename
        {
            this._id = channelID;
            this.newID = newID;
            this.op = op;
        }
         * */
        public Folder_(string path, string channelID, int op) //remove
        {
            this._id = channelID;
            this.path = path;            
            this.op = op;
        }
        
        public Folder_()
        {
        }

        [XmlAttribute]
        public string newID,_id,_objectxml, path;
        [XmlAttribute]
        public int op;
        
        
        
    }       
}
