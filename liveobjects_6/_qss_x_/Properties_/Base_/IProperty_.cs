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

namespace QS._qss_x_.Properties_.Base_
{
    public interface IProperty_
    {
        /// <summary>
        /// Index of the property in the list of properties maintained by a data flow component.
        /// </summary>
        QS.Fx.Base.Index _Index
        {
            get;
        }

        /// <summary>
        /// Unique global identifier of the property class that determines its type and behavior.
        /// </summary>
        QS.Fx.Base.Identifier _Identifier
        {
            get;
        }

        /// <summary>
        /// Humand-friendly name of the property.
        /// </summary>
        string _Name
        {
            get;
        }

        /// <summary>
        /// Describes the type of value represented by the property.
        /// </summary>
        string _Comment
        {
            get;
        }

        /// <summary>
        /// Specifies whether the value of the property has been assigned at least once.
        /// </summary>
        bool _IsDefined
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies whether the value of a dependent property is outdated and must be recalculated.
        /// </summary>
        bool _IsOutdated
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies whether the value of the property has been updated since the last time it has been retrieved.
        /// </summary>
        bool _IsUpdated
        {
            get;
            set;
        }

        /// <summary>
        /// The version of the value stored in this property.
        /// </summary>
        QS.Fx.Value.Classes.IPropertyVersion _Version
        {
            get;
            set;
        }

        /// <summary>
        /// The stored value.
        /// </summary>
        QS.Fx.Serialization.ISerializable _Value
        {
            get;
            set;
        }

        /// <summary>
        /// Attempts to update the property with the specified version and value, and specifies whether the value 
        /// has indeed been updated and changed.
        /// </summary>
        /// <param name="version">The new version.</param>
        /// <param name="value">The new value.</param>
        /// <returns></returns>
        bool _Update(QS.Fx.Value.Classes.IPropertyVersion version, QS.Fx.Serialization.ISerializable value);

/*
        QS.Fx.Base.ID _Identifier
        {
            get;
        }


        QS._qss_x_.Properties_.Base_.PropertyValue_ _Value
        {
            get;
            set;
        }

        void _Forward(IProperty_ _property);
        void _Refresh();
*/
    }
}
