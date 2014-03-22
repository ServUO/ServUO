using System;
using Server.Mobiles;

namespace Server.Items
{ 
    
    public class ConjurersTrinket : BaseTalisman
    {
        [Constructable]
        public ConjurersTrinket()
            : base(0x2F58)
        { 	
			this.Hue = 0x4AA;
			this.Name = "Conjurer's Trinket";
            this.Slayer = TalismanSlayerName.Undead;
			this.Attributes.BonusStr = 1;			
            this.Attributes.RegenHits = 2;
            this.Attributes.WeaponDamage = 20;
			this.Attributes.AttackChance = 10;
        }

        public ConjurersTrinket(Serial serial)
            : base(serial)
        {
        }

// Conjurer's Trinket
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