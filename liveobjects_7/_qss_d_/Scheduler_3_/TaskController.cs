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

namespace QS._qss_d_.Scheduler_3_
{
    public class TaskController : MarshalByRefObject, ITask, IComparable<TaskController>
    {
        public TaskController(
            Scheduler owner, string id, string name, string description, DateTime time, double priority, string executable, string arguments)
            : this(owner, id, new Task(name, description, time, priority, executable, arguments))
        {
        }

        public TaskController(Scheduler owner, string id, Task task)
        {
            this.owner = owner;
            this.id = id;
            this.task = task;
        }

        private Scheduler owner;
        private string id;
        private Task task;
        private QS._qss_d_.Components_.FileReadableLogger logger;
        
        #region Accessors

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        public Task Task
        {
            get { return task; }
            set { task = value; }
        }

        public QS._core_c_.Base.IReadableLogger Logger
        {
            get
            {
                if (logger == null)
                    logger = new QS._qss_d_.Components_.FileReadableLogger(owner.Root + "\\" + id + "\\log.txt");
                return logger;
            }
        }

        #endregion

        #region Helpers

        private bool CanModify
        {
            get { return task.Status == TaskStatus.Pending || task.Status == TaskStatus.Disabled; }
        }

        private void CheckModify()
        {
            if (!CanModify)
                throw new Exception("Cannot modify this task, the task is either running, it has completed or it was aborted.");
        }

        private void Save()
        {
            owner.Save(this);
        }

        #endregion

        #region ITask Members

        void ITask.Enable()
        {
            lock (this)
            {
                if (task.Status == TaskStatus.Disabled)
                {
                    task.Status = TaskStatus.Pending;
                    Save();
                }
                else
                    throw new Exception("Cannot enable, this task is either already running, or it has completed, crashed or was aborted.");
            }

            owner.EnabledTask(this);
        }

        void ITask.Disable()
        {
            lock (this)
            {
                if (task.Status == TaskStatus.Pending)
                {
                    task.Status = TaskStatus.Disabled;
                    Save();
                }
                else
                    throw new Exception("Cannot disable, this task is either already running, or it has completed, crashed or was aborted.");
            }
        }

        void ITask.Abort()
        {
            owner.AbortTask(this);
        }

        string ITask.Log
        {
            get
            {
                lock (this)
                {
                    return ((QS._core_c_.Base.IOutputReader)logger).CurrentContents;
                }
            }
        }

        void ITask.Upload(string name, byte[] contents)
        {
            lock (this)
            {
                CheckModify();

                string filesdir = owner.Root + "\\" + id + "\\files\\";
                if (!Directory.Exists(filesdir))
                    Directory.CreateDirectory(filesdir);

                using (FileStream stream = new FileStream(filesdir + name, FileMode.Create, FileAccess.Write))
                {
                    stream.Write(contents, 0, contents.Length);
                }
                task.AddFile(name);

                Save();
            }
        }

        string[] ITask.Files
        {
            get { return task.Files; }
        }

        TaskStatus ITask.Status
        {
            get { return task.Status; }
        }

        string ITask.ID
        {
            get { return id; }
        }

        string ITask.Name
        {
            get { return task.Name; }
            set 
            {
                lock (this)
                {
                    task.Name = value;
                    Save();
                }
            }
        }

        string ITask.Description
        {
            get { return task.Description; }
            set
            {
                lock (this)
                {
                    task.Description = value;
                    Save();
                }
            }
        }

        string ITask.Executable
        {
            get { return task.Executable; }
            set
            {
                lock (this)
                {
                    CheckModify();
                    task.Executable = value;
                    Save();
                }
            }
        }

        string ITask.Arguments
        {
            get { return task.Arguments; }
            set 
            {
                lock (this)
                {
                    CheckModify();
                    task.Arguments = value;
                    Save();
                }
            }
        }

        DateTime ITask.Time
        {
            get { return task.Time; }
            set 
            {
                lock (this)
                {
                    CheckModify();
                    task.Time = value;
                    Save();
                }
            }
        }

        double ITask.Priority
        {
            get { return task.Priority; }
            set 
            {
                lock (this)
                {
                    CheckModify();
                    task.Priority = value;
                    Save();
                }
            }
        }

        #endregion

        #region IComparable<TaskController> Members

        int IComparable<TaskController>.CompareTo(TaskController other)
        {
            int result = ((IComparable<Task>) task).CompareTo(other.task);
            if (result != 0)
                return result;
            return id.CompareTo(other.id);
        }

        #endregion
    }
}
