//This script was created by Lord Talon. 
//This Grand Skillball sets all skills to 120

using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Misc;

namespace Server
{
       public class GrandSkillball: Item
       {
           [Constructable]
           public GrandSkillball(): base( 0x1870 )
           {
               Weight = 1.0; 
			   Hue = 1153; 
			   Name = "Grand Skillball"; 
			   Movable =  true;
               LootType = LootType.Blessed;
		}
           public override void OnDoubleClick(Mobile m)
           {
               if (!IsChildOf(m.Backpack))
                   m.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
               else
               {
                   Server.Skills skills = m.Skills;
                   for (int i = 0; i < skills.Length; ++i)

                       skills[i].Base = 120;
                   this.Delete();
               }
           }

		public GrandSkillball( Serial serial ) : base( serial ) 
		{ 
		} 
       
		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 1 ); // version 
		}

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 
		} 
	}
}
       