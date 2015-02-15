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
using System.Threading;
using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("7F2001452C4045318677CE3E34A4A031")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded |
        QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [Serializable]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Experiment_Component_WebCrawler)]
    public sealed class WebCrawler_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Experiment_.Object_.IWebCrawler_,
        QS._qss_x_.Experiment_.Interface_.IWebCrawler_, QS.Fx.Replication.IReplicated<WebCrawler_>, 
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public WebCrawler_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IWebCrawlerClient_,
                    QS._qss_x_.Experiment_.Interface_.IWebCrawler_>(this);

            if (!Directory.Exists(_cache_dir))
            {
                Directory.CreateDirectory(_cache_dir);
            }
        }

        public WebCrawler_()
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
            QS._qss_x_.Experiment_.Interface_.IWebCrawlerClient_,
                QS._qss_x_.Experiment_.Interface_.IWebCrawler_> _workendpoint;

        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        const string _cache_dir = @"C:\Users\Public\webcrawlercache";
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();
        [QS.Fx.Base.Inspectable]
        private IDictionary<string, IDictionary<string, bool>> _links = new Dictionary<string, IDictionary<string, bool>>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IWebCrawler_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IWebCrawlerClient_,
                QS._qss_x_.Experiment_.Interface_.IWebCrawler_>
                    QS._qss_x_.Experiment_.Object_.IWebCrawler_._Work
        {
            get { return this._workendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region Utility 

        private void _GetLinks(string _url, string _raw_html)
        {
            if (_url.EndsWith("/"))
            {
                _url = _url.Substring(0, _url.Length - 1);
            }
            IDictionary<string, bool> _dict;
            if (!_links.TryGetValue(_url, out _dict))
            {
                _dict = new Dictionary<string, bool>();
                _links.Add(_url, _dict);
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
                    if (_href_index == -1)
                        break;
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
                    string _tmp;
                    if (_link.ToLower().StartsWith("http"))
                    {
                        _tmp = _link;
                    }
                    else
                    {
                        _tmp = _url + "/" + _link;
                    }
                    if (_tmp.EndsWith("/"))
                    {
                        _tmp = _tmp.Substring(0, _tmp.Length - 1);
                    }
                    _tmp = _tmp.Replace("//", "/").Replace("http:", "http:/");

                    if (_tmp != _url && !_dict.ContainsKey(_tmp))
                        _dict.Add(_tmp, true);


                    _current_index = _end;

                }
                else
                {
                    break;
                }

            }
        }

        private string _GetMD5(string _url)
        {
            if (_md5 == null)
            {
                _md5 = new MD5CryptoServiceProvider();
            }
            byte[] _tmp = _md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(_url));
            StringBuilder _sb = new StringBuilder();
            for (int i = 0; i < _tmp.Length; i++)
            {
                _sb.Append(_tmp[i].ToString("X2"));
            }
            return _sb.ToString();
        }

        private void _Cache(string _html, string _md5_id)
        {
            TextWriter _tw = new StreamWriter(new FileStream(Path.Combine(_cache_dir, _md5_id), FileMode.CreateNew));
            _tw.Write(_html);
            _tw.Close();
        }

        private void TrimAdd(string _url, IDictionary<string, bool> _dict)
        {
            if (_url.EndsWith("/"))
            {
                _url = _url.Substring(0, _url.Length - 1);
            }
            _links.Add(_url, _dict);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IWebCrawler_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IWebCrawler_._Work(string _fromurl)
        {
            if (_fromurl.EndsWith("/"))
            {
                _fromurl = _fromurl.Substring(0, _fromurl.Length - 1);
            }
            string _md5_id = _GetMD5(_fromurl);
            string _path = Path.Combine(_cache_dir, _md5_id);
            if (File.Exists(_path))
            {
                string _html;
                using (TextReader _tr = new StreamReader(new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    _html = _tr.ReadToEnd();
                }
                _GetLinks(_fromurl, _html);
            }
            else
            {
                // download from net



                if (_fromurl.EndsWith(".ico"))
                {
                    if (!_links.ContainsKey(_fromurl))
                        TrimAdd(_fromurl, new Dictionary<string, bool>());
                }
                else if (_fromurl == "http://uportal.cornell.edu")
                {
                    if (!_links.ContainsKey(_fromurl))
                        TrimAdd(_fromurl, new Dictionary<string, bool>());
                }
                else
                {
                    //this._mycontext.Platform.Logger.Log("Miss");
                    return; // this disables actual web access, faster
                    try
                    {
                        WebClient _webclient = new WebClient();
                        _webclient.DownloadStringCompleted +=
                            new DownloadStringCompletedEventHandler(
                                delegate(object _o, DownloadStringCompletedEventArgs _a)
                                {

                                    string _s = _a.Result;
                                    _GetLinks(_fromurl, _s);

                                    // todo
                                });


                        //_webclient.DownloadStringAsync(new Uri(_fromurl));
                        string _str = _webclient.DownloadString(new Uri(_fromurl));
                        _GetLinks(_fromurl, _str);
                        _Cache(_str, _md5_id);
                    }
                    catch (Exception e)
                    {
                        if (_mycontext != null)
                        {
                            _mycontext.Platform.Logger.Log("Exception: Caught on " + _fromurl);
                        }
                    }
                }

            }
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IWebCrawler_._Done()
        {
            this._workendpoint.Interface._Done(this._links);
            this._links = new Dictionary<string, IDictionary<string, bool>>();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<WebCrawler_> Members

        void QS.Fx.Replication.IReplicated<WebCrawler_>.Export(WebCrawler_ _other)
        {
            _other._links.Clear();
        }

        void QS.Fx.Replication.IReplicated<WebCrawler_>.Import(WebCrawler_ _other)
        {
            foreach (KeyValuePair<string, IDictionary<string, bool>> _element in _other._links)
            {
                IDictionary<string, bool> _dict;
                if (!_links.TryGetValue(_element.Key, out _dict))
                {
                    _links.Add(_element);
                }
                else
                {
                    foreach (KeyValuePair<string, bool> _link in _element.Value)
                    {
                        if (!_dict.ContainsKey(_link.Key))
                        {
                            _dict.Add(_link);
                        }
                    }
                }
            }
            _other._links.Clear();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ISerializable Members


        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int _count;

                _count = this._links.Count;

                int _headersize = sizeof(int) * (2 * _count + 1);
                int _c = _count;
                int _totalsize = _headersize;
                foreach (KeyValuePair<string, IDictionary<string, bool>> _element in this._links)
                {
                    if (_c-- > 0)
                    {
                        _totalsize += _element.Key.Length;
                        //_totalsize += sizeof(_element.Value.Count);
                        

                        foreach (KeyValuePair<string, bool> _s in _element.Value)
                        {
                            _totalsize += 4;
                            _totalsize += _s.Key.Length;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                return new QS.Fx.Serialization.SerializableInfo(
                    (ushort)QS.ClassID.Experiment_Component_WebCrawler, _headersize, _totalsize, _count);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            int _count;
            _count = (this._links != null) ? this._links.Count : 0;


            fixed (byte* _parray = _header.Array)
            {
                byte* _pbuffer = _parray + _header.Offset;
                *((int*)_pbuffer) = _count;
                _pbuffer += sizeof(int);

                if (_count > 0)
                {
                    int _c = _count;
                    foreach (KeyValuePair<string, IDictionary<string, bool>> _element in this._links)
                    {
                        if (_c-- > 0)
                        {
                            *((int*)_pbuffer) = _element.Key.Length;
                            _pbuffer += sizeof(int);

                            *((int*)_pbuffer) = _element.Value.Count;
                            _pbuffer += sizeof(int);

                            _data.Add(new QS.Fx.Base.Block(Encoding.ASCII.GetBytes(_element.Key)));

                            foreach(KeyValuePair<string,bool> _s in _element.Value) {
                                _data.Add(new QS.Fx.Base.Block(BitConverter.GetBytes(_s.Key.Length)));
                                _data.Add(new QS.Fx.Base.Block(Encoding.ASCII.GetBytes(_s.Key)));
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
                this._links = new Dictionary<string, IDictionary<string, bool>>(_count);
                while (_count-- > 0)
                {
                    int _key_length = *((int*)_pbuffer);
                    _pbuffer += sizeof(int);
                    int _inner_count = *((int*)_pbuffer);
                    _pbuffer += sizeof(int);

                    string _key = Encoding.ASCII.GetString(_data.Array, _data.Offset, _key_length);
                    _data.consume(_key_length);

                    IDictionary<string,bool> _tmp_list = new  Dictionary<string,bool>(_inner_count);
                    for (int i = 0; i < _inner_count; i++)
                    {
                        int _str_len = BitConverter.ToInt32(_data.Array, _data.Offset);
                        _data.consume(sizeof(int));
                        string _ele = Encoding.ASCII.GetString(_data.Array, _data.Offset, _str_len);
                        _data.consume(_str_len);
                        try
                        {
                            _tmp_list.Add(_ele, true);
                        }
                        catch (Exception e)
                        {

                        }
                    }

                    this._links.Add(_key, _tmp_list);

                }
            }
            _header.consume(sizeof(int) * (2 * _count + 1));
        }
        #endregion
    }
}
