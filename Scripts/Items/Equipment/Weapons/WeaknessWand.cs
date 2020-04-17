using Server.Spells.First;

namespace Server.Items
{
    public class WeaknessWand : BaseWand
    {
        [Constructable]
        public WeaknessWand()
            : base(WandEffect.Weakness, 5, 30)
        {
        }

        public WeaknessWand(Serial serial)
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
            Cast(new WeakenSpell(from, this));
        }
    }
}