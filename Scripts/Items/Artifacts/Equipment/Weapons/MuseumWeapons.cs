using System;

namespace Server.Items
{
    public class BlackthornsKryss : Kryss
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public BlackthornsKryss()
            : base()
        {
            this.Hue = 0x5E5;

            this.Slayer = SlayerGroup.RandomSuperSlayerAOS();
			
            this.Attributes.WeaponSpeed = 25;
            this.Attributes.WeaponDamage = 50;
			
            this.WeaponAttributes.UseBestSkill = 1;
            this.WeaponAttributes.HitLeechHits = 22;
        }

        public BlackthornsKryss(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073260;
            }
        }// Blackthorn's Kryss - Museum of Vesper Replica	
        public override int InitMinHits
        {
            get
            {
                return 80;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 80;
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

    public class SwordOfJustice : VikingSword
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public SwordOfJustice()
            : base()
        {
            this.Hue = 0x47E;

            this.Slayer = SlayerGroup.RandomSuperSlayerAOS();
			
            this.Attributes.SpellChanneling = 1;
            this.Attributes.CastSpeed = -1;
            this.Attributes.WeaponDamage = 50;
            this.Attributes.Luck = 100;
			
            this.WeaponAttributes.UseBestSkill = 1;
            this.WeaponAttributes.HitLowerAttack = 60;
        }

        public SwordOfJustice(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073261;
            }
        }// Sword of Justice - Museum of Vesper Replica
        public override int InitMinHits
        {
            get
            {
                return 80;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 80;
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

    public class GeoffreysAxe : ExecutionersAxe
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public GeoffreysAxe()
            : base()
        {
            this.Hue = 0x21;

            this.Slayer = SlayerGroup.RandomSuperSlayerAOS();
			
            this.Attributes.BonusStr = 10;
            this.Attributes.AttackChance = 15;
            this.Attributes.WeaponDamage = 40;
            this.Attributes.Luck = 150;
			
            this.WeaponAttributes.ResistFireBonus = 10;			
            this.WeaponAttributes.UseBestSkill = 1;
            this.WeaponAttributes.HitLowerAttack = 60;
        }

        public GeoffreysAxe(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073263;
            }
        }// Geoffrey's Axe - Museum of Vesper Replica
        public override int InitMinHits
        {
            get
            {
                return 80;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 80;
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