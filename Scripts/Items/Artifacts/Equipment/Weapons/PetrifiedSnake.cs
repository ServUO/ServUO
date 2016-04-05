using System;

namespace Server.Items
{
    public class PetrifiedSnake : SerpentStoneStaff
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public PetrifiedSnake()
            : base()
        {
            this.Name = ("Petrified Snake");
		
            this.Hue = 460;
			
            this.AbsorptionAttributes.EaterPoison = 20;	
            this.Slayer = SlayerName.ReptilianDeath;
            this.WeaponAttributes.HitMagicArrow = 30;
            this.WeaponAttributes.HitLowerDefend = 30;		
            this.Attributes.WeaponSpeed = 30;
            this.Attributes.WeaponDamage = 50;	
            this.AosElementDamages.Poison = 100;		
            this.WeaponAttributes.ResistPoisonBonus = 10;			
        }

        public PetrifiedSnake(Serial serial)
            : base(serial)
        {
        }

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
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
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