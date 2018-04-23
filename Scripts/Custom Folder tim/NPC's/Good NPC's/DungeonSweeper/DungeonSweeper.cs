using System; 
using System.Collections; 
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
	{ 
   	[CorpseName( "a dungeon sweeper corpse" )] 
   	public class DungeonSweeper : BaseCreature 
   		{ 
      		[Constructable] 
      		public DungeonSweeper() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
         		{  
         		Name = "a dungeon sweeper"; 
         		Hue = 2106; 
	 		Body = 775;
         

         		SetStr( 186, 300 ); 
         		SetDex( 181, 295 ); 
         		SetInt( 61, 75 ); 

         		SetDamage( 13, 28 );

	 		SetDamageType( ResistanceType.Physical, 60 );
	 		SetDamageType( ResistanceType.Poison, 40 );

	 		SetResistance( ResistanceType.Physical, 45, 55 );
	 		SetResistance( ResistanceType.Fire, 40, 50 );
	 		SetResistance( ResistanceType.Cold, 25, 35 );
	 		SetResistance( ResistanceType.Poison, 65, 75 );
	 		SetResistance( ResistanceType.Energy, 25, 35 ); 

          
         		SetSkill( SkillName.MagicResist, 105.0, 107.5 );  
         		SetSkill( SkillName.Tactics, 85.0, 97.5 ); 
         		SetSkill( SkillName.Wrestling, 85.0, 97.5 ); 

         		Fame = 13000; 
         		Karma = -13000; 

         		VirtualArmor = 50;
 
         		}

      		public override void GenerateLoot()
	 		{
	 		AddLoot( LootPack.FilthyRich );
	 		AddLoot( LootPack.Gems, Utility.Random( 1, 2 ) );
	 		}

      		private DateTime m_NextPickup; 

      		public override void OnThink() 
         		{ 
         		base.OnThink(); 

         		if ( DateTime.Now < m_NextPickup ) 
            			return; 

         		m_NextPickup = DateTime.Now + TimeSpan.FromSeconds( 2.5 + (2.5 * Utility.RandomDouble()) ); 

         		ArrayList Trash = new ArrayList(); 
         		foreach ( Item item in this.GetItemsInRange( 2 ) ) 
            		{ 
            		if ( item.Movable ) 
               		Trash.Add(item); 
            		} 
         		Type[] exemptlist = new Type[]{ typeof(MandrakeRoot), typeof(Ginseng), typeof(AxeOfTheHeavens)}; //Short example list 
         		bool TrashIt = true; 
         		for (int i = 0; i < Trash.Count; i++) 
            		{ 
            		for (int j = 0; j < exemptlist.Length; j++) 
               		{ 
               		if ( (Trash[i]).GetType() == exemptlist[j] ) 
                  		TrashIt = false; 
               		} 
            		if (TrashIt) 
               		((Item)Trash[i]).Delete(); 
            			TrashIt = true; 
            		} 
         	} 

      		public DungeonSweeper( Serial serial ) : base( serial ) 
      		{ 
      		} 

      		public override void Serialize( GenericWriter writer ) 
      		{ 
      		base.Serialize( writer ); 
      		writer.Write( (int) 0 ); 
      		} 

      		public override void Deserialize( GenericReader reader ) 
      		{ 
      		base.Deserialize( reader ); 
      		int version = reader.ReadInt(); 
      		} 
   	} 
}