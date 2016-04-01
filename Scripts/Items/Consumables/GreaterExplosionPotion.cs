using System;

namespace Server.Items
{
    public class GreaterExplosionPotion : BaseExplosionPotion
    {
        [Constructable]
        public GreaterExplosionPotion()
            : base(PotionEffect.ExplosionGreater)
        {
        }

        public GreaterExplosionPotion(Serial serial)
            : base(serial)
        {
        }

        public override int MinDamage
        {
            get
            {
                return Core.AOS ? 20 : 15;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return Core.AOS ? 40 : 30;
            }
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
    }
}