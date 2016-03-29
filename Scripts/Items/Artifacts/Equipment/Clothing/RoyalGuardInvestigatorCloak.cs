using System;

namespace Server.Items
{
    public class RoyalGuardInvestigatorsCloak : Cloak
    {
        [Constructable]
        public RoyalGuardInvestigatorsCloak()
            : base()
        { 
            this.Hue = 1163;
            
            this.SkillBonuses.SetValues(0, SkillName.Stealth, 20.0);        
        }

        public RoyalGuardInvestigatorsCloak(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1112409;
            }
        }// Royal Guard Investigator's Cloak [Replica]
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
        }
    }
}