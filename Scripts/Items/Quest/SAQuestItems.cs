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
            this.Stackable = true;
            this.Weight = 1.0;
            this.Hue = 648;
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
                this.m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = from as PlayerMobile;

                if (this.m_Item.Deleted)
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
                            this.wall = new SecretStoneWallNS();
                            this.wallandvine = new StoneWallAndVineAddon();
                        }
                        else if (addoncomponent.Addon is DungeonWallAndVineAddon)
                        {
                            this.wall = new SecretDungeonWallNS();
                            this.wallandvine = new DungeonWallAndVineAddon();
                        }

                        this.wall.MoveToWorld(new Point3D(Xs, addoncomponent.Y, addoncomponent.Z), addoncomponent.Map);

                        addoncomponent.Delete();

                        this.m_Item.Consume();

                        this.wall.PublicOverheadMessage(0, 1358, 1111662); // The acid quickly burns through the writhing wallvines, revealing the strange wall.

                        Timer.DelayCall(TimeSpan.FromSeconds(15.0), delegate()
                        {
                            this.wallandvine.MoveToWorld(this.wall.Location, this.wall.Map);

                            this.wall.Delete();
                            this.wallandvine.PublicOverheadMessage(0, 1358, 1111663); // The vines recover from the acid and, spreading like tentacles, reclaim their grip over the wall.
                        });
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1111657); // The acid swiftly burn through it.
                    this.m_Item.Consume();
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
			this.Hue = 2108;
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
            this.Stackable = true;
            this.Amount = amount;
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
            this.Stackable = true;
            this.Amount = amount;
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
            this.Stackable = true;
            this.Amount = amount;
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

    public class EnchantEssence : Item
    {
        [Constructable]
        public EnchantEssence()
            : this(1)
        {
        }

        [Constructable]
        public EnchantEssence(int amount)
            : base(0x2DB2)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public EnchantEssence(Serial serial)
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
            : base(0x5726)
        {
            this.Stackable = true;
            this.Amount = amount;
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
            : base(0x3189)
        {
            this.Stackable = true;
            this.Amount = amount;
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
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
            this.Stackable = true;
            this.Amount = amount;

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

    public class MagicalResidue : Item
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
            this.Stackable = true;
            this.Amount = amount;
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
            this.Stackable = true;
            this.Amount = amount;
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

    public class RelicFragment : Item
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
            this.Stackable = true;
            this.Amount = amount;
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
			this.Stackable = true;
			this.Amount = amount;
            this.Hue = 1359;
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
            this.Stackable = true;
            this.Amount = amount;
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
            this.Stackable = true;
            this.Amount = amount;
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (ItemID != 0x1437)
                ItemID = 0x1437;
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
            this.Stackable = true;
            this.Amount = amount;
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
            this.Stackable = true;
            this.Amount = amount;
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
            this.Stackable = true;
            this.Amount = amount;

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
            this.Stackable = true;
            this.Amount = amount;

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
}