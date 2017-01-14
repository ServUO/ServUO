using System;
using Server;

namespace Server.Items
{
    public class AbyssalDragonfish : RareFish
    {
        public override int LabelNumber { get { return 1116118; } }

        [Constructable]
        public AbyssalDragonfish()
            : base(Utility.RandomMinMax(17637, 17638))
        {
            Hue = FishInfo.GetFishHue(this.GetType());
        }

        public AbyssalDragonfish(Serial serial) : base(serial) { }

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

    public class BlackMarlin : RareFish
    {
        public override int LabelNumber { get { return 1116099; } }

        [Constructable]
        public BlackMarlin()
            : base(Utility.RandomMinMax(17156, 17157))
        {
            Hue = FishInfo.GetFishHue(this.GetType());
        }

        public BlackMarlin(Serial serial) : base(serial) { }

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

    public class BlueMarlin : RareFish
    {
        public override int LabelNumber { get { return 1116097; } }

        [Constructable]
        public BlueMarlin()
            : base(Utility.RandomMinMax(17156, 17157))
        {
            Hue = FishInfo.GetFishHue(this.GetType());
        }

        public BlueMarlin(Serial serial) : base(serial) { }

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

    public class DungeonPike : RareFish
    {
        public override int LabelNumber { get { return 1116107; } }

        [Constructable]
        public DungeonPike()
            : base(Utility.RandomMinMax(17603, 17604))
        {
            Hue = FishInfo.GetFishHue(this.GetType());
        }

        public DungeonPike(Serial serial) : base(serial) { }

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

    public class GiantSamuraiFish : RareFish
    {
        public override int LabelNumber { get { return 1116103; } }

        [Constructable]
        public GiantSamuraiFish()
            : base(Utility.RandomMinMax(17158, 17159))
        {
            Hue = FishInfo.GetFishHue(this.GetType());
        }

        public GiantSamuraiFish(Serial serial) : base(serial) { }

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

    public class GoldenTuna : RareFish
    {
        public override int LabelNumber { get { return 1116102; } }

        [Constructable]
        public GoldenTuna()
            : base(Utility.RandomMinMax(17154, 17155))
        {
            Hue = FishInfo.GetFishHue(this.GetType());
        }

        public GoldenTuna(Serial serial) : base(serial) { }

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

    public class Kingfish : RareFish
    {
        public override int LabelNumber { get { return 1116085; } }

        [Constructable]
        public Kingfish()
            : base(Utility.RandomMinMax(17158, 17159))
        {
            Hue = FishInfo.GetFishHue(this.GetType());
        }

        public Kingfish(Serial serial) : base(serial) { }

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

    public class LanternFish : RareFish
    {
        public override int LabelNumber { get { return 1116106; } }

        [Constructable]
        public LanternFish()
            : base(Utility.RandomMinMax(17605, 17606))
        {
            Hue = FishInfo.GetFishHue(this.GetType());
        }

        public LanternFish(Serial serial) : base(serial) { }

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

    public class RainbowFish : RareFish
    {
        public override int LabelNumber { get { return 1116108; } }

        [Constructable]
        public RainbowFish()
            : base(Utility.RandomMinMax(17154, 17155))
        {
            Hue = FishInfo.GetFishHue(this.GetType());
        }

        public RainbowFish(Serial serial) : base(serial) { }

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

    public class SeekerFish : RareFish
    {
        public override int LabelNumber { get { return 1116109; } }

        [Constructable]
        public SeekerFish()
            : base(Utility.RandomMinMax(17158, 17159))
        {
            Hue = FishInfo.GetFishHue(this.GetType());
        }

        public SeekerFish(Serial serial) : base(serial) { }

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

    public class SpringDragonfish : RareFish
    {
        public override int LabelNumber { get { return 1116104; } }

        [Constructable]
        public SpringDragonfish()
            : base(Utility.RandomMinMax(17637, 17638))
        {
            Hue = FishInfo.GetFishHue(this.GetType());
        }

        public SpringDragonfish(Serial serial) : base(serial) { }

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

    public class StoneFish : RareFish
    {
        public override int LabelNumber { get { return 1116110; } }

        [Constructable]
        public StoneFish()
            : base(Utility.RandomMinMax(17605, 17606))
        {
            Hue = FishInfo.GetFishHue(this.GetType());
        }

        public StoneFish(Serial serial) : base(serial) { }

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

    public class WinterDragonfish : RareFish
    {
        public override int LabelNumber { get { return 1116105; } }

        [Constructable]
        public WinterDragonfish()
            : base(Utility.RandomMinMax(17637, 17638))
        {
            Hue = FishInfo.GetFishHue(this.GetType());
        }

        public WinterDragonfish(Serial serial) : base(serial) { }

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

    public class ZombieFish : RareFish
    {
        public override int LabelNumber { get { return 1116101; } }

        [Constructable]
        public ZombieFish()
            : base(Utility.RandomMinMax(17603, 17604))
        {
            Hue = FishInfo.GetFishHue(this.GetType());
        }

        public ZombieFish(Serial serial) : base(serial) { }

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