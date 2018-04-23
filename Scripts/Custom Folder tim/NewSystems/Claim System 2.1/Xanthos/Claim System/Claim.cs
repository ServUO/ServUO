#region AuthorHeader
//
//  Claim System version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Misc;
using Server.Spells;
using Server.Guilds;
using Server.Mobiles;
using Server.Targeting;
using Server.Commands;
using Server.Engines.PartySystem;
using Xanthos.Utilities;

namespace Xanthos.Claim
{
	public class Claim
	{
		public static void Initialize()
		{
			if ( ClaimConfig.EnableClaim )
			{
				CommandHandlers.Register( "Claim", AccessLevel.Player, new CommandEventHandler( Claim_OnCommand ) );
				CommandHandlers.Register( "Cl", AccessLevel.Player, new CommandEventHandler( Claim_OnCommand ) );
			}
			if ( ClaimConfig.EnableGrab )
			{
				CommandHandlers.Register( "Grab", AccessLevel.Player, new CommandEventHandler( Grab_OnCommand ) );
				CommandHandlers.Register( "Gr", AccessLevel.Player, new CommandEventHandler( Grab_OnCommand ) );
			}
		}

		public enum ClaimOption
		{
			None,
			Carve,
			SetTypes,
			Error
		}

		[Usage( "Claim [-c|-t]" )]
		[Description( "Claim a corpse for Gold; -c to carve the corpse first, -t to view or change the types to Loot." )]
		public static void Claim_OnCommand( CommandEventArgs e )
		{
			ClaimOption option = GetOptions( e );

			switch ( option )
			{
				case ClaimOption.Error:
					Misc.SendCommandDetails( e.Mobile, "Claim" );
					return;

				case ClaimOption.None:
					e.Mobile.Target = new ClaimCmdTarget( option );
					e.Mobile.SendMessage( "Choose a corpse to claim for gold." );
					break;

				case ClaimOption.Carve:
					e.Mobile.Target = new ClaimCmdTarget( option );
					e.Mobile.SendMessage( "Choose a corpse to carve and claim for gold." );
					break;

				case ClaimOption.SetTypes:
					e.Mobile.Target = new SetLootBagTarget();
					e.Mobile.SendMessage( "Choose a loot bag to set the items that will be collected." );
					break;
			}
		}

		[Usage( "Grab [-t]" )]
		[Description( "Grab lootable items off of the ground and claim nearby corpses; -t to view or change the types to Loot." )]
		public static void Grab_OnCommand( CommandEventArgs e )
		{
			ClaimOption option = GetOptions( e );

			switch ( option )
			{
				case ClaimOption.Error:
					Misc.SendCommandDetails( e.Mobile, "Grab" );
					return;

				case ClaimOption.None:
					break;

				case ClaimOption.Carve:
					goto case ClaimOption.Error;

				case ClaimOption.SetTypes:
					e.Mobile.Target = new SetLootBagTarget();
					e.Mobile.SendMessage( "Choose a loot bag to set the items that will be collected." );
					break;
			}

			Mobile from = e.Mobile;

			if ( from.Alive == false )
			{
				from.PlaySound( 1069 ); //hey
				from.SendMessage( "You cannot do that while you are dead!" );
				return;
			}
			else if ( 0 != ClaimConfig.CompetitiveGrabRadius && BlockingMobilesInRange( from, ClaimConfig.GrabRadius ))
			{
				from.PlaySound( 1069 ); //hey
				from.SendMessage( "You are too close to another player to do that!" );
				return;
			}

			Container goldBag = GetGoldBag( from );
			Container silverBag = GetSilverBag( from );
			Container lootBag = GetLootBag( from );

			// Cleanup silver
			if ( ClaimConfig.AggregateSilver )
				AggregateSilver( from, silverBag );

			ArrayList items = new ArrayList();
			ArrayList corpses = new ArrayList();

			// Gather lootable corpses and items into lists
			foreach ( Item item in from.GetItemsInRange( ClaimConfig.GrabRadius ))
			{
				if ( !from.InLOS( item ) || !item.IsAccessibleTo( from ) || !(item.Movable || item is Corpse) )
					continue;

				if ( item is Corpse && CorpseIsLootable( from, item as Corpse, false ))
					corpses.Add( item );

				else if ( null != item && LootBag.TypeIsLootable( lootBag, item ) )
					items.Add( item );
			}

			// Drop all of the items into the player's bag/pack
			foreach ( Item item in items )
			{
				if ( item is Gold )
					GoldToLedger.Deposit( e.Mobile, item ); //DropGold( from, item as Gold, goldBag );
				else if ( item is Server.Factions.Silver )
					DropSilver( from, item as Server.Factions.Silver, silverBag );
				else
					DropLoot( from, item, lootBag );
			}

			// Loot and claim the corpses
			int corpsesClaimed = 0;

			foreach ( Item item in corpses )
				corpsesClaimed = ClaimCorpse( from, item as Corpse, ClaimOption.None ) ? corpsesClaimed + 1 : corpsesClaimed;

			if ( corpsesClaimed > 0 )
				from.SendMessage( "You claim {0} and recieve a reward.", corpsesClaimed == 1 ? "a corpse" : "some corpses" );
		}

		public static void Reclaim( Mobile from, ClaimOption option )
		{
			from.Target = new ClaimCmdTarget( option );
			from.SendMessage( "Choose another corpse to claim." );
		}

		private static ClaimOption GetOptions( CommandEventArgs e )
		{
			ClaimOption option = ClaimOption.None;

			if ( 1 <= e.Length )
			{
				string str = e.GetString( 0 ).ToLower();

				if ( str.Equals( "-c" ) )
					option = ClaimOption.Carve;
				else if ( str.Equals( "-t" ) )
					option = ClaimOption.SetTypes;
				else
					option = ClaimOption.Error;
			}
			return option;
		}

		public static bool ClaimCorpse( Mobile from, Corpse corpse, ClaimOption option )
		{
			if ( null == corpse || corpse.Owner == from )
				return false;

			Container goldBag = GetGoldBag( from );
			Container silverBag = GetSilverBag( from );
			Container lootBag = GetLootBag( from );

			if ( ClaimConfig.AggregateSilver )
				AggregateSilver( from, silverBag );

			if ( ClaimOption.Carve == option && !(corpse.Owner is PlayerMobile) )
				corpse.Carve( from, null );

			LootCorpse( from, corpse, option, goldBag, silverBag, lootBag );
			AwardGold( from, corpse, goldBag );
			corpse.Delete();

			return true;
		}

		public static void LootCorpse( Mobile from, Corpse corpse, ClaimOption option, Container goldBag, Container silverBag, Container lootBag )
		{
			ArrayList items = new ArrayList( corpse.Items.Count );

			foreach ( Item item in corpse.Items )
			{
				if ( item != null && LootBag.TypeIsLootable( lootBag, item ) )
					items.Add( item );
			}

			for ( int i = 0; i < items.Count; i++ )
			{
				Item item = (Item)items[ i ];

				if ( item is Gold )
					GoldToLedger.Deposit( from, item ); //DropGold( from, item as Gold, goldBag );
				else if ( item is Server.Factions.Silver )
					DropSilver( from, item as Server.Factions.Silver, silverBag );
				else
					DropLoot( from, item, lootBag );
			}
		}

		private static bool CorpseIsLootable( Mobile from, Corpse corpse, bool notify )
		{
			if ( null == corpse )
				return false;

			bool result = false;
			string notification = "";

			if ( corpse.Owner == from )
				notification = "You may not claim your own corpses.";
			else if ( corpse.Owner is PlayerMobile && !ClaimConfig.LootPlayers )
				notification = "You may not loot player corpses.";
			else
			{
				BaseCreature creature = corpse.Owner as BaseCreature;

				if ( null != creature && creature.IsBonded )
					notification = "You may not loot the corpses of bonded pets.";
				else if ( null != creature && creature.Fame <= ClaimConfig.FreelyLootableFame )
					result = true;
				else
					result = corpse.CheckLoot( from, null ) && !( corpse.IsCriminalAction( from ) );

			}

			if ( false == result && notify )
			{
				from.PlaySound( 1074 );		// no
				from.SendMessage( notification );
			}

			return result;
		}

		public static Container GetGoldBag( Mobile from )
		{
			Container goldBag = from.Backpack.FindItemByType( ClaimConfig.GoldBagType ) as Container;

			return ( null == goldBag ) ? from.Backpack : goldBag;
		}

		public static Container GetSilverBag( Mobile from  )
		{
			Container silverBag = from.Backpack.FindItemByType( ClaimConfig.SilverBagType ) as Container;

			return ( null == silverBag ) ? from.Backpack : silverBag;
		}

		public static Container GetLootBag( Mobile from )
		{
			Container lootBag = from.Backpack.FindItemByType( ClaimConfig.LootBagType ) as Container;

			return ( null == lootBag ) ? from.Backpack : lootBag;
		}

		public static void AggregateSilver( Mobile from, Container silverBag )
		{
			int amount = 0;

			Item[] silverStacks = from.Backpack.FindItemsByType( typeof(Server.Factions.Silver), false );

			if ( null != silverStacks )
			{
				for (int i = silverStacks.Length - 1; i >= 0; i--)
				{
					amount += silverStacks[ i ].Amount;
					silverStacks[ i ].Delete();
				}
				if ( amount > 0 )
					DropSilver( from, new Server.Factions.Silver( amount ), silverBag );
			}
		}

		public static void AwardGold( Mobile from, Corpse corpse, Container goldBag )
		{
			BaseCreature mob = corpse.Owner as BaseCreature;
			int mobBasis = ( mob == null ? ClaimConfig.MinimumReward : mob.Fame + Math.Abs( mob.Karma ) );
			int playerBasis = ( from.Fame + Math.Abs( from.Karma ) );
			int amount = Math.Max( (int)((mobBasis + playerBasis) / 1.5) / ClaimConfig.FameDivisor, ClaimConfig.MinimumReward );

            DropGold(from, new Gold(amount), goldBag);

		}

		public static void DropGold( Mobile from, Gold gold, Container goldBag )
		{
            /* Check to see if player has a CheckBook */
            Item item = from.Backpack.FindItemByType(typeof(CheckBook));
            CheckBook checkBook = item as CheckBook;

			GoldToLedger.Deposit( from, gold );
            // if (checkBook == null)
            // {
                // if (!goldBag.TryDropItem(from, gold, false))	// Attempt to stack it
                    // goldBag.DropItem(gold);
            // }
            // else
            // {
                // checkBook.Token += gold.Amount;
                // gold.Delete();
            // }

			from.PlaySound( 0x2E6 ); // drop gold sound
		}

		public static void DropSilver( Mobile from, Server.Factions.Silver silver, Container silverBag )
		{
			if ( !silverBag.TryDropItem( from, silver, false ) )	// Attempt to stack it
				silverBag.DropItem( silver );

			from.PlaySound( 0x2E6 ); // drop gold sound
		}

		public static void DropLoot( Mobile from, Item loot, Container lootBag )
		{
			if ( !lootBag.TryDropItem( from, loot, false ) )	// Attempt to stack it
				lootBag.DropItem( loot );

			from.PlaySound( 0x2E6 ); // drop gold sound
		}

		public static bool BlockingMobilesInRange( Mobile from, int range )
		{
			foreach ( Mobile mobile in from.GetMobilesInRange( range ) )
			{
				if ( mobile is PlayerMobile && IsBlockingMobile( from, mobile ) )
					return true;
			}
			return false;
		}


		public static bool IsBlockingMobile( Mobile looter, Mobile other )
		{
			// Self, invisible players and any staff don't count
			if ( looter == other || other.Hidden || other.AccessLevel > AccessLevel.Player )
				return false;

			Guild looterGuild = SpellHelper.GetGuildFor( looter );
			Guild otherGuild = SpellHelper.GetGuildFor( other );

			if ( null != looterGuild && null != otherGuild && ( looterGuild == otherGuild || looterGuild.IsAlly( otherGuild ) ) )
				return false;

			Party party = Party.Get( looter );

			return !( null != party && party.Contains( other ) );
		}

		private class SetLootBagTarget : Target
		{
			public SetLootBagTarget() : base( -1, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object target )
			{
				LootBag bag = target as Xanthos.Claim.LootBag;

				if ( bag == null )
					from.SendMessage( "That is not a Loot Bag." );

				else if ( !bag.IsChildOf( from.Backpack ) )
					from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.

				else
					from.SendGump( new LootTypesGump( from, bag ) );
			}
		}

		private class ClaimCmdTarget : Target
		{
			ClaimOption m_Option;
			public ClaimCmdTarget( ClaimOption option ) : base( ClaimConfig.ClaimRadius, false, TargetFlags.None )
			{
				m_Option = option;
			}

			protected override void OnTarget( Mobile from, object target )
			{
				Corpse corpse = target as Corpse;

				if ( from.Alive == false )
				{
					from.PlaySound( 1069 ); //hey
					from.SendMessage( "You cannot do that while you are dead!" );
				}
				else if ( 0 != ClaimConfig.CompetitiveClaimRadius && BlockingMobilesInRange( from, ClaimConfig.CompetitiveClaimRadius ) )
				{
					from.PlaySound( 1069 ); //hey
					from.SendMessage( "You are too close to another player to do that!" );
				}
				else if ( null == corpse )
				{
					from.PlaySound( 1066 ); // giggle
					from.SendMessage( "That isn't a corpse!" );
				}
				else if ( !from.InLOS( corpse ) || !corpse.IsAccessibleTo( from ) )
				{
					from.PlaySound( 1069 ); //hey
					from.SendMessage( "You are unable to reach that!" );
				}
				else if ( CorpseIsLootable( from, corpse, true ) )
				{
					if ( Claim.ClaimCorpse( from, corpse, m_Option ) )
						from.SendMessage( "You claim the corpse and recieve a reward." );

					Reclaim( from, m_Option );
				}
			}
		}
	}
}