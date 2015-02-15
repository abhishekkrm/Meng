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

#include <stdio.h>
#include <intrin.h>

#pragma intrinsic(__rdtsc)

#define PROFILE_CLOCK

// #include "IClock.h"

using namespace System;

namespace QS
{
	namespace _core_c_
	{
		namespace Core
		{
			#pragma warning(disable:4793) 
						
			inline unsigned __int64 RDTSC()
			{
				int cpuinfo[4];  
				__cpuid(cpuinfo, 0);
				return __rdtsc();
			}
			
			#pragma warning(default:4793)

			public ref class Clock : public QS::Fx::Inspection::Inspectable, public QS::Fx::Clock::IClock
			{
			public:

				static Clock()
				{
					commonClock = gcnew Clock();
				}

				static property Clock^ SharedClock
				{
					Clock^ get() { return commonClock; }
				}

				Clock()
				{
#if defined(PROFILE_CLOCK)
					 _statistics_cycle = gcnew System::Collections::Generic::List<QS::_core_e_::Data::XY>();
					 _statistics_cpufrequency = gcnew System::Collections::Generic::List<QS::_core_e_::Data::XY>();
#endif
					Microsoft::Win32::RegistryKey^ registryKey = Microsoft::Win32::Registry::LocalMachine->OpenSubKey(
						"HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");
					Object^ value;
					if (registryKey == nullptr || (value = registryKey->GetValue("~MHz")) == nullptr)
						throw gcnew Exception("Cannot access registry.");
					cpufrequency = ((double)((int) value)) * 1000000;						
					LARGE_INTEGER f;
					if (!QueryPerformanceFrequency(&f))
						throw gcnew Exception("Cannot read performance frequency");
					counterfrequency = f.QuadPart;
					unsigned __int64 cycles = RDTSC();
					while (true)
					{
						LARGE_INTEGER c;
						if (!QueryPerformanceCounter(&c))
							throw gcnew Exception("Cannot read performance counter");						
						unsigned __int64 newcycles = RDTSC();
						if ((((double) (newcycles - cycles)) / cpufrequency) > 0.0001)
							cycles = newcycles;
						else
						{
							lastcycles = cycles;
							lastcounter = c.QuadPart;
							break;
						}
					}
					cycle = 1 / cpufrequency;
					correction = 0;
					nextcycles = lastcycles + cpufrequency; 
				}

				//property unsigned __int64 RDTSC
				//{
				//	unsigned __int64 get() { return QS::_core_c_::Core::RDTSC(); }
				//}

				//property double Frequency
				//{
				//	double get() { return frequency; }
				//}

				virtual property double Time
				{
					double get() {	return ((double) ((unsigned __int64) (RDTSC() - initialcycles))) * cycle + correction; }
				}

				virtual void Adjust(double delta)
				{
					throw gcnew NotSupportedException();
				}

				void Correct(double multiplycycle, double addcorrection)
				{
					cycle = cycle * multiplycycle;
					correction = correction + addcorrection;
				}

				void Correct()
				{
					unsigned __int64 cycles = RDTSC();
					if (cycles > nextcycles)
					{
						__int64 counter;
						while (true)
						{
							LARGE_INTEGER c;
							if (!QueryPerformanceCounter(&c))
								throw gcnew Exception("Cannot read performance counter");						
							//LARGE_INTEGER f;
							//if (!QueryPerformanceFrequency(&f))
							//	throw gcnew Exception("Cannot read performance frequency");
							unsigned __int64 newcycles = RDTSC();
							if ((((double) (newcycles - cycles)) * cycle) > 0.0001)
								cycles = newcycles;
							else
							{
								counter = c.QuadPart;
								//counterfrequency = f.QuadPart;
								break;
							}
						}
						double timestamp = ((double) ((unsigned __int64) (cycles - initialcycles))) * cycle + correction;
						double cpufrequency_sample = ((((double) counterfrequency) * ((double) (cycles - lastcycles))) / ((double) (counter - lastcounter)));

						//double deviation = Math::Abs((cpufrequency_sample / cpufrequency) - 1);
						//if (deviation > 0.1)
						//	cpufrequency = cpufrequency * 0.5 + cpufrequency_sample * 0.5;
						cpufrequency = cpufrequency * 0.9 + cpufrequency_sample * 0.1;

						double newcycle = 1.0 / cpufrequency;
						double newcorrection = correction + ((double) (cycles - initialcycles)) * (cycle - newcycle);
						cycle = newcycle;
						correction = newcorrection;		
						lastcounter = counter;
						lastcycles = cycles;
						if (ncorrections > 30) 
							nextcycles = cycles + (1.0 / ((double) cycle));
						else if (ncorrections > 25) 
							nextcycles = cycles + (0.5 / ((double) cycle));
						else if (ncorrections > 20) 
							nextcycles = cycles + (0.2 / ((double) cycle));
						else if (ncorrections > 15) 
							nextcycles = cycles + (0.1 / ((double) cycle));
						else if (ncorrections > 10) 
							nextcycles = cycles + (0.05 / ((double) cycle));
						else if (ncorrections > 5) 
							nextcycles = cycles + (0.02 / ((double) cycle));
						else
							nextcycles = cycles + (0.01 / ((double) cycle));
#if defined(PROFILE_CLOCK)
						_statistics_cycle->Add(QS::_core_e_::Data::XY(cycles, cycle));
						_statistics_cpufrequency->Add(QS::_core_e_::Data::XY(cycles, cpufrequency));
#endif
						while ((((double) ((unsigned __int64) (RDTSC() - initialcycles))) * cycle + correction) < timestamp)
							System::Threading::Thread::Sleep(0);
						ncorrections++;
					}
				}

			private:

				[QS::Fx::Base::Inspectable]
				double cpufrequency;
				[QS::Fx::Base::Inspectable]
				__int64 counterfrequency;
				[QS::Fx::Base::Inspectable]
				unsigned __int64 initialcycles;
				[QS::Fx::Base::Inspectable]
				unsigned __int64 lastcycles;
				[QS::Fx::Base::Inspectable]
				unsigned __int64 nextcycles;
				[QS::Fx::Base::Inspectable]
				__int64 lastcounter;
				[QS::Fx::Base::Inspectable]
				double cycle;
				[QS::Fx::Base::Inspectable]
				double correction;
				[QS::Fx::Base::Inspectable]
				__int32 ncorrections;

				static Clock^ commonClock;

#if defined(PROFILE_CLOCK)
				System::Collections::Generic::List<QS::_core_e_::Data::XY>^ _statistics_cycle;
				[QS::Fx::Base::InspectableAttribute]
				property QS::_core_e_::Data::IDataSet^ _Statistics_Cycle
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(_statistics_cycle->ToArray()); }
				}
				System::Collections::Generic::List<QS::_core_e_::Data::XY>^ _statistics_cpufrequency;
				[QS::Fx::Base::InspectableAttribute]
				property QS::_core_e_::Data::IDataSet^ _Statistics_CpuFrequency
				{
					QS::_core_e_::Data::IDataSet^ get() { return gcnew QS::_core_e_::Data::XYSeries(_statistics_cpufrequency->ToArray()); }
				}
#endif
			};
		}
	}
}
