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
    public sealed class _Protocol : QS._qss_x_.Agents_.Base.IProtocol
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

        private static readonly _Protocol protocol = new _Protocol();

        private _Protocol()
        {
        }

        #endregion

        // plumbing

        #region IProtocol.Name

        string QS._qss_x_.Agents_.Base.IProtocol.Name
        {
            get { return "_Protocol"; }
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
            return new Bind((IEndpoint)endpoint);
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

        #region Interface IEndpoint @@@@@@@@@@@

/*
        public delegate void CommittableCallback(uint m);
        public delegate void AbortableCallback(uint m);
*/

        public interface IEndpoint
        {
/*
            event CommittableCallback OnCommittable;
            event AbortableCallback OnAbortable;
            void Commit(uint m);
            void Abort(uint m);
*/
        }

        #endregion

        // controllers

        #region Class Control @@@@@@@@@@

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

            #region Fields @@@@@@@@

            [QS.Fx.Base.Inspectable]
            public Control uppercontrol, lowercontrol;
            [QS.Fx.Base.Inspectable]
            public QS.Fx.Logging.ILogger logger;

/*
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public UIntSet _CommitOk;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public UIntSet _AbortOk;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public UInt _Ack;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public UIntSet _ToCommit;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public UIntSet _ToAbort;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public UInt _Clean;
*/

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

        #region Class Bind @@@@@@@@@@@@

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        private sealed class Bind : Control, QS._qss_x_.Agents_.Base.IProtocolBind
        {
            #region Constructor @@@@@@@@@@

            public Bind(IEndpoint endpoint)
            {
                lock (this)
                {
                    this.endpoint = endpoint;

/*
                    endpoint.OnCommittable += new CommittableCallback(this._OnCommittableCallback);
                    endpoint.OnAbortable += new AbortableCallback(this._OnAbortableCallback);
*/
                }

                _Initialization();
            }

            #endregion

            #region Fields @@@@@@@@@@

            [QS.Fx.Base.Inspectable]
            public IEndpoint endpoint;

/*
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public UIntSet _Completed;
*/

            #endregion

            #region _Initialization @@@@@@@

            public override void _Initialization()
            {
                lock (this)
                {
/*
                    this._CommitOk = UIntSet.EmptySet.Clone();
                    this._AbortOk = UIntSet.EmptySet.Clone();
                    this._Ack = new UInt(0);
                    this._Completed = UIntSet.EmptySet.Clone();
                    this._ToCommit = UIntSet.EmptySet.Clone();
                    this._ToAbort = UIntSet.EmptySet.Clone();
                    this._Clean = new UInt(0);
*/
                }
            }

            #endregion

/*
            #region _OnCommittableCallback

            private void _OnCommittableCallback(uint m)
            {
                lock (this)
                {
                    this._CommitOk.Insert(m);

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

                    if (uppercontrol != null)
                        uppercontrol._UpdatedLower();
                }
            }

            #endregion
*/

            #region _UpdatedLower

            public override void _UpdatedLower()
            {
                System.Diagnostics.Debug.Assert(false);
            }

            #endregion

            #region _UpdatedUpper @@@@@@@@@@@

            public override void _UpdatedUpper()
            {
/*
                UInt _new_Clean = uppercontrol._Clean;
                UIntSet _new_ToCommit = uppercontrol._ToCommit.Clone();
                UIntSet _new_ToAbort = uppercontrol._ToAbort.Clone();

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

                this._Clean.SetTo(_new_Clean);

                // rules for ack

                UIntSet _new_Completed = this._ToCommit.Sum(this._ToAbort, uint.MaxValue);
#if DEBUG_MonotonicChecks
                Value.MonotonicCheck<UIntSet>(this._Completed, _new_Completed);
#endif
                this._Completed.SetTo(_new_Completed);

                UInt _new_Ack = this._Completed.MaxContiguous(this._Ack);
#if DEBUG_MonotonicChecks
                Value.MonotonicCheck<UInt>(this._Ack, _new_Ack);
#endif
                this._Ack.SetTo(_new_Ack);

                if (this._Clean.IsDefined && this._Clean.Number > 0)
                {
                    this._CommitOk = this._CommitOk.Cut(this._Clean.Number);
                    this._AbortOk = this._AbortOk.Cut(this._Clean.Number);
                }
*/
            }

            #endregion
        }

        #endregion

        #region Class Peer @@@@@@@@@@

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        private sealed class Peer : Control, QS._qss_x_.Agents_.Base.IProtocolPeer, QS._qss_x_.Agents_.Base.IProtocolControl
        {
            #region Constructor

            public Peer(QS._qss_x_.Agents_.Base.IProtocolPeerContext peercontext) : base()
            {
                this.peercontext = peercontext;
            }

            #endregion

            #region Fields @@@@@@@@@

            [QS.Fx.Base.Inspectable]
            public bool updatedupper1, updatedupper2;
            [QS.Fx.Base.Inspectable]
            public QS._qss_x_.Agents_.Base.IProtocolPeerContext peercontext;

/*
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _aggr_CommitOk;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _aggr_AbortOk;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public Versioned<UInt> _aggr_Ack;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _diss_ToCommit;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _diss_ToAbort;
            [QS.TMS.Inspection.Inspectable]
            [QS.Fx.Printing.Printable]
            public Versioned<UInt> _diss_Clean;
*/

            #endregion

            #region IProtocolControl.Initialize

            void QS._qss_x_.Agents_.Base.IProtocolControl.Initialize(QS.Fx.Logging.ILogger logger)
            {
                this.logger = logger;
                logger.Log("This peer agent is starting as " + (peercontext.IsRegular ? "a normal" : "an associate") + " member of the peer group.");
            }

            #endregion

            #region _Initialization @@@@@@@@

            public override void _Initialization()
            {
                lock (this)
                {
/*
                    this._CommitOk = UIntSet.Undefined.Clone();
                    this._AbortOk = UIntSet.Undefined.Clone();
                    this._Ack = new UInt(0);
                    this._ToCommit = UIntSet.EmptySet.Clone();
                    this._ToAbort = UIntSet.EmptySet.Clone();
                    this._Clean = new UInt(0);

                    this._aggr_CommitOk = new Versioned<UIntSet>(QS.Fx.Runtime.Version.Undefined, UIntSet.Undefined.Clone());
                    this._aggr_AbortOk = new Versioned<UIntSet>(QS.Fx.Runtime.Version.Undefined, UIntSet.Undefined.Clone());
                    this._aggr_Ack = new Versioned<UInt>(QS.Fx.Runtime.Version.Undefined, UInt.Undefined.Clone());
                    this._diss_ToCommit = new Versioned<UIntSet>(QS.Fx.Runtime.Version.Undefined, UIntSet.Undefined.Clone());
                    this._diss_ToAbort = new Versioned<UIntSet>(QS.Fx.Runtime.Version.Undefined, UIntSet.Undefined.Clone());
                    this._diss_Clean = new Versioned<UInt>(QS.Fx.Runtime.Version.Undefined, UInt.Undefined.Clone());
*/
                }
            }

            #endregion

            #region _UpdatedLower @@@@@@@@@@@

            public override void _UpdatedLower()
            {
                if (lowercontrol is Bind)
                {
/*
                    this._CommitOk.SetTo(((Bind) lowercontrol)._CommitOk);
                    this._AbortOk.SetTo(((Bind) lowercontrol)._AbortOk);

                    UInt _new_Ack = ((Bind)lowercontrol)._Ack;
#if DEBUG_MonotonicChecks
                    Value.MonotonicCheck<UInt>(this._Ack, _new_Ack);
#endif
                    this._Ack.SetTo(_new_Ack);
*/
                }
                else
                {
                    System.Diagnostics.Debug.Assert(lowercontrol is Peer);

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
                }
            }

            #endregion

            #region _UpdatedUpper

            public override void _UpdatedUpper()
            {
                updatedupper1 = true;
            }

            #endregion

            #region _UpperConsume1 @@@@@@@@@@

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
/*
                        this._diss_ToCommit.UpdateTo(new Versioned<UIntSet>(version, uppercontrol._ToCommit));
                        this._diss_ToAbort.UpdateTo(new Versioned<UIntSet>(version, uppercontrol._ToAbort));
                        Versioned<UInt> _new_diss_Clean = new Versioned<UInt>(version, uppercontrol._Clean);
#if DEBUG_MonotonicChecks
                        Versioned<UInt>.MonotonicCheck(this._diss_Clean, _new_diss_Clean);
#endif
                        this._diss_Clean.UpdateTo(_new_diss_Clean);
*/

                        updatedupper2 = true;
                    }
                }
            }

            #endregion

            #region _UpperConsume2 @@@@@@@@@

            private void _UpperConsume2
            (
/*
                Versioned<UIntSet> _incoming_diss_ToCommit,
                Versioned<UIntSet> _incoming_diss_ToAbort,
                 Versioned<UInt> _incoming_diss_Clean
*/
            )
            {
#if DEBUG_LogGenerously_2
                logger.Log("_UpperConsume2");
#endif

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
            }

            #endregion

            #region _UpperConsume3 @@@@@@@@@@@@

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

/*
                    // [rule] Clean += parent.Clean
                    this._Clean.Number = Math.Max(this._Clean.Number, this._diss_Clean.Value.Number);
                    // [rule] ToCommit := parent.ToCommit
                    this._ToCommit = this._diss_ToCommit.Value.Clone();
                    // [rule] ToAbort := parent.ToAbort
                    this._ToAbort = this._diss_ToAbort.Value.Clone();
*/

                    if (lowercontrol != null)
                        lowercontrol._UpdatedUpper();
                }
            }

            #endregion

            #region IProtocolPeer.Aggregate (1) @@@@@@@@@@

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

/*
                    if (peercontext.IsRegular)
                    {
                        outgoing._aggr_CommitOk = new Versioned<UIntSet>(version,
                            this._CommitOk.Clone(MaximumNumberOfRangesForNetworkCommunication));
                        outgoing._aggr_AbortOk = new Versioned<UIntSet>(version,
                            this._AbortOk.Clone(MaximumNumberOfRangesForNetworkCommunication));
                        outgoing._aggr_Ack = new Versioned<UInt>(version, this._Ack.Clone());
                    }
                    else
                    {
                        outgoing._aggr_CommitOk = new Versioned<UIntSet>(version, UIntSet.FullSet.Clone());
                        outgoing._aggr_AbortOk = new Versioned<UIntSet>(version, UIntSet.EmptySet.Clone());
                        outgoing._aggr_Ack = new Versioned<UInt>(version, new UInt(uint.MaxValue));
                    }

                    outgoing._diss_ToCommit = UIntSet.CloneVersioned(this._diss_ToCommit, MaximumNumberOfRangesForNetworkCommunication);
                    outgoing._diss_ToAbort = UIntSet.CloneVersioned(this._diss_ToAbort, MaximumNumberOfRangesForNetworkCommunication);
                    outgoing._diss_Clean = this._diss_Clean.Clone();
*/
                }

                return true;
            }

            #endregion

            #region IProtocolPeer.Aggregate (2) @@@@@@@@@@

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
                    _UpperConsume2
                        (
/*
                            incoming._diss_ToCommit, incoming._diss_ToAbort, incoming._diss_Clean
*/
                        );
                    _UpperConsume3();

                    AggregationToken outgoing = new AggregationToken();
                    _outgoing = outgoing;

/*
                    if (peercontext.IsRegular)
                    {
                        outgoing._aggr_CommitOk = new Versioned<UIntSet>(incoming._aggr_CommitOk.Version,
                            incoming._aggr_CommitOk.Value.Intersect(this._CommitOk, MaximumNumberOfRangesForNetworkCommunication));

                        outgoing._aggr_AbortOk = new Versioned<UIntSet>(incoming._aggr_AbortOk.Version,
                            incoming._aggr_AbortOk.Value.Sum(this._AbortOk, MaximumNumberOfRangesForNetworkCommunication));

                        outgoing._aggr_Ack = new Versioned<UInt>(incoming._aggr_Ack.Version,
                            new UInt(Math.Min(incoming._aggr_Ack.Value.Number, this._Ack.Number)));
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
*/
                }

                return true;
            }

            #endregion

            #region IProtocolPeer.Aggregate (3) @@@@@@@@@@

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
                    _UpperConsume2
                        (
/*
                            incoming._diss_ToCommit, incoming._diss_ToAbort, incoming._diss_Clean
*/
                        );
                    _UpperConsume3();

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
*/

                    if (uppercontrol != null)
                        uppercontrol._UpdatedLower();
                }

                return true;
            }

            #endregion

            #region IProtocolPeer.Disseminate (1) @@@@@@@@@@@

            bool QS._qss_x_.Agents_.Base.IProtocolPeer.Disseminate(QS._qss_x_.Runtime_1_.VERSION version, out QS.Fx.Serialization.ISerializable _outgoing)
            {
#if DEBUG_LogGenerously_2
                logger.Log("Disseminate_1 : version = " + version.ToString());
#endif

                lock (this)
                {
                    DisseminationToken outgoing = new DisseminationToken();
                    _outgoing = outgoing;

/*
                    outgoing._aggr_CommitOk = UIntSet.CloneVersioned(this._aggr_CommitOk, MaximumNumberOfRangesForNetworkCommunication);
                    outgoing._aggr_AbortOk = UIntSet.CloneVersioned(this._aggr_AbortOk, MaximumNumberOfRangesForNetworkCommunication);
                    outgoing._aggr_Ack = this._aggr_Ack.Clone();
                    outgoing._diss_ToCommit = UIntSet.CloneVersioned(this._diss_ToCommit, MaximumNumberOfRangesForNetworkCommunication);
                    outgoing._diss_ToAbort = UIntSet.CloneVersioned(this._diss_ToAbort, MaximumNumberOfRangesForNetworkCommunication);
                    outgoing._diss_Clean = this._diss_Clean.Clone();
*/
                }

                return true;
            }

            #endregion

            #region IProtocolPeer.Disseminate (2) @@@@@@@@@@@

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

/*
                    this._aggr_CommitOk.UpdateTo(incoming._aggr_CommitOk);
                    this._aggr_AbortOk.UpdateTo(incoming._aggr_AbortOk);

#if DEBUG_MonotonicChecks
                    Versioned<UInt>.MonotonicCheck(this._aggr_Ack, incoming._aggr_Ack);
#endif
                    this._aggr_Ack.UpdateTo(incoming._aggr_Ack);
*/

                    _UpperConsume2
                        (                        
/*
                            incoming._diss_ToCommit, incoming._diss_ToAbort, incoming._diss_Clean
*/
                        );
                    _UpperConsume3();

                    DisseminationToken outgoing = incoming;
                    _outgoing = outgoing;

                    if (uppercontrol != null)
                        uppercontrol._UpdatedLower();
                }

                return true;
            }

            #endregion

            #region IProtocolPeer.Disseminate (3) @@@@@@@@

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

/*
                    this._aggr_CommitOk.UpdateTo(incoming._aggr_CommitOk);
                    this._aggr_AbortOk.UpdateTo(incoming._aggr_AbortOk);
#if DEBUG_MonotonicChecks
                    Versioned<UInt>.MonotonicCheck(this._aggr_Ack, incoming._aggr_Ack);
#endif
                    this._aggr_Ack.UpdateTo(incoming._aggr_Ack);
*/

                    _UpperConsume2
                        (
/*
                            incoming._diss_ToCommit, incoming._diss_ToAbort, incoming._diss_Clean    
*/
                        );
                    _UpperConsume3();

                    if (uppercontrol != null)
                        uppercontrol._UpdatedLower();
                }

                return true;
            }

            #endregion
        }

        #endregion

        #region Class Root @@@@@@@@

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

            #region _Initialization @@@@@@@@@@

            public override void _Initialization()
            {
                lock (this)
                {
/*
                    this._CommitOk = UIntSet.Undefined.Clone();
                    this._AbortOk = UIntSet.Undefined.Clone();
                    this._Ack = new UInt(0);
                    this._ToCommit = UIntSet.EmptySet.Clone();
                    this._ToAbort = UIntSet.EmptySet.Clone();
                    this._Clean = new UInt(0);
*/
                }
            }

            #endregion

            #region _UpdatedLower @@@@@@@@@@@@

            public override void _UpdatedLower()
            {
                System.Diagnostics.Debug.Assert(lowercontrol is Peer);

/*
                // [rule] CommitOk := children.CommitOk
                UIntSet _new_CommitOk = ((Peer)lowercontrol)._aggr_CommitOk.Value;
                this._CommitOk.SetTo(_new_CommitOk);

                // [rule] AbortOk := children.AbortOk
                UIntSet _new_AbortOk = ((Peer)lowercontrol)._aggr_AbortOk.Value;
                this._AbortOk.SetTo(_new_AbortOk);
                
                // [rule] Ack := children.Ack
                UInt _new_Ack = ((Peer)lowercontrol)._aggr_Ack.Value;
#if DEBUG_MonotonicChecks
                Value.MonotonicCheck<UInt>(this._Ack, _new_Ack);
#endif
                this._Ack.SetTo(_new_Ack);
                
                // [rule] global.ToCommit := CommitOk
                if (this._CommitOk.IsDefined)
                    this._ToCommit.SetTo(this._CommitOk);
                
                // [rule] global.ToAbort := AbortOk
                if (this._AbortOk.IsDefined)
                    this._ToAbort.SetTo(this._AbortOk);
                
                // [rule] global.Clean := Ack
                if (this._Ack.IsDefined)
                    this._Clean.SetTo(this._Ack);
*/

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

        #region Class AggregationToken @@@@@@@@@@@@@

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Protocols_Protocol_AggregationToken)]
        public sealed class AggregationToken : QS.Fx.Serialization.ISerializable
        {
            #region Constructor

            public AggregationToken()
            {
            }

            #endregion

            #region Fields @@@@@@@@@

/*
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _aggr_CommitOk;
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _aggr_AbortOk;
            [QS.Fx.Printing.Printable]
            public Versioned<UInt> _aggr_Ack;
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _diss_ToCommit;
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _diss_ToAbort;
            [QS.Fx.Printing.Printable]
            public Versioned<UInt> _diss_Clean;
*/

            #endregion

            #region ISerializable Members @@@@@@@@@@

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo info =
                        new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Protocols_Protocol_AggregationToken);
/*
                    info.AddAnother(_aggr_CommitOk.SerializableInfo);
                    info.AddAnother(_aggr_AbortOk.SerializableInfo);
                    info.AddAnother(_aggr_Ack.SerializableInfo);
                    info.AddAnother(_diss_ToCommit.SerializableInfo);
                    info.AddAnother(_diss_ToAbort.SerializableInfo);
                    info.AddAnother(_diss_Clean.SerializableInfo);
*/
                    return info;
                }
            }

            void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
/*
                _aggr_CommitOk.SerializeTo(ref header, ref data);
                _aggr_AbortOk.SerializeTo(ref header, ref data);
                _aggr_Ack.SerializeTo(ref header, ref data);
                _diss_ToCommit.SerializeTo(ref header, ref data);
                _diss_ToAbort.SerializeTo(ref header, ref data);
                _diss_Clean.SerializeTo(ref header, ref data);
*/
            }

            void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
/*
                _aggr_CommitOk.DeserializeFrom(ref header, ref data);
                _aggr_AbortOk.DeserializeFrom(ref header, ref data);
                _aggr_Ack.DeserializeFrom(ref header, ref data);
                _diss_ToCommit.DeserializeFrom(ref header, ref data);
                _diss_ToAbort.DeserializeFrom(ref header, ref data);
                _diss_Clean.DeserializeFrom(ref header, ref data);
*/
            }

            #endregion
        }

        #endregion

        #region Class DisseminationToken @@@@@@@@@@@@

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Protocols_Protocol_DisseminationToken)]
        public sealed class DisseminationToken : QS.Fx.Serialization.ISerializable
        {
            #region Constructor

            public DisseminationToken()
            {
            }

            #endregion

            #region Fields @@@@@@@@@

/*
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _aggr_CommitOk;
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _aggr_AbortOk;
            [QS.Fx.Printing.Printable]
            public Versioned<UInt> _aggr_Ack;
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _diss_ToCommit;
            [QS.Fx.Printing.Printable]
            public Versioned<UIntSet> _diss_ToAbort;
            [QS.Fx.Printing.Printable]
            public Versioned<UInt> _diss_Clean;
*/

            #endregion

            #region ISerializable Members @@@@@@@@@

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo info =
                        new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Protocols_Protocol_DisseminationToken);
/*
                    info.AddAnother(_aggr_CommitOk.SerializableInfo);
                    info.AddAnother(_aggr_AbortOk.SerializableInfo);
                    info.AddAnother(_aggr_Ack.SerializableInfo);
                    info.AddAnother(_diss_ToCommit.SerializableInfo);
                    info.AddAnother(_diss_ToAbort.SerializableInfo);
                    info.AddAnother(_diss_Clean.SerializableInfo);
*/
                    return info;
                }
            }

            void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
/*
                _aggr_CommitOk.SerializeTo(ref header, ref data);
                _aggr_AbortOk.SerializeTo(ref header, ref data);
                _aggr_Ack.SerializeTo(ref header, ref data);
                _diss_ToCommit.SerializeTo(ref header, ref data);
                _diss_ToAbort.SerializeTo(ref header, ref data);
                _diss_Clean.SerializeTo(ref header, ref data);
*/
            }

            void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
/*
                _aggr_CommitOk.DeserializeFrom(ref header, ref data);
                _aggr_AbortOk.DeserializeFrom(ref header, ref data);
                _aggr_Ack.DeserializeFrom(ref header, ref data);
                _diss_ToCommit.DeserializeFrom(ref header, ref data);
                _diss_ToAbort.DeserializeFrom(ref header, ref data);
                _diss_Clean.DeserializeFrom(ref header, ref data);
*/
            }

            #endregion
        }

        #endregion
    }
}
