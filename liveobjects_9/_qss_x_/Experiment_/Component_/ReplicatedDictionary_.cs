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

//#define AUTOCLEAR
#define USE_STRING_STRING
#define PROFILE_ADD

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("7C075B9550374d83880EE9521E849B6D", "ReplicatedDictionary_")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded |
        QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Experiment_Component_ReplicatedDictionary)]
    [Serializable]
    public sealed class ReplicatedDictionary_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Experiment_.Object_.IDictionary_,
        QS._qss_x_.Experiment_.Interface_.IDictionary_, QS.Fx.Replication.IReplicated<ReplicatedDictionary_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public ReplicatedDictionary_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._clock = _mycontext.Platform.Clock;
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IDictionaryClient_,
                    QS._qss_x_.Experiment_.Interface_.IDictionary_>(this);
#if PROFILE_ADD
            this._replica_add_samples = new List<QS._qss_c_.Statistics_.Samples2D>();
#endif
        }

        public ReplicatedDictionary_()
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
            QS._qss_x_.Experiment_.Interface_.IDictionaryClient_,
                QS._qss_x_.Experiment_.Interface_.IDictionary_> _workendpoint;

        [QS.Fx.Base.Inspectable]
        private int _ratio;
        [QS.Fx.Base.Inspectable]
        private int _last_count = 0;
        [QS.Fx.Base.Inspectable]
        private int _count = 0;
        [QS.Fx.Base.Inspectable]
        private IDictionary<string,
#if USE_STRING_STRING
 string
#else
        IList<string>
#endif
> _dict = new Dictionary<string,
#if USE_STRING_STRING
 string
#else
                IList<string>
#endif
>();
        [QS.Fx.Base.Inspectable]
        private const int _ratio_factor = 10;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
#if PROFILE_ADD
        
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _add_samples = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        private IList<QS._qss_c_.Statistics_.Samples2D> _replica_add_samples;
#endif
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IDictionary_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS._qss_x_.Experiment_.Interface_.IDictionaryClient_, QS._qss_x_.Experiment_.Interface_.IDictionary_> QS._qss_x_.Experiment_.Object_.IDictionary_._Work
        {
            get { return this._workendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IDictionary_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Add(string _key, string _value)
        {
#if PROFILE_ADD
            double _t1 = this._clock.Time;
#endif
#if USE_STRING_STRING
            
                _dict.Add(_key, _value);
            
#else
            
                if (!_dict.ContainsKey(_key))
                {
                    IList<string> _l = new List<string>();
                    _l.Add(_value);
                    _dict.Add(_key, _l);
                }
                else
                {
                    _dict[_key].Add(_value);
                }
            
#endif
#if PROFILE_ADD
                double _t2 = this._clock.Time;
                this._add_samples.Add(_t1, _t2 - _t1);
#endif
        }


        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._AddFromFile(string _path, int start, int length)
        {
#if !USE_STRING_STRING
            using (TextReader _tr = new StreamReader(new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                string _line;
                int _line_num = 0;
                while ((_line = _tr.ReadLine()) != null)
                {
                    if (_line_num++ < start)
                        continue;

                    if (_line_num > start + length)
                        break;

                    string[] _t = _line.Split(':');
                    if (_t.Length != 2)
                    {
                        throw new Exception("In file (" + _path + "), improperly formatted line: " + _line);
                    }
                    string _key = _t[0];
                    string _value = _t[1];
                    if (!_dict.ContainsKey(_key))
                    {
                        IList<string> _l = new List<string>();
                        _l.Add(_value);
                        _dict.Add(_key, _l);
                    }
                    else
                    {
                        _dict[_key].Add(_value);
                    }
                }
            }
#endif

        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Get(string _key)
        {

            try
            {
                this._workendpoint.Interface._Got(_key, _dict[_key]);
            }
            catch (Exception e)
            {
                this._workendpoint.Interface._Got(_key, null);
            }
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Clear()
        {
            this._dict.Clear();

            this._workendpoint.Interface._Cleared();
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Set_Ratio(double _ratio)
        {
            this._ratio = (int)(_ratio * _ratio_factor);
        }


        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Count()
        {
            this._workendpoint.Interface._Counted(this._dict.Count);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<Dictionary_> Members

        void QS.Fx.Replication.IReplicated<ReplicatedDictionary_>.Export(ReplicatedDictionary_ _other)
        {
            _other._dict.Clear();
            _other._clock = this._clock;
        }



        void QS.Fx.Replication.IReplicated<ReplicatedDictionary_>.Import(ReplicatedDictionary_ _other)
        {
#if PROFILE_ADD
            this._replica_add_samples.Add(_other._add_samples);
#endif
#if !AUTOCLEAR
#if USE_STRING_STRING
            foreach (KeyValuePair<string, string> _element in _other._dict)
            {

                if (!this._dict.ContainsKey(_element.Key))
                    this._dict.Add(_element);
                else
                {
                    this._dict[_element.Key] = _element.Value;
                }
            }
            this._last_count = this._dict.Count;
#else
            foreach (KeyValuePair<string, IList<string>> _element in _other._dict)
            {
                IList<string> _list;
                if (!this._dict.TryGetValue(_element.Key, out _list))
                {
                    this._dict.Add(_element);
                }
                else
                {

                    if (!_element.Value.Equals(_list))
                    {
                        foreach (string _v in _element.Value)
                        {



                            _list.Add(_v);
                        }
                    }

                }
            }
#endif
#endif
        }



        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
#if AUTOCLEAR
                this._dict.Clear();
#endif
                int _count = this._dict.Count;

                int _headersize = sizeof(int) * (2 * _count + 1);

                int _totalsize = _headersize;

#if USE_STRING_STRING
                foreach (KeyValuePair<string, string> _element in this._dict)
                {
                    _totalsize += _element.Value.Length;
                }

#else
                foreach (KeyValuePair<string, IList<string>> _element in this._dict)
                {
                    _totalsize += _element.Key.Length;
                    foreach (string _s in _element.Value)
                    {
                        _totalsize += _s.Length;
                        _totalsize += 4;
                    }
                }
#endif
                return new QS.Fx.Serialization.SerializableInfo(
                    (ushort)QS.ClassID.Experiment_Component_ReplicatedDictionary, _headersize, _totalsize, _count);
            }

        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
#if AUTOCLEAR
            this._dict.Clear();
#endif
#if !USE_STRING_STRING
            int _count = (this._dict != null) ? this._dict.Count : 0;
            fixed (byte* _parray = _header.Array)
            {
                byte* _pbuffer = _parray + _header.Offset;
                *((int*)_pbuffer) = _count;
                _pbuffer += sizeof(int);

                if (_count > 0)
                {
                    foreach (KeyValuePair<string, IList<string>> _element in this._dict)
                    {
                        *((int*)_pbuffer) = _element.Key.Length;
                        _pbuffer += sizeof(int);

                        *((int*)_pbuffer) = _element.Value.Count;
                        _pbuffer += sizeof(int);

                        _data.Add(new QS.Fx.Base.Block(Encoding.ASCII.GetBytes(_element.Key)));

                        for (int i = 0; i < _element.Value.Count; i++)
                        {
                            _data.Add(new QS.Fx.Base.Block(BitConverter.GetBytes(_element.Value[i].Length)));
                            _data.Add(new QS.Fx.Base.Block(Encoding.ASCII.GetBytes(_element.Value[i])));
                        }
                    }
                }
            }
            _header.consume(sizeof(int) * (2 * _count + 1));
#endif
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            int _count;
#if AUTOCLEAR
            _count = 0;
            this._dict = new Dictionary<string, IList<string>>(0);
#else
#if !USE_STRING_STRING
            fixed (byte* _parray = _header.Array)
            {
                byte* _pbuffer = _parray + _header.Offset;
                _count = *((int*)_pbuffer);
                _pbuffer += sizeof(int);
                this._dict = new Dictionary<string, IList<string>>(_count);
                while (_count-- > 0)
                {
                    int _key_length = *((int*)_pbuffer);
                    _pbuffer += sizeof(int);
                    int _inner_count = *((int*)_pbuffer);
                    _pbuffer += sizeof(int);

                    string _key = Encoding.ASCII.GetString(_data.Array, _data.Offset, _key_length);
                    _data.consume(_key_length);

                    IList<string> _tmp_list = new List<string>(_inner_count);
                    for (int i = 0; i < _inner_count; i++)
                    {
                        int _str_len = BitConverter.ToInt32(_data.Array, _data.Offset);
                        _data.consume(sizeof(int));
                        string _ele = Encoding.ASCII.GetString(_data.Array, _data.Offset, _str_len);
                        _data.consume(_str_len);

                        _tmp_list.Add(_ele);
                    }

                    this._dict.Add(_key, _tmp_list);

                }
            }
            _header.consume(sizeof(int) * (2 * _count + 1));
#endif
#endif
        }

        #endregion


        #region IDictionary_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._AddMultiple(string[] _key, string[] _value)
        {
#if !USE_STRING_STRING
            if (_key.Length != _value.Length)
                throw new Exception("ERRR");
            for (int i = 0; i < _key.Length; i++)
            {

                IList<string> _list;
                if (!this._dict.TryGetValue(_key[i], out _list))
                {
                    _list = new List<string>();
                    _list.Add(_value[i]);
                    this._dict.Add(_key[i], _list);
                }
                else
                {
                    _list.Add(_value[i]);
                }
            }
#endif
        }

        #endregion


        #region IDictionary_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._DumpStats()
        {
#if PROFILE_ADD
            if (this._replica_add_samples.Count == 8)
            {
                double _min_x = double.MaxValue;
                double _max_x = 0;
                foreach (QS._qss_c_.Statistics_.Samples2D _s in this._replica_add_samples)
                {
                    foreach (QS._core_e_.Data.XY _xy in _s.Samples)
                    {
                        if (_xy.x + _xy.y > _max_x)
                        {
                            _max_x = _xy.x + _xy.y;
                        }
                        if (_xy.x < _min_x)
                        {
                            _min_x = _xy.x;
                        }
                    }
                }
                this._mycontext.Platform.Logger.Log("Add duration: " + (_max_x - _min_x).ToString());
            }
#endif
        }

        #endregion
    }
}
