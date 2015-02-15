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

namespace QS._qss_c_.Components_2_
{
    public abstract class SerializableListOf<C> : Base3_.KnownClass, QS.Fx.Serialization.ISerializable, IEnumerable<C>, Base3_.IFormattable 
        where C : QS.Fx.Serialization.ISerializable, new()
    {
        public SerializableListOf()
        {
        }

        private IList<C> list;

        protected virtual IList<C> CreateList(int nelements)
        {
            return SerializableListOf<C>.Create(nelements);
        }

        public IList<C> List
        {
            get
            {
                if (list == null)
                    list = this.CreateList(0);

                return list;
            }
        }

        public void Add(C element)
        {
            List.Add(element);
        }

        public void Add(IEnumerable<C> elements)
        {
            IList<C> list = this.List;
            foreach (C element in elements)
                list.Add(element);
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get { return SerializableListOf<C>.SerializableInfo(this.ClassID, list); }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            Serialize(list, ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            list = Deserialize(new CreateCallback(this.CreateList), ref header, ref data);
        }

        #endregion

        #region Static Stuff

        public static QS.Fx.Serialization.SerializableInfo SerializableInfo(ClassID classID, IList<C> list)
        {
            QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)classID, (ushort)sizeof(ushort), sizeof(ushort), 0);
            foreach (C element in list)
                info.AddAnother(element.SerializableInfo);
            return info;
        }

        public unsafe static void Serialize(IList<C> list, ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                *((ushort*)(arrayptr + header.Offset)) = (ushort) list.Count;
            }
            header.consume(sizeof(ushort));
            foreach (C element in list)
                element.SerializeTo(ref header, ref data);
        }

        public delegate IList<C> CreateCallback(int nelements);

        public unsafe static IList<C> Deserialize(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            return Deserialize(DefaultCreateCallback, ref header, ref data);
        }

        private static IList<C> Create(int nelements)
        {
            return (nelements > 0) ? new List<C>(nelements) : new List<C>();
        }

        private static CreateCallback DefaultCreateCallback = new CreateCallback(Create);

        public unsafe static IList<C> Deserialize(CreateCallback createCallback,
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            IList<C> list;

            int count;
            fixed (byte* arrayptr = header.Array)
            {
                count = (int)(*((ushort*)(arrayptr + header.Offset)));
            }
            header.consume(sizeof(ushort));

            list = createCallback(count);

            while (count-- > 0)
            {
                C element = new C();
                element.DeserializeFrom(ref header, ref data);
                list.Add(element);
            }

            return list;
        }

        #endregion

        #region IEnumerable<C> Members

        IEnumerator<C> IEnumerable<C>.GetEnumerator()
        {
            return (list != null) ? list.GetEnumerator() : EmptyEnumerator();
        }

        #endregion

        private static IEnumerator<C> EmptyEnumerator()
        {
            yield break;
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (list != null) ? list.GetEnumerator() : EmptyEnumerator();
        }

        #endregion

        public override string ToString()
        {
            return "(" + QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<C>(list, ", ") + ")";
        }

        #region IFormattable Members

        string QS._qss_c_.Base3_.IFormattable.ToString(int columnOffset)
        {
            StringBuilder s = new StringBuilder();
            string columns = new string(' ', columnOffset);
            if (list != null)
            {
                s.Append(columns);
                s.AppendLine("(");
                foreach (C element in list)
                {
                    if (element is Base3_.IFormattable)
                        s.Append(((Base3_.IFormattable)element).ToString(columnOffset + 2));
                    else
                    {
                        s.Append(columns);
                        s.Append("  ");
                        s.AppendLine(element.ToString());
                    }
                }
                s.Append(columns);
                s.AppendLine(")");
            }
            else
            {
                s.Append(columns);
                s.AppendLine("()");
            }
            return s.ToString();
        }

        #endregion
    }
}
