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

#endregion

namespace QS._qss_d_.Scheduler_1_
{
    [System.Serializable]
    public class Request
    {
        public Request()
        {
        }

        public Request(string[] hostAddresses, Service_2_.ServiceDeploymentRequest[] deploymentRequests, 
            bool restartServicesOnStart, bool restartServicesBetweenExperiments, string executablePath, 
            Experiment[] experimentSpecifications)
        {
            this.hostAddresses = hostAddresses;
            this.deploymentRequests = deploymentRequests;
            this.restartServicesBetweenExperiments = restartServicesBetweenExperiments;
            this.restartServicesOnStart = restartServicesOnStart;
            this.executablePath = executablePath;
            this.experimentSpecifications = experimentSpecifications;
        }

        private bool restartServicesOnStart, restartServicesBetweenExperiments;
        private Service_2_.ServiceDeploymentRequest[] deploymentRequests;
        private string[] hostAddresses;
        private string executablePath;
        private Experiment[] experimentSpecifications;

        [System.Serializable]
        public class Experiment
        {
            public Experiment()
            {
            }

            public Experiment(string experimentClass, QS._core_c_.Components.AttributeSet experimentArguments)
            {
                this.experimentClass = experimentClass;
                this.experimentArguments = experimentArguments;
            }

            private string experimentClass;
            private QS._core_c_.Components.AttributeSet experimentArguments;

            public string ExperimentClass
            {
                get { return experimentClass; }
                set { experimentClass = value; }
            }

            public QS._core_c_.Components.AttributeSet ExperimentArguments
            {
                get { return experimentArguments; }
                set { experimentArguments = value; }
            }
        }

        public Experiment[] ExperimentSpecifications
        {
            get { return experimentSpecifications; }
            set { experimentSpecifications = value; }
        }

        public bool RestartServicesOnStart
        {
            get { return restartServicesOnStart; }
            set { restartServicesOnStart = value; }
        }

        public bool RestartServicesBetweenExperiments
        {
            get { return restartServicesBetweenExperiments; }
            set { restartServicesBetweenExperiments = value; }
        }

        public string[] HostAddresses
        {
            get { return hostAddresses; }
            set { hostAddresses = value; }
        }

        public Service_2_.ServiceDeploymentRequest[] DeploymentRequests
        {
            get { return deploymentRequests; }
            set { deploymentRequests = value; }
        }

        public string ExecutablePath
        {
            get { return executablePath; }
            set { executablePath = value; }
        }
    }
}
