using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class AcidPopper : Item
    {
        public override int LabelNumber => 1095058;  // Acid Popper

        [Constructable]
        public AcidPopper()
            : this(1)
        {
        }

        [Constructable]
        public AcidPopper(int amount)
            : base(0x2808)
        {
            Hue = 0x3F;
            Stackable = true;
            Amount = amount;
        }

        public AcidPopper(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
            else
                from.BeginTarget(1, false, TargetFlags.None, BurnWeb_Callback);
        }

        private void BurnWeb_Callback(Mobile from, object targeted)
        {
            SpiderWebbing web = targeted as SpiderWebbing;

            if (web != null)
            {
                from.SendLocalizedMessage(1113240); // The acid popper bursts and burns away the webbing.

                Effects.SendPacket(from.Location, from.Map, new TargetParticleEffect(this, 0x374A, 1, 10, 0x557, 0, 0x139D, 3, 0));

                from.PlaySound(0x3E);
                from.PlaySound(0x22F);

                web.Delete();
                Consume();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
