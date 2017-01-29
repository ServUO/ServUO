using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Mobiles;
namespace drNO.ThieveItems
{
    class BalmOfWisdom : BaseBalmOrLotion
    {
        [Constructable]
        public BalmOfWisdom()
            : base(0x1847)
        {
            Name = "Balm of Wisdom";
            m_EffectType = ThieveConsumableEffect.BalmOfWisdomEffect;

        }

        protected override void OnUse(PlayerMobile by)
        {

            base.OnUse(by);
        }
        public override void OnDoubleClick(Mobile from)
        {
            OnUse((PlayerMobile)from);
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
