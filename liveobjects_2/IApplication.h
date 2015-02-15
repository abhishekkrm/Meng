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

#include "IApplicationContext.h"

namespace QS
{
	namespace Fx
	{
		namespace Unmanaged
		{
			// NOTE: The application class, returned by "CreateApplication", must implement this interface.

			class __declspec(dllexport) IApplication abstract
			{
			public:

				// NOTE: These methods are called once upon initialization and to signal that the application should terminate.

				virtual void Initialize(IApplicationContext *pcontext) = 0;
				virtual void Cleanup() = 0;
			};

			// NOTE: The application DLL must contain a global function named "CreateApplication" with the following signature.

			typedef IApplication*(*ApplicationConstructor)();
		}
	}
}


// NOTE: The global function may be conveniently inserted into the DLL by using the following macro.

#define APPLICATION_CONSTRUCTOR(code) extern "C" { __declspec(dllexport) QS::Fx::Unmanaged::IApplication* CreateApplication() { code }; }
