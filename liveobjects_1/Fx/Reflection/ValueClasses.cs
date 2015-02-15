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
    public static class ValueClasses
    {
        #region Constants

        public const string _system_bool = "C381859311354457BE946F29970EAF7A";
        public const string _system_byte = "57E11A1677AE45F08C91585DBA4A6ACF";
        public const string _system_char = "20A2213F58674FEC886B734A7E2E4CB9";
        public const string _system_short = "B395680336FB4F73A703744F9E4BE230";
        public const string _system_int = "0050FE8BDDED4177B412ECA584CDADEF";
        public const string _system_long = "2FA325AC734C45FB96D3F08E0EE4F328";
        public const string _system_string = "2FEB1E028342422CBD38CB074B957973";
        public const string _system_ushort = "C4990F01361649BD956C4DF2E4B34AD7";
        public const string _system_uint = "9AF56A2E76824981B3564DC97D8B3661";
        public const string _system_ulong = "54D8497E84B441B48B892D60E5B95311";
        public const string _system_void = "F07495FCC5EE4306B404B4DC0C16C2A8";
        public const string _system_float = "81C97327A0364A6C9847688AE070B476";
        public const string _system_double = "5AEF576893AA4F69B49A6B89F9EA206F";

        public const string _system_IEnumerable = "34F4573DAF9D4352BDBCA48847420DB9";
        public const string _system_IDictionary = "BADC799F52734E258AF98994E0D2BC07";
        public const string _system_IList = "E0440410A124468BA4A373FF15E5D524";
        public const string _system_ArraySegment = "7A0E1787143D46518E9D888C4D3F603C";
        public const string _system_Array = "B6A3CFFA688541e0BC6C21C8F0E7B9D0";
        public const string _system_ICollection = "198D23FACB194486994B392E41E049CB";


        public const string _system_bools = "444C29481DF448C2ABE9D4EA81B1AE38";
        public const string _system_bytes = "98454EAB1F5146E2ADD1C9308F14C00C";
        public const string _system_chars = "40A8921BA0AF4328AD553277EE029A71";
        public const string _system_shorts = "1F1677C4EF22493CA46A7BE2AAB677C9";
        public const string _system_ints = "4AF20F88616740C395276B12059B832E";
        public const string _system_longs = "1D2BA86B19D045FFBEB855D3E38F268D";
        public const string _system_strings = "CD964F8C66384D289E3FB87881F9343B";
        public const string _system_ushorts = "25025445B7434B8986B40C9469C756AA";
        public const string _system_uints = "0134755AED784B4296EB6D27CF3CFF21";
        public const string _system_ulongs = "D0BCD37A28A045068CF5C01BE707590D";
        public const string _system_floats = "65BE0492C1CA42D8BFA297E3C994E884";
        public const string _system_doubles = "492A5260219948179E12BDBCE04C9FE3";
        public const string _system_object = "5DDE0B6759974514B6F040D335664029";

        public const string _Endpoint = "648A07ADE3494FF6B72127CE581FE5EE";
        public const string _Interface = "EB81015761F84E06A287132EBD14418B";
        public const string _Object = "58B37C32B8E549C5BC6EE4A0F5C1467E";

        public const string _ValueClass = "50450357B6344E06A310C63E38740482";
        public const string _InterfaceClass = "C4A3A82E75C6458BBA74C070A8D71A6A";
        public const string _EndpointClass = "855F3141A681400FA004147D396A5EF0";
        public const string _ObjectClass = "9D606B7185694D66A6143E7FE0EB857F";
        public const string _ComponentClass = "113DCD55160540E1874E5DF8068C1863";
        public const string _InterfaceConstraintClass = "029BD6C5D9C54A1795A3E28A1C25CC8B";
        public const string _EndpointConstraintClass = "9D9E3E791F5A45298A515DDFF7AF273C";
        public const string _ObjectConstraintClass = "C10DFD1E5F7046B8B550393BFF2198F9";

        public const string _XmlObject = "75C5F8CBAC1F447697C551272B15FA3C";
        public const string _XmlValueClass = "B626AEAE561F497686633267F1244A9D";
        public const string _XmlInterfaceClass = "2EF623ED38274277A0BBF19C7BDCA6D9";
        public const string _XmlEndpointClass = "AF4D8D53F9A448EAAA54128044AFAF3B";
        public const string _XmlObjectClass = "275EC36E67DC43D2ADD72230DEA98E6F";
        public const string _XmlObjects = "E4583B1F10B6435A8A200C4D9781BB4B";

        public const string Block = "A66104B7F5534439A109E2AC9366E42C";
        public const string ContentRef = "C92F7CFE120C4932BCE2B0606FFB55AE";
        public const string Color = "2E5E63EE344A4F709DA9766CDC73CEF2";
        public const string Configuration = "C587F49605864DD5B72F62E568F9E3FD";
        public const string Coordinates = "5CD78703A5504945BE5867CDFF4771E7";
        public const string IConfiguration = "507C701F8C6E4BEEAF90C47644853784";
        public const string ISerializable = "148B8C6B886A49029CD81610CB75EEE6";
        public const string Text = "982130E40A294E3CAD5441841E87941E";
        public const string Frame = "137E793B3B6345E689B0701C2F9019E6";
        public const string _s_desktop_operation = "8E058A84A89546389352AD2F551696CD";
        public const string _s_desktop_state = "BE02D391402D436EB62473CC4E04E7F4";
        public const string _s_folder_operation = "B0592A046E5E47E69A1AC39E412B7AD0";
        public const string _s_folder_state = "5B1608BEE89B434F967E92EDADF0EEB2";
        public const string _s_map_operation = "B14D312B1327435C8E24875B5B764FFC";
        public const string _s_map_state = "1DE0DB0CB0424EF6A98FBE65C8596083";
        public const string _s_centralized_cc_msg = "8AF1AFAF193F41668B4E9915DD4CEEE6";
        public const string _s_channelobject = "87C98E8628884C8098B810933BC7C095";
        public const string _s_outgoingobject = "1CFD7E792B8D470DA5796EB33EDB6675";
        public const string _s_incomingobject = "926F00C269B24B258C90D3EB23790CDF";
        public const string _s_qsmcontrol = "9B6C433D6AC84BA6BF9FD34DF7FC6548";

        //

        public const string _s_id = "3C36CBFA0A8240129B2F45793C4389E9";
        
        public const string _s_address = "4BD58FAE1AEB4537B42CE0C2640528F9";
        public const string _s_name = "F864A06B836844EDBA7D392899627DED";
        public const string _s_identifier = "24D8914967A443BE93FD68B4F3CE82A5";
        public const string _s_incarnation = "FF88E580C97A40B99DA71B6FA38F1354";
        public const string _s_index = "C9CE9867BE264A0098A688B0CB865053";
        public const string _s_midentifier = "DB7BDCCB995740e28515C4324BCDADBF";

        public const string UINT32 = "5EF445665D9B485FB4CFE24A979B3499";
        public const string UINT64 = "7ACA0A0EF8E8442784CA7921D11B2CC8";
        public const string UINT128 = "5A832D09E9224EA3976E0AD1E5283E33";

        public const string XY = "BD9358FBE616491FA9DC5C2E3821C24A";

        public const string _s_imember_1 = "3444F396D81845E5B59363D6FD2F0068";
        public const string _s_imember_2 = "D7BD9C74F130404290EA3FE7849391F4";
        public const string _s_imembership = "5D969E8C2EE24E0584118E4FA40D9690";
        public const string _s_imembershipchange = "2DB9C70ACF9D498F8CFFD9A03168F280";

        public const string _s_iversioned = "42C4694E35DC4786B365B585AD67130D";
        public const string _s_iset = "8380BA31F79041C7870C322E9D5639D1";
        public const string _s_isetchange = "CE38EF78B2094A10984E9E1424449D7B";

        public const string _s_member_1 = "14705AB201EF462D951467BFC5C104DC";
        public const string _s_member_2 = "DFB7E0F7F084433488136BBDAFFD46E2";
        public const string _s_membership = "7FBB290BE29342829241CE7D4AC11662";
        public const string _s_versioned = "E82D7031036C4488BA0247F60C9F75A0";
        public const string _s_set = "05F573CB0D424EF39169CF28E601FF2A";
        public const string _s_setchange = "9B35A28CD7164D609A86739551B6A5E6";

        public const string _s_property_control = "A111D47DECB14FE49FEFE57E85268DB3";
        public const string _s_property_message = "A1543900025C4E9D89059C343752E304";

        public const string _i_propertyvalue = "0989BCEA8FD640B8839425F4A3C9C1A6";
        public const string _i_propertyvalues = "5625CE21E547465BB61A1D71F76B8F3A";
        public const string _i_propertyversion = "62B5071869114B55B13A3194FDC72BB4";

        public const string _s_propertyvalue = "E4339B9A373B4D8A9CA3818E26F1F8DD";
        public const string _s_propertyvalues = "6964BDFFBC6F42608EF7AD7E622F81AD";
        public const string _s_propertyversion = "8A96762C3D28442492A9F84046FDD193";

        public const string _s_record = "6D592E6B7CD346048E9837CB253CA77B";

        public const string _s_multicastclient_checkpoint = "B98F4BC2091348AE89BFA05FBC4EDD33";
        public const string _s_multicastclient_message = "D85EB9B8938B4EC3B27FA95168373F64";

        public const string _c_property = "9EC817D47B694A9799CBA1872011245B";
        public const string _c_properties = "925D792334414DF1BE6B7197EB72DF1A";
        public const string _c_uset = "7E37A9D7CDCF4BBB839E8F58F47EF959";
        public const string _c_uint = "36AFF46D303849C28C4D7AFB9EBED55D";

        public const string _propertiescontrol = "BCE1EC60D85D435C82FC633FECA2AC83";
        public const string _propertycontrol = "E40903AA15EC40229EF0DBA8EB671863";

        public const string ValueClass_01 = "C31EDA748EA14C06B32B42A1B69A0591";
        public const string ValueClass_02 = "16780564BA69486584E8D58AE9F523DD";

        public const string ExplorableMetadata = "6EF06561B0AE41e9ADA30859AD837551";

        #endregion
    }
}
