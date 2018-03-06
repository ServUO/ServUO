using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Craft;

using CustomsFramework;

namespace Server.Items
{
    public abstract class Food : Item, IEngravable, IQuality
    {
        private Mobile m_Poisoner;
        private Poison m_Poison;
        private int m_FillFactor;
        private bool m_PlayerConstructed;
        private ItemQuality _Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Poisoner
        {
            get
            {
                return m_Poisoner;
            }
            set
            {
                m_Poisoner = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed
        {
            get
            {
                return m_PlayerConstructed;
            }
            set
            {
                m_PlayerConstructed = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Poison Poison
        {
            get
            {
                return m_Poison;
            }
            set
            {
                m_Poison = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FillFactor
        {
            get
            {
                return m_FillFactor;
            }
            set
            {
                m_FillFactor = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual ItemQuality Quality { get { return _Quality; } set { _Quality = value; InvalidateProperties(); } }

		private string m_EngravedText = string.Empty;

		[CommandProperty(AccessLevel.GameMaster)]
		public string EngravedText
		{
			get { return m_EngravedText; }
			set
			{
				if (value != null)
					m_EngravedText = value;
				else
					m_EngravedText = string.Empty;

				InvalidateProperties();
			}
		}

		public Food(int itemID)
            : this(1, itemID)
        {
        }

        public Food(int amount, int itemID)
            : base(itemID)
        {
            Stackable = true;
            Amount = amount;
            m_FillFactor = 1;
        }

        public Food(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDuped(Item newItem)
        {
            Food food = newItem as Food;

            if (food == null)
                return;

            food.PlayerConstructed = m_PlayerConstructed;
            food.Poisoner = m_Poisoner;
            food.Poison = m_Poison;
            food.Quality = _Quality;
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            return quality;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive)
                list.Add(new ContextMenus.EatEntry(from, this));
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (_Quality == ItemQuality.Exceptional)
            {
                list.Add(1060636); // Exceptional
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!Movable)
                return;

            if (from.InRange(GetWorldLocation(), 1))
            {
                Eat(from);
            }
        }

        public override bool WillStack(Mobile from, Item dropped)
        {
            return dropped is Food && ((Food)dropped).PlayerConstructed == PlayerConstructed && base.WillStack(from, dropped);
        }

		public override void AddNameProperty(ObjectPropertyList list)
		{
			base.AddNameProperty(list);

			if (!String.IsNullOrEmpty(EngravedText))
			{
				list.Add(1072305, Utility.FixHtml(EngravedText)); // Engraved: ~1_INSCRIPTION~
			}
		}

		public virtual bool Eat(Mobile from)
        {
            // Fill the Mobile with FillFactor
            if (CheckHunger(from))
            {
                // Play a random "eat" sound
                from.PlaySound(Utility.Random(0x3A, 3));

                if (from.Body.IsHuman && !from.Mounted)
                {
                    if (Core.SA)
                    {
                        from.Animate(AnimationType.Eat, 0);
                    }
                    else
                    {
                        from.Animate(34, 5, 1, true, false, 0);
                    }
                }

                if (m_Poison != null)
                    from.ApplyPoison(m_Poisoner, m_Poison);

                Consume();

                EventSink.InvokeOnConsume(new OnConsumeEventArgs(from, this));

                return true;
            }

            return false;
        }

        public virtual bool CheckHunger(Mobile from)
        {
            return FillHunger(from, m_FillFactor);
        }

        public static bool FillHunger(Mobile from, int fillFactor)
        {
            if (from.Hunger >= 20)
            {
                from.SendLocalizedMessage(500867); // You are simply too full to eat any more!
                return false;
            }

            int iHunger = from.Hunger + fillFactor;

            if (from.Stam < from.StamMax)
                from.Stam += Utility.Random(6, 3) + fillFactor / 5;

            if (iHunger >= 20)
            {
                from.Hunger = 20;
                from.SendLocalizedMessage(500872); // You manage to eat the food, but you are stuffed!
            }
            else
            {
                from.Hunger = iHunger;

                if (iHunger < 5)
                    from.SendLocalizedMessage(500868); // You eat the food, but are still extremely hungry.
                else if (iHunger < 10)
                    from.SendLocalizedMessage(500869); // You eat the food, and begin to feel more satiated.
                else if (iHunger < 15)
                    from.SendLocalizedMessage(500870); // After eating the food, you feel much less hungry.
                else
                    from.SendLocalizedMessage(500871); // You feel quite full after consuming the food.
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)7); // version

            writer.Write((int)_Quality);

			writer.Write(m_EngravedText);

            writer.Write((bool)m_PlayerConstructed);
            writer.Write(m_Poisoner);

            Poison.Serialize(m_Poison, writer);
            writer.Write(m_FillFactor);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        switch ( reader.ReadInt() )
                        {
                            case 0:
                                m_Poison = null;
                                break;
                            case 1:
                                m_Poison = Poison.Lesser;
                                break;
                            case 2:
                                m_Poison = Poison.Regular;
                                break;
                            case 3:
                                m_Poison = Poison.Greater;
                                break;
                            case 4:
                                m_Poison = Poison.Deadly;
                                break;
                        }

                        break;
                    }
                case 2:
                    {
                        m_Poison = Poison.Deserialize(reader);
                        break;
                    }
                case 3:
                    {
                        m_Poison = Poison.Deserialize(reader);
                        m_FillFactor = reader.ReadInt();
                        break;
                    }
                case 4:
                    {
                        m_Poisoner = reader.ReadMobile();
                        goto case 3;
                    }
                case 5:
                    {
                        m_PlayerConstructed = reader.ReadBool();
                        goto case 4;
                    }
				case 6:
					m_EngravedText = reader.ReadString();
					goto case 5;
                case 7:
                    _Quality = (ItemQuality)reader.ReadInt();
                    goto case 6;
            }
        }
    }

    public class BreadLoaf : Food
    {
        public override ItemQuality Quality { get { return ItemQuality.Normal; } set { } }

        [Constructable]
        public BreadLoaf()
            : this(1)
        {
        }

        [Constructable]
        public BreadLoaf(int amount)
            : base(amount, 0x103B)
        {
            Weight = 1.0;
            FillFactor = 3;
        }

        public BreadLoaf(Serial serial)
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

    public class Bacon : Food
    {
        [Constructable]
        public Bacon()
            : this(1)
        {
        }

        [Constructable]
        public Bacon(int amount)
            : base(amount, 0x979)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Bacon(Serial serial)
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

    public class SlabOfBacon : Food
    {
        [Constructable]
        public SlabOfBacon()
            : this(1)
        {
        }

        [Constructable]
        public SlabOfBacon(int amount)
            : base(amount, 0x976)
        {
            Weight = 1.0;
            FillFactor = 3;
        }

        public SlabOfBacon(Serial serial)
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

    public class FishSteak : Food
    {
        public override ItemQuality Quality { get { return ItemQuality.Normal; } set { } }

        public override double DefaultWeight
        {
            get
            {
                return 0.1;
            }
        }

        [Constructable]
        public FishSteak()
            : this(1)
        {
        }

        [Constructable]
        public FishSteak(int amount)
            : base(amount, 0x97B)
        {
            FillFactor = 3;
        }

        public FishSteak(Serial serial)
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

    public class CheeseWheel : Food
    {
        public override double DefaultWeight
        {
            get
            {
                return 0.1;
            }
        }

        [Constructable]
        public CheeseWheel()
            : this(1)
        {
        }

        [Constructable]
        public CheeseWheel(int amount)
            : base(amount, 0x97E)
        {
            FillFactor = 3;
        }

        public CheeseWheel(Serial serial)
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

    public class CheeseWedge : Food
    {
        public override double DefaultWeight
        {
            get
            {
                return 0.1;
            }
        }

        [Constructable]
        public CheeseWedge()
            : this(1)
        {
        }

        [Constructable]
        public CheeseWedge(int amount)
            : base(amount, 0x97D)
        {
            FillFactor = 3;
        }

        public CheeseWedge(Serial serial)
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

    public class CheeseSlice : Food
    {
        public override double DefaultWeight
        {
            get
            {
                return 0.1;
            }
        }

        [Constructable]
        public CheeseSlice()
            : this(1)
        {
        }

        [Constructable]
        public CheeseSlice(int amount)
            : base(amount, 0x97C)
        {
            FillFactor = 1;
        }

        public CheeseSlice(Serial serial)
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

    public class FrenchBread : Food
    {
        [Constructable]
        public FrenchBread()
            : this(1)
        {
        }

        [Constructable]
        public FrenchBread(int amount)
            : base(amount, 0x98C)
        {
            Weight = 2.0;
            FillFactor = 3;
        }

        public FrenchBread(Serial serial)
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

    public class FriedEggs : Food
    {
        public override ItemQuality Quality { get { return ItemQuality.Normal; } set { } }

        [Constructable]
        public FriedEggs()
            : this(1)
        {
        }

        [Constructable]
        public FriedEggs(int amount)
            : base(amount, 0x9B6)
        {
            Weight = 1.0;
            FillFactor = 4;
        }

        public FriedEggs(Serial serial)
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

    public class CookedBird : Food
    {
        public override ItemQuality Quality { get { return ItemQuality.Normal; } set { } }

        [Constructable]
        public CookedBird()
            : this(1)
        {
        }

        [Constructable]
        public CookedBird(int amount)
            : base(amount, 0x9B7)
        {
            Weight = 1.0;
            FillFactor = 5;
        }

        public CookedBird(Serial serial)
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

    public class RoastPig : Food
    {
        [Constructable]
        public RoastPig()
            : this(1)
        {
        }

        [Constructable]
        public RoastPig(int amount)
            : base(amount, 0x9BB)
        {
            Weight = 45.0;
            FillFactor = 20;
        }

        public RoastPig(Serial serial)
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

    public class Sausage : Food
    {
        [Constructable]
        public Sausage()
            : this(1)
        {
        }

        [Constructable]
        public Sausage(int amount)
            : base(amount, 0x9C0)
        {
            Weight = 1.0;
            FillFactor = 4;
        }

        public Sausage(Serial serial)
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

    public class Ham : Food
    {
        [Constructable]
        public Ham()
            : this(1)
        {
        }

        [Constructable]
        public Ham(int amount)
            : base(amount, 0x9C9)
        {
            Weight = 1.0;
            FillFactor = 5;
        }

        public Ham(Serial serial)
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

    public class Cake : Food
    {
        [Constructable]
        public Cake()
            : base(0x9E9)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 10;
        }

        public Cake(Serial serial)
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

    public class Ribs : Food
    {
        public override ItemQuality Quality { get { return ItemQuality.Normal; } set { } }

        [Constructable]
        public Ribs()
            : this(1)
        {
        }

        [Constructable]
        public Ribs(int amount)
            : base(amount, 0x9F2)
        {
            Weight = 1.0;
            FillFactor = 5;
        }

        public Ribs(Serial serial)
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

    public class Cookies : Food
    {
        [Constructable]
        public Cookies()
            : base(0x160b)
        {
            Stackable = Core.ML;
            Weight = 1.0;
            FillFactor = 4;
        }

        public Cookies(Serial serial)
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

    public class Muffins : Food
    {
        [Constructable]
        public Muffins()
            : base(0x9eb)
        {
            Stackable = true;
            Weight = 1.0;
            FillFactor = 4;
        }

        public Muffins(Serial serial)
            : base(serial)
        {
        }

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
                Stackable = true;
        }
    }

    [TypeAlias("Server.Items.Pizza")]
    public class CheesePizza : Food
    {
        public override int LabelNumber
        {
            get
            {
                return 1044516;
            }
        }// cheese pizza

        [Constructable]
        public CheesePizza()
            : base(0x1040)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 6;
        }

        public CheesePizza(Serial serial)
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

    public class SausagePizza : Food
    {
        public override int LabelNumber
        {
            get
            {
                return 1044517;
            }
        }// sausage pizza

        [Constructable]
        public SausagePizza()
            : base(0x1040)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 6;
        }

        public SausagePizza(Serial serial)
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

    #if false
	public class Pizza : Food
	{
		[Constructable]
		public Pizza() : base( 0x1040 )
		{
			Stackable = false;
			Weight = 1.0;
			FillFactor = 6;
		}

		public Pizza( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
    #endif

    public class FruitPie : Food
    {
        public override int LabelNumber
        {
            get
            {
                return 1041346;
            }
        }// baked fruit pie

        [Constructable]
        public FruitPie()
            : base(0x1041)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 5;
        }

        public FruitPie(Serial serial)
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

    public class MeatPie : Food
    {
        public override int LabelNumber
        {
            get
            {
                return 1041347;
            }
        }// baked meat pie

        [Constructable]
        public MeatPie()
            : base(0x1041)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 5;
        }

        public MeatPie(Serial serial)
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

    public class PumpkinPie : Food
    {
        public override int LabelNumber
        {
            get
            {
                return 1041348;
            }
        }// baked pumpkin pie

        [Constructable]
        public PumpkinPie()
            : base(0x1041)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 5;
        }

        public PumpkinPie(Serial serial)
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

    public class ApplePie : Food
    {
        public override int LabelNumber
        {
            get
            {
                return 1041343;
            }
        }// baked apple pie

        [Constructable]
        public ApplePie()
            : base(0x1041)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 5;
        }

        public ApplePie(Serial serial)
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

    public class PeachCobbler : Food
    {
        public override int LabelNumber
        {
            get
            {
                return 1041344;
            }
        }// baked peach cobbler

        [Constructable]
        public PeachCobbler()
            : base(0x1041)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 5;
        }

        public PeachCobbler(Serial serial)
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

    public class Quiche : Food
    {
        public override int LabelNumber
        {
            get
            {
                return 1041345;
            }
        }// baked quiche

        [Constructable]
        public Quiche()
            : base(0x1041)
        {
            Stackable = Core.ML;
            Weight = 1.0;
            FillFactor = 5;
        }

        public Quiche(Serial serial)
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

    public class LambLeg : Food
    {
        public override ItemQuality Quality { get { return ItemQuality.Normal; } set { } }

        [Constructable]
        public LambLeg()
            : this(1)
        {
        }

        [Constructable]
        public LambLeg(int amount)
            : base(amount, 0x160a)
        {
            Weight = 2.0;
            FillFactor = 5;
        }

        public LambLeg(Serial serial)
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

    public class ChickenLeg : Food
    {
        public override ItemQuality Quality { get { return ItemQuality.Normal; } set { } }

        [Constructable]
        public ChickenLeg()
            : this(1)
        {
        }

        [Constructable]
        public ChickenLeg(int amount)
            : base(amount, 0x1608)
        {
            Weight = 1.0;
            FillFactor = 4;
        }

        public ChickenLeg(Serial serial)
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

    [FlipableAttribute(0xC74, 0xC75)]
    public class HoneydewMelon : Food
    {
        [Constructable]
        public HoneydewMelon()
            : this(1)
        {
        }

        [Constructable]
        public HoneydewMelon(int amount)
            : base(amount, 0xC74)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public HoneydewMelon(Serial serial)
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

    [FlipableAttribute(0xC64, 0xC65)]
    public class YellowGourd : Food
    {
        [Constructable]
        public YellowGourd()
            : this(1)
        {
        }

        [Constructable]
        public YellowGourd(int amount)
            : base(amount, 0xC64)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public YellowGourd(Serial serial)
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

    [FlipableAttribute(0xC66, 0xC67)]
    public class GreenGourd : Food
    {
        [Constructable]
        public GreenGourd()
            : this(1)
        {
        }

        [Constructable]
        public GreenGourd(int amount)
            : base(amount, 0xC66)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public GreenGourd(Serial serial)
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

    [FlipableAttribute(0xC7F, 0xC81)]
    public class EarOfCorn : Food
    {
        [Constructable]
        public EarOfCorn()
            : this(1)
        {
        }

        [Constructable]
        public EarOfCorn(int amount)
            : base(amount, 0xC81)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public EarOfCorn(Serial serial)
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

    public class Turnip : Food
    {
        [Constructable]
        public Turnip()
            : this(1)
        {
        }

        [Constructable]
        public Turnip(int amount)
            : base(amount, 0xD3A)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Turnip(Serial serial)
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

    public class SheafOfHay : Item
    {
        [Constructable]
        public SheafOfHay()
            : base(0xF36)
        {
            Weight = 10.0;
        }

        public SheafOfHay(Serial serial)
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

    public class ThreeTieredCake : Item, IQuality
    {
        private ItemQuality _Quality;
        private int _Pieces;

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality { get { return _Quality; } set { _Quality = value; InvalidateProperties(); } }

        public bool PlayerConstructed { get { return true; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Pieces 
        { 
            get { return _Pieces; }
            set 
            { 
                _Pieces = value; 

                if (_Pieces <= 0) 
                    Delete(); 
            } 
        }

        public override int LabelNumber { get { return 1098235; } } // A Three Tiered Cake 

        [Constructable]
        public ThreeTieredCake()
            : base(0x4BA3)
        {
            Weight = 1.0;
            Pieces = 10;
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            return quality;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                var cake = new Cake();
                cake.ItemID = 0x4BA4;

                from.PrivateOverheadMessage(Network.MessageType.Regular, 1154, 1157341, from.NetState); // *You cut a slice from the cake.*
                from.AddToBackpack(cake);

                Pieces--;
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (_Quality == ItemQuality.Exceptional)
            {
                list.Add(1060636); // Exceptional
            }
        }

        public ThreeTieredCake(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)_Quality);
            writer.Write(_Pieces);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _Quality = (ItemQuality)reader.ReadInt();
            _Pieces = reader.ReadInt();
        }
    }
}