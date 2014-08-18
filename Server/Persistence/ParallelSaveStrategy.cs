using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CustomsFramework;
using Server.Guilds;

namespace Server
{
    public sealed class ParallelSaveStrategy : SaveStrategy
    {
        private readonly int processorCount;
        private readonly Queue<Item> _decayQueue;
        private SaveMetrics metrics;
        private SequentialFileWriter itemData, itemIndex;
        private SequentialFileWriter mobileData, mobileIndex;
        private SequentialFileWriter guildData, guildIndex;
        private SequentialFileWriter customData, customIndex;
        private Consumer[] consumers;
        private int cycle;
        private bool finished;
        public ParallelSaveStrategy(int processorCount)
        {
            this.processorCount = processorCount;

            this._decayQueue = new Queue<Item>();
        }

        public override string Name
        {
            get
            {
                return "Parallel";
            }
        }
        public override void Save(SaveMetrics metrics, bool permitBackgroundWrite)
        {
            this.metrics = metrics;

            this.OpenFiles();

            this.consumers = new Consumer[this.GetThreadCount()];

            for (int i = 0; i < this.consumers.Length; ++i)
            {
                this.consumers[i] = new Consumer(this, 256);
            }

            IEnumerable<ISerializable> collection = new Producer();

            foreach (ISerializable value in collection)
            {
                while (!this.Enqueue(value))
                {
                    if (!this.Commit())
                    {
                        Thread.Sleep(0);
                    }
                }
            }

            this.finished = true;

            this.SaveTypeDatabases();

            WaitHandle.WaitAll(
                Array.ConvertAll<Consumer, WaitHandle>(
                    this.consumers,
                    delegate(Consumer input)
                    {
                        return input.completionEvent;
                    }));

            this.Commit();

            this.CloseFiles();
        }

        public override void ProcessDecay()
        {
            while (this._decayQueue.Count > 0)
            {
                Item item = this._decayQueue.Dequeue();

                if (item.OnDecay())
                {
                    item.Delete();
                }
            }
        }

        private int GetThreadCount()
        {
            return this.processorCount - 1;
        }

        private void SaveTypeDatabases()
        {
            this.SaveTypeDatabase(World.ItemTypesPath, World.m_ItemTypes);
            this.SaveTypeDatabase(World.MobileTypesPath, World.m_MobileTypes);
            this.SaveTypeDatabase(World.DataTypesPath, World._DataTypes);
        }

        private void SaveTypeDatabase(string path, List<Type> types)
        {
            BinaryFileWriter bfw = new BinaryFileWriter(path, false);

            bfw.Write(types.Count);

            foreach (Type type in types)
            {
                bfw.Write(type.FullName);
            }

            bfw.Flush();

            bfw.Close();
        }

        private void OpenFiles()
        {
            this.itemData = new SequentialFileWriter(World.ItemDataPath, this.metrics);
            this.itemIndex = new SequentialFileWriter(World.ItemIndexPath, this.metrics);

            this.mobileData = new SequentialFileWriter(World.MobileDataPath, this.metrics);
            this.mobileIndex = new SequentialFileWriter(World.MobileIndexPath, this.metrics);

            this.guildData = new SequentialFileWriter(World.GuildDataPath, this.metrics);
            this.guildIndex = new SequentialFileWriter(World.GuildIndexPath, this.metrics);

            this.customData = new SequentialFileWriter(World.DataBinaryPath, this.metrics);
            this.customIndex = new SequentialFileWriter(World.DataIndexPath, this.metrics);

            this.WriteCount(this.itemIndex, World.Items.Count);
            this.WriteCount(this.mobileIndex, World.Mobiles.Count);
            this.WriteCount(this.guildIndex, BaseGuild.List.Count);
            this.WriteCount(this.customIndex, World.Data.Count);
        }

        private void WriteCount(SequentialFileWriter indexFile, int count)
        {
            byte[] buffer = new byte[4];

            buffer[0] = (byte)(count);
            buffer[1] = (byte)(count >> 8);
            buffer[2] = (byte)(count >> 16);
            buffer[3] = (byte)(count >> 24);

            indexFile.Write(buffer, 0, buffer.Length);
        }

        private void CloseFiles()
        {
            this.itemData.Close();
            this.itemIndex.Close();

            this.mobileData.Close();
            this.mobileIndex.Close();

            this.guildData.Close();
            this.guildIndex.Close();

            this.customData.Close();
            this.customIndex.Close();

            World.NotifyDiskWriteComplete();
        }

        private void OnSerialized(ConsumableEntry entry)
        {
            ISerializable value = entry.value;
            BinaryMemoryWriter writer = entry.writer;

            Item item = value as Item;

            if (item != null)
                this.Save(item, writer);
            else
            {
                Mobile mob = value as Mobile;

                if (mob != null)
                    this.Save(mob, writer);
                else
                {
                    BaseGuild guild = value as BaseGuild;

                    if (guild != null)
                        this.Save(guild, writer);
                    else
                    {
                        SaveData data = value as SaveData;

                        if (data != null)
                            this.Save(data, writer);
                    }
                }
            }
        }

        private void Save(Item item, BinaryMemoryWriter writer)
        {
            int length = writer.CommitTo(this.itemData, this.itemIndex, item.m_TypeRef, item.Serial);

            if (this.metrics != null)
            {
                this.metrics.OnItemSaved(length);
            }

            if (item.Decays && item.Parent == null && item.Map != Map.Internal && DateTime.UtcNow > (item.LastMoved + item.DecayTime))
            {
                this._decayQueue.Enqueue(item);
            }
        }

        private void Save(Mobile mob, BinaryMemoryWriter writer)
        {
            int length = writer.CommitTo(this.mobileData, this.mobileIndex, mob.m_TypeRef, mob.Serial);

            if (this.metrics != null)
            {
                this.metrics.OnMobileSaved(length);
            }
        }

        private void Save(BaseGuild guild, BinaryMemoryWriter writer)
        {
            int length = writer.CommitTo(this.guildData, this.guildIndex, 0, guild.Id);

            if (this.metrics != null)
            {
                this.metrics.OnGuildSaved(length);
            }
        }

        private void Save(SaveData data, BinaryMemoryWriter writer)
        {
            int length = writer.CommitTo(this.customData, this.customIndex, data._TypeID, data.Serial);

            if (this.metrics != null)
                this.metrics.OnDataSaved(length);
        }

        private bool Enqueue(ISerializable value)
        {
            for (int i = 0; i < this.consumers.Length; ++i)
            {
                Consumer consumer = this.consumers[this.cycle++ % this.consumers.Length];

                if ((consumer.tail - consumer.head) < consumer.buffer.Length)
                {
                    consumer.buffer[consumer.tail % consumer.buffer.Length].value = value;
                    consumer.tail++;

                    return true;
                }
            }

            return false;
        }

        private bool Commit()
        {
            bool committed = false;

            for (int i = 0; i < this.consumers.Length; ++i)
            {
                Consumer consumer = this.consumers[i];

                while (consumer.head < consumer.done)
                {
                    this.OnSerialized(consumer.buffer[consumer.head % consumer.buffer.Length]);
                    consumer.head++;

                    committed = true;
                }
            }

            return committed;
        }

        private struct ConsumableEntry
        {
            public ISerializable value;
            public BinaryMemoryWriter writer;
        }

        private sealed class Producer : IEnumerable<ISerializable>
        {
            private readonly IEnumerable<Item> items;
            private readonly IEnumerable<Mobile> mobiles;
            private readonly IEnumerable<BaseGuild> guilds;
            private readonly IEnumerable<SaveData> data;
            public Producer()
            {
                this.items = World.Items.Values;
                this.mobiles = World.Mobiles.Values;
                this.guilds = BaseGuild.List.Values;
                this.data = World.Data.Values;
            }

            public IEnumerator<ISerializable> GetEnumerator()
            {
                foreach (Item item in this.items)
                    yield return item;

                foreach (Mobile mob in this.mobiles)
                    yield return mob;

                foreach (BaseGuild guild in this.guilds)
                    yield return guild;

                foreach (SaveData data in this.data)
                    yield return data;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class Consumer
        {
            public readonly ManualResetEvent completionEvent;
            public readonly ConsumableEntry[] buffer;
            public int head, done, tail;
            private readonly ParallelSaveStrategy owner;
            private readonly Thread thread;
            public Consumer(ParallelSaveStrategy owner, int bufferSize)
            {
                this.owner = owner;

                this.buffer = new ConsumableEntry[bufferSize];

                for (int i = 0; i < this.buffer.Length; ++i)
                {
                    this.buffer[i].writer = new BinaryMemoryWriter();
                }

                this.completionEvent = new ManualResetEvent(false);

                this.thread = new Thread(Processor);

                this.thread.Name = "Parallel Serialization Thread";

                this.thread.Start();
            }

            private void Processor()
            {
                try
                {
                    while (!this.owner.finished)
                    {
                        this.Process();
                        Thread.Sleep(0);
                    }

                    this.Process();

                    this.completionEvent.Set();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            private void Process()
            {
                ConsumableEntry entry;

                while (this.done < this.tail)
                {
                    entry = this.buffer[this.done % this.buffer.Length];

                    entry.value.Serialize(entry.writer);

                    ++this.done;
                }
            }
        }
    }
}