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
using System.Text;

//using live objects namespaces
using QS.Fx.Value.Classes;
using QS.Fx.Base;
using QS.Fx.Value;

namespace Quilt.Bootstrap
{
    [QS.Fx.Reflection.ValueClass("9C907CFF6C894be6B2DFDA2E295B016F", "BootstrapMembership")]
    [QS.Fx.Serialization.ClassID(QS.ClassID.BootstrapMembership)]
    public sealed class BootstrapMembership:
        IMembership<Incarnation, BootstrapMember>,
        QS.Fx.Serialization.ISerializable, IDisposable
    {
        #region Field

        private Name group_name;
        private Incarnation group_incarnation;
        private List<BootstrapMember> members;
        private PatchInfo extend;

        #endregion

        #region Access

        public Name GroupName
        {
            get { return group_name; }
        }

        public PatchInfo PatchInfo
        {
            get { return extend; }
        }

        public List<BootstrapMember> Members
        {
            get { return members; }
        }

        #endregion

        #region Construct

        public BootstrapMembership()
        {

        }

        /// <summary>
        /// BootstrapMember is deep copied into Membership, others are shallow copied
        /// </summary>
        /// <param name="group_name"></param>
        /// <param name="group_incarnation"></param>
        /// <param name="members"></param>
        /// <param name="extend"></param>
        public BootstrapMembership(Name group_name, Incarnation group_incarnation, IEnumerable<BootstrapMember> members, PatchInfo extend)
        {
            this.group_name = group_name;
            this.group_incarnation = group_incarnation;
            this.members = new List<BootstrapMember>();
            foreach (BootstrapMember member in members)
            {
                BootstrapMember bsmember = (BootstrapMember)(((ICloneable)member).Clone());
                this.members.Add(bsmember);
            }
            this.extend = extend;
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.BootstrapMembership, sizeof(ushort));
                info.AddAnother(((QS.Fx.Serialization.ISerializable)group_name).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)group_incarnation).SerializableInfo);

                foreach (BootstrapMember member in members)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)member).SerializableInfo);

                info.AddAnother(((QS.Fx.Serialization.ISerializable)extend).SerializableInfo);

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
                    *((ushort*)(_pheader)) = (ushort)((members != null) ? members.Count : 0);
                }

                header.consume(sizeof(ushort));

                if (this.group_name != null)
                    ((QS.Fx.Serialization.ISerializable)group_name).SerializeTo(ref header, ref data);
                if (this.group_incarnation != null)
                    ((QS.Fx.Serialization.ISerializable)group_incarnation).SerializeTo(ref header, ref data);

                if (members != null)
                {
                    foreach (BootstrapMember member in members)
                    {
                        ((QS.Fx.Serialization.ISerializable)member).SerializeTo(ref header, ref data);
                    }
                }

                if (this.extend != null)
                    ((QS.Fx.Serialization.ISerializable)extend).SerializeTo(ref header, ref data);
            }
            catch (Exception exc)
            {
                throw new Exception("GroupMembership SerializeTo Exception");
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

            this.group_name = new Name();
            ((QS.Fx.Serialization.ISerializable)group_name).DeserializeFrom(ref header, ref data);
            this.group_incarnation = new Incarnation();
            ((QS.Fx.Serialization.ISerializable)group_incarnation).DeserializeFrom(ref header, ref data);

            this.members = new List<BootstrapMember>();
            if (membercount > 0)
            {
                while (membercount > 0)
                {
                    BootstrapMember member = (BootstrapMember)(QS._core_c_.Base3.Serializer.CreateObject((ushort)QS.ClassID.BootstrapMember));
                    ((QS.Fx.Serialization.ISerializable)member).DeserializeFrom(ref header, ref data);
                    this.members.Add(member);
                    membercount--;
                }
            }

            this.extend = (PatchInfo)(QS._core_c_.Base3.Serializer.CreateObject((ushort)QS.ClassID.PatchInfo));
            ((QS.Fx.Serialization.ISerializable)extend).DeserializeFrom(ref header, ref data);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            foreach (BootstrapMember member in this.members)
            {
                ((IDisposable)member).Dispose();
            }
            members.Clear();
        }

        #endregion

        #region IMembership<Incarnation,BootstrapMember> Members

        Incarnation IMembership<Incarnation, BootstrapMember>.Incarnation
        {
            get { return this.group_incarnation; }
        }

        bool IMembership<Incarnation, BootstrapMember>.Incremental
        {
            get { return false; }
        }

        IEnumerable<BootstrapMember> IMembership<Incarnation, BootstrapMember>.Members
        {
            get { return this.members; }
        }

        #endregion
    }
}
