using System;
using Server.Targeting;

namespace Server.Items
{
    public abstract class CookableFood : Item
    {
        private int m_CookingLevel;

        [CommandProperty(AccessLevel.GameMaster)]
        public int CookingLevel
        {
            get
            {
                return this.m_CookingLevel;
            }
            set
            {
                this.m_CookingLevel = value;
            }
        }

        public CookableFood(int itemID, int cookingLevel)
            : base(itemID)
        {
            this.m_CookingLevel = cookingLevel;
        }

        public CookableFood(Serial serial)
            : base(serial)
        {
        }

        public abstract Food Cook();

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
            // Version 1
            writer.Write((int)this.m_CookingLevel);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_CookingLevel = reader.ReadInt();

                        break;
                    }
            }
        }

        #if false
		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			from.Target = new InternalTarget( this );
		}
        #endif

        public static bool IsHeatSource(object targeted)
        {
            int itemID;

            if (targeted is Item)
                itemID = ((Item)targeted).ItemID;
            else if (targeted is StaticTarget)
                itemID = ((StaticTarget)targeted).ItemID;
            else
                return false;

            if (itemID >= 0xDE3 && itemID <= 0xDE9)
                return true; // Campfire
            else if (itemID >= 0x461 && itemID <= 0x48E)
                return true; // Sandstone oven/fireplace
            else if (itemID >= 0x92B && itemID <= 0x96C)
                return true; // Stone oven/fireplace
            else if (itemID == 0xFAC)
                return true; // Firepit
            else if (itemID >= 0x184A && itemID <= 0x184C)
                return true; // Heating stand (left)
            else if (itemID >= 0x184E && itemID <= 0x1850)
                return true; // Heating stand (right)
            else if (itemID >= 0x398C && itemID <= 0x399F)
                return true; // Fire field

            return false;
        }

        private class InternalTarget : Target
        {
            private readonly CookableFood m_Item;

            public InternalTarget(CookableFood item)
                : base(1, false, TargetFlags.None)
            {
                this.m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Item.Deleted)
                    return;

                if (CookableFood.IsHeatSource(targeted))
                {
                    if (from.BeginAction(typeof(CookableFood)))
                    {
                        from.PlaySound(0x225);

                        this.m_Item.Consume();

                        InternalTimer t = new InternalTimer(from, targeted as IPoint3D, from.Map, this.m_Item);
                        t.Start();
                    }
                    else
                    {
                        from.SendLocalizedMessage(500119); // You must wait to perform another action
                    }
                }
            }

            private class InternalTimer : Timer
            {
                private readonly Mobile m_From;
                private readonly IPoint3D m_Point;
                private readonly Map m_Map;
                private readonly CookableFood m_CookableFood;
			
                public InternalTimer(Mobile from, IPoint3D p, Map map, CookableFood cookableFood)
                    : base(TimeSpan.FromSeconds(5.0))
                {
                    this.m_From = from;
                    this.m_Point = p;
                    this.m_Map = map;
                    this.m_CookableFood = cookableFood;
                }

                protected override void OnTick()
                {
                    this.m_From.EndAction(typeof(CookableFood));

                    if (this.m_From.Map != this.m_Map || (this.m_Point != null && this.m_From.GetDistanceToSqrt(this.m_Point) > 3))
                    {
                        this.m_From.SendLocalizedMessage(500686); // You burn the food to a crisp! It's ruined.
                        return;
                    }

                    if (this.m_From.CheckSkill(SkillName.Cooking, this.m_CookableFood.CookingLevel, 100))
                    {
                        Food cookedFood = this.m_CookableFood.Cook();

                        if (this.m_From.AddToBackpack(cookedFood))
                            this.m_From.PlaySound(0x57);
                    }
                    else
                    {
                        this.m_From.SendLocalizedMessage(500686); // You burn the food to a crisp! It's ruined.
                    }
                }
            }
        }
    }

    // ********** RawRibs **********
    public class RawRibs : CookableFood
    {
        [Constructable]
        public RawRibs()
            : this(1)
        {
        }

        [Constructable]
        public RawRibs(int amount)
            : base(0x9F1, 10)
        {
            this.Weight = 1.0;
            this.Stackable = true;
            this.Amount = amount;
        }

        public RawRibs(Serial serial)
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

        public override Food Cook()
        {
            return new Ribs();
        }
    }

    // ********** RawLambLeg **********
    public class RawLambLeg : CookableFood
    {
        [Constructable]
        public RawLambLeg()
            : this(1)
        {
        }

        [Constructable]
        public RawLambLeg(int amount)
            : base(0x1609, 10)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public RawLambLeg(Serial serial)
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

            if (version == 0 && this.Weight == 1)
                this.Weight = -1;
        }

        public override Food Cook()
        {
            return new LambLeg();
        }
    }

    // ********** RawChickenLeg **********
    public class RawChickenLeg : CookableFood
    {
        [Constructable]
        public RawChickenLeg()
            : base(0x1607, 10)
        {
            this.Weight = 1.0;
            this.Stackable = true;
        }

        public RawChickenLeg(Serial serial)
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

        public override Food Cook()
        {
            return new ChickenLeg();
        }
    }

    // ********** RawBird **********
    public class RawBird : CookableFood
    {
        [Constructable]
        public RawBird()
            : this(1)
        {
        }

        [Constructable]
        public RawBird(int amount)
            : base(0x9B9, 10)
        {
            this.Weight = 1.0;
            this.Stackable = true;
            this.Amount = amount;
        }

        public RawBird(Serial serial)
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

        public override Food Cook()
        {
            return new CookedBird();
        }
    }

    // ********** UnbakedPeachCobbler **********
    public class UnbakedPeachCobbler : CookableFood
    {
        public override int LabelNumber
        {
            get
            {
                return 1041335;
            }
        }// unbaked peach cobbler

        [Constructable]
        public UnbakedPeachCobbler()
            : base(0x1042, 25)
        {
            this.Weight = 1.0;
        }

        public UnbakedPeachCobbler(Serial serial)
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

        public override Food Cook()
        {
            return new PeachCobbler();
        }
    }

    // ********** UnbakedFruitPie **********
    public class UnbakedFruitPie : CookableFood
    {
        public override int LabelNumber
        {
            get
            {
                return 1041334;
            }
        }// unbaked fruit pie

        [Constructable]
        public UnbakedFruitPie()
            : base(0x1042, 25)
        {
            this.Weight = 1.0;
        }

        public UnbakedFruitPie(Serial serial)
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

        public override Food Cook()
        {
            return new FruitPie();
        }
    }

    // ********** UnbakedMeatPie **********
    public class UnbakedMeatPie : CookableFood
    {
        public override int LabelNumber
        {
            get
            {
                return 1041338;
            }
        }// unbaked meat pie

        [Constructable]
        public UnbakedMeatPie()
            : base(0x1042, 25)
        {
            this.Weight = 1.0;
        }

        public UnbakedMeatPie(Serial serial)
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

        public override Food Cook()
        {
            return new MeatPie();
        }
    }

    // ********** UnbakedPumpkinPie **********
    public class UnbakedPumpkinPie : CookableFood
    {
        public override int LabelNumber
        {
            get
            {
                return 1041342;
            }
        }// unbaked pumpkin pie

        [Constructable]
        public UnbakedPumpkinPie()
            : base(0x1042, 25)
        {
            this.Weight = 1.0;
        }

        public UnbakedPumpkinPie(Serial serial)
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

        public override Food Cook()
        {
            return new PumpkinPie();
        }
    }

    // ********** UnbakedApplePie **********
    public class UnbakedApplePie : CookableFood
    {
        public override int LabelNumber
        {
            get
            {
                return 1041336;
            }
        }// unbaked apple pie

        [Constructable]
        public UnbakedApplePie()
            : base(0x1042, 25)
        {
            this.Weight = 1.0;
        }

        public UnbakedApplePie(Serial serial)
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

        public override Food Cook()
        {
            return new ApplePie();
        }
    }

    // ********** UncookedCheesePizza **********
    [TypeAlias("Server.Items.UncookedPizza")]
    public class UncookedCheesePizza : CookableFood
    {
        public override int LabelNumber
        {
            get
            {
                return 1041341;
            }
        }// uncooked cheese pizza

        [Constructable]
        public UncookedCheesePizza()
            : base(0x1083, 20)
        {
            this.Weight = 1.0;
        }

        public UncookedCheesePizza(Serial serial)
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

            if (this.ItemID == 0x1040)
                this.ItemID = 0x1083;

            if (this.Hue == 51)
                this.Hue = 0;
        }

        public override Food Cook()
        {
            return new CheesePizza();
        }
    }

    // ********** UncookedSausagePizza **********
    public class UncookedSausagePizza : CookableFood
    {
        public override int LabelNumber
        {
            get
            {
                return 1041337;
            }
        }// uncooked sausage pizza

        [Constructable]
        public UncookedSausagePizza()
            : base(0x1083, 20)
        {
            this.Weight = 1.0;
        }

        public UncookedSausagePizza(Serial serial)
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

        public override Food Cook()
        {
            return new SausagePizza();
        }
    }

    #if false
	// ********** UncookedPizza **********
	public class UncookedPizza : CookableFood
	{
		[Constructable]
		public UncookedPizza() : base( 0x1083, 20 )
		{
			Weight = 1.0;
		}

		public UncookedPizza( Serial serial ) : base( serial )
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

			if ( ItemID == 0x1040 )
				ItemID = 0x1083;

			if ( Hue == 51 )
				Hue = 0;
		}

		public override Food Cook()
		{
			return new Pizza();
		}
	}
    #endif

    // ********** UnbakedQuiche **********
    public class UnbakedQuiche : CookableFood
    {
        public override int LabelNumber
        {
            get
            {
                return 1041339;
            }
        }// unbaked quiche

        [Constructable]
        public UnbakedQuiche()
            : base(0x1042, 25)
        {
            this.Weight = 1.0;
        }

        public UnbakedQuiche(Serial serial)
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

        public override Food Cook()
        {
            return new Quiche();
        }
    }

    // ********** Eggs **********
    public class Eggs : CookableFood
    {
        [Constructable]
        public Eggs()
            : this(1)
        {
        }

        [Constructable]
        public Eggs(int amount)
            : base(0x9B5, 15)
        {
            this.Weight = 1.0;
            this.Stackable = true;
            this.Amount = amount;
        }

        public Eggs(Serial serial)
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

            if (version < 1)
            {
                this.Stackable = true;

                if (this.Weight == 0.5)
                    this.Weight = 1.0;
            }
        }

        public override Food Cook()
        {
            return new FriedEggs();
        }
    }

    // ********** BrightlyColoredEggs **********
    public class BrightlyColoredEggs : CookableFood
    {
        public override string DefaultName
        {
            get
            {
                return "brightly colored eggs";
            }
        }

        [Constructable]
        public BrightlyColoredEggs()
            : base(0x9B5, 15)
        {
            this.Weight = 0.5;
            this.Hue = 3 + (Utility.Random(20) * 5);
        }

        public BrightlyColoredEggs(Serial serial)
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

        public override Food Cook()
        {
            return new FriedEggs();
        }
    }

    // ********** EasterEggs **********
    public class EasterEggs : CookableFood
    {
        public override int LabelNumber
        {
            get
            {
                return 1016105;
            }
        }// Easter Eggs

        [Constructable]
        public EasterEggs()
            : base(0x9B5, 15)
        {
            this.Weight = 0.5;
            this.Hue = 3 + (Utility.Random(20) * 5);
        }

        public EasterEggs(Serial serial)
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

        public override Food Cook()
        {
            return new FriedEggs();
        }
    }

    // ********** CookieMix **********
    public class CookieMix : CookableFood
    {
        [Constructable]
        public CookieMix()
            : base(0x103F, 20)
        {
            this.Weight = 1.0;
        }

        public CookieMix(Serial serial)
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

        public override Food Cook()
        {
            return new Cookies();
        }
    }

    // ********** CakeMix **********
    public class CakeMix : CookableFood
    {
        public override int LabelNumber
        {
            get
            {
                return 1041002;
            }
        }// cake mix

        [Constructable]
        public CakeMix()
            : base(0x103F, 40)
        {
            this.Weight = 1.0;
        }

        public CakeMix(Serial serial)
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

        public override Food Cook()
        {
            return new Cake();
        }
    }

    public class RawFishSteak : CookableFood
    {
        public override double DefaultWeight
        {
            get
            {
                return 0.1;
            }
        }

        [Constructable]
        public RawFishSteak()
            : this(1)
        {
        }

        [Constructable]
        public RawFishSteak(int amount)
            : base(0x097A, 10)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public RawFishSteak(Serial serial)
            : base(serial)
        {
        }

        public override Food Cook()
        {
            return new FishSteak();
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