using System;
using System.Collections.Generic;
using System.Text;
using Server.Mobiles;
using Server.Items;
using Server; 
namespace drNO.ThieveItems
{
    class BalmOfStrength : BaseBalmOrLotion
    {

        public override int LabelNumber
        {
            get
            {
                return 1094940;
            }
        }
        protected override void OnUse(PlayerMobile by)
        {

            base.OnUse(by); 
        }

        [Constructable]
        public BalmOfStrength()
            : base(0xEFB)
        {
            m_EffectType = ThieveConsumableEffect.BalmOfStrengthEffect;
            Name = "Balm of Strength";
        }

        public override void OnDoubleClick(Mobile from)
        {
            OnUse((PlayerMobile)from); 
        }

 

        protected override void ApplyEffect(PlayerMobile pm)
        {
            pm.AddStatMod(new StatMod(StatType.Str, "Balm", 10, m_EffectDuration)); 
            
            pm.SendLocalizedMessage(1095136); //You apply the balm and suddenly feel stronger!
            base.ApplyEffect(pm);

        }

          public BalmOfStrength(Serial serial)
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
