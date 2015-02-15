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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Base3_
{
/*
    public class ObjectPool<T> : IObjectPool<T>  
    {
        public ObjectPool(CreateCallback<T> callback, int initialNumberOfSegments, int segmentSize)
        {
            this.callback = callback;
            segments = new System.Collections.Generic.List<Segment>(initialNumberOfSegments);
            firstfree = firstused = null;
            this.segmentSize = segmentSize;
        }

        private System.Collections.Generic.IList<Segment> segments;
        private CreateCallback<T> callback;
        private IAllocatedObject<T> firstfree, firstused;
        private int segmentSize;

        private void increaseCapacity()
        {
            Segment segment = new Segment(segmentSize);



        }

        private struct Segment
        {
            public Segment(int size)
            {
                elements = new ObjectWrapper[size];
            }

            public void linkTo(ref IAllocatedObject<T> firstfree)
            {
                for (int ind = 0; ind < elements.Length - 1; ind++)
                    elements[ind] = new ObjectWrapper(new Linkable<T>(default(T)));
            }

            public ObjectWrapper[] elements;
        }

        private struct ObjectWrapper : IAllocatedObject<T>
        {
            public ObjectWrapper(Linkable<T> linkableWrapper)
            {
                this.linkableWrapper = linkableWrapper;
            }

            public Linkable<T> linkableWrapper;
        
            #region IAllocatedObject<T> Members

            T  IAllocatedObject<T>.Object
            {
                get { return linkableWrapper.Object; }
            }

            void  IAllocatedObject<T>.release()
            {
 	            // ...........................................................
            }

            #endregion
        }

        #region IObjectPool<T> Members

        IAllocatedObject<T> IObjectPool<T>.AllocateObject
        {
            get 
            {  
 	            // ...........................................................
                return null;
            }
        }

        #endregion
    }
*/
}
