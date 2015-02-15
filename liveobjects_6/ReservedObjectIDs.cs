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

namespace QS
{
	/// <summary>
	/// Summary description for ReservedObjectIDs.
	/// </summary>
	/// 

	public enum ReservedObjectID : uint
	{
        Nothing                                                                                                                                            =           0,

        // Mahesh's Objects

		ServerGMS																																		=		1000,
		ClientGMS, FailureDetectorClient, FailureDetectorServer,

		// Krzys's Objects

		Minimum_CMSHMS																							                                =		2000,
		
		BaseSender, 
        ReliableSender, 
        CryptographicSender, FBCASTSender, SimpleFlushingDevice,
		DirectIPMCSender, BufferingIPMCSender, CBCASTSender, ABCASTSender, UDPBasedRPCClient, 
		UDPBasedRPCServer, TCPBasedRPCClient, TCPBasedRPCServer, GenericVSMulticastingDevice, 
		SimpleMulticastingSender, 
		VSSender, N2FlushingDevice, UltrareliableSender, RoutingSender, UnorderedBufferingSender, 
		SequentialBufferingSender, CMSWrapper, Membership_Client, Membership_Server, 
		Scattering_RetransmittingScatterer_MessageChannel,
		Scattering_RetransmittingScatterer_AcknowledgementChannel, ViewController_FlushingChannel, 
		ViewController_MessageChannel,
		RPCProxy_RequestChannel, RPCProxy_ResponseChannel, AllocationClient, AllocationServer,
		IPMulticast_AllocationServer, AtomicScatterer, SimpleRS_Messages, SimpleRS_Acknowledgements,
        Membership_ClientAgent, Membership_CentralizedServer, ReliableSender_MessageChannel, 
		ReliableSender_AcknowledgementChannel,
        Interoperability_Remoting_ServerObject, RPC3_SimpleCaller_RequestChannel, 
		RPC3_SimpleCaller_ResponseChannel,
        FailureDetector_ServerAgent, Senders3_ReliableSender_MessageChannel, 
		Senders3_ReliableSender_AcknowledgementChannel, Multicasting3_SimpleSender, 
		Gossiping_SimpleController, Aggregation_AggregationAgent, Unwrapper, ChoppingSender, 
		RegionGossipingChannel, Multicasting3_SimpleSender1, Multicasting3_SimpleSender2,
		Multicasting3_SimpleSender3, Aggregation2_AggregationAgent, Aggregation3_AggregationAgent,
		CompressingSender, Aggregation4_AggregationController1, Aggregation4_Agent,
		Aggregation4_AggregationController2, 
		Senders3_ReliableSender2_MessageChannel, Senders3_ReliableSender2_CommandChannel,
		Senders5_ReliableSink_MessageChannel, Senders5_ReliableSink_CommandChannel,
		Senders5_ReliableSink_AcknowledgementChannel, Senders5_BufferingSink, FailureDetection_Centralized_Server,
        Senders3_ReliableSender_MessageChannel_2, Senders3_ReliableSender_AcknowledgementChannel_2,
        Senders6_InstanceReceiver, Senders6_ReliableSender_MessageChannel, 
        Senders6_ReliableSender_AcknowledgementChannel, FailureDetection_Subscriber, Multicasting5_LoggingReceiver,
        Multicasting5_PlainReceiver, Gossiping2_RegionalAgent, Gossiping2_RegionalAgent_InterregionalChannel,
        Multicasting5_GossipingRRVS, Multicasting5_GossipingRRVS_AckChannel,
        Gossiping5_GossipingRRVS_MessageChannel,
        Rings4_RingRRVS_DataChannel, 
        Rings4_RingRRVS_AckChannel, 
        Rings4_RingRRVS_TokenChannel,
        Receivers4_RegionalController_AgentChannel,
        Receivers4_RegionalController_MessageChannel, 
        Receivers4_RegionalController_AckChannel,
        Rings6_ReceivingAgent1_InterPartitionChannel, 
        Rings6_ReceivingAgent1_IntraPartitionChannel,
        Rings6_ReceivingAgent1_ForwardingChannel,
        Rings6_ReceivingAgent1_RequestingChannel,
        Rings6_ReceivingAgent1_NakChannel,
        Rings6_ReceivingAgent1_AckChannel,
        Rings6_SenderController1_DataChannel,
        Time_Synchronization_SynchronizationAgent2, 
        Time_Synchronization_SynchronizationAgent2_Client,
        Time_Synchronization_SynchronizationAgent3,
        Receivers5_InstanceReceiver_MessageChannel,
        Receivers5_InstanceReceiver_AcknowledgementChannel,
        Membership3_GMS,
        Membership3_Client_ResponseChannel,
        Membership3_Client_NotificationChannel,
        Fx_Services_Hosting_MessageChannel,
        Fx_Services_Hosting_ResponseChannel,
        Batching_Unpacker,
        SimpleChoppingSender,
        Rings6_SenderController1_RetransmissionChannel,
        Multicasting7_DispatcherRV2,
        Framework2_Group,
        Fx_Machine_Components_Replica,
        Fx_Backbone_Node,
        Fx_Unmanaged_Engine,
        QsmChannel,

		CMSTest001																																		=		3000,
		CMSTest002, CMSTest003, 
		xTest_Test005,

		HostManagementObject																													=		4000,
		
		Maximum_CMSHMS																															=		9999,

        User_Min                                                                                                                                          =   10000,
        User_Max                                                                                                                                         =   UInt32.MaxValue
	}
}
