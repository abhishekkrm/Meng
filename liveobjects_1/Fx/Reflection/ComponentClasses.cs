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

namespace QS.Fx.Reflection
{
    public static class ComponentClasses
    {
        #region Constants

        public const string AppFileBacking = "C9420ABA41FB4a1fAABCFA978FE16355";
        public const string AppMulticast = "E9BEC4586D7B4971B28FFBFBE58700B8";
        public const string Aggregator = "66773BB2E9764f78B08AD321168DCDB8";
        public const string Arnie = "0D365D36996040B4A28AF6593EC2A356";
        public const string Box_3D = "43CA76B7A90346C4B713D43271A2EF62";
        public const string CCFactory = "8F1524575FD840e99E91AB01A1EB6476";
        public const string Centralized_CC = "E3A4A62EB93948CCA20B1CB459C06270";
        public const string Centralized_CC_ = "7D48592140AE4E64A1ED8B2E18CC6BB9";
        public const string Centralized_CC_SVR = "8E01EF279EC44A16A1414B7B8C1F6271";
        public const string CentralizedCommunicationChannel = "8A32984EC01F4F3C9A64BDB2F64E28F8";
        public const string CentralizedCommunicationChannelServer = "A18F4780477449BCB00730B363D847C6";
        public const string CentralizedCommunicationChannelServerUI = "E5D1255605CA4AD5A7A60B4613575792";
        public const string Channel_1 = "89BF6594F5884B6495F5CD78C5372FC6";
        public const string Channel_2 = "D0A2EBF3E2F5416B8224987327C3B105";
        public const string ChannelTester = "9DF66193B7E548f098FD42A2DFC707BE";
/*
        public const string Channel1 = "EA3E0C5FD05A4386B8A44ADB11D91259";
        public const string Channel1Controller = "31F90A091F624F1B8394980365DDA252";
*/
        public const string CheckpointSynchronizer = "824656C7A74A450EAE1E1552C64FAC39";
        public const string SecureUI_ = "91F400F345EC429E8F89431AE4B12421";
        public const string Color = "579CBBAB3C7A46E2BD5A91D8A5280FFB";
        public const string Color_UI = "369286FE08334A4BAF34D57BBFFC1D0A";
        public const string Components_ = "CAACC8A70AA64C32B2B4A229B23A6BC6";
        public const string Control_X = "F272B8A1807F461DAE957C802D58A8ED";
        public const string Controller = "CDA5BDE9C8E145A791B2E34773A22A14";
        public const string Coordinates_1 = "AA471AB3A6644F8A8EB55E72B4078CA6";
        public const string Coordinates_2 = "2D360778BB184A76A544E5740006A618";
        public const string Counter = "B080A94A653D494BB89C733DB38490F4";
        public const string DAClient = "69793BB3E31F491190715176C3559F5B";
        public const string DAProtocolStackManager = "18C8C1242AFA415eB3DD37450E2DC71E";
        public const string DataFlow = "80A2A56F902D4765829942C873BEF3A5";
        public const string DelegationChannel = "64B478832ECC41B1B2BD9F23E2CD0D68";
        public const string DelegationChannel_0 = "34BFC487AF954C7FAED21947F36EF364";
        public const string DelegationChannel_1 = "330B652832FE4ff98C3204F12F655703";
        public const string DelegationManager = "6DE13CA791A348bbB776719868626547";
        public const string Deserializer = "FAEC13B6015A4B83BFF3758BDCA1AA30";
        public const string Designer = "E14098CDE688422F8EB3D75E2A5B6951";
        public const string Designer_2 = "1384B13DE88B4D7D8D7A09F3B83693D7";
        public const string Discovery = "AE69CF2875364BDDAF35010EBED83318";
        public const string ObjectDesigner = "8BA035CBEEE641089C5D8A3C2FD1C74E";
        public const string Desktop = "570482D150074A5381C8F5FD8C348503";
        public const string DictionaryClient = "F3C67B3D76D94E3A9CF03D0782362D59";
        public const string Dot_3D = "099520AE9C824A3C90CB4394A07C964E";
        public const string ExternalLibrary = "5002F7730B3443D1AA20F1DF150C820B";
        public const string Factory_2 = "1F0423C2A2CB41748DD281E208540BD6";
        public const string FolderView = "0C4E4E5F6DB24C04910D1CBE473CBE24";
        public const string Group = "48FFA2291DC84359A24C17149B6177AA";
        public const string Image_2D = "88D82BF52EA1496696002257989DE3D3";
        public const string Image_3D = "045C396266C740E99A6FD70B1DCC1FD0";
        public const string Infrastructure = "CF4177CD93624B12B1197177AC0049F4";
        public const string InfrastructureUI = "20B0764337994F39A65BA9C582AAD591";
        public const string Invitation = "CE316AAEAC3B4194B6937CCAA17453C2";
        public const string InvitationUI = "89C081DC0D5C4D3182E8E92FE45ED5FE";
        public const string InteractiveUser = "EE1B6F046A534C8EA86083171C965C3A";
        public const string Library = "BFB405E1F3134A33AF093B0D4901318B";
        public const string Loader = "991815B1E97F4BEF811476F01270FDA8";
        public const string LocalCommunicationChannel = "4FF15AA1A8BD458BA3847691DF96D289";
        public const string LocalFile = "0F6280C168784136A591A5B4CB2DB197";
        public const string LocalFolder = "4A9D7A367CCF496A9A80891C48D5E773";
        public const string SharedFolder = "A23A3D5432FB40BAB87181FD0943299A";
        public const string Map = "BB4AC54C762E42F38315D8485EED2B13";
        public const string Membership = "CDCFC3B0D84D4E6BA22C5F337609AAA1";
        public const string MembershipChannel = "89539B824D2D49F39035934AE5B6C9F1";
        public const string CentralizedChannelOverAsbtractTransport_ = "5ECD30B4FDF24a81B1A791567893BB6F";
        public const string CentralizedChannelOverAsbtractTransport_2 = "5ECE40B4FDF24a81B1A791567893BB6F";
        public const string CentralizedChannelOverAsbtractTransportClient1_ = "59D92C9DD1934c70894012978D789C45";
        public const string CentralizedChannelOverAsbtractTransportClient_2 = "59D62C9DD1936c70894012978D789C46";
        public const string CentralizedChannelOverAsbtractTransportSwitchClient = "89D72C9ED1937c70994012978D789C46";
        public const string MulticastChannel = "22B987492B164910877120F9E85EE8A6";
        public const string MulticastClient = "F01AA89B005A4B119702B6A076816160";
        public const string Model_2D = "187E05305BC34527B2AACC0C788499BA";
        public const string Model_3D = "C69D58A635524737981E4C8AB9279EDD";
        public const string Movie_UI = "02A4E1A46C57470E95FF25CFF8A5B9AB";
        public const string Orderer = "B2BA1EF90BF043089701B5FB3FEAFCA3";
        public const string AggregationComponent = "2C1D2D7BD5EB4F0080FFD0F8E9C7D7B2";
        public const string PropertiesUI = "CB75EC4568B1484689B4062827DBCE4F";
        public const string Publisher = "2BCA5325645D4AD383BBA81EE4E86A0A";
        public const string QSM_1 = "0ED202A3F1BA4D03B114C93FC79CEF48";
        public const string QSM = "AE211F7CFA6745248164F21CA8AA0A76";
        public const string QsmController_Object_ = "F2AF7A9E947B4FBD93F1FC71EBC94E8E";
        public const string QuickSilver_ = "2A8D55BCC9EC4095B7E0196526DE79D0";
        public const string QuickSilver_C_ = "6B010A146B0A4A8FA4D1254792C7ED9A";
        public const string GMS = "E9E5F330885F4580BCA69FD57E32AF5B";
        public const string PermissiveAuthority = "562E06D0B3DD4DFAB5C73650ED40FA67";
        public const string Registry = "E8C9D312C27649F39812B8E99AA67CBC";
        public const string Reliable_ = "850579E008B1468aBCA2A9E997E2E091";
        public const string ReliableCommunicationChannel = "3233A844A6F24c6a8F14342F3F20E2C8";
        public const string RingAggregator = "911AE0A710BE4972A193FB6EE81D680F";
        public const string Ring_0_ = "ED5893FBF74E4DABAA3E878AE59C1528";
        public const string SecureTcpChannel = "7F14063FA2084763A5225E36C3984E14";
        public const string SecureTcpChannelController = "21303BB83BD041F5AB36034A3637CAD5";
        public const string Service = "0640CF7D3F654E17AE6C82BC71E7F87D";
        public const string Subscriber = "5D9569F39310453AAAC9787D2034C081";
        public const string Switch = "5F9564F39310453AAAC9787D2034C071";
        public const string SynchronizationChannel = "C3CC81E857784B41823C660E1F4D0CDE";
        public const string SynchronizationClient = "E87D7C8E07624D0582524F9EA81412DA";
        public const string Test1 = "D224D7779B474A22822A7E16F2609353";
        public const string Test2 = "6B85C43886F84AA2AC2BFE94F8FE283D";
        public const string TestDataFlowClient = "F796BA062907479f988A250B680A489A";
        public const string TestDataFlowClient2 = "049586752907479f988A250B680A489B";
        public const string Text = "E3EA16F4ECC84B3B998E01F3B78E9AC9";
        public const string TestTestChannel = "D3CA16F4ECC84B3B998E01F3B88E9AC8";
        public const string TcpTransport = "713FC6360CAC480AB4E717E7D498D40C";
        public const string TreeAggregator = "7C36E18744FF4E86A2E3CD9844CC73A0";
        public const string Tree_0_ = "A808976046F0484B858EC3B553A711F6";
        public const string UI = "3A917D03934D400D8836175B536EF461";
        public const string UI_Properties = "A756D81BEC0E4591B3B72FE8B783E778";
        public const string UIOnCheckpointedCommunicationChannel = "EFF56489658F44EC90C85346BAADC22D";
        public const string UnreliableTreeDisseminationChannel = "33691B3A753844079D70CC9B958ED80F";
        public const string UnreliableRingDisseminationChannel = "80AFF63400354E82921D2664E3E497E1";
        public const string LocalHierarchicalAgent = "87AEF8E88398417EA53305C29652239A";
        public const string UnreliableTransport = "BF27E90F0C82437DA1B6686ABE1EBFBD";
        public const string Uplink = "A521ED8478D74DDDA849FAEC4624BE53";
        public const string Uplink_ = "2F9D833CE34F4F2E916DB8BB9AAD263B";
        public const string UplinkController = "12D8EB6F01A44E6C88692691CB163003";
        public const string Viewfinder = "36A8784294CE45109D42BEFF888BE5A3";
        public const string Window = "C3073050408D40529B63DD5BD932319D";
        public const string Window_X = "0FF58C3C33E449B8B8F76060EBB469E7";
        public const string XmlChannel = "7EB42417EB864FF485F685025C447808";

/*
        public const string Component = "5657F0B1BD6C432FAD72031B481BED77";
        public const string Foo = "45114693ADE14300B4297411C9668FC2";
        public const string SampleControl = "AAEA5D55-1B42-4f2a-9016-EBEACF58C04C";
*/

        public const string Reader_1 = "1B79EE13E5604950AE3541037FA9B15A";
        public const string Encryptor_1 = "2EC5D45C0399408096DC61925B3066F3";
        public const string Decryptor_1 = "41E52C0836394766AF8E28A8FBD91FC9";
        public const string Writer_1 = "E82931960FB340BA841F077CAB480132";
        public const string Zeroes_1 = "E3B07F4F4DFB496E8F7DC10660000B38";
        public const string Terminator_1 = "D4377FFF88FE46438D6CFB5A77C9CAAA";
        public const string Objects_1 = "6D0FCF08044343E3821D6B2DB53B2B01";

        public const string Experiment_1 = "EF465183AE764E2C81687E0A949DDFE1";

        public const string Protocol = "E866DF19A6874BC7BD930F5D8711B21A";

        public const string ComponentClass_01 = "A66C09D530704FFDA754A9F4FA6A9FB8";
        public const string ComponentClass_02 = "47AEC6AB018D4125ACAD0756C9A35AAF";
        public const string ComponentClass_03 = "2C69EE2637AF409FB6BA0047CA1D8686";
        public const string ComponentClass_04 = "9B5C451D59B1457A8A8001157CE471D6";
        public const string ComponentClass_05 = "F0A4FBD66B9A4B4A94976B55FD42F808";
        public const string ComponentClass_06 = "DDC37CFA2B2041AA9C27A9A2C312C151";
        public const string ComponentClass_07 = "87E8AA73FF984F1DB4D68B2E7E6AA083";
        public const string ComponentClass_08 = "697AAFC389BB4522B36BB5CE982EA6EB";
        public const string ComponentClass_09 = "F56DF9A9B4CE4C1299D6CDE78D8F1693";
        public const string ComponentClass_10 = "A000595F070B40E9BDA51E791CC9D3DB";
        public const string ComponentClass_11 = "D08001156990442EAA60F546732FC981";
        public const string Ping = "ABF2C4AC90A54DF694D1D52DE4C24B47";
        public const string Pong = "2F59F082B15640B69BB04E11CC59DFF3";
        public const string MapReduce = "A4DB2BCF1EC84F45B735CAE3FDDA2C0B";

        public const string ObjectExplorer = "F8D52BE193BA44c494CDB70E425393B2";
        #endregion
    }
}
