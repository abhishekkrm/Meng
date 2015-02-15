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

#define OPTION_EnableMulticastPerGroup

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Membership2.ClientState
{
    public abstract class Group<ViewClass> : QS.Fx.Inspection.Inspectable, IGroup where ViewClass : class, IGroupView
    {
        public Group(Base3_.GroupID groupID
#if OPTION_EnableMulticastPerGroup            
            , QS.Fx.Network.NetworkAddress multicastAddress
#endif            
            )
        {
            this.groupID = groupID;
            this.currentView = default(ViewClass);
#if OPTION_EnableMulticastPerGroup            
            this.multicastAddress = multicastAddress;
#endif            

            this.viewCollection = new System.Collections.Generic.SortedDictionary<uint, ViewClass>();
			inspectable_viewCollectionProxy = new QS._qss_e_.Inspection_.DictionaryWrapper1<uint, ViewClass>("Views", viewCollection,
				new QS._qss_e_.Inspection_.DictionaryWrapper1<uint, ViewClass>.ConversionCallback(Helpers_.FromString.UInt32));
		}

		[QS.Fx.Base.Inspectable("GroupID", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected Base3_.GroupID groupID;

		[QS.Fx.Base.Inspectable("Current View", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected ViewClass currentView;

		[QS.Fx.Base.Inspectable("Views", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected QS._qss_e_.Inspection_.DictionaryWrapper1<uint, ViewClass> inspectable_viewCollectionProxy;
		protected System.Collections.Generic.IDictionary<uint, ViewClass> viewCollection;

        protected abstract void initialize();
        protected abstract void flush(ViewClass flushedView, 
            IEnumerable<QS._core_c_.Base3.InstanceID> removedAddresses, IEnumerable<QS._core_c_.Base3.InstanceID> unaffectedAddresses);
        protected abstract void crashed(ViewClass oldView, IEnumerable<QS._core_c_.Base3.InstanceID> removedAddresses); 

#if OPTION_EnableMulticastPerGroup            
        protected QS.Fx.Network.NetworkAddress multicastAddress;

        public QS.Fx.Network.NetworkAddress MulticastAddress
        {
            get { return multicastAddress; }
        }

        QS.Fx.Network.NetworkAddress IGroup.Address
        {
            get { return multicastAddress; }
        }

#endif            

        public ViewClass CurrentView
        {
            get { return currentView; }
        }

        public ViewClass this[uint seqno]
        {
            get { return viewCollection[seqno]; }
        }

        #region IGroup Members

        public Base3_.GroupID ID
        {
            get { return groupID; }
        }

        IGroupView IGroup.CurrentView
        {
            get { return currentView; }
        }

        IGroupView IGroup.this[uint seqno]
        {
            get { return viewCollection[seqno]; }
        }

        public void InstallView(IGroupView view)
        {
            if (!(view is ViewClass))
                throw new Exception("Attempted to install a view of incompatible type.");

            ViewClass previousView = currentView;
            currentView = (ViewClass) view;
            viewCollection[view.SeqNo] = currentView;

            if (previousView == null)
                initialize();
            else
            {
                IEnumerable<QS._core_c_.Base3.InstanceID> added, removed, same;
                CalculateDiff(previousView, currentView, out added, out removed, out same);

                flush(previousView, removed, same);

                foreach (ViewClass earlierView in viewCollection.Values)
                {
                    if (earlierView.SeqNo != currentView.SeqNo && earlierView.SeqNo != previousView.SeqNo)
                        crashed(earlierView, removed);
                }
            }
        }

        #endregion

        private static void CalculateDiff(IGroupView oldView, IGroupView newView, 
            out IEnumerable<QS._core_c_.Base3.InstanceID> added, out IEnumerable<QS._core_c_.Base3.InstanceID> removed, out IEnumerable<QS._core_c_.Base3.InstanceID> same)
        {
            System.Collections.Generic.ICollection<QS._core_c_.Base3.InstanceID> unaffectedAddresses = 
                new System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>();
            System.Collections.Generic.ICollection<QS._core_c_.Base3.InstanceID> removedAddresses = 
                new System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>();
            System.Collections.Generic.ICollection<QS._core_c_.Base3.InstanceID> introducedAddresses = 
                new System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>();

            foreach (IRegionView regionView in oldView.RegionViews)
                foreach (QS._core_c_.Base3.InstanceID address in regionView.Members)
                    removedAddresses.Add(address);

            foreach (IRegionView regionView in newView.RegionViews)
                foreach (QS._core_c_.Base3.InstanceID address in regionView.Members)
                {
                    if (removedAddresses.Remove(address))
                        unaffectedAddresses.Add(address);
                    else
                        introducedAddresses.Add(address);                
                }

            added = introducedAddresses;
            removed = removedAddresses;
            same = unaffectedAddresses;
        }
    }
}
