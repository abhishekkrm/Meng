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

//#define DEBUG_MSG
//#define DEBUG_MSG_2

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("E7219A24E2D440359B8CAC9D7CC8957C")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded |
        QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [Serializable]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Experiment_Component_KMeans)]
    public sealed class KMeans_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Experiment_.Object_.IKMeans_,
        QS._qss_x_.Experiment_.Interface_.IKMeans_, QS.Fx.Replication.IReplicated<KMeans_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public KMeans_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("k value", QS.Fx.Reflection.ParameterClass.Value)] 
            int _k

            )
        {
#if DEBUG_MSG
            _mycontext.Platform.Logger.Log("Entering Constructor");
#endif
            this._mycontext = _mycontext;
            this._k = _k;
            this._log = _mycontext.Platform.Logger;

            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IKMeansClient_,
                    QS._qss_x_.Experiment_.Interface_.IKMeans_>(this);
            this._workendpoint.OnConnected += new QS.Fx.Base.Callback(_workendpoint_OnConnected);
            this._init = true;

            // add a list of points for each cluster, empty until some _Work is done
            for (int i = 0; i < _k; i++)
            {
                _clustering.Add(i, new List<Point3D_>());
            }


#if DEBUG_MSG
            _log.Log("Leaving Constructor");
#endif
        }

        void _workendpoint_OnConnected()
        {
            this._workendpoint.Interface._Set_K(this._k);
        }

        public KMeans_()
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
            QS._qss_x_.Experiment_.Interface_.IKMeansClient_,
                QS._qss_x_.Experiment_.Interface_.IKMeans_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Logging.ILogger _log;


        [QS.Fx.Base.Inspectable]
        private IDictionary<int, List<Point3D_>> _clustering = new Dictionary<int, List<Point3D_>>();
        [QS.Fx.Base.Inspectable]
        private int _k = -1;
        [QS.Fx.Base.Inspectable]
        private bool _init = false;


        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IKMeans_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IKMeansClient_,
                QS._qss_x_.Experiment_.Interface_.IKMeans_>
                    QS._qss_x_.Experiment_.Object_.IKMeans_._Work
        {
            get { return this._workendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region Point3D_

        [QS.Fx.Reflection.ValueClass("973E46CB2EB34567A548A38079517AD9")]
        [Serializable]
        public struct Point3D_
        {
            public Point3D_(double x, double y, double z)
            {
                this._x = x;
                this._y = y;
                this._z = z;
            }

            public double _x, _y, _z;

            public bool EqualsWithPrecision(Point3D_ c2, int _precision)
            {
                if (Math.Round(_x, _precision) == Math.Round(c2._x, _precision) && Math.Round(_y, _precision) == Math.Round(c2._y, _precision) && Math.Round(_z, _precision) == Math.Round(c2._z, _precision))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public static bool operator ==(Point3D_ c1, Point3D_ c2)
            {
                if (c1._x == c2._x && c1._y == c2._y && c1._z == c2._z)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public static bool operator !=(Point3D_ c1, Point3D_ c2)
            {
                if (c1 == c2)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        #endregion


        private double _Distance(Point3D_ _a, Point3D_ _b)
        {
            // sqrt( sum( (p_i - q_i)^2, 1, n ) )

            double _sum = Math.Pow(_a._x - _b._x, 2) + Math.Pow(_a._y - _b._y, 2) + Math.Pow(_a._z - _b._z, 2);
            return Math.Sqrt(_sum);
        }

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IKMeans_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IKMeans_._Work(Array __means, Array __data)
        {
#if DEBUG_MSG
            _log.Log("      Entering Work");
#endif
            try
            {

                Point3D_[] _means = (Point3D_[])__means;
                Point3D_[] _data = (Point3D_[])__data;

                // iterate over each data point we've received
                for (int j = 0; j < _data.Length; j++)
                {
                    double _min = double.PositiveInfinity;
                    int _min_id = -1;

                    // calculate the distance from this point to each mean
                    for (int i = 0; i < _means.Length; i++)
                    {
                        double _d = _Distance(_means[i], _data[j]);
                        if (_d < _min)
                        {
                            _min = _d;
                            _min_id = i;
                        }
                    }

                    // cluster this point with the appropriate mean (min distance)
                    try
                    {
                        _clustering[_min_id].Add(_data[j]);
                    }
                    catch (KeyNotFoundException _e)
                    {
                        _clustering[_min_id] = new List<Point3D_>();
                        _clustering[_min_id].Add(_data[j]);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("F", e);
            }
#if DEBUG_MSG
            _log.Log("      Leaving Work");
#endif
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IKMeans_._Done()
        {
#if DEBUG_MSG_2
            _log.Log("            Entering Done");
#endif
            try
            {
                Point3D_[] _new_means = new Point3D_[this._k];

                // iterate over each k value and find the new mean of the points
                // clustered into this k-value
                for (int j = 0; j < _clustering.Count; j++)
                {
                    double _sum_x, _sum_y, _sum_z;
                    _sum_x = _sum_y = _sum_z = 0;
                    List<Point3D_> _element = _clustering[j];
                    
                    // find the average for this particular k-value
                    for (int i = 0; i < _element.Count; i++)
                    {
                        _sum_x += _element[i]._x;
                        _sum_y += _element[i]._y;
                        _sum_z += _element[i]._z;
                    }
                    _sum_x /= _element.Count;
                    _sum_y /= _element.Count;
                    _sum_z /= _element.Count;

                    _new_means[j] = new Point3D_(_sum_x, _sum_y, _sum_z);

                }


                // reset for the next set of Work calls.
                foreach (KeyValuePair<int, List<Point3D_>> _k in _clustering)
                {
                    _k.Value.Clear();
                }
                
                this._workendpoint.Interface._Done(_new_means);
            }
            catch (Exception e)
            {
                throw new Exception();
            }


#if DEBUG_MSG_2
            _log.Log("            Leaving Done");
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<KMeans_> Members

        void QS.Fx.Replication.IReplicated<KMeans_>.Export(KMeans_ _other)
        {
#if DEBUG_MSG_2
            _log.Log("   Entering Export");
#endif

            _other._log = this._log;
            _other._clustering.Clear();

            // set the k value.
            _other._k = _k;

            // initialize _clustering to ahve a list for each k-value
            for (int i = 0; i < _k; i++)
            {
                _other._clustering.Add(i, new List<Point3D_>());
            }
            //Thread.Sleep(1000);
#if DEBUG_MSG_2
            _log.Log("   Leaving Export");
#endif
        }

        void QS.Fx.Replication.IReplicated<KMeans_>.Import(KMeans_ _other)
        {
#if DEBUG_MSG_2
            _log.Log("         Entering Import");
#endif
            try
            {

                // merge each k-value list one at a time.
                for (int i = 0; i < this._k; i++)
                {
                    List<Point3D_> _list;
                    _list = _other._clustering[i];

                    // add each point to the master list
                    foreach (Point3D_ _p in _list)
                    {
                        try
                        {
                            _clustering[i].Add(_p);
                        }
                        catch (KeyNotFoundException _e)
                        {
                            _clustering[i] = new List<Point3D_>();
                            _clustering[i].Add(_p);
                        }
                    }
                    _list.Clear();
                }
                

            }
            catch (Exception e)
            {
                throw new Exception("D", e);
            }

#if DEBUG_MSG_2
            _log.Log("         Leaving Import");
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int _count = this._clustering.Count;
                int _headersize = sizeof(int) * (2 * _count + 1);
                int _totalsize = _headersize;
                foreach (KeyValuePair<int, List<Point3D_>> _element in this._clustering)
                {
                    _totalsize += sizeof(double) * (_element.Value.Count) * 3;
                }
                return new QS.Fx.Serialization.SerializableInfo(
                    (ushort)QS.ClassID.Experiment_Component_KMeans, _headersize, _totalsize, _count);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            int _count = (this._clustering != null) ? this._clustering.Count : 0;
            fixed (byte* _parray = _header.Array)
            {
                byte* _pbuffer = _parray + _header.Offset;
                *((int*)_pbuffer) = _count;
                _pbuffer += sizeof(int);
                if (_count > 0)
                {
                    foreach (KeyValuePair<int, List<Point3D_>> _element in this._clustering)
                    {
                        *((int*)_pbuffer) = _element.Key;
                        _pbuffer += sizeof(int);
                        *((int*)_pbuffer) = _element.Value.Count;
                        _pbuffer += sizeof(int);
                        foreach (Point3D_ _p in _element.Value)
                        {
                            _data.Add(new QS.Fx.Base.Block(BitConverter.GetBytes(_p._x)));
                            _data.Add(new QS.Fx.Base.Block(BitConverter.GetBytes(_p._y)));
                            _data.Add(new QS.Fx.Base.Block(BitConverter.GetBytes(_p._z)));
                        }
                    }
                }
            }
            _header.consume(sizeof(int) * (2 * _count + 1));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            int _count;
            fixed (byte* _parray = _header.Array)
            {
                byte* _pbuffer = _parray + _header.Offset;
                _count = *((int*)_pbuffer);
                _pbuffer += sizeof(int);
                this._clustering = new Dictionary<int,List<Point3D_>>(_count);
                for(int j =0;j<_count;j++)
                {
                    int _key = *((int*)_pbuffer);
                    _pbuffer += sizeof(int);
                    int _list_len = *((int*)_pbuffer);
                    _pbuffer += sizeof(int);
                    List<Point3D_> _l = new List<Point3D_>(_list_len);
                    for (int i = 0; i < _list_len; i++)
                    {

                        double _x = BitConverter.ToDouble(_data.Array, _data.Offset);
                        _data.consume(sizeof(double));
                        double _y = BitConverter.ToDouble(_data.Array, _data.Offset);
                        _data.consume(sizeof(double));
                        double _z = BitConverter.ToDouble(_data.Array, _data.Offset);
                        _data.consume(sizeof(double));
                        _l.Add(new Point3D_(_x, _y, _z));
                    }
                    this._clustering.Add(_key, _l);
                }
            }
            _header.consume(sizeof(int) * (2 * _count + 1));
        }

        #endregion
    }
}
