using System;

namespace Server.Items
{
    public class LegacyOfDespair : DreadSword
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public LegacyOfDespair()
        {
            this.Name = ("Legacy Of Despair");
		
            this.Hue = 48;
		
            this.Attributes.WeaponSpeed = 30;
            this.Attributes.WeaponDamage = 60;
            this.WeaponAttributes.HitLowerDefend = 50;
            this.WeaponAttributes.HitLowerAttack = 50;
            this.WeaponAttributes.HitCurse = 10;		
            this.AosElementDamages.Cold = 75;
            this.AosElementDamages.Poison = 25;			
        }

        public LegacyOfDespair(Serial serial)
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