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

namespace QS.Fx.Printing
{
    /// <summary>
    /// This attribute is used to annotate a given class or class member as printable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property)]
    public class PrintableAttribute : System.Attribute
    {
        /// <summary>
        /// The default printing style.
        /// </summary>
        public const PrintingStyle DefaultStyle = PrintingStyle.Undefined;

        /// <summary>
        /// Marks the class or member as printable, using the default name, style and memebr selection scheme.
        /// </summary>
        public PrintableAttribute() : this(null, DefaultStyle)
        {
        }

        /// <summary>
        /// Marks the class or member as printable, with the explicitly specified display name.
        /// </summary>
        /// <param name="name">The display name.</param>
        public PrintableAttribute(string name) : this(name, DefaultStyle)
        {
        }

        /// <summary>
        /// Marks the class or member as printable, with the explicitly specified printing style.
        /// </summary>
        /// <param name="style">The printing style to use.</param>
        public PrintableAttribute(PrintingStyle style) : this(null, style)
        {
        }

        /// <summary>
        /// Marks the class or member as printable, with the explicitly specified name and printing style.
        /// </summary>
        /// <param name="name">The display name.</param>
        /// <param name="style">The printing style to use.</param>
        public PrintableAttribute(string name, PrintingStyle style) : this(name, style, SelectionOption.Implicit)
        {
        }

        /// <summary>
        /// Marks the class or member as printable, with the explicitly specified printing style and member selection scheme.
        /// </summary>
        /// <param name="style">The printing style to use.</param>
        /// <param name="selectionOption">The member selection scheme to use.</param>
        public PrintableAttribute(PrintingStyle style, SelectionOption selectionOption) : this(null, style, selectionOption)
        {
        }

        /// <summary>
        /// Marks the class or member as printable, with the explicitly specified name, printing style and member selection scheme.
        /// </summary>
        /// <param name="name">The display name.</param>
        /// <param name="style">The printing style to use.</param>
        /// <param name="selectionOption">The member selection scheme to use.</param>
        public PrintableAttribute(string name, PrintingStyle style, SelectionOption selectionOption)
        {
            this.name = name;
            this.style = style;
            this.selectionOption = selectionOption;
        }

        private string name;
        private PrintingStyle style;
        private SelectionOption selectionOption;

        /// <summary>
        /// The display name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// The printing style to use.
        /// </summary>
        public PrintingStyle Style
        {
            get { return style; }
        }

        /// <summary>
        /// The member selection scheme to use.
        /// </summary>
        public SelectionOption SelectionOption
        {
            get { return selectionOption; }
        }
    }
}
