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
    public class Sample2D_Merger_
    {

        public Sample2D_Merger_()
        {

        }

        private class Offset_ : QS.Fx.Inspection.Inspectable
        {
            public Offset_(QS._qss_c_.Statistics_.Samples2D _d)
            {
                this._xy = _d.Samples;
            }
            [QS.Fx.Base.Inspectable]
            public QS._core_e_.Data.XY[] _xy;
            [QS.Fx.Base.Inspectable]
            public int offset = 0;
        }

        public QS._qss_c_.Statistics_.Samples2D _Merge_Samples(QS._qss_c_.Statistics_.Samples2D[] _list)
        {
            IList<Offset_> _l = new List<Offset_>();
            foreach (QS._qss_c_.Statistics_.Samples2D _s in _list)
            {
                _l.Add(new Offset_(_s));
            }
            return _Merge_Samples(_l);
        }

        public QS._qss_c_.Statistics_.Samples2D _Merge_Samples(IList<QS._qss_c_.Statistics_.Samples2D> _list)
        {
            IList<Offset_> _l = new List<Offset_>();
            foreach (QS._qss_c_.Statistics_.Samples2D _s in _list)
            {
                _l.Add(new Offset_(_s));
            }
            return _Merge_Samples(_l);
        }


        private QS._qss_c_.Statistics_.Samples2D _Merge_Samples(IList<Offset_> _list)
        {
            QS._qss_c_.Statistics_.Samples2D _ret = new QS._qss_c_.Statistics_.Samples2D();
            QS._core_e_.Data.XY _last = new QS._core_e_.Data.XY(0, 0);
            while (!_WorktimesEmpty(_list))
            {
                QS._core_e_.Data.XY _p;

                _p = _MinPoint_X(_list);

                while (_p.x == _last.x && _p.y == _last.y)
                {
                    _p = _MinPoint_X(_list);
                }


                _ret.Add(_p.x, _p.y);
            }

            return _ret;
        }

        private QS._core_e_.Data.XY _MinPoint_X(IList<Offset_> _list)
        {
            double _min_x = double.PositiveInfinity;
            int _index = -1;
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].offset < _list[i]._xy.Length &&
                    _list[i]._xy[_list[i].offset].x < _min_x)
                {
                    _index = i;
                    _min_x = _list[i]._xy[_list[i].offset].x;
                }
            }
            return _list[_index]._xy[_list[_index].offset++];
        }



        private bool _WorktimesEmpty(IList<Offset_> _list)
        {
            int _num_empty = 0;
            foreach (Offset_ _o in _list)
            {
                if (_o._xy.Length == _o.offset)
                {
                    _num_empty++;
                }
            }
            if (_num_empty == _list.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}

