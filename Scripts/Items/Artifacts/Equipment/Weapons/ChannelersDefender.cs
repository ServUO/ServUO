using System;

namespace Server.Items
{
    public class ChannelersDefender : GlassSword
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113518; } } // Channeler's Defender
		
        [Constructable]
        public ChannelersDefender()
        {
            this.Name = ("Channeler's Defender");
		
            Hue = 95;	
            Attributes.DefendChance = 10;				
            Attributes.AttackChance = 5;	
            Attributes.LowerManaCost = 5;
            Attributes.WeaponSpeed = 20;					
            Attributes.CastRecovery = 1;		
            Attributes.SpellChanneling = 1;	
            WeaponAttributes.HitLowerAttack = 60;
            AosElementDamages.Energy = 100;		
        }

        public ChannelersDefender(Serial serial)
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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}