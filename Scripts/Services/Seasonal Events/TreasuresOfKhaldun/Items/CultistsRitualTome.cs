using System;

namespace Server.Items
{
    public class CultistsRitualTome : Spellbook
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1158717; } } // Cultist's Ritual Tome
        
        [Constructable]
        public CultistsRitualTome()
            : base()
        {
            Hue = 2745;
            LootType = LootType.Blessed;

            Slayer = SlayerGroup.RandomSuperSlayerTOL();
            //Caddellite Infused
            Attributes.DefendChance = 5;
            Attributes.SpellDamage = 25;
            Attributes.CastRecovery = 2;
            Attributes.LowerManaCost = 4;
        }

        public CultistsRitualTome(Serial serial)
            : base(serial)
        {
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
