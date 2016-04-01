using System;
using Server.Spells.Third;

namespace Server.Items
{
    public class FireballWand : BaseWand
    {
        [Constructable]
        public FireballWand()
            : base(WandEffect.Fireball, 5, Core.ML ? 109 : 15)
        {
        }

        public FireballWand(Serial serial)
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
            this.Cast(new FireballSpell(from, this));
        }
    }
}