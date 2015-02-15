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

﻿#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

// using NUnit.Framework;

#endregion

/*
namespace QS.TMS.NUnit
{
    [TestFixture]
    public abstract class GenericTest
    {
        private const uint defaultNumberOfNodes = 20;

        public GenericTest()
        {
        }

        private QS.TMS.Runtime.IEnvironment environment;
        private QS.CMS.Base.Logger logger;

        protected virtual QS.TMS.Runtime.IEnvironment CreateEnvironment(QS.Fx.Logging.ILogger logger)
        {
            return new QS.TMS.Environments.EmulatedEnvironment(logger, defaultNumberOfNodes, true);
        }

        protected QS.TMS.Runtime.IEnvironment Environment
        {
            get { return environment; }
        }

        protected void RunExperiment(ExperimentCallback experimentCallback)
        {
            this.RunExperiment(experimentCallback, QS.CMS.Components.AttributeSet.None);
        }

        protected void RunExperiment<T>(ExperimentCallback experimentCallback, string name, T argument)
        {
            this.RunExperiment(experimentCallback, new QS.CMS.Components.AttributeSet(name, argument));
        }

        protected void RunExperiment(ExperimentCallback experimentCallback, QS.CMS.Components.AttributeSet args)
        {
            using (Experiment experiment = new Experiment(experimentCallback))
            {
                QS.CMS.Base.Logger logger = new QS.CMS.Base.Logger(true);
                QS.CMS.Components.AttributeSet resultSet = new QS.CMS.Components.AttributeSet(20);
                experiment.run(environment, logger, args, resultSet);
                foreach (CMS.Collections.IDictionaryEntry dic_en in resultSet.Attributes)
                {
                    logger.Log(this, "Result_Attribute: \"" + dic_en.Key.ToString() + "\" = " +
                        ((dic_en.Value != null) ? dic_en.Value.ToString() : "(null)") + "\n");
                }
            }
        }

        protected void RunExperiment(System.Type experimentClass)
        {
            this.RunExperiment(experimentClass, QS.CMS.Components.AttributeSet.None);
        }

        protected void RunExperiment(System.Type experimentClass, QS.CMS.Components.AttributeSet args)
        {
            try
            {
                using (QS.TMS.Experiments.IExperiment experiment = (QS.TMS.Experiments.IExperiment)
                    experimentClass.GetConstructor(Type.EmptyTypes).Invoke(new object[] {}))
                {
                    QS.CMS.Components.AttributeSet resultSet = new QS.CMS.Components.AttributeSet(20);
                    experiment.run(this.Environment, logger, args, resultSet);
                    foreach (CMS.Collections.IDictionaryEntry dic_en in resultSet.Attributes)
                    {
                        logger.Log(this, "Result_Attribute: \"" + dic_en.Key.ToString() + "\" = " +
                            ((dic_en.Value != null) ? dic_en.Value.ToString() : "(null)") + "\n");
                    }
                }
            }
            catch (Exception exc)
            {
                global::NUnit.Framework.Assert.Fail("Experiment failed: " + exc.ToString() + "; " + exc.StackTrace);
            }
        }

        [TestFixtureSetUp]
        public void SetUp()
        {
            logger = new QS.CMS.Base.Logger(false, 
                new CMS.Base.ConsoleWrapper(new CMS.Base.WriteLineCallback(Console.WriteLine)));
            environment = CreateEnvironment(logger);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            environment.Dispose();
        }

        protected delegate void ExperimentCallback(Runtime.IEnvironment environment, QS.Fx.Logging.ILogger logger, 
            QS.CMS.Components.IAttributeSet args, QS.CMS.Components.IAttributeSet resultAttributes);

        protected class Experiment : TMS.Experiments.IExperiment
        {
            public Experiment(ExperimentCallback callback)
            {
                this.callback = callback;
            }

            private ExperimentCallback callback;

            public void run(Runtime.IEnvironment environment, QS.Fx.Logging.ILogger logger,
                QS.CMS.Components.IAttributeSet args, QS.CMS.Components.IAttributeSet results)
            {
                callback(environment, logger, args, results);
            }

            public void Dispose()
            {
            }
        }
    }
}
*/
