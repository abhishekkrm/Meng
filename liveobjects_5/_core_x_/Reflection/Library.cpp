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

#include "stdafx.h"
#include "Library.h"

using namespace System;
using namespace System::Collections::Generic;

namespace QS
{
	namespace _core_x_
	{
		namespace Reflection
		{
			bool Library::IsManaged(String^ filename)
			{
				bool ismanaged_ = false;

				pin_ptr<const wchar_t> filename_ = PtrToStringChars(filename);
				HANDLE file = CreateFile(filename_, 
					GENERIC_READ, FILE_SHARE_READ,NULL,OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);	
				if (file == INVALID_HANDLE_VALUE)
					throw gcnew Exception(L"CreateFile failed");
				HANDLE filemapping = CreateFileMapping(file, NULL, PAGE_READONLY, 0, 0, NULL);
				if (!filemapping)
					throw gcnew Exception(L"CreateFileMapping failed");
				BYTE* filebytes = (BYTE *) MapViewOfFile(filemapping, FILE_MAP_READ, 0, 0, 0);
				if (!filebytes)
					throw gcnew Exception(L"MapViewOfFile failed");

				IMAGE_DOS_HEADER *pdosheader = (IMAGE_DOS_HEADER *) filebytes;
				IMAGE_NT_HEADERS *pntheaders = (IMAGE_NT_HEADERS *)((BYTE *) pdosheader + pdosheader->e_lfanew);
				IMAGE_SECTION_HEADER* psectionheader = (IMAGE_SECTION_HEADER *)((BYTE *) pntheaders + sizeof(IMAGE_NT_HEADERS));
				if (pntheaders->Signature == IMAGE_NT_SIGNATURE)
				{
					DWORD dwnetheadertablelocation = 
						pntheaders->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR].VirtualAddress;
					if (dwnetheadertablelocation)
					{
						DWORD offset = 0;
						for (int j = 0; j < pntheaders->FileHeader.NumberOfSections; j++, psectionheader++)
						{
							DWORD cbmaxondisk = min(psectionheader->Misc.VirtualSize, psectionheader->SizeOfRawData);
							DWORD startsection = psectionheader->VirtualAddress;
							DWORD endsection = startsection + cbmaxondisk;
							if ((dwnetheadertablelocation >= startsection) && (dwnetheadertablelocation < endsection))
							{
								offset =  (psectionheader->PointerToRawData ) + (dwnetheadertablelocation - startsection);
								break;
							}				
						}
						IMAGE_COR20_HEADER *pnetheader = (IMAGE_COR20_HEADER *)((BYTE *) pdosheader + offset);
						if (pnetheader)
							ismanaged_ = true;
					}
				}

				UnmapViewOfFile(filebytes);
				CloseHandle(filemapping);
				CloseHandle(file);

				return ismanaged_;
			}
		}
	}
}
