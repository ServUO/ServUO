using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomsFramework;
using Server.Guilds;

namespace Server
{
    public sealed class DynamicSaveStrategy : SaveStrategy
    {
        private readonly ConcurrentBag<Item> _decayBag;
        private readonly BlockingCollection<QueuedMemoryWriter> _itemThreadWriters;
        private readonly BlockingCollection<QueuedMemoryWriter> _mobileThreadWriters;
        private readonly BlockingCollection<QueuedMemoryWriter> _guildThreadWriters;
        private readonly BlockingCollection<QueuedMemoryWriter> _dataThreadWriters;
        private SaveMetrics _metrics;
        private SequentialFileWriter _itemData, _itemIndex;
        private SequentialFileWriter _mobileData, _mobileIndex;
        private SequentialFileWriter _guildData, _guildIndex;
        private SequentialFileWriter _customData, _customIndex;
        public DynamicSaveStrategy()
        {
            this._decayBag = new ConcurrentBag<Item>();
            this._itemThreadWriters = new BlockingCollection<QueuedMemoryWriter>();
            this._mobileThreadWriters = new BlockingCollection<QueuedMemoryWriter>();
            this._guildThreadWriters = new BlockingCollection<QueuedMemoryWriter>();
            this._dataThreadWriters = new BlockingCollection<QueuedMemoryWriter>();
        }

        public override string Name
        {
            get
            {
                return "Dynamic";
            }
        }
        public override void Save(SaveMetrics metrics, bool permitBackgroundWrite)
        {
            this._metrics = metrics;

            this.OpenFiles();

            Task[] saveTasks = new Task[4];

            saveTasks[0] = this.SaveItems();
            saveTasks[1] = this.SaveMobiles();
            saveTasks[2] = this.SaveGuilds();
            saveTasks[3] = this.SaveData();

            this.SaveTypeDatabases();

            if (permitBackgroundWrite)
            {
                //This option makes it finish the writing to disk in the background, continuing even after Save() returns.
                Task.Factory.ContinueWhenAll(saveTasks, _ =>
                {
                    this.CloseFiles();

                    World.NotifyDiskWriteComplete();
                });
            }
            else
            {
                Task.WaitAll(saveTasks);	//Waits for the completion of all of the tasks(committing to disk)
                this.CloseFiles();
            }
        }

        public override void ProcessDecay()
        {
            Item item;

            while (this._decayBag.TryTake(out item))
            {
                if (item.OnDecay())
                {
                    item.Delete();
                }
            }
        }

        private Task StartCommitTask(BlockingCollection<QueuedMemoryWriter> threadWriter, SequentialFileWriter data, SequentialFileWriter index)
        {
            Task commitTask = Task.Factory.StartNew(() =>
            {
                while (!(threadWriter.IsCompleted))
                {
                    QueuedMemoryWriter writer;

                    try
                    {
                        writer = threadWriter.Take();
                    }
                    catch (InvalidOperationException)
                    {
                        //Per MSDN, it's fine if we're here, successful completion of adding can rarely put us into this state.
                        break;
                    }

                    writer.CommitTo(data, index);
                }
            });

            return commitTask;
        }

        private Task SaveItems()
        {
            //Start the blocking consumer; this runs in background.
            Task commitTask = this.StartCommitTask(this._itemThreadWriters, this._itemData, this._itemIndex);

            IEnumerable<Item> items = World.Items.Values;

            //Start the producer.
            Parallel.ForEach(items, () => new QueuedMemoryWriter(),
                (Item item, ParallelLoopState state, QueuedMemoryWriter writer) =>
                {
                    long startPosition = writer.Position;

                    item.Serialize(writer);

                    int size = (int)(writer.Position - startPosition);

                    writer.QueueForIndex(item, size);

                    if (item.Decays && item.Parent == null && item.Map != Map.Internal && DateTime.UtcNow > (item.LastMoved + item.DecayTime))
                    {
                        this._decayBag.Add(item);
                    }

                    if (this._metrics != null)
                    {
                        this._metrics.OnItemSaved(size);
                    }

                    return writer;
                },
                (writer) =>
                {
                    writer.Flush();

                    this._itemThreadWriters.Add(writer);
                });

            this._itemThreadWriters.CompleteAdding();	//We only get here after the Parallel.ForEach completes.  Lets our task 

            return commitTask;
        }

        private Task SaveMobiles()
        {
            //Start the blocking consumer; this runs in background.
            Task commitTask = this.StartCommitTask(this._mobileThreadWriters, this._mobileData, this._mobileIndex);

            IEnumerable<Mobile> mobiles = World.Mobiles.Values;

            //Start the producer.
            Parallel.ForEach(mobiles, () => new QueuedMemoryWriter(),
                (Mobile mobile, ParallelLoopState state, QueuedMemoryWriter writer) =>
                {
                    long startPosition = writer.Position;

                    mobile.Serialize(writer);

                    int size = (int)(writer.Position - startPosition);

                    writer.QueueForIndex(mobile, size);

                    if (this._metrics != null)
                    {
                        this._metrics.OnMobileSaved(size);
                    }

                    return writer;
                },
                (writer) =>
                {
                    writer.Flush();

                    this._mobileThreadWriters.Add(writer);
                });

            this._mobileThreadWriters.CompleteAdding();	//We only get here after the Parallel.ForEach completes.  Lets our task tell the consumer that we're done

            return commitTask;
        }

        private Task SaveGuilds()
        {
            //Start the blocking consumer; this runs in background.
            Task commitTask = this.StartCommitTask(this._guildThreadWriters, this._guildData, this._guildIndex);

            IEnumerable<BaseGuild> guilds = BaseGuild.List.Values;

            //Start the producer.
            Parallel.ForEach(guilds, () => new QueuedMemoryWriter(),
                (BaseGuild guild, ParallelLoopState state, QueuedMemoryWriter writer) =>
                {
                    long startPosition = writer.Position;

                    guild.Serialize(writer);

                    int size = (int)(writer.Position - startPosition);

                    writer.QueueForIndex(guild, size);

                    if (this._metrics != null)
                    {
                        this._metrics.OnGuildSaved(size);
                    }

                    return writer;
                },
                (writer) =>
                {
                    writer.Flush();

                    this._guildThreadWriters.Add(writer);
                });

            this._guildThreadWriters.CompleteAdding();	//We only get here after the Parallel.ForEach completes.  Lets our task 

            return commitTask;
        }

        private Task SaveData()
        {
            Task commitTask = this.StartCommitTask(this._dataThreadWriters, this._customData, this._customIndex);

            IEnumerable<SaveData> data = World.Data.Values;

            Parallel.ForEach(data, () => new QueuedMemoryWriter(),
                (SaveData saveData, ParallelLoopState state, QueuedMemoryWriter writer) =>
                {
                    long startPosition = writer.Position;

                    saveData.Serialize(writer);

                    int size = (int)(writer.Position - startPosition);

                    writer.QueueForIndex(saveData, size);

                    if (this._metrics != null)
                        this._metrics.OnDataSaved(size);

                    return writer;
                },
                (writer) =>
                {
                    writer.Flush();

                    this._dataThreadWriters.Add(writer);
                });

            this._dataThreadWriters.CompleteAdding();

            return commitTask;
        }

        private void OpenFiles()
        {
            this._itemData = new SequentialFileWriter(World.ItemDataPath, this._metrics);
            this._itemIndex = new SequentialFileWriter(World.ItemIndexPath, this._metrics);

            this._mobileData = new SequentialFileWriter(World.MobileDataPath, this._metrics);
            this._mobileIndex = new SequentialFileWriter(World.MobileIndexPath, this._metrics);

            this._guildData = new SequentialFileWriter(World.GuildDataPath, this._metrics);
            this._guildIndex = new SequentialFileWriter(World.GuildIndexPath, this._metrics);

            this._customData = new SequentialFileWriter(World.DataBinaryPath, this._metrics);
            this._customIndex = new SequentialFileWriter(World.DataIndexPath, this._metrics);

            this.WriteCount(this._itemIndex, World.Items.Count);
            this.WriteCount(this._mobileIndex, World.Mobiles.Count);
            this.WriteCount(this._guildIndex, BaseGuild.List.Count);
            this.WriteCount(this._customIndex, World.Data.Count);
        }

        private void CloseFiles()
        {
            this._itemData.Close();
            this._itemIndex.Close();

            this._mobileData.Close();
            this._mobileIndex.Close();

            this._guildData.Close();
            this._guildIndex.Close();

            this._customData.Close();
            this._customIndex.Close();
        }

        private void WriteCount(SequentialFileWriter indexFile, int count)
        {
            //Equiv to GenericWriter.Write( (int)count );
            byte[] buffer = new byte[4];

            buffer[0] = (byte)(count);
            buffer[1] = (byte)(count >> 8);
            buffer[2] = (byte)(count >> 16);
            buffer[3] = (byte)(count >> 24);

            indexFile.Write(buffer, 0, buffer.Length);
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
    }
}