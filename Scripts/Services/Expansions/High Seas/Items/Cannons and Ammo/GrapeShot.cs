using System;
using Server;

namespace Server.Items
{
    public class Grapeshot : Item, ICommodity, ICannonAmmo
    {
        public override int LabelNumber { get { return 1116030; } } // grapeshot
        public override double DefaultWeight { get { return 3.5; } }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public AmmunitionType AmmoType { get { return AmmunitionType.Grapeshot; } }

        [Constructable]
        public Grapeshot()
            : this(1)
        {
        }

        [Constructable]
        public Grapeshot(int amount)
            : base(0xA2BF)
        {
            Stackable = true;
            Amount = amount;
        }

        public Grapeshot(Serial serial)
            : base(serial)
        {
        }

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

    public class LightGrapeshot : Item, ICommodity, ICannonAmmo
    {
        public override int LabelNumber { get { return 1116030; } }
        public override double DefaultWeight { get { return 3.5; } }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public AmmunitionType AmmoType { get { return AmmunitionType.Grapeshot; } }

        [Constructable]
        public LightGrapeshot() : this(1)
        {
        }

        [Constructable]
        public LightGrapeshot(int amount) : base(16869)
        {
            Stackable = true;
            Amount = amount;
        }

        public LightGrapeshot(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Core.EJ)
            {
                Replacer.Replace(this, new Grapeshot());
            }
        }
    }

    public class HeavyGrapeshot : Item, ICommodity, ICannonAmmo
    {
        public override int LabelNumber { get { return 1116166; } }
        public override double DefaultWeight { get { return 7.0; } }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public AmmunitionType AmmoType { get { return AmmunitionType.Grapeshot; } }

        [Constructable]
        public HeavyGrapeshot() : this(1)
        {
        }

        [Constructable]
        public HeavyGrapeshot(int amount) : base(16869)
        {
            Stackable = true;
            Amount = amount;
        }

        public HeavyGrapeshot(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Core.EJ)
            {
                Replacer.Replace(this, new Grapeshot());
            }
        }
    }
}
