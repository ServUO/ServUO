using System;
using System.Collections.Generic;
using Server.ContextMenus;

using CustomsFramework;

namespace Server.Items
{
    public abstract class Food : Item, IEngravable
    {
        private Mobile m_Poisoner;
        private Poison m_Poison;
        private int m_FillFactor;
        private bool m_PlayerConstructed;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Poisoner
        {
            get
            {
                return this.m_Poisoner;
            }
            set
            {
                this.m_Poisoner = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed
        {
            get
            {
                return this.m_PlayerConstructed;
            }
            set
            {
                this.m_PlayerConstructed = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Poison Poison
        {
            get
            {
                return this.m_Poison;
            }
            set
            {
                this.m_Poison = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FillFactor
        {
            get
            {
                return this.m_FillFactor;
            }
            set
            {
                this.m_FillFactor = value;
            }
        }

		private string m_EngravedText = string.Empty;

		[CommandProperty(AccessLevel.GameMaster)]
		public string EngravedText
		{
			get { return this.m_EngravedText; }
			set
			{
				if (value != null)
					this.m_EngravedText = value;
				else
					this.m_EngravedText = string.Empty;
				this.InvalidateProperties();
			}
		}

		public Food(int itemID)
            : this(1, itemID)
        {
        }

        public Food(int amount, int itemID)
            : base(itemID)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.m_FillFactor = 1;
        }

        public Food(Serial serial)
            : base(serial)
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive)
                list.Add(new ContextMenus.EatEntry(from, this));
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.Movable)
                return;

            if (from.InRange(this.GetWorldLocation(), 1))
            {
                this.Eat(from);
            }
        }

        public override bool StackWith(Mobile from, Item dropped, bool playSound)
        {
            if (dropped is Food && ((Food)dropped).PlayerConstructed == this.PlayerConstructed)
                return base.StackWith(from, dropped, playSound);
            else
                return false;
        }

		public override void AddNameProperty(ObjectPropertyList list)
		{
			base.AddNameProperty(list);

			if (!String.IsNullOrEmpty(this.EngravedText))
			{
				list.Add(1072305, this.EngravedText); // Engraved: ~1_INSCRIPTION~
			}
		}

		public virtual bool Eat(Mobile from)
        {
            // Fill the Mobile with FillFactor
            if (this.CheckHunger(from))
            {
                // Play a random "eat" sound
                from.PlaySound(Utility.Random(0x3A, 3));

                if (from.Body.IsHuman && !from.Mounted)
                    from.Animate(34, 5, 1, true, false, 0);

                if (this.m_Poison != null)
                    from.ApplyPoison(this.m_Poisoner, this.m_Poison);

                this.Consume();

                EventSink.InvokeOnConsume(new OnConsumeEventArgs(from, this));

                return true;
            }

            return false;
        }

        public virtual bool CheckHunger(Mobile from)
        {
            return FillHunger(from, this.m_FillFactor);
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

            writer.Write((int)6); // version

			writer.Write(this.m_EngravedText);

            writer.Write((bool)m_PlayerConstructed);
            writer.Write(this.m_Poisoner);

            Poison.Serialize(this.m_Poison, writer);
            writer.Write(this.m_FillFactor);
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
                                this.m_Poison = null;
                                break;
                            case 1:
                                this.m_Poison = Poison.Lesser;
                                break;
                            case 2:
                                this.m_Poison = Poison.Regular;
                                break;
                            case 3:
                                this.m_Poison = Poison.Greater;
                                break;
                            case 4:
                                this.m_Poison = Poison.Deadly;
                                break;
                        }

                        break;
                    }
                case 2:
                    {
                        this.m_Poison = Poison.Deserialize(reader);
                        break;
                    }
                case 3:
                    {
                        this.m_Poison = Poison.Deserialize(reader);
                        this.m_FillFactor = reader.ReadInt();
                        break;
                    }
                case 4:
                    {
                        this.m_Poisoner = reader.ReadMobile();
                        goto case 3;
                    }
                case 5:
                    {
                        this.m_PlayerConstructed = reader.ReadBool();
                        goto case 4;
                    }
				case 6:
					this.m_EngravedText = reader.ReadString();
					goto case 5;
            }
        }
    }

    public class BreadLoaf : Food
    {
        [Constructable]
        public BreadLoaf()
            : this(1)
        {
        }

        [Constructable]
        public BreadLoaf(int amount)
            : base(amount, 0x103B)
        {
            this.Weight = 1.0;
            this.FillFactor = 3;
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
            this.Weight = 1.0;
            this.FillFactor = 1;
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
            this.Weight = 1.0;
            this.FillFactor = 3;
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
            this.FillFactor = 3;
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
            this.FillFactor = 3;
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
            this.FillFactor = 3;
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
            this.FillFactor = 1;
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
            this.Weight = 2.0;
            this.FillFactor = 3;
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
        [Constructable]
        public FriedEggs()
            : this(1)
        {
        }

        [Constructable]
        public FriedEggs(int amount)
            : base(amount, 0x9B6)
        {
            this.Weight = 1.0;
            this.FillFactor = 4;
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
        [Constructable]
        public CookedBird()
            : this(1)
        {
        }

        [Constructable]
        public CookedBird(int amount)
            : base(amount, 0x9B7)
        {
            this.Weight = 1.0;
            this.FillFactor = 5;
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
            this.Weight = 45.0;
            this.FillFactor = 20;
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
            this.Weight = 1.0;
            this.FillFactor = 4;
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
            this.Weight = 1.0;
            this.FillFactor = 5;
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 10;
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
        [Constructable]
        public Ribs()
            : this(1)
        {
        }

        [Constructable]
        public Ribs(int amount)
            : base(amount, 0x9F2)
        {
            this.Weight = 1.0;
            this.FillFactor = 5;
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
            this.Stackable = Core.ML;
            this.Weight = 1.0;
            this.FillFactor = 4;
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 4;
        }

        public Muffins(Serial serial)
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 6;
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 6;
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
			this.Weight = 1.0;
			this.FillFactor = 6;
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 5;
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 5;
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 5;
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 5;
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 5;
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
            this.Stackable = Core.ML;
            this.Weight = 1.0;
            this.FillFactor = 5;
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
        [Constructable]
        public LambLeg()
            : this(1)
        {
        }

        [Constructable]
        public LambLeg(int amount)
            : base(amount, 0x160a)
        {
            this.Weight = 2.0;
            this.FillFactor = 5;
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
        [Constructable]
        public ChickenLeg()
            : this(1)
        {
        }

        [Constructable]
        public ChickenLeg(int amount)
            : base(amount, 0x1608)
        {
            this.Weight = 1.0;
            this.FillFactor = 4;
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
            this.Weight = 1.0;
            this.FillFactor = 1;
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
            this.Weight = 1.0;
            this.FillFactor = 1;
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
            this.Weight = 1.0;
            this.FillFactor = 1;
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
            this.Weight = 1.0;
            this.FillFactor = 1;
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
            this.Weight = 1.0;
            this.FillFactor = 1;
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
            this.Weight = 10.0;
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
}