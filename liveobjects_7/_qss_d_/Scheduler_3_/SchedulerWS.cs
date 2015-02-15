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
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace QS._qss_d_.Scheduler_3_
{
    /// <summary>
    /// Summary description for Scheduler
    /// </summary>
    [WebService(Namespace = "http://cluskong.cs.cornell.edu")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class SchedulerWS : WebService
    {
        public SchedulerWS()
        {
            //Uncomment the following line if using designed components 
            //InitializeComponent(); 

            scheduler = (IScheduler) Activator.GetObject(typeof(IScheduler), Scheduler.GenerateURL("localhost"));
        }

        private IScheduler scheduler;

        [WebMethod]
        public string[] GetTasks()
        {
            return scheduler.GetTasks();
        }

        [WebMethod]
        public string CreateTask(string name, string description, DateTime time, double priority, string executable, string arguments)
        {
            return scheduler.CreateTask(name, description, time, priority, executable, arguments).ID;
        }

        [WebMethod]
        public bool UploadFile(string taskID, string fileName, byte[] fileContents)
        {
            scheduler.GetTask(taskID).Upload(fileName, fileContents);
            return true;
        }

        [WebMethod]
        public bool GetTaskInfo(string taskID, out string name, out string description, out DateTime time, out double priority, 
            out string executable, out string arguments, out string status, out string[] files)
        {
            ITask task = scheduler.GetTask(taskID);
            name = task.Name;
            description = task.Description;
            time = task.Time;
            priority = task.Priority;
            executable = task.Executable;
            arguments = task.Arguments;
            status = task.Status.ToString();
            files = task.Files;
            return true;
        }

        [WebMethod]
        public bool SetTaskInfo(string taskID, string name, string description, DateTime time, double priority, string executable, string arguments)
        {
            ITask task = scheduler.GetTask(taskID);
            task.Name = name;
            task.Description = description;
            task.Time = time;
            task.Priority = priority;
            task.Executable = executable;
            task.Arguments = arguments;
            return true;
        }
    }
}
