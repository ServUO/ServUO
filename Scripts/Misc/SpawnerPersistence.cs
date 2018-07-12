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

/* This script has a purpose, and please adhere to the advice before adding versions.
 * This is used for modifying, removing, adding existing spawners, etc for existing shards,
 * used for modifying, removing, adding existing spawners, etc for existing shards.
 * As this is a collaborative effort for ServUO, it's important that any modifications to 
 * existing shards be handled for new shareds.  For example, if your swapping out some spawners,
 * common practice will be to edit the spawner files for fresh-loaded servers. Please refer to
 * ServUO.com community with any questions or concerns.
 */

namespace Server
{
    public class SpawnerPersistence
    {
        [Flags]
        public enum SpawnerVersion
        {
            None            = 0x00000000,
            Initial         = 0x00000001,
            Sphinx          = 0x00000002,
            IceHoundRemoval = 0x00000004,
            PaladinAndKrakin= 0x00000008,
            TrinsicPaladins = 0x00000010,
        }

        public static string FilePath = Path.Combine("Saves/Misc", "SpawnerPresistence.bin");

        private static bool _FirstRun = true;

        private static int _Version;
        public static int Version { get { return _Version; } }

        public static SpawnerVersion VersionFlag { get; set; }

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
            else if (_Version == 0) // new server, no need to run the new stuff.
            {
                // This way, fresh servers won't duplicate any spawners that should have already been adjusted for a fresh server
                foreach (int i in Enum.GetValues(typeof(SpawnerVersion)))
                {
                    if (i == 0x00000000)
                        continue;

                    VersionFlag |= (SpawnerVersion)i;
                }
            }

            #region Commands
            CommandSystem.Register("ConvertSpawners", AccessLevel.Administrator, e =>
            {
                string str = "By selecting OK, you will wipe all XmlSpawners that were placed via World Load, and will replace " +
                             "with standard spawners. Any existing spawner with special symbols, such as , <> / will not be converted. " +
                             "Be advised, this process will take several minutes to complete.";

                if (_SpawnsConverted)
                    str += "<br><br>You have already ran this conversion. Run Again?";

                e.Mobile.SendGump(new WarningGump(1019005, 30720, str, 0xFFFFFF, 400, 300, (from, ok, state) =>
                {
                    if (ok)
                    {
                        from.SendMessage("Stand by while spawners are converted. This may take a few minutes...");
                        Timer.DelayCall(ConvertXmlToSpawners);
                    }
                }, null, true));
            });

            CommandSystem.Register("RevertXmlSpawners", AccessLevel.Administrator, e =>
                {
                    string str = "By selecting OK, you will wipe all XmlSpawners that were left over from conversion to " +
                                 "standard spawners. All standard spawners will be deleted, and xmlspawners will be re-added. " +
                                 "Be advised, this process will take several minutes to complete.";

                    e.Mobile.SendGump(new WarningGump(1019005, 30720, str, 0xFFFFFF, 400, 300, (from, ok, state) =>
                    {
                        if (ok)
                        {
                            from.SendMessage("Stand by while spawners are converted back to XmlSpawners. This may take a few minutes...");
                            Timer.DelayCall(() => RevertToXmlSpawners(e.Mobile));
                        }
                    }, null, true));
                });

            CommandSystem.Register("WipeAllXmlSpawners", AccessLevel.Administrator, e =>
                {
                    WipeSpawnersFromFile();
                });
            #endregion
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write((int)12);

                    writer.Write((int)VersionFlag);

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

                        if (_Version > 10)
                            VersionFlag = (SpawnerVersion)reader.ReadInt();

                        if (_Version > 2)
                        {
                            _FirstRun = reader.ReadBool();
                            _SpawnsConverted = reader.ReadBool();
                        }
                    });
        }

        /// <summary>
        /// Checks version, and calls code appropriately.  Version 10 implements SpawnerFlag so servers don't miss out and skip versions.
        /// After this point, there is no need to increase version anymore unless any changes 
        /// </summary>
        public static void CheckVersion()
        {
            switch (_Version)
            {
                case 12:
                case 11:
                    if ((VersionFlag & SpawnerVersion.TrinsicPaladins) == 0)
                    {
                        SpawnTrinsicPaladins();
                        VersionFlag |= SpawnerVersion.TrinsicPaladins;
                    }

                    if ((VersionFlag & SpawnerVersion.PaladinAndKrakin) == 0)
                    {
                        RemovePaladinsAndKrakens();
                        VersionFlag |= SpawnerVersion.PaladinAndKrakin;
                    }

                    if ((VersionFlag & SpawnerVersion.IceHoundRemoval) == 0)
                    {
                        RemoveIceHounds();
                        VersionFlag |= SpawnerVersion.IceHoundRemoval;
                    }

                    if ((VersionFlag & SpawnerVersion.Sphinx) == 0)
                    {
                        AddSphinx();
                        VersionFlag |= SpawnerVersion.Sphinx;
                    }
                    goto case 10;
                case 10:
                    if((VersionFlag & SpawnerVersion.Initial) == 0)
                        VersionFlag |= SpawnerVersion.Initial;
                    break;
                case 9:
                    LoadFromXmlSpawner("Spawns/twistedweald.xml", Map.Ilshenar, "TwistedWealdTrigger1");
                    LoadFromXmlSpawner("Spawns/twistedweald.xml", Map.Ilshenar, "TwistedWealdTrigger2");
                    LoadFromXmlSpawner("Spawns/twistedweald.xml", Map.Ilshenar, "TwistedWealdTrigger3");
                    LoadFromXmlSpawner("Spawns/twistedweald.xml", Map.Ilshenar, "TwistedWealdTrigger4");
                    ReplaceUnderworldVersion9();
                    break;
                case 8:
                    ReplaceSolenHivesVersion8();
                    break;
                case 7:
                case 6:
                    ReplaceTwistedWealdVersion7();
                    RunicReforging.ItemNerfVersion6();
                    break;
                case 5:
                    HonestyItemsVersion5();
                    break;
                case 4:
                    BrigandsVersion4();
                    break;
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

        #region Trinny Paladins
        public static void SpawnTrinsicPaladins()
        {
            LoadFromXmlSpawner("Spawns/trammel.xml", Map.Trammel, "TrinsicPaladinSpawner");
            LoadFromXmlSpawner("Spawns/felucca.xml", Map.Felucca, "TrinsicPaladinSpawner");
        }
        #endregion

        #region Remove Paladins And Krakens
        public static void RemovePaladinsAndKrakens()
        {
            Remove("HirePaladin");
            Remove("Kraken", sp => !Region.Find(sp.Location, sp.Map).IsPartOf("Shame"));
            ToConsole("Paladins and Krakens removed from spawners.");
        }
        #endregion

        #region Remove Ice Hounds
        public static void RemoveIceHounds()
        {
            Remove("icehound");
            ToConsole("Ice Hounds removed from spawners.");
        }
        #endregion

        #region Version 11
        public static void AddSphinx()
        {
            Server.Engines.GenerateForgottenPyramid.Generate(null);
            ToConsole("Generated Fortune Sphinx.");
        }
        #endregion

        #region Version 9
        public static void ReplaceUnderworldVersion9()
        {
            ReplaceSpawnersByRegionName("Underworld", Map.TerMur, "underworld");

            ReplaceSpawnersByRectangle(new Rectangle2D(5640, 1776, 295, 263), Map.Trammel, null);
            ReplaceSpawnersByRectangle(new Rectangle2D(5640, 1776, 295, 263), Map.Felucca, "solenhives");

            QuestHintItem hint = new DuganMissingQuestCorpse();
            hint.MoveToWorld(new Point3D(1038, 1182, -52), Map.TerMur);

            Static item = new Static(7400);
            item.MoveToWorld(new Point3D(1040, 1181, -53), Map.TerMur);

            item = new Static(7390);
            item.MoveToWorld(new Point3D(1041, 1185, -50), Map.TerMur);

            item = new Static(7390);
            item.MoveToWorld(new Point3D(1036, 1185, -52), Map.TerMur);

            hint = new FlintLostLogbookHint();
            hint.MoveToWorld(new Point3D(1044, 976, -30), Map.TerMur);

            hint = new FlintLostBarrelHint();
            hint.MoveToWorld(new Point3D(1043, 1003, -43), Map.TerMur);

            hint = new FlintLostBarrelHint();
            hint.MoveToWorld(new Point3D(1048, 1027, -32), Map.TerMur);

            GenerateUnderworldRooms.GenerateRevealTiles();

            ToConsole("Placed Quest Statics.");
        }
        #endregion

        #region Version 8
        public static void ReplaceSolenHivesVersion8()
        {
            ReplaceSpawnersByRectangle(new Rectangle2D(5640, 1776, 295, 263), Map.Trammel, null);
            ReplaceSpawnersByRectangle(new Rectangle2D(5640, 1776, 295, 263), Map.Felucca, "solenhives");
        }
        #endregion

        #region Version 6 & 7
        public static void ReplaceTwistedWealdVersion7()
        {
            ReplaceSpawnersByRegionName("Twisted Weald", Map.Ilshenar, "twistedweald");
        }
        #endregion

        #region Version 5
        public static void HonestyItemsVersion5()
        {
            Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
                {
                    int count = 0;
                    foreach (var item in World.Items.Values.Where(i => i.HonestyItem && !ItemFlags.GetTaken(i)))
                    {
                        RunicReforging.GenerateRandomItem(item, 0, 100, 1000);
                        count++;
                    }

                    ToConsole(String.Format("Honesty items given magical properties: {0}", count.ToString()));
                });
        }
        #endregion

        #region Version 4
        public static void BrigandsVersion4()
        {
            Replace("humanbrigand", "brigand", null);
            Replace("humanbrigandcamp", "brigandcamp", null);
        }
        #endregion

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

            foreach (var spawner in World.Items.Values.OfType<ISpawner>())
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

            foreach (var spawner in World.Items.Values.OfType<ISpawner>().Where(s => s is Item && ((Item)s).Name != null && ((Item)s).Name.ToLower().IndexOf(name.ToLower()) >= 0))
            {
                if (Replace(spawner, current, replace, check))
                    count++;
            }

            ToConsole(String.Format("Spawn Replacement: {0} spawners named {1} replaced [{2} replaced with {3}].", count.ToString(), name, current, replace));
        }

        public static bool Replace(ISpawner spwner, string current, string replace, string check)
        {
            bool replaced = false;

            if (spwner is XmlSpawner)
            {
                XmlSpawner spawner = (XmlSpawner)spwner;

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
            }
            else if (spwner is Spawner)
            {
                Spawner spawner = (Spawner)spwner;

                for(int i = 0; i < spawner.SpawnObjects.Count; i++)
                {
                    var so = spawner.SpawnObjects[i];

                    string typeName = so.SpawnName.ToLower();
                    string lookingFor = current.ToLower();

                    if (typeName != null && typeName.IndexOf(lookingFor) >= 0)
                    {
                        if (String.IsNullOrEmpty(check) || typeName.IndexOf(check) < 0)
                        {
                            so.SpawnName = typeName.Replace(lookingFor, replace);

                            if (!replaced)
                                replaced = true;
                        }
                    }
                }
            }

            return replaced;
        }

        /// <summary>
        /// Removes a SpawnerObject string, either the string or entire line
        /// </summary>
        /// <param name="toRemove">string to remove from line</param>
        public static void Remove(string toRemove, Func<XmlSpawner, bool> predicate = null)
        {
            int count = 0;
            int deleted = 0;

            var list = new List<XmlSpawner>(World.Items.Values.OfType<XmlSpawner>());

            foreach (var spawner in list)
            {
                if (predicate == null || predicate(spawner))
                {
                    count += Remove(spawner, toRemove, ref deleted);
                }
            }

            ColUtility.Free(list);
            ToConsole(String.Format("Spawn Removal: {0} spawn lines removed containing -{1}-. [{2} deleted].", count.ToString(), toRemove, deleted));
        }

        public static int Remove(XmlSpawner spawner, string toRemove, ref int deleted)
        {
            List<XmlSpawner.SpawnObject> remove = new List<XmlSpawner.SpawnObject>();

            foreach (var obj in spawner.SpawnObjects)
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

            int count = remove.Count;

            foreach (var obj in remove)
            {
                spawner.RemoveSpawnObject(obj);

                foreach (var e in obj.SpawnedObjects.OfType<IEntity>())
                {
                    e.Delete();
                    deleted++;
                }
            }

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
            else if (spawner is Spawner && ((Spawner)spawner).SpawnObjects != null && ((Spawner)spawner).SpawnObjects.Count > 0)
            {
                List<SpawnObject> names = ((Spawner)spawner).SpawnObjects;

                list = new string[names.Count];

                for (int i = 0; i < names.Count; i++)
                {
                    list[i] = names[i].SpawnName;
                }
            }

            return list;
        }

        public static void ReplaceSpawnersByRegionName(string region, Map map, string file)
        {
            string path = null;

            if (file != null)
            {
                path = string.Format("Spawns/{0}.xml", file);

                if (!File.Exists(path))
                {
                    ToConsole(String.Format("Cannot proceed. {0} does not exist.", file), ConsoleColor.Red);
                    return;
                }
            }

            foreach (var r in Region.Regions.Where(reg => reg.Map == map && reg.Name == region))
            {
                List<Item> list = r.GetEnumeratedItems().Where(i => i is XmlSpawner || i is Spawner).ToList();

                foreach (var item in list)
                {
                    item.Delete();
                }

                ToConsole(String.Format("Deleted {0} Spawners in {1}.", list.Count, region));
                ColUtility.Free(list);
            }

            if (path != null)
            {
                LoadFromXmlSpawner(path, map);
            }
        }

        public static void ReplaceSpawnersByRectangle(Rectangle2D rec, Map map, string file)
        {
            string path = null;

            if (file != null)
            {
                path = string.Format("Spawns/{0}.xml", file);

                if (!File.Exists(path))
                {
                    ToConsole(String.Format("Cannot proceed. {0} does not exist.", file), ConsoleColor.Red);
                    return;
                }
            }

            IPooledEnumerable eable = map.GetItemsInBounds(rec);
            List<Item> list = new List<Item>();

            foreach (Item item in eable)
            {
                if(item is XmlSpawner || item is Spawner)
                {
                    list.Add(item);
                }
            }

            foreach (var item in list)
                item.Delete();

            ToConsole(String.Format("Deleted {0} Spawners in {1}.", list.Count, map.ToString()));

            ColUtility.Free(list);
            eable.Free();

            if (path != null)
            {
                LoadFromXmlSpawner(path, map);
            }
        }

        public static void LoadFromXmlSpawner(string location, Map map, string prefix = null)
        {
            string filename = XmlSpawner.LocateFile(location);

            string SpawnerPrefix = prefix == null ? string.Empty : prefix;
            int processedmaps;
            int processedspawners;

            XmlSpawner.XmlLoadFromFile(filename, SpawnerPrefix, null, Point3D.Zero, map, false, 0, false, out processedmaps, out processedspawners);

            ToConsole(String.Format("Created {0} spawners from {1} with -{2}- prefix.", processedspawners, location, SpawnerPrefix == string.Empty ? "NO" : SpawnerPrefix));
        }

        #region XmlSpawner to Spawner Conversion
        public static void ConvertXmlToSpawners()
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

                                        if (loc != Point3D.Zero && spawnMap != null && spawnMap != Map.Internal)
                                        {
                                            if (!ConvertSpawnerByLocation(loc, spawnMap, dr, ref keep))
                                            {
                                                failed++;
                                            }
                                            else
                                            {
                                                converted++;
                                            }
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

        private static bool DeleteSpawner(string id)
        {
            if (id == null)
                return false;

            XmlSpawner spawner = World.Items.Values.OfType<XmlSpawner>().FirstOrDefault(s => s.UniqueId == id);

            if (spawner != null)
            {
                spawner.Delete();
                return true;
            }

            return false;
        }

        private static bool ConvertSpawner(XmlSpawner spawner, DataRow d, ref int keep)
        {
            if (spawner != null)
            {
                SpawnObject[] spawns = new SpawnObject[spawner.SpawnObjects.Length];

                for (int i = 0; i < spawner.SpawnObjects.Length; i++)
                {
                    var obj = spawner.SpawnObjects[i];

                    if (obj == null || obj.TypeName == null)
                        continue;

                    spawns[i] = new SpawnObject(obj.TypeName, obj.MaxCount);
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
                                                 spawner.SpawnRange,
                                                 spawns.ToList());

                newSpawner.Group = spawner.Group;
                newSpawner.Running = spawner.Running;

                newSpawner.MoveToWorld(spawner.Location, spawner.Map);
                spawner.Delete();
                return true;
            }

            return false;
        }

        private static bool HasSpecialXmlSpawnerString(SpawnObject[] spawns)
        {
            foreach (var obj in spawns)
            {
                if (obj.SpawnName != null)
                {
                    foreach (var s in _SpawnerSymbols)
                    {
                        if (obj.SpawnName.Contains(s))
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

        public static void RevertToXmlSpawners(Mobile from)
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

                ToConsole(String.Format("Found {0} Xmlspawner files for removal.", files == null ? 0 : files.Count), files != null && files.Count > 0 ? ConsoleColor.Green : ConsoleColor.Red);
                ToConsole("Deleting spawners...", ConsoleColor.Cyan);
                long start = Core.TickCount;

                if (files != null && files.Count > 0)
                {
                    int deletedxml = 0;
                    int nospawner = 0;

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

                                    if (DeleteSpawner(id))
                                    {
                                        deletedxml++;
                                    }
                                    else
                                    {
                                        nospawner++;
                                    }
                                }
                            }
                        }

                        fs.Close();
                    }

                    ToConsole(String.Format("Deleted {0} XmlSpawners [{2} no id] in {3} seconds.", deletedxml, nospawner, ((Core.TickCount - start) / 1000).ToString()), ConsoleColor.Cyan);
                }
                else
                {
                    ToConsole(String.Format("Directory Not Found: {0}", filename), ConsoleColor.Red);
                }
            }
        }
        #endregion

        /// <summary>
        /// Deletes all spawners from a specific file. This can be used to delete spawners from a specific system where the spawner wasn't 
        /// Generated from the Spawn Folder.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filename"></param>
        public static void RemoveSpawnsFromXmlFile(string directory, string filename)
        {
            if (System.IO.Directory.Exists(directory) == true)
            {
                List<string> files = null;

                try
                {
                    files = new List<string>(Directory.GetFiles(directory, filename + ".xml"));
                }
                catch { }

                ToConsole(String.Format("Found {0} Xmlspawner files for removal.", files == null ? "0" : files.Count.ToString()), files != null && files.Count > 0 ? ConsoleColor.Green : ConsoleColor.Red);
                ToConsole("Deleting spawners...", ConsoleColor.Cyan);

                if (files != null && files.Count > 0)
                {
                    int deletedxml = 0;

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

                                    if (DeleteSpawner(id))
                                    {
                                        deletedxml++;
                                    }
                                }
                            }
                        }

                        fs.Close();
                    }

                    ToConsole(String.Format("Deleted {0} XmlSpawners from {1}/{2}.xml.", deletedxml, directory, filename), ConsoleColor.Cyan);
                }
                else
                {
                    ToConsole(String.Format("File Not Found: {0}", filename), ConsoleColor.Red);
                }
            }
        }


        /// <summary>
        /// Used in place of XmlSpawner wipe all spawners. This iterates through the Spawn Folder and deletes those spawners only.
        /// This will keep spawners for seprate systems in place. This is called in DeleteWorld gump.
        /// </summary>
        public static void WipeSpawnersFromFile()
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
                ToConsole("Deleting spawners...", ConsoleColor.Cyan);
                long start = Core.TickCount;

                if (files != null && files.Count > 0)
                {
                    int deletedxml = 0;
                    int nodelelete = 0;

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

                                    if (DeleteSpawner(id))
                                    {
                                        deletedxml++;
                                    }
                                    else
                                    {
                                        bool deleted = false;

                                        try
                                        {
                                            Point3D loc = new Point3D(int.Parse((string)dr["CentreX"]), int.Parse((string)dr["CentreY"]), int.Parse((string)dr["CentreZ"]));
                                            Map spawnMap = Map.Parse((string)dr["Map"]);
                                            string name = (string)dr["Name"];

                                            if (spawnMap != null)
                                            {
                                                IPooledEnumerable eable = spawnMap.GetItemsInRange(loc, 0);

                                                foreach (Item item in eable)
                                                {
                                                    if (item is XmlSpawner && item.Name == name)
                                                    {
                                                        item.Delete();
                                                        deletedxml++;
                                                        deleted = true;
                                                        break;
                                                    }
                                                }

                                                eable.Free();
                                            }
                                        }
                                        catch { }

                                        if (!deleted)
                                        {
                                            nodelelete++;

                                            ToConsole(String.Format("Failed to Delete: {0} in {1}", (string)dr["Name"], file));
                                        }
                                    }
                                }
                            }
                        }

                        fs.Close();
                    }

                    ToConsole(String.Format("Deleted {0} XmlSpawners [{1} failed] in {2} seconds.", deletedxml, nodelelete, ((Core.TickCount - start) / 1000).ToString()), ConsoleColor.Cyan);
                }
                else
                {
                    ToConsole(String.Format("Directory Not Found: {0}", filename), ConsoleColor.Red);
                }
            }
        }
    }
}