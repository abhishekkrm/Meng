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
using System.Linq;
using System.Text;
using System.Diagnostics;
using GOTransport.Common;
using System.Threading;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Debugging;
using GOBaseLibrary.Common;

namespace GOTransport.Core
{
    /// <summary>
    /// select gossip messages to include based on biasing them on certain criteria (gravity)
    /// </summary>
    public class GravitationalContentSelector : IContentSelector
    {
        #region private fields

        private GroupObjectGraph groupObjectGraph;
        private GroupOverlapGraph groupOverlapGraph;
        private RumorBuffer rumorBuffer;
        private PlatformWorkerThreadHelper platformWorkerThreadHelper;
        private Prioritizer prioritizer;
        private DebuggingContext debuggingContext;

        #endregion

        #region constructors

        public GravitationalContentSelector(ref RumorBuffer _rumorBuffer,
                                            ref GroupObjectGraph _groupObjectGraph,
                                            ref GroupOverlapGraph _groupOverlapGraph,
                                            ref PlatformWorkerThreadHelper _platformWorkerThreadHelper,
                                            ref DebuggingContext _debbugingContext)
        {
            rumorBuffer = _rumorBuffer;
            groupObjectGraph = _groupObjectGraph;
            groupOverlapGraph = _groupOverlapGraph;
            platformWorkerThreadHelper = _platformWorkerThreadHelper;
            debuggingContext = _debbugingContext;
            prioritizer = new Prioritizer(ref groupObjectGraph, ref groupOverlapGraph, ref rumorBuffer, ref _debbugingContext);
        }

        #endregion

        #region IContentSelector Members

        IGossip IContentSelector.Select(IGraphElement destination)
        {
            try
            {
                IGossip selected;
                double sumUtility = 0;

                // List<IGossip> rumors = rumorBuffer.GetAll();
                PriorityQueue<IGossip, DateTime> rumors = rumorBuffer.GetAll();

                if (rumors.Count == 0)
                {
                    return null;
                }

                //return rumors.First();

                foreach (PriorityQueueItem<IGossip, DateTime> rumor in rumors)
                // for (int iter = 0; iter < rumors.Count; iter++)
                {
                    if (platformWorkerThreadHelper.IsSent(destination, rumor.Value)
                        || (rumor.Value as Rumor).HopCount == 0)

                    //if ((rumors[iter] as Rumor).HopCount == 0)
                    {
                        rumor.Value.Utility = 0;
                        //(rumors[iter] as Rumor).Utility = 0;
                    }
                    else
                    {
                        rumor.Value.Utility = prioritizer.utility(destination, rumor.Value);
                        sumUtility += rumor.Value.Utility;
                        //rumors[iter].Utility = prioritizer.utility(destination, rumors[iter]);
                        //sumUtility += rumors[iter].Utility;
                    }

                    //Trace.WriteLine(
                    //                    debuggingContext + "Rumor: " + rumors[iter].Id + " of [" + rumors.Count + "] rumors"
                    //                    + ", destination node: " + destination.GetID()
                    //                    + ", destination group: " + rumors[iter].DestinationGroup.GetID()
                    //                    + ", utility: " + rumors[iter].Utility);
                }

                return SelectContent(sumUtility, rumors, out selected);
            }
            catch (Exception e)
            {
                throw new UseCaseException("1400", "GravitationalContentSelector::Select " + e + ", " + e.StackTrace);
            }
        }
        
        #endregion

        #region private methods

        private static IGossip SelectContentContinuousSum(double _sumUtility, List<IGossip> _rumors, out IGossip _selected)
        // private static IGossip SelectContentContinuousSum(double _sumUtility, PriorityQueue<IGossip, DateTime> _rumors, out IGossip _selected)
        {
            try
            {
                int rand = new Random().Next(0, _rumors.Count);
                double total = 0;

                _selected = _rumors[rand] as Rumor;

                for (int i = 0; i < _rumors.Count; i++)
                {
                    total += _rumors[i].Utility / _sumUtility;
                    if (total > _selected.Utility)
                    {
                        return _rumors[i];
                    }
                }

                // still not found an element, just return at random
                return _rumors[rand];
            }
            catch (Exception e)
            {
                throw new UseCaseException("1420", "GravitationalContentSelector::Select " + e + ", " + e.StackTrace);
            }
        }

        private static IGossip SelectContent(double _sumUtility, PriorityQueue<IGossip, DateTime> _rumors, out IGossip _selected)
        {
            try
            {
                List<IGossip> proportionalRumors = new List<IGossip>();

                lock (_rumors)
                {
                    foreach (PriorityQueueItem<IGossip, DateTime> rumor in _rumors)
                    // foreach (Rumor rumor in _rumors)
                    {
                        double numberOfRepeatitions = Math.Ceiling((rumor.Value.Utility / _sumUtility) * 100);
                        // double numberOfRepeatitions = Math.Ceiling((rumor.Utility / _sumUtility) * 100);

                        for (int iter = 0; iter < numberOfRepeatitions; iter++)
                        {
                            // proportionalRumors.Add(rumor);
                            proportionalRumors.Add(rumor.Value);
                        }
                    }

                    if (proportionalRumors.Count == 0)
                    {
                        _selected = null;
                        return null;
                    }

                    int rand = new Random().Next(0, proportionalRumors.Count);

                    _selected = proportionalRumors.ElementAt(rand) as Rumor;
                    return _selected;
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("1440", "GravitationalContentSelector::SelectContent " + e + ", " + e.StackTrace);
            }
        }

        #endregion
    }
}
