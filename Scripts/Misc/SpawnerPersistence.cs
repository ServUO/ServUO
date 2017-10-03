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
        public static string FilePath = Path.Combine("Saves/Misc", "SpawnerPresistence.bin");

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
                    writer.Write((int)2);
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

        /// <summary>
        /// Checks version, and calls code appropriately.  Do not use goto keyword unless you want to call the previous version.
        /// </summary>
        public static void CheckVersion()
        {
            switch (_Version)
            {
                case 1:
                    RemoveSpawnVersion1();
                    break;
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

        #region Version 1
        public static void RemoveSpawnVersion1()
        {
            Remove("SeaHorse");
            Delete("Valem");
        }
        #endregion

        #region Version 0
        public static Dictionary<Type, Type[]> QuestQuesterTypes;

        /// <summary>
        /// Any quests that have questers as null, will assign the quester. Some quests don't have questers...
        /// </summary>
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

        /// <summary>
        /// Disables SmartSpawning on XmlSpawners for any spawners that have paramater 'check' as a spawn object type
        /// </summary>
        /// <param name="check">System.Type to look for in SpawnObject lines</param>
        /// <param name="subclasses">Can the spawn object type derive from check?</param>
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
        #endregion

        /// <summary>
        /// Deletes the entire spawner if 'current' if found as an spawn object
        /// </summary>
        /// <param name="current"></param>
        public static void Delete(string current)
        {
            List<XmlSpawner> toDelete = new List<XmlSpawner>();

            foreach (var spawner in World.Items.Values.OfType<XmlSpawner>())
            {
                foreach (var obj in spawner.SpawnObjects)
                {
                    if (obj == null || obj.TypeName == null)
                        continue;

                    string typeName = obj.TypeName.ToLower();
                    string lookingFor = current.ToLower();

                    if (typeName != null && typeName.IndexOf(lookingFor) >= 0)
                    {
                        toDelete.Add(spawner);
                        break;
                    }
                }
            }

            foreach (var spawner in toDelete)
            {
                spawner.Delete();
            }

            ToConsole(String.Format("Spawner Deletion: deleted {0} spawners containing -{1}-.", toDelete.Count.ToString(), current));

            ColUtility.Free(toDelete);
        }

        /// <summary>
        /// Replaces a certain string value with another in any XmlSpawner SpawnObject line
        /// </summary>
        /// <param name="current">What we're looing for</param>
        /// <param name="replace">What we're replacing it with</param>
        /// <param name="check">if the SpawnObject line contains check, we ignore this line altogether</param>
        public static void Replace(string current, string replace, string check)
        {
            int count = 0;

            foreach (var spawner in World.Items.Values.OfType<XmlSpawner>())
            {
                if (Replace(spawner, current, replace, check))
                    count++;
            }

            ToConsole(String.Format("Spawn Replacement: {0} spawners replaced [{1} replaced with {2}].", count.ToString(), current, replace));
        }

        /// <summary>
        /// Replaces a certain string value with another in any XmlSpawner SpawnObject line
        /// </summary>
        /// <param name="current">What we're looing for</param>
        /// <param name="replace">What we're replacing it with</param>
        /// <param name="name">executes replace only if spawner name contains parameter</param>
        /// <param name="check">if the SpawnObject line contains check, we ignore this line altogether</param>
        public static void Replace(string current, string replace, string name, string check)
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
                if (obj == null || obj.TypeName == null)
                    continue;

                string typeName = obj.TypeName.ToLower();
                string lookingFor = current.ToLower();

                if (typeName != null && typeName.IndexOf(lookingFor) >= 0)
                {
                    if (String.IsNullOrEmpty(check) || typeName.IndexOf(check) < 0)
                    {
                        obj.TypeName = typeName.Replace(lookingFor, replace);

                        if (!replaced)
                            replaced = true;
                    }
                }
            }

            return replaced;
        }

        /// <summary>
        /// Removes a SpawnerObject string, either the string or entire line
        /// </summary>
        /// <param name="toRemove">string to remove from line</param>
        public static void Remove(string toRemove)
        {
            int count = 0;

            foreach (var spawner in World.Items.Values.OfType<XmlSpawner>())
            {
                count += Remove(spawner, toRemove);
            }

            ToConsole(String.Format("Spawn Removal: {0} spawn lines removed containing -{1}-.", count.ToString(), toRemove));
        }

        public static int Remove(XmlSpawner spawner, string toRemove)
        {
            int count = 0;

            List<XmlSpawner.SpawnObject> remove = new List<XmlSpawner.SpawnObject>();
            List<XmlSpawner.SpawnObject> objects = spawner.SpawnObjects.ToList();

            foreach (var obj in objects)
            {
                if (obj == null || obj.TypeName == null)
                    continue;

                string typeName = obj.TypeName.ToLower();
                string lookingFor = toRemove.ToLower();

                if (typeName != null && typeName.IndexOf(lookingFor) >= 0)
                {
                    remove.Add(obj);
                }
            }

            count = remove.Count;

            foreach (var obj in remove)
                objects.Remove(obj);

            if (count > 0)
                spawner.SpawnObjects = objects.ToArray();

            ColUtility.Free(remove);

            return count;
        }

        /// <summary>
        /// performs a pre-specified action (use lamba with action) if the conditions are met
        /// </summary>
        /// <param name="lineCheck">Condition for a specific string in the spawn object line</param>
        /// <param name="nameCheck">Condition for the spawner name</param>
        /// <param name="exempt">Condition to prevent action being performed on spawner</param>
        /// <param name="action">action to be performed, setup in calling method</param>
        public static void ActionOnSpawner(string lineCheck, string nameCheck, string exempt, Action<XmlSpawner> action)
        {
            int count = 0;

            if (action != null)
            {
                List<XmlSpawner> list = World.Items.Values.OfType<XmlSpawner>().Where(s => nameCheck == null || s.Name.ToLower().IndexOf(nameCheck.ToLower()) >= 0).ToList();

                foreach (var spawner in list)
                {
                    if (ActionOnSpawner(spawner, lineCheck, exempt, action))
                        count++;
                }

                ColUtility.Free(list);
            }

            ToConsole(String.Format("Spawner Action: Performed action to {0} spawners{1}.", count.ToString(), lineCheck != null ? " containing " + lineCheck + "." : "."));
        }

        public static bool ActionOnSpawner(XmlSpawner spawner, string lineCheck, string exempt, Action<XmlSpawner> action)
        {
            foreach (var obj in spawner.SpawnObjects.Where(o => !String.IsNullOrEmpty(o.TypeName)))
            {
                if (obj == null || obj.TypeName == null)
                    continue;

                string spawnObject = obj.TypeName.ToLower();
                string lookFor = lineCheck != null ? lineCheck.ToLower() : null;

                if ((lookFor == null || spawnObject.IndexOf(lookFor) >= 0) && (exempt == null || spawnObject.IndexOf(exempt.ToLower()) >= 0))
                {
                    action(spawner);
                    return true;
                }
            }

            return false;
        }
    }
}