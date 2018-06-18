using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class AcidSac : Item
    {
        [Constructable]
        public AcidSac()
            : base(0x0C67)
        {
            Stackable = true;
            Weight = 1.0;
            Hue = 648;
        }

        public AcidSac(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1111654;
            }
        }// acid sac
        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1111656); // What do you wish to use the acid on?
                from.Target = new InternalTarget(this);
            }
            else
                from.SendLocalizedMessage(1080063); // This must be in your backpack to use it.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private class InternalTarget : Target
        {
            private readonly Item m_Item;
            private Item wall;
            private Item wallandvine;
            public InternalTarget(Item item)
                : base(2, false, TargetFlags.None)
            {
                m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = from as PlayerMobile;

                if (m_Item.Deleted)
                    return;

                if (targeted is AddonComponent)
                {
                    AddonComponent addoncomponent = (AddonComponent)targeted;

                    if (addoncomponent is MagicVinesComponent || addoncomponent is StoneWallComponent || addoncomponent is DungeonWallComponent)
                    {
                        int Xs = addoncomponent.X;

                        if (addoncomponent is MagicVinesComponent)
                            Xs += -1;

                        if (addoncomponent.Addon is StoneWallAndVineAddon)
                        {
                            wall = new SecretStoneWallNS();
                            wallandvine = new StoneWallAndVineAddon();
                        }
                        else if (addoncomponent.Addon is DungeonWallAndVineAddon)
                        {
                            wall = new SecretDungeonWallNS();
                            wallandvine = new DungeonWallAndVineAddon();
                        }

                        wall.MoveToWorld(new Point3D(Xs, addoncomponent.Y, addoncomponent.Z), addoncomponent.Map);

                        addoncomponent.Delete();

                        m_Item.Consume();

                        wall.PublicOverheadMessage(0, 1358, 1111662); // The acid quickly burns through the writhing wallvines, revealing the strange wall.

                        Timer.DelayCall(TimeSpan.FromSeconds(15.0), delegate()
                        {
                            wallandvine.MoveToWorld(wall.Location, wall.Map);

                            wall.Delete();
                            wallandvine.PublicOverheadMessage(0, 1358, 1111663); // The vines recover from the acid and, spreading like tentacles, reclaim their grip over the wall.
                        });
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1111657); // The acid swiftly burn through it.
                    m_Item.Consume();
                    return; // Exit the method, because addoncomponent is null
                }
            }
        }
    }

    public class AncientPotteryFragments : Item
    {
        [Constructable]
        public AncientPotteryFragments()
			: base(0x2243)
        {
			Hue = 2108;
        }

        public AncientPotteryFragments(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112990;
            }
        }// Ancient Pottery fragments
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class BouraPelt : Item
    {
        [Constructable]
        public BouraPelt()
            : this(1)
        {
        }

        [Constructable]
        public BouraPelt(int amount)
            : base(0x5742)
        {
            Stackable = true;
            Amount = amount;
        }

        public BouraPelt(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113355;
            }
        }// boura pelt
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class ClawSlasherVeils : Item
    {
        [Constructable]
        public ClawSlasherVeils()
            : this(1)
        {
        }

        [Constructable]
        public ClawSlasherVeils(int amount)
            : base(0x2DB8)
        {
            Stackable = true;
            Amount = amount;
        }

        public ClawSlasherVeils(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1031704;
            }
        }// Claw of Slasher of Veils
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class CongealedSlugAcid : Item
    {
        [Constructable]
        public CongealedSlugAcid()
            : this(1)
        {
        }

        [Constructable]
        public CongealedSlugAcid(int amount)
            : base(0x5742)
        {
            Stackable = true;
            Amount = amount;
        }

        public CongealedSlugAcid(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112901;
            }
        }// Congealed Slug Acid
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [TypeAlias("Server.Items.EnchantEssence")]
    public class EnchantedEssence : Item, ICommodity
    {
        [Constructable]
        public EnchantedEssence()
            : this(1)
        {
        }

        [Constructable]
        public EnchantedEssence(int amount)
            : base(0x2DB2)
        {
            Stackable = true;
            Amount = amount;
        }

        public EnchantedEssence(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1031698;
            }
        }// Enchaned Essence
		int ICommodity.DescriptionNumber
        {
            get
            {
                return this.LabelNumber;
            }
        }
        bool ICommodity.IsDeedable
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class FairyDragonWing : Item
    {
        [Constructable]
        public FairyDragonWing()
            : this(1)
        {
        }

        [Constructable]
        public FairyDragonWing(int amount)
            : base(0x1084)
        {
            Hue = 1111;

            Stackable = true;
            Amount = amount;
        }

        public FairyDragonWing(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112899;
            }
        }// Fairy Dragon Wing
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class LeatherWolfSkin : Item
    {
        [Constructable]
        public LeatherWolfSkin()
            : this(1)
        {
        }

        [Constructable]
        public LeatherWolfSkin(int amount)
            : base(0xDF8)
        {
            Stackable = true;
            Amount = amount;

            Hue = 248;
        }

        public LeatherWolfSkin(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112906;
            }
        }// leather wolf skin
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                ItemID = 0xDF8;
                Hue = 248;
            }
        }
    }

    public class LuckyCoin : Item
    {
        [Constructable]
        public LuckyCoin()
            : this(1)
        {
        }

        [Constructable]
        public LuckyCoin(int amount)
            : base(0xF87)
        {
            Stackable = true;
            Amount = amount;

            Hue = 1174;
        }

        public LuckyCoin(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113366;
            }
        }// lucky coin

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) && Amount >= 1)
            {
                from.SendLocalizedMessage(1113367); // Make a wish then toss me into sacred waters!!
                from.Target = new InternalTarget(this);
            }
        }

        private class InternalTarget : Target
        {
            private LuckyCoin m_Coin;

            public InternalTarget(LuckyCoin coin)
                : base(3, false, TargetFlags.None)
            {
                m_Coin = coin;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is AddonComponent && ((AddonComponent)targeted).Addon is FountainOfFortune)
                {
                    AddonComponent c = (AddonComponent)targeted;

                    if (c.Addon is FountainOfFortune)
                        ((FountainOfFortune)c.Addon).OnTarget(from, m_Coin);
                }
                else
                    from.SendLocalizedMessage(1113369); // That is not sacred waters. Try looking in the Underworld.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class MagicalResidue : Item, ICommodity
    {
        [Constructable]
        public MagicalResidue()
            : this(1)
        {
        }

        [Constructable]
        public MagicalResidue(int amount)
            : base(0x2DB1)
        {
            Stackable = true;
            Amount = amount;
        }

        public MagicalResidue(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1031697;
            }
        }// Magical Residue
		int ICommodity.DescriptionNumber
        {
            get
            {
                return this.LabelNumber;
            }
        }
        bool ICommodity.IsDeedable
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PileInspectedIngots : Item
    {
        [Constructable]
        public PileInspectedIngots()
            : this(1)
        {
        }

        [Constructable]
        public PileInspectedIngots(int amount)
            : base(0x1BEA)
        {
            Stackable = true;
            Amount = amount;
        }

        public PileInspectedIngots(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (ItemID != 0x1BEA)
                ItemID = 0x1BEA;
        }
    }

    public class RelicFragment : Item, ICommodity
    {
        [Constructable]
        public RelicFragment()
            : this(1)
        {
        }

        [Constructable]
        public RelicFragment(int amount)
            : base(0x2DB3)
        {
            Stackable = true;
            Amount = amount;
        }

        public RelicFragment(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1031699;
            }
        }// Relic Fragment
		int ICommodity.DescriptionNumber
        {
            get
            {
                return this.LabelNumber;
            }
        }
        bool ICommodity.IsDeedable
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SearedFireAntGoo : Item
    {
        [Constructable]
        public SearedFireAntGoo()
			: this(1)
        {
        }

		[Constructable]
		public SearedFireAntGoo(int amount)
            : base(0x122E)
		{
			Stackable = true;
			Amount = amount;
            Hue = 1359;
		}

        public SearedFireAntGoo(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112902;
            }
        }// Seared Fire Ant Goo
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (ItemID != 0x122E)
                ItemID = 0x122E;
        }
    }

    public class StygianDragonHead : Item
    {
        [Constructable]
        public StygianDragonHead()
            : this(1)
        {
        }

        [Constructable]
        public StygianDragonHead(int amount)
            : base(0x2DB4)
        {
            Stackable = true;
            Amount = amount;
        }

        public StygianDragonHead(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1031700;
            }
        }// Stygian Dragon Head
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class TatteredAncientScroll : Item
    {
        [Constructable]
        public TatteredAncientScroll()
            : this(1)
        {
        }

        [Constructable]
        public TatteredAncientScroll(int amount)
            : base(0x1437)
        {
        }

        public TatteredAncientScroll(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112991;
            }
        }// Tattered Remnants of an Ancient Scroll
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && ItemID != 0x1437)
                ItemID = 0x1437;

            if (version == 0 && Stackable)
            {
                Stackable = false;
                Amount = 1;
            }
        }
    }

    public class UndamagedIronBeetleScale : Item
    {
        [Constructable]
        public UndamagedIronBeetleScale()
            : this(1)
        {
        }

        [Constructable]
        public UndamagedIronBeetleScale(int amount)
            : base(0x26B3)
        {
            Stackable = true;
            Amount = amount;
        }

        public UndamagedIronBeetleScale(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112905;
            }
        }// Undamaged Iron Beetle Scale
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (ItemID != 0x26B3)
                ItemID = 0x26B3;
        }
    }

    public class UndeadGargHorn : Item
    {
        [Constructable]
        public UndeadGargHorn()
            : this(1)
        {
        }

        [Constructable]
        public UndeadGargHorn(int amount)
            : base(0x315C)
        {
            Stackable = true;
            Amount = amount;
        }

        public UndeadGargHorn(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112903;
            }
        }// Undamaged Undead Gargoyle Horns
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (ItemID != 0x315C)
                ItemID = 0x315C;
        }
    }

    public class UndeadGargMedallion : Item
    {
        [Constructable]
        public UndeadGargMedallion()
            : this(1)
        {
        }

        [Constructable]
        public UndeadGargMedallion(int amount)
            : base(0x1088)
        {
            Stackable = true;
            Amount = amount;

            Hue = 2207;
            LootType = LootType.Blessed;
        }

        public UndeadGargMedallion(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112907;
            }
        }// Undead Gargoyle Medallion
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (ItemID != 0x1088)
                ItemID = 0x1088;
        }
    }

    [TypeAlias("Server.Items.UntransTome")]
    public class UntranslatedAncientTome : Item
    {
        [Constructable]
        public UntranslatedAncientTome()
            : this(1)
        {
        }

        [Constructable]
        public UntranslatedAncientTome(int amount)
            : base(0x0FF2)
        {
            Stackable = true;
            Amount = amount;

            Hue = 2405;
        }

        public UntranslatedAncientTome(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112992;
            }
        }// Untranslated Ancient Tome
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (ItemID != 0x0FF2)
                ItemID = 0x0FF2;
        }
    }

    public class CrystalDust : Item
    {
        public override int LabelNumber { get { return 1112328; } } // crystal dust

        [Constructable]
        public CrystalDust()
            : this(1)
        {
        }

        [Constructable]
        public CrystalDust(int amount)
            : base(16393)
        {
            Hue = 2103;
            Stackable = true;

            Amount = amount;
        }

        public CrystalDust(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class BarrelOfBarley : Item
    {
        public override int LabelNumber { get { return 1094999; } } // Barrel of Barley

        [Constructable]
        public BarrelOfBarley()
            : base(4014)
        {
            Weight = 25;
        }

        public BarrelOfBarley(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FlintsLogbook : Item
    {
        public override int LabelNumber { get { return 1095000; } } // Flint's Logbook

        [Constructable]
        public FlintsLogbook()
            : base(7185)
        {
        }

        public FlintsLogbook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BottleOfFlintsPungnentBrew : BaseBeverage
    {
        public override int LabelNumber
        {
            get
            {
                return IsEmpty ? 1113607 : 1094967; // a bottle of Flint's Pungent Brew
            }
        }

        [Constructable]
        public BottleOfFlintsPungnentBrew()
            : base(BeverageType.Ale)
        {
        }

        public override int ComputeItemID()
        {
            return 0x99F;
        }

        public override int MaxQuantity { get { return 5; } }

        public BottleOfFlintsPungnentBrew(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Flipable(6870, 6871)]
    public class KegOfFlintsPungnentBrew : Item
    {
        public override int LabelNumber { get { return 1113608; } } // a keg of Flint's Pungent Brew

        [Constructable]
        public KegOfFlintsPungnentBrew()
            : base(6870)
        {
            Weight = 25;
        }

        public KegOfFlintsPungnentBrew(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FloorTrapComponent : Item
    {
        public override int LabelNumber { get { return 1095001; } } // Floor Trap Components

        [Constructable]
        public FloorTrapComponent()
            : base(Utility.RandomMinMax(3117, 3120))
        {
        }

        public FloorTrapComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class DuganMissingQuestCorpse : QuestHintItem
    {
        public override Type QuestType { get { return typeof(Server.Engines.Quests.Missing); } }

        [Constructable]
        public DuganMissingQuestCorpse()
            : base(1094954) // You observe the remains of four humans here.  As you observe the tragic scene, you are reminded that you promised to bring evidence to Elder Dugan of their fate.
        {
        }

        public DuganMissingQuestCorpse(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class FlintLostBarrelHint : QuestHintItem
    {
        public override Type QuestType { get { return typeof(Server.Engines.Quests.ThievesBeAfootQuest); } }
        public override Type QuestItemType { get { return typeof(BarrelOfBarley); } }
        public override int DefaultRange { get { return 5; } }

        [Constructable]
        public FlintLostBarrelHint()
            : base(1094963) // The smug smell of Barley fills this chamber.
        {
        }

        public FlintLostBarrelHint(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class FlintLostLogbookHint : QuestHintItem
    {
        public override Type QuestType { get { return typeof(Server.Engines.Quests.BibliophileQuest); } }
        public override Type QuestItemType { get { return typeof(FlintsLogbook); } }
        public override int DefaultRange { get { return 5; } }

        [Constructable]
        public FlintLostLogbookHint()
            : base(1094974) // This appears to be Flint's logbook.  It is not clear why the goblins were using it in a ritual.  Perhaps they were summoning a nefarious intention?
        {
        }

        public FlintLostLogbookHint(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}