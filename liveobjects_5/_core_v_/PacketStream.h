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

#include "AsyncStream.h"
#include "Packet.h"

// typedef HRESULT (* PacketStreamReadCallback)(DWORD videoid, LONGLONG llPos, PBYTE pbBuffer, DWORD dwBytesToRead, LPDWORD pdwBytesRead);

class CPacketStream : public CAsyncStream
{
public:

	CPacketStream(LONGLONG llLength); // , DWORD videoid, PacketStreamReadCallback packetStreamReadCallback);
    virtual ~CPacketStream();

	HRESULT SetPointer(LONGLONG llPos);
	HRESULT Read(PBYTE pbBuffer, DWORD dwBytesToRead, BOOL bAlign, LPDWORD pdwBytesRead);
	LONGLONG Size(LONGLONG *pSizeAvailable);
	DWORD Alignment();
	void Lock();
	void Unlock();

	// void _SetFrom(LONGLONG llFrom);
	// void _SetTo(LONGLONG llTo);
	// void _SetLength(LONGLONG llLength);

	void Append(PBYTE pbBuffer, DWORD dwLength);

	LONGLONG _GetPosition();
	LONGLONG _GetFrom();
	LONGLONG _GetTo();
	LONGLONG _GetLength();

private:

    CCritSec m_csLock;
    LONGLONG m_llFrom;
    LONGLONG m_llTo;
    LONGLONG m_llLength;
    LONGLONG m_llPosition;
	CPacket *m_pHead;
	CPacket *m_pTail;
	// DWORD m_videoid;
	// PacketStreamReadCallback m_packetStreamReadCallback;
};
