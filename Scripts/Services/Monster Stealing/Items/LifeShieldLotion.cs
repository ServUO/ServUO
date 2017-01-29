using System;
using System.Collections.Generic;
using System.Text;
using Server.Mobiles;
using Server.Items;
using Server;
namespace drNO.ThieveItems
{
    class LifeShieldLotion : BaseBalmOrLotion
    {

        //public override int LabelNumber
        //{
        //    get
        //    {
        //        return 1094940;
        //    }
        //}


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
        protected override void OnUse(PlayerMobile by)
        {

            base.OnUse(by);
        }

        [Constructable]
        public LifeShieldLotion()
            : base(0xEFC)
        {
            m_EffectType = ThieveConsumableEffect.LifeShieldLotionEffect;
            Name = "Life Shield Lotion";
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

        public LifeShieldLotion(Serial serial)
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
