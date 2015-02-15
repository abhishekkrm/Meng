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

#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections::Generic;
using namespace System::Diagnostics;
using namespace System::ServiceModel;

namespace QS
{
	namespace _core_x_
	{
		namespace Uplink_old_
		{
/*
			[QS::Fx::Reflection::ComponentClassAttribute(QS::Fx::Reflection::ComponentClasses::UplinkController, "Uplink Controller",
				"")]
			ref class Controller sealed 
				: public QS::Fx::Object::Classes::IFactory<QS::Fx::Object::Classes::IObject>, 
				  public QS::Fx::Interface::Classes::IFactory<QS::Fx::Object::Classes::IObject> 
			{
			public:

				#pragma region Constructor and destructor

				Controller([QS::Fx::Reflection::ParameterAttribute("capacity", QS::Fx::Reflection::ParameterClass::Value)] int _capacity);
				~Controller();

				#pragma endregion

				#pragma region IFactory<IObject> Members

				virtual property QS::Fx::Endpoint::Classes::IInterface<QS::Fx::Interface::Classes::IFactory<QS::Fx::Object::Classes::IObject>>^
					QS.Fx.Object.Classes.IFactory<QS.Fx.Object.Classes.IObject>.Endpoint
				{
					get { return this._endpoint; }
				}

				#pragma endregion

			private:

				QS::Fx::Endpoint::Interface<QS::Fx::Interface::Classes::IFactory<QS::Fx::Object::Classes::IObject>>^ _endpoint;
				int _capacity;
			};
*/
		}
	}
}
