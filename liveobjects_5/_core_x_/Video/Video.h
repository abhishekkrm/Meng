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

#include "../../_core_v_/PacketReader.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections::Generic;
using namespace System::Diagnostics;

namespace QS
{
	namespace _core_x_
	{
		namespace Video
		{
			public ref class Video :  public IVideo
			{
			public:

				Video(IntPtr handle, System::Drawing::Rectangle rectangle, VideoType videotype, __int64 videosize);
				~Video();

				#pragma region IVideo Members

				virtual property IntPtr Window
				{
					IntPtr get() = IVideo::Window::get;
				}

				virtual property System::Drawing::Rectangle Rectangle
				{
					System::Drawing::Rectangle get() = IVideo::Rectangle::get;
					void set(System::Drawing::Rectangle rectangle) = IVideo::Rectangle::set;
				}

				virtual property VideoType Type
				{
					VideoType get() = IVideo::Type::get;
				}

				virtual property __int64 Length
				{
					__int64 get() = IVideo::Length::get;
				}

				virtual property __int64 Position
				{
					__int64 get() = IVideo::Position::get;
				}

				virtual property __int64 MinBuffered
				{
					__int64 get() = IVideo::MinBuffered::get;
				}

				virtual property __int64 MaxBuffered
				{
					__int64 get() = IVideo::MaxBuffered::get;
				}

				virtual void Start() = IVideo::Start;
				virtual void Stop() = IVideo::Stop;

				virtual void Append(QS::Fx::Base::Block block) = IVideo::Append;

				#pragma endregion

			private:

				IntPtr handle;
				System::Drawing::Rectangle rectangle;
				VideoType videotype;
				__int64 videosize;
				CMediaType* pmt;
				ULARGE_INTEGER *puliSize;
				CPacketStream *pStream;
				CPacketReader *pReader;
				IFilterGraph *pFG;
				IVideoWindow *pVW;
				IMediaControl *pMC;
				IMediaEvent *pME;

				// __int32 videoid;

				// __int64 buffered, removed;
				// __int32 blockindex;
				System::Collections::Generic::List<QS::Fx::Base::Block>^ blocks;

				// HRESULT _PacketStreamRead(
				//	LONGLONG llPos, PBYTE pbBuffer, DWORD dwBytesToRead, LPDWORD pdwBytesRead);

				// static __int32 lastvideo;
				// static IDictionary<__int32, Video^>^ videos;
				// static HRESULT _PacketStreamReadCallback(
				//	DWORD videoid, LONGLONG llPos, PBYTE pbBuffer, DWORD dwBytesToRead, LPDWORD pdwBytesRead);
			};
		}
	}
}
