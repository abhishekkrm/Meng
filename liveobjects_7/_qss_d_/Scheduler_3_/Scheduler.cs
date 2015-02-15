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
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Diagnostics;

namespace QS._qss_d_.Scheduler_3_
{
    public class Scheduler : MarshalByRefObject, IScheduler, IDisposable
    {
        #region Initialization and Cleanup

        public static IScheduler GetClient(string hostname)
        {
            return (IScheduler)Activator.GetObject(typeof(IScheduler), Scheduler.GenerateURL(hostname));
        }

        private const int DefaultPort = 65509;
        private const string DefaultURI = "Scheduler.soap";

        public static string GenerateURL(string hostname)
        {
            return "http://" + hostname + ":" + DefaultPort.ToString() + "/" + DefaultURI;
        }

        public Scheduler(Configuration configuration)
        {
            logger = new QS._core_c_.Base.Logger(QS._core_c_.Base2.PreciseClock.Clock, true);

            this.root = configuration.Root;

            guiThread = new Thread(new ThreadStart(this.GuiMain));
            guiThread.Start();
            guiStarted.WaitOne();

            try
            {
                RemotingConfiguration.Configure(Process.GetCurrentProcess().MainModule.FileName + ".config", false);
            }
            catch (Exception)
            {
            }

            ChannelServices.RegisterChannel(channel = new HttpChannel(DefaultPort), false);

            myref = RemotingServices.Marshal(this, DefaultURI, typeof(IScheduler));

            logger.Console = console.Console;

            if (Directory.Exists(root))
            {
                foreach (string path in Directory.GetDirectories(root))
                {
                    string id = path.Substring(path.LastIndexOf("\\") + 1);
                    try
                    {                        
                        TaskController task = Load(id);
                        tasks.Add(id, task);
                    }
                    catch (Exception exc)
                    {
                        logger.Log(this, "Cannot load task \"" + id + "\".\n" + exc.ToString());
                    }
                }
            }
            else
                Directory.CreateDirectory(root);

            Notify();

            schedulerThread = new Thread(new ThreadStart(SchedulerLoop));
            schedulerThread.Start();
        }

        private void GuiMain()
        {
            console = new SchedulerConsole(this);
            guiStarted.Set();
            System.Windows.Forms.Application.Run(console);
        }

        #endregion

        private HttpChannel channel;
        private QS._core_c_.Base.Logger logger;
        private ManualResetEvent guiStarted = new ManualResetEvent(false);
        private Thread guiThread, schedulerThread;
        private SchedulerConsole console;
        private ObjRef myref;
        private string root;
        private IDictionary<string, TaskController> tasks = new Dictionary<string, TaskController>();

        private bool enabled = true, finished;
        private AutoResetEvent recheck = new AutoResetEvent(false);
        private TaskController current;
        private event EventHandler tasksChanged;

        public event EventHandler OnChange
        {
            add { tasksChanged += value; }
            remove { tasksChanged -= value; }
        }
        
        #region Scheduling

        private void SchedulerLoop()
        {
            while (!finished)
            {
                if (enabled)
                {
                    TaskController task = Schedule();
                    if (task != null)
                    {
                        lock (task)
                        {
                            if (task.Task.Status == TaskStatus.Pending)
                            {
                                task.Task.Status = TaskStatus.Running;
                                Notify();
                                RunTask(task);
                                continue;
                            }
                        }
                    }
                }

                Notify();
                recheck.WaitOne();
            }
        }

        private TaskController Schedule()
        {
            TaskController selectedTask = null;
            lock (this)
            {
                foreach (TaskController task in tasks.Values)
                {
                    switch (task.Task.Status)
                    {
                        case TaskStatus.Running:
                            {
                                lock (task)
                                {
                                    task.Task.Status = TaskStatus.Aborted;
                                    task.Logger.Log(this, "Task was detected abandoned.");
                                    Save(task);
                                }
                            }
                            break;

                        case TaskStatus.Pending:
                            {
                                if ((selectedTask == null || ((IComparable<TaskController>)task).CompareTo(selectedTask) < 0))                                
                                    selectedTask = task;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            return selectedTask;
        }

        private void RunTask(TaskController task)
        {
            current = task;

            Process process = new Process();
            string working_directory = root + "\\" + task.ID + "\\files\\";
            if (!Directory.Exists(working_directory))
                Directory.CreateDirectory(working_directory);
            process.StartInfo.FileName = task.Task.Executable;
            process.StartInfo.WorkingDirectory = working_directory;
            process.StartInfo.Arguments = task.Task.Arguments;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardError = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(
                delegate(object obj, EventArgs args)
                {
                    lock (current)
                    {
                        if (current.Task.Status == TaskStatus.Running)
                        {
                            if (process.ExitCode != 0)
                                current.Task.Status = TaskStatus.Failed;
                            else
                                current.Task.Status = TaskStatus.Completed;
                            Save(current);
                        }
                        recheck.Set();
                    }
                });
            process.Start();

            do
            {
                Monitor.Exit(task);
                recheck.WaitOne();
                Monitor.Enter(task);
            }
            while (!finished && task.Task.Status == TaskStatus.Running);

            if (!process.HasExited)
                process.Kill();
        }

        public void AbortTask(TaskController task)
        {
            lock (task)
            {
                if (task.Task.Status == TaskStatus.Running)
                {
                    task.Task.Status = TaskStatus.Aborted;
                    task.Logger.Log(this, "The user has aborted the task.");
                    Save(task);

                    if (ReferenceEquals(task, current))
                        recheck.Set();
                }
                else
                    throw new Exception("Cannot abort this task, the task is not running.");
            }
        }

        public void EnabledTask(TaskController task)
        {
            recheck.Set();
        }

        #endregion

        #region Accessors

        public string Root
        {
            get { return root; }
        }

        #endregion

        #region Helpers

        private string GenerateID()
        {
            return DateTime.Now.ToString("yyMMddHHmmssff");
        }

        private void Notify()
        {
            if (tasksChanged != null)
                tasksChanged(null, null);
        }

        #endregion

        #region IScheduler Members

        bool IScheduler.Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        ITask IScheduler.CreateTask(string name, string description, DateTime time, double priority, string executable, string arguments)
        {
            TaskController task;
            lock (this)
            {
                string id = GenerateID();
                string taskroot;
                while (tasks.ContainsKey(id) || Directory.Exists((taskroot = root + "\\" + id)))
                {
                    Thread.Sleep(1000);
                    id = GenerateID();
                }
                Directory.CreateDirectory(taskroot);                
                task = new TaskController(this, id, name, description, time, priority, executable, arguments);
                Save(task);
                tasks.Add(id, task);                
            }
            Notify();
            return task;
        }

        ITask IScheduler.GetTask(string id)
        {
            return tasks[id];
        }

        string[] IScheduler.GetTasks()
        {
            List<string> ids = new List<string>();
            foreach (ITask task in tasks.Values)
                ids.Add(task.ID);
            return ids.ToArray();
        }

        #endregion

        #region Loading and Saving

        public void Save(TaskController task)
        {
            using (StreamWriter writer = new StreamWriter(root + "\\" + task.ID + "\\task.xml"))
            {
                (new XmlSerializer(typeof(Task))).Serialize(writer, task.Task);
            }
        }

        public TaskController Load(string id)
        {
            Task task;
            using (StreamReader reader = new StreamReader(root + "\\" + id + "\\task.xml"))
            {
                task = (Task) (new XmlSerializer(typeof(Task))).Deserialize(reader);
            }

            return new TaskController(this, id, task);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            finished = true;
            recheck.Set();
            RemotingServices.Unmarshal(myref);
            ChannelServices.UnregisterChannel(channel);
            if (!schedulerThread.Join(3000))
            {
                schedulerThread.Abort();
                schedulerThread.Join(1000);
            }
            guiThread.Abort();
            guiThread.Join(1000);
        }

        #endregion
    }
}
