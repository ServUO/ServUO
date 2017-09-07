using System;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Server.Engines.Quests;

namespace Server
{
    public class SpawnerPresistence
    {
        public static string FilePath = Path.Combine("Saves", "SpawnerPresistence.bin");

        private static int _Version;
        public static int Version { get { return _Version; } }

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static void Initialize()
        {
            CheckVersion();
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write((int)1);
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                    FilePath,
                    reader =>
                    {
                        _Version = reader.ReadInt();
                    });
        }

        public static void CheckVersion()
        {
            switch (_Version)
            {
                case 0:
                    CheckSmartSpawn(typeof(BaseVendor), true);
                    CheckQuestQuesters();
                    break;
            }
        }

        public static void ToConsole(string str, ConsoleColor color = ConsoleColor.Green)
        {
            Utility.PushColor(color);
            Console.WriteLine("[Spawner Persistence v{0}] {1}", _Version.ToString(), str);
            Utility.PopColor();
        }

        private static void CheckSmartSpawn(Type check, bool subclasses)
        {
            int count = 0;

            List<XmlSpawner> spawners = World.Items.Values.OfType<XmlSpawner>().Where(s => s.SmartSpawning).ToList();

            foreach (var spawner in spawners)
            {
                if (CheckSmartSpawn(spawner, check, subclasses))
                    count++;
            }

            ColUtility.Free(spawners);

            ToConsole(String.Format("Smart Spawn Removal: {0} spawners [type {1}] smart spawning disabled.", count.ToString(), check.Name));
        }

        public static Dictionary<Type, Type[]> QuestQuesterTypes;

        public static void CheckQuestQuesters()
        {
            ToConsole("Assigning Questers where null...");

            QuestQuesterTypes = new Dictionary<Type, Type[]>();

            foreach (var quester in World.Mobiles.Values.OfType<MondainQuester>())
            {
                Type t = quester.GetType();

                if (QuestQuesterTypes.ContainsKey(t))
                    continue;

                Type[] quests = quester.Quests;

                if (quests != null && quests.Length > 0)
                    QuestQuesterTypes[t] = quests;
            }

            foreach (var item in World.Items.Values.OfType<BaseQuestItem>())
            {
                Type t = item.GetType();

                if (QuestQuesterTypes.ContainsKey(t))
                    continue;

                Type[] quests = item.Quests;

                if (quests != null && quests.Length > 0)
                    QuestQuesterTypes[t] = quests;
            }

            int count = 0;

            foreach (var pm in World.Mobiles.Values.OfType<PlayerMobile>())
            {
                foreach (var quest in pm.Quests.Where(q => q.QuesterType == null))
                {
                    foreach (var kvp in QuestQuesterTypes)
                    {
                        if (quest.QuesterType != null)
                            break;

                        foreach (var type in kvp.Value)
                        {
                            if (type == quest.GetType())
                            {
                                quest.QuesterType = kvp.Key;
                                count++;
                                break;
                            }
                        }
                    }
                }
            }

            ToConsole(String.Format("Quester Re-assignment: {0} quests re-assigned quester type. Some quester types may still be null. These quests will need to be quit.", count.ToString()), ConsoleColor.DarkRed);
        }

        private static bool CheckSmartSpawn(XmlSpawner spawner, Type check, bool subclasses)
        {
            foreach (var obj in spawner.SpawnObjects)
            {
                if (obj.TypeName != null)
                {
                    var t = ScriptCompiler.FindTypeByName(BaseXmlSpawner.ParseObjectType(obj.TypeName));

                    if (t != null && (t == check || (subclasses && t.IsSubclassOf(check))))
                    {
                        spawner.SmartSpawning = false;

                        if (spawner.CurrentCount == 0)
                            spawner.DoRespawn = true;

                        return true;
                    }
                }
            }

            return false;
        }

        public static void Relpace(string current, string replace, string check)
        {
            int count = 0;

            foreach (var spawner in World.Items.Values.OfType<XmlSpawner>())
            {
                if (Replace(spawner, current, replace, check))
                    count++;
            }

            ToConsole(String.Format("Spawn Replacement: {0} spawners replaced [{1} replaced with {2}].", count.ToString(), current, replace));
        }

        public static void Relpace(string current, string replace, string name, string check)
        {
            int count = 0;

            foreach (var spawner in World.Items.Values.OfType<XmlSpawner>().Where(s => s.Name != null && s.Name.ToLower().IndexOf(name.ToLower()) >= 0))
            {
                if (Replace(spawner, current, replace, check))
                    count++;
            }

            ToConsole(String.Format("Spawn Replacement: {0} spawners named {1} replaced [{2} replaced with {3}].", count.ToString(), name, current, replace));
        }

        public static bool Replace(XmlSpawner spawner, string current, string replace, string check)
        {
            bool replaced = false;

            foreach (var obj in spawner.SpawnObjects)
            {
                string typeName = obj.TypeName;

                if (typeName != null && typeName.IndexOf(current) >= 0)
                {
                    if (check == null || typeName.IndexOf(check) < 0)
                    {
                        obj.TypeName = typeName.Replace(current, replace);

                        if (!replaced)
                            replaced = true;
                    }
                }
            }

            return replaced;
        }

        public static void Remove(string toRemove, bool entireLine = true)
        {
            int count = 0;

            foreach (var spawner in World.Items.Values.OfType<XmlSpawner>())
            {
                count += Remove(spawner, toRemove, entireLine);
            }

            ToConsole(String.Format("Spawn Removal: {0} spawn lines {1} containing -{2}-.", count.ToString(), entireLine ? "removed" : "cut", toRemove));
        }

        public static int Remove(XmlSpawner spawner, string toRemove, bool entireLine)
        {
            int count = 0;

            foreach (var obj in spawner.SpawnObjects)
            {
                string typeName = obj.TypeName;

                if (typeName != null && typeName.IndexOf(toRemove) >= 0)
                {
                    if (entireLine)
                    {
                        count++;
                        obj.TypeName = null;
                    }
                    else
                    {
                        count++;
                        obj.TypeName = typeName.Replace(toRemove, "");
                    }
                }
            }

            return count;
        }
    }
}