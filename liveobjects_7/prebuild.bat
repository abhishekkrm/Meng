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
echo %1..\liveobjects_2\%2liveobjects_2.dll
set root=%1%2\%3
set mppg=%VSSDK100Install%VisualStudioIntegration\Tools\Bin\MPPG.exe
set mplex=%VSSDK100Install%VisualStudioIntegration\Tools\Bin\MPLex.exe
set mpframe=%VSSDK100Install%VisualStudioIntegration\Tools\Bin\MPLex.frame
if not exist %root% set root=%1%3
xcopy /y /f %root%\liveobjects_2.dll %4
xcopy /y /f %root%\liveobjects_2.pdb %4
xcopy /y /f %root%\liveobjects_3.dll %4
xcopy /y /f %root%\liveobjects_3.pdb %4
xcopy /y /f %root%\liveobjects_5.dll %4
xcopy /y /f %root%\liveobjects_5.pdb %4
rem | echo __________UNOBFUSCATED_ONLY_SECTION_ENTER__________
rem | "%mppg%" -report %5_qss_p_\Parser_\Parser_.y > %5_qss_p_\Parser_\Report_.txt 2> %5_qss_p_\Parser_\Output_.txt
rem | echo ________________________________________________________________________________ >> %5_qss_p_\Parser_\Output_.txt
rem | "%mppg%" -mplex -conflicts %5_qss_p_\Parser_\Parser_.y > %5_qss_p_\Parser_\Parser_.cs 2>> %5_qss_p_\Parser_\Output_.txt
rem | "%mplex%" /frame:"%mpframe%" /out:%5_qss_p_\Parser_\Lexer_.cs %5_qss_p_\Parser_\Lexer_.lex
rem | echo __________UNOBFUSCATED_ONLY_SECTION_LEAVE__________
rem | cd %5_qss_p_\ParserGenerator_
rem | _patch.bat
