using Server.Spells.Fourth;

namespace Server.Items
{
    public class LightningWand : BaseWand
    {
        [Constructable]
        public LightningWand()
            : base(WandEffect.Lightning, 5, 109)
        {
        }

        public LightningWand(Serial serial)
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

        public override void OnWandUse(Mobile from)
        {
            Cast(new LightningSpell(from, this));
        }
    }
}
