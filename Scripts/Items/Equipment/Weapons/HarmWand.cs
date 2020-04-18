using Server.Spells.Second;

namespace Server.Items
{
    public class HarmWand : BaseWand
    {
        [Constructable]
        public HarmWand()
            : base(WandEffect.Harming, 5, 109)
        {
        }

        public HarmWand(Serial serial)
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
            Cast(new HarmSpell(from, this));
        }
    }
}
