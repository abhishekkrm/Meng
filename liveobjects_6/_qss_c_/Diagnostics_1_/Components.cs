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
using System.Reflection;

namespace QS._qss_c_.Diagnostics_1_
{
/*
    public class Components : IComponentContainer, TMS.Inspection.IAttributeCollection
    {
        public Components(object dataObject) 
            : this((dataObject is IComponentContainer) ? (IComponentContainer) dataObject : ComponentsOf(dataObject))
        {
        }

        private static IEnumerable<KeyValuePair<string, IDiagnosticsComponent>> ComponentsOf(object dataObject)
        {
            if (dataObject != null)
            {
                List<KeyValuePair<string, IDiagnosticsComponent>> components = new List<KeyValuePair<string, IDiagnosticsComponent>>();
                foreach (FieldInfo fieldInfo in dataObject.GetType().GetFields())
                {
                    object value = fieldInfo.GetValue(dataObject);
                    if (value != null)
                    {
                        object[] attributeObjects = fieldInfo.GetCustomAttributes(typeof(ComponentAttribute), false);
                        ComponentAttribute componentAttribute = attributeObjects.Length > 0 ? (attributeObjects[0] as ComponentAttribute) : null;

                        IDiagnosticsComponent component = null;
                        if (typeof(IDiagnosticsComponent).IsAssignableFrom(value.GetType()))
                            component = (IDiagnosticsComponent) value;
                        else if (componentAttribute != null)
                            component = new Components(ComponentsOf(value));

                        if (component != null)
                            components.Add(new KeyValuePair<string, IDiagnosticsComponent>(
                                componentAttribute != null ? componentAttribute.Name : fieldInfo.Name, component));                        
                    }
                }
                return components;
            }
            else
                return null;
        }

        public Components(IEnumerable<KeyValuePair<string,IDiagnosticsComponent>> components)
        {
            this.subcomponents = new Dictionary<string, IDiagnosticsComponent>();
            if (components != null)
                foreach (KeyValuePair<string, IDiagnosticsComponent> element in components)
                    subcomponents.Add(element.Key, element.Value);
        }

        private IDictionary<string, IDiagnosticsComponent> subcomponents;

        #region IDiagnosticsComponent Members

        ComponentClass IDiagnosticsComponent.Class
        {
            get { return ComponentClass.Container; }
        }

        bool IDiagnosticsComponent.Enabled
        {
            get 
            {
                foreach (IDiagnosticsComponent component in subcomponents.Values)
                    if (component.Enabled)
                        return true;
                return false; 
            }

            set 
            {
                foreach (IDiagnosticsComponent component in subcomponents.Values)
                    component.Enabled = value;
            } 
        }

        void IDiagnosticsComponent.ResetComponent()
        {
            foreach (IDiagnosticsComponent component in subcomponents.Values)
                component.ResetComponent();
        }

        #endregion

        #region IComponentContainer Members

        IEnumerable<string> IComponentContainer.Names
        {
            get { return subcomponents.Keys; }
        }

        IEnumerable<IDiagnosticsComponent> IComponentContainer.Subcomponents
        {
            get { return subcomponents.Values; }
        }

        IDiagnosticsComponent IComponentContainer.this[string name]
        {
            get { return subcomponents[name]; }
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,IDiagnosticsComponent>> Members

        IEnumerator<KeyValuePair<string, IDiagnosticsComponent>> IEnumerable<KeyValuePair<string, IDiagnosticsComponent>>.GetEnumerator()
        {
            return subcomponents.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return subcomponents.GetEnumerator();
        }

        #endregion

        #region IAttributeCollection Members

        IEnumerable<string> QS.TMS.Inspection.IAttributeCollection.AttributeNames
        {
            get { return subcomponents.Keys; }
        }

        QS.TMS.Inspection.IAttribute QS.TMS.Inspection.IAttributeCollection.this[string attributeName]
        {
            get { return new TMS.Inspection.ScalarAttribute(attributeName, subcomponents[attributeName]); }
        }

        #endregion

        #region IAttribute Members

        string QS.TMS.Inspection.IAttribute.Name
        {
            get { return "Collection"; }
        }

        QS.TMS.Inspection.AttributeClass QS.TMS.Inspection.IAttribute.AttributeClass
        {
            get { return QS.TMS.Inspection.AttributeClass.COLLECTION; }
        }

        #endregion
    }
*/ 
}
