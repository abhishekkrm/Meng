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
using System.IO;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(
        QS.Fx.Reflection.ComponentClasses.LocalFile, "LocalFile", "A file object based on a local filesystem file, useful for debugging.")]
    public sealed class LocalFile 
        : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IValue<byte[]>, QS.Fx.Interface.Classes.IValue<byte[]>
    {
        #region Constructor

        public LocalFile(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("path", QS.Fx.Reflection.ParameterClass.Value)] string _path)        
        {
            this._path = _path;

            this._valueendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IValueClient<byte[]>, QS.Fx.Interface.Classes.IValue<byte[]>>(this);
            this._valueendpoint.OnConnect += new QS.Fx.Base.Callback(this._ClientConnectCallback);
            this._valueendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._ClientDisconnectCallback);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private string _path;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
        QS.Fx.Interface.Classes.IValueClient<byte[]>, QS.Fx.Interface.Classes.IValue<byte[]>> _valueendpoint;

        #endregion

        #region _ClientConnectCallback

        private void _ClientConnectCallback()
        {
            this._valueendpoint.Interface.Set(this._Read());
        }

        #endregion

        #region _ClientDisconnectCallback

        private void _ClientDisconnectCallback()
        {
        }

        #endregion

        #region IValue<byte[]> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IValueClient<byte[]>, QS.Fx.Interface.Classes.IValue<byte[]>> 
                QS.Fx.Object.Classes.IValue<byte[]>.Endpoint
        {
            get { return this._valueendpoint; }
        }

        #endregion

        #region IValue<byte[]> Members

        byte[] QS.Fx.Interface.Classes.IValue<byte[]>.Get()
        {
            return this._Read();
        }

        void QS.Fx.Interface.Classes.IValue<byte[]>.Set(byte[] _value)
        {
            this._Write(_value);
        }

        #endregion

        #region _Read

        private byte[] _Read()
        {
            lock (this)
            {
                if (File.Exists(this._path))
                {
                    using (FileStream _stream = new FileStream(this._path, FileMode.Open, FileAccess.Read))
                    {
                        byte[] _data = new byte[(int)_stream.Length];
                        int _totalread = 0;
                        while (_totalread < _data.Length)
                        {
                            int _read = _stream.Read(_data, _totalread, _data.Length - _totalread);
                            if (_read < 0)
                                throw new Exception("Could not read from the file.");
                            _totalread += _read;
                        }
                        return _data;
                    }
                }
                else
                    return null;
            }
        }

        #endregion

        #region _Write

        private void _Write(byte[] _data)
        {
            lock (this)
            {
                using (FileStream _stream = new FileStream(this._path, FileMode.Create, FileAccess.Write))
                {
                    _stream.Write(_data, 0, _data.Length);
/*
                    int _totalwritten = 0;
                    while (_totalwritten < _data.Length)
                    {
                        int _written = _stream.Write(_data, _totalwritten, _data.Length - _totalwritten);
                        if (_written < 0)
                            throw new Exception("Could not write to the file.");
                        _totalwritten += _written;
                    }
*/
                }
            }
        }

        #endregion
    }
}
