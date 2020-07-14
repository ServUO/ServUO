using Server.Engines.Craft;
using System;

namespace Server.Items
{
    public abstract class CookableFood : Item, IQuality, ICommodity
    {
        private ItemQuality _Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality { get { return _Quality; } set { _Quality = value; InvalidateProperties(); } }

        public bool PlayerConstructed => true;

        public CookableFood(int itemID)
            : base(itemID)
        {
        }

        public CookableFood(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override void AddCraftedProperties(ObjectPropertyList list)
        {
            if (_Quality == ItemQuality.Exceptional)
            {
                list.Add(1060636); // Exceptional
            }
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            return quality;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(3); // version

            writer.Write((int)_Quality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Quality = (ItemQuality)reader.ReadInt();

            if (version < 3)
                reader.ReadInt();
        }
    }

    public class RawRibs : CookableFood
    {
        [Constructable]
        public RawRibs()
            : this(1)
        {
        }

        [Constructable]
        public RawRibs(int amount)
            : base(0x9F1)
        {
            Weight = 1.0;
            Stackable = true;
            Amount = amount;
        }

        public RawRibs(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class RawDinoRibs : CookableFood
    {
        public override int LabelNumber => 1126045; // raw dino ribs
        public override double DefaultWeight => 0.1;

        [Constructable]
        public RawDinoRibs()
            : this(1)
        {
        }

        [Constructable]
        public RawDinoRibs(int amount)
            : base(0xA425)
        {
            Stackable = true;
            Amount = amount;
        }

        public RawDinoRibs(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class RawSeaSerpentSteak : CookableFood
    {
        public override int LabelNumber => 1126041; // raw serpent steak
        public override double DefaultWeight => 0.1;

        [Constructable]
        public RawSeaSerpentSteak()
            : this(1)
        {
        }

        [Constructable]
        public RawSeaSerpentSteak(int amount)
            : base(0xA421)
        {
            Stackable = true;
            Amount = amount;
        }

        public RawSeaSerpentSteak(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class RawLambLeg : CookableFood
    {
        [Constructable]
        public RawLambLeg()
            : this(1)
        {
        }

        [Constructable]
        public RawLambLeg(int amount)
            : base(0x1609)
        {
            Stackable = true;
            Amount = amount;
        }

        public RawLambLeg(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class RawChickenLeg : CookableFood
    {
        [Constructable]
        public RawChickenLeg()
            : base(0x1607)
        {
            Weight = 1.0;
            Stackable = true;
        }

        public RawChickenLeg(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class RawBird : CookableFood
    {
        [Constructable]
        public RawBird()
            : this(1)
        {
        }

        [Constructable]
        public RawBird(int amount)
            : base(0x9B9)
        {
            Weight = 1.0;
            Stackable = true;
            Amount = amount;
        }

        public RawBird(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class UnbakedPeachCobbler : CookableFood
    {
        public override int LabelNumber => 1041335;// unbaked peach cobbler

        [Constructable]
        public UnbakedPeachCobbler()
            : base(0x1042)
        {
            Weight = 1.0;
        }

        public UnbakedPeachCobbler(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class UnbakedFruitPie : CookableFood
    {
        public override int LabelNumber => 1041334;// unbaked fruit pie

        [Constructable]
        public UnbakedFruitPie()
            : base(0x1042)
        {
            Weight = 1.0;
        }

        public UnbakedFruitPie(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class UnbakedMeatPie : CookableFood
    {
        public override int LabelNumber => 1041338;// unbaked meat pie

        [Constructable]
        public UnbakedMeatPie()
            : base(0x1042)
        {
            Weight = 1.0;
        }

        public UnbakedMeatPie(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class UnbakedPumpkinPie : CookableFood
    {
        public override int LabelNumber => 1041342;// unbaked pumpkin pie

        [Constructable]
        public UnbakedPumpkinPie()
            : base(0x1042)
        {
            Weight = 1.0;
        }

        public UnbakedPumpkinPie(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class UnbakedApplePie : CookableFood
    {
        public override int LabelNumber => 1041336;// unbaked apple pie

        [Constructable]
        public UnbakedApplePie()
            : base(0x1042)
        {
            Weight = 1.0;
        }

        public UnbakedApplePie(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class UncookedCheesePizza : CookableFood
    {
        public override int LabelNumber => 1041341;// uncooked cheese pizza

        [Constructable]
        public UncookedCheesePizza()
            : base(0x1083)
        {
            Weight = 1.0;
        }

        public UncookedCheesePizza(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class UncookedSausagePizza : CookableFood
    {
        public override int LabelNumber => 1041337;// uncooked sausage pizza

        [Constructable]
        public UncookedSausagePizza()
            : base(0x1083)
        {
            Weight = 1.0;
        }

        public UncookedSausagePizza(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class UnbakedQuiche : CookableFood
    {
        public override int LabelNumber => 1041339;// unbaked quiche

        [Constructable]
        public UnbakedQuiche()
            : base(0x1042)
        {
            Weight = 1.0;
        }

        public UnbakedQuiche(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class Eggs : CookableFood
    {
        [Constructable]
        public Eggs()
            : this(1)
        {
        }

        [Constructable]
        public Eggs(int amount)
            : base(0x9B5)
        {
            Weight = 1.0;
            Stackable = true;
            Amount = amount;
        }

        public Eggs(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class BrightlyColoredEggs : CookableFood
    {
        public override string DefaultName => "brightly colored eggs";

        [Constructable]
        public BrightlyColoredEggs()
            : base(0x9B5)
        {
            Weight = 0.5;
            Hue = 3 + (Utility.Random(20) * 5);
        }

        public BrightlyColoredEggs(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class EasterEggs : CookableFood
    {
        public override int LabelNumber => 1016105;// Easter Eggs

        [Constructable]
        public EasterEggs()
            : base(0x9B5)
        {
            Weight = 0.5;
            Hue = 3 + (Utility.Random(20) * 5);
        }

        public EasterEggs(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class CookieMix : CookableFood
    {
        [Constructable]
        public CookieMix()
            : base(0x103F)
        {
            Weight = 1.0;
        }

        public CookieMix(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class CakeMix : CookableFood
    {
        public override int LabelNumber => 1041002;// cake mix

        [Constructable]
        public CakeMix()
            : base(0x103F)
        {
            Weight = 1.0;
        }

        public CakeMix(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class RawFishSteak : CookableFood, ICommodity
    {
        public override double DefaultWeight => 0.1;

        [Constructable]
        public RawFishSteak()
            : this(1)
        {
        }

        [Constructable]
        public RawFishSteak(int amount)
            : base(0x097A)
        {
            Stackable = true;
            Amount = amount;
        }

        public RawFishSteak(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class RawRotwormMeat : CookableFood
    {
        [Constructable]
        public RawRotwormMeat()
            : this(1)
        {
        }

        [Constructable]
        public RawRotwormMeat(int amount)
            : base(0x2DB9)
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public RawRotwormMeat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
