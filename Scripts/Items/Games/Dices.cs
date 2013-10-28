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
            this.Weight = 1.0;
        }

        public Dices(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
                return;

            this.Roll(from);
        }

        public void OnTelekinesis(Mobile from)
        {
            Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5022);
            Effects.PlaySound(this.Location, this.Map, 0x1F5);

            this.Roll(from);
        }

        public void Roll(Mobile from)
        {
            this.PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("*{0} rolls {1}, {2}*", from.Name, Utility.Random(1, 6), Utility.Random(1, 6)));
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