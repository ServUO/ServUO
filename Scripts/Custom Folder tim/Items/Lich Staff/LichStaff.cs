//Tukaram June2017
using System;
using Server;

namespace Server.Items
{
    public class LichStaff : WildStaff
    {
		public override int ArtifactRarity{ get{ return 10; } } 

		public override int InitMinHits{ get{ return 75; } }  
		public override int InitMaxHits{ get{ return 100; } }

        public override int AosMinDamage { get { return 45; } }  
        public override int AosMaxDamage { get { return 55; } }

        [Constructable]
		public LichStaff()  
		{
			Name = "Lich Staff"; 
			Hue = 1152;  
			Weight = 3; 
             Slayer = SlayerName.Silver;  
            // LootType = LootType.Blessed;  //Newbie, Blessed, Cursed, or leave commented out for Regular.

             Attributes.BonusMana = 10;
             Attributes.BonusInt = 10;
             Attributes.SpellChanneling = 1;  //Spell chanelling and night sight are 1 if true, leave remarked out if false.
             Attributes.LowerManaCost = 10;
             Attributes.LowerRegCost = 10;
             Attributes.NightSight = 1;
             WeaponAttributes.UseBestSkill = 1;


        }


		public LichStaff ( Serial serial ) : base( serial )  
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

	}
}
