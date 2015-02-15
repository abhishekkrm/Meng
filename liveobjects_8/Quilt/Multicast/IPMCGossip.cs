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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using QS.Fx.Serialization;
using QS.Fx.Base;
using QS.Fx.Value;

using Quilt.Bootstrap;

namespace Quilt.Multicast
{
    //[QS.Fx.Reflection.ValueClass("", "IpmcGossip")]
    [QS.Fx.Serialization.ClassID(QS.ClassID.IpmcGossip)]
    public class IPMCGossip : ISerializable
    {
        #region Constructor

        public IPMCGossip(Dictionary<string, Tuple_<HeartbeatMember, Name>> _members, BootstrapMember _self, bool _delegate)
        {
            foreach (KeyValuePair<string, Tuple_<HeartbeatMember, Name>> kvp in _members)
            {
                this._members = new List<Tuple_<HeartbeatMember, Name>>();
                this._members.Add(kvp.Value);
            }

            HeartbeatMember self = new HeartbeatMember(_self, DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
            this._members.Add(new Tuple_<HeartbeatMember, Name>(self, new Name(_delegate.ToString()), 0));
        }

        public IPMCGossip()
        {
        }

        #endregion

        #region Field

        //private Name _group_name;
        private List<Tuple_<HeartbeatMember, Name>> _members;
      
        #endregion

        #region Acess

        //public Name Group_Name
        //{
        //    get { return _group_name; }
        //}

        public List<Tuple_<HeartbeatMember, Name>> Members
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
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.IpmcGossip, sizeof(ushort));
                //info.AddAnother(((QS.Fx.Serialization.ISerializable)_group_name).SerializableInfo);

                foreach (Tuple_<HeartbeatMember, Name> member in _members)
                {
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)member.x).SerializableInfo);
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)member.y).SerializableInfo);
                }
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
                    foreach (Tuple_<HeartbeatMember, Name> member in _members)
                    {
                        ((QS.Fx.Serialization.ISerializable)member.x).SerializeTo(ref header, ref data);
                        ((QS.Fx.Serialization.ISerializable)member.y).SerializeTo(ref header, ref data);
                    }
                }

            }
            catch (Exception exc)
            {
                throw new Exception("IPMCGossip SerializeTo Exception");
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref ConsumableBlock header, ref ConsumableBlock data)
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

            this._members = new List<Tuple_<HeartbeatMember, Name>>();
            if (membercount > 0)
            {
                while (membercount > 0)
                {
                    HeartbeatMember member = new HeartbeatMember();
                    ((QS.Fx.Serialization.ISerializable)member).DeserializeFrom(ref header, ref data);
                    Name name = new Name();
                    ((QS.Fx.Serialization.ISerializable)name).DeserializeFrom(ref header, ref data);
                    this._members.Add(new Tuple_<HeartbeatMember, Name>(member, name, 0));
                    membercount--;
                }
            }

        }

        #endregion
    }
}
