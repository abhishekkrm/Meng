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
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace QS._core_e_.Repository
{
    [XmlType("data")]
    [Serializable]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public class ScalarAttribute : Attribute, QS.Fx.Inspection.IScalarAttribute, QS.Fx.Inspection.IAttribute, IAttribute
    {
        private static readonly QS._core_c_.Serialization.ISerializer Serializer = QS._core_c_.Serialization.Serializer1.Serializer;
        private const int FILENAMELENGTH = 20;
        private static readonly System.Random random = new Random();

        public static string GenerateFileName(string root)
        {
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            string path, name;
            do
            {
                char[] nameChars = new char[FILENAMELENGTH];
                for (int ind = 0; ind < nameChars.Length; ind++)
                {
                    int n = random.Next(62);
                    nameChars[ind] = (n < 10) ? ((char)(((int) '0') + n)) : ((n < 36) ? ((char)(((int) 'a') + (n - 10))) : ((char)(((int) 'A') + (n - 36))));
                }
                name = new string(nameChars);
                path = root + "\\" + name;
            }
            while (System.IO.File.Exists(path));

            return name;
        }

        public ScalarAttribute(IRepository repository, string key, QS.Fx.Inspection.IScalarAttribute attribute) 
            : base(repository, key, attribute.Name)
        {           
            object valueObject = attribute.Value;

            if (valueObject is QS._core_c_.Statistics.IFileOutput)
            {
                this.filename = QS._core_c_.Helpers.Path.RelativePath(repository.Root + "\\" + key, ((QS._core_c_.Statistics.IFileOutput) valueObject).Filename);                
                this._objectToSerialize = this.cachedObject = null;
            }
            else
            {
                string name = attribute.Name;

                bool saveToFile = !valueObject.GetType().IsSerializable ||
                    valueObject.GetType().GetCustomAttributes(typeof(QS._core_c_.Serialization.BLOBAttribute), true).Length > 0;

                if (saveToFile)
                {
                    this.cachedObject = valueObject;
                    this.filename = GenerateFileName(repository.Root + "\\" + key);
                    this._objectToSerialize = null;

                    string path = repository.Root + "\\" + key + "\\" + filename;
                    if (System.IO.File.Exists(path))
                        throw new Exception("Cannot create attribute, file \"" + path + "\" already exists.");

                    using (FileStream stream = new FileStream(path, FileMode.CreateNew))
                    {
                        byte[] nameBytes = Encoding.Unicode.GetBytes(name);
                        byte[] countBytes = BitConverter.GetBytes(nameBytes.Length);
                        stream.Write(countBytes, 0, countBytes.Length);
                        stream.Write(nameBytes, 0, nameBytes.Length);
                        Serializer.Serialize(cachedObject, stream);
                    }
                }
                else
                {
                    this.filename = null;

                    QS.Fx.Inspection.IAttribute valueAttribute = valueObject as QS.Fx.Inspection.IAttribute;
                    if (valueAttribute != null)
                    {
                        switch (valueAttribute.AttributeClass)
                        {
                            case QS.Fx.Inspection.AttributeClass.COLLECTION:
                                this._objectToSerialize = new AttributeCollection(repository, key, (QS.Fx.Inspection.IAttributeCollection)valueAttribute);
                                break;

                            case QS.Fx.Inspection.AttributeClass.SCALAR:
                                this._objectToSerialize = new ScalarAttribute(repository, key, (QS.Fx.Inspection.IScalarAttribute)valueAttribute);
                                break;
                        }
                    }
                    else
                        this._objectToSerialize = valueObject;

                    this.cachedObject = _objectToSerialize;
                }
            }
        }

        public ScalarAttribute(IRepository repository, string key, string filename) : base(repository, key, null)
        {
            this.filename = filename;

            if (filename != null)
                _Load();
        }

        public ScalarAttribute()
        {
        }

        [QS.Fx.Printing.Printable]
        private string filename;

        [QS.Fx.Printing.Printable]
        private object cachedObject;
        [QS.Fx.Printing.NonPrintable]
        private object _objectToSerialize;

        private void _Load()
        {
            if (_objectToSerialize != null)
                cachedObject = _objectToSerialize;
            else
            {
                string path = repository.Root + "\\" + key + "\\" + filename;
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    byte[] countBytes = new byte[Marshal.SizeOf(typeof(int))];
                    stream.Read(countBytes, 0, countBytes.Length);
                    byte[] nameBytes = new byte[BitConverter.ToInt32(countBytes, 0)];
                    stream.Read(nameBytes, 0, nameBytes.Length);
                    name = Encoding.Unicode.GetString(nameBytes);
                    cachedObject = Serializer.Deserialize(stream);

                    if (cachedObject is Attribute)
                    {
                        ((Attribute)cachedObject).Repository = repository;
                        ((Attribute)cachedObject).Key = key;
                    }
                }
            }
        }

        #region Accessors

        [XmlAttribute("file")]
        public string File
        {
            get { return filename; }
            set { filename = value; }
        }

        [XmlElement("value")]
        public object _ObjectToSerialize
        {
            get { return _objectToSerialize; }
            set { _objectToSerialize = value; }
        }

        [XmlIgnore]
        public object Object
        {
            get { return cachedObject; }
            set { cachedObject = value; }
        }

        #endregion

        #region IScalarAttribute Members

        object QS.Fx.Inspection.IScalarAttribute.Value
        {
            get 
            { 
                lock (this)
                {
                    if (cachedObject == null)
                        _Load();

                    return cachedObject;
                }
            }
        }

        #endregion

        #region IAttribute Members

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.SCALAR; }
        }

        #endregion

        #region IAttribute Members

        string IAttribute.Ref
        {
            get 
            {
                return key + ";" + filename; 
            }
        }

        #endregion

        public override string Key
        {
            set
            {
                base.Key = value;
                if (_objectToSerialize is Attribute)
                {
                    ((Attribute)_objectToSerialize).Repository = repository;
                    ((Attribute)_objectToSerialize).Key = value;
                }
                if (cachedObject is Attribute)
                {
                    ((Attribute)_objectToSerialize).Repository = repository;
                    ((Attribute)cachedObject).Key = value;
                }
            }
        }
    }
}
