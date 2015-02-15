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
// #define DEBUG_LogPropertyValueAssignments

// #define DEBUG_MonotonicChecks
#define OPTION_RejectDisseminatedParentValuesFromOldCofigurationIncarnations

using System;
using System.Collections.Generic;
using System.Text;

using QS._qss_x_.Runtime_1_;
using QS._qss_x_.Runtime_2_;

namespace QS._qss_x_.Protocols_
{
    public sealed class CoordinatedPhases : QS._qss_x_.Agents_.Base.IProtocol
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

        private static readonly CoordinatedPhases protocol = new CoordinatedPhases();

        private CoordinatedPhases()
        {
        }

        #endregion

        // plumbing

        #region IProtocol.Name

        string QS._qss_x_.Agents_.Base.IProtocol.Name
        {
            get { return "CoordinatedPhases"; }
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

        #region Interface IEndpoint

        public interface IEndpoint
        {
            void Phase(uint k);
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
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            public Control uppercontrol, lowercontrol;
            [QS.Fx.Base.Inspectable]
            public QS.Fx.Logging.ILogger logger;

            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
            public UINT _Last;
            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
            public UINT _Next;

            #endregion

            #region IProtocolControl.Initialize

            void QS._qss_x_.Agents_.Base.IProtocolControl.Initialize(QS.Fx.Logging.ILogger logger)
            {
                lock (this)
                {
                    this.logger = logger;
                    _Initialization();
                }
            }

            #endregion

            #region IProtocolControl.ConnectUpper

            void QS._qss_x_.Agents_.Base.IProtocolControl.ConnectUpper(QS._qss_x_.Agents_.Base.IProtocolControl control)
            {
                lock (this)
                {
                    uppercontrol = (Control) control;
                    _UpdatedUpper();
                }
            }

            #endregion

            #region IProtocolControl.ConnectLower

            void QS._qss_x_.Agents_.Base.IProtocolControl.ConnectLower(QS._qss_x_.Agents_.Base.IProtocolControl control)
            {
                lock (this)
                {
                    lowercontrol = (Control) control;
                    _UpdatedLower();
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

            public Bind(IEndpoint endpoint) : base()
            {
                lock (this)
                {
                    this.endpoint = endpoint;
                }
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            public IEndpoint endpoint;

            #endregion

            #region _Initialization

            public override void _Initialization()
            {
                lock (this)
                {
                    this._Last = new UINT(0);
#if DEBUG_LogPropertyValueAssignments
                    logger.Log("_Last = " + this._Last.ToString());
#endif
                    this._Next = UINT.Undefined.Clone();
#if DEBUG_LogPropertyValueAssignments
                    logger.Log("_Next = " + this._Next.ToString());
#endif
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
                bool _updated_Next = false, _updated_Last = false;

                if (uppercontrol._Next.IsDefined)
                {
                    UINT _new_Next = uppercontrol._Next;
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
                    UINT _new_Last = this._Next.Clone();
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
            public VERSIONED<UINT> _aggr_Last;
            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
            public VERSIONED<UINT> _diss_Next;
            [QS.Fx.Base.Inspectable]
            [QS.Fx.Printing.Printable]
            public VERSIONED<UINT> _diss_Last;

            #endregion

            #region IProtocolControl.Initialize

            void QS._qss_x_.Agents_.Base.IProtocolControl.Initialize(QS.Fx.Logging.ILogger logger)
            {
                this.logger = logger;
                logger.Log("This peer agent is starting as " + (peercontext.IsRegular ? "a normal" : "an associate") + " member of the peer group.");
                _Initialization();
            }

            #endregion

            #region _Initialization

            public override void _Initialization()
            {
                lock (this)
                {
                    this._Last = UINT.Undefined.Clone();
#if DEBUG_LogPropertyValueAssignments
                    logger.Log("_Last = " + this._Last.ToString());
#endif
                    this._Next = UINT.Undefined.Clone();
#if DEBUG_LogPropertyValueAssignments
                    logger.Log("_Next = " + this._Next.ToString());
#endif
                    this._aggr_Last = new VERSIONED<UINT>(QS._qss_x_.Runtime_1_.VERSION.Undefined, UINT.Undefined.Clone());
#if DEBUG_LogPropertyValueAssignments
                    logger.Log("_aggr_Last = " + this._aggr_Last.ToString());
#endif
                    this._diss_Next = new VERSIONED<UINT>(QS._qss_x_.Runtime_1_.VERSION.Undefined, UINT.Undefined.Clone());
#if DEBUG_LogPropertyValueAssignments
                    logger.Log("_diss_Next = " + this._diss_Next.ToString());
#endif
                    this._diss_Last = new VERSIONED<UINT>(QS._qss_x_.Runtime_1_.VERSION.Undefined, UINT.Undefined.Clone());
#if DEBUG_LogPropertyValueAssignments
                    logger.Log("_diss_Last = " + this._diss_Last.ToString());
#endif
                }
            }

            #endregion

            #region _UpdatedLower

            public override void _UpdatedLower()
            {
                if (lowercontrol is Bind)
                {
                    UINT _new_Last = ((Bind) lowercontrol)._Last;
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
                }
                else
                {
                    System.Diagnostics.Debug.Assert(lowercontrol is Peer);

                    UINT _new_Last = ((Peer)lowercontrol)._aggr_Last.Value;
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
                        if (uppercontrol._Next.IsDefined)
                        {
                            VERSIONED<UINT> _new_diss_Next = new VERSIONED<UINT>(version, uppercontrol._Next);
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
                            VERSIONED<UINT> _new_diss_Last = new VERSIONED<UINT>(version, uppercontrol._Last);
#if DEBUG_MonotonicChecks
                            Versioned<UInt>.MonotonicCheck(this._diss_Last, _new_diss_Last);
#endif
                            this._diss_Last.UpdateTo(_new_diss_Last);

#if DEBUG_LogPropertyValueAssignments
                            logger.Log("_diss_Last = " + this._diss_Last.ToString());
#endif
                        }

                        updatedupper2 = true;
                    }
                }
            }

            #endregion

            #region _UpperConsume2

            private void _UpperConsume2(QS._qss_x_.Runtime_1_.VERSION version, VERSIONED<UINT> _incoming_diss_Next, VERSIONED<UINT> _incoming_diss_Last)
            {
#if DEBUG_LogGenerously_2
                logger.Log("_UpperConsume2");
#endif

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
            }

            #endregion

            #region _UpperConsume3

            private void _UpperConsume3(QS._qss_x_.Runtime_1_.VERSION version)
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

/*
                    if (this._diss_Last.Version.IsDefined && this._diss_Last.Value.IsDefined && this._Last.IsDefined)
                    {
                        if (this._diss_Last.Value.IsLess(this._Last))
                        {
                            if (!peercontext.IsRegular)
                            {
                                logger.Log("UPDATING TO REGULAR");
                                peercontext.IsRegular = true;
                                // System.Diagnostics.Debug.Assert(false, "Not Implemented : Upgrading Associate to Regular");
                            }
                        }
                        else
                        {
                            if (peercontext.IsRegular)
                            {
                                logger.Log("DEGRADING TO ASSOCIATE");
                                peercontext.IsRegular = false;
                                // System.Diagnostics.Debug.Assert(false, "Not Implemented: Degrading Regular to Associate");
                            }
                        }
                    }
*/

                    if (lowercontrol != null)
                        lowercontrol._UpdatedUpper();
                }
            }

            #endregion

            #region _CheckConditions

            private void _CheckConditions()
            {
                if (this._aggr_Last.Version.IsDefined && this._aggr_Last.Value.IsDefined && this._Last.IsDefined)
                {
                    if (this._aggr_Last.Value.IsLess(this._Last))
                    {
#if DEBUG_LogPropertyValueAssignments
                        logger.Log("CONDITION SATISFIED : " + this._aggr_Last.Value.ToString() + " <= " + this._Last.ToString());
#endif

                        if (!peercontext.IsRegular)
                        {
                            logger.Log("UPDATING TO REGULAR");
                            peercontext.IsRegular = true;
                            // System.Diagnostics.Debug.Assert(false, "Not Implemented : Upgrading Associate to Regular");
                        }
                    }
                    else
                    {
                        if (peercontext.IsRegular)
                        {
                            logger.Log("DEGRADING TO ASSOCIATE");
                            peercontext.IsRegular = false;
                            // System.Diagnostics.Debug.Assert(false, "Not Implemented: Degrading Regular to Associate");
                        }
                    }
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
                    _UpperConsume3(version);

                    if (peercontext.IsRegular)
                    {
                        outgoing._isset_aggr_Last = true;
                        outgoing._aggr_Last = new VERSIONED<UINT>(version, this._Last.Clone());
                    }
                    else
                    {
                        outgoing._isset_aggr_Last = false;
                        outgoing._aggr_Last = new VERSIONED<UINT>(version, UINT.Undefined.Clone());
                    }

                    outgoing._diss_Next = this._diss_Next.Clone();
                    outgoing._diss_Last = this._diss_Last.Clone();
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
                    _UpperConsume2(version, incoming._diss_Next, incoming._diss_Last);
                    _UpperConsume3(version);

                    AggregationToken outgoing = new AggregationToken();
                    _outgoing = outgoing;

                    if (peercontext.IsRegular)
                    {
                        outgoing._isset_aggr_Last = true;
                        outgoing._aggr_Last = new VERSIONED<UINT>(incoming._aggr_Last.Version, 
                            incoming._isset_aggr_Last ? UINT.Min(incoming._aggr_Last.Value, this._Last) : this._Last.Clone());
                    }
                    else
                    {
                        outgoing._isset_aggr_Last = incoming._isset_aggr_Last;
                        outgoing._aggr_Last = incoming._aggr_Last.Clone();
                    }

                    outgoing._diss_Next.UpdateTo(this._diss_Next);
                    outgoing._diss_Last.UpdateTo(this._diss_Last);
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
                    _UpperConsume2(version, incoming._diss_Next, incoming._diss_Last);
                    _UpperConsume3(version);

                    if (version.CompareTo(this._aggr_Last.Version) > 0)
                    {
                        VERSIONED<UINT> _new_aggr_Last;
                        bool _isset_aggr_Last;
                        if (peercontext.IsRegular)
                        {
                            _isset_aggr_Last = true;
                            _new_aggr_Last = new VERSIONED<UINT>(incoming._aggr_Last.Version,
                                incoming._isset_aggr_Last ? UINT.Min(incoming._aggr_Last.Value, this._Last) : this._Last.Clone());
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

                    outgoing._aggr_Last = this._aggr_Last.Clone();
                    outgoing._diss_Next = this._diss_Next.Clone();
                    outgoing._diss_Last = this._diss_Last.Clone();

                    _CheckConditions();
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

#if DEBUG_MonotonicChecks
                    Versioned<UInt>.MonotonicCheck(this._aggr_Last, incoming._aggr_Last);
#endif
                    this._aggr_Last.UpdateTo(incoming._aggr_Last);

#if DEBUG_LogPropertyValueAssignments
                    logger.Log("_aggr_Last = " + this._aggr_Last.ToString());
#endif

                    _UpperConsume2(version, incoming._diss_Next, incoming._diss_Last);
                    _UpperConsume3(version);

                    DisseminationToken outgoing = incoming;
                    _outgoing = outgoing;

                    if (uppercontrol != null)
                        uppercontrol._UpdatedLower();

                    _CheckConditions();
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

#if DEBUG_MonotonicChecks
                    Versioned<UInt>.MonotonicCheck(this._aggr_Last, incoming._aggr_Last);
#endif
                    this._aggr_Last.UpdateTo(incoming._aggr_Last);

#if DEBUG_LogPropertyValueAssignments
                    logger.Log("_aggr_Last = " + this._aggr_Last.ToString());
#endif

                    _UpperConsume2(version, incoming._diss_Next, incoming._diss_Last);
                    _UpperConsume3(version);

                    if (uppercontrol != null)
                        uppercontrol._UpdatedLower();

                    _CheckConditions();
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
                    this._Last = UINT.Undefined.Clone();
#if DEBUG_LogPropertyValueAssignments
                    logger.Log("_Last = " + this._Last.ToString());
#endif
                    this._Next = UINT.Undefined.Clone();
#if DEBUG_LogPropertyValueAssignments
                    logger.Log("_Next = " + this._Next.ToString());
#endif
                }
            }

            #endregion

            #region _UpdatedLower

            public override void _UpdatedLower()
            {
                System.Diagnostics.Debug.Assert(lowercontrol is Peer);

                bool _updated_Last = false;
                UINT _new_Last = ((Peer)lowercontrol)._aggr_Last.Value;
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
                    UINT _new_Next = UINT.Sum(this._Last, UINT.One);
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
        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Protocols_CoordinatedPhases_AggregationToken)]
        public sealed class AggregationToken : QS.Fx.Serialization.ISerializable
        {
            #region Constructor

            public AggregationToken()
            {
            }

            #endregion

            #region Fields

            [QS.Fx.Printing.Printable]
            public bool _isset_aggr_Last;
            [QS.Fx.Printing.Printable]
            public VERSIONED<UINT> _aggr_Last;
            [QS.Fx.Printing.Printable]
            public VERSIONED<UINT> _diss_Next;
            [QS.Fx.Printing.Printable]
            public VERSIONED<UINT> _diss_Last;

            #endregion

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo info =
                        new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Protocols_CoordinatedPhases_AggregationToken);
                    QS._qss_c_.Base3_.SerializationHelper.ExtendSerializableInfo_Bool(ref info);
                    info.AddAnother(_aggr_Last.SerializableInfo);
                    info.AddAnother(_diss_Next.SerializableInfo);
                    info.AddAnother(_diss_Last.SerializableInfo);
                    return info;
                }
            }

            void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                QS._qss_c_.Base3_.SerializationHelper.Serialize_Bool(ref header, ref data, _isset_aggr_Last);
                _aggr_Last.SerializeTo(ref header, ref data);
                _diss_Next.SerializeTo(ref header, ref data);
                _diss_Last.SerializeTo(ref header, ref data);
            }

            void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                _isset_aggr_Last = QS._qss_c_.Base3_.SerializationHelper.Deserialize_Bool(ref header, ref data);
                _aggr_Last.DeserializeFrom(ref header, ref data);
                _diss_Next.DeserializeFrom(ref header, ref data);
                _diss_Last.DeserializeFrom(ref header, ref data);
            }

            #endregion
        }

        #endregion

        #region Class DisseminationToken

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Protocols_CoordinatedPhases_DisseminationToken)]
        public sealed class DisseminationToken : QS.Fx.Serialization.ISerializable
        {
            #region Constructor

            public DisseminationToken()
            {
            }

            #endregion

            #region Fields

            [QS.Fx.Printing.Printable]
            public VERSIONED<UINT> _aggr_Last;
            [QS.Fx.Printing.Printable]
            public VERSIONED<UINT> _diss_Next;
            [QS.Fx.Printing.Printable]
            public VERSIONED<UINT> _diss_Last;

            #endregion

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get
                {
                    QS.Fx.Serialization.SerializableInfo info =
                        new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Protocols_CoordinatedPhases_DisseminationToken);
                    info.AddAnother(_aggr_Last.SerializableInfo);
                    info.AddAnother(_diss_Next.SerializableInfo);
                    info.AddAnother(_diss_Last.SerializableInfo);
                    return info;
                }
            }

            void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                _aggr_Last.SerializeTo(ref header, ref data);
                _diss_Next.SerializeTo(ref header, ref data);
                _diss_Last.SerializeTo(ref header, ref data);
            }

            void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                _aggr_Last.DeserializeFrom(ref header, ref data);
                _diss_Next.DeserializeFrom(ref header, ref data);
                _diss_Last.DeserializeFrom(ref header, ref data);
            }

            #endregion
        }

        #endregion
    }
}
