using Server.Network;
using System;

namespace Server.Items
{
    public abstract class BaseMagicFish : Item
    {
        public BaseMagicFish(int hue)
            : base(0xDD6)
        {
            Hue = hue;
        }

        public BaseMagicFish(Serial serial)
            : base(serial)
        {
        }

        public virtual int Bonus => 0;
        public virtual StatType Type => StatType.Str;
        public override double DefaultWeight => 1.0;
        public virtual bool Apply(Mobile from)
        {
            bool applied = Spells.SpellHelper.AddStatOffset(from, Type, Bonus, TimeSpan.FromMinutes(1.0));

            if (!applied)
                from.SendLocalizedMessage(502173); // You are already under a similar effect.

            return applied;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (Apply(from))
            {
                from.FixedEffect(0x375A, 10, 15);
                from.PlaySound(0x1E7);
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 501774); // You swallow the fish whole!
                Delete();
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

    public class PrizedFish : BaseMagicFish
    {
        [Constructable]
        public PrizedFish()
            : base(51)
        {
        }

        public PrizedFish(Serial serial)
            : base(serial)
        {
        }

        public override int Bonus => 5;
        public override StatType Type => StatType.Int;
        public override int LabelNumber => 1041073;// prized fish
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Hue == 151)
                Hue = 51;
        }
    }

    public class WondrousFish : BaseMagicFish
    {
        [Constructable]
        public WondrousFish()
            : base(86)
        {
        }

        public WondrousFish(Serial serial)
            : base(serial)
        {
        }

        public override int Bonus => 5;
        public override StatType Type => StatType.Dex;
        public override int LabelNumber => 1041074;// wondrous fish
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Hue == 286)
                Hue = 86;
        }
    }

    public class TrulyRareFish : BaseMagicFish
    {
        [Constructable]
        public TrulyRareFish()
            : base(76)
        {
        }

        public TrulyRareFish(Serial serial)
            : base(serial)
        {
        }

        public override int Bonus => 5;
        public override StatType Type => StatType.Str;
        public override int LabelNumber => 1041075;// truly rare fish
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Hue == 376)
                Hue = 76;
        }
    }

    public class PeculiarFish : BaseMagicFish
    {
        [Constructable]
        public PeculiarFish()
            : base(66)
        {
        }

        public PeculiarFish(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1041076;// highly peculiar fish
        public override bool Apply(Mobile from)
        {
            from.Stam += 10;
            return true;
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

            if (Hue == 266)
                Hue = 66;
        }
    }
}