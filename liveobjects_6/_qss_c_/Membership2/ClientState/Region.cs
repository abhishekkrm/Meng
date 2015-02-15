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

namespace QS._qss_c_.Membership2.ClientState
{
    public abstract class Region<ViewClass> : QS.Fx.Inspection.Inspectable, IRegion, IDisposable where ViewClass : class, IRegionView
    {
        public Region(Base3_.RegionID regionID, Base3_.RegionSig regionSig, QS.Fx.Network.NetworkAddress multicastAddress)
        {
            this.regionID = regionID;
            this.regionSig = regionSig;
            this.multicastAddress = multicastAddress;
            this.currentView = default(ViewClass);

            this.viewCollection = new System.Collections.Generic.SortedDictionary<uint, ViewClass>();
			inspectable_viewCollectionProxy = new QS._qss_e_.Inspection_.DictionaryWrapper1<uint, ViewClass>("Views", viewCollection,
				new QS._qss_e_.Inspection_.DictionaryWrapper1<uint, ViewClass>.ConversionCallback(Helpers_.FromString.UInt32));
		}

		[QS.Fx.Base.Inspectable("RegionID", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected Base3_.RegionID regionID;
		[QS.Fx.Base.Inspectable("Signature", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected Base3_.RegionSig regionSig;
		[QS.Fx.Base.Inspectable("Multicast Address", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected QS.Fx.Network.NetworkAddress multicastAddress;
		[QS.Fx.Base.Inspectable("Current View", QS.Fx.Base.AttributeAccess.ReadOnly)]
        protected ViewClass currentView;

		[QS.Fx.Base.Inspectable("Views", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected QS._qss_e_.Inspection_.DictionaryWrapper1<uint, ViewClass> inspectable_viewCollectionProxy;
		protected System.Collections.Generic.IDictionary<uint, ViewClass> viewCollection;

		protected abstract void initialize();
        protected abstract void cleanup();
        protected abstract void flush(ViewClass flushedView,
            IEnumerable<QS._core_c_.Base3.InstanceID> removedAddresses, IEnumerable<QS._core_c_.Base3.InstanceID> unaffectedAddresses);
        protected abstract void crashed(ViewClass oldView, IEnumerable<QS._core_c_.Base3.InstanceID> removedAddresses); 

        public ViewClass this[uint seqno]
        {
            get { return viewCollection[seqno]; }
        }

        public ViewClass CurrentView
        {
            get { return currentView; }
        }

        #region IRegion Members

        public Base3_.RegionID ID
        {
            get { return regionID; }
        }

        public Base3_.RegionSig Signature
        {
            get { return regionSig; }
        }

        public QS.Fx.Network.NetworkAddress Address
        {
            get { return multicastAddress; }
        }

        IRegionView IRegion.this[uint seqno]
        {
            get { return viewCollection[seqno]; }
        }

        IRegionView IRegion.CurrentView
        {
            get { return currentView; }
        }

        public void InstallView(IRegionView view)
        {
            if (!(view is ViewClass))
                throw new Exception("Attempted to install a view of incompatible type.");

            ViewClass previousView = currentView;
            currentView = (ViewClass)view;
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

        private static void CalculateDiff(IRegionView oldView, IRegionView newView,
            out IEnumerable<QS._core_c_.Base3.InstanceID> added, out IEnumerable<QS._core_c_.Base3.InstanceID> removed, out IEnumerable<QS._core_c_.Base3.InstanceID> same)
        {
            System.Collections.Generic.ICollection<QS._core_c_.Base3.InstanceID> unaffectedAddresses =
                new System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>();
            System.Collections.Generic.ICollection<QS._core_c_.Base3.InstanceID> removedAddresses =
                new System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>();
            System.Collections.Generic.ICollection<QS._core_c_.Base3.InstanceID> introducedAddresses =
                new System.Collections.ObjectModel.Collection<QS._core_c_.Base3.InstanceID>();

            foreach (QS._core_c_.Base3.InstanceID address in oldView.Members)
                removedAddresses.Add(address);

            foreach (QS._core_c_.Base3.InstanceID address in newView.Members)
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

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            cleanup();
        }

        #endregion
    }
}
