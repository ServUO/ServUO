using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Server.Commands;

namespace Server
{
    public static class TimerRegistry
    {
        public static readonly bool Debug = false;
        private const int _RegistryThreshold = 250;

        public static void Initialize()
        {
            CommandSystem.Register("CheckTimers", AccessLevel.Administrator, e =>
            {
                foreach (var kvp in Timers)
                {
                    Utility.WriteConsoleColor(ConsoleColor.Green, "Timer ID: {0}", kvp.Key);

                    int elements = 0;
                    int timerCount = kvp.Value.Count;

                    for (int i = 0; i < kvp.Value.Count; i++)
                    {
                        Console.WriteLine("Delay/Interval: {0}", kvp.Value[i].Interval);
                        Console.WriteLine("Timer Priority: {0}", kvp.Value[i].Priority);

                        var dic = kvp.Value[i].GetType().GetProperty("Registry").GetValue(kvp.Value[i], null) as IDictionary;

                        if (dic != null)
                        {
                            elements += dic.Count;
                        }
                    }

                    Console.WriteLine("Timers: {0}", timerCount);
                    Console.WriteLine("Registered elements: {0}!", elements);
                }
            });
        }

        public static Dictionary<string, List<Timer>> Timers { get; set; } = new Dictionary<string, List<Timer>>();

        public static void Register<T>(string id, T instance, TimeSpan duration, Action<T> callback)
        {
            Register(id, instance, duration, TimeSpan.Zero, true, true, callback);
        }

        public static void Register<T>(string id, T instance, TimeSpan duration, TimerPriority? priority, Action<T> callback)
        {
            Register(id, instance, duration, TimeSpan.Zero, true, true, priority, callback);
        }

        public static void Register<T>(string id, T instance, TimeSpan duration, TimeSpan delay, Action<T> callback)
        {
            Register(id, instance, duration, delay, true, true, null, callback);
        }

        public static void Register<T>(string id, T instance, TimeSpan duration, TimeSpan delay, TimerPriority? priority, Action<T> callback)
        {
            Register(id, instance, duration, delay, true, true, priority, callback);
        }

        public static void Register<T>(string id, T instance, TimeSpan duration, bool removeOnExpire, Action<T> callback)
        {
            Register(id, instance, duration, TimeSpan.Zero, removeOnExpire, true, callback);
        }

        public static void Register<T>(string id, T instance, TimeSpan duration, bool removeOnExpire, TimerPriority? priority, Action<T> callback)
        {
            Register(id, instance, duration, TimeSpan.Zero, removeOnExpire, true, priority, callback);
        }

        public static void Register<T>(string id, T instance, TimeSpan duration, TimeSpan delay, bool removeOnExpire, Action<T> callback)
        {
            Register(id, instance, duration, delay, removeOnExpire, true, null, callback);
        }

        public static void Register<T>(string id, T instance, TimeSpan duration, TimeSpan delay, bool removeOnExpire, TimerPriority? priority, Action<T> callback)
        {
            Register(id, instance, duration, delay, removeOnExpire, true, priority, callback);
        }

        public static void Register<T>(string id, T instance, TimeSpan duration, TimeSpan delay, bool removeOnExpire, bool checkDeleted, Action<T> callback)
        {
            Register(id, instance, duration, delay, removeOnExpire, true, null, callback);
        }

        public static void Register<T>(string id, T instance, TimeSpan duration, TimeSpan delay, bool removeOnExpire, bool checkDeleted, TimerPriority? priority, Action<T> callback)
        {
            if (HasTimer(id, instance))
            {
                return;
            }

            var timer = GetTimer(id, instance, true);

            if(Debug) Console.WriteLine("Registering: {0} - {1}...", id, instance);

            if (timer == null)
            {
                if (Debug) Console.WriteLine("Timer not Found, creating new one...");
                timer = new RegistryTimer<T>(delay == TimeSpan.Zero ? ProcessDelay(duration) : delay, callback, removeOnExpire, checkDeleted, priority);

                Timers[id].Add(timer);
                timer.Start();
            }
            else if(Debug)
            {
                Console.WriteLine("Timer Found, adding to existing registry...");
            }

            if (!timer.Registry.ContainsKey(instance))
            {
                if (Debug) Console.WriteLine("Adding {0} to the timer registry!", instance);
                timer.Registry[instance] = Core.TickCount + (long)duration.TotalMilliseconds;
            }
            else if (Debug)
            {
                Console.WriteLine("Instnace already exists in the timer registry!");
            }
        }

        public static void RemoveFromRegistry<T>(string id, T instance)
        {
            var timer = GetTimerFor(id, instance);

            if (timer != null && timer.Registry.ContainsKey(instance))
            {
                timer.Registry.Remove(instance);
                if (Debug) Console.WriteLine("Removing {0} from the registry", instance);

                if (timer.Registry.Count == 0)
                {
                    UnregisterTimer(timer);
                }
            }
        }

        public static bool UpdateRegistry<T>(string id, T instance, TimeSpan duration)
        {
            var timer = GetTimerFor(id, instance);

            if (Debug) Console.WriteLine("Updating Registry for {0} - {1}...", id, instance);
            if (timer != null)
            {
                if (Debug) Console.WriteLine("Complete!");
                timer.Registry[instance] = Core.TickCount + (long)duration.TotalMilliseconds;
                return true;
            }

            if (Debug)
            {
                Console.WriteLine("Failed, timer not found");
            }

            return false;
        }

        public static RegistryTimer<T> GetTimer<T>(string id, T instance, bool create)
        {
            if (Timers.ContainsKey(id))
            {
                return Timers[id].FirstOrDefault(t => t is RegistryTimer<T> regTimer && regTimer.Registry.Count < _RegistryThreshold) as RegistryTimer<T>;
            }

            if (create)
            {
                Timers[id] = new List<Timer>();
            }

            return null;
        }

        public static RegistryTimer<T> GetTimerFor<T>(string id, T instance)
        {
            if (Timers.ContainsKey(id))
            {
                return Timers[id].FirstOrDefault(t => t is RegistryTimer<T> timer && timer.Registry.ContainsKey(instance)) as RegistryTimer<T>;
            }

            return null;
        }

        public static bool HasTimer<T>(string id, T instance)
        {
            return GetTimerFor(id, instance) != null;
        }

        public static void UnregisterTimer(Timer timer)
        {
            timer.Stop();
            var id = GetTimerID(timer);

            if (!string.IsNullOrEmpty(id) && Timers.ContainsKey(id))
            {
                Timers[id].Remove(timer);

                if (Timers[id].Count == 0)
                {
                    if (Debug) Console.WriteLine("Remove {0} from the timer list", id);
                    Timers.Remove(id);
                }
            }
        }

        public static string GetTimerID(Timer timer)
        {
            foreach (var kvp in Timers)
            {
                if (kvp.Value.Any(t => t == timer))
                {
                    return kvp.Key;
                }
            }

            return string.Empty;
        }

        public static TimeSpan ProcessDelay(TimeSpan duration)
        {
            var seconds = duration.TotalSeconds;

            if (seconds >= 86400) // 1 day
            {
                return TimeSpan.FromMinutes(5);
            }

            if (seconds >= 3600) // 1 hour
            {
                return TimeSpan.FromMinutes(1);
            }

            if (seconds >= 600) // 10 minutes
            {
                return TimeSpan.FromSeconds(1);
            }

            var mils = duration.TotalMilliseconds;

            if (mils < 10)
            {
                return TimeSpan.Zero;
            }

            if (mils < 250)
            {
                return TimeSpan.FromMilliseconds(10);
            }

            if (mils < 500)
            {
                return TimeSpan.FromMilliseconds(250);
            }

            return TimeSpan.FromMilliseconds(500);
        }
    }

    public class RegistryTimer<T> : Timer
    {
        public Dictionary<T, long> Registry { get; set; } = new Dictionary<T, long>();

        public Action<T> Callback { get; set; }
        public bool RemoveOnExpire { get; set; }
        public bool CheckDeleted { get; set; }

        public RegistryTimer(TimeSpan delay, Action<T> callback, bool removeOnExpire, bool checkDeleted, TimerPriority? priority)
            : base(delay, delay)
        {
            Callback = callback;
            RemoveOnExpire = removeOnExpire;
            CheckDeleted = checkDeleted;

            if (priority != null)
            {
                Priority = (TimerPriority)priority;
            }
        }

        protected override void OnTick()
        {
            List<T> instances = Registry.Keys.Where(v => Registry[v] <= Core.TickCount || (CheckDeleted && v is IEntity e && e.Deleted)).ToList();

            for (int i = 0; i < instances.Count; i++)
            {
                var instance = instances[i];

                if (IsDeleted(instance))
                {
                    if (TimerRegistry.Debug) Console.WriteLine("Removing from Registry [Deleted]: {0}", instance);
                    Registry.Remove(instance);
                }
                else
                {
                    if (Callback != null)
                    {
                        Callback(instance);
                    }

                    if (IsExpired(instance))
                    {
                        if (TimerRegistry.Debug) Console.WriteLine("Removing from Registry [Processed]: {0}", instance);
                        Registry.Remove(instance);
                    }
                }
            }

            ColUtility.Free(instances);

            if (Registry.Count == 0)
            {
                TimerRegistry.UnregisterTimer(this);
            }
        }

        private bool IsExpired(T instance)
        {
            return RemoveOnExpire && (!Registry.ContainsKey(instance) || Registry[instance] <= Core.TickCount);
        }

        private bool IsDeleted(T instance)
        {
            return CheckDeleted && instance is IEntity e && e.Deleted;
        }
    }
}
