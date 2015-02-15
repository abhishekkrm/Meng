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

#include "StdAfx.h"
#include "Video.h"

#include "../../_core_v_/PacketReader.h"

using namespace System;
using namespace System::Threading;
using namespace System::Runtime::InteropServices;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Collections::Generic;

namespace QS
{
	namespace _core_x_
	{
		namespace Video
		{
			#pragma region Constructor

			Video::Video(IntPtr handle, System::Drawing::Rectangle rectangle, VideoType videotype, __int64 videosize)
			{
//				Monitor::Enter(QS::_core_x_::Video::Video::typeid);
//				try
//				{
//					if (videos == nullptr)
//						videos = gcnew Dictionary<__int32, Video^>();
//					videoid = ++lastvideo;
//					videos->Add(videoid, this);
//				}
//				finally
//				{
//					Monitor::Exit(QS::_core_x_::Video::Video::typeid);
//				}

				this->handle = handle;
				this->rectangle = rectangle;
				this->videotype = videotype;
				this->videosize = videosize;

//				this->buffered = 0;
//				this->removed = 0;
//				this->blockindex = 0;
				this->blocks = gcnew System::Collections::Generic::List<QS::Fx::Base::Block>();

				pmt = NULL;
				puliSize = NULL;
				pStream = NULL;
				pReader = NULL;
				pFG = NULL;
				pVW = NULL;
				pMC = NULL;
				pME = NULL;

				pmt = new CMediaType();
				pmt->majortype = MEDIATYPE_Stream;
				puliSize = new ULARGE_INTEGER();

				switch (videotype)
				{
					case VideoType::MPEG1System:
						pmt->subtype = MEDIASUBTYPE_MPEG1System;
						break;

					case VideoType::MPEG1Audio:
						pmt->subtype = MEDIASUBTYPE_MPEG1Audio;
						break;

					case VideoType::MPEG1Video:
						pmt->subtype = MEDIASUBTYPE_MPEG1Video;
						break;

					case VideoType::MPEG1VideoCD:
						pmt->subtype = MEDIASUBTYPE_MPEG1VideoCD;
						break;

					case VideoType::Avi:
						pmt->subtype = MEDIASUBTYPE_Avi;
						break;

					case VideoType::QTMovie:
						pmt->subtype = MEDIASUBTYPE_QTMovie;
						break;

					case VideoType::WAVE:
						pmt->subtype = MEDIASUBTYPE_WAVE;
						break;
	
					default:
						throw gcnew Exception("Unknown video type.");
				}

				puliSize->QuadPart = videosize;

				pStream = new CPacketStream((LONGLONG) puliSize->QuadPart); // , (DWORD) videoid, &_PacketStreamReadCallback);
			}

			#pragma endregion

			#pragma region _PacketStreamReadCallback

//			HRESULT Video::_PacketStreamReadCallback(DWORD videoid,
//				LONGLONG llPos, PBYTE pbBuffer, DWORD dwBytesToRead, LPDWORD pdwBytesRead)
//			{
//				HRESULT result;
//
//				Monitor::Enter(QS::_core_x_::Video::Video::typeid);
//				try
//				{
//					if (videos == nullptr)
//						throw gcnew Exception();
//					Video ^video = videos[(__int32) videoid];					
//					result = video->_PacketStreamRead(llPos, pbBuffer, dwBytesToRead, pdwBytesRead);
//				}
//				finally
//				{
//					Monitor::Exit(QS::_core_x_::Video::Video::typeid);
//				}				
//				return result;
//			}

			#pragma endregion

			#pragma region Destructor

			Video::~Video()
			{
				if (pMC)
				{
					pMC->Stop();
					pMC->Release();
				}

				if (pME)
					pME->Release();

				if (pVW)
				{
					pVW->put_Visible(OAFALSE);
					pVW->put_Owner(NULL);
					pVW->Release();
				}

				if (pReader)
					pReader->Release();

				if (pStream)
					delete pStream;

				if (pFG)
				{
					ULONG ulRelease = pFG->Release();
					if (ulRelease != 0)
						throw gcnew Exception("Filter graph could not be released.");
				}

				if (puliSize)
					delete puliSize;

				if (pmt)
					delete pmt;

//				CoUninitialize();
			}

			#pragma endregion

			#pragma region IVideo::Window

			IntPtr Video::Window::get()
			{ 
				return handle; 
			}

			#pragma endregion

			#pragma region IVideo::Rectangle

			System::Drawing::Rectangle Video::Rectangle::get()
			{
				return rectangle;
			}

			void Video::Rectangle::set(System::Drawing::Rectangle rectangle)
			{
				this->rectangle = rectangle;
				if (pVW)
					pVW->SetWindowPosition(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
			}

			#pragma endregion

			#pragma region IVideo::Type

			VideoType Video::Type::get()
			{ 
				return videotype;
			}

			#pragma endregion

			#pragma region IVideo::Length

			__int64 Video::Length::get()
			{ 
				return videosize;
			}

			#pragma endregion

			#pragma region IVideo::Position

			__int64 Video::Position::get()
			{ 
				return pStream->_GetPosition(); 
			}

			#pragma endregion

			#pragma region IVideo::MinBuffered

			__int64 Video::MinBuffered::get()
			{ 
				return pStream->_GetFrom(); 
			}

			#pragma endregion

			#pragma region IVideo::MaxBuffered

			__int64 Video::MaxBuffered::get()
			{ 
				return pStream->_GetTo(); 
			}

			#pragma endregion

			#pragma region IVideo::Wait

/*
			void Video::Wait()
			{
				HRESULT hr;
				LONG levCode;
				hr = pME->WaitForCompletion(INFINITE, &levCode);
				if (FAILED(hr))
					throw gcnew Exception("WaitForCompletion failed.");
			}
*/

			#pragma endregion

			#pragma region IVideo::Start

			void Video::Start()
			{
				System::Threading::Monitor::Enter(this);
				try
				{
					if (pReader)
						throw gcnew Exception("Already started.");

					HRESULT hr = S_OK;
					pReader = new CPacketReader(pStream, pmt, &hr);
					if (FAILED(hr) || pReader == NULL)
					{
						delete pReader;
						throw gcnew Exception("Could not create filter.");
					}
					pReader->AddRef();

					IFilterGraph *_pFG;
					hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC, IID_IFilterGraph, (void **) &_pFG);
					if (FAILED(hr))
						throw gcnew Exception("Can't  create filter graph.");
					pFG = _pFG;

					hr = (pFG)->AddFilter(pReader, NULL);
					if (FAILED(hr))
						throw gcnew Exception("Can't add filter to the graph.");

					IGraphBuilder *pBuilder;
					hr = (pFG)->QueryInterface(IID_IGraphBuilder, (void **) &pBuilder);
					if (FAILED(hr))
						throw gcnew Exception("Can't get the IGraphBuilder interface.");

					hr = pBuilder->Render(pReader->GetPin(0));
					pBuilder->Release();
					if (FAILED(hr))
						throw gcnew Exception("Can't render the filter graph.");

					IVideoWindow *_pVW;
					hr = pFG->QueryInterface(IID_IVideoWindow, (void **) &_pVW);
					if (FAILED(hr))
						throw gcnew Exception("Cannot get interface IVideoWindow");
					pVW = _pVW;

					pVW->put_Owner((OAHWND) handle.ToInt32());
					pVW->put_WindowStyle(WS_CHILD | WS_CLIPSIBLINGS);

					pVW->SetWindowPosition(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
					pVW->SetWindowForeground(OATRUE);

					IMediaControl *_pMC;
					hr = pFG->QueryInterface(IID_IMediaControl, (void **) &_pMC);
					if (FAILED(hr))
						throw gcnew Exception("Cannot get interface IMediaControl");
					pMC = _pMC;

					IMediaEvent *_pME;
					hr = pFG->QueryInterface(IID_IMediaEvent, (void **) &_pME);
					if (FAILED(hr))
					{
						pMC->Release();
						throw gcnew Exception("Cannot get interface IMediaEvent");
					}
					pME = _pME;

					OAEVENT oEvent;
					hr = pME->GetEventHandle(&oEvent);
					if (FAILED(hr))
						throw gcnew Exception("Can't get event handle.");

					hr = pMC->Run();
					if (FAILED(hr))
						throw gcnew Exception("Can't run the graph.");
				}
				finally 
				{
					System::Threading::Monitor::Exit(this);
				}
			}

			#pragma endregion

			#pragma region IVideo::Stop

			void Video::Stop()
			{
				throw gcnew NotImplementedException();
/*
				System::Threading::Monitor::Enter(this);
				try
				{
				}
				finally 
				{
					System::Threading::Monitor::Exit(this);
				}
*/
			}

			#pragma endregion

			#pragma region IVideo::Append

			void Video::Append(QS::Fx::Base::Block block)
			{
				System::Threading::Monitor::Enter(this);
				try
				{
					blocks->Add(block);
					// buffered += block.size;
					pStream->Append(((PBYTE) block.address.ToPointer()) + (DWORD) block.offset, (DWORD) block.size);
				}
				finally 
				{
					System::Threading::Monitor::Exit(this);
				}
			}

			#pragma endregion

			#pragma region _DownloadFile

/*
			void Video::_DownloadFile()
			{
				int len = lstrlen(filename);
				if (len >= 4 && filename[len - 4] == TEXT('.'))
					lpType = filename + len - 3;
				else
					throw gcnew Exception("Invalid file extension.");

				if (lstrcmpi(lpType, TEXT("mpg")) == 0) 
					pmt->subtype = MEDIASUBTYPE_MPEG1System;
				else if(lstrcmpi(lpType, TEXT("mpa")) == 0)
					pmt->subtype = MEDIASUBTYPE_MPEG1Audio;
				else if(lstrcmpi(lpType, TEXT("mpv")) == 0)
					pmt->subtype = MEDIASUBTYPE_MPEG1Video;
				else if(lstrcmpi(lpType, TEXT("dat")) == 0)
					pmt->subtype = MEDIASUBTYPE_MPEG1VideoCD;
				else if(lstrcmpi(lpType, TEXT("avi")) == 0)
					pmt->subtype = MEDIASUBTYPE_Avi;
				else if(lstrcmpi(lpType, TEXT("mov")) == 0)
					pmt->subtype = MEDIASUBTYPE_QTMovie;
				else if(lstrcmpi(lpType, TEXT("wav")) == 0)
					pmt->subtype = MEDIASUBTYPE_WAVE;
				else
					throw gcnew Exception("Unknown file type.");

				HANDLE hFile = CreateFile(filename, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);
				if (hFile == INVALID_HANDLE_VALUE)
					throw gcnew Exception("Could not open file.");

				puliSize->LowPart = GetFileSize(hFile, &puliSize->HighPart);

				pbMem = new BYTE[puliSize->LowPart];
				if (pbMem == NULL)
					throw gcnew Exception("Could not allocate memory.");

				DWORD dwBytesRead;

				if (!ReadFile(hFile, (LPVOID) pbMem, puliSize->LowPart, &dwBytesRead, NULL) || (dwBytesRead != puliSize->LowPart))
				{
					CloseHandle(hFile);
					throw gcnew Exception("Could not read file.");
				}

				CloseHandle(hFile);
			}
*/

			#pragma endregion

			#pragma region _PacketStreamRead

//			HRESULT Video::_PacketStreamRead(
//				LONGLONG llPos, PBYTE pbBuffer, DWORD dwBytesToRead, LPDWORD pdwBytesRead)
//			{
//				System::Threading::Monitor::Enter(this);
//				try
//				{
//					*pdwBytesRead = 0;
//
//					__int64 from = llPos;
//					if (from < removed || from > buffered)
//						throw gcnew Exception("Illegal offset");
//
//					__int64 to = llPos + dwBytesToRead;
//					if (to > buffered)
//						to = buffered;
//					if (to < from)
//						throw gcnew Exception("Illegal length");
//
//					__int64 here = removed;
//					__int32 index = blockindex;
//					while (here + blocks[index].size < from && index < blocks->Count)
//					{
//						here += blocks[index].size;
//						index++;
//					}
//
//					char *pbuffer = (char *) pbBuffer;
//
//					__int32 size = from - to;
//					__int32 offset = from - here;
//					__int32 done = 0;
//
//					while (size > 0 && index < blocks->Count)
//					{
//						char *pblock = ((char *) blocks[index].address.ToPointer()) + blocks[index].offset + offset;
//						int n = blocks[index].size;
//						if (n > size)
//							n = size;
//
//						CopyMemory((PVOID) pbuffer, (PVOID) pblock, n);
//
//						done += n;
//						index++;
//						offset = 0;
//						size -= n;
//					}
//
//					*pdwBytesRead = done;
//
//					if (size > 0)
//						return E_FAIL;
//					else
//						return S_OK;
//				}
//				catch (Exception^ exception)
//				{
//					System::Threading::Monitor::Exit(this);
//					return E_FAIL;
//				}
//				System::Threading::Monitor::Exit(this);
//			}

			#pragma endregion
		}
	}
}
