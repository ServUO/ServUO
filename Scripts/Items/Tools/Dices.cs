using System;
using Server.Network;

namespace Server.Items
{
    public class Dices : Item, ITelekinesisable
    {
        [Constructable]
        public Dices()
            : base(0xFA7)
        {
            Weight = 1.0;
        }

        public Dices(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
                return;

            Roll(from);
        }

        public void OnTelekinesis(Mobile from)
        {
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5022);
            Effects.PlaySound(Location, Map, 0x1F5);

            Roll(from);
        }

        public void Roll(Mobile from)
        {
            int one = Utility.Random(1, 6);
            int two = Utility.Random(1, 6);

            this.SendLocalizedMessage(MessageType.Emote, 1042713, AffixType.Prepend, from.Name + " ", ""); // The first die rolls to a stop and shows:
            this.SendLocalizedMessage(MessageType.Regular, 1042714, AffixType.Append, " " + one.ToString(), ""); // The first die rolls to a stop and shows:
            this.SendLocalizedMessage(MessageType.Regular, 1042715, AffixType.Append, " " + two.ToString(), ""); // The second die stops and shows:
            this.SendLocalizedMessage(MessageType.Regular, 1042716, AffixType.Append, " " + (one + two).ToString(), ""); // Total for this roll:
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
}