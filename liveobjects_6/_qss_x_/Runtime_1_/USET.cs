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

// #define DEBUG_VerifyIntersect
// #define DEBUG_VerifySum
// #define DEBUG_VerifySubstract

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Runtime_1_
{
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._c_uset)]
    [QS._qss_x_.Language_.Structure_.ValueType("USET", QS._qss_x_.Language_.Structure_.PredefinedType.USET)]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Runtime_UIntSet)]
    public sealed class USET : /* ISetOf<uint>, */ IValue<USET>, QS.Fx.Serialization.ISerializable
    {
        public static uint MaximumNumberOfRanges = 100;

        #region Constructors

        public USET()
        {
        }

        private USET(IEnumerable<QS._qss_c_.Base1_.Range<uint>> ranges, uint maxcovered)
        {
            this.ranges = (ranges != null) ? (new List<QS._qss_c_.Base1_.Range<uint>>(ranges)) : null;
            this.maxcovered = maxcovered;
        }

        private static readonly USET undefined = new USET(null, 0);
        private static readonly USET emptyset = new USET(null, uint.MaxValue);
        private static readonly USET fullset = 
            new USET(new QS._qss_c_.Base1_.Range<uint>[] { new QS._qss_c_.Base1_.Range<uint>(1, uint.MaxValue) }, uint.MaxValue);

        [QS._qss_x_.Language_.Structure_.Operator("Undefined", QS._qss_x_.Language_.Structure_.PredefinedOperator.Undefined, true)]
        public static USET Undefined
        {
            get { return undefined; }
        }

        [QS._qss_x_.Language_.Structure_.Operator("Empty", QS._qss_x_.Language_.Structure_.PredefinedOperator.Empty, true)]
        public static USET EmptySet
        {
            get { return emptyset; }
        }

        [QS._qss_x_.Language_.Structure_.Operator("Full", true)]
        public static USET FullSet
        {
            get { return fullset; }
        }

        #endregion

        #region Constants

        // private const int MaximumNumberOfRanges = 100;
        private const int MaximumValueToCheck = 100000;

        #endregion

        #region Fields

        private List<QS._qss_c_.Base1_.Range<uint>> ranges;
        private uint maxcovered;

        #endregion

        #region Accessors

        public IList<QS._qss_c_.Base1_.Range<uint>> Ranges
        {
            get { return ranges; }
        }

        public uint MaxCovered
        {
            get { return maxcovered; }
        }

        #endregion

        #region ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Runtime_UIntSet);
                int headersize = sizeof(uint) + sizeof(ushort) + ((ranges != null) ? (headersize = 2 * sizeof(uint) * ranges.Count) : 0);
                info.HeaderSize += headersize;
                info.Size += headersize;    
                return info;
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* _pheader = header.Array)
            {
                byte* pheader = _pheader + header.Offset;
                *((uint*) pheader) = maxcovered;
                pheader += sizeof(uint);
                *((ushort*) pheader) = (ushort)((ranges != null) ? ranges.Count : 0);
                pheader += sizeof(ushort);
                if (ranges != null)
                {
                    foreach (QS._qss_c_.Base1_.Range<uint> range in ranges)
                    {
                        *((uint*) pheader) = range.From;
                        pheader += sizeof(uint);
                        *((uint*) pheader) = range.To;
                        pheader += sizeof(uint);
                    }
                }
            }
            header.consume(sizeof(uint) + sizeof(ushort) + ((ranges != null) ? (2 * sizeof(uint) * ranges.Count) : 0));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* _pheader = header.Array)
            {
                byte* pheader = _pheader + header.Offset;
                maxcovered = *((uint*)pheader);
                pheader += sizeof(uint);
                int nranges = (int)(*((ushort*)pheader));
                pheader += sizeof(ushort);
                if (nranges > 0)
                {
                    ranges = new List<QS._qss_c_.Base1_.Range<uint>>(nranges);
                    while (nranges-- > 0)
                    {
                        uint from = *((uint*) pheader);
                        pheader += sizeof(uint);
                        uint to = *((uint*) pheader);
                        pheader += sizeof(uint);
                        ranges.Add(new QS._qss_c_.Base1_.Range<uint>(from, to));
                    }
                }
                else
                    ranges = null;
            }
            header.consume(sizeof(uint) + sizeof(ushort) + ((ranges != null) ? (2 * sizeof(uint) * ranges.Count) : 0));
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            if (maxcovered == 0)
                return "undefined";
            else
            {
                StringBuilder s = new StringBuilder();
                s.Append("(");
                if (ranges != null)
                {
                    for (int ind = 0; ind < ranges.Count; ind++)
                    {
                        if (ind > 0)
                            s.Append(", ");
                        if (ranges[ind].To == ranges[ind].From)
                            s.Append(ranges[ind].From.ToString());
                        else
                            s.Append(ranges[ind].From.ToString() + "-" + ranges[ind].To.ToString());
                    }
                }
                s.Append(")");
                if (maxcovered != uint.MaxValue)
                {
                    s.Append("; ");
                    s.Append(maxcovered.ToString());
                }
                return s.ToString();
            }
        }

        #endregion

        #region IValue<UIntSet>.IsDefined

        [QS._qss_x_.Language_.Structure_.Operator("IsDefined", QS._qss_x_.Language_.Structure_.PredefinedOperator.IsDefined, true)]
        public bool IsDefined
        {
            get { return maxcovered > 0; }
        }

        #endregion

        #region IValue<UIntSet>.SetTo

        public void SetTo(USET other)
        {
            ranges = (other.ranges != null) ? (new List<QS._qss_c_.Base1_.Range<uint>>(other.ranges)) : null;
            maxcovered = other.maxcovered;
        }

        #endregion

        #region IValue<UIntSet>.Clone

        public USET Clone()
        {
            USET copy = new USET();
            copy.maxcovered = maxcovered;
            if (ranges != null)
                copy.ranges = new List<QS._qss_c_.Base1_.Range<uint>>(ranges);
            return copy;
        }

        public static VERSIONED<USET> CloneVersioned(VERSIONED<USET> element, uint maximumnumberofranges)
        {
            throw new NotImplementedException();
//            return new VERSIONED<USET>(element.Version, 
//                element.Value.IsDefined // (element.Value != null) 
//                ? element.Value.Clone(maximumnumberofranges) : null);
        }

        public USET Clone(uint maximumnumberofranges)
        {
            USET copy = new USET();            
            if (ranges != null)
            {
                if (ranges.Count > maximumnumberofranges)
                {
                    copy.ranges = new List<QS._qss_c_.Base1_.Range<uint>>((int) maximumnumberofranges);
                    int count = 0;
                    foreach (QS._qss_c_.Base1_.Range<uint> _range in ranges)
                    {
                        if (count < maximumnumberofranges)
                        {
                            copy.ranges.Add(_range);
                            count++;
                        }
                        else
                        {
                            copy.maxcovered = _range.From - 1;
                            break;
                        }
                    }
                }
                else
                {
                    copy.ranges = new List<QS._qss_c_.Base1_.Range<uint>>(ranges);
                    copy.maxcovered = maxcovered;
                }
            }
            else
                copy.maxcovered = maxcovered;
            return copy;
        }

        #endregion

        #region IValue<UIntSet>.Erase

        public USET Erase()
        {
            USET copy = new USET();
            copy.maxcovered = maxcovered;
            copy.ranges = ranges;
            this.maxcovered = 0;
            this.ranges = null;
            return copy;
        }

        #endregion

        #region Insert

        [QS._qss_x_.Language_.Structure_.Operator("Insert", QS._qss_x_.Language_.Structure_.PredefinedOperator.Insert)]
        public void Insert(UINT element)
        {
            if (!element.IsDefined)
                throw new Exception("Cannot insert an element that has not been defined.");
            Insert(element.Number);
        }

        [QS._qss_x_.Language_.Structure_.Operator("Insert", QS._qss_x_.Language_.Structure_.PredefinedOperator.Insert)]
        public void Insert(uint element)
        {
            if (ranges != null)
            {
                bool done = false;
                int position = -1;
                for (int ind = 0; ind < ranges.Count; ind++)
                {
                    if (element > ranges[ind].To)
                    {
                        if (element == ranges[ind].To + 1)
                        {
                            ranges[ind] = new QS._qss_c_.Base1_.Range<uint>(ranges[ind].From, element);
                            done = true;
                            break;
                        }
                        else
                        {
                            position = ind;
                            continue;
                        }
                    }
                    else if (element < ranges[ind].From)
                    {
                        if (element + 1 == ranges[ind].From)
                        {
                            ranges[ind] = new QS._qss_c_.Base1_.Range<uint>(element, ranges[ind].To);
                            done = true;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        done = true;
                        break;
                    }
                }

                if (!done)
                {
                    List<QS._qss_c_.Base1_.Range<uint>> _ranges = new List<QS._qss_c_.Base1_.Range<uint>>(ranges.Count + 1);
                    for (int ind = 0; ind <= position; ind++)
                        _ranges.Add(ranges[ind]);
                    _ranges.Add(new QS._qss_c_.Base1_.Range<uint>(element, element));
                    for (int ind = (position + 1); ind < ranges.Count; ind++)
                        _ranges.Add(ranges[ind]);
                    ranges = _ranges;
                }
            }
            else
            {
                ranges = new List<QS._qss_c_.Base1_.Range<uint>>(1);
                ranges.Add(new QS._qss_c_.Base1_.Range<uint>(element, element));
                maxcovered = uint.MaxValue;
            }
        }

        #endregion

        #region Intersect

        [QS._qss_x_.Language_.Structure_.Operator("Intersect", QS._qss_x_.Language_.Structure_.PredefinedOperator.Intersect)]
        public USET Intersect(USET other)
        {
            return Intersect(other, USET.MaximumNumberOfRanges);
        }

        [QS._qss_x_.Language_.Structure_.Operator("Intersect", QS._qss_x_.Language_.Structure_.PredefinedOperator.Intersect)]
        public USET Intersect(USET other, uint maximumnumberofranges)
        {
            List<QS._qss_c_.Base1_.Range<uint>> _output;
            uint _maxcovered = Math.Min(maxcovered, other.maxcovered);

            if (_maxcovered > 0 && ranges != null && other.ranges != null)
            {
                _output = new List<QS._qss_c_.Base1_.Range<uint>>(Math.Max(ranges.Count, other.ranges.Count));
                uint _bothcovered = 0;
                IEnumerator<QS._qss_c_.Base1_.Range<uint>> _enum1 = ranges.GetEnumerator(), _enum2 = other.ranges.GetEnumerator();
                bool _finished1 = !_enum1.MoveNext(), _finished2 = !_enum2.MoveNext();
                while (_bothcovered < _maxcovered)
                {
                    while (!_finished1 && _enum1.Current.To <= _bothcovered)
                        _finished1 = !_enum1.MoveNext();

                    while (!_finished2 && _enum2.Current.To <= _bothcovered)
                        _finished2 = !_enum2.MoveNext();

                    if (_finished1 || _finished2)
                    {
                        _bothcovered = _maxcovered;
                        break;
                    }
                    else
                    {
                        uint _newcovered = Math.Max(_enum1.Current.From, _enum2.Current.From) - 1;
                        if (_newcovered > _bothcovered)
                            _bothcovered = _newcovered;
                        else
                        {
                            if (_output.Count < maximumnumberofranges)
                            {
                                _newcovered = Math.Min(_enum1.Current.To, _enum2.Current.To);
                                _output.Add(new QS._qss_c_.Base1_.Range<uint>(_bothcovered + 1, _newcovered));
                                _bothcovered = _newcovered;
                            }
                            else
                                break;
                        }
                    }
                }
                _maxcovered = _bothcovered;

#if DEBUG_VerifyIntersect

                for (uint ind = 0; ind < Math.Min(_maxcovered, MaximumValueToCheck); ind++)
                {
                    bool is1 = false, is2 = false, is3 = false;

                    foreach (QS.CMS.Base.Range<uint> _r in this.ranges)
                    {
                        if (_r.From <= ind && ind <= _r.To)
                        {
                            is1 = true;
                            break;
                        }
                    }

                    foreach (QS.CMS.Base.Range<uint> _r in other.ranges)
                    {
                        if (_r.From <= ind && ind <= _r.To)
                        {
                            is2 = true;
                            break;
                        }
                    }

                    foreach (QS.CMS.Base.Range<uint> _r in _output)
                    {
                        if (_r.From <= ind && ind <= _r.To)
                        {
                            is3 = true;
                            break;
                        }
                    }

                    if (is3 != (is1 && is2))
                    {
                        System.Diagnostics.Debug.Assert(false, "Intersect error at element (" + ind.ToString() +
                            ").\nArgument1 = " + this.ToString() + "\nArgument2 = " + other.ToString() +
                            "\nArgument1 * Argument2 = " + (new UIntSet(_output, _maxcovered)).ToString() + "\n");
                    }
                }
#endif
            }
            else
                _output = null;

            USET _result = new USET();
            _result.ranges = _output;
            _result.maxcovered = _maxcovered;
            return _result;
        }

        #endregion

        #region Sum

        [QS._qss_x_.Language_.Structure_.Operator("Union", QS._qss_x_.Language_.Structure_.PredefinedOperator.Union)]
        public USET Sum(USET other)
        {
            return Sum(other, USET.MaximumNumberOfRanges);
        }

        [QS._qss_x_.Language_.Structure_.Operator("Union", QS._qss_x_.Language_.Structure_.PredefinedOperator.Union)]
        public USET Sum(USET other, uint maximumnumberofranges)
        {
            List<QS._qss_c_.Base1_.Range<uint>> _output;
            uint _maxcovered = Math.Min(maxcovered, other.maxcovered);

            if (_maxcovered > 0)
            {
                if (ranges != null)
                {
                    if (other.ranges != null)
                    {
                        _output = new List<QS._qss_c_.Base1_.Range<uint>>(Math.Max(ranges.Count, other.ranges.Count));
                        uint _bothcovered = 0;
                        IEnumerator<QS._qss_c_.Base1_.Range<uint>> _enum1 = ranges.GetEnumerator(), _enum2 = other.ranges.GetEnumerator();
                        bool _finished1 = !_enum1.MoveNext(), _finished2 = !_enum2.MoveNext();

                        while (_bothcovered < _maxcovered)
                        {
                            if (!_finished1 || !_finished2)
                            {
                                uint _newcovered = _maxcovered;

                                if (!_finished1 && _enum1.Current.From <= _newcovered)
                                    _newcovered = _enum1.Current.From - 1;
                                if (!_finished2 && _enum2.Current.From <= _newcovered)
                                    _newcovered = _enum2.Current.From - 1;

                                if (_newcovered > _bothcovered)
                                    _bothcovered = _newcovered;

                                if (_output.Count < maximumnumberofranges)
                                {
                                    uint _newfrom = _bothcovered + 1;
                                    uint _newto = _bothcovered;

                                    while (true)
                                    {
                                        if (!_finished1 && _enum1.Current.From <= (_newto + 1))
                                        {
                                            _newto = Math.Max(_newto, _enum1.Current.To);
                                            _finished1 = !_enum1.MoveNext();
                                        }
                                        else if (!_finished2 && _enum2.Current.From <= (_newto + 1))
                                        {
                                            _newto = Math.Max(_newto, _enum2.Current.To);
                                            _finished2 = !_enum2.MoveNext();
                                        }
                                        else
                                            break;
                                    }

                                    if (_newto >= _newfrom)
                                    {
                                        _output.Add(new QS._qss_c_.Base1_.Range<uint>(_newfrom, _newto));
                                        _bothcovered = _newto;
                                    }
                                }
                                else
                                    break;
                            }
                            else
                            {
                                _bothcovered = _maxcovered;
                                break;
                            }
                        }

#if DEBUG_VerifySum

                        for (uint ind = 0; ind < Math.Min(_maxcovered, MaximumValueToCheck); ind++)
                        {
                            bool is1 = false, is2 = false, is3 = false;

                            foreach (QS.CMS.Base.Range<uint> _r in this.ranges)
                            {
                                if (_r.From <= ind && ind <= _r.To)
                                {
                                    is1 = true;
                                    break;
                                }
                            }

                            foreach (QS.CMS.Base.Range<uint> _r in other.ranges)
                            {
                                if (_r.From <= ind && ind <= _r.To)
                                {
                                    is2 = true;
                                    break;
                                }
                            }

                            foreach (QS.CMS.Base.Range<uint> _r in _output)
                            {
                                if (_r.From <= ind && ind <= _r.To)
                                {
                                    is3 = true;
                                    break;
                                }
                            }

                            if (is3 != (is1 || is2))
                            {
                                System.Diagnostics.Debug.Assert(false, "Sum error at element (" + ind.ToString() +
                                    ").\nArgument1 = " + this.ToString() + "\nArgument2 = " + other.ToString() +
                                    "\nArgument1 + Argument2 = " + (new UIntSet(_output, _maxcovered)).ToString() + "\n");
                            }
                        }
#endif

                        _maxcovered = _bothcovered;
                    }
                    else
                        _output = new List<QS._qss_c_.Base1_.Range<uint>>(ranges);
                }
                else
                {
                    if (other.ranges != null)
                        _output = new List<QS._qss_c_.Base1_.Range<uint>>(other.ranges);
                    else
                        _output = null;
                }
            }
            else
                _output = null;

            USET _result = new USET();
            _result.ranges = _output;
            _result.maxcovered = _maxcovered;
            return _result;
        }

        #endregion

        #region Substract

        [QS._qss_x_.Language_.Structure_.Operator("Diff", QS._qss_x_.Language_.Structure_.PredefinedOperator.Diff)]
        public USET Substract(USET other)
        {
            return Substract(other, USET.MaximumNumberOfRanges);
        }

        [QS._qss_x_.Language_.Structure_.Operator("Diff", QS._qss_x_.Language_.Structure_.PredefinedOperator.Diff)]
        public USET Substract(USET other, uint maximumnumberofranges)
        {
            List<QS._qss_c_.Base1_.Range<uint>> _output;
            uint _maxcovered = Math.Min(maxcovered, other.maxcovered);

            if (_maxcovered > 0 && ranges != null)
            {
                if (other.ranges != null)
                {
                    _output = new List<QS._qss_c_.Base1_.Range<uint>>(Math.Max(ranges.Count, other.ranges.Count));
                    uint _bothcovered = 0;
                    IEnumerator<QS._qss_c_.Base1_.Range<uint>> _enum1 = ranges.GetEnumerator(), _enum2 = other.ranges.GetEnumerator();
                    bool _finished1 = !_enum1.MoveNext(), _finished2 = !_enum2.MoveNext();

                    while (_bothcovered < _maxcovered)
                    {
                        while (!_finished1 && _enum1.Current.To <= _bothcovered)
                            _finished1 = !_enum1.MoveNext();

                        if (_finished1)
                        {
                            _bothcovered = _maxcovered;
                            break;
                        }

                        if (_bothcovered + 1 < _enum1.Current.From)
                            _bothcovered = _enum1.Current.From - 1;

                        while (!_finished2 && _enum2.Current.To <= _bothcovered)
                            _finished2 = !_enum2.MoveNext();

                        if (!_finished2)
                        {
                            if (_enum1.Current.To < _enum2.Current.From)
                            {
                                if (_output.Count < maximumnumberofranges)
                                {
                                    _output.Add(new QS._qss_c_.Base1_.Range<uint>(
                                        Math.Max(_bothcovered + 1, _enum1.Current.From), _enum1.Current.To));
                                    _bothcovered = _enum1.Current.To;
                                    _finished1 = !_enum1.MoveNext();
                                }
                                else
                                    break;
                            }
                            else
                            {
                                if (_enum1.Current.From < _enum2.Current.From)
                                {
                                    if (_output.Count < maximumnumberofranges)
                                    {
                                        _output.Add(new QS._qss_c_.Base1_.Range<uint>(
                                            Math.Max(_bothcovered + 1, _enum1.Current.From), _enum2.Current.From - 1));
                                        _bothcovered = _enum2.Current.To;
                                        _finished2 = !_enum2.MoveNext();
                                    }
                                    else
                                        break;
                                }
                                else
                                {
                                    _bothcovered = _enum2.Current.To;
                                    _finished2 = !_enum2.MoveNext();
                                }
                            }
                        }
                        else
                        {
                            while (!_finished1 && _output.Count < maximumnumberofranges)
                            {
                                _output.Add(new QS._qss_c_.Base1_.Range<uint>(
                                    Math.Max(_bothcovered + 1, _enum1.Current.From), _enum1.Current.To));
                                _bothcovered = _enum1.Current.To;
                                _finished1 = !_enum1.MoveNext();
                            }

                            if (_finished1)
                                _bothcovered = _maxcovered;
                            else
                                _bothcovered = _enum1.Current.From - 1;

                            break;
                        }
                    }

#if DEBUG_VerifySubstract

                    for (uint ind = 0; ind < Math.Min(_maxcovered, MaximumValueToCheck); ind++)
                    {
                        bool is1 = false, is2 = false, is3 = false;

                        foreach (QS.CMS.Base.Range<uint> _r in this.ranges)
                        {
                            if (_r.From <= ind && ind <= _r.To)
                            {
                                is1 = true;
                                break;
                            }
                        }

                        foreach (QS.CMS.Base.Range<uint> _r in other.ranges)
                        {
                            if (_r.From <= ind && ind <= _r.To)
                            {
                                is2 = true;
                                break;
                            }
                        }

                        foreach (QS.CMS.Base.Range<uint> _r in _output)
                        {
                            if (_r.From <= ind && ind <= _r.To)
                            {
                                is3 = true;
                                break;
                            }
                        }

                        if (is3 != (is1 && !is2))
                        {
                            System.Diagnostics.Debug.Assert(false, "Substraction error at element (" + ind.ToString() + 
                                ").\nArgument1 = " + this.ToString() + "\nArgument2 = " + other.ToString() + 
                                "\nArgument1 - Argument2 = " + (new UIntSet(_output, _maxcovered)).ToString() + "\n");
                        }
                    }
#endif

                    _maxcovered = _bothcovered;
                }
                else
                    _output = new List<QS._qss_c_.Base1_.Range<uint>>(ranges);
            }
            else
                _output = null;

            USET _result = new USET();
            _result.ranges = _output;
            _result.maxcovered = _maxcovered;
            return _result;
        }

        #endregion

        #region Random

        private static readonly System.Random random = new Random();
        public static USET Random(uint maxnum, uint maxintervals)
        {
            List<QS._qss_c_.Base1_.Range<uint>> ranges = new List<QS._qss_c_.Base1_.Range<uint>>();
            uint[] points = new uint[2 * (1 + random.Next((int) maxintervals - 1))];
            for (int ind = 0; ind < points.Length; ind++)
                points[ind] = 1 + (uint) random.Next((int) maxnum);
            Array.Sort<uint>(points);
            uint maxpoint = 0;
            for (int ind = 0; ind < points.Length; ind += 2)
            {
                if (points[ind] > maxpoint)
                    ranges.Add(new QS._qss_c_.Base1_.Range<uint>(points[ind], points[ind + 1]));
                maxpoint = points[ind + 1];
            }
            return new USET(ranges, (uint) random.Next(((int) points[points.Length - 1]), (int) maxnum));
        }

        #endregion

        #region Test

        public static void Test(uint maxnum, uint maxintervals, uint maxiterations)
        {
            for (int iteration = 0; iteration < maxiterations; iteration++)
            {
                System.Console.WriteLine((iteration + 1).ToString("000000") + " / " + maxiterations.ToString("000000"));

                USET x = Random(maxnum, maxintervals);
                USET y = Random(maxnum, maxintervals);
                USET a = x.Sum(y, maxintervals / 2);
                USET b = x.Intersect(y, maxintervals / 2);
                USET c = x.Substract(y, maxintervals / 2);

                for (uint ind = 1; ind <= Math.Min(maxnum, Math.Min(x.maxcovered, y.maxcovered)); ind++)
                {
                    bool isx = false, isy = false, isa = false, isb = false, isc = false;

                    foreach (QS._qss_c_.Base1_.Range<uint> _r in x.ranges)
                    {
                        if (_r.From <= ind && ind <= _r.To)
                        {
                            isx = true;
                            break;
                        }
                    }

                    foreach (QS._qss_c_.Base1_.Range<uint> _r in y.ranges)
                    {
                        if (_r.From <= ind && ind <= _r.To)
                        {
                            isy = true;
                            break;
                        }
                    }

                    if (ind <= a.maxcovered)
                    {
                        foreach (QS._qss_c_.Base1_.Range<uint> _r in a.ranges)
                        {
                            if (_r.From <= ind && ind <= _r.To)
                            {
                                isa = true;
                                break;
                            }
                        }

                        if (isa != (isx || isy))
                        {
                            System.Console.WriteLine("sum failed on element " + ind.ToString() +
                                "\nx = " + x.ToString() + "\ny = " + y.ToString() + "\na = " + a.ToString());
                            USET _a = x.Sum(y, maxintervals / 2);
                        }
                    }

                    if (ind <= b.maxcovered)
                    {
                        foreach (QS._qss_c_.Base1_.Range<uint> _r in b.ranges)
                        {
                            if (_r.From <= ind && ind <= _r.To)
                            {
                                isb = true;
                                break;
                            }
                        }

                        if (isb != (isx && isy))
                        {
                            System.Console.WriteLine("intersection failed on element " + ind.ToString() +
                                "\nx = " + x.ToString() + "\ny = " + y.ToString() + "\nb = " + b.ToString());
                            USET _b = x.Intersect(y, maxintervals / 2);
                        }
                    }

                    if (ind <= c.maxcovered)
                    {
                        foreach (QS._qss_c_.Base1_.Range<uint> _r in c.ranges)
                        {
                            if (_r.From <= ind && ind <= _r.To)
                            {
                                isc = true;
                                break;
                            }
                        }

                        if (isc != (isx && !isy))
                        {
                            System.Console.WriteLine("substraction failed on element " + ind.ToString() +
                                "\nx = " + x.ToString() + "\ny = " + y.ToString() + "\nv = " + c.ToString());
                            USET _c = x.Substract(y, maxintervals / 2);
                        }
                    }
                }
            }
        }

        #endregion

        #region Trim

        [QS._qss_x_.Language_.Structure_.Operator("Trim")]
        public void Trim(uint maximumnumberofranges)
        {
            if (ranges != null && ranges.Count > maximumnumberofranges)
            {
                maxcovered = ranges[(int)maximumnumberofranges].From - 1;
                ranges.RemoveRange((int)maximumnumberofranges, ranges.Count - 1);
            }
        }

        #endregion

        #region Cut

        [QS._qss_x_.Language_.Structure_.Operator("Cut")]
        public USET Cut(uint maximumtoremove)
        {
            if (ranges != null)
            {
                List<QS._qss_c_.Base1_.Range<uint>> _output = new List<QS._qss_c_.Base1_.Range<uint>>(ranges.Count);
                foreach (QS._qss_c_.Base1_.Range<uint> _range in ranges)
                {
                    if (_range.From > maximumtoremove)
                        _output.Add(_range);
                    else if (_range.To > maximumtoremove)
                        _output.Add(new QS._qss_c_.Base1_.Range<uint>(Math.Max(_range.From, maximumtoremove + 1), _range.To));                
                }
                return new USET(_output, this.maxcovered);
            }
            else
                return new USET(null, this.maxcovered);
        }

        #endregion

        #region MaxContiguous

        public UINT MaxContiguous(UINT _existingmaxcontiguous)
        {
            uint _maxcontiguous = _existingmaxcontiguous.IsDefined ? _existingmaxcontiguous.Number : 0;

            if (maxcovered > _maxcontiguous && ranges != null)
            {
                foreach (QS._qss_c_.Base1_.Range<uint> _range in ranges)
                {
                    if (_range.To > _maxcontiguous)
                    {
                        if (_range.From > _maxcontiguous + 1)
                            break;
                        else
                            _maxcontiguous = _range.To;
                    }
                }
            }
            
            return new UINT(_maxcontiguous);
        }

        #endregion

        #region IComparable<UIntSet> Members

        public Ordering CompareTo(USET other)
        {
            uint _maxcovered = Math.Min(maxcovered, other.maxcovered);
            if (_maxcovered > 0)
            {
                if (ranges != null)
                {
                    if (other.ranges != null)
                    {
                        bool _isnotsmaller = false, _isnotlarger = false;
                        uint _maxchecked = 0;
                        IEnumerator<QS._qss_c_.Base1_.Range<uint>> _enum1 = ranges.GetEnumerator(), _enum2 = other.ranges.GetEnumerator();
                        bool _finished1 = !_enum1.MoveNext(), _finished2 = !_enum2.MoveNext();
                        while (_maxchecked < _maxcovered)
                        {
                            while (!_finished1 && _enum1.Current.To <= _maxchecked)
                                _finished1 = !_enum1.MoveNext();

                            while (!_finished2 && _enum2.Current.To <= _maxchecked)
                                _finished2 = !_enum2.MoveNext();

                            if (!_finished1)
                            {
                                if (!_finished2)
                                {
                                    uint _newchecked = Math.Max(_enum1.Current.From, _enum2.Current.From) - 1;
                                    if (_newchecked > _maxchecked)
                                        _maxchecked = _newchecked;

                                    if (_enum1.Current.From > _maxchecked + 1)
                                    {
                                        _isnotlarger = true;
                                        if (_isnotsmaller)
                                            return Ordering.Uncomparable;
                                        _maxchecked = Math.Min(_enum1.Current.From, _enum2.Current.To);
                                    }
                                    else if (_enum2.Current.From > _maxchecked + 1)
                                    {
                                        _isnotsmaller = true;
                                        if (_isnotlarger)
                                            return Ordering.Uncomparable;
                                        _maxchecked = Math.Min(_enum2.Current.From, _enum1.Current.To);
                                    }
                                    else
                                        _maxchecked = Math.Min(_enum1.Current.To, _enum2.Current.To);
                                }
                                else
                                {
                                    if (_enum1.Current.From <= _maxcovered)
                                        _isnotsmaller = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (!_finished2)
                                {
                                    if (_enum2.Current.From <= _maxcovered)
                                        _isnotlarger = true;
                                    break;
                                }
                                break;
                            }
                        }

                        if (_isnotsmaller)
                        {
                            if (_isnotlarger)
                                return Ordering.Uncomparable;
                            else
                                return Ordering.Larger;
                        }
                        else
                        {
                            if (_isnotlarger)
                                return Ordering.Smaller;
                            else
                                return Ordering.Identical;
                        }
                    }
                    else
                        return Ordering.Larger;
                }
                else
                {
                    if (other.ranges != null)
                        return Ordering.Smaller;
                    else
                        return Ordering.Identical;
                }
            }
            else
                return Ordering.Identical;
        }

        #endregion

        #region IsLess

        [QS._qss_x_.Language_.Structure_.Operator("IsLess", QS._qss_x_.Language_.Structure_.PredefinedOperator.LTE)]
        public bool IsLess(USET other)
        {
            uint _maxcovered = Math.Min(maxcovered, other.maxcovered);
            if (_maxcovered == 0 || ranges == null)
                return true;
            if (other.ranges == null)
                return false;
            uint _maxchecked = 0;
            IEnumerator<QS._qss_c_.Base1_.Range<uint>> _enum1 = ranges.GetEnumerator(), _enum2 = other.ranges.GetEnumerator();
            bool _finished1 = !_enum1.MoveNext(), _finished2 = !_enum2.MoveNext();
            while (_maxchecked < _maxcovered)
            {
                while (!_finished1 && _enum1.Current.To <= _maxchecked)
                    _finished1 = !_enum1.MoveNext();

                while (!_finished2 && _enum2.Current.To <= _maxchecked)
                    _finished2 = !_enum2.MoveNext();

                if (_finished1)
                    return true;

                if (_finished2)
                    return _enum1.Current.From > _maxcovered;

                uint _newchecked = Math.Max(_enum1.Current.From, _enum2.Current.From) - 1;
                if (_newchecked > _maxchecked)
                    _maxchecked = _newchecked;
                if (_enum2.Current.From > _maxchecked + 1)
                    return false;
                else if (_enum1.Current.From > _maxchecked + 1)
                    _maxchecked = Math.Min(_enum1.Current.From, _enum2.Current.To);
                else
                    _maxchecked = Math.Min(_enum1.Current.To, _enum2.Current.To);
            }

            return true;
        }

        #endregion

        #region _SubstractionCheck

        public void _SubstractionCheck(UINT _argument1, UINT _argument2, UINT _difference)
        {
            // .........
        }

        #endregion

        #region Substract_2

        [QS._qss_x_.Language_.Structure_.Operator("Diff2", QS._qss_x_.Language_.Structure_.PredefinedOperator.Diff)]
        public USET Substract_2(USET other)
        {
            return Substract_2(other, USET.MaximumNumberOfRanges);
        }

        // this substraction prevents any information loss, it only substract as much as it can deduce but preserves anything it isn't sure about
        [QS._qss_x_.Language_.Structure_.Operator("Diff2", QS._qss_x_.Language_.Structure_.PredefinedOperator.Diff)]
        public USET Substract_2(USET other, uint maximumnumberofranges)
        {
            List<QS._qss_c_.Base1_.Range<uint>> _output;
            if (maxcovered > 0 && ranges != null)
            {
                if (other.ranges != null)
                {
                    _output = new List<QS._qss_c_.Base1_.Range<uint>>(Math.Max(ranges.Count, other.ranges.Count));
                    uint _bothcovered = 0;
                    IEnumerator<QS._qss_c_.Base1_.Range<uint>> _enum1 = ranges.GetEnumerator(), _enum2 = other.ranges.GetEnumerator();
                    bool _finished1 = !_enum1.MoveNext(), _finished2 = !_enum2.MoveNext();

                    while (_bothcovered < maxcovered)
                    {
                        while (!_finished1 && _enum1.Current.To <= _bothcovered)
                            _finished1 = !_enum1.MoveNext();

                        if (_finished1)
                            break;

                        if (_bothcovered + 1 < _enum1.Current.From)
                            _bothcovered = _enum1.Current.From - 1;

                        while (!_finished2 && _enum2.Current.To <= _bothcovered)
                            _finished2 = !_enum2.MoveNext();

                        if (!_finished2)
                        {
                            if (_enum1.Current.To < _enum2.Current.From)
                            {
                                if (_output.Count < maximumnumberofranges)
                                {
                                    _output.Add(new QS._qss_c_.Base1_.Range<uint>(
                                        Math.Max(_bothcovered + 1, _enum1.Current.From), _enum1.Current.To));
                                    _bothcovered = _enum1.Current.To;
                                    _finished1 = !_enum1.MoveNext();
                                }
                                else
                                    break;
                            }
                            else
                            {
                                if (_enum1.Current.From < _enum2.Current.From)
                                {
                                    if (_output.Count < maximumnumberofranges)
                                    {
                                        _output.Add(new QS._qss_c_.Base1_.Range<uint>(
                                            Math.Max(_bothcovered + 1, _enum1.Current.From), _enum2.Current.From - 1));
                                        _bothcovered = _enum2.Current.To;
                                        _finished2 = !_enum2.MoveNext();
                                    }
                                    else
                                        break;
                                }
                                else
                                {
                                    _bothcovered = _enum2.Current.To;
                                    _finished2 = !_enum2.MoveNext();
                                }
                            }
                        }
                        else
                        {
                            while (!_finished1 && _output.Count < maximumnumberofranges)
                            {
                                _output.Add(new QS._qss_c_.Base1_.Range<uint>(
                                    Math.Max(_bothcovered + 1, _enum1.Current.From), _enum1.Current.To));
                                _bothcovered = _enum1.Current.To;
                                _finished1 = !_enum1.MoveNext();
                            }

                            if (_finished1)
                                _bothcovered = maxcovered;
                            else
                                _bothcovered = _enum1.Current.From - 1;

                            break;
                        }
                    }
                }
                else
                    _output = new List<QS._qss_c_.Base1_.Range<uint>>(ranges);
            }
            else
                _output = null;

            USET _result = new USET();
            _result.ranges = _output;
            _result.maxcovered = maxcovered;
            return _result;
        }

        #endregion
    }
}
