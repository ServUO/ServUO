using Server.ContextMenus;
using Server.Engines.Quests;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using System.Collections.Generic;

namespace Server.Items
{
    public class MyrmidexRewardBag : Backpack
    {
        public MyrmidexRewardBag()
        {
            Hue = BaseReward.RewardBagHue();

            switch (Utility.Random(4))
            {
                default:
                case 0: DropItem(new RecipeScroll(Utility.RandomMinMax(900, 905))); break;
                case 1: DropItem(new EodonTribeRewardTitleToken()); break;
                case 2: DropItem(new RecipeScroll(455)); break;
                case 3: DropItem(new MoonstoneCrystal()); break;
            }
        }

        public MyrmidexRewardBag(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class EodonianRewardBag : Backpack
    {
        public EodonianRewardBag()
        {
            Hue = BaseReward.RewardBagHue();

            switch (Utility.Random(4))
            {
                default:
                case 0: DropItem(new MonsterStatuette(MonsterStatuetteType.SakkhranBirdOfPrey)); break;
                case 1: DropItem(new EodonTribeRewardTitleToken()); break;
                case 2: DropItem(new RecipeScroll(1000)); break;
                case 3:
                    if (0.5 > Utility.RandomDouble())
                        DropItem(new RawMoonstoneLargeAddonDeed());
                    else
                        DropItem(new RawMoonstoneSmallAddonDeed());
                    break;
            }
        }

        public EodonianRewardBag(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class MoonstoneCrystal : Item, ISecurable
    {
        public static Dictionary<int, Point3D> Locations { get; set; }
        private SecureLevel m_SecureLevel;

        public static void Initialize()
        {
            Locations = new Dictionary<int, Point3D>();

            Locations[1156705] = new Point3D(715, 1866, 40); // Eodon Moongate
            Locations[1156706] = new Point3D(642, 1721, 40); // Barako Village
            Locations[1156707] = new Point3D(701, 2106, 40); // Jukari Village
            Locations[1156708] = new Point3D(355, 1873, 0);  // Kurak Village
            Locations[1156709] = new Point3D(552, 1471, 40); // Sakkhra Village
            Locations[1156710] = new Point3D(412, 1595, 40); // Urali Village
            Locations[1156711] = new Point3D(167, 1800, 80); // Barrab Village
            Locations[1156712] = new Point3D(929, 2016, 50); // Shadowguard
            Locations[1156713] = new Point3D(731, 1603, 40); // The great ape cave
            Locations[1156714] = new Point3D(878, 2105, 40); // The Volcano
            Locations[1156715] = new Point3D(390, 1690, 40); // Dragon Turtle Habitat
            Locations[1156716] = new Point3D(269, 1726, 80); // Britannian Encampment
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return m_SecureLevel;
            }
            set
            {
                m_SecureLevel = value;
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override int LabelNumber => 1124143;  // Moonstone Crystal

        [Constructable]
        public MoonstoneCrystal() : base(0x9CBB)
        {
            Weight = 10;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if ((IsLockedDown || IsSecure) && from.InRange(GetWorldLocation(), 2))
            {
                from.SendGump(new InternalGump(from as PlayerMobile, this));
            }
            else if (!from.InRange(GetWorldLocation(), 2))
            {
                from.SendLocalizedMessage(500295); // You are too far away to do that.
            }
            else
                from.SendLocalizedMessage(502692); // This must be in a house and be locked down to work.
        }

        private class InternalGump : Gump
        {
            public Item Moonstone { get; }
            public PlayerMobile User { get; }

            public InternalGump(PlayerMobile pm, Item moonstone) : base(75, 75)
            {
                Moonstone = moonstone;
                User = pm;

                AddGumpLayout();
            }

            public void AddGumpLayout()
            {
                AddBackground(0, 0, 400, 400, 9270);

                AddHtmlLocalized(0, 15, 400, 16, 1154645, "#1156704", 0xFFFF, false, false); // Select your destination:

                ColUtility.For(Locations, (i, key, value) =>
                {
                    AddHtmlLocalized(60, 45 + (i * 25), 250, 16, key, 0xFFFF, false, false);
                    AddButton(20, 50 + (i * 25), 2117, 2118, key, GumpButtonType.Reply, 0);
                });
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                if (info.ButtonID > 0)
                {
                    int id = info.ButtonID;

                    if (Locations.ContainsKey(id))
                    {
                        Point3D p = Locations[id];

                        if (CheckTravel(p))
                        {
                            BaseCreature.TeleportPets(User, p, Map.TerMur);
                            User.Combatant = null;
                            User.Warmode = false;
                            User.Hidden = true;

                            User.MoveToWorld(p, Map.TerMur);

                            Effects.PlaySound(p, Map.TerMur, 0x1FE);
                        }
                    }
                }
            }

            private bool CheckTravel(Point3D p)
            {
                if (!User.InRange(Moonstone.GetWorldLocation(), 2) || User.Map != Moonstone.Map)
                {
                    User.SendLocalizedMessage(500295); // You are too far away to do that.
                }
                else if (User.Murderer)
                {
                    User.SendLocalizedMessage(1019004); // You are not allowed to travel there.
                }
                else if (User.Criminal)
                {
                    User.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                }
                else if (Spells.SpellHelper.CheckCombat(User))
                {
                    User.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                }
                else if (User.Spell != null)
                {
                    User.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
                }
                else if (User.Map == Map.TerMur && User.InRange(p, 1))
                {
                    User.SendLocalizedMessage(1019003); // You are already there.
                }
                else
                    return true;

                return false;
            }
        }

        public MoonstoneCrystal(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write((int)m_SecureLevel);  // At first, need to save world with this line before next starting.
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_SecureLevel = (SecureLevel)reader.ReadInt();  // If you have not saved world with above line in Serialize(), you should not add this line.
        }
    }

    [TypeAlias("Server.Items.KotlPowerCoil")]
    public class KotlPowerCore : Item
    {
        public override int LabelNumber => 1124179;  // Kotl Power Core

        [Constructable]
        public KotlPowerCore() : base(40147)
        {
        }

        public KotlPowerCore(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Flipable(40253, 40252)]
    public class EodonianWallMap : Item
    {
        public override int LabelNumber => 1156690;  // Wall Map of Eodon

        [Constructable]
        public EodonianWallMap()
            : base(40253)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 5))
            {
                Gump g = new Gump(0, 0);
                g.AddImage(0, 0, 49999);

                from.SendGump(g);
            }
        }

        public EodonianWallMap(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class RawMoonstoneLargeAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new RawMoonstoneLargeAddonDeed();

        [Constructable]
        public RawMoonstoneLargeAddon()
        {
            AddComponent(new LocalizedAddonComponent(40129, 1124130), 0, 0, 0);
        }

        public RawMoonstoneLargeAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class RawMoonstoneLargeAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new RawMoonstoneLargeAddon();

        public override int LabelNumber => 1156703;

        [Constructable]
        public RawMoonstoneLargeAddonDeed()
        {
        }

        public RawMoonstoneLargeAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class RawMoonstoneSmallAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new RawMoonstoneSmallAddonDeed();

        [Constructable]
        public RawMoonstoneSmallAddon()
        {
            AddComponent(new LocalizedAddonComponent(40136, 1124130), 0, 0, 0);
        }

        public RawMoonstoneSmallAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class RawMoonstoneSmallAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new RawMoonstoneSmallAddon();

        public override int LabelNumber => 1156702;

        [Constructable]
        public RawMoonstoneSmallAddonDeed()
        {
        }

        public RawMoonstoneSmallAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
