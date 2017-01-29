using System;
using System.Collections.Generic;
using Server.Mobiles; 
using System.Text;
using Server; 

namespace drNO.ThieveItems
{
    class StoneSkinLotion : BaseBalmOrLotion

    {
        protected override void ApplyEffect(PlayerMobile pm)
        {
            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Cold, -5));
            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Fire, -5));

            pm.AddResistanceMod(new ResistanceMod(ResistanceType.Physical, 30)); 

            base.ApplyEffect(pm);
        }

        [Constructable] 
        public StoneSkinLotion()
            : base(0xEFD)
        {
            Name = "Stone Skin Lotion";
            m_EffectType = ThieveConsumableEffect.StoneSkinLotionEffect; 
        }

        public override void OnDoubleClick(Mobile from)
        {
            OnUse((PlayerMobile)from);
        }


        public StoneSkinLotion(Serial serial)
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
