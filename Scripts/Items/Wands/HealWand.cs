using System;
using Server.Spells.First;

namespace Server.Items
{
    public class HealWand : BaseWand
    {
        [Constructable]
        public HealWand()
            : base(WandEffect.Healing, 10, Core.ML ? 109 : 25)
        {
        }

        public HealWand(Serial serial)
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
            this.Cast(new HealSpell(from, this));
        }
    }
}