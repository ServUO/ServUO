using System;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Server.Engines.Quests;
using System.Xml;
using System.Data;
using Server.Commands;
using Server.Gumps;
using Server.Multis;

namespace Server
{
    public class SpawnerPresistence
    {
        public static string FilePath = Path.Combine("Saves/Misc", "SpawnerPresistence.bin");

        private static bool _FirstRun = true;

        private static int _Version;
        public static int Version { get { return _Version; } }

        private static bool _SpawnsConverted;
        public static bool SpawnsConverted { get { return _SpawnsConverted; } }

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static void Initialize()
        {
            if (!_FirstRun)
            {
                CheckVersion();
            }

            CommandSystem.Register("ConvertSpawners", AccessLevel.GameMaster, e =>
            {
                string str = "By selecting OK, you will wipe all XmlSpawners that were placed via World Load, and will replace " +
                             "with standard spawners. Any existing spawner with special symbols, such as , <> / will not be converted.";

                if (_SpawnsConverted)
                    str += "<br><br>You have already ran this conversion. Run Again?";

                e.Mobile.SendGump(new WarningGump(1019005, 30720, str, 0xFFFFFF, 400, 300, (from, ok, state) =>
                {
                    if (ok)
                    {
                        from.SendMessage("Stand by while spawners are converted. This may take a few minutes...");
                        ConvertXmlToSpanwers();
                    }
                }, null, true));
            });
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write((int)4);
                    writer.Write(false);
                    writer.Write(_SpawnsConverted);
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                    FilePath,
                    reader =>
                    {
                        _Version = reader.ReadInt();

                        if (_Version > 2)
                        {
                            _FirstRun = reader.ReadBool();
                            _SpawnsConverted = reader.ReadBool();
                        }
                    });
        }

        /// <summary>
        /// Checks version, and calls code appropriately.  Do not use goto keyword unless you want to call the previous version.
        /// </summary>
        public static void CheckVersion()
        {
            switch (_Version)
            {
                case 3:
                    FixCampSpawnersVersion3();
                    break;
                case 2: // Nothing
                    break;
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

        #region Version 3
        private static void FixCampSpawnersVersion3()
        {
            ActionOnSpawner(typeof(BaseCamp), null, null, null, spawner =>
            {
                if (spawner is XmlSpawner)
                {
                    var s = spawner as XmlSpawner;

                    s.MinDelay = TimeSpan.FromMinutes(5);
                    s.MaxDelay = TimeSpan.FromMinutes(10);
                }
                else if (spawner is Spawner)
                {
                    var s = spawner as Spawner;

                    s.MinDelay = TimeSpan.FromMinutes(5);
                    s.MaxDelay = TimeSpan.FromMinutes(10);
                }
            }, true);
        }
        #endregion

        #region Version 1
        private static void RemoveSpawnVersion1()
        {
            Remove("SeaHorse");
            Delete("Valem");
        }
        #endregion

        #region Version 0
        private static Dictionary<Type, Type[]> QuestQuesterTypes;

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
        /*public static void ActionOnSpawner(string lineCheck, string nameCheck, string exempt, Action<XmlSpawner> action, bool inherits = false)
        {
            int count = 0;

            if (action != null)
            {
                List<XmlSpawner> list = World.Items.Values.OfType<XmlSpawner>().Where(s => nameCheck == null || s.Name.ToLower().IndexOf(nameCheck.ToLower()) >= 0).ToList();

                foreach (var spawner in list)
                {
                    if (ActionOnSpawner(spawner, lineCheck, exempt, action, inherits))
                        count++;
                }

                ColUtility.Free(list);
            }

            ToConsole(String.Format("Spawner Action: Performed action to {0} spawners{1}.", count.ToString(), lineCheck != null ? " containing " + lineCheck + "." : "."));
        }

        public static bool ActionOnSpawner(XmlSpawner spawner, string lineCheck, string exempt, Action<XmlSpawner> action, bool inherits)
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
        }*/

        /// <summary>
        /// performs a pre-specified action (use lamba with action) if the conditions are met
        /// </summary>
        /// <param name="lineCheck">Condition for a specific string in the spawn object line</param>
        /// <param name="nameCheck">Condition for the spawner name</param>
        /// <param name="exempt">Condition to prevent action being performed on spawner</param>
        /// <param name="action">action to be performed, setup in calling method</param>
        public static void ActionOnSpawner(Type typeCheck, string lineCheck, string nameCheck, string exempt, Action<ISpawner> action, bool inherits = false)
        {
            int count = 0;

            if (action != null)
            {
                List<ISpawner> list = World.Items.Values.OfType<ISpawner>().Where(s => 
                    nameCheck == null ||  (s is Item && ((Item)s).Name != null && ((Item)s).Name.ToLower().IndexOf(nameCheck.ToLower()) >= 0)).ToList();

                foreach (ISpawner spawner in list)
                {
                    if (ActionOnSpawner(spawner, typeCheck, lineCheck, exempt, action, inherits))
                        count++;
                }

                ColUtility.Free(list);
            }

            ToConsole(String.Format("Spawner Action: Performed action to {0} spawners{1}", 
                count.ToString(), lineCheck != null ? " containing " + lineCheck + "." : typeCheck != null ? " containing " + typeCheck.Name + "." : "."));
        }

        public static bool ActionOnSpawner(ISpawner spawner, Type typeCheck, string lineCheck, string exempt, Action<ISpawner> action, bool inherits)
        {
            string[] list = GetSpawnList(spawner);

            if (list == null)
                return false;

            foreach (var str in list)
            {
                if (string.IsNullOrEmpty(str))
                    continue;

                string spawnObject = str.ToLower();

                if (typeCheck != null)
                {
                    Type t;

                    if (spawner is Spawner)
                        t = ScriptCompiler.FindTypeByName(spawnObject);
                    else
                        t = ScriptCompiler.FindTypeByName(BaseXmlSpawner.ParseObjectType(spawnObject));

                    if (t == typeCheck || (t != null && inherits && t.IsSubclassOf(typeCheck)))
                    {
                        if (action != null)
                            action(spawner);

                        return true;
                    }
                }
                else
                {
                    string lookFor = lineCheck != null ? lineCheck.ToLower() : null;

                    if ((lookFor == null || spawnObject.IndexOf(lookFor) >= 0) && (exempt == null || spawnObject.IndexOf(exempt.ToLower()) <= 0))
                    {
                        if (action != null)
                            action(spawner);

                        return true;
                    }
                }
            }

            return false;
        }

        private static string[] GetSpawnList(ISpawner spawner)
        {
            string[] list = null;

            if (spawner is XmlSpawner && ((XmlSpawner)spawner).SpawnObjects != null && ((XmlSpawner)spawner).SpawnObjects.Length > 0)
            {
                list = ((XmlSpawner)spawner).SpawnObjects.Select(obj => obj.TypeName).ToArray();
            }
            else if (spawner is Spawner && ((Spawner)spawner).SpawnNames != null && ((Spawner)spawner).SpawnNames.Count > 0)
            {
                List<string> names = ((Spawner)spawner).SpawnNames;

                list = new string[names.Count];

                for (int i = 0; i < names.Count; i++)
                {
                    list[i] = names[i];
                }
            }

            return list;
        }

        #region XmlSpawner to Spawner Conversion
        public static void ConvertXmlToSpanwers()
        {
            string filename = "Spawns";

            if (System.IO.Directory.Exists(filename) == true)
            {
                List<string> files = null;
                string[] dirs = null;

                try
                {
                    files = new List<string>(Directory.GetFiles(filename, "*.xml"));
                    dirs = Directory.GetDirectories(filename);
                }
                catch { }

                if (dirs != null && dirs.Length > 0)
                {
                    foreach (var dir in dirs)
                    {
                        try
                        {
                            string[] dirFiles = Directory.GetFiles(dir, "*.xml");
                            files.AddRange(dirFiles);
                        }
                        catch { }
                    }
                }

                ToConsole(String.Format("Found {0} Xmlspawner files for conversion.", files.Count), files != null && files.Count > 0 ? ConsoleColor.Green : ConsoleColor.Red);

                if (files != null && files.Count > 0)
                {
                    int converted = 0;
                    int failed = 0;
                    int keep = 0;

                    foreach (string file in files)
                    {
                        FileStream fs = null;

                        try
                        {
                            fs = File.Open(file, FileMode.Open, FileAccess.Read);
                        }
                        catch { }

                        if (fs == null)
                        {
                            ToConsole(String.Format("Unable to open {0} for loading", filename), ConsoleColor.Red);
                            continue;
                        }

                        DataSet ds = new DataSet("Spawns");

                        try
                        {
                            ds.ReadXml(fs);
                        }
                        catch
                        {
                            fs.Close();
                            ToConsole(String.Format("Error reading xml file {0}", filename), ConsoleColor.Red);
                            continue;
                        }

                        if (ds.Tables != null && ds.Tables.Count > 0)
                        {
                            if (ds.Tables["Points"] != null && ds.Tables["Points"].Rows.Count > 0)
                            {
                                foreach (DataRow dr in ds.Tables["Points"].Rows)
                                {
                                    string id = null;

                                    try
                                    {
                                        id = (string)dr["UniqueId"];
                                    }
                                    catch { }

                                    bool convert = id != null && ConvertSpawner(id, dr);

                                    if (!convert)
                                    {
                                        Point3D loc = Point3D.Zero;
                                        Map spawnMap = null;

                                        try
                                        {
                                            loc = new Point3D(int.Parse((string)dr["CentreX"]), int.Parse((string)dr["CentreY"]), int.Parse((string)dr["CentreZ"]));
                                            spawnMap = Map.Parse((string)dr["Map"]);
                                        }
                                        catch{}

                                        if (loc != Point3D.Zero && spawnMap != null && spawnMap != Map.Internal && !ConvertSpawnerByLocation(loc, spawnMap, dr, ref keep))
                                        {
                                            failed++;
                                        }
                                    }
                                    else
                                    {
                                        converted++;
                                    }
                                }
                            }
                        }

                        fs.Close();
                    }

                    if (converted > 0)
                        ToConsole(String.Format("Converted {0} XmlSpawners to standard spawners.", converted), ConsoleColor.Green);

                    if (failed > 0)
                        ToConsole(String.Format("Failed to convert {0} XmlSpawners to standard spawners. {1} kept due to XmlSpawner Functionality", failed, keep), ConsoleColor.Red);

                    _SpawnsConverted = true;
                }
                else
                {
                    ToConsole(String.Format("Directory Not Found: {0}", filename), ConsoleColor.Red);
                }
            }
        }

        private static bool ConvertSpawnerByLocation(Point3D p, Map map, DataRow dr, ref int keep)
        {
            XmlSpawner spawner = World.Items.Values.OfType<XmlSpawner>().FirstOrDefault(s => s.Location == p && s.Map == map);

            return ConvertSpawner(spawner, dr, ref keep);
        }

        private static bool ConvertSpawner(string id, DataRow dr)
        {
            XmlSpawner spawner = World.Items.Values.OfType<XmlSpawner>().FirstOrDefault(s => s.UniqueId == id);

            int c = 0;
            return ConvertSpawner(spawner, dr, ref c);
        }

        private static bool ConvertSpawner(XmlSpawner spawner, DataRow d, ref int keep)
        {
            if (spawner != null)
            {
                string[] spawns = new string[spawner.SpawnObjects.Length];

                for (int i = 0; i < spawner.SpawnObjects.Length; i++)
                {
                    var obj = spawner.SpawnObjects[i];

                    if (obj == null || obj.TypeName == null)
                        continue;

                    spawns[i] = obj.TypeName;
                }

                if (HasSpecialXmlSpawnerString(spawns))
                {
                    keep++;
                    return false;
                }

                Spawner newSpawner = new Spawner(spawner.MaxCount,
                                                 spawner.MinDelay,
                                                 spawner.MaxDelay,
                                                 spawner.Team,
                                                 spawner.HomeRange,
                                                 spawns.ToList());

                newSpawner.Group = spawner.Group;
                newSpawner.Running = spawner.Running;

                newSpawner.MoveToWorld(spawner.Location, spawner.Map);
                spawner.Delete();
                return true;
            }

            return false;
        }

        private static bool HasSpecialXmlSpawnerString(string[] spawns)
        {
            foreach (var line in spawns)
            {
                if (line != null)
                {
                    foreach (var s in _SpawnerSymbols)
                    {
                        if (line.Contains(s))
                            return true;
                    }
                }
            }

            return false;
        }

        private static string[] _SpawnerSymbols =
        {
            "/", "<", ">", ",", "{", "}"
        };
        #endregion
    }
}