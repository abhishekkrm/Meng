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
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.IO;

namespace QS._qss_x_.Administrator_
{
/*
    public sealed class Message
    {
        #region Constructors

        public Message(int seqno, int followup_seqno, MessageType messagetype, int operation, byte[][] arguments, 
            ResponseCallback )
        {
            this.seqno = seqno;
            this.followup_seqno = followup_seqno;
            this.messagetype = messagetype;
            this.operation = operation;
            this.arguments = arguments;
        }

        #endregion

        #region Fields

        private int seqno, followup_seqno, operation;
        private MessageType messagetype;
        private byte[][] arguments;

        #endregion

        #region Accessors

        public int SeqNo
        {
            get { return seqno; }
            set { seqno = value; }
        }

        public int FollowUp_SeqNo
        {
            get { return followup_seqno; }
            set { followup_seqno = value; }
        }

        public MessageType MessageType
        {
            get { return messagetype; }
            set { messagetype = value; }
        }

        public int Operation
        {
            get { return operation; }
            set { operation = value; }
        }

        public byte[][] Arguments
        {
            get { return arguments; }
            set { arguments = value; }
        }

        #endregion

        #region Serialization

        public unsafe void Serialize(Stream stream)
        {
            int narguments = (arguments != null) ? arguments.Length : 0;
            byte[] header = new byte[5 * sizeof(int)];
            fixed (byte *pheader = header)
            {                
                *((int*) pheader) = seqno;
                *((int*) (pheader + sizeof(int))) = followup_seqno;
                *((int*) (pheader + 2 * sizeof(int))) = (int) messagetype;
                *((int*) (pheader + 3 * sizeof(int))) = operation;
                *((int*) (pheader + 4 * sizeof(int))) = narguments;
                stream.Write(header, 0, header.Length);
                for (int k = 0; k < narguments; k++)
                {
                    *((int*) pheader) = arguments[k].Length;
                    stream.Write(header, 0, sizeof(int));
                    stream.Write(arguments[k], 0, arguments[k].Length);
                }
            }
        }

        public unsafe void Deserialize(Stream stream)
        {
            int narguments;
            byte[] header = new byte[5 * sizeof(int)];
            if (stream.Read(header, 0, header.Length) != header.Length)
                throw new Exception();
            fixed (byte *pheader = header)
            {
                seqno = *((int*)pheader);
                followup_seqno = *((int*)(pheader + sizeof(int)));
                messagetype = (MessageType)(*((int*)(pheader + 2 * sizeof(int))));
                operation = *((int*)(pheader + 3 * sizeof(int)));
                narguments = *((int*)(pheader + 4 * sizeof(int)));
                arguments = new byte[narguments][];
                for (int k = 0; k < narguments; k++)
                {
                    stream.Read(header, 0, sizeof(int));
                    int length = *((int *) pheader);
                    arguments[k] = new byte[length];
                    stream.Read(arguments[k], 0, length);
                }
            }
        }

        #endregion
    }
*/ 
}
