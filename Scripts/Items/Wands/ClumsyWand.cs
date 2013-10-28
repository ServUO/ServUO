using System;
using Server.Spells.First;

namespace Server.Items
{
    public class ClumsyWand : BaseWand
    {
        [Constructable]
        public ClumsyWand()
            : base(WandEffect.Clumsiness, 5, 30)
        {
        }

        public ClumsyWand(Serial serial)
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
            this.Cast(new ClumsySpell(from, this));
        }
    }
}