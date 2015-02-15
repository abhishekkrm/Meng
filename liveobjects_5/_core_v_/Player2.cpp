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
#include "Form1.h"

#include "MemReader.h"

using namespace Player2;

int window_handle;

void ThreadCallback(System::Object ^obj)
{
	Application::EnableVisualStyles();
	Application::SetCompatibleTextRenderingDefault(false); 
	Form1 ^form1 = gcnew Form1();
	window_handle = form1->Handle.ToInt32();
	dynamic_cast<System::Threading::ManualResetEvent ^>(obj)->Set();
	Application::Run(form1);
}

int movie(int window_handle);

[STAThreadAttribute]
int main(array<System::String ^> ^args)
{
	System::Threading::Thread ^thread = nullptr;
	if (args->Length > 0)
		window_handle = Convert::ToInt32(args[0]);
	else
	{
		thread = gcnew System::Threading::Thread(
			gcnew System::Threading::ParameterizedThreadStart(&ThreadCallback));
		System::Threading::ManualResetEvent ^ready = gcnew System::Threading::ManualResetEvent(false);
		thread->Start(ready);
		ready->WaitOne();
	}

	int result = movie(window_handle);

	if (thread != nullptr)
		thread->Join();

	return result;
}

HRESULT SelectAndRender(CMemReader *pReader, IFilterGraph **pFG);
HRESULT PlayFileWait(IFilterGraph *pFG);

int movie(int window_handle)
{
	LPTSTR filename = L"C:\\Users\\krzys\\Videos\\espresso1.mpg";
    DWORD dwKBPerSec = 10000;
    LPTSTR lpType;

    CMediaType mt;
    mt.majortype = MEDIATYPE_Stream;

    int len = lstrlen(filename);
    if (len >= 4 && filename[len - 4] == TEXT('.'))
    {
        lpType = filename + len - 3;
    }
    else
    {
        _tprintf(_T("Invalid file extension\n"));
        return 1;
    }

    if (lstrcmpi(lpType, TEXT("mpg")) == 0) 
        mt.subtype = MEDIASUBTYPE_MPEG1System;
    else if(lstrcmpi(lpType, TEXT("mpa")) == 0)
        mt.subtype = MEDIASUBTYPE_MPEG1Audio;
    else if(lstrcmpi(lpType, TEXT("mpv")) == 0)
        mt.subtype = MEDIASUBTYPE_MPEG1Video;
    else if(lstrcmpi(lpType, TEXT("dat")) == 0)
        mt.subtype = MEDIASUBTYPE_MPEG1VideoCD;
    else if(lstrcmpi(lpType, TEXT("avi")) == 0)
        mt.subtype = MEDIASUBTYPE_Avi;
    else if(lstrcmpi(lpType, TEXT("mov")) == 0)
        mt.subtype = MEDIASUBTYPE_QTMovie;
    else if(lstrcmpi(lpType, TEXT("wav")) == 0)
        mt.subtype = MEDIASUBTYPE_WAVE;
    else
    {
        _tprintf(_T("Unknown file type: %s\n"), lpType);
        return 1;
    }

    HANDLE hFile = CreateFile(filename, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);
    if (hFile == INVALID_HANDLE_VALUE)
    {
        _tprintf(_T("Could not open %s\n"), filename);
        return 1;
    }

    ULARGE_INTEGER uliSize;
    uliSize.LowPart = GetFileSize(hFile, &uliSize.HighPart);

    PBYTE pbMem = new BYTE[uliSize.LowPart];
    if (pbMem == NULL)
    {
        _tprintf(_T("Could not allocate %d bytes\n"), uliSize.LowPart);
        return 1;
    }

    DWORD dwBytesRead;

    if (!ReadFile(hFile, (LPVOID) pbMem, uliSize.LowPart, &dwBytesRead, NULL) || (dwBytesRead != uliSize.LowPart))
    {
        _tprintf(_T("Could not read file\n"));
        CloseHandle(hFile);
        return 1;
    }

    CloseHandle(hFile);

    HRESULT hr = S_OK;

    CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

    CMemStream Stream(pbMem, (LONGLONG)uliSize.QuadPart, dwKBPerSec);

    CMemReader *rdr = new CMemReader(&Stream, &mt, &hr);
    if (FAILED(hr) || rdr == NULL)
    {
        delete rdr;
        _tprintf(_T("Could not create filter - HRESULT 0x%8.8X\n"), hr);
        CoUninitialize();
        return 1;
    }

    rdr->AddRef();

    IFilterGraph *pFG = NULL;
    hr = SelectAndRender(rdr, &pFG);

    if (FAILED(hr))
    {
        _tprintf(_T("Failed to create graph and render file - HRESULT 0x%8.8X"), hr);
    }
    else
    {
		IVideoWindow *pVW = NULL;
		if (FAILED(pFG->QueryInterface(IID_IVideoWindow, (void **) &pVW)))
		{
			_tprintf(_T("Cannot get interface IVideoWindow"));
			return -1;
		}

		pVW->put_Owner((OAHWND) window_handle);
		pVW->put_WindowStyle(WS_CHILD | WS_CLIPSIBLINGS);
		RECT vwrect;
		GetClientRect((HWND) window_handle, &vwrect);
		pVW->SetWindowPosition(0, 0, vwrect.right, vwrect.bottom);

        HRESULT hr = PlayFileWait(pFG);
        if (FAILED(hr))
        {
            _tprintf(_T("Failed to play graph - HRESULT 0x%8.8X"), hr);
        }

		pVW->put_Visible(OAFALSE);
		pVW->put_Owner(NULL);
		pVW->Release();
    }

    rdr->Release();

    if (pFG)
    {
        ULONG ulRelease = pFG->Release();
        if (ulRelease != 0)
        {
            _tprintf(_T("Filter graph count not 0! (was %d)"), ulRelease);
        }
    }

    CoUninitialize();
	return 0;
}

HRESULT SelectAndRender(CMemReader *pReader, IFilterGraph **ppFG)
{
    CheckPointer(pReader,E_POINTER);
    CheckPointer(ppFG,E_POINTER);

    HRESULT hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC, IID_IFilterGraph, (void**) ppFG);
    if (FAILED(hr))
    {
        return hr;
    }

    hr = (*ppFG)->AddFilter(pReader, NULL);

    if (FAILED(hr))
    {
        return hr;
    }

    IGraphBuilder *pBuilder;

    hr = (*ppFG)->QueryInterface(IID_IGraphBuilder, (void **)&pBuilder);
    if (FAILED(hr))
    {
        return hr;
    }

    hr = pBuilder->Render(pReader->GetPin(0));

    pBuilder->Release();
    return hr;
}


HRESULT PlayFileWait(IFilterGraph *pFG)
{
    CheckPointer(pFG,E_POINTER);

    HRESULT hr;
    IMediaControl *pMC=0;
    IMediaEvent   *pME=0;

    hr = pFG->QueryInterface(IID_IMediaControl, (void **)&pMC);
    if (FAILED(hr))
    {
        return hr;
    }

    hr = pFG->QueryInterface(IID_IMediaEvent, (void **)&pME);
    if (FAILED(hr))
    {
        pMC->Release();
        return hr;
    }

    OAEVENT oEvent;
    hr = pME->GetEventHandle(&oEvent);
    if (SUCCEEDED(hr))
    {
        hr = pMC->Run();

        if(SUCCEEDED(hr))
        {
            LONG levCode;
            hr = pME->WaitForCompletion(INFINITE, &levCode);
        }
    }

    pMC->Release();
    pME->Release();

    return hr;
}
