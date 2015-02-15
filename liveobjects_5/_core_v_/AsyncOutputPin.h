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

#include "AsyncIo.h"

class CAsyncReader;

class CAsyncOutputPin : public IAsyncReader, public CBasePin
{
public:

    CAsyncOutputPin(HRESULT *phr, CAsyncReader *pReader, CAsyncIo *pIo, CCritSec *pLock);
    ~CAsyncOutputPin();

    DECLARE_IUNKNOWN
    STDMETHODIMP NonDelegatingQueryInterface(REFIID, void **);

    STDMETHODIMP Connect(IPin *pReceivePin, const AM_MEDIA_TYPE *pmt);
    HRESULT GetMediaType(int iPosition, CMediaType *pMediaType);
    HRESULT CheckMediaType(const CMediaType *pType);
    HRESULT CheckConnect(IPin *pPin);
    HRESULT CompleteConnect(IPin *pReceivePin);
    HRESULT BreakConnect();

    STDMETHODIMP RequestAllocator(IMemAllocator *pPreferred, ALLOCATOR_PROPERTIES *pProps, IMemAllocator **ppActual);
    STDMETHODIMP Request(IMediaSample *pSample, DWORD_PTR dwUser);
    STDMETHODIMP WaitForNext(DWORD dwTimeout, IMediaSample **ppSample, DWORD_PTR *pdwUser);
    STDMETHODIMP SyncReadAligned(IMediaSample *pSample);
    STDMETHODIMP SyncRead(LONGLONG llPosition, LONG lLength, BYTE* pBuffer);
    STDMETHODIMP Length(LONGLONG* pTotal, LONGLONG* pAvailable);
    STDMETHODIMP BeginFlush();
    STDMETHODIMP EndFlush();

protected:

    CAsyncReader *m_pReader;
    CAsyncIo *m_pIo;
    BOOL m_bQueriedForAsyncReader;

    HRESULT InitAllocator(IMemAllocator **ppAlloc);
};
