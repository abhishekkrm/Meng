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


//#define PROFILE_ADD

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("ED541281D8124dc7BCE2F0F6C0CFA5E2", "ProfileSimpleTasks_")]
    public sealed class ProfileSimpleTasks_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject
        
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        private double Avg(double[] _d)
        {
            double _sum = 0;
            for (int i = 0; i < _d.Length; i++)
            {
                _sum += _d[i];
            }
            _sum /= _d.Length;
            return _sum * 1000000000;
        }

        public ProfileSimpleTasks_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._clock = _mycontext.Platform.Clock;
            Random _r = new Random();

            #region Profile Strings

            int _num_strings = 1000;

            #region Small

            int _small_string_size = 20;
            
            double[] _small_string_samples = new double[_num_strings];

            string[] _strings = new string[_num_strings];
            for (int i = 0; i < _num_strings; i++)
            {
                byte[] _b = new byte[_small_string_size];
                _r.NextBytes(_b);
                _strings[i] = ASCIIEncoding.ASCII.GetString(_b);
            }

            for (int i = 0; i < _num_strings; i += 2)
            {
                
                double _t1 = this._clock.Time;

                if (_strings[i] == _strings[i + 1])
                {
                    double _t2 = this._clock.Time;
                    _small_string_samples[i] = _t2 - _t1;
                }
                else
                {
                    double _t2 = this._clock.Time;
                    _small_string_samples[i] = _t2 - _t1;
                }
                 
            }

            this._mycontext.Platform.Logger.Log("Average time for small string compare: " + Avg(_small_string_samples));
            #endregion

            #region Long

            int _long_string_size = 1024 * 10;
            double[] _long_string_samples = new double[_num_strings];

            _strings = new string[_num_strings];
            for (int i = 0; i < _num_strings; i++)
            {
                byte[] _b = new byte[_long_string_size];
                _r.NextBytes(_b);
                _strings[i] = ASCIIEncoding.ASCII.GetString(_b);
            }

            for (int i = 0; i < _num_strings; i += 2)
            {

                double _t1 = this._clock.Time;

                if (_strings[i] == _strings[i + 1])
                {
                    double _t2 = this._clock.Time;
                    _long_string_samples[i] = _t2 - _t1;
                }
                else
                {
                    double _t2 = this._clock.Time;
                    _long_string_samples[i] = _t2 - _t1;
                }

            }

            this._mycontext.Platform.Logger.Log("Average time for long string compare: " + Avg(_long_string_samples));

            #endregion

            #endregion

            #region Profile Dictionary Adds

            int _num_elements = 1000;

            #region Small

            int _small_key = 20;

            Dictionary<string, string> _dict = new Dictionary<string, string>(_num_elements);
            double[] _small_dict_samples = new double[_num_elements];
            string[] _keys = new string[_num_elements];
            for (int i = 0; i < _num_elements; i++)
            {
                byte[] _b = new byte[_small_key];
                _r.NextBytes(_b);
                _keys[i] = Encoding.ASCII.GetString(_b);
            }

            for (int i = 0; i < _num_elements; i++)
            {
                double _t1 = this._clock.Time;
                _dict.Add(_keys[i], string.Empty);
                double _t2 = this._clock.Time;
                _small_dict_samples[i] = _t2 - _t1;
            }

            this._mycontext.Platform.Logger.Log("Average time for short dict add: " + Avg(_small_dict_samples));

            #endregion

            #region Long

            int _long_key = 1024;


            _dict = new Dictionary<string, string>(_num_elements);
            double[] _long_dict_samples = new double[_num_elements];
            _keys = new string[_num_elements];
            for (int i = 0; i < _num_elements; i++)
            {
                byte[] _b = new byte[_long_key];
                _r.NextBytes(_b);
                _keys[i] = Encoding.ASCII.GetString(_b);
            }

            for (int i = 0; i < _num_elements; i++)
            {
                double _t1 = this._clock.Time;
                _dict.Add(_keys[i], string.Empty);
                double _t2 = this._clock.Time;
                _long_dict_samples[i] = _t2 - _t1;
            }

            this._mycontext.Platform.Logger.Log("Average time for long dict add: " + Avg(_long_dict_samples));

            #endregion
            
            #endregion

            #region Profile Allocate Bytes

            int _num_chunks = 1000;

            #region Small

            int _small_chunk = 1024;


            byte[][] _chunks = new byte[_num_chunks][];
            double[] _small_chunk_samples = new double[_num_chunks];

            for (int i = 0; i < _num_chunks; i++)
            {
                double _t1 = this._clock.Time;
                
                _chunks[i] = new byte[_small_chunk];

                double _t2 = this._clock.Time;
                _small_chunk_samples[i] = _t2 - _t1;
            }

            this._mycontext.Platform.Logger.Log("Average time for short chunk alloc: " + Avg(_small_chunk_samples));

            #endregion

            #region Long

            int _long_chunk = 1024 * 1024;


            _chunks = new byte[_num_chunks][];
            double[] _long_chunk_samples = new double[_num_chunks];

            for (int i = 0; i < _num_chunks; i++)
            {
                double _t1 = this._clock.Time;

                _chunks[i] = new byte[_long_chunk];

                double _t2 = this._clock.Time;
                _long_chunk_samples[i] = _t2 - _t1;
            }

            this._mycontext.Platform.Logger.Log("Average time for long chunk alloc: " + Avg(_long_chunk_samples));

            #endregion

            #endregion

        }


        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields
        [QS.Fx.Base.Inspectable]
      
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;


        #endregion

    }
}
