using System;

namespace Server.Items
{
    [FlipableAttribute(0x182E, 0x182F, 0x1830, 0x1831)]
    public class SmallFlask : Item
    {
        [Constructable]
        public SmallFlask()
            : base(0x182E)
        {
            this.Weight = 1.0;
            this.Movable = true;
        }

        public SmallFlask(Serial serial)
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

    [FlipableAttribute(0x182A, 0x182B, 0x182C, 0x182D)]
    public class MediumFlask : Item
    {
        [Constructable]
        public MediumFlask()
            : base(0x182A)
        {
            this.Weight = 1.0;
            this.Movable = true;
        }

        public MediumFlask(Serial serial)
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

    [FlipableAttribute(0x183B, 0x183C, 0x183D)]
    public class LargeFlask : Item
    {
        [Constructable]
        public LargeFlask()
            : base(0x183B)
        {
            this.Weight = 1.0;
            this.Movable = true;
        }

        public LargeFlask(Serial serial)
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

    [FlipableAttribute(0x1832, 0x1833, 0x1834, 0x1835, 0x1836, 0x1837)]
    public class CurvedFlask : Item
    {
        [Constructable]
        public CurvedFlask()
            : base(0x1832)
        {
            this.Weight = 1.0;
            this.Movable = true;
        }

        public CurvedFlask(Serial serial)
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

    [FlipableAttribute(0x1838, 0x1839, 0x183A)]
    public class LongFlask : Item
    {
        [Constructable]
        public LongFlask()
            : base(0x1838)
        {
            this.Weight = 1.0;
            this.Movable = true;
        }

        public LongFlask(Serial serial)
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

    [Flipable(0x1810, 0x1811)]
    public class SpinningHourglass : Item
    {
        [Constructable]
        public SpinningHourglass()
            : base(0x1810)
        {
            this.Weight = 1.0;
            this.Movable = true;
        }

        public SpinningHourglass(Serial serial)
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

    public class GreenBottle : Item 
    { 
        [Constructable] 
        public GreenBottle()
            : base(0x0EFB)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public GreenBottle(Serial serial)
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

    public class RedBottle : Item 
    { 
        [Constructable] 
        public RedBottle()
            : base(0x0EFC)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public RedBottle(Serial serial)
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

    public class SmallBrownBottle : Item 
    { 
        [Constructable] 
        public SmallBrownBottle()
            : base(0x0EFD)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public SmallBrownBottle(Serial serial)
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

    public class SmallGreenBottle : Item 
    { 
        [Constructable] 
        public SmallGreenBottle()
            : base(0x0F01)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public SmallGreenBottle(Serial serial)
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

    public class SmallVioletBottle : Item 
    { 
        [Constructable] 
        public SmallVioletBottle()
            : base(0x0F02)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public SmallVioletBottle(Serial serial)
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

    public class TinyYellowBottle : Item 
    { 
        [Constructable] 
        public TinyYellowBottle()
            : base(0x0F03)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public TinyYellowBottle(Serial serial)
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

    //remove 
    public class SmallBlueFlask : Item 
    { 
        [Constructable] 
        public SmallBlueFlask()
            : base(0x182A)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public SmallBlueFlask(Serial serial)
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

    public class SmallYellowFlask : Item 
    { 
        [Constructable] 
        public SmallYellowFlask()
            : base(0x182B)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public SmallYellowFlask(Serial serial)
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

    public class SmallRedFlask : Item 
    { 
        [Constructable] 
        public SmallRedFlask()
            : base(0x182C)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public SmallRedFlask(Serial serial)
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

    public class SmallEmptyFlask : Item 
    { 
        [Constructable] 
        public SmallEmptyFlask()
            : base(0x182D)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public SmallEmptyFlask(Serial serial)
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

    public class YellowBeaker : Item 
    { 
        [Constructable] 
        public YellowBeaker()
            : base(0x182E)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public YellowBeaker(Serial serial)
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

    public class RedBeaker : Item 
    { 
        [Constructable] 
        public RedBeaker()
            : base(0x182F)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public RedBeaker(Serial serial)
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

    public class BlueBeaker : Item 
    { 
        [Constructable] 
        public BlueBeaker()
            : base(0x1830)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public BlueBeaker(Serial serial)
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

    public class GreenBeaker : Item 
    { 
        [Constructable] 
        public GreenBeaker()
            : base(0x1831)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public GreenBeaker(Serial serial)
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

    public class EmptyCurvedFlaskW : Item 
    { 
        [Constructable] 
        public EmptyCurvedFlaskW()
            : base(0x1832)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public EmptyCurvedFlaskW(Serial serial)
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

    public class RedCurvedFlask : Item 
    { 
        [Constructable] 
        public RedCurvedFlask()
            : base(0x1833)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public RedCurvedFlask(Serial serial)
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

    public class LtBlueCurvedFlask : Item 
    { 
        [Constructable] 
        public LtBlueCurvedFlask()
            : base(0x1834)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public LtBlueCurvedFlask(Serial serial)
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

    public class EmptyCurvedFlaskE : Item 
    { 
        [Constructable] 
        public EmptyCurvedFlaskE()
            : base(0x1835)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public EmptyCurvedFlaskE(Serial serial)
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

    public class BlueCurvedFlask : Item 
    { 
        [Constructable] 
        public BlueCurvedFlask()
            : base(0x1836)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public BlueCurvedFlask(Serial serial)
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

    public class GreenCurvedFlask : Item 
    { 
        [Constructable] 
        public GreenCurvedFlask()
            : base(0x1837)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public GreenCurvedFlask(Serial serial)
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

    public class RedRibbedFlask : Item 
    { 
        [Constructable] 
        public RedRibbedFlask()
            : base(0x1838)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public RedRibbedFlask(Serial serial)
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

    public class VioletRibbedFlask : Item 
    { 
        [Constructable] 
        public VioletRibbedFlask()
            : base(0x1839)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public VioletRibbedFlask(Serial serial)
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

    public class EmptyRibbedFlask : Item 
    { 
        [Constructable] 
        public EmptyRibbedFlask()
            : base(0x183A)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public EmptyRibbedFlask(Serial serial)
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

    public class LargeYellowFlask : Item 
    { 
        [Constructable] 
        public LargeYellowFlask()
            : base(0x183B)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public LargeYellowFlask(Serial serial)
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

    public class LargeVioletFlask : Item 
    { 
        [Constructable] 
        public LargeVioletFlask()
            : base(0x183C)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public LargeVioletFlask(Serial serial)
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

    public class LargeEmptyFlask : Item 
    { 
        [Constructable] 
        public LargeEmptyFlask()
            : base(0x183D)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public LargeEmptyFlask(Serial serial)
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

    public class AniRedRibbedFlask : Item 
    { 
        [Constructable] 
        public AniRedRibbedFlask()
            : base(0x183E)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public AniRedRibbedFlask(Serial serial)
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

    public class AniLargeVioletFlask : Item 
    { 
        [Constructable] 
        public AniLargeVioletFlask()
            : base(0x1841)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public AniLargeVioletFlask(Serial serial)
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

    public class AniSmallBlueFlask : Item 
    { 
        [Constructable] 
        public AniSmallBlueFlask()
            : base(0x1844)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public AniSmallBlueFlask(Serial serial)
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

    public class SmallBlueBottle : Item 
    { 
        [Constructable] 
        public SmallBlueBottle()
            : base(0x1847)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public SmallBlueBottle(Serial serial)
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

    public class SmallGreenBottle2 : Item 
    { 
        [Constructable] 
        public SmallGreenBottle2()
            : base(0x1848)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public SmallGreenBottle2(Serial serial)
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

    [FlipableAttribute(0x185B, 0x185C)] 
    public class EmptyVialsWRack : Item 
    { 
        [Constructable] 
        public EmptyVialsWRack()
            : base(0x185B)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public EmptyVialsWRack(Serial serial)
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

    [FlipableAttribute(0x185D, 0x185E)] 
    public class FullVialsWRack : Item 
    { 
        [Constructable] 
        public FullVialsWRack()
            : base(0x185D)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public FullVialsWRack(Serial serial)
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

    public class EmptyVial : Item 
    { 
        [Constructable] 
        public EmptyVial()
            : base(0x0E24)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public EmptyVial(Serial serial)
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

    public class HourglassAni : Item 
    { 
        [Constructable] 
        public HourglassAni()
            : base(0x1811)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public HourglassAni(Serial serial)
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

    public class Hourglass : Item 
    { 
        [Constructable] 
        public Hourglass()
            : base(0x1810)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public Hourglass(Serial serial)
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

    public class TinyRedBottle : Item 
    { 
        [Constructable] 
        public TinyRedBottle()
            : base(0x0F04)
        { 
            this.Weight = 1.0;
            this.Movable = true; 
        }

        public TinyRedBottle(Serial serial)
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