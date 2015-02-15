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
using System.Runtime.InteropServices;

namespace QS._qss_e_.Repository_
{
    public class Repository : QS._core_e_.Repository.IRepository
    {
        public static KeyValuePair<string, object> LoadBinObj(string file_name)
        {
            string object_name;
            object loaded_object;
            LoadBinObj(file_name, out object_name, out loaded_object);
            return new KeyValuePair<string, object>(object_name, loaded_object);
        }

        public static void LoadBinObj(string file_name, out string object_name, out object loaded_object)
        {
            using (FileStream stream = new FileStream(file_name, FileMode.Open))
            {
                byte[] countBytes = new byte[Marshal.SizeOf(typeof(int))];
                stream.Read(countBytes, 0, countBytes.Length);
                byte[] nameBytes = new byte[BitConverter.ToInt32(countBytes, 0)];
                stream.Read(nameBytes, 0, nameBytes.Length);
                object_name = Encoding.Unicode.GetString(nameBytes);
                loaded_object = ((QS._core_c_.Serialization.ISerializer)QS._core_c_.Serialization.Serializer1.Serializer).Deserialize(stream);
            }
        }

        public static QS.Fx.Inspection.IAttribute Save(QS._core_e_.Repository.IRepository repository, string[] path, object obj)
        {
            if (path.Length < 2)
                throw new Exception("Path too short.");

            string foldername = repository.Root + "\\" + path[0];
            if (!Directory.Exists(foldername))
                Directory.CreateDirectory(foldername);

            QS._core_e_.Repository.Attribute keyAttribute;
            string keymainfile = foldername + "\\root.xml";
            if (File.Exists(keymainfile))
            {
                using (StreamReader reader = new StreamReader(keymainfile))
                {
                    keyAttribute = (QS._core_e_.Repository.Attribute)(new System.Xml.Serialization.XmlSerializer(typeof(Attribute))).Deserialize(reader);
                    keyAttribute.Repository = repository;
                    keyAttribute.Key = path[0];
                }
            }
            else
            {
                keyAttribute = new QS._core_e_.Repository.AttributeCollection(repository, path[0], new QS.Fx.Inspection.AttributeCollection());
            }

            QS._core_e_.Repository.Attribute attribute = keyAttribute;
            int level = 1;
            while (true)
            {
                if (!(attribute is QS._core_e_.Repository.AttributeCollection))
                    throw new Exception("Cannot add, the path contains a prefix that is not an attribute collection.");
                QS._core_e_.Repository.AttributeCollection collection = (QS._core_e_.Repository.AttributeCollection)attribute;

                bool alreadyThere = collection.TryGet(path[level], out attribute);
                bool isLast = level == (path.Length - 1);                

                if (isLast)
                {
                    if (alreadyThere)
                        throw new Exception("Attribute already exists.");
                    else
                    {
                        attribute = new QS._core_e_.Repository.ScalarAttribute(repository, path[0], new QS.Fx.Inspection.ScalarAttribute(path[level], obj));
                        collection.Add(attribute.Name, attribute);
                        break;
                    }
                }
                else
                {
                    if (!alreadyThere)
                    {
                        attribute = new QS._core_e_.Repository.AttributeCollection(repository, path[0], path[level]);
                        collection.Add(attribute.Name, attribute);
                    }
                }

                level++;
            }

            using (StreamWriter writer = new StreamWriter(keymainfile))
            {
                (new System.Xml.Serialization.XmlSerializer(typeof(QS._core_e_.Repository.Attribute))).Serialize(writer, keyAttribute);
            }

            return attribute;
        }

        public static void Save(QS._core_e_.Repository.IRepository repository, string key, QS._core_e_.Repository.Attribute attribute)
        {
            string foldername = repository.Root + "\\" + key;

            if (!Directory.Exists(foldername))
                Directory.CreateDirectory(foldername);

            string filename = foldername + "\\root.xml";

            using (StreamWriter writer = new StreamWriter(filename))
            {
                (new System.Xml.Serialization.XmlSerializer(typeof(Attribute))).Serialize(writer, attribute);
            }
        }

        public Repository(string root)
        {
            this.root = root;
        }

        private string root;

        #region IRepository Members

        QS._core_e_.Repository.IAttribute QS._core_e_.Repository.IRepository.AttributeOf(string reference)
        {
            int semicolon = reference.IndexOf(';');
            string key = reference.Substring(0, semicolon);
            string filename = reference.Substring(semicolon + 1);
            if (File.Exists(root + "\\" + key + "\\" + filename))
                return new QS._core_e_.Repository.ScalarAttribute(this, key, filename);
            else
                return null;
        }

        string QS._core_e_.Repository.IRepository.Root
        {
            get { return root; }
        }

        void QS._core_e_.Repository.IRepository.Add(string name, QS.Fx.Inspection.IAttribute attribute)
        {
            if (name.Contains(";"))
                throw new Exception("Name of an element added to the root folder of the repository cannot contain semicolons.");

            lock (this)
            {
                // Inspection.IAttributeCollection attributeCollection = attribute as Inspection.IAttributeCollection;
                // if (attributeCollection == null)
                // throw new Exception("This repository can only store attribute collections.");

                string foldername = root + "\\" + name;
                string filename = foldername + "\\root.xml";

                if (Directory.Exists(foldername) && File.Exists(filename))
                    throw new Exception("Attribute with name \"" + name + "\" already exists in this collection.");
                    
                Directory.CreateDirectory(foldername);

                using (StreamWriter writer = new StreamWriter(filename))
                {
                    QS._core_e_.Repository.Attribute ac;
                    switch (attribute.AttributeClass)
                    {
                        case QS.Fx.Inspection.AttributeClass.COLLECTION:
                            ac = new QS._core_e_.Repository.AttributeCollection(this, name, (QS.Fx.Inspection.IAttributeCollection) attribute);
                            break;

                        case QS.Fx.Inspection.AttributeClass.SCALAR:
                            ac = new QS._core_e_.Repository.ScalarAttribute(this, name, (QS.Fx.Inspection.IScalarAttribute)attribute);
                            break;

                        default:
                            throw new Exception("Unknown attribute type.");
                    }

                    (new System.Xml.Serialization.XmlSerializer(typeof(Attribute))).Serialize(writer, ac);
                }
            }
        }

        #endregion

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get 
            {
                List<string> names = new List<string>();
                foreach (string path in Directory.GetDirectories(root))
                {
                    int from = path.LastIndexOf('\\') + 1;
                    // int to = path.LastIndexOf(".xml");
                    names.Add(path.Substring(from)); // , to - from));
                }
                return names;
            }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get 
            {
                using (StreamReader reader = new StreamReader(root + "\\" + attributeName + "\\root.xml"))
                {
                    QS._core_e_.Repository.Attribute attribute = (QS._core_e_.Repository.Attribute)(new System.Xml.Serialization.XmlSerializer(typeof(Attribute))).Deserialize(reader);
                    attribute.Repository = this;
                    attribute.Key = attributeName;
                    return attribute;
                }                
            }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return root; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
        }

        #endregion
    }
}
