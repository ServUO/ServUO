using System;

namespace Server.Items
{
    public class MinaxsArmor : FemaleStuddedChest
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public MinaxsArmor()
            : base()
        {
            this.Hue = 0x453;
		
            this.Attributes.RegenMana = 2;
            this.ArmorAttributes.MageArmor = 1;			
        }

        public MinaxsArmor(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073257;
            }
        }// Minax's Armor - Museum of Vesper Replica
        public override int BasePhysicalResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 5;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 25;
            }
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

    public class KeeoneansChainMail : ChainChest
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public KeeoneansChainMail()
            : base()
        {
            this.Hue = 0x84E;
		
            this.Attributes.RegenHits = 3;
            this.Attributes.NightSight = 1;
            this.ArmorAttributes.MageArmor = 1;			
        }

        public KeeoneansChainMail(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073264;
            }
        }// Keeonean's Chain Mail - Museum of Vesper Replica
        public override int BasePhysicalResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 20;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 15;
            }
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

    public class VesperOrderShield : OrderShield
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public VesperOrderShield()
            : base()
        {
            this.Hue = 0x835;
		
            this.Attributes.SpellChanneling = 1;
            this.Attributes.Luck = 80;		
            this.Attributes.CastSpeed = -1;		
            this.Attributes.AttackChance = 15;		
            this.Attributes.DefendChance = 15;		
        }

        public VesperOrderShield(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073258;
            }
        }// Order Shield - Museum of Vesper Replica
        public override int BasePhysicalResistance
        {
            get
            {
                return 1;
            }
        }
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

    public class VesperChaosShield : ChaosShield
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public VesperChaosShield()
            : base()
        {
            this.Hue = 0xFA;
		
            this.Attributes.SpellChanneling = 1;
            this.Attributes.CastRecovery = 2;	
            this.Attributes.CastSpeed = 1;		
			
            this.ArmorAttributes.SelfRepair = 1;
        }

        public VesperChaosShield(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073259;
            }
        }// Chaos Shield - Museum of Vesper Replica
        public override int BasePhysicalResistance
        {
            get
            {
                return 1;
            }
        }
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