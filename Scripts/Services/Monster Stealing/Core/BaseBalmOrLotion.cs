using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Mobiles; 

namespace Server.Items
{
    public class BaseBalmOrLotion : BaseThieveConsumable
    {
        public BaseBalmOrLotion(int itemId) : base(itemId) 
        {
            m_EffectDuration = TimeSpan.FromMinutes(30);
            Weight = 1.0; 
        }

        protected override void OnUse(PlayerMobile by)
        {
            if (m_EffectType == ThieveConsumableEffect.None)
            {
                by.SendMessage("This balm or lotion is corrupted. Please contact a game master");
                return; 
            }

            if (CanUse(by, this))
            {
                ApplyEffect(by);
            }
            else
            {
                by.SendLocalizedMessage(1095133);//You are already under the effect of a balm or lotion.
            }
        }

        
        public BaseBalmOrLotion(Serial serial)
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
