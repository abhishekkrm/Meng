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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("6C165D2FCE99429DBB30DB27EFA5C635")]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class Tester_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Tester_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("filename", QS.Fx.Reflection.ParameterClass.Value)] 
            string _filename,
            [QS.Fx.Reflection.Parameter("cooldown", QS.Fx.Reflection.ParameterClass.Value)] 
            double _cooldown,
            [QS.Fx.Reflection.Parameter("configurations", QS.Fx.Reflection.ParameterClass.Value)] 
            string _configurations)
        {
            this._mycontext = _mycontext;
            this._filename = _filename;
            this._cooldown = _cooldown;
            List<string> _c = new List<string>();
            using (StringReader _reader = new StringReader(_configurations))
            {
                string _configuration;
                while ((_configuration = _reader.ReadLine()) != null)
                {
                    if ((_configuration = _configuration.Trim()).Length > 0)
                        _c.Add(_configuration);
                }
            }
            this._configurations = _c.ToArray();
            this._mycontext.Platform.Scheduler.Schedule(
                new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(delegate(object _o) { this._Work(); })));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private string _filename;
        [QS.Fx.Base.Inspectable]
        private double _cooldown;
        [QS.Fx.Base.Inspectable]
        private string[] _configurations;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region _Work

        private void _Work()
        {
            this._mycontext.Platform.Logger.Log("Filename : " + this._filename);
            int _i = 0;
            foreach (string _c in this._configurations)
            {
                if (this._cooldown > 0)
                {
                    this._mycontext.Platform.Logger.Log("Sleep (" + this._cooldown.ToString() + ")");
                    Thread.Sleep(TimeSpan.FromSeconds(this._cooldown));
                }
                _i++;
                this._mycontext.Platform.Logger.Log("Configuration " + _i.ToString() + "/" + this._configurations.Length.ToString() + " : " + _c);
                Process _process = new Process();
                _process.StartInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
                _process.StartInfo.Arguments = this._filename + " " + _c;
                _process.StartInfo.CreateNoWindow = false;
                _process.StartInfo.UseShellExecute = true;
                _process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                if (!_process.Start())
                    throw new Exception("Canont start the process!");
                else
                    _process.WaitForExit();
            }
            this._mycontext.Platform.Logger.Log("Completed.");
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
