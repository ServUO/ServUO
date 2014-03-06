using System;

namespace Server.Items
{
    public class FallenMysticsSpellbook : Spellbook
    {
        [Constructable]
        public FallenMysticsSpellbook()
            : this((ulong)0)
        {
        }

        [Constructable]
        public FallenMysticsSpellbook(ulong content)
            : base(content, 0x2D9D)
        {
            this.Name = ("Fallen Mystic's Spellbook");
		
            this.Hue = 687;
			
            this.SkillBonuses.SetValues(0, SkillName.Mysticism, 10.0);			
            this.Attributes.LowerManaCost = 5;	
            this.Attributes.RegenMana = 1;
            this.Attributes.LowerRegCost = 10;
            this.Attributes.CastRecovery = 1;
            this.Attributes.CastSpeed = 1;		
            this.Attributes.SpellDamage = 10;
            this.Slayer = SlayerName.Fey;
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