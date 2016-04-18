using System;

namespace Server.Items
{
    public class EternalGuardianStaff : GnarledStaff
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public EternalGuardianStaff()
        {
            this.Name = ("Eternal Guardian Staff");
		
            this.Hue = 95;
			
            this.SkillBonuses.SetValues(0, SkillName.Mysticism, 15.0);		
            this.Attributes.SpellDamage = 10;
            this.Attributes.LowerManaCost = 5;	
            this.Attributes.SpellChanneling = 1;	
        }

        public EternalGuardianStaff(Serial serial)
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
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}