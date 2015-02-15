rem | Copyright (c) 2004-2009 Krzysztof Ostrowski. All rights reserved.
rem | 
rem | Redistribution and use in source and binary forms,
rem | with or without modification, are permitted provided that the following conditions
rem | are met:
rem | 
rem | 1. Redistributions of source code must retain the above copyright
rem |    notice, this list of conditions and the following disclaimer.
rem | 
rem | 2. Redistributions in binary form must reproduce the above
rem |    copyright notice, this list of conditions and the following
rem |    disclaimer in the documentation and/or other materials provided
rem |    with the distribution.
rem | 
rem | THIS SOFTWARE IS PROVIDED "AS IS" BY THE ABOVE COPYRIGHT HOLDER(S)
rem | AND ALL OTHER CONTRIBUTORS AND ANY EXPRESS OR IMPLIED WARRANTIES,
rem | INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
rem | MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
rem | IN NO EVENT SHALL THE ABOVE COPYRIGHT HOLDER(S) OR ANY OTHER
rem | CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
rem | SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
rem | LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
rem | USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
rem | ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
rem | OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
rem | OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
rem | SUCH DAMAGE.
rem | 
@echo on
xcopy /y /f %1\..\liveobjects_1\bin\%5\liveobjects_1.dll %4\
xcopy /y /f %1\..\liveobjects_1\bin\%5\liveobjects_1.pdb %4\
xcopy /y /f %1\..\liveobjects_4\bin\%5\liveobjects_4.dll %4\
xcopy /y /f %1\..\liveobjects_4\bin\%5\liveobjects_4.pdb %4\
xcopy /y /f %1\..\liveobjects_6\bin\%5\liveobjects_6.dll %4\
xcopy /y /f %1\..\liveobjects_6\bin\%5\liveobjects_6.pdb %4\
xcopy /y /f %1\..\liveobjects_7\bin\%5\liveobjects_7.dll %4\
xcopy /y /f %1\..\liveobjects_7\bin\%5\liveobjects_7.pdb %4\
xcopy /y /f %1\..\liveobjects_8\bin\%5\liveobjects_8.dll %4\
xcopy /y /f %1\..\liveobjects_8\bin\%5\liveobjects_8.pdb %4\
xcopy /y /f %1\..\liveobjects_9\bin\%5\liveobjects_9.dll %4\
xcopy /y /f %1\..\liveobjects_9\bin\%5\liveobjects_9.pdb %4\
mkdir C:\liveobjects\bin
rem - if exist C:\liveobjects\bin\liveobjects_9.dll C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\regasm.exe /unregister C:\liveobjects\bin\liveobjects_9.dll
rem - if exist C:\liveobjects\bin\liveobjects_8.dll C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\regasm.exe /unregister C:\liveobjects\bin\liveobjects_8.dll
rem - if exist C:\liveobjects\bin\liveobjects_7.dll C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\regasm.exe /unregister C:\liveobjects\bin\liveobjects_7.dll
rem - if exist C:\liveobjects\bin\liveobjects_6.dll C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\regasm.exe /unregister C:\liveobjects\bin\liveobjects_6.dll
rem - if exist C:\liveobjects\bin\liveobjects_5.dll C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\regasm.exe /unregister C:\liveobjects\bin\liveobjects_5.dll
rem - if exist C:\liveobjects\bin\liveobjects_4.dll C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\regasm.exe /unregister C:\liveobjects\bin\liveobjects_4.dll
rem - if exist C:\liveobjects\bin\liveobjects_1.dll C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\regasm.exe /unregister C:\liveobjects\bin\liveobjects_1.dll
del /f /q C:\liveobjects\bin\liveobjects_1.dll
del /f /q C:\liveobjects\bin\liveobjects_1.pdb
del /f /q C:\liveobjects\bin\liveobjects_2.dll
del /f /q C:\liveobjects\bin\liveobjects_2.pdb
del /f /q C:\liveobjects\bin\liveobjects_3.dll
del /f /q C:\liveobjects\bin\liveobjects_3.pdb
del /f /q C:\liveobjects\bin\liveobjects_4.dll
del /f /q C:\liveobjects\bin\liveobjects_4.pdb
del /f /q C:\liveobjects\bin\liveobjects_5.dll
del /f /q C:\liveobjects\bin\liveobjects_5.pdb
del /f /q C:\liveobjects\bin\liveobjects_6.dll
del /f /q C:\liveobjects\bin\liveobjects_6.pdb
del /f /q C:\liveobjects\bin\liveobjects_7.dll
del /f /q C:\liveobjects\bin\liveobjects_7.pdb
del /f /q C:\liveobjects\bin\liveobjects_8.dll
del /f /q C:\liveobjects\bin\liveobjects_8.pdb
del /f /q C:\liveobjects\bin\liveobjects_9.dll
del /f /q C:\liveobjects\bin\liveobjects_9.pdb
