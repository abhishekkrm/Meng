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

// #define DEBUG_SetAcknowledgement
#define DEBUG_DisseminateDecisionsWithoutMerging
// #define DEBUG_LogGenerously
// #define DEBUG_LogGenerously_2
// #define DEBUG_MonotonicChecks

using System;
using System.Collections.Generic;
using System.Text;

using QS._qss_x_.Runtime_1_;
using QS._qss_x_.Runtime_2_;

namespace QS._qss_x_.Protocols_
{
    public sealed class Simple2PC : QS._qss_x_.Agents_.Base.IProtocol
    {        
        // constants

        #region Constants

        public static uint MaximumNumberOfRangesForNetworkCommunication = 100;

        #endregion

        //constructor

        #region Constructor

        public static QS._qss_x_.Agents_.Base.IProtocol Protocol
        {
            get { return protocol; }
        }

        private static readonly Simple2PC protocol = new Simple2PC();

        private Simple2PC()
        {
        }

        #endregion

        // plumbing

        #region IProtocol.Name

        string QS._qss_x_.Agents_.Base.IProtocol.Name
        {
            get { return "Simple2PC"; }
        }

        #endregion

        #region IProtocol.Interface

        Type QS._qss_x_.Agents_.Base.IProtocol.Interface
        {
            get { return typeof(IEndpoint); }
        }

        #endregion

        #region IProtocol.Bind

        QS._qss_x_.Agents_.Base.IProtocolBind QS._qss_x_.Agents_.Base.IProtocol.Bind(object endpoint)
        {
            System.Diagnostics.Debug.Assert((endpoint != null) && typeof(IEndpoint).IsAssignableFrom(endpoint.GetType()));
            return new Bind((IEndpoint) endpoint);
        }

        #endregion

        #region IProtocol.Peer

        QS._qss_x_.Agents_.Base.IProtocolPeer QS._qss_x_.Agents_.Base.IProtocol.Peer(QS._qss_x_.Agents_.Base.IProtocolPeerContext peercontext)
        {
            return new Peer(peercontext);
        }

        #endregion

        #region IProtocol.Root

        QS._qss_x_.Agents_.Base.IProtocolRoot QS._qss_x_.Agents_.Base.IProtocol.Root()
        {
            return new Root();
        }

        #endregion

        // interface

        #region Interface IEndpoint

        public delegate void CommittableCallback(uint m);
        public delegate void AbortableCallback(uint m);

        public interface IEndpoint
        {
            event CommittableCallback OnCommittable;
            event AbortableCallback OnAbortable;
            void Commit(uint m);
            void Abort(uint m);
        }

        #endregion

        // controllers

        #region Class Control
        
        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        private abstract class Control : QS.Fx.Inspection.Inspectable, QS._qss_x_.Agents_.Base.IProtocolControl
        {
            #region Constructor

            public Control()
            {
                lock (this)
                {
                    _Initialization();
                }
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            public Control uppercontrol, lowercontrol;
            [QS.Fx.Base.Inspectable]
            public QS.Fx.Logging.ILogger logger;

            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
            public USET _CommitOk;
            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
            public USET _AbortOk;
            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public UIntSet _Ack;
#else
            public UINT _Ack;
#endif
            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
            public USET _ToCommit;
            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
            public USET _ToAbort;
            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public UIntSet _Clean;
#else
            public UINT _Clean;
#endif

            #endregion

            #region IProtocolControl.Initialize

            void QS._qss_x_.Agents_.Base.IProtocolControl.Initialize(QS.Fx.Logging.ILogger logger)
            {
                this.logger = logger;
            }

            #endregion

            #region IProtocolControl.ConnectUpper

            void QS._qss_x_.Agents_.Base.IProtocolControl.ConnectUpper(QS._qss_x_.Agents_.Base.IProtocolControl control)
            {
                lock (this)
                {
                    uppercontrol = (Control) control;
                }
            }

            #endregion

            #region IProtocolControl.ConnectLower

            void QS._qss_x_.Agents_.Base.IProtocolControl.ConnectLower(QS._qss_x_.Agents_.Base.IProtocolControl control)
            {
                lock (this)
                {
                    lowercontrol = (Control) control;
                }
            }

            #endregion

            #region _Initialization

            public virtual void _Initialization()
            {
            }

            #endregion

            #region _UpdatedLower

            public virtual void _UpdatedLower()
            {
            }

            #endregion

            #region _UpdatedUpper

            public virtual void _UpdatedUpper()
            {
            }

            #endregion
        }

        #endregion

        #region Class Bind

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        private sealed class Bind : Control, QS._qss_x_.Agents_.Base.IProtocolBind
        {
            #region Constructor

            public Bind(IEndpoint endpoint)
            {
                lock (this)
                {
                    this.endpoint = endpoint;

                    endpoint.OnCommittable += new CommittableCallback(this._OnCommittableCallback);
                    endpoint.OnAbortable += new AbortableCallback(this._OnAbortableCallback);
                }

                _Initialization();
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            public IEndpoint endpoint;

#if DEBUG_SetAcknowledgement
#else
            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
            public USET _Completed;
#endif
            #endregion

            #region _Initialization

            public override void _Initialization()
            {
                lock (this)
                {
                    this._CommitOk = USET.EmptySet.Clone();
                    this._AbortOk = USET.EmptySet.Clone();
#if DEBUG_SetAcknowledgement
                    this._Ack = UIntSet.EmptySet.Clone();
#else
                    this._Ack = new UINT(0);
                    this._Completed = USET.EmptySet.Clone();
#endif
                    this._ToCommit = USET.EmptySet.Clone();
                    this._ToAbort = USET.EmptySet.Clone();
#if DEBUG_SetAcknowledgement
                    this._Clean = UIntSet.EmptySet.Clone();
#else
                    this._Clean = new UINT(0);
#endif
                }
            }

            #endregion

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

            #region _UpdatedLower

            public override void _UpdatedLower()
            {
                System.Diagnostics.Debug.Assert(false);
            }

            #endregion

            #region _UpdatedUpper

            public override void _UpdatedUpper()
            {
#if DEBUG_SetAcknowledgement
                UIntSet _new_Clean = uppercontrol._Clean;
#else
                UINT _new_Clean = uppercontrol._Clean;
#endif

#if DEBUG_DisseminateDecisionsWithoutMerging
                USET _new_ToCommit = uppercontrol._ToCommit.Clone();
                USET _new_ToAbort = uppercontrol._ToAbort.Clone();
#else
#if DEBUG_SetAcknowledgement
                UIntSet _new_ToCommit = uppercontrol._ToCommit.Substract(_new_Clean, uint.MaxValue);
                UIntSet _new_ToAbort = uppercontrol._ToAbort.Substract(_new_Clean, uint.MaxValue);
#else
                UIntSet _new_ToCommit = uppercontrol._ToCommit.Cut(_new_Clean.Number);
                UIntSet _new_ToAbort = uppercontrol._ToAbort.Cut(_new_Clean.Number);
#endif
#endif

                USET _added_ToCommit = _new_ToCommit.Substract_2(this._ToCommit, uint.MaxValue);
                USET _added_ToAbort = _new_ToAbort.Substract_2(this._ToAbort, uint.MaxValue);

//                UIntSet _added_Clean = _new_Clean.Substract(this._Clean);

//                logger.Log("SUBSTRACT(1) : \nArgument1 = " + _new_ToCommit.ToString() + "\nArgument2 = " + this._ToCommit.ToString() + 
//                    "\nArgument1 - Argument2 = " + _added_ToCommit.ToString());

//                logger.Log("SUBSTRACT(2) : \nArgument1 = " + _new_ToAbort.ToString() + "\nArgument2 = " + this._ToAbort.ToString() +
//                    "\nArgument1 - Argument2 = " + _added_ToAbort.ToString());

                if (_added_ToCommit.Ranges != null)
                {
                    foreach (QS._qss_c_.Base1_.Range<uint> _r in _added_ToCommit.Ranges)
                    {
                        for (uint _m = _r.From; _m <= Math.Min(_r.To, _added_ToCommit.MaxCovered); _m++)
                            endpoint.Commit(_m);
                    }
                }

                if (_added_ToAbort.Ranges != null)
                {
                    foreach (QS._qss_c_.Base1_.Range<uint> _r in _added_ToAbort.Ranges)
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
                USET _new_Completed = this._ToCommit.Sum(this._ToAbort, uint.MaxValue);
#if DEBUG_MonotonicChecks
                Value.MonotonicCheck<UIntSet>(this._Completed, _new_Completed);
#endif
                this._Completed.SetTo(_new_Completed);
#endif
#else
                System.Diagnostics.Debug.Assert(false);
#endif

/*
                if (this._ToCommit.IsDefined)
                {
#if DEBUG_SetAcknowledgement
                    this._Ack.SetTo(this._Ack.Sum(this._ToCommit, uint.MaxValue));
#else
                    this._Completed.SetTo(this._Completed.Sum(this._ToCommit, uint.MaxValue));
#endif
                }

                if (this._ToAbort.IsDefined)
                {
#if DEBUG_SetAcknowledgement
                    this._Ack.SetTo(this._Ack.Sum(this._ToAbort, uint.MaxValue));
#else
                    this._Completed.SetTo(this._Completed.Sum(this._ToAbort, uint.MaxValue));
#endif
                }
*/

#if DEBUG_SetAcknowledgement
#else
                UINT _new_Ack = this._Completed.MaxContiguous(this._Ack);
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
            }

            #endregion
        }

        #endregion

        #region Class Peer

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        private sealed class Peer : Control, QS._qss_x_.Agents_.Base.IProtocolPeer, QS._qss_x_.Agents_.Base.IProtocolControl
        {
            #region Constructor

            public Peer(QS._qss_x_.Agents_.Base.IProtocolPeerContext peercontext) : base()
            {
                this.peercontext = peercontext;
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            public bool updatedupper1, updatedupper2;
            [QS.Fx.Base.Inspectable]
            public QS._qss_x_.Agents_.Base.IProtocolPeerContext peercontext;

            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
            public VERSIONED<USET> _aggr_CommitOk;
            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
            public VERSIONED<USET> _aggr_AbortOk;
            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public Versioned<UIntSet> _aggr_Ack;
#else
            public VERSIONED<UINT> _aggr_Ack;
#endif
            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
            public VERSIONED<USET> _diss_ToCommit;
            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
            public VERSIONED<USET> _diss_ToAbort;
            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public Versioned<UIntSet> _diss_Clean;
#else
            public VERSIONED<UINT> _diss_Clean;
#endif

            #endregion

            #region IProtocolControl.Initialize

            void QS._qss_x_.Agents_.Base.IProtocolControl.Initialize(QS.Fx.Logging.ILogger logger)
            {
                this.logger = logger;
                logger.Log("This peer agent is starting as " + (peercontext.IsRegular ? "a normal" : "an associate") + " member of the peer group.");
            }

            #endregion

            #region _Initialization

            public override void _Initialization()
            {
                lock (this)
                {
                    this._CommitOk = USET.Undefined.Clone();
                    this._AbortOk = USET.Undefined.Clone();
#if DEBUG_SetAcknowledgement
                    this._Ack = UIntSet.Undefined.Clone();
#else
                    this._Ack = new UINT(0);
#endif
                    this._ToCommit = USET.EmptySet.Clone();
                    this._ToAbort = USET.EmptySet.Clone();
#if DEBUG_SetAcknowledgement
                    this._Clean = UIntSet.EmptySet.Clone();
#else
                    this._Clean = new UINT(0);
#endif

                    this._aggr_CommitOk = new VERSIONED<USET>(QS._qss_x_.Runtime_1_.VERSION.Undefined, USET.Undefined.Clone());
                    this._aggr_AbortOk = new VERSIONED<USET>(QS._qss_x_.Runtime_1_.VERSION.Undefined, USET.Undefined.Clone());
#if DEBUG_SetAcknowledgement
                    this._aggr_Ack = new Versioned<UIntSet>(QS.Fx.Runtime.Version.Undefined, null);
#else
                    this._aggr_Ack = new VERSIONED<UINT>(QS._qss_x_.Runtime_1_.VERSION.Undefined, UINT.Undefined.Clone());
#endif
                    this._diss_ToCommit = new VERSIONED<USET>(QS._qss_x_.Runtime_1_.VERSION.Undefined, USET.Undefined.Clone());
                    this._diss_ToAbort = new VERSIONED<USET>(QS._qss_x_.Runtime_1_.VERSION.Undefined, USET.Undefined.Clone());
#if DEBUG_SetAcknowledgement
                    this._diss_Clean = new Versioned<UIntSet>(QS.Fx.Runtime.Version.Undefined, null);
#else
                    this._diss_Clean = new VERSIONED<UINT>(QS._qss_x_.Runtime_1_.VERSION.Undefined, UINT.Undefined.Clone());
#endif
                }
            }

            #endregion

            #region _UpdatedLower

            public override void _UpdatedLower()
            {
                if (lowercontrol is Bind)
                {
                    this._CommitOk.SetTo(((Bind) lowercontrol)._CommitOk);
                    this._AbortOk.SetTo(((Bind) lowercontrol)._AbortOk);

                    UINT _new_Ack = ((Bind) lowercontrol)._Ack;
#if DEBUG_MonotonicChecks
                    Value.MonotonicCheck<UInt>(this._Ack, _new_Ack);
#endif
                    this._Ack.SetTo(_new_Ack);
                }
                else
                {
                    System.Diagnostics.Debug.Assert(lowercontrol is Peer);

                    // [rule] CommitOk := children.CommitOk
                    this._CommitOk.SetTo(((Peer) lowercontrol)._aggr_CommitOk.Value);
                    // [rule] AbortOk := children.AbortOk
                    this._AbortOk.SetTo(((Peer) lowercontrol)._aggr_AbortOk.Value);
                    // [rule] Ack := children.Ack
                    UINT _new_Ack = ((Peer)lowercontrol)._aggr_Ack.Value;
#if DEBUG_MonotonicChecks
                    Value.MonotonicCheck<UInt>(this._Ack, _new_Ack);
#endif
                    this._Ack.SetTo(_new_Ack);
                }
            }

            #endregion

            #region _UpdatedUpper

            public override void _UpdatedUpper()
            {
                updatedupper1 = true;
            }

            #endregion

            #region _UpperConsume1

            private void _UpperConsume1(QS._qss_x_.Runtime_1_.VERSION version)
            {
#if DEBUG_LogGenerously_2
                logger.Log("_UpperConsume1 : version = " + version.ToString());
#endif

                if (updatedupper1)
                {
#if DEBUG_LogGenerously
                    logger.Log("_UpperConsume1(" + version.ToString() + ") : Updating from Upper");
#endif

                    updatedupper1 = false;
                    if (uppercontrol != null)
                    {
                        this._diss_ToCommit.UpdateTo(new VERSIONED<USET>(version, uppercontrol._ToCommit));
                        this._diss_ToAbort.UpdateTo(new VERSIONED<USET>(version, uppercontrol._ToAbort));
#if DEBUG_SetAcknowledgement
                        this._diss_Clean.UpdateTo(new Versioned<UIntSet>(version, uppercontrol._Clean));
#else
                        VERSIONED<UINT> _new_diss_Clean = new VERSIONED<UINT>(version, uppercontrol._Clean);
#if DEBUG_MonotonicChecks
                        Versioned<UInt>.MonotonicCheck(this._diss_Clean, _new_diss_Clean);
#endif
                        this._diss_Clean.UpdateTo(_new_diss_Clean);
#endif
                        updatedupper2 = true;
                    }
                }
            }

            #endregion

            #region _UpperConsume2

            private void _UpperConsume2
            (
                VERSIONED<USET> _incoming_diss_ToCommit, 
                VERSIONED<USET> _incoming_diss_ToAbort,
#if DEBUG_SetAcknowledgement
                Versioned<UIntSet> _incoming_diss_Clean
#else
                VERSIONED<UINT> _incoming_diss_Clean
#endif
            )
            {
#if DEBUG_LogGenerously_2
                logger.Log("_UpperConsume2");
#endif

                if (this._diss_ToCommit.UpdateTo(_incoming_diss_ToCommit))
                    updatedupper2 = true;
                if (this._diss_ToAbort.UpdateTo(_incoming_diss_ToAbort))
                    updatedupper2 = true;

                if (_incoming_diss_Clean.Version.IsDefined) // && _incoming_diss_Clean.Value != null)
                {
#if DEBUG_MonotonicChecks
                    Versioned<UInt>.MonotonicCheck(this._diss_Clean, _incoming_diss_Clean);
#endif
                    if (this._diss_Clean.UpdateTo(_incoming_diss_Clean))
                        updatedupper2 = true;
                }
            }

            #endregion

            #region _UpperConsume3

            private void _UpperConsume3()
            {
#if DEBUG_LogGenerously_2
                logger.Log("_UpperConsume3");
#endif

                if (updatedupper2)
                {
#if DEBUG_LogGenerously
                    logger.Log("_UpperConsume3 : Updating this");
#endif

                    updatedupper2 = false;

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

                    if (lowercontrol != null)
                        lowercontrol._UpdatedUpper();
                }
            }

            #endregion

            #region IProtocolPeer.Aggregate (1)

            bool QS._qss_x_.Agents_.Base.IProtocolPeer.Aggregate(QS._qss_x_.Runtime_1_.VERSION version, out QS.Fx.Serialization.ISerializable _outgoing)
            {
#if DEBUG_LogGenerously_2
                logger.Log("Aggregate_1 : version = " + version.ToString());
#endif

                lock (this)
                {
                    AggregationToken outgoing = new AggregationToken();
                    _outgoing = outgoing;

                    _UpperConsume1(version);
                    _UpperConsume3();

                    if (peercontext.IsRegular)
                    {
                        outgoing._aggr_CommitOk = new VERSIONED<USET>(version,
                            this._CommitOk.Clone(MaximumNumberOfRangesForNetworkCommunication));

                        outgoing._aggr_AbortOk = new VERSIONED<USET>(version,
                            this._AbortOk.Clone(MaximumNumberOfRangesForNetworkCommunication));

#if DEBUG_SetAcknowledgement
                    outgoing._aggr_Ack = new Versioned<UIntSet>(version,
                        this._Ack.Clone(MaximumNumberOfRangesForNetworkCommunication));
#else
                        outgoing._aggr_Ack = new VERSIONED<UINT>(version, this._Ack.Clone());
#endif
                    }
                    else
                    {
                        // inserting neutral values for properties that are strict
                        outgoing._aggr_CommitOk = new VERSIONED<USET>(version, USET.FullSet.Clone());
                        outgoing._aggr_AbortOk = new VERSIONED<USET>(version, USET.EmptySet.Clone());
                        outgoing._aggr_Ack = new VERSIONED<UINT>(version, new UINT(uint.MaxValue));
                    }

                    outgoing._diss_ToCommit = USET.CloneVersioned(this._diss_ToCommit, MaximumNumberOfRangesForNetworkCommunication);
                    outgoing._diss_ToAbort = USET.CloneVersioned(this._diss_ToAbort, MaximumNumberOfRangesForNetworkCommunication);
#if DEBUG_SetAcknowledgement
                    outgoing._diss_Clean = UIntSet.CloneVersioned(this._diss_Clean, MaximumNumberOfRangesForNetworkCommunication);
#else
                    outgoing._diss_Clean = this._diss_Clean.Clone();
#endif


                }

                return true;
            }

            #endregion

            #region IProtocolPeer.Aggregate (2)

            bool QS._qss_x_.Agents_.Base.IProtocolPeer.Aggregate(QS._qss_x_.Runtime_1_.VERSION version, 
                QS.Fx.Serialization.ISerializable _incoming, out QS.Fx.Serialization.ISerializable _outgoing)
            {
#if DEBUG_LogGenerously_2
                logger.Log("Aggregate_2 : version = " + version.ToString());
#endif

                lock (this)
                {
                    AggregationToken incoming = (AggregationToken)_incoming;

                    if (incoming == null)
                    {
                        _outgoing = null;
                        return false;
                    }

                    _UpperConsume1(version);
                    _UpperConsume2(incoming._diss_ToCommit, incoming._diss_ToAbort, incoming._diss_Clean);
                    _UpperConsume3();

                    AggregationToken outgoing = new AggregationToken();
                    _outgoing = outgoing;

                    if (peercontext.IsRegular)
                    {
                        outgoing._aggr_CommitOk = new VERSIONED<USET>(incoming._aggr_CommitOk.Version,
                            incoming._aggr_CommitOk.Value.Intersect(this._CommitOk, MaximumNumberOfRangesForNetworkCommunication));

                        outgoing._aggr_AbortOk = new VERSIONED<USET>(incoming._aggr_AbortOk.Version,
                            incoming._aggr_AbortOk.Value.Sum(this._AbortOk, MaximumNumberOfRangesForNetworkCommunication));

#if DEBUG_SetAcknowledgement
                    outgoing._aggr_Ack = new Versioned<UIntSet>(incoming._aggr_Ack.Version,
                        incoming._aggr_Ack.Value.Intersect(this._Ack, MaximumNumberOfRangesForNetworkCommunication));
#else
                        outgoing._aggr_Ack = new VERSIONED<UINT>(incoming._aggr_Ack.Version,
                            new UINT(Math.Min(incoming._aggr_Ack.Value.Number, this._Ack.Number)));
#endif
                    }
                    else
                    {
                        outgoing._aggr_CommitOk = incoming._aggr_CommitOk.Clone();
                        outgoing._aggr_AbortOk = incoming._aggr_AbortOk.Clone();
                        outgoing._aggr_Ack = incoming._aggr_Ack.Clone();
                    }

                    outgoing._diss_ToCommit.UpdateTo(this._diss_ToCommit);
                    if (outgoing._diss_ToCommit.Value.IsDefined) // outgoing._diss_ToCommit.Value != null)
                        outgoing._diss_ToCommit.Value.Trim(MaximumNumberOfRangesForNetworkCommunication);
                    
                    outgoing._diss_ToAbort.UpdateTo(this._diss_ToAbort);
                    if (outgoing._diss_ToAbort.Value.IsDefined) // outgoing._diss_ToAbort.Value != null)
                        outgoing._diss_ToAbort.Value.Trim(MaximumNumberOfRangesForNetworkCommunication);
                    
                    outgoing._diss_Clean.UpdateTo(this._diss_Clean);

#if DEBUG_SetAcknowledgement
                    if (outgoing._diss_Clean.Value != null)
                        outgoing._diss_Clean.Value.Trim(MaximumNumberOfRangesForNetworkCommunication);
#endif
                }

                return true;
            }

            #endregion

            #region IProtocolPeer.Aggregate (3)

            bool QS._qss_x_.Agents_.Base.IProtocolPeer.Aggregate(QS._qss_x_.Runtime_1_.VERSION version, QS.Fx.Serialization.ISerializable _incoming)
            {
#if DEBUG_LogGenerously_2
                logger.Log("Aggregate_3 : version = " + version.ToString());
#endif

                lock (this)
                {
                    AggregationToken incoming = (AggregationToken)_incoming;

                    if (incoming == null)
                        return false;

                    _UpperConsume1(version);
                    _UpperConsume2(incoming._diss_ToCommit, incoming._diss_ToAbort, incoming._diss_Clean);
                    _UpperConsume3();

                    if (version.CompareTo(this._aggr_CommitOk.Version) > 0)
                    {
                        if (peercontext.IsRegular)
                        {
                            this._aggr_CommitOk = new QS._qss_x_.Runtime_1_.VERSIONED<USET>(
                                incoming._aggr_CommitOk.Version, incoming._aggr_CommitOk.Value.Intersect(this._CommitOk, uint.MaxValue));
                        }
                        else
                            this._aggr_CommitOk = incoming._aggr_CommitOk.Clone();
                    }

                    if (version.CompareTo(this._aggr_AbortOk.Version) > 0)
                    {
                        if (peercontext.IsRegular)
                        {
                            this._aggr_AbortOk = new QS._qss_x_.Runtime_1_.VERSIONED<USET>(
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
                        VERSIONED<UINT> _new_aggr_Ack;
                        if (peercontext.IsRegular)
                        {
                            _new_aggr_Ack = new QS._qss_x_.Runtime_1_.VERSIONED<UINT>(incoming._aggr_Ack.Version,
                                new UINT(Math.Min(incoming._aggr_Ack.Value.Number, this._Ack.Number)));
                        }
                        else
                            _new_aggr_Ack = incoming._aggr_Ack.Clone();

#if DEBUG_MonotonicChecks
                        Versioned<UInt>.MonotonicCheck(this._aggr_Ack, _new_aggr_Ack);
#endif
                        this._aggr_Ack = _new_aggr_Ack;
                    }
#endif

                    if (uppercontrol != null)
                        uppercontrol._UpdatedLower();
                }

                return true;
            }

            #endregion

            #region IProtocolPeer.Disseminate (1)

            bool QS._qss_x_.Agents_.Base.IProtocolPeer.Disseminate(QS._qss_x_.Runtime_1_.VERSION version, out QS.Fx.Serialization.ISerializable _outgoing)
            {
#if DEBUG_LogGenerously_2
                logger.Log("Disseminate_1 : version = " + version.ToString());
#endif

                lock (this)
                {
                    DisseminationToken outgoing = new DisseminationToken();
                    _outgoing = outgoing;

                    outgoing._aggr_CommitOk = USET.CloneVersioned(this._aggr_CommitOk, MaximumNumberOfRangesForNetworkCommunication);
                    outgoing._aggr_AbortOk = USET.CloneVersioned(this._aggr_AbortOk, MaximumNumberOfRangesForNetworkCommunication);
#if DEBUG_SetAcknowledgement
                    outgoing._aggr_Ack = UIntSet.CloneVersioned(this._aggr_Ack, MaximumNumberOfRangesForNetworkCommunication);
#else
                    outgoing._aggr_Ack = this._aggr_Ack.Clone();
#endif
                    outgoing._diss_ToCommit = USET.CloneVersioned(this._diss_ToCommit, MaximumNumberOfRangesForNetworkCommunication);
                    outgoing._diss_ToAbort = USET.CloneVersioned(this._diss_ToAbort, MaximumNumberOfRangesForNetworkCommunication);
#if DEBUG_SetAcknowledgement
                    outgoing._diss_Clean = UIntSet.CloneVersioned(this._diss_Clean, MaximumNumberOfRangesForNetworkCommunication);
#else
                    outgoing._diss_Clean = this._diss_Clean.Clone();
#endif
                }

                return true;
            }

            #endregion

            #region IProtocolPeer.Disseminate (2)

            bool QS._qss_x_.Agents_.Base.IProtocolPeer.Disseminate(QS._qss_x_.Runtime_1_.VERSION version,
                QS.Fx.Serialization.ISerializable _incoming, out QS.Fx.Serialization.ISerializable _outgoing)
            {
#if DEBUG_LogGenerously_2
                logger.Log("Disseminate_2 : version = " + version.ToString());
#endif

                lock (this)
                {
                    DisseminationToken incoming = (DisseminationToken)_incoming;

                    if (incoming == null)
                    {
                        _outgoing = null;
                        return false;
                    }

                    this._aggr_CommitOk.UpdateTo(incoming._aggr_CommitOk);
                    this._aggr_AbortOk.UpdateTo(incoming._aggr_AbortOk);

#if DEBUG_MonotonicChecks
                    Versioned<UInt>.MonotonicCheck(this._aggr_Ack, incoming._aggr_Ack);
#endif
                    this._aggr_Ack.UpdateTo(incoming._aggr_Ack);

                    _UpperConsume2(incoming._diss_ToCommit, incoming._diss_ToAbort, incoming._diss_Clean);
                    _UpperConsume3();

                    // in this protocol, just pass it further
                    DisseminationToken outgoing = incoming; 
                    _outgoing = outgoing;

//                    outgoing._diss_ToCommit.UpdateTo(this._diss_ToCommit);
//                    outgoing._diss_ToAbort.UpdateTo(this._diss_ToAbort);
//                    outgoing._diss_Clean.UpdateTo(this._diss_Clean);

                    if (uppercontrol != null)
                        uppercontrol._UpdatedLower();
                }

                return true;
            }

            #endregion

            #region IProtocolPeer.Disseminate (3)

            bool QS._qss_x_.Agents_.Base.IProtocolPeer.Disseminate(QS._qss_x_.Runtime_1_.VERSION version, QS.Fx.Serialization.ISerializable _incoming)
            {
#if DEBUG_LogGenerously_2
                logger.Log("Disseminate_3 : version = " + version.ToString());
#endif

                lock (this)
                {
                    DisseminationToken incoming = (DisseminationToken)_incoming;

                    if (incoming == null)
                        return false;

                    this._aggr_CommitOk.UpdateTo(incoming._aggr_CommitOk);
                    this._aggr_AbortOk.UpdateTo(incoming._aggr_AbortOk);
#if DEBUG_MonotonicChecks
                    Versioned<UInt>.MonotonicCheck(this._aggr_Ack, incoming._aggr_Ack);
#endif
                    this._aggr_Ack.UpdateTo(incoming._aggr_Ack);

                    _UpperConsume2(incoming._diss_ToCommit, incoming._diss_ToAbort, incoming._diss_Clean);
                    _UpperConsume3();

                    if (uppercontrol != null)
                        uppercontrol._UpdatedLower();
                }

                return true;
            }

            #endregion
        }

        #endregion

        #region Class Root

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        private sealed class Root : Control, QS._qss_x_.Agents_.Base.IProtocolRoot
        {
            #region Constructor

            public Root() : base()
            {
            }

            #endregion

            #region Fields

            #endregion

            #region _Initialization

            public override void _Initialization()
            {
                lock (this)
                {
                    this._CommitOk = USET.Undefined.Clone();
                    this._AbortOk = USET.Undefined.Clone();
#if DEBUG_SetAcknowledgement
                    this._Ack = UIntSet.Undefined.Clone();
#else
                    this._Ack = new UINT(0);
#endif
                    this._ToCommit = USET.EmptySet.Clone();
                    this._ToAbort = USET.EmptySet.Clone();
#if DEBUG_SetAcknowledgement
                    this._Clean = UIntSet.EmptySet.Clone();
#else
                    this._Clean = new UINT(0);
#endif
                }
            }

            #endregion

            #region _UpdatedLower

            public override void _UpdatedLower()
            {
                System.Diagnostics.Debug.Assert(lowercontrol is Peer);

                // [rule] CommitOk := children.CommitOk
                USET _new_CommitOk = ((Peer)lowercontrol)._aggr_CommitOk.Value;
// #if DEBUG_MonotonicChecks
//                 Value.MonotonicCheck<UIntSet>(this._CommitOk, _new_CommitOk);
// #endif
                this._CommitOk.SetTo(_new_CommitOk);
                // [rule] AbortOk := children.AbortOk
                USET _new_AbortOk = ((Peer)lowercontrol)._aggr_AbortOk.Value;
// #if DEBUG_MonotonicChecks
//                 Value.MonotonicCheck<UIntSet>(this._AbortOk, _new_AbortOk);
// #endif
                this._AbortOk.SetTo(_new_AbortOk);
                // [rule] Ack := children.Ack
                UINT _new_Ack = ((Peer)lowercontrol)._aggr_Ack.Value;
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

                if (lowercontrol != null)
                    lowercontrol._UpdatedUpper();
            }

            #endregion

            #region _UpdatedUpper

            public override void _UpdatedUpper()
            {
                System.Diagnostics.Debug.Assert(false);
            }

            #endregion
        }

        #endregion

        // tokens

        #region Class AggregationToken

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Protocols_Simple2PC_AggregationToken)]
        public sealed class AggregationToken : QS.Fx.Serialization.ISerializable
        {
            #region Constructor

            public AggregationToken()
            {
            }

            #endregion

            #region Fields

            [QS.Fx.Printing.Printable]
            public VERSIONED<USET> _aggr_CommitOk;
            [QS.Fx.Printing.Printable]
            public VERSIONED<USET> _aggr_AbortOk;
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public Versioned<UIntSet> _aggr_Ack;
#else
            public VERSIONED<UINT> _aggr_Ack;
#endif
            [QS.Fx.Printing.Printable]
            public VERSIONED<USET> _diss_ToCommit;
            [QS.Fx.Printing.Printable]
            public VERSIONED<USET> _diss_ToAbort;
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public Versioned<UIntSet> _diss_Clean;
#else
            public VERSIONED<UINT> _diss_Clean;
#endif
            #endregion

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get 
                {
                    QS.Fx.Serialization.SerializableInfo info = 
                        new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Protocols_Simple2PC_AggregationToken);
                    info.AddAnother(_aggr_CommitOk.SerializableInfo);
                    info.AddAnother(_aggr_AbortOk.SerializableInfo);
                    info.AddAnother(_aggr_Ack.SerializableInfo);
                    info.AddAnother(_diss_ToCommit.SerializableInfo);
                    info.AddAnother(_diss_ToAbort.SerializableInfo);
                    info.AddAnother(_diss_Clean.SerializableInfo);
                    return info;
                }
            }

            void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                _aggr_CommitOk.SerializeTo(ref header, ref data);
                _aggr_AbortOk.SerializeTo(ref header, ref data);
                _aggr_Ack.SerializeTo(ref header, ref data);
                _diss_ToCommit.SerializeTo(ref header, ref data);
                _diss_ToAbort.SerializeTo(ref header, ref data);
                _diss_Clean.SerializeTo(ref header, ref data);
            }

            void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                _aggr_CommitOk.DeserializeFrom(ref header, ref data);
                _aggr_AbortOk.DeserializeFrom(ref header, ref data);
                _aggr_Ack.DeserializeFrom(ref header, ref data);
                _diss_ToCommit.DeserializeFrom(ref header, ref data);
                _diss_ToAbort.DeserializeFrom(ref header, ref data);
                _diss_Clean.DeserializeFrom(ref header, ref data);
            }

            #endregion
        }

        #endregion

        #region Class DisseminationToken

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Protocols_Simple2PC_DisseminationToken)]
        public sealed class DisseminationToken : QS.Fx.Serialization.ISerializable
        {
            #region Constructor

            public DisseminationToken()
            {
            }

            #endregion

            #region Fields

            [QS.Fx.Printing.Printable]
            public VERSIONED<USET> _aggr_CommitOk;
            [QS.Fx.Printing.Printable]
            public VERSIONED<USET> _aggr_AbortOk;
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public Versioned<UIntSet> _aggr_Ack;
#else
            public VERSIONED<UINT> _aggr_Ack;
#endif
            [QS.Fx.Printing.Printable]
            public VERSIONED<USET> _diss_ToCommit;
            [QS.Fx.Printing.Printable]
            public VERSIONED<USET> _diss_ToAbort;
            [QS.Fx.Printing.Printable]
#if DEBUG_SetAcknowledgement
            public Versioned<UIntSet> _diss_Clean;
#else
            public VERSIONED<UINT> _diss_Clean;
#endif

            #endregion

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo info =
                        new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Protocols_Simple2PC_DisseminationToken);
                    info.AddAnother(_aggr_CommitOk.SerializableInfo);
                    info.AddAnother(_aggr_AbortOk.SerializableInfo);
                    info.AddAnother(_aggr_Ack.SerializableInfo);
                    info.AddAnother(_diss_ToCommit.SerializableInfo);
                    info.AddAnother(_diss_ToAbort.SerializableInfo);
                    info.AddAnother(_diss_Clean.SerializableInfo);
                    return info;
                }
            }

            void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                _aggr_CommitOk.SerializeTo(ref header, ref data);
                _aggr_AbortOk.SerializeTo(ref header, ref data);
                _aggr_Ack.SerializeTo(ref header, ref data);
                _diss_ToCommit.SerializeTo(ref header, ref data);
                _diss_ToAbort.SerializeTo(ref header, ref data);
                _diss_Clean.SerializeTo(ref header, ref data);
            }

            void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                _aggr_CommitOk.DeserializeFrom(ref header, ref data);
                _aggr_AbortOk.DeserializeFrom(ref header, ref data);
                _aggr_Ack.DeserializeFrom(ref header, ref data);
                _diss_ToCommit.DeserializeFrom(ref header, ref data);
                _diss_ToAbort.DeserializeFrom(ref header, ref data);
                _diss_Clean.DeserializeFrom(ref header, ref data);
            }

            #endregion
        }

        #endregion
    }
}
