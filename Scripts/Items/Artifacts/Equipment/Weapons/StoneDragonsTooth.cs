using System;

namespace Server.Items
{
    // Based off a Dagger
    [FlipableAttribute(0x902, 0x406A)]
    public class StoneDragonsTooth : GargishDagger
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public StoneDragonsTooth()
            : base()
        {
            this.Name = ("Stone Dragon's Tooth");
		
            this.Hue = 2407;
			
            this.Attributes.WeaponSpeed = 10;
            this.Attributes.WeaponDamage = 50;
            this.Attributes.RegenHits = 3;
            this.WeaponAttributes.HitMagicArrow = 40;
            this.WeaponAttributes.HitLowerDefend = 30;	
            this.WeaponAttributes.ResistFireBonus = 10;	
            this.AbsorptionAttributes.EaterPoison = 10;		
            this.AosElementDamages.Poison = 100;			
        }

        public StoneDragonsTooth(Serial serial)
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