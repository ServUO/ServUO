/* Created By Kingdoms Development Team* Edited by Sneezy/*/
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class RangerCloak : BaseCloak
	{
		
		[Constructable]
		public RangerCloak() : base( 11013 )
		{
                          Weight = 0;
                          Name = "Ranger's Cloak";
                          Hue = 1270;
               SkillBonuses.SetValues( 0, SkillName.AnimalTaming, 10.0 );
               SkillBonuses.SetValues( 1, SkillName.AnimalLore, 10.0 );
               SkillBonuses.SetValues( 2, SkillName.Veterinary, 10.0 ); 
           
		}

        public RangerCloak(Serial serial)
            : base(serial)
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

			if ( Weight == 1.0 )
				Weight = 2.0;
		}
	}
}
