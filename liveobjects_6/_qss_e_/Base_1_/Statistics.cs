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

namespace QS._qss_e_.Base_1_
{
    public static class Statistics
    {
        public static QS._core_c_.Components.Attribute StatisticsOf(QS.Fx.Inspection.IAttributeCollection attributeCollection)
        {
            QS._core_c_.Components.AttributeSet collectionAttribute = new QS._core_c_.Components.AttributeSet();
            foreach (string attributeName in attributeCollection.AttributeNames)
            {
                QS.Fx.Inspection.IAttribute attributeValue = attributeCollection[attributeName];
                if (attributeValue is QS._qss_e_.Base_1_.IStatisticsCollector)
                    collectionAttribute.Add(new QS._core_c_.Components.Attribute(attributeName,
                        new QS._core_c_.Components.AttributeSet(((QS._qss_e_.Base_1_.IStatisticsCollector)attributeValue).Statistics)));
            }
            return new QS._core_c_.Components.Attribute(attributeCollection.Name, collectionAttribute);
        }

        public static QS._core_c_.Components.Attribute StatisticsOf<K, C>(string name, IDictionary<K, C> dictionary)
        {
            QS._core_c_.Components.AttributeSet collectionAttribute = new QS._core_c_.Components.AttributeSet();
            foreach (KeyValuePair<K, C> element in dictionary)
            {
                if (element.Value is QS._qss_e_.Base_1_.IStatisticsCollector)
                    collectionAttribute.Add(new QS._core_c_.Components.Attribute(element.Key.ToString(),
                        new QS._core_c_.Components.AttributeSet(((QS._qss_e_.Base_1_.IStatisticsCollector) element.Value).Statistics)));
            }
            return new QS._core_c_.Components.Attribute(name, collectionAttribute);
        }
    }
}
