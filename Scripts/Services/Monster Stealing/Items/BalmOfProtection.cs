using System;
using System.Collections.Generic;
using System.Text;
using Server.Mobiles;
using Server.Items;
using Server;
namespace drNO.ThieveItems
{
    class BalmOfProtection : BaseBalmOrLotion
    {

        //public override int LabelNumber
        //{
        //    get
        //    {
        //        return 1094940;
        //    }
        //}


        public static double HandleDamage(PlayerMobile pm, double damage)
        {

            if (IsUnderThieveConsumableEffect(pm,ThieveConsumableEffect.BalmOfProtectionEffect))
            {
                int rnd = 50 + Utility.Random(51); 

                damage = damage- (damage * (rnd / 100.0)); 
                return damage; 
            }
            else 
            {
                return damage; 
            }

        }
        protected override void OnUse(PlayerMobile by)
        {

            base.OnUse(by);
        }

        [Constructable]
        public BalmOfProtection()
            : base(0x1C18)
        {
            m_EffectType = ThieveConsumableEffect.BalmOfProtectionEffect;
            Name = "Balm of Protection";
            Hue = 0x499; 
        }

        public override void OnDoubleClick(Mobile from)
        {
            OnUse((PlayerMobile)from);
        }



        protected override void ApplyEffect(PlayerMobile pm)
        {

            base.ApplyEffect(pm);
            pm.SendLocalizedMessage(1095143); //
        }

        public BalmOfProtection(Serial serial)
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
