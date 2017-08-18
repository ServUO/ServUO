using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    [TypeAlias("drNO.ThieveItems.BalmOfWisdom")]
    public class BalmOfWisdom : BaseBalmOrLotion
    {
        public override int LabelNumber { get { return 1094941; } }  // Balm of Wisdom

        [Constructable]
        public BalmOfWisdom()
            : base(0x1847)
        {
            m_EffectType = ThieveConsumableEffect.BalmOfWisdomEffect;
        }

        protected override void ApplyEffect(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Int, "Balm", 10, m_EffectDuration));
            pm.SendLocalizedMessage(1095137);//You apply the balm and suddenly feel wiser!
            base.ApplyEffect(pm);
        }

        public BalmOfWisdom(Serial serial)
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
    }
}
