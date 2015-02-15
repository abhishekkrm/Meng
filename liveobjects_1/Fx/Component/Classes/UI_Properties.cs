/*

Copyright (c) 2009 Revant Kapoor, Yilin Qin, Krzysztof Ostrowski. All rights reserved.

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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace QS.Fx.Component.Classes
{
//    public delegate void IPropertyCallback();

    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.UI_Properties, "UI_Properties", "A base class for objects with a graphical user interface which exposes properties.")]
    public class UI_Properties : UserControl, QS.Fx.Object.Classes.ILogging_UI_Properties, QS.Fx.Inspection.IInspectable, QS.Fx.Interface.Classes.IMetadata
    {
        #region Constructor

        public UI_Properties(QS.Fx.Object.IContext context)
        {
            this.context = context;
            this._ui = context.ExportedUI(this);
            this._logging = context.ImportedInterface<QS.Fx.Interface.Classes.ILog>();
            this._properties = context.DualInterface<QS.Fx.Interface.Classes.IMetadataClient,
                                              QS.Fx.Interface.Classes.IMetadata>(this);
            explorableProperties = new Dictionary<string, QS.Fx.Value.Classes.IExplorableMetadata<object>>();
            propertyNames = new List<string>();

            propertyNames.Add("prop1");
            propertyNames.Add("prop2");
            propertyNames.Add("prop3");

            //adding properties
            QS.Fx.Value.Classes.IExplorableMetadata<object> prop1 = new ExplorableProperty<object>("prop1", "test1");
            QS.Fx.Value.Classes.IExplorableMetadata<object> prop1child = new ExplorableProperty<object>("prop1child", "child1");
            prop1.addChild(prop1child.Name);
            explorableProperties.Add("prop1/prop1child", prop1child);

            QS.Fx.Value.Classes.IExplorableMetadata<object> prop2 = new ExplorableProperty<object>("prop2", 123);
            QS.Fx.Value.Classes.IExplorableMetadata<object> prop3 = new ExplorableProperty<object>("prop3", "test3");
            explorableProperties.Add("prop1",prop1);
            explorableProperties.Add("prop2",prop2);
            explorableProperties.Add("prop3",prop3);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("context")]
        private QS.Fx.Object.IContext context;
        [QS.Fx.Base.Inspectable("ui")]
        private QS.Fx.Endpoint.Internal.IExportedUI _ui;
        [QS.Fx.Base.Inspectable("logging")]
        private QS.Fx.Endpoint.Internal.IImportedInterface<QS.Fx.Interface.Classes.ILog> _logging;
        [QS.Fx.Base.Inspectable("properties")]
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IMetadataClient,
                                              QS.Fx.Interface.Classes.IMetadata> _properties;

        private QS.Fx.Inspection.IAttributeCollection _attributecollection;

        public IDictionary<string, QS.Fx.Value.Classes.IExplorableMetadata<object>> explorableProperties;

        private List<string> propertyNames;

        //public delegate void setProp(string name, object value);

        //public setProp pSetProp; // functoin for callback in derived class

  //      private IPropertyCallback DelCall;

        #endregion

        #region IInspectable Members

        QS.Fx.Inspection.IAttributeCollection QS.Fx.Inspection.IInspectable.Attributes
        {
            get
            {
                lock (this)
                {
                    if (this._attributecollection == null)
                        this._attributecollection = new QS.Fx.Inspection.AttributesOf(this);
                    return this._attributecollection;
                }
            }
        }

        #endregion

        #region Log

        protected void Log(string message)
        {
            if (_logging.IsConnected)
                _logging.Interface.Log(message);
        }

        #endregion

 //       void QS.Fx.Interface.Classes.IProperty.GetMetadata(IPropertyCallback Del)
 //       {
            //Del();
            
 //           this.DelCall += Del;
 //       }

        public void propertyChanged(String name, object newValue)
        {
            QS.Fx.Value.Classes.IExplorableMetadata<object> val;
            if (explorableProperties.TryGetValue(name, out val))
            {
                    val.Value = newValue;
                    val.IndexedName = name;
                    if (_properties.IsConnected)
                    {
                       // _properties.Interface.MetadataChanged(val);
                        _properties.Interface.MetadataCallback();
                    }
            }
            
            //if (this.DelCall.GetInvocationList().GetLength(0) != 0)
            //{
            //    this.DelCall.Invoke();
            //}
        }

        #region ILogging_UI_Properties Members

        QS.Fx.Endpoint.Classes.IExportedUI QS.Fx.Object.Classes.ILogging_UI_Properties.UI
        {
            get { return _ui; }
        }

        QS.Fx.Endpoint.Classes.IImportedInterface<QS.Fx.Interface.Classes.ILog> QS.Fx.Object.Classes.ILogging_UI_Properties.Logging
        {
            get { return _logging; }
        }

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IMetadataClient, QS.Fx.Interface.Classes.IMetadata> QS.Fx.Object.Classes.ILogging_UI_Properties.Properties
        {
            get { return _properties; }
        }

        #endregion

        #region Defining properties
        private class ExplorableProperty<ValueType>: QS.Fx.Value.Classes.IExplorableMetadata<ValueType>
        {

            public ExplorableProperty(String name,ValueType value)
            {
                this.name = name;
                this.value = value;
                this.childrenNames = new List<string>();
            }
            string indexedName;
            string name;
            ValueType value;
            List<string> childrenNames;
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> pObject;
            #region IExplorableProperty<string> Members

            string QS.Fx.Value.Classes.IExplorableMetadata<ValueType>.Name
            {
                get
                {
                    return name;
                }
                set
                {
                    this.name = value;
                }
            }

            ValueType QS.Fx.Value.Classes.IExplorableMetadata<ValueType>.Value
            {
                get
                {
                    return this.value;
                }
                set
                {
                    this.value = value;
                    //setTextThroughProperty(value.ToString());
                }
            }

            IEnumerable<string> QS.Fx.Value.Classes.IExplorableMetadata<ValueType>.ChildrenName
            {
                get
                {
                    return childrenNames;
                }
                set
                {
                    childrenNames.Clear();
                    childrenNames.AddRange(value);
                }
            }



            public void addChild(string name)
            {
                childrenNames.Add(name);
            }


            public string IndexedName
            {
                get
                {
                    return this.indexedName;
                }
                set
                {
                    this.indexedName = value;
                }
            }

            #endregion

            #region IExplorableMetadata<ValueType> Members


            public QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> parentObject
            {
                get
                {
                    return pObject;
                }
                set
                {
                    this.pObject = value;
                }
            }

            #endregion
        }
        #endregion

        #region IMetadata Members

        IEnumerable<string> QS.Fx.Interface.Classes.IMetadata.GetMetadata()
        {
            return propertyNames;
        }

        bool QS.Fx.Interface.Classes.IMetadata.TryGetMetadata(string name, out QS.Fx.Value.Classes.IExplorableMetadata<object> value)
        {
            bool result =  explorableProperties.ContainsKey(name);
            if (!result)
            {
                value = null;
                return false;
            }
            
            explorableProperties.TryGetValue(name, out value);
            return true;
        }

        public virtual bool SetMetadata(string name, object value)
        {
           /*
            QS.Fx.Value.Classes.IExplorableMetadata<object> metadata;
            if(!explorableProperties.TryGetValue(name, out metadata))
                return false;
            metadata.Value = value;
            //explorableProperties..Add(name, metadata);
            QS.Fx.Value.Classes.IExplorableMetadata<object> metadata1;
            explorableProperties.TryGetValue(name, out metadata1);
            */
            //this.pSetProp(name, value);
            return true;
        }

        #endregion
    }
}
