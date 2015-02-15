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
            this._Last = new UInt(0);
#if DEBUG_LogPropertyValueAssignments
            logger.Log("_Last = " + this._Last.ToString());
#endif
            this._Next = UInt.Undefined.Clone();
#if DEBUG_LogPropertyValueAssignments
            logger.Log("_Next = " + this._Next.ToString());
#endif
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_initialization_o
/*
            this._Last = UInt.Undefined.Clone();
#if DEBUG_LogPropertyValueAssignments
            logger.Log("_Last = " + this._Last.ToString());
#endif
            this._Next = UInt.Undefined.Clone();
#if DEBUG_LogPropertyValueAssignments
            logger.Log("_Next = " + this._Next.ToString());
#endif
            this._aggr_Last = new Versioned<UInt>(QS.Fx.Runtime.Version.Undefined, UInt.Undefined.Clone());
#if DEBUG_LogPropertyValueAssignments
            logger.Log("_aggr_Last = " + this._aggr_Last.ToString());
#endif
            this._diss_Next = new Versioned<UInt>(QS.Fx.Runtime.Version.Undefined, UInt.Undefined.Clone());
#if DEBUG_LogPropertyValueAssignments
            logger.Log("_diss_Next = " + this._diss_Next.ToString());
#endif
            this._diss_Last = new Versioned<UInt>(QS.Fx.Runtime.Version.Undefined, UInt.Undefined.Clone());
#if DEBUG_LogPropertyValueAssignments
            logger.Log("_diss_Last = " + this._diss_Last.ToString());
#endif
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _root_initialization_o
/*
            this._Last = UInt.Undefined.Clone();
#if DEBUG_LogPropertyValueAssignments
            logger.Log("_Last = " + this._Last.ToString());
#endif
            this._Next = UInt.Undefined.Clone();
#if DEBUG_LogPropertyValueAssignments
            logger.Log("_Next = " + this._Next.ToString());
#endif
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_aggregate_1_o
/*
            if (peercontext.IsRegular)
            {
                outgoing._isset_aggr_Last = true;
                outgoing._aggr_Last = new Versioned<UInt>(version, this._Last.Clone());
            }
            else
            {
                outgoing._isset_aggr_Last = false;
                outgoing._aggr_Last = new Versioned<UInt>(version, UInt.Undefined.Clone());
            }

            outgoing._diss_Next = this._diss_Next.Clone();
            outgoing._diss_Last = this._diss_Last.Clone();
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_aggregate_2_o
/*
            if (peercontext.IsRegular)
            {
                outgoing._isset_aggr_Last = true;
                outgoing._aggr_Last = new Versioned<UInt>(incoming._aggr_Last.Version,
                    incoming._isset_aggr_Last ? UInt.Min(incoming._aggr_Last.Value, this._Last) : this._Last.Clone());
            }
            else
            {
                outgoing._isset_aggr_Last = incoming._isset_aggr_Last;
                outgoing._aggr_Last = incoming._aggr_Last.Clone();
            }

            outgoing._diss_Next.UpdateTo(this._diss_Next);
            outgoing._diss_Last.UpdateTo(this._diss_Last);
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_aggregate_3_o
/*
            if (version.CompareTo(this._aggr_Last.Version) > 0)
            {
                Versioned<UInt> _new_aggr_Last;
                bool _isset_aggr_Last;
                if (peercontext.IsRegular)
                {
                    _isset_aggr_Last = true;
                    _new_aggr_Last = new Versioned<UInt>(incoming._aggr_Last.Version,
                        incoming._isset_aggr_Last ? UInt.Min(incoming._aggr_Last.Value, this._Last) : this._Last.Clone());
                }
                else
                {
                    _isset_aggr_Last = incoming._isset_aggr_Last;
                    _new_aggr_Last = incoming._aggr_Last.Clone();
                }

                if (_isset_aggr_Last && _new_aggr_Last.Version.IsDefined && _new_aggr_Last.Value.IsDefined)
                {
#if DEBUG_MonotonicChecks
                    Versioned<UInt>.MonotonicCheck(this._aggr_Last, _new_aggr_Last);
#endif
                    this._aggr_Last = _new_aggr_Last;

#if DEBUG_LogPropertyValueAssignments
                    logger.Log("_aggr_Last = " + this._aggr_Last.ToString());
#endif
                }
            }
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_disseminate_1_o
/*
            outgoing._aggr_Last = this._aggr_Last.Clone();
            outgoing._diss_Next = this._diss_Next.Clone();
            outgoing._diss_Last = this._diss_Last.Clone();
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_disseminate_2_and_3_o
/*
#if DEBUG_MonotonicChecks
            Versioned<UInt>.MonotonicCheck(this._aggr_Last, incoming._aggr_Last);
#endif
            this._aggr_Last.UpdateTo(incoming._aggr_Last);

#if DEBUG_LogPropertyValueAssignments
            logger.Log("_aggr_Last = " + this._aggr_Last.ToString());
#endif
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _bind_updated_upper_o
/*
            bool _updated_Next = false, _updated_Last = false;
            if (uppercontrol._Next.IsDefined)
            {
                UInt _new_Next = uppercontrol._Next;
                if (_new_Next.IsDefined)
                {
#if DEBUG_MonotonicChecks
                    Value.MonotonicCheck<UInt>(this._Next, _new_Next);
#endif
                    _updated_Next = _new_Next.Number != this._Next.Number;
                    this._Next.SetTo(_new_Next);
#if DEBUG_LogPropertyValueAssignments
                    logger.Log("_Next = " + this._Next.ToString());
#endif
                    if (_updated_Next)
                        endpoint.Phase(this._Next.Number);
                }
            }
            if (_updated_Next && this._Next.IsDefined)
            {
                UInt _new_Last = this._Next.Clone();
#if DEBUG_MonotonicChecks
                Value.MonotonicCheck<UInt>(this._Last, _new_Last);
#endif
                _updated_Last = _new_Last.Number != this._Last.Number;
                this._Last.SetTo(_new_Last);
#if DEBUG_LogPropertyValueAssignments
                logger.Log("_Last = " + this._Last.ToString());
#endif
                if (_updated_Last)
                    uppercontrol._UpdatedLower();
            }
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_updated_lower_bind_o
/*
            UInt _new_Last = ((Bind)lowercontrol)._Last;
            if (_new_Last.IsDefined)
            {
#if DEBUG_MonotonicChecks
                Value.MonotonicCheck<UInt>(this._Last, _new_Last);
#endif
                this._Last.SetTo(_new_Last);

#if DEBUG_LogPropertyValueAssignments
                logger.Log("_Last = " + this._Last.ToString());
#endif
            }
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_updated_lower_peer_o
/*
            UInt _new_Last = ((Peer)lowercontrol)._aggr_Last.Value;
            if (_new_Last.IsDefined)
            {
#if DEBUG_MonotonicChecks
                Value.MonotonicCheck<UInt>(this._Last, _new_Last);
#endif
                this._Last.SetTo(_new_Last);

#if DEBUG_LogPropertyValueAssignments
                logger.Log("_Last = " + this._Last.ToString());
#endif
            }
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_upper_consume_1_o
/*
            if (uppercontrol._Next.IsDefined)
            {
                Versioned<UInt> _new_diss_Next = new Versioned<UInt>(version, uppercontrol._Next);
#if DEBUG_MonotonicChecks
                Versioned<UInt>.MonotonicCheck(this._diss_Next, _new_diss_Next);
#endif
                this._diss_Next.UpdateTo(_new_diss_Next);

#if DEBUG_LogPropertyValueAssignments
                logger.Log("_diss_Next = " + this._diss_Next.ToString());
#endif
            }

            if (uppercontrol._Last.IsDefined)
            {
                Versioned<UInt> _new_diss_Last = new Versioned<UInt>(version, uppercontrol._Last);
#if DEBUG_MonotonicChecks
                Versioned<UInt>.MonotonicCheck(this._diss_Last, _new_diss_Last);
#endif
                this._diss_Last.UpdateTo(_new_diss_Last);

#if DEBUG_LogPropertyValueAssignments
                logger.Log("_diss_Last = " + this._diss_Last.ToString());
#endif
            }
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_upper_consume_2_arguments_o
/*
                incoming._diss_Next, incoming._diss_Last
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_upper_consume_2_parameters_o
/*
QS.Fx.Runtime.Version version, Versioned<UInt> _incoming_diss_Next, Versioned<UInt> _incoming_diss_Last
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_upper_consume_2_o
/*
            if (_incoming_diss_Next.Version.IsDefined && _incoming_diss_Next.Value.IsDefined)
            {
#if DEBUG_MonotonicChecks
                Versioned<UInt>.MonotonicCheck(this._diss_Next, _incoming_diss_Next);
#endif
                if (this._diss_Next.UpdateTo(_incoming_diss_Next))
                    updatedupper2 = true;

#if DEBUG_LogPropertyValueAssignments
                logger.Log("_diss_Next = " + this._diss_Next.ToString());
#endif
            }

            if (_incoming_diss_Last.Version.IsDefined && _incoming_diss_Last.Value.IsDefined)
            {
#if DEBUG_MonotonicChecks
                Versioned<UInt>.MonotonicCheck(this._diss_Last, _incoming_diss_Last);
#endif
                if (this._diss_Last.UpdateTo(_incoming_diss_Last))
                    updatedupper2 = true;

#if DEBUG_LogPropertyValueAssignments
                logger.Log("_diss_Last = " + this._diss_Last.ToString());
#endif
            }
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _peer_upper_consume_3_o
/*
            if (this._diss_Next.Version.IsDefined && this._diss_Next.Value.IsDefined
#if OPTION_RejectDisseminatedParentValuesFromOldCofigurationIncarnations
                && this._diss_Next.Version.Major >= version.Major
#endif
)
            {
                this._Next.SetTo(this._diss_Next.Value);

#if DEBUG_LogPropertyValueAssignments
                logger.Log("_Next = " + this._Next.ToString());
#endif
            }
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _root_updated_lower_o
/*
            bool _updated_Last = false;
            UInt _new_Last = ((Peer)lowercontrol)._aggr_Last.Value;
            if (_new_Last.IsDefined)
            {
#if DEBUG_MonotonicChecks
                Value.MonotonicCheck<UInt>(this._Last, _new_Last);
#endif
                if (_new_Last.IsDefined != this._Last.IsDefined || _new_Last.Number != this._Last.Number)
                    _updated_Last = true;
                this._Last.SetTo(_new_Last);
#if DEBUG_LogPropertyValueAssignments
                logger.Log("_Last = " + this._Last.ToString());
#endif
            }
            bool _updated_Next = false;
            if (_updated_Last)
            {
                UInt _new_Next = UInt.Sum(this._Last, UInt.Number1);
#if DEBUG_MonotonicChecks
                Value.MonotonicCheck<UInt>(this._Next, _new_Next);
#endif
                if (_new_Next.IsDefined != this._Next.IsDefined || _new_Next.Number != this._Next.Number)
                    _updated_Next = true;
                this._Next.SetTo(_new_Next);
#if DEBUG_LogPropertyValueAssignments
                logger.Log("_Next = " + this._Next.ToString());
#endif
            }
*/

// ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            // _check_conditions_o
/*
            if (this._aggr_Last.Version.IsDefined && this._aggr_Last.Value.IsDefined && this._Last.IsDefined)
            {
                if (this._aggr_Last.Value.IsLess(this._Last))
                {
#if DEBUG_LogPropertyValueAssignments
                    logger.Log("CONDITION SATISFIED : " + this._aggr_Last.Value.ToString() + " <= " + this._Last.ToString());
#endif

                    _Upgrade();
                }
                else
                {
                    _Degrade();
                }
            }
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
            public bool _isset_aggr_Last;
            [QS.Fx.Printing.Printable]
            public Versioned<UInt> _aggr_Last;
            [QS.Fx.Printing.Printable]
            public Versioned<UInt> _diss_Next;
            [QS.Fx.Printing.Printable]
            public Versioned<UInt> _diss_Last;
*/

            // _aggregationtoken_serializableinfo_o
/*
            QS.CMS.Base3.SerializationHelper.ExtendSerializableInfo_Bool(ref info);
            info.AddAnother(_aggr_Last.SerializableInfo);
            info.AddAnother(_diss_Next.SerializableInfo);
            info.AddAnother(_diss_Last.SerializableInfo);
*/

            // _aggregationtoken_serializeto_o
/*
            QS.CMS.Base3.SerializationHelper.Serialize_Bool(ref header, ref data, _isset_aggr_Last);
            _aggr_Last.SerializeTo(ref header, ref data);
            _diss_Next.SerializeTo(ref header, ref data);
            _diss_Last.SerializeTo(ref header, ref data);
*/

            // _aggregationtoken_deserializefrom_o
/*
            _isset_aggr_Last = QS.CMS.Base3.SerializationHelper.Deserialize_Bool(ref header, ref data);
            _aggr_Last.DeserializeFrom(ref header, ref data);
            _diss_Next.DeserializeFrom(ref header, ref data);
            _diss_Last.DeserializeFrom(ref header, ref data);
*/

            // _disseminationtoken_fields_o
/*
            [QS.Fx.Printing.Printable]
            public Versioned<UInt> _aggr_Last;
            [QS.Fx.Printing.Printable]
            public Versioned<UInt> _diss_Next;
            [QS.Fx.Printing.Printable]
            public Versioned<UInt> _diss_Last;
*/

            // _disseminationtoken_serializableinfo_o
/*
            info.AddAnother(_aggr_Last.SerializableInfo);
            info.AddAnother(_diss_Next.SerializableInfo);
            info.AddAnother(_diss_Last.SerializableInfo);
*/

            // _disseminationtoken_serializeto_o
/*
            _aggr_Last.SerializeTo(ref header, ref data);
            _diss_Next.SerializeTo(ref header, ref data);
            _diss_Last.SerializeTo(ref header, ref data);
*/

            // _disseminationtoken_deserializefrom_o
/*
            _aggr_Last.DeserializeFrom(ref header, ref data);
            _diss_Next.DeserializeFrom(ref header, ref data);
            _diss_Last.DeserializeFrom(ref header, ref data);
*/

// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            // _control_additional_fields_o
/*
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public UInt _Last;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public UInt _Next;
*/ 

            // _bind_additional_fields_o
/*
*/

            // _peer_additional_fields_o
/*
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public Versioned<UInt> _aggr_Last;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public Versioned<UInt> _diss_Next;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public Versioned<UInt> _diss_Last;
*/

            // _root_additional_fields_o
/*
*/

// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            // _bind_register_callbacks_o
/*
*/

            // _bind_callbacks_o
/*
*/

// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
}
