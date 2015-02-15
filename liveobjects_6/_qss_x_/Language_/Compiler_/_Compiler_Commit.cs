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

namespace QS._qss_x_.Language_.Compiler_
{

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _bind_initialization_o
/*
            this._CommitOk = UIntSet.EmptySet.Clone();
            this._AbortOk = UIntSet.EmptySet.Clone();
#if DEBUG_SetAcknowledgement
            this._Ack = UIntSet.EmptySet.Clone();
#else
            this._Ack = new UInt(0);
            this._Completed = UIntSet.EmptySet.Clone();
#endif
            this._ToCommit = UIntSet.EmptySet.Clone();
            this._ToAbort = UIntSet.EmptySet.Clone();
#if DEBUG_SetAcknowledgement
            this._Clean = UIntSet.EmptySet.Clone();
#else
            this._Clean = new UInt(0);
#endif
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_initialization_o
/*
            this._CommitOk = UIntSet.Undefined.Clone();
            this._AbortOk = UIntSet.Undefined.Clone();
#if DEBUG_SetAcknowledgement
            this._Ack = UIntSet.Undefined.Clone();
#else
            this._Ack = new UInt(0);
#endif
            this._ToCommit = UIntSet.EmptySet.Clone();
            this._ToAbort = UIntSet.EmptySet.Clone();
#if DEBUG_SetAcknowledgement
            this._Clean = UIntSet.EmptySet.Clone();
#else
            this._Clean = new UInt(0);
#endif

            this._aggr_CommitOk = new Versioned<UIntSet>(QS.Fx.Runtime.Version.Undefined, UIntSet.Undefined.Clone());
            this._aggr_AbortOk = new Versioned<UIntSet>(QS.Fx.Runtime.Version.Undefined, UIntSet.Undefined.Clone());
#if DEBUG_SetAcknowledgement
            this._aggr_Ack = new Versioned<UIntSet>(QS.Fx.Runtime.Version.Undefined, null);
#else
            this._aggr_Ack = new Versioned<UInt>(QS.Fx.Runtime.Version.Undefined, UInt.Undefined.Clone());
#endif
            this._diss_ToCommit = new Versioned<UIntSet>(QS.Fx.Runtime.Version.Undefined, UIntSet.Undefined.Clone());
            this._diss_ToAbort = new Versioned<UIntSet>(QS.Fx.Runtime.Version.Undefined, UIntSet.Undefined.Clone());
#if DEBUG_SetAcknowledgement
            this._diss_Clean = new Versioned<UIntSet>(QS.Fx.Runtime.Version.Undefined, null);
#else
            this._diss_Clean = new Versioned<UInt>(QS.Fx.Runtime.Version.Undefined, UInt.Undefined.Clone());
#endif
*/
    
// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _root_initialization_o
/*
            this._CommitOk = UIntSet.Undefined.Clone();
            this._AbortOk = UIntSet.Undefined.Clone();
#if DEBUG_SetAcknowledgement
            this._Ack = UIntSet.Undefined.Clone();
#else
            this._Ack = new UInt(0);
#endif
            this._ToCommit = UIntSet.EmptySet.Clone();
            this._ToAbort = UIntSet.EmptySet.Clone();
#if DEBUG_SetAcknowledgement
            this._Clean = UIntSet.EmptySet.Clone();
#else
            this._Clean = new UInt(0);
#endif
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_aggregate_1_o
/*
            if (peercontext.IsRegular)
            {
                outgoing._aggr_CommitOk = new Versioned<UIntSet>(version,
                    this._CommitOk.Clone(MaximumNumberOfRangesForNetworkCommunication));

                outgoing._aggr_AbortOk = new Versioned<UIntSet>(version,
                    this._AbortOk.Clone(MaximumNumberOfRangesForNetworkCommunication));

#if DEBUG_SetAcknowledgement
            outgoing._aggr_Ack = new Versioned<UIntSet>(version,
                this._Ack.Clone(MaximumNumberOfRangesForNetworkCommunication));
#else
                outgoing._aggr_Ack = new Versioned<UInt>(version, this._Ack.Clone());
#endif
            }
            else
            {
                // inserting neutral values for properties that are strict
                outgoing._aggr_CommitOk = new Versioned<UIntSet>(version, UIntSet.FullSet.Clone());
                outgoing._aggr_AbortOk = new Versioned<UIntSet>(version, UIntSet.EmptySet.Clone());
                outgoing._aggr_Ack = new Versioned<UInt>(version, new UInt(uint.MaxValue));
            }

            outgoing._diss_ToCommit = UIntSet.CloneVersioned(this._diss_ToCommit, MaximumNumberOfRangesForNetworkCommunication);
            outgoing._diss_ToAbort = UIntSet.CloneVersioned(this._diss_ToAbort, MaximumNumberOfRangesForNetworkCommunication);
#if DEBUG_SetAcknowledgement
            outgoing._diss_Clean = UIntSet.CloneVersioned(this._diss_Clean, MaximumNumberOfRangesForNetworkCommunication);
#else
            outgoing._diss_Clean = this._diss_Clean.Clone();
#endif
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_aggregate_2_o
/*
            if (peercontext.IsRegular)
            {
                outgoing._aggr_CommitOk = new Versioned<UIntSet>(incoming._aggr_CommitOk.Version,
                    incoming._aggr_CommitOk.Value.Intersect(this._CommitOk, MaximumNumberOfRangesForNetworkCommunication));

                outgoing._aggr_AbortOk = new Versioned<UIntSet>(incoming._aggr_AbortOk.Version,
                    incoming._aggr_AbortOk.Value.Sum(this._AbortOk, MaximumNumberOfRangesForNetworkCommunication));

#if DEBUG_SetAcknowledgement
            outgoing._aggr_Ack = new Versioned<UIntSet>(incoming._aggr_Ack.Version,
                incoming._aggr_Ack.Value.Intersect(this._Ack, MaximumNumberOfRangesForNetworkCommunication));
#else
                outgoing._aggr_Ack = new Versioned<UInt>(incoming._aggr_Ack.Version,
                    new UInt(Math.Min(incoming._aggr_Ack.Value.Number, this._Ack.Number)));
#endif
            }
            else
            {
                outgoing._aggr_CommitOk = incoming._aggr_CommitOk.Clone();
                outgoing._aggr_AbortOk = incoming._aggr_AbortOk.Clone();
                outgoing._aggr_Ack = incoming._aggr_Ack.Clone();
            }

            outgoing._diss_ToCommit.UpdateTo(this._diss_ToCommit);
            if (outgoing._diss_ToCommit.Value != null)
                outgoing._diss_ToCommit.Value.Trim(MaximumNumberOfRangesForNetworkCommunication);

            outgoing._diss_ToAbort.UpdateTo(this._diss_ToAbort);
            if (outgoing._diss_ToAbort.Value != null)
                outgoing._diss_ToAbort.Value.Trim(MaximumNumberOfRangesForNetworkCommunication);

            outgoing._diss_Clean.UpdateTo(this._diss_Clean);

#if DEBUG_SetAcknowledgement
            if (outgoing._diss_Clean.Value != null)
                outgoing._diss_Clean.Value.Trim(MaximumNumberOfRangesForNetworkCommunication);
#endif
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_aggregate_3_o
/*
            if (version.CompareTo(this._aggr_CommitOk.Version) > 0)
            {
                if (peercontext.IsRegular)
                {
                    this._aggr_CommitOk = new QS.Fx.Runtime.Versioned<UIntSet>(
                        incoming._aggr_CommitOk.Version, incoming._aggr_CommitOk.Value.Intersect(this._CommitOk, uint.MaxValue));
                }
                else
                    this._aggr_CommitOk = incoming._aggr_CommitOk.Clone();
            }

            if (version.CompareTo(this._aggr_AbortOk.Version) > 0)
            {
                if (peercontext.IsRegular)
                {
                    this._aggr_AbortOk = new QS.Fx.Runtime.Versioned<UIntSet>(
                        incoming._aggr_AbortOk.Version, incoming._aggr_AbortOk.Value.Sum(this._AbortOk, uint.MaxValue));
                }
                else
                    this._aggr_AbortOk = incoming._aggr_AbortOk.Clone();
            }

#if DEBUG_SetAcknowledgement
            if (version.CompareTo(this._aggr_Ack.Version) > 0)
                this._aggr_Ack = new QS.Fx.Runtime.Versioned<UIntSet>(
                    incoming._aggr_Ack.Version, incoming._aggr_Ack.Value.Intersect(this._Ack, uint.MaxValue));
#else
            if (version.CompareTo(this._aggr_Ack.Version) > 0)
            {
                Versioned<UInt> _new_aggr_Ack;
                if (peercontext.IsRegular)
                {
                    _new_aggr_Ack = new QS.Fx.Runtime.Versioned<UInt>(incoming._aggr_Ack.Version,
                        new UInt(Math.Min(incoming._aggr_Ack.Value.Number, this._Ack.Number)));
                }
                else
                    _new_aggr_Ack = incoming._aggr_Ack.Clone();

#if DEBUG_MonotonicChecks
                Versioned<UInt>.MonotonicCheck(this._aggr_Ack, _new_aggr_Ack);
#endif
                this._aggr_Ack = _new_aggr_Ack;
            }
#endif
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_disseminate_1_o
/*
            outgoing._aggr_CommitOk = UIntSet.CloneVersioned(this._aggr_CommitOk, MaximumNumberOfRangesForNetworkCommunication);
            outgoing._aggr_AbortOk = UIntSet.CloneVersioned(this._aggr_AbortOk, MaximumNumberOfRangesForNetworkCommunication);
#if DEBUG_SetAcknowledgement
            outgoing._aggr_Ack = UIntSet.CloneVersioned(this._aggr_Ack, MaximumNumberOfRangesForNetworkCommunication);
#else
            outgoing._aggr_Ack = this._aggr_Ack.Clone();
#endif
            outgoing._diss_ToCommit = UIntSet.CloneVersioned(this._diss_ToCommit, MaximumNumberOfRangesForNetworkCommunication);
            outgoing._diss_ToAbort = UIntSet.CloneVersioned(this._diss_ToAbort, MaximumNumberOfRangesForNetworkCommunication);
#if DEBUG_SetAcknowledgement
            outgoing._diss_Clean = UIntSet.CloneVersioned(this._diss_Clean, MaximumNumberOfRangesForNetworkCommunication);
#else
            outgoing._diss_Clean = this._diss_Clean.Clone();
#endif
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_disseminate_2_and_3_o
/*
            this._aggr_CommitOk.UpdateTo(incoming._aggr_CommitOk);
            this._aggr_AbortOk.UpdateTo(incoming._aggr_AbortOk);

#if DEBUG_MonotonicChecks
            Versioned<UInt>.MonotonicCheck(this._aggr_Ack, incoming._aggr_Ack);
#endif
            this._aggr_Ack.UpdateTo(incoming._aggr_Ack);
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _bind_updated_upper_o
/*
#if DEBUG_SetAcknowledgement
                UIntSet _new_Clean = uppercontrol._Clean;
#else
                UInt _new_Clean = uppercontrol._Clean;
#endif

#if DEBUG_DisseminateDecisionsWithoutMerging
                UIntSet _new_ToCommit = uppercontrol._ToCommit.Clone();
                UIntSet _new_ToAbort = uppercontrol._ToAbort.Clone();
#else
#if DEBUG_SetAcknowledgement
                UIntSet _new_ToCommit = uppercontrol._ToCommit.Substract(_new_Clean, uint.MaxValue);
                UIntSet _new_ToAbort = uppercontrol._ToAbort.Substract(_new_Clean, uint.MaxValue);
#else
                UIntSet _new_ToCommit = uppercontrol._ToCommit.Cut(_new_Clean.Number);
                UIntSet _new_ToAbort = uppercontrol._ToAbort.Cut(_new_Clean.Number);
#endif
#endif

                UIntSet _added_ToCommit = _new_ToCommit.Substract_2(this._ToCommit, uint.MaxValue);
                UIntSet _added_ToAbort = _new_ToAbort.Substract_2(this._ToAbort, uint.MaxValue);

                if (_added_ToCommit.Ranges != null)
                {
                    foreach (QS.CMS.Base.Range<uint> _r in _added_ToCommit.Ranges)
                    {
                        for (uint _m = _r.From; _m <= Math.Min(_r.To, _added_ToCommit.MaxCovered); _m++)
                            endpoint.Commit(_m);
                    }
                }

                if (_added_ToAbort.Ranges != null)
                {
                    foreach (QS.CMS.Base.Range<uint> _r in _added_ToAbort.Ranges)
                    {
                        for (uint _m = _r.From; _m <= Math.Min(_r.To, _added_ToAbort.MaxCovered); _m++)
                            endpoint.Abort(_m);
                    }
                }

                this._ToCommit.SetTo(_new_ToCommit);
                this._ToAbort.SetTo(_new_ToAbort);

#if DEBUG_MonotonicChecks
                Value.MonotonicCheck<UInt>(this._Clean, _new_Clean);
#endif

#if DEBUG_SetAcknowledgement
                this._Clean.SetTo(_new_Clean);
#else
                this._Clean.SetTo(_new_Clean);
#endif

                // rules for ack

#if DEBUG_DisseminateDecisionsWithoutMerging
#if DEBUG_SetAcknowledgement
                this._Ack.SetTo(this._ToCommit.Sum(this._ToAbort, uint.MaxValue));
#else
                UIntSet _new_Completed = this._ToCommit.Sum(this._ToAbort, uint.MaxValue);
#if DEBUG_MonotonicChecks
                Value.MonotonicCheck<UIntSet>(this._Completed, _new_Completed);
#endif
                this._Completed.SetTo(_new_Completed);
#endif
#else
                System.Diagnostics.Debug.Assert(false);
#endif

#if DEBUG_SetAcknowledgement
#else
                UInt _new_Ack = this._Completed.MaxContiguous(this._Ack);
#if DEBUG_MonotonicChecks
                Value.MonotonicCheck<UInt>(this._Ack, _new_Ack);
#endif
                this._Ack.SetTo(_new_Ack);
#endif

#if DEBUG_SetAcknowledgement
                if (this._Clean.IsDefined)
                {
                    this._CommitOk.SetTo(this._CommitOk.Substract(this._Clean, uint.MaxValue));
                    this._AbortOk.SetTo(this._AbortOk.Substract(this._Clean, uint.MaxValue));
                }
#else
                if (this._Clean.IsDefined && this._Clean.Number > 0)
                {
                    this._CommitOk = this._CommitOk.Cut(this._Clean.Number);
                    this._AbortOk = this._AbortOk.Cut(this._Clean.Number);
                }
#endif
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_updated_lower_bind_o
/*
            this._CommitOk.SetTo(((Bind)lowercontrol)._CommitOk);
            this._AbortOk.SetTo(((Bind)lowercontrol)._AbortOk);

            UInt _new_Ack = ((Bind)lowercontrol)._Ack;
#if DEBUG_MonotonicChecks
            Value.MonotonicCheck<UInt>(this._Ack, _new_Ack);
#endif
            this._Ack.SetTo(_new_Ack);
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_updated_lower_peer_o
/*
            // [rule] CommitOk := children.CommitOk
            this._CommitOk.SetTo(((Peer)lowercontrol)._aggr_CommitOk.Value);
            // [rule] AbortOk := children.AbortOk
            this._AbortOk.SetTo(((Peer)lowercontrol)._aggr_AbortOk.Value);
            // [rule] Ack := children.Ack
            UInt _new_Ack = ((Peer)lowercontrol)._aggr_Ack.Value;
#if DEBUG_MonotonicChecks
            Value.MonotonicCheck<UInt>(this._Ack, _new_Ack);
#endif
            this._Ack.SetTo(_new_Ack);
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_upper_consume_1_o
/*
            this._diss_ToCommit.UpdateTo(new Versioned<UIntSet>(version, uppercontrol._ToCommit));
            this._diss_ToAbort.UpdateTo(new Versioned<UIntSet>(version, uppercontrol._ToAbort));
#if DEBUG_SetAcknowledgement
            this._diss_Clean.UpdateTo(new Versioned<UIntSet>(version, uppercontrol._Clean));
#else
            Versioned<UInt> _new_diss_Clean = new Versioned<UInt>(version, uppercontrol._Clean);
#if DEBUG_MonotonicChecks
            Versioned<UInt>.MonotonicCheck(this._diss_Clean, _new_diss_Clean);
#endif
            this._diss_Clean.UpdateTo(_new_diss_Clean);
#endif
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_upper_consume_2_arguments_o
/*
            incoming._diss_ToCommit, incoming._diss_ToAbort, incoming._diss_Clean
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_upper_consume_2_parameters_o
/*
            Versioned<UIntSet> _incoming_diss_ToCommit,
            Versioned<UIntSet> _incoming_diss_ToAbort,
#if DEBUG_SetAcknowledgement
            Versioned<UIntSet> _incoming_diss_Clean
#else
            Versioned<UInt> _incoming_diss_Clean
#endif
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_upper_consume_2_o
/*
            if (this._diss_ToCommit.UpdateTo(_incoming_diss_ToCommit))
                updatedupper2 = true;
            if (this._diss_ToAbort.UpdateTo(_incoming_diss_ToAbort))
                updatedupper2 = true;

            if (_incoming_diss_Clean.Version.IsDefined && _incoming_diss_Clean.Value != null)
            {
#if DEBUG_MonotonicChecks
                Versioned<UInt>.MonotonicCheck(this._diss_Clean, _incoming_diss_Clean);
#endif
                if (this._diss_Clean.UpdateTo(_incoming_diss_Clean))
                    updatedupper2 = true;
            }
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_upper_consume_3_o
/*
            // [rule] Clean += parent.Clean
#if DEBUG_SetAcknowledgement
            this._Clean = this._Clean.Sum(this._diss_Clean.Value, uint.MaxValue);
#else
            this._Clean.Number = Math.Max(this._Clean.Number, this._diss_Clean.Value.Number);
#endif

#if DEBUG_DisseminateDecisionsWithoutMerging
            // [rule] ToCommit := parent.ToCommit
            this._ToCommit = this._diss_ToCommit.Value.Clone();
#else
            // [rule] ToCommit += parent.ToCommit \ Clean
#if DEBUG_SetAcknowledgement
            this._ToCommit = this._ToCommit.Sum(this._diss_ToCommit.Value, uint.MaxValue).Substract(this._Clean, uint.MaxValue);
#else
            this._ToCommit = this._ToCommit.Sum(this._diss_ToCommit.Value, uint.MaxValue).Cut(this._Clean.Number);
#endif
#endif

#if DEBUG_DisseminateDecisionsWithoutMerging
            // [rule] ToAbort := parent.ToAbort
            this._ToAbort = this._diss_ToAbort.Value.Clone();
#else
            // [rule] ToAbort += parent.ToAbort \ Clean
#if DEBUG_SetAcknowledgement
            this._ToAbort = this._ToAbort.Sum(this._diss_ToAbort.Value, uint.MaxValue).Substract(this._Clean, uint.MaxValue);
#else
            this._ToAbort = this._ToAbort.Sum(this._diss_ToAbort.Value, uint.MaxValue).Cut(this._Clean.Number);
#endif
#endif
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _root_updated_lower_o
/*
            // [rule] CommitOk := children.CommitOk
            UIntSet _new_CommitOk = ((Peer)lowercontrol)._aggr_CommitOk.Value;
            // #if DEBUG_MonotonicChecks
            //                 Value.MonotonicCheck<UIntSet>(this._CommitOk, _new_CommitOk);
            // #endif
            this._CommitOk.SetTo(_new_CommitOk);
            // [rule] AbortOk := children.AbortOk
            UIntSet _new_AbortOk = ((Peer)lowercontrol)._aggr_AbortOk.Value;
            // #if DEBUG_MonotonicChecks
            //                 Value.MonotonicCheck<UIntSet>(this._AbortOk, _new_AbortOk);
            // #endif
            this._AbortOk.SetTo(_new_AbortOk);
            // [rule] Ack := children.Ack
            UInt _new_Ack = ((Peer)lowercontrol)._aggr_Ack.Value;
#if DEBUG_MonotonicChecks
            Value.MonotonicCheck<UInt>(this._Ack, _new_Ack);
#endif
            this._Ack.SetTo(_new_Ack);
            // [rule] global.ToCommit := CommitOk
            if (this._CommitOk.IsDefined)
            {
                // #if DEBUG_MonotonicChecks
                //                     Value.MonotonicCheck<UIntSet>(this._ToCommit, this._CommitOk);
                // #endif
                this._ToCommit.SetTo(this._CommitOk);
            }
            // [rule] global.ToAbort := AbortOk
            if (this._AbortOk.IsDefined)
            {
                // #if DEBUG_MonotonicChecks
                //                     Value.MonotonicCheck<UIntSet>(this._ToAbort, this._AbortOk);
                // #endif
                this._ToAbort.SetTo(this._AbortOk);
            }
            // [rule] global.Clean := Ack
            if (this._Ack.IsDefined)
            {
#if DEBUG_MonotonicChecks
                Value.MonotonicCheck<UInt>(this._Clean, this._Ack);
#endif
                this._Clean.SetTo(this._Ack);
            }
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _check_conditions_o
/*
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            // _aggregationtoken_fields_o
/*
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _aggr_CommitOk;
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _aggr_AbortOk;
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public Versioned<UIntSet> _aggr_Ack;
#else
            public Versioned<UInt> _aggr_Ack;
#endif
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _diss_ToCommit;
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _diss_ToAbort;
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public Versioned<UIntSet> _diss_Clean;
#else
            public Versioned<UInt> _diss_Clean;
#endif
*/

            // _aggregationtoken_serializableinfo_o
/*
            info.AddAnother(_aggr_CommitOk.SerializableInfo);
            info.AddAnother(_aggr_AbortOk.SerializableInfo);
            info.AddAnother(_aggr_Ack.SerializableInfo);
            info.AddAnother(_diss_ToCommit.SerializableInfo);
            info.AddAnother(_diss_ToAbort.SerializableInfo);
            info.AddAnother(_diss_Clean.SerializableInfo);
*/

            // _aggregationtoken_serializeto_o
/*
            _aggr_CommitOk.SerializeTo(ref header, ref data);
            _aggr_AbortOk.SerializeTo(ref header, ref data);
            _aggr_Ack.SerializeTo(ref header, ref data);
            _diss_ToCommit.SerializeTo(ref header, ref data);
            _diss_ToAbort.SerializeTo(ref header, ref data);
            _diss_Clean.SerializeTo(ref header, ref data);
*/

            // _aggregationtoken_deserializefrom_o
/*
            _aggr_CommitOk.DeserializeFrom(ref header, ref data);
            _aggr_AbortOk.DeserializeFrom(ref header, ref data);
            _aggr_Ack.DeserializeFrom(ref header, ref data);
            _diss_ToCommit.DeserializeFrom(ref header, ref data);
            _diss_ToAbort.DeserializeFrom(ref header, ref data);
            _diss_Clean.DeserializeFrom(ref header, ref data);
*/

            // _disseminationtoken_fields_o
/*
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _aggr_CommitOk;
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _aggr_AbortOk;
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public Versioned<UIntSet> _aggr_Ack;
#else
            public Versioned<UInt> _aggr_Ack;
#endif
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _diss_ToCommit;
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _diss_ToAbort;
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public Versioned<UIntSet> _diss_Clean;
#else
            public Versioned<UInt> _diss_Clean;
#endif
*/

            // _disseminationtoken_serializableinfo_o
/*
            info.AddAnother(_aggr_CommitOk.SerializableInfo);
            info.AddAnother(_aggr_AbortOk.SerializableInfo);
            info.AddAnother(_aggr_Ack.SerializableInfo);
            info.AddAnother(_diss_ToCommit.SerializableInfo);
            info.AddAnother(_diss_ToAbort.SerializableInfo);
            info.AddAnother(_diss_Clean.SerializableInfo);
*/

            // _disseminationtoken_serializeto_o
/*
            _aggr_CommitOk.SerializeTo(ref header, ref data);
            _aggr_AbortOk.SerializeTo(ref header, ref data);
            _aggr_Ack.SerializeTo(ref header, ref data);
            _diss_ToCommit.SerializeTo(ref header, ref data);
            _diss_ToAbort.SerializeTo(ref header, ref data);
            _diss_Clean.SerializeTo(ref header, ref data);
*/

            // _disseminationtoken_deserializefrom_o
/*
            _aggr_CommitOk.DeserializeFrom(ref header, ref data);
            _aggr_AbortOk.DeserializeFrom(ref header, ref data);
            _aggr_Ack.DeserializeFrom(ref header, ref data);
            _diss_ToCommit.DeserializeFrom(ref header, ref data);
            _diss_ToAbort.DeserializeFrom(ref header, ref data);
            _diss_Clean.DeserializeFrom(ref header, ref data);
*/

// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            // _control_additional_fields_o
/*
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public UIntSet _CommitOk;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public UIntSet _AbortOk;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public UIntSet _Ack;
#else
            public UInt _Ack;
#endif
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public UIntSet _ToCommit;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public UIntSet _ToAbort;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public UIntSet _Clean;
#else
            public UInt _Clean;
#endif
 */ 

            // _bind_additional_fields_o
/*
#if DEBUG_SetAcknowledgement
#else
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public UIntSet _Completed;
#endif 
*/

            // _peer_additional_fields_o
/*
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _aggr_CommitOk;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _aggr_AbortOk;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public Versioned<UIntSet> _aggr_Ack;
#else
            public Versioned<UInt> _aggr_Ack;
#endif
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _diss_ToCommit;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _diss_ToAbort;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public Versioned<UIntSet> _diss_Clean;
#else
            public Versioned<UInt> _diss_Clean;
#endif
*/

            // _root_additional_fields_o
/*
*/

// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            // _bind_register_callbacks_o
/*
            endpoint.OnCommittable += new CommittableCallback(this._OnCommittableCallback);
            endpoint.OnAbortable += new AbortableCallback(this._OnAbortableCallback);
*/

            // _bind_callbacks_o
/*
            #region _OnCommittableCallback

            private void _OnCommittableCallback(uint m)
            {
                lock (this)
                {
                    this._CommitOk.Insert(m);

                    //                    this._Completed.SetTo(this._ToCommit.Sum(this._ToAbort, uint.MaxValue));

                    if (uppercontrol != null)
                        uppercontrol._UpdatedLower();
                }
            }

            #endregion

            #region _OnAbortableCallback

            private void _OnAbortableCallback(uint m)
            {
                lock (this)
                {
                    this._AbortOk.Insert(m);

                    //                    this._Completed.SetTo(this._ToCommit.Sum(this._ToAbort, uint.MaxValue));

                    if (uppercontrol != null)
                        uppercontrol._UpdatedLower();
                }
            }

            #endregion
 */

// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
}
