using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
	public class PetRezStone : Item
	{
		private int m_CostToRez;

		[CommandProperty( AccessLevel.GameMaster )]
		public int CostToRez
		{
			get{ return m_CostToRez; }
			set{ m_CostToRez = value; this.InvalidateProperties(); }
		}

		[Constructable]
		public PetRezStone() : base( 4484 )
		{
			Name = "A Pet Resurrection Stone";
			Movable = false;
			Hue = 781;
		}

		public PetRezStone( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if (m_CostToRez > 0)
			{
				list.Add("Price to Resurrect: " + m_CostToRez.ToString("N0") + " gold.");
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( this.GetWorldLocation(), 1 ) )
				from.SendLocalizedMessage( 502138 );
			else
				from.SendGump( new PetRezStoneGump((PlayerMobile)from, this, m_CostToRez) );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_CostToRez );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch(version)
			{
				case 0:
				{
					m_CostToRez = reader.ReadInt();
					break;
				}
			}
		}

		public void RezAPet( Mobile f, object t )
		{
			if ( !(t is BaseCreature) )
			{
				f.SendMessage("That's not a pet!");
				return;
			}

			BaseCreature c = t as BaseCreature;

			if ( c == null )
				return;

			if ( !c.Controlled )
			{
				f.SendMessage("That's not a pet!");
			}
			else if ( c.ControlMaster != f )
			{
				f.SendMessage("That's not your pet!");
			}
			else if ( !c.IsDeadPet )
			{
				f.SendMessage("That pet is already alive!");
			}
			else if ( !c.InRange( this.GetWorldLocation(), 2 ) )
			{
				f.SendMessage("Your pet need to be closer to the stone for the magic to work.");
			}
			else if ( c.Map == null || !c.Map.CanFit( c.Location, 16, false, false ) )
			{
				f.SendLocalizedMessage( 503256 ); // You fail to resurrect the creature.
			}
			else if ( ChargePlayer( (PlayerMobile)f ) )
			{
				c.PlaySound( 0x214 );
				c.FixedEffect( 0x376A, 10, 16 );
				c.ResurrectPet();

				//for ( int i = 0; i < c.Skills.Length; ++i )	//Decrease all skills on pet.
					//c.Skills[i].Base -= 0.2;
			}
		}

		private bool ChargePlayer( PlayerMobile p )
		{
			if ( m_CostToRez > 0 )
			{
				if ( Banker.Withdraw( p, m_CostToRez ) )
				{
					p.SendLocalizedMessage( 1060398, m_CostToRez.ToString("N0") ); // Amount charged
					p.SendLocalizedMessage( 1060022, Banker.GetBalance( p ).ToString("N0") ); // Amount left, from bank
				}
				else
				{
					p.SendMessage("Unfortunately, you don't have enough gold in the bank to resurrect your pet.");
					return false;
				}
			}
			return true;
		}
	}

	public class PetRezStoneTarget : Target
	{
		private PetRezStone m_Stone;

		public PetRezStoneTarget( PetRezStone stone ) : base( 18, false, TargetFlags.None )
		{
			m_Stone = stone;
		}

		protected override void OnTarget( Mobile f, object t )
		{
			m_Stone.RezAPet( f, t );
		}
	}
}

namespace Server.Gumps
{
	public class PetRezStoneGump : Gump
	{
		private PetRezStone m_Stone;
		private PlayerMobile m_From;

		public PetRezStoneGump( PlayerMobile from, PetRezStone stone, int cost )
			: base( 75, 75 )
		{
			m_Stone = stone;
			m_From = from;
			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddImage(60, 125, 30501);
			this.AddImage(169, 129, 71);
			if (cost > 0)
			{
				this.AddHtml( 190, 143, 134, 77, @"<basefont color=white>The magic of this stone can return your pet to the living for a price.", (bool)false, (bool)false);
				string costString = string.Format("<basefont color=white>Cost: <basefont color=#FFFFAA>{0} gold<basefont color=white>", cost.ToString("N0"));
				this.AddHtml( 189, 234, 133, 21, costString, (bool)false, (bool)false);
			}
			else
				this.AddHtml( 190, 143, 134, 77, @"<basefont color=white>The magic of this stone can return your pet to the living.", (bool)false, (bool)false);

			this.AddButton(186, 264, 247, 248, (int)Buttons.Okay, GumpButtonType.Reply, 0);
			this.AddButton(260, 264, 242, 241, (int)Buttons.Cancel, GumpButtonType.Reply, 0);

		}
		
		public enum Buttons
		{
			Cancel,
			Okay
		}

		public override void OnResponse( NetState ns, RelayInfo info )
		{
			switch( info.ButtonID )
			{
				case (int)Buttons.Okay:
				{
					m_From.SendMessage("Target the pet you wish to resurrect.");
					m_From.Target = new PetRezStoneTarget( m_Stone );
					break;
				}
				case (int)Buttons.Cancel:
				{
					m_From.SendMessage("You decide not to resurrect a pet.");
					break;
				}
			}
		}
	}
}
