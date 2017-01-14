using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Server.Engines.Quests.Haven;
using Server.Engines.Quests.Necro;
using Server.Items;

namespace Server.Commands
{
    public class DecorateDelete
    {
        public static void Initialize()
        {
            CommandSystem.Register("DecorateDelete", AccessLevel.Administrator, new CommandEventHandler(DecorateDelete_OnCommand));
        }

        [Usage("DecorateDelete")]
        [Description("Deletes world decoration.")]
        private static void DecorateDelete_OnCommand(CommandEventArgs e)
        {
            m_Mobile = e.Mobile;
            m_Count = 0;

            m_Mobile.SendMessage("Deleting world decoration, please wait.");

			// We still do all this for backward-compatibility
            Generate("Data/Decoration/Britannia", Map.Trammel, Map.Felucca);
            Generate("Data/Decoration/Trammel", Map.Trammel);
            Generate("Data/Decoration/Felucca", Map.Felucca);
            Generate("Data/Decoration/Ilshenar", Map.Ilshenar);
            Generate("Data/Decoration/Malas", Map.Malas);
            Generate("Data/Decoration/Tokuno", Map.Tokuno);

			WeakEntityCollection.Delete("deco");

            m_Mobile.SendMessage("Deleting complete.");
        }

        public static void Generate(string folder, params Map[] maps)
        {
            if (!Directory.Exists(folder))
                return;

            string[] files = Directory.GetFiles(folder, "*.cfg");

            for (int i = 0; i < files.Length; ++i)
            {
                ArrayList list = DecorationListDelete.ReadAll(files[i]);

                #region Mondain's Legacy
                m_List = list;
                #endregion

                for (int j = 0; j < list.Count; ++j)
                    m_Count += ((DecorationListDelete)list[j]).Generate(maps);
            }
        }

        #region Mondain's Legacy
        public static Item FindByID(int id)
        {
            if (m_List == null)
                return null;

            for (int j = 0; j < m_List.Count; ++j)
            {
                DecorationList list = (DecorationList)m_List[j];

                if (list.ID == id)
                    return list.Constructed;
            }

            return null;
        }

        private static ArrayList m_List;
        #endregion

        private static Mobile m_Mobile;
        private static int m_Count;
    }

    public class DecorationListDelete
    {
        private Type m_Type;
        private int m_ItemID;
        private string[] m_Params;
        private ArrayList m_Entries;

        #region Mondain's Legacy
        private Item m_Constructed;

        public Item Constructed
        {
            get
            {
                return this.m_Constructed;
            }
        }

        public int ID
        {
            get
            {
                for (int i = 0; i < this.m_Params.Length; ++i)
                {
                    if (this.m_Params[i].StartsWith("ID"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            return Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                    }
                }

                return 0;
            }
        }
        #endregion

        public DecorationListDelete()
        {
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

        public Item Construct()
        {
            Item item;

            try
            {
                if (this.m_Type == typeofStatic)
                {
                    item = new Static(this.m_ItemID);
                }
                #region Mondain's Legacy
                else if (this.m_Type == typeof(SecretSwitch))
                {
                    int id = 0;

                    for (int i = 0; i < this.m_Params.Length; ++i)
                    {
                        if (this.m_Params[i].StartsWith("SecretWall"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                            {
                                id = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                                break;
                            }
                        }
                    }

                    Item wall = Decorate.FindByID(id);

                    item = new SecretSwitch(this.m_ItemID, wall as SecretWall);
                }
                else if (this.m_Type == typeof(SecretWall))
                {
                    SecretWall wall = new SecretWall(this.m_ItemID);

                    for (int i = 0; i < this.m_Params.Length; ++i)
                    {
                        if (this.m_Params[i].StartsWith("MapDest"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                wall.MapDest = Map.Parse(this.m_Params[i].Substring(++indexOf));
                        }
                        else if (this.m_Params[i].StartsWith("PointDest"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                wall.PointDest = Point3D.Parse(this.m_Params[i].Substring(++indexOf));
                        }
                    }

                    item = wall;
                }
                #endregion
                else if (this.m_Type == typeofLocalizedStatic)
                {
                    int labelNumber = 0;

                    for (int i = 0; i < this.m_Params.Length; ++i)
                    {
                        if (this.m_Params[i].StartsWith("LabelNumber"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                            {
                                labelNumber = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                                break;
                            }
                        }
                    }

                    item = new LocalizedStatic(this.m_ItemID, labelNumber);
                }
                else if (this.m_Type == typeofLocalizedSign)
                {
                    int labelNumber = 0;

                    for (int i = 0; i < this.m_Params.Length; ++i)
                    {
                        if (this.m_Params[i].StartsWith("LabelNumber"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                            {
                                labelNumber = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                                break;
                            }
                        }
                    }

                    item = new LocalizedSign(this.m_ItemID, labelNumber);
                }
                else if (this.m_Type == typeofAnkhWest || this.m_Type == typeofAnkhNorth)
                {
                    bool bloodied = false;

                    for (int i = 0; !bloodied && i < this.m_Params.Length; ++i)
                        bloodied = (this.m_Params[i] == "Bloodied");

                    if (this.m_Type == typeofAnkhWest)
                        item = new AnkhWest(bloodied);
                    else
                        item = new AnkhNorth(bloodied);
                }
                else if (this.m_Type == typeofMarkContainer)
                {
                    bool bone = false;
                    bool locked = false;
                    Map map = Map.Malas;

                    for (int i = 0; i < this.m_Params.Length; ++i)
                    {
                        if (this.m_Params[i] == "Bone")
                        {
                            bone = true;
                        }
                        else if (this.m_Params[i] == "Locked")
                        {
                            locked = true;
                        }
                        else if (this.m_Params[i].StartsWith("TargetMap"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                map = Map.Parse(this.m_Params[i].Substring(++indexOf));
                        }
                    }

                    MarkContainer mc = new MarkContainer(bone, locked);

                    mc.TargetMap = map;
                    mc.Description = "strange location";

                    item = mc;
                }
                else if (this.m_Type == typeofHintItem)
                {
                    int range = 0;
                    int messageNumber = 0;
                    string messageString = null;
                    int hintNumber = 0;
                    string hintString = null;
                    TimeSpan resetDelay = TimeSpan.Zero;

                    for (int i = 0; i < this.m_Params.Length; ++i)
                    {
                        if (this.m_Params[i].StartsWith("Range"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                range = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                        }
                        else if (this.m_Params[i].StartsWith("WarningString"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                messageString = this.m_Params[i].Substring(++indexOf);
                        }
                        else if (this.m_Params[i].StartsWith("WarningNumber"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                messageNumber = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                        }
                        else if (this.m_Params[i].StartsWith("HintString"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                hintString = this.m_Params[i].Substring(++indexOf);
                        }
                        else if (this.m_Params[i].StartsWith("HintNumber"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                hintNumber = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                        }
                        else if (this.m_Params[i].StartsWith("ResetDelay"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                resetDelay = TimeSpan.Parse(this.m_Params[i].Substring(++indexOf));
                        }
                    }

                    HintItem hi = new HintItem(this.m_ItemID, range, messageNumber, hintNumber);

                    hi.WarningString = messageString;
                    hi.HintString = hintString;
                    hi.ResetDelay = resetDelay;

                    item = hi;
                }
                else if (this.m_Type == typeofWarningItem)
                {
                    int range = 0;
                    int messageNumber = 0;
                    string messageString = null;
                    TimeSpan resetDelay = TimeSpan.Zero;

                    for (int i = 0; i < this.m_Params.Length; ++i)
                    {
                        if (this.m_Params[i].StartsWith("Range"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                range = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                        }
                        else if (this.m_Params[i].StartsWith("WarningString"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                messageString = this.m_Params[i].Substring(++indexOf);
                        }
                        else if (this.m_Params[i].StartsWith("WarningNumber"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                messageNumber = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                        }
                        else if (this.m_Params[i].StartsWith("ResetDelay"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                resetDelay = TimeSpan.Parse(this.m_Params[i].Substring(++indexOf));
                        }
                    }

                    WarningItem wi = new WarningItem(this.m_ItemID, range, messageNumber);

                    wi.WarningString = messageString;
                    wi.ResetDelay = resetDelay;

                    item = wi;
                }
                else if (this.m_Type == typeofCannon)
                {
                    CannonDirection direction = CannonDirection.North;

                    for (int i = 0; i < this.m_Params.Length; ++i)
                    {
                        if (this.m_Params[i].StartsWith("CannonDirection"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                direction = (CannonDirection)Enum.Parse(typeof(CannonDirection), this.m_Params[i].Substring(++indexOf), true);
                        }
                    }

                    item = new Cannon(direction);
                }
                else if (this.m_Type == typeofSerpentPillar)
                {
                    string word = null;
                    Rectangle2D destination = new Rectangle2D();

                    for (int i = 0; i < this.m_Params.Length; ++i)
                    {
                        if (this.m_Params[i].StartsWith("Word"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                word = this.m_Params[i].Substring(++indexOf);
                        }
                        else if (this.m_Params[i].StartsWith("DestStart"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                destination.Start = Point2D.Parse(this.m_Params[i].Substring(++indexOf));
                        }
                        else if (this.m_Params[i].StartsWith("DestEnd"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                destination.End = Point2D.Parse(this.m_Params[i].Substring(++indexOf));
                        }
                    }

                    item = new SerpentPillar(word, destination);
                }
                else if (this.m_Type.IsSubclassOf(typeofBeverage))
                {
                    BeverageType content = BeverageType.Liquor;
                    bool fill = false;

                    for (int i = 0; !fill && i < this.m_Params.Length; ++i)
                    {
                        if (this.m_Params[i].StartsWith("Content"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                            {
                                content = (BeverageType)Enum.Parse(typeof(BeverageType), this.m_Params[i].Substring(++indexOf), true);
                                fill = true;
                            }
                        }
                    }

                    if (fill)
                        item = (Item)Activator.CreateInstance(this.m_Type, new object[] { content });
                    else
                        item = (Item)Activator.CreateInstance(this.m_Type);
                }
                else if (this.m_Type.IsSubclassOf(typeofBaseDoor))
                {
                    DoorFacing facing = DoorFacing.WestCW;

                    for (int i = 0; i < this.m_Params.Length; ++i)
                    {
                        if (this.m_Params[i].StartsWith("Facing"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                            {
                                facing = (DoorFacing)Enum.Parse(typeof(DoorFacing), this.m_Params[i].Substring(++indexOf), true);
                                break;
                            }
                        }
                    }

                    item = (Item)Activator.CreateInstance(this.m_Type, new object[] { facing });
                }
                else
                {
                    item = (Item)Activator.CreateInstance(this.m_Type);
                }
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Bad type: {0}", this.m_Type), e);
            }

            if (item is BaseAddon)
            {
                if (item is MaabusCoffin)
                {
                    MaabusCoffin coffin = (MaabusCoffin)item;

                    for (int i = 0; i < this.m_Params.Length; ++i)
                    {
                        if (this.m_Params[i].StartsWith("SpawnLocation"))
                        {
                            int indexOf = this.m_Params[i].IndexOf('=');

                            if (indexOf >= 0)
                                coffin.SpawnLocation = Point3D.Parse(this.m_Params[i].Substring(++indexOf));
                        }
                    }
                }
                else if (this.m_ItemID > 0)
                {
                    List<AddonComponent> comps = ((BaseAddon)item).Components;

                    for (int i = 0; i < comps.Count; ++i)
                    {
                        AddonComponent comp = (AddonComponent)comps[i];

                        if (comp.Offset == Point3D.Zero)
                            comp.ItemID = this.m_ItemID;
                    }
                }
            }
            else if (item is BaseLight)
            {
                bool unlit = false, unprotected = false;

                for (int i = 0; i < this.m_Params.Length; ++i)
                {
                    if (!unlit && this.m_Params[i] == "Unlit")
                        unlit = true;
                    else if (!unprotected && this.m_Params[i] == "Unprotected")
                        unprotected = true;
					
                    if (unlit && unprotected)
                        break;
                }

                if (!unlit)
                    ((BaseLight)item).Ignite();
                if (!unprotected)
                    ((BaseLight)item).Protected = true;

                if (this.m_ItemID > 0)
                    item.ItemID = this.m_ItemID;
            }
            else if (item is Server.Mobiles.Spawner)
            {
                Server.Mobiles.Spawner sp = (Server.Mobiles.Spawner)item;

                sp.NextSpawn = TimeSpan.Zero;

                for (int i = 0; i < this.m_Params.Length; ++i)
                {
                    if (this.m_Params[i].StartsWith("Spawn"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.SpawnNames.Add(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("MinDelay"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.MinDelay = TimeSpan.Parse(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("MaxDelay"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.MaxDelay = TimeSpan.Parse(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("NextSpawn"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.NextSpawn = TimeSpan.Parse(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("Count"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.Count = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("Team"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.Team = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("HomeRange"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.HomeRange = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("Running"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.Running = Utility.ToBoolean(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("Group"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            sp.Group = Utility.ToBoolean(this.m_Params[i].Substring(++indexOf));
                    }
                }
            }
            else if (item is RecallRune)
            {
                RecallRune rune = (RecallRune)item;

                for (int i = 0; i < this.m_Params.Length; ++i)
                {
                    if (this.m_Params[i].StartsWith("Description"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            rune.Description = this.m_Params[i].Substring(++indexOf);
                    }
                    else if (this.m_Params[i].StartsWith("Marked"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            rune.Marked = Utility.ToBoolean(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("TargetMap"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            rune.TargetMap = Map.Parse(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("Target"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            rune.Target = Point3D.Parse(this.m_Params[i].Substring(++indexOf));
                    }
                }
            }
            else if (item is SkillTeleporter)
            {
                SkillTeleporter tp = (SkillTeleporter)item;

                for (int i = 0; i < this.m_Params.Length; ++i)
                {
                    if (this.m_Params[i].StartsWith("Skill"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Skill = (SkillName)Enum.Parse(typeof(SkillName), this.m_Params[i].Substring(++indexOf), true);
                    }
                    else if (this.m_Params[i].StartsWith("RequiredFixedPoint"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Required = Utility.ToInt32(this.m_Params[i].Substring(++indexOf)) * 0.01;
                    }
                    else if (this.m_Params[i].StartsWith("Required"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Required = Utility.ToDouble(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("MessageString"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.MessageString = this.m_Params[i].Substring(++indexOf);
                    }
                    else if (this.m_Params[i].StartsWith("MessageNumber"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.MessageNumber = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("PointDest"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.PointDest = Point3D.Parse(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("MapDest"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.MapDest = Map.Parse(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("Creatures"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Creatures = Utility.ToBoolean(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("SourceEffect"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.SourceEffect = Utility.ToBoolean(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("DestEffect"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.DestEffect = Utility.ToBoolean(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("SoundID"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.SoundID = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("Delay"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Delay = TimeSpan.Parse(this.m_Params[i].Substring(++indexOf));
                    }
                }

                if (this.m_ItemID > 0)
                    item.ItemID = this.m_ItemID;
            }
            else if (item is KeywordTeleporter)
            {
                KeywordTeleporter tp = (KeywordTeleporter)item;

                for (int i = 0; i < this.m_Params.Length; ++i)
                {
                    if (this.m_Params[i].StartsWith("Substring"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Substring = this.m_Params[i].Substring(++indexOf);
                    }
                    else if (this.m_Params[i].StartsWith("Keyword"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Keyword = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("Range"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Range = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("PointDest"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.PointDest = Point3D.Parse(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("MapDest"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.MapDest = Map.Parse(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("Creatures"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Creatures = Utility.ToBoolean(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("SourceEffect"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.SourceEffect = Utility.ToBoolean(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("DestEffect"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.DestEffect = Utility.ToBoolean(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("SoundID"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.SoundID = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("Delay"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Delay = TimeSpan.Parse(this.m_Params[i].Substring(++indexOf));
                    }
                }

                if (this.m_ItemID > 0)
                    item.ItemID = this.m_ItemID;
            }
            else if (item is Teleporter)
            {
                Teleporter tp = (Teleporter)item;

                for (int i = 0; i < this.m_Params.Length; ++i)
                {
                    if (this.m_Params[i].StartsWith("PointDest"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.PointDest = Point3D.Parse(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("MapDest"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.MapDest = Map.Parse(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("Creatures"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Creatures = Utility.ToBoolean(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("SourceEffect"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.SourceEffect = Utility.ToBoolean(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("DestEffect"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.DestEffect = Utility.ToBoolean(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("SoundID"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.SoundID = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                    }
                    else if (this.m_Params[i].StartsWith("Delay"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            tp.Delay = TimeSpan.Parse(this.m_Params[i].Substring(++indexOf));
                    }
                }

                if (this.m_ItemID > 0)
                    item.ItemID = this.m_ItemID;
            }
            else if (item is FillableContainer)
            {
                FillableContainer cont = (FillableContainer)item;

                for (int i = 0; i < this.m_Params.Length; ++i)
                {
                    if (this.m_Params[i].StartsWith("ContentType"))
                    {
                        int indexOf = this.m_Params[i].IndexOf('=');

                        if (indexOf >= 0)
                            cont.ContentType = (FillableContentType)Enum.Parse(typeof(FillableContentType), this.m_Params[i].Substring(++indexOf), true);
                    }
                }

                if (this.m_ItemID > 0)
                    item.ItemID = this.m_ItemID;
            }
            else if (this.m_ItemID > 0)
            {
                item.ItemID = this.m_ItemID;
            }

            item.Movable = false;

            for (int i = 0; i < this.m_Params.Length; ++i)
            {
                if (this.m_Params[i].StartsWith("Light"))
                {
                    int indexOf = this.m_Params[i].IndexOf('=');

                    if (indexOf >= 0)
                        item.Light = (LightType)Enum.Parse(typeof(LightType), this.m_Params[i].Substring(++indexOf), true);
                }
                else if (this.m_Params[i].StartsWith("Hue"))
                {
                    int indexOf = this.m_Params[i].IndexOf('=');

                    if (indexOf >= 0)
                    {
                        int hue = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));

                        if (item is DyeTub)
                            ((DyeTub)item).DyedHue = hue;
                        else
                            item.Hue = hue;
                    }
                }
                else if (this.m_Params[i].StartsWith("Name"))
                {
                    int indexOf = this.m_Params[i].IndexOf('=');

                    if (indexOf >= 0)
                        item.Name = this.m_Params[i].Substring(++indexOf);
                }
                else if (this.m_Params[i].StartsWith("Amount"))
                {
                    int indexOf = this.m_Params[i].IndexOf('=');

                    if (indexOf >= 0)
                    {
                        // Must supress stackable warnings
                        bool wasStackable = item.Stackable;

                        item.Stackable = true;
                        item.Amount = Utility.ToInt32(this.m_Params[i].Substring(++indexOf));
                        item.Stackable = wasStackable;
                    }
                }
            }

            return item;
        }

        private static readonly Queue<Item> m_DeleteQueue = new Queue<Item>();

        private static bool FindItemDelete(int x, int y, int z, Map map, Item srcItem)
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
                    {
                        m_DeleteQueue.Enqueue(item);
                        res = true;
                    }
                    /*else if (Math.Abs(item.Z - z) < 8)
                    m_DeleteQueue.Enqueue(item);*/
                }
            }
            else if ((TileData.ItemTable[itemID & 0x3FFF].Flags & TileFlag.LightSource) != 0)
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
                            /*if ( item.Light != lt )
                            m_DeleteQueue.Enqueue( item );
                            else*/
                            res = true;
                            m_DeleteQueue.Enqueue(item);
                        }
                        else if ((item.ItemData.Flags & TileFlag.LightSource) != 0 && item.ItemData.Name == srcName)
                        {
                            //m_DeleteQueue.Enqueue( item );
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
                        {
                            //m_DeleteQueue.Enqueue(item);
                        }
                        else
                        {
                            m_DeleteQueue.Enqueue(item);
                            res = true;
                        }
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
                        m_DeleteQueue.Enqueue(item);
                        res = true;
                        //eable.Free();
                        //return true;
                    }
                }
            }

            eable.Free();

            while (m_DeleteQueue.Count > 0)
                m_DeleteQueue.Dequeue().Delete();

            return res;
        }

        public int Generate(Map[] maps)
        {
            int count = 0;

            Item item = null;

            for (int i = 0; i < this.m_Entries.Count; ++i)
            {
                DecorationEntry entry = (DecorationEntry)this.m_Entries[i];
                Point3D loc = entry.Location;
                string extra = entry.Extra;

                for (int j = 0; j < maps.Length; ++j)
                {
                    if (item == null)
                        item = this.Construct();

                    #region Mondain's Legacy
                    this.m_Constructed = item;
                    #endregion

                    if (item == null)
                        continue;

                    if (FindItemDelete(loc.X, loc.Y, loc.Z, maps[j], item))
                    {
                        ++count;
                    }
                }
            }

            if (item != null)
                item.Delete();

            return count;
        }

        public static ArrayList ReadAll(string path)
        {
            using (StreamReader ip = new StreamReader(path))
            {
                ArrayList list = new ArrayList();

                for (DecorationListDelete v = Read(ip); v != null; v = Read(ip))
                    list.Add(v);

                return list;
            }
        }

        private static readonly string[] m_EmptyParams = new string[0];

        public static DecorationListDelete Read(StreamReader ip)
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

            DecorationListDelete list = new DecorationListDelete();

            int indexOf = line.IndexOf(' ');

            list.m_Type = ScriptCompiler.FindTypeByName(line.Substring(0, indexOf++), true);

            if (list.m_Type == null)
                throw new ArgumentException(String.Format("Type not found for header: '{0}'", line));

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

    public class DecorateMLDelete
    {
        public static void Initialize()
        {
            CommandSystem.Register("DecorateMLDelete", AccessLevel.Administrator, new CommandEventHandler(DecorateMLDelete_OnCommand));
        }

        [Usage("DecorateMLDelete")]
        [Description("Deletes Mondain's Legacy world decoration.")]
        private static void DecorateMLDelete_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Deleting Mondain's Legacy world decoration, please wait.");

            DecorateDelete.Generate("Data/Mondain's Legacy/Trammel", Map.Trammel);
            DecorateDelete.Generate("Data/Mondain's Legacy/Felucca", Map.Felucca);
            DecorateDelete.Generate("Data/Mondain's Legacy/Ilshenar", Map.Ilshenar);
            DecorateDelete.Generate("Data/Mondain's Legacy/Malas", Map.Malas);
            DecorateDelete.Generate("Data/Mondain's Legacy/Tokuno", Map.Tokuno);

            PeerlessAltar altar;
            PeerlessTeleporter tele;
            PrismOfLightPillar pillar;

            // Bedlam - Malas
            altar = new BedlamAltar();
            FindItem(86, 1627, 0, Map.Malas, altar);
            tele = new PeerlessTeleporter(altar);
            FindItem(99, 1617, 50, Map.Malas, tele);
            tele.Delete();
            altar.Delete();

            // Blighted Grove - Trammel
            altar = new BlightedGroveAltar();
            FindItem(6502, 875, 0, Map.Trammel, altar);
            tele = new PeerlessTeleporter(altar);
            FindItem(6511, 949, 26, Map.Trammel, tele);
            tele.Delete();
            altar.Delete();

            // Blighted Grove - Felucca
            altar = new BlightedGroveAltar();
            FindItem(6502, 875, 0, Map.Felucca, altar);
            tele = new PeerlessTeleporter(altar);
            FindItem(6511, 949, 26, Map.Felucca, tele);
            tele.Delete();
            altar.Delete();

            // Palace of Paroxysmus - Trammel
            altar = new ParoxysmusAltar();
            FindItem(6511, 506, -34, Map.Trammel, altar);
            tele = new PeerlessTeleporter(altar);
            FindItem(6518, 365, 46, Map.Trammel, tele);
            tele.Delete();
            altar.Delete();

            // Palace of Paroxysmus - Felucca
            altar = new ParoxysmusAltar();
            FindItem(6511, 506, -34, Map.Felucca, altar);
            tele = new PeerlessTeleporter(altar);
            FindItem(6518, 365, 46, Map.Felucca, tele);
            tele.Delete();
            altar.Delete();

            // Prism of Light - Trammel
            altar = new PrismOfLightAltar();
            FindItem(6509, 167, 6, Map.Trammel, altar);
            tele = new PeerlessTeleporter(altar);
            tele.ItemID = 0xDDA;
            FindItem(6501, 137, -20, Map.Trammel, tele);
            tele.Delete();
            pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
            FindItem(6506, 167, 0, Map.Trammel, pillar);
            pillar.Delete();

            pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
            FindItem(6509, 164, 0, Map.Trammel, pillar);
            pillar.Delete();

            pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
            FindItem(6506, 164, 0, Map.Trammel, pillar);
            pillar.Delete();

            pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
            FindItem(6512, 167, 0, Map.Trammel, pillar);
            pillar.Delete();

            pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
            FindItem(6509, 170, 0, Map.Trammel, pillar);
            pillar.Delete();

            pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
            FindItem(6512, 170, 0, Map.Trammel, pillar);
            pillar.Delete();
            altar.Delete();

            // Prism of Light - Felucca
            altar = new PrismOfLightAltar();
            FindItem(6509, 167, 6, Map.Felucca, altar);
            tele = new PeerlessTeleporter(altar);
            tele.ItemID = 0xDDA;
            FindItem(6501, 137, -20, Map.Felucca, tele);
            tele.Delete();

            pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
            FindItem(6506, 167, 0, Map.Felucca, pillar);
            pillar.Delete();

            pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
            FindItem(6509, 164, 0, Map.Felucca, pillar);
            pillar.Delete();

            pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x581);
            FindItem(6506, 164, 0, Map.Felucca, pillar);
            pillar.Delete();

            pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
            FindItem(6512, 167, 0, Map.Felucca, pillar);
            pillar.Delete();

            pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
            FindItem(6509, 170, 0, Map.Felucca, pillar);
            pillar.Delete();

            pillar = new PrismOfLightPillar((PrismOfLightAltar)altar, 0x481);
            FindItem(6512, 170, 0, Map.Felucca, pillar);
            pillar.Delete();
            altar.Delete();

            // Citadel - Malas
            altar = new CitadelAltar();
            FindItem(90, 1884, 0, Map.Malas, altar);
            tele = new PeerlessTeleporter(altar);
            FindItem(114, 1955, 0, Map.Malas, tele);
            tele.Delete();
            altar.Delete();

            // Twisted Weald - Ilshenar
            altar = new TwistedWealdAltar();
            FindItem(2170, 1255, -60, Map.Ilshenar, altar);
            tele = new PeerlessTeleporter(altar);
            FindItem(2139, 1271, -57, Map.Ilshenar, tele);
            tele.Delete();
            altar.Delete();

            e.Mobile.SendMessage("Mondain's Legacy world decoration deleting complete.");
        }

        private static readonly Queue<Item> m_DeleteQueue = new Queue<Item>();

        public static bool FindItem(int x, int y, int z, Map map, Item test)
        {
            return FindItem(new Point3D(x, y, z), map, test);
        }

        public static bool FindItem(Point3D p, Map map, Item test)
        {
            bool result = false;

            IPooledEnumerable eable = map.GetItemsInRange(p);

            foreach (Item item in eable)
            {
                if (item.Z == p.Z && item.ItemID == test.ItemID)
                {
                    m_DeleteQueue.Enqueue(item);
                    result = true;
                }
            }

            eable.Free();

            while (m_DeleteQueue.Count > 0)
                m_DeleteQueue.Dequeue().Delete();

            return result;
        }
    }
}