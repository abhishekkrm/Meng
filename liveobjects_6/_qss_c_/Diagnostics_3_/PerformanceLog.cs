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

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Diagnostics_3_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class PerformanceLog : QS.Fx.Inspection.Inspectable, QS._core_c_.Diagnostics2.IModule, IPerformanceLog
    {
        public PerformanceLog(QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock, double samplingFrequency)
        {
            this.clock = clock;
            this.alarmClock = alarmClock;
            this.samplingFrequency = samplingFrequency;

            if (samplingFrequency < 0)
                throw new Exception("Bad arguments.");

            if (samplingFrequency > 0)
                alarm = alarmClock.Schedule(1 / samplingFrequency, new QS.Fx.Clock.AlarmCallback(this.SamplingCallback), null);
        }

        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();
        [QS._core_c_.Diagnostics.ComponentCollection("Categories")]
        private IDictionary<string, Category> categories = new Dictionary<string, Category>();
        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IAlarm alarm;
        private double samplingFrequency;
        [QS.Fx.Base.Inspectable("Number of Samples Collected")]
        private int numberOfSamplesCollected;

        #region Sampling Callback

        private void SamplingCallback(QS.Fx.Clock.IAlarm alarm)
        {
            if (!alarm.Cancelled)
            {
                Sample();

                lock (this)
                {
                    numberOfSamplesCollected++;
                    if (samplingFrequency > 0)
                        alarm.Reschedule(1 / samplingFrequency);
                }
            }
        }

        #endregion

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        #region AddCounter

        public void AddCounter(string categoryName, string instanceName, string counterName)
        {
            Category category;
            lock (this)
            {
                if (!categories.TryGetValue(categoryName, out category))
                {
                    System.Diagnostics.PerformanceCounterCategory category_found = null;
                    foreach (System.Diagnostics.PerformanceCounterCategory c in System.Diagnostics.PerformanceCounterCategory.GetCategories())
                    {
                        if (c.CategoryName.Equals(categoryName))
                            category_found = c;
                    }

                    if (category_found != null)
                    {
                        category = new Category(this, categoryName, category_found);
                        categories.Add(categoryName, category);
                        ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register(categoryName, ((QS._core_c_.Diagnostics2.IModule)category).Component);
                    }
                    else
                        throw new Exception("No category named \"" + categoryName + "\" exists.");
                }
            }

            category.AddCounter(instanceName, counterName);
        }

        #endregion

        #region Sample

        public void Sample()
        {
            Sample(clock.Time);
        }

        public void Sample(double time)
        {
            lock (this)
            {
                foreach (Category category in categories.Values)
                    category.Sample(time);
            }
        }

        #endregion

        #region Sampling Frequency

        public double SamplingFrequency
        {
            get { return samplingFrequency; }
            set 
            { 
                if (value < 0)
                    throw new Exception("Bad argument.");

                lock (this)
                {
                    if (samplingFrequency == 0)
                    {
                        samplingFrequency = value;
                        if (samplingFrequency > 0)
                            alarm = alarmClock.Schedule(1 / samplingFrequency, new QS.Fx.Clock.AlarmCallback(this.SamplingCallback), null);
                    }
                    else
                    {
                        samplingFrequency = value;
                        if (samplingFrequency == 0)
                        {
                            if (alarm != null)
                                alarm.Cancel();
                            alarm = null;
                        }
                    }
                }
            }
        }

        #endregion

        #region Class Category

        [QS._core_c_.Diagnostics.ComponentContainer]
        private class Category : QS.Fx.Inspection.Inspectable, QS._core_c_.Diagnostics2.IModule
        {
            public Category(PerformanceLog performanceLog, string name, System.Diagnostics.PerformanceCounterCategory category)
            {
                this.category = category;
                this.performanceLog = performanceLog;
                this.name = name;
            }

            private System.Diagnostics.PerformanceCounterCategory category;
            private PerformanceLog performanceLog;
            private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();
            private string name;
            [QS._core_c_.Diagnostics.ComponentCollection("Instances")]
            private IDictionary<string, Instance> instances = new Dictionary<string, Instance>();

            #region IModule Members

            QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
            {
                get { return diagnosticsContainer; }
            }

            #endregion

            #region AddCounter

            public void AddCounter(string instanceName, string counterName)
            {
                if (instanceName != null && instanceName.Length > 0)
                {
                    Instance instance;
                    lock (this)
                    {
                        if (!instances.TryGetValue(instanceName, out instance))
                        {
                            if (category.InstanceExists(instanceName))
                            {
                                instance = new Instance(this, instanceName);
                                instances.Add(instanceName, instance);
                                ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register(instanceName, ((QS._core_c_.Diagnostics2.IModule)instance).Component);
                            }
                            else
                                throw new Exception("No instance \"" + instanceName + "\" exists in category \"" + name + "\".");
                        }
                    }

                    instance.AddCounter(counterName);
                }
                else
                {
                    foreach (string name in category.GetInstanceNames())
                        AddCounter(name, counterName);
                }
            }

            #endregion

            #region Sample

            public void Sample(double time)
            {
                lock (this)
                {
                    foreach (Instance instance in instances.Values)
                        instance.Sample(time);
                }
            }

            #endregion

            #region Class Instance

            [QS._core_c_.Diagnostics.ComponentContainer]
            private class Instance : QS.Fx.Inspection.Inspectable, QS._core_c_.Diagnostics2.IModule
            {
                public Instance(Category category, string name)
                {
                    this.category = category;
                    this.name = name;
                }

                private Category category;
                private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();
                private string name;
                [QS._core_c_.Diagnostics.ComponentCollection("Counters")]
                private IDictionary<string, Counter> counters = new Dictionary<string, Counter>();

                #region IModule Members

                QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
                {
                    get { return diagnosticsContainer; }
                }

                #endregion

                #region AddCounter

                public void AddCounter(string counterName)
                {
                    lock (this)
                    {
                        if (counterName != null && counterName.Length > 0)
                        {
                            if (counters.ContainsKey(counterName))
                                throw new Exception("Counter already exists.");
                            else
                            {
                                if (category.category.CounterExists(counterName))
                                {
                                    Counter counter = new Counter(this, counterName);
                                    counters.Add(counterName, counter);
                                    ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register(counterName, ((QS._core_c_.Diagnostics2.IModule)counter).Component);
                                }
                                else
                                    throw new Exception("No counter \"" + counterName + "\" exists in category \"" + category.name + "\".");
                            }
                        }
                        else
                        {
                            foreach (System.Diagnostics.PerformanceCounter c in category.category.GetCounters(name))
                                AddCounter(c.CounterName);
                        }
                    }
                }

                #endregion

                #region Sample

                public void Sample(double time)
                {
                    lock (this)
                    {
                        foreach (Counter counter in counters.Values)
                            counter.Sample(time);
                    }
                }

                #endregion

                #region Class Counter

                [QS._core_c_.Diagnostics.ComponentContainer]
                private class Counter : QS.Fx.Inspection.Inspectable, QS._core_c_.Diagnostics2.IModule
                {
                    public Counter(Instance instance, string name)
                    {
                        this.instance = instance;
                        this.name = name;
                        samples = new QS._qss_c_.Statistics_.Samples2D(name);
                        diagnosticsProperty = new QS._core_c_.Diagnostics2.Property(samples);
                        counter = new System.Diagnostics.PerformanceCounter(instance.category.name, name, instance.name);
                    }

                    private Instance instance;
                    private QS._core_c_.Diagnostics2.Property diagnosticsProperty;
                    private string name;
                    [QS._core_c_.Diagnostics.Component("Samples")]
                    private Statistics_.Samples2D samples;
                    private System.Diagnostics.PerformanceCounter counter;

                    #region IModule Members

                    QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
                    {
                        get { return diagnosticsProperty; }
                    }

                    #endregion

                    #region Sample

                    public void Sample(double time)
                    {
                        lock (this)
                        {
                            samples.Add(time, (double) counter.NextValue());
                        }
                    }

                    #endregion
                }

                #endregion
            }

            #endregion

        }

        #endregion
    }
}
