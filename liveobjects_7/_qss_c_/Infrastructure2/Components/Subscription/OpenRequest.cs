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

namespace QS._qss_c_.Infrastructure2.Components.Subscription
{
    public class OpenRequest
    {
        public OpenRequest(string group_name, Interfaces.Common.OpeningMode opening_mode, Interfaces.Common.AccessMode access_mode, 
            IEnumerable<Interfaces.Common.Attribute> group_properties, string type_name, IEnumerable<Interfaces.Common.Attribute> type_attributes,
            IEnumerable<Interfaces.Common.Attribute> custom_attributes)
        {
            this.group_name = group_name;
            this.opening_mode = opening_mode;
            this.access_mode = access_mode;
            this.group_properties = group_properties;
            this.type_name = type_name;
            this.type_attributes = type_attributes;
            this.custom_attributes = custom_attributes;
        }

        private string group_name, type_name;
        private Interfaces.Common.OpeningMode opening_mode;
        private Interfaces.Common.AccessMode access_mode;
        private IEnumerable<Interfaces.Common.Attribute> group_properties, type_attributes, custom_attributes;
        private Base3_.GroupID groupID;

        #region Accessors

        public Base3_.GroupID GroupID
        {
            get { return groupID; }
            set { groupID = value; }
        }

        public string GroupName
        {
            get { return group_name; }
        }

        public string TypeName
        {
            get { return type_name; }
        }

        public Interfaces.Common.OpeningMode OpeningMode
        {
            get { return opening_mode; }
        }

        public Interfaces.Common.AccessMode AccessMode
        {
            get { return access_mode; }
        }

        public IEnumerable<Interfaces.Common.Attribute> GroupProperties
        {
            get { return group_properties; }
        }

        public IEnumerable<Interfaces.Common.Attribute> TypeAttributes
        {
            get { return type_attributes; }
        }

        public IEnumerable<Interfaces.Common.Attribute> CustomAttributes
        {
            get { return custom_attributes; }
        }

        #endregion
    }
}
