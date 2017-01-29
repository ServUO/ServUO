using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Mobiles; 

namespace drNO.ThieveItems
{
    class BalmOfSwiftness : BaseBalmOrLotion
    {
        [Constructable] 
        public BalmOfSwiftness()
            : base(0x1848)
        {
            Name = "Balm of Swiftness";
            m_EffectType = ThieveConsumableEffect.BalmOfSwiftnessEffect;
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
            pm.AddStatMod(new StatMod(StatType.Dex, "Balm", 10, m_EffectDuration));
            pm.SendLocalizedMessage(1095138);//You apply the balm and suddenly feel more agile!
            base.ApplyEffect(pm);
        }

        public BalmOfSwiftness(Serial serial)
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
