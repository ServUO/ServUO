using System;

namespace Server.Items
{
    public class HealPotion : BaseHealPotion
    {
        [Constructable]
        public HealPotion()
            : base(PotionEffect.Heal)
        {
        }

        public HealPotion(Serial serial)
            : base(serial)
        {
        }

        public override int MinHeal
        {
            get
            {
                return (Core.AOS ? 13 : 6);
            }
        }
        public override int MaxHeal
        {
            get
            {
                return (Core.AOS ? 16 : 20);
            }
        }
        public override double Delay
        {
            get
            {
                return (Core.AOS ? 8.0 : 10.0);
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