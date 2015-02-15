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
#include "AsyncFilter.h"

/*
CAsyncFilter::CAsyncFilter(LPUNKNOWN pUnk, HRESULT *phr)
:	CAsyncReader(NAME("Packet Reader"), pUnk, &m_Stream, phr),
    m_pFileName(NULL),
    m_pbData(NULL)
{
}

CAsyncFilter::~CAsyncFilter()
{
	delete [] m_pbData;
	delete [] m_pFileName;
}

CUnknown * WINAPI CAsyncFilter::CreateInstance(LPUNKNOWN pUnk, HRESULT *phr)
{
    ASSERT(phr);
    return new CAsyncFilter(pUnk, phr);
}

STDMETHODIMP CAsyncFilter::NonDelegatingQueryInterface(REFIID riid, void **ppv)
{
    if (riid == IID_IFileSourceFilter) 
        return GetInterface((IFileSourceFilter *) this, ppv);
	else 
        return CAsyncReader::NonDelegatingQueryInterface(riid, ppv);
}

STDMETHODIMP CAsyncFilter::Load(LPCOLESTR lpwszFileName, const AM_MEDIA_TYPE *pmt)
{
    CheckPointer(lpwszFileName, E_POINTER);

    int cch = lstrlenW(lpwszFileName) + 1;

#ifndef UNICODE
    TCHAR *lpszFileName=0;
    lpszFileName = new char[cch * 2];
    if (!lpszFileName) {
  	    return E_OUTOFMEMORY;
    }
    WideCharToMultiByte(GetACP(), 0, lpwszFileName, -1,
			lpszFileName, cch, NULL, NULL);
#else
    TCHAR lpszFileName[MAX_PATH]={0};
    (void)StringCchCopy(lpszFileName, NUMELMS(lpszFileName), lpwszFileName);
#endif
    CAutoLock lck(&m_csFilter);

    CMediaType cmt;
    if (NULL == pmt) 
	{
        cmt.SetType(&MEDIATYPE_Stream);
        cmt.SetSubtype(&MEDIASUBTYPE_NULL);
    } 
	else 
	{
        cmt = *pmt;
    }

    if (!ReadTheFile(lpszFileName)) 
	{
#ifndef UNICODE
        delete [] lpszFileName;
#endif
        return E_FAIL;
    }

    m_Stream.Init(m_pbData, m_llSize);

    m_pFileName = new WCHAR[cch];

    if (m_pFileName!=NULL)
	    CopyMemory(m_pFileName, lpwszFileName, cch*sizeof(WCHAR));

	m_mt = cmt;

    cmt.bTemporalCompression = TRUE;
    cmt.lSampleSize = 1;

    return S_OK;
}

STDMETHODIMP CAsyncFilter::GetCurFile(LPOLESTR * ppszFileName, AM_MEDIA_TYPE *pmt)
{
    CheckPointer(ppszFileName, E_POINTER);
    *ppszFileName = NULL;

    if (m_pFileName!=NULL) 
	{
    	DWORD n = sizeof(WCHAR)*(1+lstrlenW(m_pFileName));

        *ppszFileName = (LPOLESTR) CoTaskMemAlloc( n );
        if (*ppszFileName!=NULL) 
		{
              CopyMemory(*ppszFileName, m_pFileName, n);
        }
    }

    if (pmt!=NULL) 
	{
        CopyMediaType(pmt, &m_mt);
    }

    return NOERROR;
}

BOOL CAsyncFilter::ReadTheFile(LPCTSTR lpszFileName)
{
    DWORD dwBytesRead;

    HANDLE hFile = CreateFile(lpszFileName, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, NULL);
    if (hFile == INVALID_HANDLE_VALUE) 
    {
        DbgLog((LOG_TRACE, 2, TEXT("Could not open %s\n"), lpszFileName));
        return FALSE;
    }

    ULARGE_INTEGER uliSize;
    uliSize.LowPart = GetFileSize(hFile, &uliSize.HighPart);

    PBYTE pbMem = new BYTE[uliSize.LowPart];
    if (pbMem == NULL) 
    {
        CloseHandle(hFile);
        return FALSE;
    }

    if (!ReadFile(hFile, (LPVOID) pbMem, uliSize.LowPart, &dwBytesRead, NULL) || (dwBytesRead != uliSize.LowPart))
    {
        DbgLog((LOG_TRACE, 1, TEXT("Could not read file\n")));

        delete [] pbMem;
        CloseHandle(hFile);
        return FALSE;
    }

    m_pbData = pbMem;
    m_llSize = (LONGLONG)uliSize.QuadPart;

    CloseHandle(hFile);

    return TRUE;
}
*/
