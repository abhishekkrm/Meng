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

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using QS.Fx.Value;
using QS.Fx.Serialization;

namespace Quilt.Core
{
    public sealed class DataBuffer : IComparer<double>
    {
        #region Construct

        public DataBuffer(int _window_size, int _timeout_per_data)
        {
            this._window_size = _window_size;
            this._data_queue = new PriorityQueue<double>(_window_size, this);

            this._timeout_per_data = _timeout_per_data;
            this._bloom_filter = new BloomFilter(12400, 3);
        }

        #endregion

        #region Field

        // Assume that smallest serial number means the closest timeout
        private PriorityQueue<double> _data_queue;
        private Dictionary<string, Data> _data_buffer = new Dictionary<string,Data>();
        private int _window_size;
        private int _timeout_per_data;
        private BloomFilter _bloom_filter;
        private double _latest_expired = 0.0;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region HasReceived

        /// <summary>
        /// Check whether has been received before
        /// </summary>
        /// <param name="_serial_no"></param>
        /// <returns></returns>
        public bool HasReceived(double _serial_no)
        {
            lock (this)
            {
                // Check bloom filter and current dictionary
                string serial_str = _serial_no.ToString();
                return _bloom_filter.Test(serial_str) && (_data_buffer.ContainsKey(serial_str) || _serial_no < _data_queue.Top());
            }
        }

        #endregion

        #region PushData

        /// <summary>
        /// Add the new data
        /// </summary>
        /// <param name="_serial_no"></param>
        /// <param name="_data"></param>
        public void PushData(double _serial_no, ISerializable _data)
        {
            Data data = new Data();
            data._data = _data;
            data._serial_no = _serial_no;
            data._timeout = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + this._timeout_per_data;
            StoreData(data);
        }

        #endregion

        #region StoreData

        /// <summary>
        /// Store received data
        /// </summary>
        /// <param name="data"></param>
        public void StoreData(Data data)
        {
            lock (this)
            {
                try
                {
                    string serial_str = data._serial_no.ToString();
                    _bloom_filter.Add(serial_str);
                    _data_queue.Push(data._serial_no);
                    _data_buffer.Add(serial_str, data);
                }
                catch (Exception exc)
                {
                    throw new Exception("DataBuffer.PushData " + exc.Message);
                }
            }
        }

        #endregion

        #region CheckTimeout

        /// <summary>
        /// Check and remove timeouted data
        /// </summary>
        public void CheckTimout()
        {
            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            lock (this)
            {
                try
                {
                    while (true)
                    {
                        double top_serial = _data_queue.Top();
                        string top_str = top_serial.ToString();
                        Data data = _data_buffer[top_str];
                        if (now < data._timeout)
                        {
                            // Even the top has not passed the current time
                            break;
                        }
                        _data_queue.Pop();
                        _data_buffer.Remove(top_str);
                        _latest_expired = top_serial;
                    }
                }
                catch (Exception exc)
                {
                    throw new Exception("DataBuffer.PushData " + exc.Message);
                }
            }
        }

        #endregion

        #region GetSnapshot

        /// <summary>
        /// Get the snapshot of data held
        /// </summary>
        /// <param name="_snapshot"></param>
        public double GetSnapshot(ref List<double> _snapshot)
        {
            _snapshot.Clear();

            foreach (KeyValuePair<string, Data> kvp in _data_buffer)
            {
                _snapshot.Add(double.Parse(kvp.Key));
            }

            return _latest_expired;
        }

        #endregion

        #region GetData

        public Data GetData(double seq)
        {
            if (seq <= _latest_expired) return null;

            Data data = null;

            _data_buffer.TryGetValue(seq.ToString(), out data);

            return data;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Data

        public class Data
        {
            public double _serial_no;
            public ISerializable _data;
            public double _timeout;
        }

        #endregion

        #region IComparer<double> Members

        public int Compare(double x, double y)
        {
            return (int)(y - x);
        }

        #endregion
    }
}
