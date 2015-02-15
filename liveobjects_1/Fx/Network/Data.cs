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

namespace QS.Fx.Network
{
    /// <summary>
    /// This structure represents a packet transmitted over the network. The packet can contain multiple contiguous memory segments.
    /// </summary>
    public struct Data 
    {
        /// <summary>
        /// Test the consistency of the structure metadata (for debugging).
        /// </summary>
        public void _CheckSize()
        {
            int actualsize = 0;
            foreach (QS.Fx.Base.Block segment in segments)
                actualsize += (int) segment.size;
            if (actualsize != size)
                throw new Exception("Bad data, actual size is " + actualsize.ToString() + ", but the recorded size is " + size.ToString() + ".");
        }

        /// <summary>
        /// Construct a packet from a list of contiguous memory segments if the total number of bytes transmitted is not known.
        /// </summary>
        /// <param name="segments">The list of memory segments to transmit.</param>
	    public Data(IList<QS.Fx.Base.Block> segments)
	    {
		    this.segments = segments;
		    this.size = 0;
		    foreach (QS.Fx.Base.Block segment in segments)
			    size += (int) segment.size;
	    }

        /// <summary>
        /// Construct a packet from a list of contiguous memory segments if the total number of bytes transmitted is known.
        /// </summary>
        /// <param name="segments">The list of memory segments to transmit.</param>
        /// <param name="size">The total number of bytes transmitted.</param>
	    public Data(IList<QS.Fx.Base.Block> segments, int size)
	    {
		    this.segments = segments;
		    this.size = size;
	    }

        /// <summary>
        /// The list of contiguous memory segments to transmit.
        /// </summary>
	    public IList<QS.Fx.Base.Block> Segments
	    {
		    get { return segments; }
	    }

        /// <summary>
        /// The total number of bytes transmitted.
        /// </summary>
	    public int Size
	    {
		    get { return size; } 
	    }

        private IList<QS.Fx.Base.Block> segments;
	    private int size;

        /// <summary>
        /// Clear the packet metadata.
        /// </summary>
        public void Clear()
        {
            segments = null;
            size = 0;
        }
    };
}
