using Server.Mobiles;

namespace Server.Items
{
    [TypeAlias("drNO.ThieveItems.LifeShieldLotion")]
    public class LifeShieldLotion : BaseBalmOrLotion
    {
        public override int LabelNumber => 1094945;  // Life Shield Lotion

        [Constructable]
        public LifeShieldLotion()
            : base(0xEFC)
        {
            m_EffectType = ThieveConsumableEffect.LifeShieldLotionEffect;
        }

        public override void OnDoubleClick(Mobile from)
        {
            OnUse((PlayerMobile)from);
        }

        protected override void ApplyEffect(PlayerMobile pm)
        {
            base.ApplyEffect(pm);
            pm.SendMessage("You applied Life Shield Lotion");
        }

        public static double HandleLifeDrain(PlayerMobile pm, double damage)
        {
            if (IsUnderThieveConsumableEffect(pm, ThieveConsumableEffect.LifeShieldLotionEffect))
            {
                int rnd = 50 + Utility.Random(50);
                int dmgMod = (int)(damage * (rnd / 100.0));
                damage = damage - dmgMod;
                return damage;
            }
            else
            {
                return damage;
            }
        }

        public LifeShieldLotion(Serial serial)
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
    }
}
