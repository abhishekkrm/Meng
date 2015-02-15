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

namespace QS.Fx.Serialization
{
	/// <summary>
	/// This attribute is used to mark the class a serializable and associate a unique identifier with it. The attribute is used by the serializer
    /// that automatically scans the registered assemblies and registers all classes annotated with this attribute. A class that has not been 
    /// marked with this attribute is not automatically registered by the serializer and must be registered manually in order to be correctly 
    /// recognized during deserialization.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class ClassIDAttribute : System.Attribute
	{
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="classID">The unique class identifier given as a 16-bit integer.</param>
        public ClassIDAttribute(ushort classID)
        {
            this.classID = classID;
        }

        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="classID">The unique class identifier from the list of QuickSilver's predefined identifiers.</param>
		public ClassIDAttribute(QS.ClassID classID)
		{
			this.classID = (ushort) classID;
		}

		private ushort classID;

        /// <summary>
        /// The unique class identifier.
        /// </summary>
		public ushort ClassID
		{
			get { return classID; }
		}
	}
}
