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

namespace QS.GUI.Components
{
    public partial class DataSetFilterSelector : UserControl, IDataSetFilterSelector
    {
        public DataSetFilterSelector()
        {
            InitializeComponent();
        }

        private QS._core_e_.Data.IDataSet dataSet, filteredDataSet;
        private FilteredData filterChain;
        private EventHandler filterChanged;

        public string[] PropertyNames
        {
            get 
            {
                Stack<string> reversed_names = new Stack<string>();
                for (FilteredData cc = filterChain; cc != null; cc = cc.parent)
                {
                    if (cc.propertyname != null)
                        reversed_names.Push(cc.propertyname);
                }

                List<string> names = new List<string>(reversed_names.Count);
                while (reversed_names.Count > 0)
                    names.Add(reversed_names.Pop());

                return names.ToArray();
            }
        }

        #region IDataSetFilterSelector Members

        private static bool filterSearchCriteria(System.Reflection.MemberInfo objMemberInfo, System.Object objSearch)
        {
            return // (objMemberInfo is System.Reflection.PropertyInfo) && 
                (objMemberInfo.GetCustomAttributes(typeof(QS._core_e_.Data.DataSourceAttribute), false).Length > 0); // &&
            // ((System.Reflection.PropertyInfo) objMemberInfo).PropertyType is QS.TMS.Data.IDataSet;
        }

        #region Classes FilteredData and FilterSelector

        private class FilteredData
        {
            public FilteredData(string name, QS._core_e_.Data.IDataSet dataSet, FilteredData parent, string propertyname)
            {
                this.name = name;
                this.dataSet = dataSet;
                this.parent = parent;
                this.propertyname = propertyname;
            }

            public string name, propertyname;
            public QS._core_e_.Data.IDataSet dataSet;
            public FilteredData parent;

            public override string  ToString()
            {
 	            return name;
            }
        }

        private class FilterSelector 
        {
            public FilterSelector(string name, QS._core_e_.Data.IDataSet dataSet, System.Reflection.PropertyInfo propertyInfo)
            {
                this.name = name;
                this.propertyInfo = propertyInfo;
                this.dataSet = dataSet;
            }

            public string name;
            public System.Reflection.PropertyInfo propertyInfo;
            public QS._core_e_.Data.IDataSet dataSet, filteredDataSet = null;

            public override string ToString()
            {
                return "+ " + name;
            }
        }

        #endregion

        event EventHandler IDataSetFilterSelector.FilterChanged
        {
            add { filterChanged += value; }
            remove { filterChanged -= value; }
        }

        QS._core_e_.Data.IDataSet IDataSetFilterSelector.SourceData
        {
            get { return dataSet; }
            set 
            {
                dataSet = value;
                ResetFilters();
                if (filterChanged != null)
                    filterChanged(this, null);
            }
        }

        private void ResetFilters()
        {
            filterChain = new FilteredData("Source DataSet", dataSet, null, null);
            filteredDataSet = dataSet;
            RedrawChain();
            listBox1.SelectedItem = filterChain;
        }

        private void RedrawChain()
        {
            listBox1.Items.Clear();
            Stack<FilteredData> reversedChain = new Stack<FilteredData>();
            for (FilteredData cc = filterChain; cc != null; cc = cc.parent)
                reversedChain.Push(cc);
            while (reversedChain.Count > 0)
                listBox1.Items.Add(reversedChain.Pop());

            if (filterChain.dataSet != null)
            {
                System.Type dataSetClass = filterChain.dataSet.GetType();

                System.Reflection.MemberInfo[] members = dataSetClass.FindMembers(
                    System.Reflection.MemberTypes.Property, System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty,
                    new System.Reflection.MemberFilter(filterSearchCriteria), null);

                foreach (System.Reflection.PropertyInfo propertyInfo in members)
                {
                    QS._core_e_.Data.DataSourceAttribute dataSetAttribute =
                        (QS._core_e_.Data.DataSourceAttribute)propertyInfo.GetCustomAttributes(
                        typeof(QS._core_e_.Data.DataSourceAttribute), false)[0];

                    listBox1.Items.Add(new FilterSelector(dataSetAttribute.Name, filterChain.dataSet, propertyInfo));
                }
            }
        }

        QS._core_e_.Data.IDataSet IDataSetFilterSelector.FilteredData
        {
            get { return filteredDataSet; }
        }

        #endregion

        #region Clicking 

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {       
            object o = listBox1.SelectedItem;
            if (o == null)
                ResetFilters();
            else if (o is FilteredData)
            {
                filteredDataSet = ((FilteredData)o).dataSet;
            }
            else if (o is FilterSelector)
            {
                FilterSelector selectedFilter = (FilterSelector) o;
                if (selectedFilter.filteredDataSet == null)
                    selectedFilter.filteredDataSet =
                        (QS._core_e_.Data.IDataSet)selectedFilter.propertyInfo.GetValue(selectedFilter.dataSet, null);
                filteredDataSet = selectedFilter.filteredDataSet;
            }
            else
                ResetFilters();

            if (filterChanged != null)
                filterChanged(this, null);            
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            object o = listBox1.SelectedItem;
            if (o is FilteredData)
            {
                filterChain = ((FilteredData)o);
                filteredDataSet = filterChain.dataSet;

                RedrawChain();

                listBox1.SelectedItem = filterChain;
            }
            else if (o is FilterSelector)
            {
                FilterSelector selectedFilter = (FilterSelector) o;
                if (selectedFilter.filteredDataSet == null)
                    selectedFilter.filteredDataSet =
                        (QS._core_e_.Data.IDataSet)selectedFilter.propertyInfo.GetValue(selectedFilter.dataSet, null);

                filterChain = new FilteredData(selectedFilter.name, selectedFilter.filteredDataSet, filterChain, selectedFilter.propertyInfo.Name);
                filteredDataSet = filterChain.dataSet;

                RedrawChain();

                listBox1.SelectedItem = filterChain;
            }
            else
                return;

            if (filterChanged != null)
                filterChanged(this, null);
        }

        #endregion
    }
}
