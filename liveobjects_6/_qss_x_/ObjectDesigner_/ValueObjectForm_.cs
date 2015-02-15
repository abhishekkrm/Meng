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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;

namespace QS._qss_x_.ObjectDesigner_
{
    public partial class ValueObjectForm_ : Form
    {
        #region Constructor

        public ValueObjectForm_(Type _type, object _object)
        {
            InitializeComponent();

            this.textBox1.Text = _type.FullName;
            this._type = _type;
            this._object = _object;

            _error = false;
            textBox2.ForeColor = Color.Black;
            
            if (_object != null)
                textBox2.Text = _object.ToString();

            if (_type.Equals(typeof(string)))
                comboBox1.Items.Add(new convertor_0_());
            else
            {
                ConstructorInfo _info1 = _type.GetConstructor(new Type[] { typeof(string) });
                if (_info1 != null)
                    comboBox1.Items.Add(new convertor_1_(_info1));
                else
                {
                    MethodInfo _info2 = _type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, null);
                    if (_info2 != null)
                        comboBox1.Items.Add(new convertor_2_(_info2));
                    else
                    {
                        _info2 = _type.GetMethod("FromString", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, null);
                        if (_info2 != null)
                            comboBox1.Items.Add(new convertor_2_(_info2));
                    }
                }
            }
            comboBox1.Items.Add(new convertor_3_(_type));
            comboBox1.SelectedItem = comboBox1.Items[0];

            i_convertor_ _my_convertor = comboBox1.SelectedItem as i_convertor_;
            this.textBox2.Text = _my_convertor.convert(_object);

            this.textBox2.BringToFront();
            this.textBox2.Select();
            this.textBox2.Focus();
            this.textBox2.SelectAll();
        }

        #endregion

        #region Fields

        private Type _type;
        private object _object;
        private QS.Fx.Base.ContextCallback _callback;
        private bool _error;

        #endregion

        #region Accessors

        public object _Object
        {
            get { return this._object; }
        }

        #endregion

        #region textBox2_KeyDown

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bool _done = false;
                lock (this)
                {
                    string _text = textBox2.Text;
                    if ((_text != null) && ((_text = _text.Trim()).Length > 0))
                    {
                        i_convertor_ _my_convertor = comboBox1.SelectedItem as i_convertor_;
                        if (_my_convertor != null)
                        {
                            try
                            {
                                this._object = _my_convertor.convert(textBox2.Text);
                                _done = true;
                            }
                            catch (Exception)
                            {
                                _done = false;
                                _error = true;
                                textBox2.ForeColor = Color.Red;
                            }
                        }
                        else
                        {
                            _done = false;
                            _error = true;
                            textBox2.ForeColor = Color.Red;
                        }
                    }
                    else
                    {
                        this._object = null;
                        _done = true;
                    }
                }
                if (_done)
                    this.Close();
            }
            else
            {
                if (_error)
                {
                    _error = false;
                    textBox2.ForeColor = Color.Black;
                }
            }
        }

        #endregion

        #region i_convertor_

        private interface i_convertor_
        {
            object convert(string _s);
            string convert(object _o);
        }

        #endregion

        #region convertor_0_

        private sealed class convertor_0_ : i_convertor_
        {
            public convertor_0_()
            {
            }

            object i_convertor_.convert(string _s)
            {
                return _s;
            }

            string i_convertor_.convert(object _o)
            {
                return (string) _o;
            }

            public override string ToString()
            {
                return "text";
            }
        }

        #endregion

        #region convertor_1_

        private sealed class convertor_1_ : i_convertor_
        {
            public convertor_1_(ConstructorInfo _info)
            {
                this._info = _info;
            }

            private ConstructorInfo _info;

            object i_convertor_.convert(string _s)
            {
                return _info.Invoke(new object[] { _s });
            }

            string i_convertor_.convert(object _o)
            {
                return (_o != null) ? _o.ToString() : string.Empty;
            }

            public override string ToString()
            {
                return "text";
            }
        }

        #endregion

        #region convertor_2_

        private sealed class convertor_2_ : i_convertor_
        {
            public convertor_2_(MethodInfo _info)
            {
                this._info = _info;
            }

            private MethodInfo _info;

            object i_convertor_.convert(string _s)
            {
                return _info.Invoke(null, new object[] { _s });
            }

            string i_convertor_.convert(object _o)
            {
                return (_o != null) ? _o.ToString() : string.Empty;
            }

            public override string ToString()
            {
                return "text";
            }
        }

        #endregion

        #region convertor_3_

        private sealed class convertor_3_ : i_convertor_
        {
            public convertor_3_(Type _type)
            {
                this._type = _type;
            }

            private Type _type;

            object i_convertor_.convert(string _s)
            {
                using (StringReader _reader = new StringReader(_s))
                {
                    return (new XmlSerializer(_type)).Deserialize(_reader);
                }
            }

            string i_convertor_.convert(object _o)
            {
                if (_o != null)
                {
                    StringBuilder _s = new StringBuilder();
                    using (StringWriter _writer = new StringWriter(_s))
                    {
                        (new XmlSerializer(_o.GetType())).Serialize(_writer, _o);
                    }
                    return _s.ToString();
                }
                else
                    return string.Empty;
            }

            public override string ToString()
            {
                return "xml";
            }
        }

        #endregion
    }
}
