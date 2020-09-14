using Server.Engines.Quests.Haven;
using Server.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Commands
{
    public class Decorate
    {
        private static string m_Key;
        public static string Key => m_Key;

        public static void Initialize()
        {
            CommandSystem.Register("Decorate", AccessLevel.Administrator, Decorate_OnCommand);
        }

        [Usage("Decorate")]
        [Description("Generates world decoration.")]
        private static void Decorate_OnCommand(CommandEventArgs e)
        {
            m_Mobile = e.Mobile;
            m_Count = 0;

            m_Mobile.SendMessage("Generating world decoration, please wait.");

            Generate("deco", "Data/Decoration/Britannia", Map.Trammel, Map.Felucca);
            Generate("deco", "Data/Decoration/Trammel", Map.Trammel);
            Generate("deco", "Data/Decoration/Felucca", Map.Felucca);
            Generate("deco", "Data/Decoration/Ilshenar", Map.Ilshenar);
            Generate("deco", "Data/Decoration/Malas", Map.Malas);
            Generate("deco", "Data/Decoration/Tokuno", Map.Tokuno);

            m_Mobile.SendMessage("World generating complete. {0} items were generated.", m_Count);
        }

        public static void Generate(string keyName, string folder, params Map[] maps)
        {
            m_Key = keyName;

            if (!Directory.Exists(folder))
                return;

            string[] files = Directory.GetFiles(folder, "*.cfg");

            for (int i = 0; i < files.Length; ++i)
            {
                List<DecorationList> list = DecorationList.ReadAll(files[i]);

                m_List = list;

                for (int j = 0; j < list.Count; ++j)
                    m_Count += list[j].Generate(maps);

                ColUtility.Free(list);
            }

            ColUtility.Free(m_List);
        }

        public static void GenerateFromFile(string keyName, string path, params Map[] maps)
        {
            m_Key = keyName;

            if (!File.Exists(path))
                return;

            List<DecorationList> list = DecorationList.ReadAll(path);
            int count = 0;

            m_List = list;

            for (int j = 0; j < list.Count; ++j)
                count += list[j].Generate(maps);

            ColUtility.Free(m_List);
            ColUtility.Free(list);

            Utility.WriteConsoleColor(ConsoleColor.Magenta, "[Decoration] Generated decoration from {0} complete, including {1} items!", path, count);
        }

        public static void GenerateRestricted(string keyName, string folder, Type restrictType, bool derivesFrom, params Map[] maps)
        {
            m_Key = keyName;

            if (!Directory.Exists(folder))
                return;

            string[] files = Directory.GetFiles(folder, "*.cfg");
            int count = 0;

            for (int i = 0; i < files.Length; ++i)
            {
                List<DecorationList> list = DecorationList.ReadRestricted(files[i], restrictType, derivesFrom);

                m_List = list;

                for (int j = 0; j < list.Count; ++j)
                    count += list[j].Generate(maps);

                ColUtility.Free(list);
            }

            ColUtility.Free(m_List);

            Utility.WriteConsoleColor(ConsoleColor.Magenta, "[Decoration] Added {0} decoration items oftype {1} from folder {2} [{3} files]!", count, restrictType.Name, folder, files.Length);
        }

        public static Item FindByID(int id)
        {
            if (m_List == null)
                return null;

            for (int j = 0; j < m_List.Count; ++j)
            {
                DecorationList list = m_List[j];

                if (list.ID == id)
                    return list.Constructed;
            }

            return null;
        }

        private static List<DecorationList> m_List;

        private static Mobile m_Mobile;
        private static int m_Count;
    }

    public class DecorationList
    {
        private Type m_Type;
        private int m_ItemID;
        private string[] m_Params;
        private ArrayList m_Entries;

        private Item m_Constructed;

        public Item Constructed => m_Constructed;

        public int ID
        {
            get
            {
                for (int i = 0; i < m_Params.Length; ++i)
                {
                    if (m_Params[i].StartsWith("ID"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            return Utility.ToInt32(m_Params[i].Substring(++indexOf));
                    }
                }

                return 0;
            }
        }

        private static readonly Type typeofStatic = typeof(Static);
        private static readonly Type typeofLocalizedStatic = typeof(LocalizedStatic);
        private static readonly Type typeofBaseDoor = typeof(BaseDoor);
        private static readonly Type typeofAnkhWest = typeof(AnkhWest);
        private static readonly Type typeofAnkhNorth = typeof(AnkhNorth);
        private static readonly Type typeofBeverage = typeof(BaseBeverage);
        private static readonly Type typeofLocalizedSign = typeof(LocalizedSign);
        private static readonly Type typeofMarkContainer = typeof(MarkContainer);
        private static readonly Type typeofWarningItem = typeof(WarningItem);
        private static readonly Type typeofHintItem = typeof(HintItem);
        private static readonly Type typeofCannon = typeof(Cannon);
        private static readonly Type typeofSerpentPillar = typeof(SerpentPillar);
        private static readonly Type typeofAddonComponent = typeof(AddonComponent);

        public Item Construct()
        {
            if (m_Type == null)
                return null;

            Item item;

            try
            {
                if (m_Type == typeofStatic)
                {
                    item = new Static(m_ItemID);
                }
                else if (m_Type == typeof(SecretSwitch))
                {
                    int id = 0;

                    for (int i = 0; i < m_Params.Length; ++i)
                    {
                        if (m_Params[i].StartsWith("SecretWall"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                            {
                                id = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                                break;
                            }
                        }
                    }

                    Item wall = Decorate.FindByID(id);

                    item = new SecretSwitch(m_ItemID, wall as SecretWall);
                }
                else if (m_Type == typeof(SecretWall))
                {
                    SecretWall wall = new SecretWall(m_ItemID);

                    for (int i = 0; i < m_Params.Length; ++i)
                    {
                        if (m_Params[i].StartsWith("MapDest"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                wall.MapDest = Map.Parse(m_Params[i].Substring(++indexOf));
                        }
                        else if (m_Params[i].StartsWith("PointDest"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                wall.PointDest = Point3D.Parse(m_Params[i].Substring(++indexOf));
                        }
                        else if (m_Params[i].StartsWith("Unlocked"))
                        {
                            wall.Locked = false;
                        }
                    }

                    item = wall;
                }
                else if (m_Type == typeofLocalizedStatic)
                {
                    int labelNumber = 0;

                    for (int i = 0; i < m_Params.Length; ++i)
                    {
                        if (m_Params[i].StartsWith("LabelNumber"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                            {
                                labelNumber = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                                break;
                            }
                        }
                    }

                    item = new LocalizedStatic(m_ItemID, labelNumber);
                }
                else if (m_Type == typeofLocalizedSign)
                {
                    int labelNumber = 0;

                    for (int i = 0; i < m_Params.Length; ++i)
                    {
                        if (m_Params[i].StartsWith("LabelNumber"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                            {
                                labelNumber = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                                break;
                            }
                        }
                    }

                    item = new LocalizedSign(m_ItemID, labelNumber);
                }
                else if (m_Type == typeofAnkhWest || m_Type == typeofAnkhNorth)
                {
                    bool bloodied = false;

                    for (int i = 0; !bloodied && i < m_Params.Length; ++i)
                        bloodied = (m_Params[i] == "Bloodied");

                    if (m_Type == typeofAnkhWest)
                        item = new AnkhWest(bloodied);
                    else
                        item = new AnkhNorth(bloodied);
                }
                else if (m_Type == typeofMarkContainer)
                {
                    bool bone = false;
                    bool locked = false;
                    Map map = Map.Malas;

                    for (int i = 0; i < m_Params.Length; ++i)
                    {
                        if (m_Params[i] == "Bone")
                        {
                            bone = true;
                        }
                        else if (m_Params[i] == "Locked")
                        {
                            locked = true;
                        }
                        else if (m_Params[i].StartsWith("TargetMap"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                map = Map.Parse(m_Params[i].Substring(++indexOf));
                        }
                    }

                    MarkContainer mc = new MarkContainer(bone, locked);

                    mc.TargetMap = map;
                    mc.Description = "strange location";

                    item = mc;
                }
                else if (m_Type == typeofHintItem)
                {
                    int range = 0;
                    int messageNumber = 0;
                    string messageString = null;
                    int hintNumber = 0;
                    string hintString = null;
                    TimeSpan resetDelay = TimeSpan.Zero;

                    for (int i = 0; i < m_Params.Length; ++i)
                    {
                        if (m_Params[i].StartsWith("Range"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                range = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                        }
                        else if (m_Params[i].StartsWith("WarningString"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                messageString = m_Params[i].Substring(++indexOf);
                        }
                        else if (m_Params[i].StartsWith("WarningNumber"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                messageNumber = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                        }
                        else if (m_Params[i].StartsWith("HintString"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                hintString = m_Params[i].Substring(++indexOf);
                        }
                        else if (m_Params[i].StartsWith("HintNumber"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                hintNumber = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                        }
                        else if (m_Params[i].StartsWith("ResetDelay"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                resetDelay = TimeSpan.Parse(m_Params[i].Substring(++indexOf));
                        }
                    }

                    HintItem hi = new HintItem(m_ItemID, range, messageNumber, hintNumber);

                    hi.WarningString = messageString;
                    hi.HintString = hintString;
                    hi.ResetDelay = resetDelay;

                    item = hi;
                }
                else if (m_Type == typeofWarningItem)
                {
                    int range = 0;
                    int messageNumber = 0;
                    string messageString = null;
                    TimeSpan resetDelay = TimeSpan.Zero;

                    for (int i = 0; i < m_Params.Length; ++i)
                    {
                        if (m_Params[i].StartsWith("Range"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                range = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                        }
                        else if (m_Params[i].StartsWith("WarningString"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                messageString = m_Params[i].Substring(++indexOf);
                        }
                        else if (m_Params[i].StartsWith("WarningNumber"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                messageNumber = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                        }
                        else if (m_Params[i].StartsWith("ResetDelay"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                resetDelay = TimeSpan.Parse(m_Params[i].Substring(++indexOf));
                        }
                    }

                    WarningItem wi = new WarningItem(m_ItemID, range, messageNumber);

                    wi.WarningString = messageString;
                    wi.ResetDelay = resetDelay;

                    item = wi;
                }
                else if (m_Type == typeofCannon)
                {
                    CannonDirection direction = CannonDirection.North;

                    for (int i = 0; i < m_Params.Length; ++i)
                    {
                        if (m_Params[i].StartsWith("CannonDirection"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                direction = (CannonDirection)Enum.Parse(typeof(CannonDirection), m_Params[i].Substring(++indexOf), true);
                        }
                    }

                    item = new Cannon(direction);
                }
                else if (m_Type == typeofSerpentPillar)
                {
                    string word = null;
                    Rectangle2D destination = new Rectangle2D();

                    for (int i = 0; i < m_Params.Length; ++i)
                    {
                        if (m_Params[i].StartsWith("Word"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                word = m_Params[i].Substring(++indexOf);
                        }
                        else if (m_Params[i].StartsWith("DestStart"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                destination.Start = Point2D.Parse(m_Params[i].Substring(++indexOf));
                        }
                        else if (m_Params[i].StartsWith("DestEnd"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                destination.End = Point2D.Parse(m_Params[i].Substring(++indexOf));
                        }
                    }

                    item = new SerpentPillar(word, destination);
                }
                else if (m_Type.IsSubclassOf(typeofBeverage))
                {
                    BeverageType content = BeverageType.Liquor;
                    bool fill = false;

                    for (int i = 0; !fill && i < m_Params.Length; ++i)
                    {
                        if (m_Params[i].StartsWith("Content"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                            {
                                content = (BeverageType)Enum.Parse(typeof(BeverageType), m_Params[i].Substring(++indexOf), true);
                                fill = true;
                            }
                        }
                    }

                    if (fill)
                        item = (Item)Activator.CreateInstance(m_Type, new object[] { content });
                    else
                        item = (Item)Activator.CreateInstance(m_Type);
                }
                else if (m_Type.IsSubclassOf(typeofBaseDoor))
                {
                    DoorFacing facing = DoorFacing.WestCW;

                    for (int i = 0; i < m_Params.Length; ++i)
                    {
                        if (m_Params[i].StartsWith("Facing"))
                        {
                            int indexOf = m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                            {
                                facing = (DoorFacing)Enum.Parse(typeof(DoorFacing), m_Params[i].Substring(++indexOf), true);
                                break;
                            }
                        }
                    }

                    item = (Item)Activator.CreateInstance(m_Type, new object[] { facing });
                }
                else if (m_Type.IsSubclassOf(typeofAddonComponent))
                {
                    switch (m_Type.Name)
                    {
                        default:
                            Utility.WriteConsoleColor(ConsoleColor.Red, "[Decorate] Warning: {0} addon component has null Addon. Will delete after next server start", m_Type.Name);
                            item = (Item)Activator.CreateInstance(m_Type);
                            break;
                        case "ArcheryButte":
                            item = new ArcheryButteAddon(m_ItemID == 0x100A ? AddonFacing.East : AddonFacing.South);
                            break;
                        case "TrainingDummy":
                            if (m_ItemID == 0x1074)
                            {
                                item = new TrainingDummyEastAddon();
                            }
                            else
                            {
                                item = new TrainingDummySouthAddon();
                            }
                            break;
                        case "DartBoard":
                            if (m_ItemID == 0x1E2F)
                            {
                                item = new DartBoardEastAddon();
                            }
                            else
                            {
                                item = new DartBoardSouthAddon();
                            }
                            break;
                    }
                }
                else
                {
                    item = (Item)Activator.CreateInstance(m_Type);
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Bad type: {0}", m_Type), e);
            }

            if (item is BaseAddon)
            {
                if (m_ItemID > 0)
                {
                    List<AddonComponent> comps = ((BaseAddon)item).Components;

                    for (int i = 0; i < comps.Count; ++i)
                    {
                        AddonComponent comp = comps[i];

                        if (comp.Offset == Point3D.Zero)
                            comp.ItemID = m_ItemID;
                    }
                }
            }
            else if (item is BaseLight)
            {
                bool unlit = false, unprotected = false;

                for (int i = 0; i < m_Params.Length; ++i)
                {
                    if (!unlit && m_Params[i] == "Unlit")
                        unlit = true;
                    else if (!unprotected && m_Params[i] == "Unprotected")
                        unprotected = true;

                    if (unlit && unprotected)
                        break;
                }

                if (!unlit)
                    ((BaseLight)item).Ignite();
                if (!unprotected)
                    ((BaseLight)item).Protected = true;

                if (m_ItemID > 0)
                    item.ItemID = m_ItemID;
            }
            else if (item is Mobiles.Spawner)
            {
                Mobiles.Spawner sp = (Mobiles.Spawner)item;

                sp.NextSpawn = TimeSpan.Zero;

                for (int i = 0; i < m_Params.Length; ++i)
                {
                    if (m_Params[i].StartsWith("Spawn"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.SpawnObjects.Add(new Mobiles.SpawnObject(m_Params[i].Substring(++indexOf)));
                    }
                    else if (m_Params[i].StartsWith("MinDelay"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.MinDelay = TimeSpan.Parse(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("MaxDelay"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.MaxDelay = TimeSpan.Parse(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("NextSpawn"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.NextSpawn = TimeSpan.Parse(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("Count"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.MaxCount = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("Team"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.Team = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("HomeRange"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.HomeRange = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("Running"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.Running = Utility.ToBoolean(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("Group"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.Group = Utility.ToBoolean(m_Params[i].Substring(++indexOf));
                    }
                }
            }
            else if (item is RecallRune)
            {
                RecallRune rune = (RecallRune)item;

                for (int i = 0; i < m_Params.Length; ++i)
                {
                    if (m_Params[i].StartsWith("Description"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            rune.Description = m_Params[i].Substring(++indexOf);
                    }
                    else if (m_Params[i].StartsWith("Marked"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            rune.Marked = Utility.ToBoolean(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("TargetMap"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            rune.TargetMap = Map.Parse(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("Target"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            rune.Target = Point3D.Parse(m_Params[i].Substring(++indexOf));
                    }
                }
            }
            else if (item is SkillTeleporter)
            {
                SkillTeleporter tp = (SkillTeleporter)item;

                for (int i = 0; i < m_Params.Length; ++i)
                {
                    if (m_Params[i].StartsWith("Skill"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Skill = (SkillName)Enum.Parse(typeof(SkillName), m_Params[i].Substring(++indexOf), true);
                    }
                    else if (m_Params[i].StartsWith("RequiredFixedPoint"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Required = Utility.ToInt32(m_Params[i].Substring(++indexOf)) * 0.1;
                    }
                    else if (m_Params[i].StartsWith("Required"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Required = Utility.ToDouble(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("MessageString"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.MessageString = m_Params[i].Substring(++indexOf);
                    }
                    else if (m_Params[i].StartsWith("MessageNumber"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.MessageNumber = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("PointDest"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.PointDest = Point3D.Parse(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("MapDest"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.MapDest = Map.Parse(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("Creatures"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Creatures = Utility.ToBoolean(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("SourceEffect"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.SourceEffect = Utility.ToBoolean(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("DestEffect"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.DestEffect = Utility.ToBoolean(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("SoundID"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.SoundID = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("Delay"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Delay = TimeSpan.Parse(m_Params[i].Substring(++indexOf));
                    }
                }

                if (m_ItemID > 0)
                    item.ItemID = m_ItemID;
            }
            else if (item is KeywordTeleporter)
            {
                KeywordTeleporter tp = (KeywordTeleporter)item;

                for (int i = 0; i < m_Params.Length; ++i)
                {
                    if (m_Params[i].StartsWith("Substring"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Substring = m_Params[i].Substring(++indexOf);
                    }
                    else if (m_Params[i].StartsWith("Keyword"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Keyword = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("Range"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Range = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("PointDest"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.PointDest = Point3D.Parse(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("MapDest"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.MapDest = Map.Parse(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("Creatures"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Creatures = Utility.ToBoolean(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("SourceEffect"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.SourceEffect = Utility.ToBoolean(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("DestEffect"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.DestEffect = Utility.ToBoolean(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("SoundID"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.SoundID = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("Delay"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Delay = TimeSpan.Parse(m_Params[i].Substring(++indexOf));
                    }
                }

                if (m_ItemID > 0)
                    item.ItemID = m_ItemID;
            }
            else if (item is Teleporter)
            {
                Teleporter tp = (Teleporter)item;

                for (int i = 0; i < m_Params.Length; ++i)
                {
                    if (m_Params[i].StartsWith("PointDest"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.PointDest = Point3D.Parse(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("MapDest"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.MapDest = Map.Parse(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("Creatures"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Creatures = Utility.ToBoolean(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("SourceEffect"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.SourceEffect = Utility.ToBoolean(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("DestEffect"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.DestEffect = Utility.ToBoolean(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("SoundID"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.SoundID = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                    }
                    else if (m_Params[i].StartsWith("Delay"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Delay = TimeSpan.Parse(m_Params[i].Substring(++indexOf));
                    }
                }

                if (m_ItemID > 0)
                    item.ItemID = m_ItemID;
            }
            else if (item is FillableContainer)
            {
                FillableContainer cont = (FillableContainer)item;

                for (int i = 0; i < m_Params.Length; ++i)
                {
                    if (m_Params[i].StartsWith("ContentType"))
                    {
                        int indexOf = m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            cont.ContentType = (FillableContentType)Enum.Parse(typeof(FillableContentType), m_Params[i].Substring(++indexOf), true);
                    }
                }

                if (m_ItemID > 0)
                    item.ItemID = m_ItemID;
            }
            else if (item is Moongate)
            {
                Moongate gate = (Moongate)item;

                foreach (string param in m_Params)
                {
                    int indexOf = param.IndexOf('=');

                    if (param.StartsWith("TargetMap"))
                        gate.TargetMap = Map.Parse(param.Substring(++indexOf));
                    else if (param.StartsWith("Target"))
                        gate.Target = Point3D.Parse(param.Substring(++indexOf));
                }
            }
            else if (item is TeleportRope)
            {
                TeleportRope rope = (TeleportRope)item;

                foreach (string param in m_Params)
                {
                    int indexOf = param.IndexOf('=');

                    if (param.StartsWith("ToMap"))
                        rope.ToMap = Map.Parse(param.Substring(++indexOf));
                    else if (param.StartsWith("ToLocation"))
                        rope.ToLocation = Point3D.Parse(param.Substring(++indexOf));
                }
            }
            else if (item is InstanceExitGate)
            {
                InstanceExitGate gate = (InstanceExitGate)item;

                foreach (string param in m_Params)
                {
                    int indexOf = param.IndexOf('=');

                    if (param.StartsWith("MapDest"))
                        gate.MapDest = Map.Parse(param.Substring(++indexOf));
                    else if (param.StartsWith("LocDest"))
                        gate.LocDest = Point3D.Parse(param.Substring(++indexOf));
                }
            }
            else if (m_ItemID > 0)
            {
                item.ItemID = m_ItemID;
            }

            item.Movable = false;

            for (int i = 0; i < m_Params.Length; ++i)
            {
                if (m_Params[i].StartsWith("Light"))
                {
                    int indexOf = m_Params[i].IndexOf('=');

                    if (indexOf >= 0)
                        item.Light = (LightType)Enum.Parse(typeof(LightType), m_Params[i].Substring(++indexOf), true);
                }
                else if (m_Params[i].StartsWith("Hue"))
                {
                    int indexOf = m_Params[i].IndexOf('=');

                    if (indexOf >= 0)
                    {
                        int hue = Utility.ToInt32(m_Params[i].Substring(++indexOf));

                        if (item is DyeTub)
                            ((DyeTub)item).DyedHue = hue;
                        else
                            item.Hue = hue;
                    }
                }
                else if (m_Params[i].StartsWith("Name"))
                {
                    int indexOf = m_Params[i].IndexOf('=');

                    if (indexOf >= 0)
                        item.Name = m_Params[i].Substring(++indexOf);
                }
                else if (m_Params[i].StartsWith("Amount"))
                {
                    int indexOf = m_Params[i].IndexOf('=');

                    if (indexOf >= 0)
                    {
                        // Must supress stackable warnings
                        bool wasStackable = item.Stackable;

                        item.Stackable = true;
                        item.Amount = Utility.ToInt32(m_Params[i].Substring(++indexOf));
                        item.Stackable = wasStackable;
                    }
                }
            }

            return item;
        }

        private static readonly Queue m_DeleteQueue = new Queue();

        private static bool FindItem(int x, int y, int z, Map map, Item srcItem)
        {
            int itemID = srcItem.ItemID;

            bool res = false;

            IPooledEnumerable eable;

            if (srcItem is BaseDoor)
            {
                eable = map.GetItemsInRange(new Point3D(x, y, z), 1);

                foreach (Item item in eable)
                {
                    if (!(item is BaseDoor))
                        continue;

                    BaseDoor bd = (BaseDoor)item;
                    Point3D p;
                    int bdItemID;

                    if (bd.Open)
                    {
                        p = new Point3D(bd.X - bd.Offset.X, bd.Y - bd.Offset.Y, bd.Z - bd.Offset.Z);
                        bdItemID = bd.ClosedID;
                    }
                    else
                    {
                        p = bd.Location;
                        bdItemID = bd.ItemID;
                    }

                    if (p.X != x || p.Y != y)
                        continue;

                    if (item.Z == z && bdItemID == itemID)
                        res = true;
                    else if (Math.Abs(item.Z - z) < 8)
                        m_DeleteQueue.Enqueue(item);
                }
            }
            else if ((TileData.ItemTable[itemID & TileData.MaxItemValue].Flags & TileFlag.LightSource) != 0)
            {
                eable = map.GetItemsInRange(new Point3D(x, y, z), 0);

                LightType lt = srcItem.Light;
                string srcName = srcItem.ItemData.Name;

                foreach (Item item in eable)
                {
                    if (item.Z == z)
                    {
                        if (item.ItemID == itemID)
                        {
                            if (item.Light != lt)
                                m_DeleteQueue.Enqueue(item);
                            else
                                res = true;
                        }
                        else if ((item.ItemData.Flags & TileFlag.LightSource) != 0 && item.ItemData.Name == srcName)
                        {
                            m_DeleteQueue.Enqueue(item);
                        }
                    }
                }
            }
            else if (srcItem is Teleporter || srcItem is FillableContainer || srcItem is BaseBook)
            {
                eable = map.GetItemsInRange(new Point3D(x, y, z), 0);

                Type type = srcItem.GetType();

                foreach (Item item in eable)
                {
                    if (item.Z == z && item.ItemID == itemID)
                    {
                        if (item.GetType() != type)
                            m_DeleteQueue.Enqueue(item);
                        else
                            res = true;
                    }
                }
            }
            else
            {
                eable = map.GetItemsInRange(new Point3D(x, y, z), 0);

                foreach (Item item in eable)
                {
                    if (item.Z == z && item.ItemID == itemID)
                    {
                        eable.Free();
                        return true;
                    }
                }
            }

            eable.Free();

            while (m_DeleteQueue.Count > 0)
                ((Item)m_DeleteQueue.Dequeue()).Delete();

            return res;
        }

        public int Generate(Map[] maps)
        {
            int count = 0;

            Item item = null;

            for (int i = 0; i < m_Entries.Count; ++i)
            {
                DecorationEntry entry = (DecorationEntry)m_Entries[i];
                Point3D loc = entry.Location;
                string extra = entry.Extra;

                for (int j = 0; j < maps.Length; ++j)
                {
                    if (item == null)
                        item = Construct();

                    m_Constructed = item;

                    if (item == null)
                        continue;

                    if (FindItem(loc.X, loc.Y, loc.Z, maps[j], item))
                    {
                    }
                    else
                    {
                        WeakEntityCollection.Add(Decorate.Key, item);
                        item.MoveToWorld(loc, maps[j]);
                        ++count;

                        if (item is BaseDoor)
                        {
                            IPooledEnumerable eable = maps[j].GetItemsInRange(loc, 1);

                            Type itemType = item.GetType();

                            foreach (Item link in eable)
                            {
                                if (link != item && link.Z == item.Z && link.GetType() == itemType)
                                {
                                    ((BaseDoor)item).Link = (BaseDoor)link;
                                    ((BaseDoor)link).Link = (BaseDoor)item;
                                    break;
                                }
                            }

                            eable.Free();
                        }
                        else if (item is MarkContainer)
                        {
                            try
                            {
                                ((MarkContainer)item).Target = Point3D.Parse(extra);
                            }
                            catch (Exception e)
                            {
                                Server.Diagnostics.ExceptionLogging.LogException(e);
                            }
                        }

                        item = null;
                    }
                }
            }

            if (item != null)
                item.Delete();

            return count;
        }

        /// <summary>
        /// Only decorate certain types. Use if a type were all deleted, etc.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="derivesFrom">indicates if DecorationList Type derives from param "type"</param>
        /// <returns></returns>
        public static List<DecorationList> ReadRestricted(string path, Type type, bool derivesFrom)
        {
            return new List<DecorationList>(ReadAll(path).Where(dl => dl.m_Type == type || (derivesFrom && dl.m_Type.IsSubclassOf(type))));
        }

        public static List<DecorationList> ReadAll(string path)
        {
            using (StreamReader ip = new StreamReader(path))
            {
                List<DecorationList> list = new List<DecorationList>();

                for (DecorationList v = Read(ip); v != null; v = Read(ip))
                    list.Add(v);

                return list;
            }
        }

        private static readonly string[] m_EmptyParams = new string[0];

        public static DecorationList Read(StreamReader ip)
        {
            string line;

            while ((line = ip.ReadLine()) != null)
            {
                line = line.Trim();

                if (line.Length > 0 && !line.StartsWith("#"))
                    break;
            }

            if (string.IsNullOrEmpty(line))
                return null;

            DecorationList list = new DecorationList();

            int indexOf = line.IndexOf(' ');

            list.m_Type = ScriptCompiler.FindTypeByName(line.Substring(0, indexOf++), true);

            if (list.m_Type == null)
                throw new ArgumentException(string.Format("Type not found for header: '{0}'", line));

            line = line.Substring(indexOf);
            indexOf = line.IndexOf('(');
            if (indexOf >= 0)
            {
                list.m_ItemID = Utility.ToInt32(line.Substring(0, indexOf - 1));

                string parms = line.Substring(++indexOf);

                if (line.EndsWith(")"))
                    parms = parms.Substring(0, parms.Length - 1);

                list.m_Params = parms.Split(';');

                for (int i = 0; i < list.m_Params.Length; ++i)
                    list.m_Params[i] = list.m_Params[i].Trim();
            }
            else
            {
                list.m_ItemID = Utility.ToInt32(line);
                list.m_Params = m_EmptyParams;
            }

            list.m_Entries = new ArrayList();

            while ((line = ip.ReadLine()) != null)
            {
                line = line.Trim();

                if (line.Length == 0)
                    break;

                if (line.StartsWith("#"))
                    continue;

                list.m_Entries.Add(new DecorationEntry(line));
            }

            return list;
        }
    }

    public class DecorationEntry
    {
        private readonly Point3D m_Location;
        private readonly string m_Extra;

        public Point3D Location => m_Location;
        public string Extra => m_Extra;

        public DecorationEntry(string line)
        {
            string x, y, z;

            Pop(out x, ref line);
            Pop(out y, ref line);
            Pop(out z, ref line);

            m_Location = new Point3D(Utility.ToInt32(x), Utility.ToInt32(y), Utility.ToInt32(z));
            m_Extra = line;
        }

        public void Pop(out string v, ref string line)
        {
            int space = line.IndexOf(' ');

            if (space >= 0)
            {
                v = line.Substring(0, space++);
                line = line.Substring(space);
            }
            else
            {
                v = line;
                line = "";
            }
        }
    }
}
