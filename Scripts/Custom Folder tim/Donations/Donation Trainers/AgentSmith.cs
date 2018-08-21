//Author:Neon Destiny
//First Alpha:11/11/03
//First part of my Matrix series
//email:NeonDestiny@hotmail.com
//ICQ:308073073
//Shard: Neon Destiny PVP




using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class AgentSmith : BaseCreature
	{

		private static bool m_Talked;

		string[] kfcsay = new string[]
		{
		"Doesn't your race realize it's doomed?",
		"Why do you bother fighting? You can't win...",
		"Feeble meat bag",
		"Ill let you go quickly, just stop fighting..."
		};
		[Constructable]
		public AgentSmith() : base( AIType.AI_Melee, FightMode.Weakest, 10, 1, 0.2, 0.4)
		{

			Name = "Agent";
			Title=" Smith";
			Hue= Utility.RandomSkinHue();
			Body = 400;
			SpeechHue=1109;
			BaseSoundID = 0;
			Team = 0;
            CantWalk = true;
			SetStr( 500, 550);
			SetDex( 100,150);
			SetInt( 50, 70);

			SetHits(400, 450);
			SetMana(50, 60);

			SetDamage( 10, 15);

			SetDamageType( ResistanceType.Physical, 80);
			SetDamageType( ResistanceType.Energy, 20);

			SetResistance( ResistanceType.Physical, 50, 55);
			SetResistance( ResistanceType.Cold, 40, 45);
			SetResistance( ResistanceType.Poison, 40, 45);
			SetResistance( ResistanceType.Energy, 20, 25);

			SetSkill( SkillName.Wrestling, 95.2, 98.6);
			SetSkill( SkillName.Tactics, 50.7, 60.4);
			SetSkill( SkillName.Anatomy, 213.5, 256.3);
			SetSkill( SkillName.MagicResist, 45.4, 54.7);

			Fame=6400;
			Karma=-10000;

			VirtualArmor= 45;

			Item Boots = new Boots();
			Boots.Movable=false;
			Boots.Hue=1;
			EquipItem( Boots );

			Item FancyShirt = new FancyShirt();
			FancyShirt.Movable=false;
			FancyShirt.Hue=1;
			EquipItem( FancyShirt );

			Item LongPants = new LongPants();
			LongPants.Movable=false;
			LongPants.Hue=1;
			EquipItem( LongPants );

			Item hair = new Item( 0x203B);
			hair.Hue = 1109;
			hair.Layer = Layer.Hair;
			hair.Movable = false;
			AddItem( hair );

			//PackGold( 1200, 1450);
			//PackMagicItems( 3, 7);
		}
	public override bool AlwaysMurderer{ get{ return true; } }

		public AgentSmith( Serial serial ) : base( serial )
		{
		}


	 public override void OnMovement( Mobile m, Point3D oldLocation )
               {
         		if( m_Talked == false )
        		 {
          		 	 if ( m.InRange( this, 4 ) )
          			  {
          				m_Talked = true;
              				SayRandom( kfcsay, this );
				this.Move( GetDirectionTo( m.Location ) );
				SpamTimer t = new SpamTimer();
				t.Start();
            			}
		}
	}

	private class SpamTimer : Timer
	{
		public SpamTimer() : base( TimeSpan.FromSeconds( 8 ) )
		{
			Priority = TimerPriority.OneSecond;
		}

		protected override void OnTick()
		{
		m_Talked = false;
		}
	}

	private static void SayRandom( string[] say, Mobile m )
	{
		m.Say( say[Utility.Random( say.Length )] );
	}
	public override void Serialize( GenericWriter writer )
      {
         base.Serialize( writer );

         writer.Write( (int) 0 ); // version
      }

      public override void Deserialize( GenericReader reader )
      {
         base.Deserialize( reader );

         int version = reader.ReadInt();
      }
   }
}