/*

Copyright (c) 2004-2009 Qi Huang. All rights reserved.

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

using QS.Fx.Value.Classes;
using QS.Fx.Base;
using QS.Fx.Value;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Quilt.Bootstrap;

namespace Quilt.Multicast
{
    [QS.Fx.Reflection.ValueClass("F532409E18F24b2fA66040D388992459", "DONetMembership")]
    [QS.Fx.Serialization.ClassID(QS.ClassID.DonetMembership)]
    public class DONetMembership:
        QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public DONetMembership(Dictionary<string, HeartbeatMember> _members, BootstrapMember _self)
        {
            foreach (KeyValuePair<string, HeartbeatMember> kvp in _members)
            {
                this._members = new List<HeartbeatMember>();
                this._members.Add(kvp.Value);
            }

            HeartbeatMember self = new HeartbeatMember(_self, DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
            this._members.Add(self);
        }

        public DONetMembership()
        {
        }

        #endregion

        #region Field

        //private Name _group_name;
        private List<HeartbeatMember> _members;

        #endregion

        #region Acess

        //public Name Group_Name
        //{
        //    get { return _group_name; }
        //}

        public List<HeartbeatMember> Members
        {
            get { return _members; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.DonetMembership, sizeof(ushort));
                //info.AddAnother(((QS.Fx.Serialization.ISerializable)_group_name).SerializableInfo);

                foreach (HeartbeatMember member in _members)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)member).SerializableInfo);

                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref ConsumableBlock header, ref IList<Block> data)
        {
            try
            {
                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                    *((ushort*)(_pheader)) = (ushort)((_members != null) ? _members.Count : 0);
                }

                header.consume(sizeof(ushort));

                //if (this._group_name != null)
                //    ((QS.Fx.Serialization.ISerializable)_group_name).SerializeTo(ref header, ref data);

                if (_members != null)
                {
                    foreach (HeartbeatMember member in _members)
                    {
                        ((QS.Fx.Serialization.ISerializable)member).SerializeTo(ref header, ref data);
                    }
                }

            }
            catch (Exception exc)
            {
                throw new Exception("DONetMembership SerializeTo Exception");
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref ConsumableBlock header, ref ConsumableBlock data)
        {
            try
            {
                ushort membercount;

                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                    membercount = *((ushort*)(_pheader));
                }

                header.consume(sizeof(ushort));

                //this._group_name = new Name();
                //((QS.Fx.Serialization.ISerializable)_group_name).DeserializeFrom(ref header, ref data);

                this._members = new List<HeartbeatMember>();
                if (membercount > 0)
                {
                    while (membercount > 0)
                    {
                        HeartbeatMember member = new HeartbeatMember();
                        ((QS.Fx.Serialization.ISerializable)member).DeserializeFrom(ref header, ref data);
                        this._members.Add(member);
                        membercount--;
                    }
                }
            }
            catch (Exception exc)
            {
                throw new Exception("DONetMembership Deserialization Exception " + exc.Message);
            }
        }

        #endregion
    }
}
