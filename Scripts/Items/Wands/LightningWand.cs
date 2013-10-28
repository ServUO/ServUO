using System;
using Server.Spells.Fourth;

namespace Server.Items
{
    public class LightningWand : BaseWand
    {
        [Constructable]
        public LightningWand()
            : base(WandEffect.Lightning, 5, Core.ML ? 109 : 20)
        {
        }

        public LightningWand(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void OnWandUse(Mobile from)
        {
            this.Cast(new LightningSpell(from, this));
        }
    }
}