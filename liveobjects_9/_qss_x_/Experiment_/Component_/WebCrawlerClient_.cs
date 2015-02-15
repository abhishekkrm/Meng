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

#define PROFILE_CPU_UTIL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("F9F0B476B22C49E388A6ADADCA90A862")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class WebCrawlerClient_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IWebCrawlerClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public WebCrawlerClient_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("url", QS.Fx.Reflection.ParameterClass.Value)] 
            string _url,
            [QS.Fx.Reflection.Parameter("levels", QS.Fx.Reflection.ParameterClass.Value)] 
            int _levels,
            [QS.Fx.Reflection.Parameter("work", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IWebCrawler_> _workreference)
        {
            this._mycontext = _mycontext;
            this._url = _url;
            this._levels = _levels;
            this._init_levels = _levels;
            this._workreference = _workreference;
            this._workproxy = this._workreference.Dereference(this._mycontext);
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IWebCrawler_,
                    QS._qss_x_.Experiment_.Interface_.IWebCrawlerClient_>(this);
            this._workconnection = this._workendpoint.Connect(this._workproxy._Work);
            this._received = 0;
            this._submitted = 1;
            this._begin = new double[this._levels];
            this._end = new double[this._levels];
#if PROFILE_CPU_UTIL
            this._cpuutil = new QS._qss_x_.Experiment_.Utility_.CPUUtil_(_mycontext);
            this._cpuutil.Start();
#endif
            this._begin[0] = this._mycontext.Platform.Clock.Time;

            this._started = this._mycontext.Platform.Clock.Time;
            this._workendpoint.Interface._Work(_url);
            
            this._workendpoint.Interface._Done();

            //for (int _i = 1; _i <= _count; _i++)
            //    this._workerendpoint.Interface._Work(_i.ToString());
            //this._workerendpoint.Interface._Done();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields
        
        [QS.Fx.Base.Inspectable]
        private double[] _begin;
        [QS.Fx.Base.Inspectable]
        private double _started;
        
        [QS.Fx.Base.Inspectable]
        private double[] _end;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _level_times = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _update_tree_times = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private string _url;
        [QS.Fx.Base.Inspectable]
        private int _levels;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IWebCrawler_> _workreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IWebCrawler_ _workproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IWebCrawler_,
                QS._qss_x_.Experiment_.Interface_.IWebCrawlerClient_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _workconnection;
        [QS.Fx.Base.Inspectable]
        private IDictionary<string, GraphNode_> _nodes = new Dictionary<string, GraphNode_>();
        [QS.Fx.Base.Inspectable]
        private GraphNode_ _root;
        [QS.Fx.Base.Inspectable]
        private bool _tree_init = false;
        [QS.Fx.Base.Inspectable]
        private int _init_levels, _received, _submitted;

#if PROFILE_CPU_UTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.CPUUtil_ _cpuutil;
#endif

        #endregion




        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region GraphNode_ 

        private class GraphNode_
        {
            public GraphNode_(string _thisurl, ICollection<GraphNode_> _tourls)
            {
                this._thisurl = _thisurl;
                this._tourls = _tourls;
            }
            public string _thisurl;
            public ICollection<GraphNode_> _tourls;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region Utility

        private ICollection<GraphNode_> _MakeNodes(ICollection<string> _tourls)
        {
            ICollection<GraphNode_> _url_node = new List<GraphNode_>();
            foreach (string _url in _tourls)
            {
                GraphNode_ _node;
                if (!_nodes.TryGetValue(_url, out _node))
                {
                    _node = new GraphNode_(_url, null);
                    _nodes.Add(_url, _node);
                }
                _url_node.Add(_node);
            }
            return _url_node;
        }

        private void _UpdateTree(string _fromurl, ICollection<string> _tourls)
        {
            if (!_tree_init) // first time
            {
                _tree_init = true;
                _root = new GraphNode_(_fromurl, _MakeNodes(_tourls));
            }
            else
            {
                GraphNode_ _node;
                if (!_nodes.TryGetValue(_fromurl, out _node))
                {
                    // i think this is never exec'd since _root should have the first _fromurl after _tree_init

                    _node = new GraphNode_(_fromurl, _MakeNodes(_tourls));
                    _nodes.Add(_fromurl, _node);
                    //throw new Exception("Wrong");
                }
                else
                {
                    if (_node._tourls == null)
                    {
                        _node._tourls = this._MakeNodes(_tourls);
                    }
                }
            }
        }
        
        private double GetAvg(QS._qss_c_.Statistics_.Samples2D _samples)
        {
            double _total = 0;
            foreach (QS._core_e_.Data.XY _sample in _samples.Samples)
                _total += _sample.y;
            _total = _total / ((double)_samples.Samples.Length);
            return _total;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IWebCrawlerClient_ Members

        void QS._qss_x_.Experiment_.Interface_.IWebCrawlerClient_._Done(IDictionary<string, IDictionary<string, bool>> _fromto)
        {
            
            double _t1 = this._mycontext.Platform.Clock.Time;
            this._end[_init_levels - _levels] = _t1;
            this._level_times.Add(_t1, _t1 - this._begin[_init_levels - _levels]);
            int _count = 0;
            
            double _t2 = this._mycontext.Platform.Clock.Time;

            foreach (KeyValuePair<string, IDictionary<string, bool>> _kvp in _fromto)
            {
                _count += _kvp.Value.Count;
                this._UpdateTree(_kvp.Key, _kvp.Value.Keys.ToList());
            }
            this._mycontext.Platform.Logger.Log("Level " + (_levels) + ": " + _count);

            this._update_tree_times.Add(_t1, this._mycontext.Platform.Clock.Time - _t2);

            if (--_levels > 0)
            {
                this._begin[_init_levels - _levels] = this._mycontext.Platform.Clock.Time;
                foreach (KeyValuePair<string, IDictionary<string, bool>> _kvp in _fromto)
                {
                    foreach (KeyValuePair<string, bool> _url in _kvp.Value)
                    {
                        if (!_nodes.ContainsKey(_url.Key) || _nodes[_url.Key]._tourls == null)
                        {
                            this._workendpoint.Interface._Work(_url.Key);
                        }
                    }

                }
                this._workendpoint.Interface._Done();
            }
            else
            {

#if PROFILE_CPU_UTIL
                this._cpuutil.Stop();
                this._cpuutil.PrintAvg();
#endif

                this._mycontext.Platform.Logger.Log("Duration: " + (this._mycontext.Platform.Clock.Time - this._started));
            }

            
            
            
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
