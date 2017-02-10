using System;
using System.Collections;
using System.IO;
using Server.Diagnostics;

namespace Server.Commands
{
    public class Profiling
    {
        public static void Initialize()
        {
            CommandSystem.Register("DumpTimers", AccessLevel.Administrator, new CommandEventHandler(DumpTimers_OnCommand));
            CommandSystem.Register("CountObjects", AccessLevel.Administrator, new CommandEventHandler(CountObjects_OnCommand));
            CommandSystem.Register("ProfileWorld", AccessLevel.Administrator, new CommandEventHandler(ProfileWorld_OnCommand));
            CommandSystem.Register("TraceInternal", AccessLevel.Administrator, new CommandEventHandler(TraceInternal_OnCommand));
            CommandSystem.Register("TraceExpanded", AccessLevel.Administrator, new CommandEventHandler(TraceExpanded_OnCommand));
            CommandSystem.Register("WriteProfiles", AccessLevel.Administrator, new CommandEventHandler(WriteProfiles_OnCommand));
            CommandSystem.Register("SetProfiles", AccessLevel.Administrator, new CommandEventHandler(SetProfiles_OnCommand));
        }

        [Usage("WriteProfiles")]
        [Description("Generates a log files containing performance diagnostic information.")]
        public static void WriteProfiles_OnCommand(CommandEventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("profiles.log", true))
                {
                    sw.WriteLine("# Dump on {0:f}", DateTime.UtcNow);
                    sw.WriteLine("# Core profiling for " + Core.ProfileTime);

                    sw.WriteLine("# Packet send");
                    BaseProfile.WriteAll(sw, PacketSendProfile.Profiles);
                    sw.WriteLine();

                    sw.WriteLine("# Packet receive");
                    BaseProfile.WriteAll(sw, PacketReceiveProfile.Profiles);
                    sw.WriteLine();

                    sw.WriteLine("# Timer");
                    BaseProfile.WriteAll(sw, TimerProfile.Profiles);
                    sw.WriteLine();

                    sw.WriteLine("# Gump response");
                    BaseProfile.WriteAll(sw, GumpProfile.Profiles);
                    sw.WriteLine();

                    sw.WriteLine("# Target response");
                    BaseProfile.WriteAll(sw, TargetProfile.Profiles);
                    sw.WriteLine();
                }
            }
            catch
            {
            }
        }

        [Usage("SetProfiles [true | false]")]
        [Description("Enables, disables, or toggles the state of core packet and timer profiling.")]
        public static void SetProfiles_OnCommand(CommandEventArgs e)
        {
            if (e.Length == 1)
                Core.Profiling = e.GetBoolean(0);
            else
                Core.Profiling = !Core.Profiling;

            e.Mobile.SendMessage("Profiling has been {0}.", Core.Profiling ? "enabled" : "disabled");
        }

        [Usage("DumpTimers")]
        [Description("Generates a log file of all currently executing timers. Used for tracing timer leaks.")]
        public static void DumpTimers_OnCommand(CommandEventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("timerdump.log", true))
                    Timer.DumpInfo(sw);
            }
            catch
            {
            }
        }

        [Usage("CountObjects")]
        [Description("Generates a log file detailing all item and mobile types in the world.")]
        public static void CountObjects_OnCommand(CommandEventArgs e)
        {
            using (StreamWriter op = new StreamWriter("objects.log"))
            {
                Hashtable table = new Hashtable();

                foreach (Item item in World.Items.Values)
                {
                    Type type = item.GetType();

                    object o = (object)table[type];

                    if (o == null)
                        table[type] = 1;
                    else
                        table[type] = 1 + (int)o;
                }

                ArrayList items = new ArrayList(table);

                table.Clear();

                foreach (Mobile m in World.Mobiles.Values)
                {
                    Type type = m.GetType();

                    object o = (object)table[type];

                    if (o == null)
                        table[type] = 1;
                    else
                        table[type] = 1 + (int)o;
                }

                ArrayList mobiles = new ArrayList(table);

                items.Sort(new CountSorter());
                mobiles.Sort(new CountSorter());

                op.WriteLine("# Object count table generated on {0}", DateTime.UtcNow);
                op.WriteLine();
                op.WriteLine();

                op.WriteLine("# Items:");

                foreach (DictionaryEntry de in items)
                    op.WriteLine("{0}\t{1:F2}%\t{2}", de.Value, (100 * (int)de.Value) / (double)World.Items.Count, de.Key);

                op.WriteLine();
                op.WriteLine();

                op.WriteLine("#Mobiles:");

                foreach (DictionaryEntry de in mobiles)
                    op.WriteLine("{0}\t{1:F2}%\t{2}", de.Value, (100 * (int)de.Value) / (double)World.Mobiles.Count, de.Key);
            }

            e.Mobile.SendMessage("Object table has been generated. See the file : <servuo root>/objects.log");
        }

        [Usage("TraceExpanded")]
        [Description("Generates a log file describing all items using expanded memory.")]
        public static void TraceExpanded_OnCommand(CommandEventArgs e)
        {
            Hashtable typeTable = new Hashtable();

            foreach (Item item in World.Items.Values)
            {
                ExpandFlag flags = item.GetExpandFlags();

                if ((flags & ~(ExpandFlag.TempFlag | ExpandFlag.SaveFlag)) == 0)
                    continue;

                Type itemType = item.GetType();

                do
                {
                    int[] countTable = typeTable[itemType] as int[];

                    if (countTable == null)
                        typeTable[itemType] = countTable = new int[9];

                    if ((flags & ExpandFlag.Name) != 0)
                        ++countTable[0];

                    if ((flags & ExpandFlag.Items) != 0)
                        ++countTable[1];

                    if ((flags & ExpandFlag.Bounce) != 0)
                        ++countTable[2];

                    if ((flags & ExpandFlag.Holder) != 0)
                        ++countTable[3];

                    if ((flags & ExpandFlag.Blessed) != 0)
                        ++countTable[4];

                    /*if ( ( flags & ExpandFlag.TempFlag ) != 0 )
                    ++countTable[5];

                    if ( ( flags & ExpandFlag.SaveFlag ) != 0 )
                    ++countTable[6];*/

                    if ((flags & ExpandFlag.Weight) != 0)
                        ++countTable[7];

                    if ((flags & ExpandFlag.Spawner) != 0)
                        ++countTable[8];

                    itemType = itemType.BaseType;
                }
                while (itemType != typeof(object));
            }

            try
            {
                using (StreamWriter op = new StreamWriter("expandedItems.log", true))
                {
                    string[] names = new string[]
                    {
                        "Name",
                        "Items",
                        "Bounce",
                        "Holder",
                        "Blessed",
                        "TempFlag",
                        "SaveFlag",
                        "Weight",
                        "Spawner"
                    };

                    ArrayList list = new ArrayList(typeTable);

                    list.Sort(new CountSorter());

                    foreach (DictionaryEntry de in list)
                    {
                        Type itemType = de.Key as Type;
                        int[] countTable = de.Value as int[];

                        op.WriteLine("# {0}", itemType.FullName);

                        for (int i = 0; i < countTable.Length; ++i)
                        {
                            if (countTable[i] > 0)
                                op.WriteLine("{0}\t{1:N0}", names[i], countTable[i]);
                        }

                        op.WriteLine();
                    }
                }
            }
            catch
            {
            }
        }

        [Usage("TraceInternal")]
        [Description("Generates a log file describing all items in the 'internal' map.")]
        public static void TraceInternal_OnCommand(CommandEventArgs e)
        {
            int totalCount = 0;
            Hashtable table = new Hashtable();

            foreach (Item item in World.Items.Values)
            {
                if (item.Parent != null || item.Map != Map.Internal)
                    continue;

                ++totalCount;

                Type type = item.GetType();
                int[] parms = (int[])table[type];

                if (parms == null)
                    table[type] = parms = new int[] { 0, 0 };

                parms[0]++;
                parms[1] += item.Amount;
            }

            using (StreamWriter op = new StreamWriter("internal.log"))
            {
                op.WriteLine("# {0} items found", totalCount);
                op.WriteLine("# {0} different types", table.Count);
                op.WriteLine();
                op.WriteLine();
                op.WriteLine("Type\t\tCount\t\tAmount\t\tAvg. Amount");

                foreach (DictionaryEntry de in table)
                {
                    Type type = (Type)de.Key;
                    int[] parms = (int[])de.Value;

                    op.WriteLine("{0}\t\t{1}\t\t{2}\t\t{3:F2}", type.Name, parms[0], parms[1], (double)parms[1] / parms[0]);
                }
            }
        }

        [Usage("ProfileWorld")]
        [Description("Prints the amount of data serialized for every object type in your world file.")]
        public static void ProfileWorld_OnCommand(CommandEventArgs e)
        {
            ProfileWorld("items", "worldprofile_items.log");
            ProfileWorld("mobiles", "worldprofile_mobiles.log");
        }

        public static void ProfileWorld(string type, string opFile)
        {
            try
            {
                ArrayList types = new ArrayList();

                using (BinaryReader bin = new BinaryReader(new FileStream(String.Format("Saves/{0}/{0}.tdb", type), FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    int count = bin.ReadInt32();

                    for (int i = 0; i < count; ++i)
                        types.Add(ScriptCompiler.FindTypeByFullName(bin.ReadString()));
                }

                long total = 0;

                Hashtable table = new Hashtable();

                using (BinaryReader bin = new BinaryReader(new FileStream(String.Format("Saves/{0}/{0}.idx", type), FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    int count = bin.ReadInt32();

                    for (int i = 0; i < count; ++i)
                    {
                        int typeID = bin.ReadInt32();
                        int serial = bin.ReadInt32();
                        long pos = bin.ReadInt64();
                        int length = bin.ReadInt32();
                        Type objType = (Type)types[typeID];

                        while (objType != null && objType != typeof(object))
                        {
                            object obj = table[objType];

                            if (obj == null)
                                table[objType] = length;
                            else
                                table[objType] = length + (int)obj;

                            objType = objType.BaseType;
                            total += length;
                        }
                    }
                }

                ArrayList list = new ArrayList(table);

                list.Sort(new CountSorter());

                using (StreamWriter op = new StreamWriter(opFile))
                {
                    op.WriteLine("# Profile of world {0}", type);
                    op.WriteLine("# Generated on {0}", DateTime.UtcNow);
                    op.WriteLine();
                    op.WriteLine();

                    foreach (DictionaryEntry de in list)
                        op.WriteLine("{0}\t{1:F2}%\t{2}", de.Value, (100 * (int)de.Value) / (double)total, de.Key);
                }
            }
            catch
            {
            }
        }

        private class CountSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                DictionaryEntry a = (DictionaryEntry)x;
                DictionaryEntry b = (DictionaryEntry)y;

                int aCount = this.GetCount(a.Value);
                int bCount = this.GetCount(b.Value);

                int v = -aCount.CompareTo(bCount);

                if (v == 0)
                {
                    Type aType = (Type)a.Key;
                    Type bType = (Type)b.Key;

                    v = aType.FullName.CompareTo(bType.FullName);
                }

                return v;
            }

            private int GetCount(object obj)
            {
                if (obj is int)
                    return (int)obj;

                if (obj is int[])
                {
                    int[] list = (int[])obj;

                    int total = 0;

                    for (int i = 0; i < list.Length; ++i)
                        total += list[i];

                    return total;
                }

                return 0;
            }
        }
    }
}
