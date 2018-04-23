// Created by AncientTimes
using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.ContextMenus;

namespace Server.Mobiles
{
	[CorpseName( "a battle polarbear corpse" )]
	public class BattlePolarBear : BaseMount
	{
        public override bool InitialInnocent { get { return true; } }
        public override bool DeleteOnRelease { get { return true; } }
        private static int[] m_IDs = new int[]
			{
				0x3EC5,
			};
			
		[Constructable]
		public BattlePolarBear() : this( "A Battle PolarBear" )
		{
		}

		public override bool SubdueBeforeTame{ get{ return false; } } // Must be beaten into submission

		[Constructable]
		public BattlePolarBear( string name ) : base( name, 0x317, 0x3EC5, AIType.AI_Melee, FightMode.Good, 10, 1, 0.2, 0.4 )
		{
            Name = "A Battle PolarBear";
            Hue = 1153;
			Body = 213;
			BaseSoundID = 0xA3;
			SetStr( 1500 );
			SetDex( 600 );
			SetInt( 500 );

			SetHits( 2000 );
            SetStam( 600 );

			SetDamage( 22, 30 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 75 );
			SetResistance( ResistanceType.Fire, 75 );
			SetResistance( ResistanceType.Cold, 75 );
			SetResistance( ResistanceType.Poison, 75 );
			SetResistance( ResistanceType.Energy, 75 );

			SetSkill( SkillName.MagicResist, 110.0 );
			SetSkill( SkillName.Tactics, 110.0 );
			SetSkill( SkillName.Wrestling, 110.0 );

			Fame = 0;
			Karma = 1000;

			Tamable = true;
			ControlSlots = 4;
			MinTameSkill = 0;

			Container pack = Backpack;

			if ( pack != null )
				pack.Delete();

			pack = new StrongBackpack();
			pack.Movable = false;

			AddItem( pack );
		}

		

		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }

		// TODO: Speed boost when hit by magic.

        public BattlePolarBear(Serial serial)
            : base(serial)
		{
		}

		#region Pack Animal Methods
		public override bool OnBeforeDeath()
		{
			if ( !base.OnBeforeDeath() )
				return false;

			PackAnimal.CombineBackpacks( this );

			return true;
		}

		public override DeathMoveResult GetInventoryMoveResultFor( Item item )
		{
			return DeathMoveResult.MoveToCorpse;
		}

		public override bool IsSnoop( Mobile from )
		{
			if ( PackAnimal.CheckAccess( this, from ) )
				return false;

			return base.IsSnoop( from );
		}

		public override bool OnDragDrop( Mobile from, Item item )
		{
			if ( CheckFeed( from, item ) )
				return true;

			if ( PackAnimal.CheckAccess( this, from ) )
			{
				AddToBackpack( item );
				return true;
			}

			return base.OnDragDrop( from, item );
		}

		public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
		{
			return PackAnimal.CheckAccess( this, from );
		}

		public override bool CheckNonlocalLift( Mobile from, Item item )
		{
			return PackAnimal.CheckAccess( this, from );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			PackAnimal.GetContextMenuEntries( this, from, list );
		}
		#endregion

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
