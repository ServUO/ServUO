using Server;
using System;
using Server.Mobiles;
using Server.Multis;
using System.Linq;

namespace Server.Items
{
    public enum TrophyStyle
    {
        Marlin,
        Dragonfish,
        BoardGrouper,
        BoardMahi,
        Lobster,
        Crab
    }

    public class FishTrophyDeed : Item
    {
        private int m_Weight;
        private Mobile m_Fisher;
        private DateTime m_DateCaught;
        private int m_DeedName;
        private int m_TrophyName;
        private int m_TrophyID;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Fisher { get { return m_Fisher; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TrophyWeight { get { return m_Weight; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DateCaught { get { return m_DateCaught; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TrophyName { get { return m_TrophyName; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DeedName { get { return m_DeedName; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TrophyID { get { return m_TrophyID; } }

        public override int LabelNumber { get { return m_DeedName; } }

        public FishTrophyDeed(int weight, Mobile fisher, DateTime caught, int deedname, int trophyname, int id) : base(0x14F0)
        {
            m_Weight = weight;
            m_Fisher = fisher;
            m_DateCaught = caught;
            m_DeedName = deedname;
            m_TrophyName = trophyname;
            m_TrophyID = id;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null && house.IsCoOwner(from))
                {
                    bool northWall = BaseAddon.IsWall(from.X, from.Y - 1, from.Z, from.Map);
                    bool westWall = BaseAddon.IsWall(from.X - 1, from.Y, from.Z, from.Map);

                    if (northWall && westWall)
                    {
                        switch (from.Direction & Direction.Mask)
                        {
                            case Direction.North:
                            case Direction.South: northWall = true; westWall = false; break;

                            case Direction.East:
                            case Direction.West: northWall = false; westWall = true; break;

                            default: from.SendMessage("Turn to face the wall on which to hang this trophy."); return;
                        }
                    }

                    BaseAddon addon = null;

                    if (northWall)
                        addon = ConstructTrophy(true);
                    else if (westWall)
                        addon = ConstructTrophy(false);
                    else
                        from.SendLocalizedMessage(1042626); // The trophy must be placed next to a wall.

                    if (addon != null)
                    {
                        house.Addons[addon] = from;
                        addon.MoveToWorld(from.Location, from.Map);
                        Delete();
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502092); // You must be in your house to do this.
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Weight >= 20)
            {
                if (m_Fisher != null)
                    list.Add(1070857, m_Fisher.Name); // Caught by ~1_fisherman~

                list.Add(1070858, m_Weight.ToString()); // ~1_weight~ stones
            }
        }

        public BaseAddon ConstructTrophy(bool north)
        {
            BaseAddon addon = null;
            switch (m_TrophyID)
            {
                case 0: addon = new FishTrophy(typeof(AutumnDragonfish),  TrophyStyle.Dragonfish, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 1: addon = new FishTrophy(typeof(BullFish),          TrophyStyle.BoardMahi, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 2: addon = new FishTrophy(typeof(FireFish),          TrophyStyle.BoardGrouper, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 3: addon = new FishTrophy(typeof(GiantKoi),          TrophyStyle.BoardMahi, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 4: addon = new FishTrophy(typeof(LavaFish),          TrophyStyle.Marlin, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 5: addon = new FishTrophy(typeof(SummerDragonfish),  TrophyStyle.Dragonfish, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 6: addon = new FishTrophy(typeof(UnicornFish),       TrophyStyle.Marlin, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 7: addon = new FishTrophy(typeof(AbyssalDragonfish), TrophyStyle.Dragonfish, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 8: addon = new FishTrophy(typeof(BlackMarlin),       TrophyStyle.Marlin, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 9: addon = new FishTrophy(typeof(BlueMarlin),        TrophyStyle.Marlin, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 10: addon = new FishTrophy(typeof(GiantSamuraiFish), TrophyStyle.BoardGrouper, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 11: addon = new FishTrophy(typeof(Kingfish),         TrophyStyle.BoardGrouper, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 12: addon = new FishTrophy(typeof(LanternFish),      TrophyStyle.BoardMahi, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 13: addon = new FishTrophy(typeof(SeekerFish),       TrophyStyle.BoardGrouper, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 14: addon = new FishTrophy(typeof(SpringDragonfish), TrophyStyle.Dragonfish, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 15: addon = new FishTrophy(typeof(StoneFish),        TrophyStyle.BoardMahi, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 16: addon = new FishTrophy(typeof(WinterDragonfish), TrophyStyle.Dragonfish, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;

                case 17: addon = new FishTrophy(typeof(BlueLobster),      TrophyStyle.Lobster, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 18: addon = new FishTrophy(typeof(BloodLobster),     TrophyStyle.Lobster, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 19: addon = new FishTrophy(typeof(DreadLobster),     TrophyStyle.Lobster, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 20: addon = new FishTrophy(typeof(VoidLobster),      TrophyStyle.Lobster, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 21: addon = new FishTrophy(typeof(StoneCrab),        TrophyStyle.Crab, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 22: addon = new FishTrophy(typeof(SpiderCrab),       TrophyStyle.Crab, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 23: addon = new FishTrophy(typeof(TunnelCrab),       TrophyStyle.Crab, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 24: addon = new FishTrophy(typeof(VoidCrab),         TrophyStyle.Crab, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;

                case 25: addon = new FishTrophy(typeof(CrystalFish),         TrophyStyle.BoardGrouper, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 26: addon = new FishTrophy(typeof(FairySalmon),         TrophyStyle.BoardGrouper, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 27: addon = new FishTrophy(typeof(GreatBarracuda),      TrophyStyle.BoardGrouper, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 28: addon = new FishTrophy(typeof(HolyMackerel),        TrophyStyle.BoardGrouper, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 29: addon = new FishTrophy(typeof(ReaperFish),          TrophyStyle.BoardGrouper, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 30: addon = new FishTrophy(typeof(YellowtailBarracuda), TrophyStyle.BoardGrouper, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 31: addon = new FishTrophy(typeof(DungeonPike),         TrophyStyle.BoardGrouper, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 32: addon = new FishTrophy(typeof(GoldenTuna),          TrophyStyle.BoardGrouper, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 33: addon = new FishTrophy(typeof(RainbowFish),         TrophyStyle.BoardGrouper, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
                case 34: addon = new FishTrophy(typeof(ZombieFish),          TrophyStyle.BoardGrouper, north, m_TrophyName, m_Weight, m_Fisher, m_DateCaught); break;
            }
            return addon;
        }

        public FishTrophyDeed(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_Weight);
            writer.Write(m_Fisher);
            writer.Write(m_DateCaught);
            writer.Write(m_TrophyName);
            writer.Write(m_DeedName);
            writer.Write(m_TrophyID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Weight = reader.ReadInt();
            m_Fisher = reader.ReadMobile();
            m_DateCaught = reader.ReadDateTime();
            m_TrophyName = reader.ReadInt();
            m_DeedName = reader.ReadInt();
            m_TrophyID = reader.ReadInt();
        }
    }

    public class FishTrophy : BaseAddon
    {
        private int m_FishWeight;
        private Mobile m_Fisher;
        private DateTime m_DateCaught;
        private Type m_TypeName;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Fisher { get { return m_Fisher; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DateCaught { get { return m_DateCaught; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int FishWeight { get { return m_FishWeight; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public Type TypeName { get { return m_TypeName; } }

        public FishTrophy(Type type, TrophyStyle style, bool north, int label, int weight, Mobile fisher, DateTime datecaught)
        {
            int hue = FishInfo.GetFishHue(type);
            m_TypeName = type;

            m_FishWeight = weight;
            m_Fisher = fisher;
            m_DateCaught = datecaught;

            switch(style)
            {
                case TrophyStyle.Marlin:
                    {
                        if(north)
                        {
                            AddComponent(new TrophyComponent(17641, label, hue), 0, 0, 0);
                            AddComponent(new TrophyComponent(17642, label, hue), -1, 0, 0); 
                            AddComponent(new TrophyComponent(17643, label, hue), 1, 0, 0); 
                        }
                        else
                        {
                            AddComponent(new TrophyComponent(17644, label, hue), 0, 0, 0); 
                            AddComponent(new TrophyComponent(17645, label, hue), 0, 1, 0); 
                            AddComponent(new TrophyComponent(17646, label, hue), 0, -1, 0); 
                        }
                        break;
                    }
                case TrophyStyle.Dragonfish:
                    {
                        if (north)
                            AddComponent(new TrophyComponent(17639, label, hue), 0, 0, 0);
                        else
                            AddComponent(new TrophyComponent(17640, label, hue), 0, 0, 0);
                        break;
                    }
                case TrophyStyle.BoardMahi:
                    {
                        if (north)
                        {
                            AddComponent(new TrophyComponent(19283, label, 0), 0, 0, 0);
                            AddComponent(new TrophyComponent(19287, label, hue), 0, 0, 0);
                        }
                        else
                        {
                            AddComponent(new TrophyComponent(19282, label, 0), 0, 0, 0);
                            AddComponent(new TrophyComponent(19286, label, hue), 0, 0, 0);
                        }
                        break;
                    }
                case TrophyStyle.BoardGrouper:
                    {
                        if (north)
                        {
                            AddComponent(new TrophyComponent(19281, label, 0), 0, 0, 0);
                            AddComponent(new TrophyComponent(19285, label, hue), 0, 0, 0);
                        }
                        else
                        {
                            AddComponent(new TrophyComponent(19280, label, 0), 0, 0, 0);
                            AddComponent(new TrophyComponent(19284, label, hue), 0, 0, 0);
                        }
                        break;
                    }
                case TrophyStyle.Crab:
                    {
                        if (north)
                        {
                            AddComponent(new TrophyComponent(18106, label, hue), 0, 0, 0);
                        }
                        else
                        {
                            AddComponent(new TrophyComponent(18107, label, hue), 0, 0, 0);
                        }
                        break;
                    }
                case TrophyStyle.Lobster:
                    {
                        if (north)
                        {
                            AddComponent(new TrophyComponent(18108, label, hue), 0, 0, 0);
                        }
                        else
                        {
                            AddComponent(new TrophyComponent(18109, label, hue), 0, 0, 0);
                        }
                        break;
                    }
            }
        }

        public Item TrophyDeed
        {
            get
            {
                var info = TaxidermyKit.TrophyInfos.FirstOrDefault(i => i.CreatureType == m_TypeName);

                if (info != null)
                {
                    return new FishTrophyDeed(m_FishWeight, m_Fisher, m_DateCaught, info.DeedNumber, info.AddonNumber, info.NorthID);
                }

                return null;
            }
        }

        public override void OnChop(Mobile from)
        {
            if (Components.Count > 0)
            {
                OnComponentUsed(Components[0], from);
            }
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (m_TypeName == null)
                return;

            var info = TaxidermyKit.TrophyInfos.FirstOrDefault(i => i.CreatureType == m_TypeName);

            if (info != null)
            {
                BaseHouse house = BaseHouse.FindHouseAt(c);

                if (house != null && (house.IsCoOwner(from) || (house.Addons.ContainsKey(this) && house.Addons[this] == from)))
                {
                    from.AddToBackpack(new FishTrophyDeed(m_FishWeight, m_Fisher, m_DateCaught, info.DeedNumber, info.AddonNumber, info.NorthID));

                    if(house.Addons.ContainsKey(this))
                        house.Addons.Remove(this);

                    Delete();
                }
                else
                {
                    from.SendLocalizedMessage(502092); // You must be in your house to do this.
                }
            }
        }

        public FishTrophy(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_TypeName.Name);
            writer.Write(m_FishWeight);
            writer.Write(m_Fisher);
            writer.Write(m_DateCaught);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            string name = reader.ReadString();
            m_TypeName = ScriptCompiler.FindTypeByName(name);
            m_FishWeight = reader.ReadInt();
            m_Fisher = reader.ReadMobile();
            m_DateCaught = reader.ReadDateTime();
        }
    }

    public class TrophyComponent : LocalizedAddonComponent
    {
        public override bool ForceShowProperties { get { return true; } }

        public TrophyComponent(int itemID, int label, int hue) : base(itemID, label)
        {
            Hue = hue;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Addon != null && Addon is FishTrophy)
            {
                FishTrophy trophy = Addon as FishTrophy;

                list.Add(1070858, trophy.FishWeight.ToString());
                list.Add(1070857, trophy.Fisher != null ? trophy.Fisher.Name : "Unknown");
                list.Add(String.Format("[{0}]", trophy.DateCaught.ToShortDateString()));
            }
        }

        public TrophyComponent(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}