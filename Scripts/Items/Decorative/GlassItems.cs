using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    [Flipable(0x182E, 0x182F, 0x1830, 0x1831)]
    public class SmallFlask : Item
    {
        [Constructable]
        public SmallFlask()
            : base(0x182E)
        {
            Weight = 1.0;
            Movable = true;
        }

        public SmallFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    [Flipable(0x182A, 0x182B, 0x182C, 0x182D)]
    public class MediumFlask : Item
    {
        [Constructable]
        public MediumFlask()
            : base(0x182A)
        {
            Weight = 1.0;
            Movable = true;
        }

        public MediumFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    [Flipable(0x183B, 0x183C, 0x183D)]
    public class LargeFlask : Item
    {
        [Constructable]
        public LargeFlask()
            : base(0x183B)
        {
            Weight = 1.0;
            Movable = true;
        }

        public LargeFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    [Flipable(0x1832, 0x1833, 0x1834, 0x1835, 0x1836, 0x1837)]
    public class CurvedFlask : Item
    {
        [Constructable]
        public CurvedFlask()
            : base(0x1832)
        {
            Weight = 1.0;
            Movable = true;
        }

        public CurvedFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    [Flipable(0x1838, 0x1839, 0x183A)]
    public class LongFlask : Item
    {
        [Constructable]
        public LongFlask()
            : base(0x1838)
        {
            Weight = 1.0;
            Movable = true;
        }

        public LongFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    [Flipable(0x1810, 0x1811)]
    public class SpinningHourglass : Item, IFlipable
    {
        public override int LabelNumber => 1044592;  // gargoyle hourglass

        [Constructable]
        public SpinningHourglass()
            : base(0x1810)
        {
            Weight = 1.0;
            Movable = true;
        }

        public void OnFlip(Mobile from)
        {
            if (ItemID == 0x1810)
            {
                ItemID = 0x1811;
            }
            else
            {
                ItemID = 0x1810;
            }
        }

        public SpinningHourglass(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class GreenBottle : Item
    {
        [Constructable]
        public GreenBottle()
            : base(0x0EFB)
        {
            Weight = 1.0;
            Movable = true;
        }

        public GreenBottle(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class RedBottle : Item
    {
        [Constructable]
        public RedBottle()
            : base(0x0EFC)
        {
            Weight = 1.0;
            Movable = true;
        }

        public RedBottle(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class SmallBrownBottle : Item
    {
        [Constructable]
        public SmallBrownBottle()
            : base(0x0EFD)
        {
            Weight = 1.0;
            Movable = true;
        }

        public SmallBrownBottle(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class SmallGreenBottle : Item
    {
        [Constructable]
        public SmallGreenBottle()
            : base(0x0F01)
        {
            Weight = 1.0;
            Movable = true;
        }

        public SmallGreenBottle(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class SmallVioletBottle : Item
    {
        [Constructable]
        public SmallVioletBottle()
            : base(0x0F02)
        {
            Weight = 1.0;
            Movable = true;
        }

        public SmallVioletBottle(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class TinyYellowBottle : Item
    {
        [Constructable]
        public TinyYellowBottle()
            : base(0x0F03)
        {
            Weight = 1.0;
            Movable = true;
        }

        public TinyYellowBottle(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    //remove 
    public class SmallBlueFlask : Item
    {
        [Constructable]
        public SmallBlueFlask()
            : base(0x182A)
        {
            Weight = 1.0;
            Movable = true;
        }

        public SmallBlueFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class SmallYellowFlask : Item
    {
        [Constructable]
        public SmallYellowFlask()
            : base(0x182B)
        {
            Weight = 1.0;
            Movable = true;
        }

        public SmallYellowFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class SmallRedFlask : Item
    {
        [Constructable]
        public SmallRedFlask()
            : base(0x182C)
        {
            Weight = 1.0;
            Movable = true;
        }

        public SmallRedFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class SmallEmptyFlask : Item
    {
        [Constructable]
        public SmallEmptyFlask()
            : base(0x182D)
        {
            Weight = 1.0;
            Movable = true;
        }

        public SmallEmptyFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class YellowBeaker : Item
    {
        [Constructable]
        public YellowBeaker()
            : base(0x182E)
        {
            Weight = 1.0;
            Movable = true;
        }

        public YellowBeaker(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class RedBeaker : Item
    {
        [Constructable]
        public RedBeaker()
            : base(0x182F)
        {
            Weight = 1.0;
            Movable = true;
        }

        public RedBeaker(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class BlueBeaker : Item
    {
        [Constructable]
        public BlueBeaker()
            : base(0x1830)
        {
            Weight = 1.0;
            Movable = true;
        }

        public BlueBeaker(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class GreenBeaker : Item
    {
        [Constructable]
        public GreenBeaker()
            : base(0x1831)
        {
            Weight = 1.0;
            Movable = true;
        }

        public GreenBeaker(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class EmptyCurvedFlaskW : Item
    {
        [Constructable]
        public EmptyCurvedFlaskW()
            : base(0x1832)
        {
            Weight = 1.0;
            Movable = true;
        }

        public EmptyCurvedFlaskW(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class RedCurvedFlask : Item
    {
        [Constructable]
        public RedCurvedFlask()
            : base(0x1833)
        {
            Weight = 1.0;
            Movable = true;
        }

        public RedCurvedFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class LtBlueCurvedFlask : Item
    {
        [Constructable]
        public LtBlueCurvedFlask()
            : base(0x1834)
        {
            Weight = 1.0;
            Movable = true;
        }

        public LtBlueCurvedFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class EmptyCurvedFlaskE : Item
    {
        [Constructable]
        public EmptyCurvedFlaskE()
            : base(0x1835)
        {
            Weight = 1.0;
            Movable = true;
        }

        public EmptyCurvedFlaskE(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class BlueCurvedFlask : Item
    {
        [Constructable]
        public BlueCurvedFlask()
            : base(0x1836)
        {
            Weight = 1.0;
            Movable = true;
        }

        public BlueCurvedFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class GreenCurvedFlask : Item
    {
        [Constructable]
        public GreenCurvedFlask()
            : base(0x1837)
        {
            Weight = 1.0;
            Movable = true;
        }

        public GreenCurvedFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class RedRibbedFlask : Item
    {
        [Constructable]
        public RedRibbedFlask()
            : base(0x1838)
        {
            Weight = 1.0;
            Movable = true;
        }

        public RedRibbedFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class VioletRibbedFlask : Item
    {
        [Constructable]
        public VioletRibbedFlask()
            : base(0x1839)
        {
            Weight = 1.0;
            Movable = true;
        }

        public VioletRibbedFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class EmptyRibbedFlask : Item
    {
        [Constructable]
        public EmptyRibbedFlask()
            : base(0x183A)
        {
            Weight = 1.0;
            Movable = true;
        }

        public EmptyRibbedFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class LargeYellowFlask : Item
    {
        [Constructable]
        public LargeYellowFlask()
            : base(0x183B)
        {
            Weight = 1.0;
            Movable = true;
        }

        public LargeYellowFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class LargeVioletFlask : Item
    {
        [Constructable]
        public LargeVioletFlask()
            : base(0x183C)
        {
            Weight = 1.0;
            Movable = true;
        }

        public LargeVioletFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class LargeEmptyFlask : Item
    {
        [Constructable]
        public LargeEmptyFlask()
            : base(0x183D)
        {
            Weight = 1.0;
            Movable = true;
        }

        public LargeEmptyFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class AniRedRibbedFlask : Item
    {
        [Constructable]
        public AniRedRibbedFlask()
            : base(0x183E)
        {
            Weight = 1.0;
            Movable = true;
        }

        public AniRedRibbedFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class AniLargeVioletFlask : Item
    {
        [Constructable]
        public AniLargeVioletFlask()
            : base(0x1841)
        {
            Weight = 1.0;
            Movable = true;
        }

        public AniLargeVioletFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class AniSmallBlueFlask : Item
    {
        [Constructable]
        public AniSmallBlueFlask()
            : base(0x1844)
        {
            Weight = 1.0;
            Movable = true;
        }

        public AniSmallBlueFlask(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class SmallBlueBottle : Item
    {
        [Constructable]
        public SmallBlueBottle()
            : base(0x1847)
        {
            Weight = 1.0;
            Movable = true;
        }

        public SmallBlueBottle(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class SmallGreenBottle2 : Item
    {
        [Constructable]
        public SmallGreenBottle2()
            : base(0x1848)
        {
            Weight = 1.0;
            Movable = true;
        }

        public SmallGreenBottle2(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    [Flipable(0x185B, 0x185C)]
    public class EmptyVialsWRack : Item
    {
        [Constructable]
        public EmptyVialsWRack()
            : base(0x185B)
        {
            Weight = 1.0;
            Movable = true;
        }

        public EmptyVialsWRack(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    [Flipable(0x185D, 0x185E)]
    public class FullVialsWRack : Item
    {
        [Constructable]
        public FullVialsWRack()
            : base(0x185D)
        {
            Weight = 1.0;
            Movable = true;
        }

        public FullVialsWRack(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class EmptyVial : Item
    {
        [Constructable]
        public EmptyVial()
            : base(0x0E24)
        {
            Weight = 1.0;
            Movable = true;
        }

        public EmptyVial(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class HourglassAni : Item
    {
        [Constructable]
        public HourglassAni()
            : base(0x1811)
        {
            Weight = 1.0;
            Movable = true;
        }

        public HourglassAni(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class Hourglass : Item
    {
        [Constructable]
        public Hourglass()
            : base(0x1810)
        {
            Weight = 1.0;
            Movable = true;
        }

        public Hourglass(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class TinyRedBottle : Item
    {
        [Constructable]
        public TinyRedBottle()
            : base(0x0F04)
        {
            Weight = 1.0;
            Movable = true;
        }

        public TinyRedBottle(Serial serial)
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

            int version = reader.ReadInt();
        }
    }

    public class EmptyVenomVial : Item
    {
        public override int LabelNumber => 1112215;  // empty venom vial
        [Constructable]
        public EmptyVenomVial()
            : base(0x0E24)
        {
            Weight = 1.0;
            Name = "Empty Venom Vial";
            Hue = 0;
        }
        public EmptyVenomVial(Serial serial)
            : base(serial)
        {
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                from.Target = new VenomTarget(this);
                from.SendLocalizedMessage(1112222); // Which creature do you wish to extract resources from?
            }
        }
        public class VenomTarget : Target
        {
            private readonly EmptyVenomVial m_EmptyVenomVial;
            public VenomTarget(Mobile from)
                : base(2, false, TargetFlags.None)
            {
            }
            public VenomTarget(EmptyVenomVial Vial)
                : base(2, false, TargetFlags.None)
            {
                m_EmptyVenomVial = Vial;
            }
            protected override void OnTarget(Mobile from, object target)
            {
                if (target is SilverSerpent)
                {
                    SilverSerpent serp = target as SilverSerpent;
                    if (serp.Hue == 1150)
                    {
                        if (0.3 > Utility.RandomDouble())
                        {
                            from.SendLocalizedMessage(1112219); // You skillfully extract additional resources from the creature.
                            from.AddToBackpack(new SilverSerpentVenom());
                            m_EmptyVenomVial.Delete();
                            serp.Hue = 0;
                        }
                        else
                            from.SendLocalizedMessage(1112218); // You handle the creature but fail to harvest any resources from it.
                    }
                    else
                        from.SendLocalizedMessage(1112223);// This serpent has already been drained of all its venom.
                }
                else
                    from.SendLocalizedMessage(1112221); // You may only use this on a silver serpent.
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
