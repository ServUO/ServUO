using System;

namespace Server.Items
{
    public class EmperorsDragon : SocketableBaseTalisman, ITokunoDyable
    {
        [Constructable]
        public EmperorsDragon() : base(0x2F59)
        {
            Hue = 589;
			Name = ("Emperor's Dragon");
			Attributes.AttackChance = 15;
			Attributes.WeaponDamage = 20;
			Attributes.WeaponSpeed = 20;
			Attributes.CastSpeed = 1;
			Attributes.LowerRegCost = 20;
			SkillBonuses.SetValues(0, SkillName.Swords, 10.0);
			SkillBonuses.SetValues(1, SkillName.Tactics, 10.0);
		}
		
		
		
		public override int ArtifactRarity 
		{ 
			get { return 100; } 
		}
		
		public override int BasePhysicalResistance
        {
            get { return 5; } 
		}
        public override int BaseFireResistance
        {
            get { return 5; }
        }
        public override int BaseColdResistance
        {
            get { return 5; }
        }
        public override int BasePoisonResistance
        {
            get { return 5; }
        }
        public override int BaseEnergyResistance
        {
            get { return 5; }
        }

        public EmperorsDragon(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version
        }
    }
}