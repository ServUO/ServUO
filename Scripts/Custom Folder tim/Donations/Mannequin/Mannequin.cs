using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Gumps;

namespace Server.Mobiles
{
	public class Mannequin : BaseCreature
	{
		public override bool ClickTitle{ get{ return false; } }
		public override bool NoHouseRestrictions{ get{ return true; } }
		public override bool AllowEquipFrom( Mobile from ){ return m_Owner == from; }
		public override bool CheckNonlocalLift( Mobile from, Item item ){ return m_Owner == from; }
		public override bool CanBeDamaged(){ return false; }
		public override bool CanBeRenamedBy( Mobile from ){ return m_Owner == from; }
		public override bool CanPaperdollBeOpenedBy(Mobile from){ return true; }

		public Mobile m_Owner;
		public Direction m_Direction;
		public bool m_Listen;

		public ArrayList m_WayPoints;
		public Point3D m_NextMove = Point3D.Zero;

		[Constructable]
		public Mannequin( Mobile owner, bool female ) : base( AIType.AI_Use_Default, FightMode.None, 1, 1, 0.2, 0.2 )
		{
			m_WayPoints = new ArrayList();
			m_Owner = owner;
			Name = "Adam";
			Title = "";
			NameHue = 1150;
			if( female )
				Body = 401;
			else
				Body = 400;

			CantWalk = true;
			Direction = Direction.South;

			SetStr( 100 );
			SetDex( 100 );
			SetInt( 100 );

			SetDamage( 1 );

			Fame = 0;
			Karma = 0;
		}

		public override void GenerateLoot()
		{
		}

		public Mannequin( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			writer.Write( m_Owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_Owner = reader.ReadMobile();
			m_Direction = Direction;
			m_WayPoints = new ArrayList();
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			if( from.InRange( this.Location, 10 ) )
				return true;
			return base.HandlesOnSpeech( from );
		}

		public override void OnSpeech( SpeechEventArgs args )
		{
			string said = args.Speech.ToLower();
			Mobile from = args.Mobile;

			if( said == this.Name.ToLower() )
			{
				if( from == m_Owner )
				{
					this.Say( "Yes?" );
					m_Listen = true;
					from.SendGump( new MannequinControl( this, from, 0 ) );
				}

				else
				{
					int say = Utility.Random( 6 );
					switch( say )
					{
						case 0:
						{
							this.Say( "No." );
							CantWalk = true;
							break;
						}
						case 1:
						{
							this.Say( "Do I know you?" );
							CantWalk = true;
							break;
						}
						case 2:
						{
							this.Say( "I don't listen to strangers" );
							CantWalk = true;
							break;
						}
						case 3:
						{
							this.Say( "Bugger off!" );
							CantWalk = true;
							break;
						}
						case 4:
						{
							this.Say( "You do not own me!" );
							CantWalk = true;
							break;
						}
						case 5:
						{
							this.Say( "You are not my master." );
							CantWalk = true;
							break;
						}
						case 6:
						{
							this.Say( "Let me think about that........No!" );
							CantWalk = true;
							break;
						}
					}
				}
			}

			if( said == "you're fired" && m_Listen && from == m_Owner )
			{
				this.Say( "I'm sorry I could not live up to your expectations..." );
				Item OnLayer = this.FindItemOnLayer( Layer.OneHanded );
				if( OnLayer != null )
					from.AddToBackpack( this.FindItemOnLayer( Layer.OneHanded ) );
				OnLayer = this.FindItemOnLayer( Layer.TwoHanded );
				if( OnLayer != null )
					from.AddToBackpack( this.FindItemOnLayer( Layer.TwoHanded ) );
				OnLayer = this.FindItemOnLayer( Layer.Shoes );
				if( OnLayer != null )
					from.AddToBackpack( this.FindItemOnLayer( Layer.Shoes ) );
				OnLayer = this.FindItemOnLayer( Layer.Shirt);
				if( OnLayer != null )
					from.AddToBackpack( this.FindItemOnLayer( Layer.Shirt ) );
				OnLayer = this.FindItemOnLayer( Layer.Helm);
				if( OnLayer != null )
					from.AddToBackpack( this.FindItemOnLayer( Layer.Helm ) );
				OnLayer = this.FindItemOnLayer( Layer.Gloves);
				if( OnLayer != null )
					from.AddToBackpack( this.FindItemOnLayer( Layer.Gloves ) );
				OnLayer = this.FindItemOnLayer( Layer.Neck);
				if( OnLayer != null )
					from.AddToBackpack( this.FindItemOnLayer( Layer.Neck ) );
				OnLayer = this.FindItemOnLayer( Layer.Waist);
				if( OnLayer != null )
					from.AddToBackpack( this.FindItemOnLayer( Layer.Waist ) );
				OnLayer = this.FindItemOnLayer( Layer.InnerTorso);
				if( OnLayer != null )
					from.AddToBackpack( this.FindItemOnLayer( Layer.InnerTorso ) );
				OnLayer = this.FindItemOnLayer( Layer.MiddleTorso);
				if( OnLayer != null )
					from.AddToBackpack( this.FindItemOnLayer( Layer.MiddleTorso ) );
				OnLayer = this.FindItemOnLayer( Layer.Arms);
				if( OnLayer != null )
					from.AddToBackpack( this.FindItemOnLayer( Layer.Arms ) );
				OnLayer = this.FindItemOnLayer( Layer.Cloak);
				if( OnLayer != null )
					from.AddToBackpack( this.FindItemOnLayer( Layer.Cloak ) );
				OnLayer = this.FindItemOnLayer( Layer.OuterTorso);
				if( OnLayer != null )
					from.AddToBackpack( this.FindItemOnLayer( Layer.OuterTorso ) );
				OnLayer = this.FindItemOnLayer( Layer.OuterLegs );
				if( OnLayer != null )
					from.AddToBackpack( this.FindItemOnLayer( Layer.OuterLegs ) );
				if( Body == 400 )
				{
					from.AddToBackpack( new MaleMannequinDeed() );
				}
				else
				{
					from.AddToBackpack( new FemaleMannequinDeed() );
				}
				this.Delete();
			}
		}

		protected override void OnLocationChange(Point3D oldLocation)
		{
			if ( m_NextMove == Point3D.Zero || m_NextMove != Location )
				return;

			// The NPC is at the waypoint
			AI = AIType.AI_Use_Default;

			CurrentWayPoint = null;
			CantWalk = true;

			foreach( WayPoint wp in m_WayPoints )
				wp.Delete();

			m_WayPoints.Clear();

			m_NextMove = Point3D.Zero;

			Direction = m_Direction;

			Server.Timer.DelayCall( TimeSpan.FromMilliseconds( 500 ), TimeSpan.FromMilliseconds( 500 ), 1, new TimerStateCallback ( OnFacingTimer ), null );
		}

		private void OnFacingTimer( object state )
		{
			Direction = m_Direction;
		}
	}
}
