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
using System.Diagnostics;
using System.Net;

namespace QS._qss_c_.Senders_1_
{
	/// <summary>
	/// This sender uses notifications from GMS to automatically create IP multicast groups
	/// and translates group target addresses into IP multicast addresses, if available, or
	/// simply unicasts using the underlying sender if not available.
	/// </summary>

/*	
	public class DirectIPMCSender : Base.ISender // , VS2.IViewChangeConsumer
	{
		public DirectIPMCSender(Components.IRequestSerializer requestSerializer, 
			Base.ISender underlyingSimpleSender, Devices.IMulticastingDevice multicastingDevice, 
			uint anticipatedNumberOfMulticastGroups)
		{
			Base.Serializer.Get.register(ClassID.DirectIPMCSender_OurMessage, 
				new Base.CreateSerializable(OurMessage.createSerializable));

			this.requestSerializer = requestSerializer;
			this.underlyingSimpleSender = underlyingSimpleSender;
			this.multicastingDevice = multicastingDevice;

			mappings = new Collections.Hashtable(anticipatedNumberOfMulticastGroups);	
		}

		private const uint localObjectID = (uint) ReservedObjectID.DirectIPMCSender;

		private class Mapping
		{
			public Mapping(Base.ViewAddress viewAddress)
			{
				this.viewAddress = viewAddress;				
				this.status = Status.REQUESTED;
				this.temporarilyCachedView = null;
			}

			public Base.ViewAddress viewAddress;
			public Status status;
			public GMS.IView temporarilyCachedView;

			public IPAddress multicastGroupAddress = IPAddress.Loopback;
			public uint multicastGroupPortNo = 0;

			public enum Status
			{
				REQUESTED, TOPREPARE, PREPARING, CONFIRMED 
			}
		}

		private Components.IRequestSerializer requestSerializer;
		private Base.ISender underlyingSimpleSender;
		private Devices.IMulticastingDevice multicastingDevice;
		private Collections.Hashtable mappings;

		private void establishANewMapping(Mapping mapping)
		{
			Debug.Assert(false, "no new mappings for now");

			// first decide on which group number, which port number
			// then create a local group
			// confirm to others, so that they know, too
			// then gather confirmations, change status to CONFIRMED
		}

		private void asynchronous_createOrRemoveMapping(Components.IAsynchronousRequest asynchronousRequest)
		{
			OurRequest request = (OurRequest) asynchronousRequest;
			Base.ViewAddress viewAddress = new Base.ViewAddress(request.gid, request.view.SeqNo);

			switch (request.type)
			{
				case OurRequest.Type.CREATE_MAPPING:
				{
					Mapping mapping = null;
					bool establish_mapping;

					lock (mappings)
					{
						Collections.IDictionaryEntry dic_en = mappings.lookupOrCreate(viewAddress);
						if (dic_en.Value == null)
						{
							dic_en.Value = new Mapping(viewAddress);
						}
						mapping = (Mapping) dic_en.Value;									
						
						establish_mapping = mapping.status == Mapping.Status.REQUESTED 
							|| mapping.status == Mapping.Status.TOPREPARE;						

						if (establish_mapping)
						{
							mapping.status = Mapping.Status.PREPARING;
							mapping.temporarilyCachedView = request.view;
						}
					}

					if (establish_mapping)
					{
						this.establishANewMapping(mapping);
					}
				}
				break;

				case OurRequest.Type.REMOVE_MAPPING:
				{
					lock (mappings)
					{
						Collections.IDictionaryEntry dic_en = mappings.remove(viewAddress);
					}
				}
				break;
			}
		}

		private class OurMessage : Base.IMessage
		{
			public static Base.ISerializable createSerializable()
			{
				return new OurMessage();
			}

			private OurMessage()
			{
			}

			public OurMessage(uint senderLOID, GMS.GroupId groupID, uint viewSeqNo, Base.IMessage message)
			{
				this.senderLOID = senderLOID;
				this.groupID = groupID;
				this.viewSeqNo = viewSeqNo;
				this.message = message;
			}

			public uint senderLOID;
			public GMS.GroupId groupID;
			public uint viewSeqNo;
			public Base.IMessage message;

			#region ISerializable Members

			public ClassID ClassIDAsSerializable
			{
				get
				{
					return ClassID.DirectIPMCSender_OurMessage;
				}
			}

			public void save(System.IO.Stream memoryStream)
			{
				byte[] buffer;
				buffer = System.BitConverter.GetBytes(senderLOID);
				memoryStream.Write(buffer, 0, buffer.Length);
				buffer = System.BitConverter.GetBytes(viewSeqNo);
				memoryStream.Write(buffer, 0, buffer.Length);
				buffer = System.BitConverter.GetBytes((ushort) message.ClassIDAsSerializable);
				memoryStream.Write(buffer, 0, buffer.Length);				
				groupID.save(memoryStream);
				message.save(memoryStream);
			}

			public void load(System.IO.Stream memoryStream)
			{
				byte[] buffer = new byte[10];
				memoryStream.Read(buffer, 0, 10);
				senderLOID = System.BitConverter.ToUInt32(buffer, 0);
				viewSeqNo = System.BitConverter.ToUInt32(buffer, 4);
				ushort classID = System.BitConverter.ToUInt16(buffer, 8);
				groupID.load(memoryStream);
				message = (Base.IMessage) Base.Serializer.Get.createObject((ClassID) classID);
				message.load(memoryStream); 
			}

			#endregion
		}		

		#region ISender Members

		public Base.IMessageReference send(Base.IClient theSender, 
			Base.IAddress destinationAddress, Base.IMessage message, Base.SendCallback sendCallback)
		{
			if (destinationAddress is Base.ViewAddress)
			{
				Base.ViewAddress viewAddress = (Base.ViewAddress) destinationAddress;

				Mapping.Status mappingStatus;
				IPAddress multicastGroupAddress = IPAddress.Loopback;		
				uint multicastGroupPortNo = 0;		
				GMS.IView temporarilyCachedView = null;		
				Mapping thisMapping = null;

				lock (mappings)
				{
					Collections.IDictionaryEntry dic_en = mappings.lookupOrCreate(viewAddress);
					if (dic_en.Value == null)
					{
						dic_en.Value = new Mapping(viewAddress);
					}

					thisMapping = (Mapping) dic_en.Value;

					mappingStatus = thisMapping.status;
					if (mappingStatus == Mapping.Status.CONFIRMED)
					{
						multicastGroupAddress = thisMapping.multicastGroupAddress;
						multicastGroupPortNo = thisMapping.multicastGroupPortNo;
					}
					else
					{
						temporarilyCachedView = thisMapping.temporarilyCachedView;

						if (mappingStatus == Mapping.Status.REQUESTED)
							thisMapping.status = Mapping.Status.TOPREPARE;
					}
				}

				if (mappingStatus == Mapping.Status.CONFIRMED)
				{
					return underlyingSimpleSender.send(theSender, 
						new Base.ObjectAddress(multicastGroupAddress, (int) multicastGroupPortNo, 
						localObjectID), message, sendCallback);
				}
				else
				{
					GMS.IView view = 
						(temporarilyCachedView != null) ? temporarilyCachedView : viewAddress.View;

					if (view != null)
					{
						Base.IMessageReference result = underlyingSimpleSender.send(
							theSender, new Base.ObjectAddressSet(view), message, sendCallback);									
						
						if (temporarilyCachedView == null)
						{
							lock (mappings)
							{
								thisMapping.temporarilyCachedView = view;
							}
						}

						if (mappingStatus == Mapping.Status.REQUESTED)
						{
							requestSerializer.enqueue(new OurRequest(
								OurRequest.Type.CREATE_MAPPING, viewAddress.GroupID, view, 
								new Components.AsynchronousRequestCallback(
								this.asynchronous_createOrRemoveMapping)));
						}

						return result;
					}
					else
					{
						throw new Exception("cannot resolve a view");
					}
				}
			}
			else
			{
				return underlyingSimpleSender.send(theSender, destinationAddress, message, sendCallback);
			}
		}

		#endregion

		private class OurRequest : Components.GenericAsynchronousRequest
		{
			public OurRequest(Type type, GMS.GroupId gid, GMS.IView view,
				Components.AsynchronousRequestCallback callback) : base(callback)
			{
				this.type = type;
				this.gid = gid;
				this.view = view;
			}

			public enum Type
			{
				CREATE_MAPPING, REMOVE_MAPPING
			}

			public Type type;
			public GMS.GroupId gid;
			public GMS.IView view;
		}

		#region IViewChangeConsumer Members

		public void viewAnnounced(QS.GMS.GroupId gid, QS.GMS.IView view)
		{
			requestSerializer.enqueue(new OurRequest(OurRequest.Type.CREATE_MAPPING, gid, view,
				new Components.AsynchronousRequestCallback(this.asynchronous_createOrRemoveMapping)));
		}

		public void viewToCleanUp(QS.GMS.GroupId gid, QS.GMS.IView view)
		{
			requestSerializer.enqueue(new OurRequest(OurRequest.Type.REMOVE_MAPPING, gid, null,
				new Components.AsynchronousRequestCallback(this.asynchronous_createOrRemoveMapping)));
		}

		#endregion
	}
*/	
}
