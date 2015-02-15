/*
Copyright (c) 2008-2009 Chuck Sakoda. All rights reserved.
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
using System.Linq;
using System.Text;

namespace QS._qss_x_.Experiment_.Utility_
{
    class TimelineCopyPrep_:QS.Fx.Inspection.Inspectable
    {

        public TimelineCopyPrep_(double begin, double end)
        {
            this._begin = begin;
            this._end = end;
            QS._core_e_.Data.XY[] _a = new QS._core_e_.Data.XY[1];
            _a[0].x = _begin;
            _a[0].y = _end;
            _data.Add(_a);
        }

        double _begin, _end;
        IList<QS._core_e_.Data.XY[]> _data = new List<QS._core_e_.Data.XY[]>();


        [QS.Fx.Base.Inspectable]
        public string ExportData
        {
            get
            {
                StringBuilder _sb = new StringBuilder();
                int _longest = 0;
                foreach (QS._core_e_.Data.XY[] _a in _data)
                {
                    if (_a.Length > _longest)
                    {
                        _longest = _a.Length;
                    }
                }

                for (int i = 0; i < _longest; i++)
                {
                    foreach (QS._core_e_.Data.XY[] _d in _data)
                    {
                        if (_d.Length > i)
                        {
                            _sb.Append(_d[i].x + "," + _d[i].y + ",");
                        }
                        else
                        {
                            _sb.Append(",");
                        }
                    }
                    _sb.Remove(_sb.Length - 1, 1);
                    _sb.AppendLine();
                }
                return _sb.ToString();
            }
        }

        public void Copy()
        {
            StringBuilder _sb = new StringBuilder();
            int _longest = 0;
            foreach (QS._core_e_.Data.XY[] _a in _data)
            {
                if (_a.Length > _longest)
                {
                    _longest = _a.Length;
                }
            }

            for (int i = 0; i < _longest; i++)
            {
                foreach (QS._core_e_.Data.XY[] _d in _data)
                {
                    if (_d.Length > i)
                    {
                        _sb.Append(_d[i].x+","+_d[i].y+",");
                    }
                    else
                    {
                        _sb.Append(",,");
                    }
                }
                _sb.Remove(_sb.Length - 1, 1);
                _sb.AppendLine();
            }

            ClipboardThread_ _t = new ClipboardThread_(_sb.ToString().Replace(',', '\t'));
        }

        public void Add(QS._qss_c_.Statistics_.Samples2D _samples)
        {

            _data.Add(_samples.Samples);
        }

        public void Add(double x, double y)
        {
            QS._core_e_.Data.XY[] _arr = new QS._core_e_.Data.XY[1];
            _arr[0].x = x;
            _arr[0].y = y;
            _data.Add(_arr);
        }
    }
}

