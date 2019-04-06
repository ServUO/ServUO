using System;

namespace Server.Items
{
    public class FallenMysticsSpellbook : Spellbook
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113867; } } // Fallen Mystic's Spellbook
		
        [Constructable]
        public FallenMysticsSpellbook()
            : this((ulong)0)
        {
        }

        [Constructable]
        public FallenMysticsSpellbook(ulong content)
            : base(content, 0x2D9D)
        {
            Hue = 687;		
            SkillBonuses.SetValues(0, SkillName.Mysticism, 10.0);			
            Attributes.LowerManaCost = 5;	
            Attributes.RegenMana = 1;
            Attributes.LowerRegCost = 10;
            Attributes.CastRecovery = 1;
            Attributes.CastSpeed = 1;		
            Attributes.SpellDamage = 10;
            Slayer = SlayerName.Fey;
        }

        public FallenMysticsSpellbook(Serial serial)
            : base(serial)
        {
        }
		
        public override SpellbookType SpellbookType
        {
            get
            {
                return SpellbookType.Mystic;
            }
        }
        public override int BookOffset
        {
            get
            {
                return 677;
            }
        }
        public override int BookCount
        {
            get
            {
                return 16;
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