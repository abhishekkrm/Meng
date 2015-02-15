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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("BEF870984932455b8DFE05101B1A87C9", "SplayTreeDictionary_")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded |
        QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Experiment_Component_SplayTreeDictionary)]
    [Serializable]
    public sealed class SplayTreeDictionary_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Experiment_.Object_.IDictionary_,
        QS._qss_x_.Experiment_.Interface_.IDictionary_, QS.Fx.Replication.IReplicated<SplayTreeDictionary_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public SplayTreeDictionary_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IDictionaryClient_,
                    QS._qss_x_.Experiment_.Interface_.IDictionary_>(this);
        }

        public SplayTreeDictionary_()
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
        private QS._qss_c_.Collections_2_.SplayOf<string, StringNode> _dict = new QS._qss_c_.Collections_2_.SplayOf<string, StringNode>();
        //private QS._qss_x_.Experiment_.Utility_.SplayTree_ _dict = new QS._qss_x_.Experiment_.Utility_.SplayTree_();
        [QS.Fx.Base.Inspectable]
        private const int _ratio_factor = 10;

        #endregion

        private class StringNode : QS._qss_c_.Collections_2_.BTNOf<string>
        {
            public StringNode(string key, string value)
                : base(key)
            {
                this._key = key;
                this._val = value;
            }

            public StringNode() { }

            private string _val;
            private string _key;
            public string Value
            {
                get
                {
                    return this._val;
                }
                set
                {
                    this._val = value;
                }
            }

            public string Key
            {
                get
                {
                    return this._key;
                }
            }
        }

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
            this._dict.Add(new StringNode(_key, _value));
            //this._dict.Add(_key, _value);

        }


        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._AddFromFile(string _path, int start, int length)
        {
            throw new NotImplementedException();

        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Get(string _key)
        {

            try
            {
                this._workendpoint.Interface._Got(_key, _dict[_key].Value);
            }
            catch (Exception e)
            {

            }
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Clear()
        {
            //this._dict = new QS._qss_x_.Experiment_.Utility_.SplayTree_();
            this._dict.Clear();

            this._workendpoint.Interface._Cleared();
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Set_Ratio(double _ratio)
        {
            throw new NotImplementedException();
        }


        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Count()
        {
            this._workendpoint.Interface._Counted(this._dict.Count);
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._AddMultiple(string[] _key, string[] _value)
        {
            throw new NotImplementedException();
        }


        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<Dictionary_> Members

        void QS.Fx.Replication.IReplicated<SplayTreeDictionary_>.Export(SplayTreeDictionary_ _other)
        {
            _other._dict = new QS._qss_c_.Collections_2_.SplayOf<string, StringNode>();
            //_other._dict = new QS._qss_x_.Experiment_.Utility_.SplayTree_();
        }



        void QS.Fx.Replication.IReplicated<SplayTreeDictionary_>.Import(SplayTreeDictionary_ _other)
        {

            foreach (StringNode _node in _other._dict)
            {

                //if (!this._dict.Contains(_node))
                    this._dict.Add(_node);
                //else
                //{
                //    this._dict[_node.Key].Value = _node.Value;
                //}
            }
            //this._dict.Add(_other._dict.
            this._last_count = this._dict.Count;

        }



        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                throw new NotImplementedException();
                int _count = this._dict.Count;

                int _headersize = sizeof(int) * (2 * _count + 1);

                int _totalsize = _headersize;


                //foreach (KeyValuePair<string, string> _element in this._dict)
                //{
                //    _totalsize += _element.Value.Length;
                //}


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


        void QS._qss_x_.Experiment_.Interface_.IDictionary_._DumpStats()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
