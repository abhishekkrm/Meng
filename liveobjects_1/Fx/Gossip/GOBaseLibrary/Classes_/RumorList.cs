/*

Copyright (c) 2004-2009 Deepak Nataraj. All rights reserved.

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
using System.Xml.Serialization;
using System.Diagnostics;
using System.Threading;

namespace GOBaseLibrary.Common
{
    /// <summary>
    /// represents a list of rumors
    /// </summary>
    [Serializable]
    public class RumorList
    {
        [XmlElement]
        public List<Rumor> rumorList;

        private int size;
        private int maxRumorListSize;
        private Node sourceNode;

        public int Size { get { return size; } }

        public RumorList()
        {
            rumorList = new List<Rumor>();
            size = 0;
            maxRumorListSize = 1024;
        }

        /// <summary>
        /// set the maximum allowed size (in bytes) of the list of rumors
        /// </summary>
        /// <param name="_size">maximum size in bytes</param>
        public void SetMaxSize(int _size)
        {
            maxRumorListSize = _size;
        }

        /// <summary>
        /// try to add a rumor to the rumor list
        /// </summary>
        /// <param name="rumor">rumor to add</param>
        /// <returns>true if the add was successful, false otherwise</returns>
        public bool TryAdd(Rumor rumor)
        {
            try
            {
                if (rumor == null)
                {
                    return false;
                }
                else if (size + rumor.Size > maxRumorListSize)
                {
                    //Trace.WriteLine("\r\n" + DateTime.Now
                    //                    + "[Thread: " + Thread.CurrentThread.ManagedThreadId + "]"
                    //                    + "Adding denied as size of rumor [" + rumor.Size + "] exceeds " + maxRumorListSize);

                    return false;
                }

                size += rumor.Size;
                rumor.SourceNode = sourceNode;
                rumor.SourceNode.Ip = Utils.GetLocalIPAddress();
                rumor.Age += (DateTime.Now.Ticks - rumor.ReceivedTS);
                rumorList.Add(rumor);

                //Trace.WriteLine("\r\n" + DateTime.Now
                //                        + "[Thread: " + Thread.CurrentThread.ManagedThreadId + "]"
                //                        + "Adding successful, rumor " + rumor.Id + " with size[" + rumor.Size + "], and limit was " + maxRumorListSize);

                return true;
            }
            catch (Exception e)
            {
                throw new UseCaseException("260", "RumorList::TryAdd " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// add a rumor verbatim
        /// </summary>
        /// <param name="rumor">rumor to add</param>
        public void AddRumorAsIs(Rumor rumor)
        {
            try
            {
                if (rumor == null)
                {
                    return;
                }

                size += rumor.Size;
                rumorList.Add(rumor);
            }
            catch (Exception e)
            {
                throw new UseCaseException("280", "RumorList::AddRumorAsIs " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// remove a rumor from the rumor list
        /// </summary>
        /// <param name="r">rumor to remove</param>
        public void Remove(Rumor r)
        {
            try
            {
                rumorList.Remove(r);
            }
            catch (Exception e)
            {
                throw new UseCaseException("300", "RumorList::RemoveRumor " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// get all the rumors in rumor list
        /// </summary>
        /// <returns>list of rumors</returns>
        public List<Rumor> GetAll()
        {
            return rumorList;
        }

        /// <summary>
        /// add a list of rumors to existing rumor list
        /// </summary>
        /// <param name="range">list of rumors to add</param>
        public void AddRange(List<Rumor> range)
        {
            try
            {
                rumorList.AddRange(range);
            }
            catch (Exception e)
            {
                throw new UseCaseException("320", "RumorList::AddRange " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// clear the current rumor list
        /// </summary>
        public void Clear()
        {
            size = 0;
            rumorList.Clear();
        }

        /// <summary>
        /// set source node for the rumor list
        /// </summary>
        /// <param name="currentNode">node to be set as source</param>
        public void SetSource(Node currentNode)
        {
            sourceNode = currentNode;
        }
    }
}
