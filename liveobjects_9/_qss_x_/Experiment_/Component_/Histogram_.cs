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

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("99644C7610B94B7AB2C55B44A2D07F6E")]
    [QS.Fx.Base.Synchronization(
        QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [Serializable]
    public sealed class Histogram_ :
        QS._qss_x_.Experiment_.Object_.IHistogram_, QS._qss_x_.Experiment_.Interface_.IHistogram_, QS.Fx.Replication.IReplicated<Histogram_>
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Histogram_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IHistogramClient_,
                    QS._qss_x_.Experiment_.Interface_.IHistogram_>(this);
        }

        public Histogram_()
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
            QS._qss_x_.Experiment_.Interface_.IHistogramClient_,
                QS._qss_x_.Experiment_.Interface_.IHistogram_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        private int[] _histogram = new int[256];
        [QS.Fx.Base.Inspectable]
        private byte[] _bmp = null;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private byte[] _bmp_non_ser = null;
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IHistogram_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IHistogramClient_,
                QS._qss_x_.Experiment_.Interface_.IHistogram_>
                    QS._qss_x_.Experiment_.Object_.IHistogram_._Work
        {
            get { return this._workendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IHistogram_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IHistogram_._Work(int _offset, int _count)
        {
            if (_bmp_non_ser == null)
            {
                if (_bmp == null)
                    throw new Exception("WTF");
                else
                {
                    _bmp_non_ser = _bmp;
                    _bmp = null;
                }
            }
            for (int _i = 0; _i < _count; _i++)
                this._histogram[(int)_bmp_non_ser[_offset + _i]]++;
        }
        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IHistogram_._Work2(byte[] _bitmap) {
            
            for (int _i = 0; _i < _bitmap.Length; _i++)
                this._histogram[(int)_bitmap[_i]]++;
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IHistogram_._Done()
        {
            this._workendpoint.Interface._Done(this._histogram);
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IHistogram_._Set_Bitmap(byte[] _bmp) 
        {
            this._bmp = _bmp;
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IHistogram_._Flush()
        {
            return;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<Histogram_> Members

        void QS.Fx.Replication.IReplicated<Histogram_>.Export(Histogram_ _other)
        {
            Array.Clear(_other._histogram, 0, _other._histogram.Length);
            _other._bmp = _bmp;
        }

        void QS.Fx.Replication.IReplicated<Histogram_>.Import(Histogram_ _other)
        {
            int _count = Math.Min(this._histogram.Length, _other._histogram.Length);
            for (int _i = 0; _i < _count; _i++)
                this._histogram[_i] += _other._histogram[_i];
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
