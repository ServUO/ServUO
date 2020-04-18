using Server.Mobiles;

namespace Server.Items
{
    [TypeAlias("drNO.ThieveItems.BalmOfProtection")]
    public class BalmOfProtection : BaseBalmOrLotion
    {
        public static double HandleDamage(PlayerMobile pm, double damage)
        {

            if (IsUnderThieveConsumableEffect(pm, ThieveConsumableEffect.BalmOfProtectionEffect))
            {
                int rnd = 50 + Utility.Random(51);

                damage = damage - (damage * (rnd / 100.0));
                return damage;
            }
            else
            {
                return damage;
            }
        }

        public override int LabelNumber => 1094943;  // Balm of Protection

        [Constructable]
        public BalmOfProtection()
            : base(0x1C18)
        {
            m_EffectType = ThieveConsumableEffect.BalmOfProtectionEffect;
            Hue = 0x499;
        }

        protected override void ApplyEffect(PlayerMobile pm)
        {
            base.ApplyEffect(pm);
            pm.SendLocalizedMessage(1095143); // You apply the ointment and suddenly feel less vulnerable!
        }

        public BalmOfProtection(Serial serial)
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
