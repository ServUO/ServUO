/*using System;
using Server;

namespace Server.Items
{
    public class LightScatterShot : Item, ICommodity, ICannonAmmo
    {
        public override double DefaultWeight { get { return 4.0; } }
        public override string DefaultName { get { return "Light Scatter Shot"; } }

        int ICommodity.DescriptionNumber { get { return 0; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public AmmoType AmmoType { get { return AmmoType.Grapeshot; } }

        [Constructable]
        public LightScatterShot()
            : this(1)
        {
        }

        [Constructable]
        public LightScatterShot(int amount)
            : base(16869)
        {
            Stackable = true;
            Amount = amount;
            Hue = 898;
        }

        public LightScatterShot(Serial serial) : base(serial) { }

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

    public class HeavyScatterShot : Item, ICommodity, ICannonAmmo
    {
        public override double DefaultWeight { get { return 8.0; } }
        public override string DefaultName { get { return "Heavy Scatter Shot"; } }

        int ICommodity.DescriptionNumber { get { return 0; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public AmmoType AmmoType { get { return AmmoType.Grapeshot; } }

        [Constructable]
        public HeavyScatterShot()
            : this(1)
        {
        }

        [Constructable]
        public HeavyScatterShot(int amount)
            : base(16869)
        {
            Stackable = true;
            Amount = amount;
            Hue = 899;
        }

        public HeavyScatterShot(Serial serial) : base(serial) { }

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

    public class LightFragShot : Item, ICommodity, ICannonAmmo
    {
        public override double DefaultWeight { get { return 6.0; } }
        public override string DefaultName { get { return "Light Fragmentation Shot"; } }

        int ICommodity.DescriptionNumber { get { return 0; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public AmmoType AmmoType { get { return AmmoType.Grapeshot; } }

        [Constructable]
        public LightFragShot()
            : this(1)
        {
        }

        [Constructable]
        public LightFragShot(int amount)
            : base(16869)
        {
            Stackable = true;
            Amount = amount;
            Hue = 390;
        }

        public LightFragShot(Serial serial) : base(serial) { }

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

    public class HeavyFragShot : Item, ICommodity, ICannonAmmo
    {
        public override double DefaultWeight { get { return 12.0; } }
        public override string DefaultName { get { return "Heavy Fragmentation Shot"; } }

        int ICommodity.DescriptionNumber { get { return 0; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public AmmoType AmmoType { get { return AmmoType.Grapeshot; } }

        [Constructable]
        public HeavyFragShot()
            : this(1)
        {
        }

        [Constructable]
        public HeavyFragShot(int amount)
            : base(16869)
        {
            Stackable = true;
            Amount = amount;
            Hue = 391;
        }

        public HeavyFragShot(Serial serial) : base(serial) { }

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

    public class LightHotShot : Item, ICommodity, ICannonAmmo
    {
        public override double DefaultWeight { get { return 6.0; } }
        public override string DefaultName { get { return "Light Hot Shot"; } }

        int ICommodity.DescriptionNumber { get { return 0; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public AmmoType AmmoType { get { return AmmoType.Cannonball; } }

        [Constructable]
        public LightHotShot()
            : this(1)
        {
        }

        [Constructable]
        public LightHotShot(int amount)
            : base(16932)
        {
            Stackable = true;
            Amount = amount;
            Hue = 1288;
        }

        public LightHotShot(Serial serial) : base(serial) { }

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

    public class HeavyHotShot : Item, ICommodity, ICannonAmmo
    {
        public override double DefaultWeight { get { return 12.0; } }
        public override string DefaultName { get { return "Heavy Hot Shot"; } }

        int ICommodity.DescriptionNumber { get { return 0; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public AmmoType AmmoType { get { return AmmoType.Cannonball; } }

        [Constructable]
        public HeavyHotShot()
            : this(1)
        {
        }

        [Constructable]
        public HeavyHotShot(int amount)
            : base(16932)
        {
            Stackable = true;
            Amount = amount;
            Hue = 1288;
        }

        public HeavyHotShot(Serial serial) : base(serial) { }

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
}*/