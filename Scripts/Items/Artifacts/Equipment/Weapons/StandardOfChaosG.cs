using System;

namespace Server.Items
{
    [FlipableAttribute(0x904, 0x406D)]
    public class StandardOfChaosG : DualPointedSpear 
    {
        [Constructable]
        public StandardOfChaosG()
        {
            this.Name = ("Standard Of Chaos");
		
            this.Hue = 2209;
			
            this.WeaponAttributes.HitHarm = 30;	
            this.WeaponAttributes.HitFireball = 20;	
            this.WeaponAttributes.HitLightning = 10;
            this.WeaponAttributes.HitLowerDefend = 40;
            this.Attributes.WeaponSpeed = 30;
            this.Attributes.WeaponDamage = -40;
            this.Attributes.CastSpeed = 1;
            this.AosElementDamages.Chaos = 100;		
			this.StrRequirement = 40;
        }

        public StandardOfChaosG(Serial serial)
            : base(serial)
        {
        }

        //TODO: DoubleBladedSpear
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

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