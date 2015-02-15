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

//#define PROFILE_1

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("5DD6E888AEA148F48036088A6C9F1EB8")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded |
        QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [Serializable]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Experiment_Component_ReverseIndex)]
    public sealed class ReverseIndex_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Experiment_.Object_.IReverseIndex_,
        QS._qss_x_.Experiment_.Interface_.IReverseIndex_, QS.Fx.Replication.IReplicated<ReverseIndex_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public ReverseIndex_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IReverseIndexClient_,
                    QS._qss_x_.Experiment_.Interface_.IReverseIndex_>(this);
            this._master = true;
        }

        public ReverseIndex_()
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
            QS._qss_x_.Experiment_.Interface_.IReverseIndexClient_,
                QS._qss_x_.Experiment_.Interface_.IReverseIndex_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        private bool _master = false;
#if !USE_LINK_ID
        [QS.Fx.Base.Inspectable]
        private Dictionary<string, IList<string>> _links = new Dictionary<string, IList<string>>();

#else
        [QS.Fx.Base.Inspectable]
        private Dictionary<string, IList<int>> _links = new Dictionary<string, IList<int>>();
#endif
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _work_times = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        private double _size;
#if PROFILE_1
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _import_times = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _export_times = new QS._qss_c_.Statistics_.Samples2D();
#endif
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReverseIndex_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IReverseIndexClient_,
                QS._qss_x_.Experiment_.Interface_.IReverseIndex_>
                    QS._qss_x_.Experiment_.Object_.IReverseIndex_._Work
        {
            get { return this._workendpoint; }
        }

        #endregion



        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReverseIndex_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IReverseIndex_._Work(string[] _paths, int _start_id)
        {

            for (int i = 0; i < _paths.Length; i++)
            {
                string _raw_html;
                using (TextReader _tr = new StreamReader(new FileStream(_paths[i], FileMode.Open)))
                {

                    _raw_html = _tr.ReadToEnd();
                }
                int _current_index = 0;
                int _a_index, _link_index, _href_index;
                int _start, _end;
                _a_index = _link_index = -2;
                int _len = _raw_html.Length;
                while (_current_index < _len)
                {
                    if (_a_index != -1 && _a_index < _current_index)
                    {
                        _a_index = _raw_html.IndexOf("<a ", _current_index);
                    }
                    if (_link_index != -1 && _link_index < _current_index)
                    {
                        _link_index = _raw_html.IndexOf("<link ", _current_index);
                    }


                    if (_a_index < _link_index)
                    {
                        if (_a_index != -1)
                        {
                            _current_index = _a_index;
                        }
                        else
                        {
                            _current_index = _link_index;
                        }
                    }
                    else
                    {
                        if (_link_index != -1)
                        {
                            _current_index = _link_index;
                        }
                        else
                        {
                            _current_index = _a_index;
                        }
                    }

                    if (_current_index > -1)
                    {
                        _href_index = _raw_html.IndexOf("href=", _current_index);
                        _start = _end = 0;
                        char _c = _raw_html[_href_index + 5];
                        switch (_c.ToString())
                        {
                            case "\'":
                                _start = _href_index + 6;
                                _end = _raw_html.IndexOf("\'", _start);
                                break;
                            case "\"":
                                _start = _href_index + 6;
                                _end = _raw_html.IndexOf("\"", _start);
                                break;
                            default:
                                _start = _href_index + 5;
                                int a, b;
                                a = _raw_html.IndexOf(' ', _start);
                                b = _raw_html.IndexOf('>', _start);
                                _end = (a < b) ? a : b;
                                break;
                        }

                        string _link = _raw_html.Substring(_start, _end - _start);
#if !USE_LINK_ID
                        IList<string> _list;
#else
                            IList<int> _list;
#endif
                        if (_links.TryGetValue(_link, out _list))
                        {

#if !USE_LINK_ID
                            _list.Add(_paths[i]);
#else
                                _list.Add(_start_id + i);
#endif
                        }
                        else
                        {
#if !USE_LINK_ID
                            _list = new List<string>();
                            _list.Add(_paths[i]);
                            _links.Add(_link, _list);
#else
                                _list = new List<int>();
                                _list.Add(_start_id + i);
                                _links.Add(_link, _list);
#endif
                        }


                        _current_index = _end;

                    }
                    else
                    {
                        break;
                    }
                }//done with this html file, onto the next
            }


        }



        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IReverseIndex_._Done()
        {
            this._workendpoint.Interface._Done(_links);
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IReverseIndex_._Set_Size(double _size)
        {
            this._size = _size;
        }
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<ReverseIndex_> Members

        void QS.Fx.Replication.IReplicated<ReverseIndex_>.Export(ReverseIndex_ _other)
        {
            _other._size = _size;
#if PROFILE_1
            double _begin=0.0;
            if (_master)
            {
                _begin = this._mycontext.Platform.Clock.Time;
            }
#endif
            _other._links.Clear();
#if PROFILE_1
      
            if (_master)
            {
                double _end = this._mycontext.Platform.Clock.Time;
                this._export_times.Add(_begin, _end - _begin);
            }
#endif
        }

        void QS.Fx.Replication.IReplicated<ReverseIndex_>.Import(ReverseIndex_ _other)
        {
#if PROFILE_1
            double _begin=0.0;
            if (_master)
            {
                _begin = this._mycontext.Platform.Clock.Time;
            }
#endif
            int _c = (int)(_size * _other._links.Count);
            foreach (KeyValuePair<string, IList<
#if !USE_LINK_ID
string
#else
int
#endif
>> _element in _other._links)
            {
                if (_c-- > 0)
                {

#if !USE_LINK_ID
                    IList<string> _list;
#else
                IList<int> _list;
#endif
                    if (!_links.TryGetValue(_element.Key, out _list))
                    {
#if !USE_LINK_ID
                        _list = new List<string>();
#else
                    _list = new List<int>();
#endif
                    }


                    foreach (
#if !USE_LINK_ID
string
#else
int
#endif
 _page in _element.Value)
                    {
                        _list.Add(_page);
                    }
                    _links[_element.Key] = _list;
                }
                else
                {
                    break;
                }
            }
            _other._links.Clear();
#if PROFILE_1
            if (_master)
            {
                double _end = this._mycontext.Platform.Clock.Time;
                this._import_times.Add(_begin, _end - _begin);
            }
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ISerializable Members


        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int _count;
                if (_size == 0)
                {
                    _count = this._links.Count;
                }
                else
                {
                    _count = (int)(this._size * this._links.Count);
                }
                int _headersize = sizeof(int) * (2 * _count + 1);
                int _c = _count;
                int _totalsize = _headersize;
                foreach (KeyValuePair<string, IList<string>> _element in this._links)
                {
                    if (_c-- > 0)
                    {
                        _totalsize += _element.Key.Length;
                        
                        //_totalsize += _element.Value.Count;
                        

                        foreach (string _s in _element.Value)
                        {
                            _totalsize += _s.Length;
                            _totalsize += 4;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                return new QS.Fx.Serialization.SerializableInfo(
                    (ushort)QS.ClassID.Experiment_Component_ReverseIndex, _headersize, _totalsize, _count);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            int _count;
            if (_size == 0)
            {
                _count = (this._links != null) ? this._links.Count : 0;
            }
            else
            {
                _count = (int)(_size * this._links.Count);
            }

            fixed (byte* _parray = _header.Array)
            {
                byte* _pbuffer = _parray + _header.Offset;
                *((int*)_pbuffer) = _count;
                _pbuffer += sizeof(int);

                if (_count > 0)
                {
                    int _c = _count;
                    foreach (KeyValuePair<string, IList<string>> _element in this._links)
                    {
                        if (_c-- > 0)
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
                        else
                        {
                            break;
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
                this._links = new Dictionary<string, IList<string>>(_count);
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

                    this._links.Add(_key, _tmp_list);

                }
            }
            _header.consume(sizeof(int) * (2 * _count + 1));
        }

        #endregion

    }
}
