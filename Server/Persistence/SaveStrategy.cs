using System;

namespace Server
{
    public abstract class SaveStrategy
    {
        public abstract string Name { get; }
        public static SaveStrategy Acquire()
        {
            if (Core.MultiProcessor)
            {
                int processorCount = Core.ProcessorCount;

#if DynamicSaveStrategy
                if (processorCount > 2)
                {
                    return new DynamicSaveStrategy();
                }
#else
                if (processorCount > 16)
                {
                    return new ParallelSaveStrategy(processorCount);
                }
#endif
                else
                {
                    return new DualSaveStrategy();
                }
            }
            else
            {
                return new StandardSaveStrategy();
            }
        }

        public abstract void Save(SaveMetrics metrics, bool permitBackgroundWrite);

        public abstract void ProcessDecay();
    }
}