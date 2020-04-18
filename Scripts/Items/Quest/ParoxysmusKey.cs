using Server.Engines.PartySystem;
using Server.Network;

namespace Server.Items
{
    public class ParoxysmusKey : MasterKey
    {
        public override int LabelNumber => 1074330;  // slimy ointment

        public ParoxysmusKey()
            : base(0xEFB)
        {
            Weight = 1.0;
            Hue = 0x497;
        }

        public ParoxysmusKey(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (Altar != null && (Altar.Owner == from || Party.Get(from) == Party.Get(Altar.Owner)))
            {
                if (Altar.Peerless == null)
                {
                    Altar.SpawnBoss();
                }

                Altar.AddFighter(from);
                ParoxysmusAltar.AddProtection(from);
                from.LocalOverheadMessage(MessageType.Regular, 58, 1074603); // You rub the slimy ointment on your body, temporarily protecting yourself from the corrosive river.
                Delete();
            }
        }

        public override int Lifespan => 600;

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

        public override bool CanOfferConfirmation(Mobile from)
        {
            if (from.Region != null && from.Region.IsPartOf("Palace of Paroxysmus"))
                return base.CanOfferConfirmation(from);

            return false;
        }
    }
}
