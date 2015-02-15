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

namespace QS._qss_e_.Data_
{
    public static class Filtering
    {        
        public static QS._core_e_.Data.IDataSet Filter(QS._core_e_.Data.IDataSet data, string filters)
        {
            if (data == null)
                throw new Exception("The provided data set is NULL.");

            foreach (string filter in filters.Split(','))
            {
                if (!filter.Equals(string.Empty))
                {
                    Type type = data.GetType();

                    PropertyInfo propertyInfo = type.GetProperty(filter,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (propertyInfo == null)
                        throw new Exception("No filtering property named \"" + filter +
                            "\" has been defined for data set of type \"" + type.ToString() + "\".");

                    data = (QS._core_e_.Data.IDataSet)propertyInfo.GetValue(data, null);
                }
            }

            return data;
        }

        public static IDictionary<string, string> FiltersOf(QS._core_e_.Data.IDataSet data)
        {
            if (data == null)
                throw new Exception("The provided data set is NULL.");
            Type type = data.GetType();

            Dictionary<string, string> filters = new Dictionary<string, string>();
            foreach (PropertyInfo propertyInfo in type.GetProperties(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object[] attribs = propertyInfo.GetCustomAttributes(typeof(QS._core_e_.Data.DataSourceAttribute), true);
                if (attribs.Length > 0)
                    filters.Add(propertyInfo.Name, ((QS._core_e_.Data.DataSourceAttribute)attribs[0]).Name);
            }

            return filters;
        }
    }
}
