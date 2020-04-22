using System;
using System.Collections.Generic;
using System.Reflection;

namespace Server
{
    public delegate void ValidationEventHandler();

    public static class ValidationQueue
    {
        public static event ValidationEventHandler StartValidation;
        public static void Initialize()
        {
            if (StartValidation != null)
                StartValidation();

            StartValidation = null;
        }
    }

    public static class ValidationQueue<T>
    {
        private static List<T> m_Queue;
        static ValidationQueue()
        {
            m_Queue = new List<T>();
            ValidationQueue.StartValidation += ValidateAll;
        }

        public static void Add(T obj)
        {
            m_Queue.Add(obj);
        }

        private static void ValidateAll()
        {
            Type type = typeof(T);

            if (type != null)
            {
                MethodInfo m = type.GetMethod("Validate", BindingFlags.Instance | BindingFlags.Public);

                if (m != null)
                {
                    for (int i = 0; i < m_Queue.Count; ++i)
                        m.Invoke(m_Queue[i], null);
                }
            }

            m_Queue.Clear();
            m_Queue = null;
        }
    }
}
