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
using System.Windows.Forms;
using System.IO;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("DA33AF3434CA4489BEC38158900F458F")]
    [QS.Fx.Base.Synchronization(
        QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
    [Serializable]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Experiment_Component_Membership)]
    public sealed class Membership_ : QS.Fx.Inspection.Inspectable,
        QS._qss_x_.Experiment_.Object_.IMembership_, QS._qss_x_.Experiment_.Interface_.IMembership_, QS.Fx.Replication.IReplicated<Membership_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Membership_(QS.Fx.Object.IContext _mycontext) : this()
        {
            this._mycontext = _mycontext;
            this._membershipendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IMembershipClient_,
                    QS._qss_x_.Experiment_.Interface_.IMembership_>(this);
        }

        public Membership_()
        {
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IMembershipClient_,
                QS._qss_x_.Experiment_.Interface_.IMembership_> _membershipendpoint;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private bool _importing;
        [QS.Fx.Base.Inspectable]
        private IDictionary<string, Member_> _membership = new Dictionary<string, Member_>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region Class Member_

        private sealed class Member_ 
        {
            public Member_(string _id, double _timestamp, bool _online)
            {
                this._id = _id;
                this._timestamp = _timestamp;
                this._online = _online;
            }

            private string _id;
            private double _timestamp;
            private bool _online;

            public string _Id
            {
                get { return this._id; }
            }

            public double _Timestamp
            {
                get { return this._timestamp; }
                set { this._timestamp = value; }
            }

            public bool _Online
            {
                get { return this._online; }
                set { this._online = value; }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IMembership_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IMembershipClient_,
                QS._qss_x_.Experiment_.Interface_.IMembership_>
                    QS._qss_x_.Experiment_.Object_.IMembership_._Membership
        {
            get { return this._membershipendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IMembership_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IMembership_._Update(string _id, double _timestamp, bool _online)
        {
            Member_ _member;
            if (this._membership.TryGetValue(_id, out _member))
            {
                if (_timestamp > _member._Timestamp)
                {
                    _member._Timestamp = _timestamp;
                    _member._Online = _online;
                }
            }
            else
                this._membership.Add(_id, new Member_(_id, _timestamp, _online));
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IMembership_._Done(bool _completed)
        {
            if (_completed)
                this._membershipendpoint.Interface._Done(true);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<Membership_> Members

        void QS.Fx.Replication.IReplicated<Membership_>.Export(Membership_ _other)
        {
            _other._importing = false;
        }

        void QS.Fx.Replication.IReplicated<Membership_>.Import(Membership_ _other)
        {
            if (!this._importing)
            {
                this._importing = true;
                this._membershipendpoint.Interface._Done(false);
            }
            foreach (Member_ _othermember in _other._membership.Values)
            {
                string _id = _othermember._Id;
                Member_ _member;
                if (this._membership.TryGetValue(_id, out _member))
                {
                    double _timestamp = _othermember._Timestamp;
                    if (_timestamp > _member._Timestamp)
                    {
                        _member._Timestamp = _timestamp;
                        _member._Online = _othermember._Online;
                    }
                }
                else
                    this._membership.Add(_id, _othermember);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get { throw new NotImplementedException(); }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            throw new NotImplementedException();
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
