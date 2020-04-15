using System;
using System.Threading.Tasks;

namespace Server
{
    public class TaskPollingTimer<T> : Timer
    {
        private readonly Task<T> m_Task;
        private readonly Action<T> m_Callback;

        public TaskPollingTimer(Task<T> task, Action<T> callback)
            : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
        {
            m_Task = task;
            m_Callback = callback;
        }

        protected override void OnTick()
        {
            if (m_Task.IsCompleted)
            {
                m_Callback(m_Task.Result);
                Stop();
            }
        }
    }
}
