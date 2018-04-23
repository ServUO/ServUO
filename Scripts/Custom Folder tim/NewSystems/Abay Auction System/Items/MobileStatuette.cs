#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using Server;
using Server.Mobiles;
using Xanthos.Utilities;
using Xanthos.Interfaces;

namespace Arya.Abay
{
	/// <summary>
	/// This item holds information about a BaseCreature sold through an Abay system
	/// </summary>
	public class MobileStatuette : Item, IShrinkItem
	{
		private BaseCreature m_Creature;

		/// <summary>
		/// Gets the creature sold through this 
		/// </summary>
		public BaseCreature ShrunkenPet
		{
			get
			{
				if ( m_Creature == null )
					return null;
				else
					return m_Creature.Deleted ? null : m_Creature;
			}
		}

		/// <summary>
		/// Creates a new MobileStatuette object - for internal use only
		/// </summary>
		private MobileStatuette( BaseCreature creature )
		{
			m_Creature = creature;
			ItemID = ShrinkTable.Lookup( m_Creature );
			Hue = m_Creature.Hue;

			m_Creature.ControlTarget = null;
			m_Creature.ControlOrder = OrderType.Stay;
			m_Creature.Internalize();
			m_Creature.SetControlMaster( null );
			m_Creature.SummonMaster = null;
			m_Creature.IsStabled = true;

			// Set the type of the creature as the name for this item
			Name = Xanthos.Utilities.Misc.GetFriendlyClassName( creature.GetType().Name );
		}

		#region Serialization

		public MobileStatuette( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize (writer);

			writer.Write( 0 ); // Version

			writer.Write( m_Creature );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize (reader);

			int version = reader.ReadInt();

			m_Creature = reader.ReadMobile() as BaseCreature;
		}

		#endregion

		/// <summary>
		/// Converta a given BaseCreature object into a statuette object which can be sold through the Abay
		/// system. Will provide feedback to the user in case some of the requirements aren't met.
		/// </summary>
		/// <param name="from">The player Abaying the creature</param>
		/// <param name="creatue">The creature being Abayed</param>
		/// <returns>A MobileStatuette if the creature can be Abayed, null if the process fails</returns>
		public static MobileStatuette Create( Mobile from, BaseCreature creature )
		{
			if ( !creature.Controlled || creature.ControlMaster != from )
			{
				from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 139 ] );
				return null;
			}

			if ( creature.IsAnimatedDead || creature.IsDeadPet )
			{
				from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 140 ] );
				return null;
			}

			if ( creature.Summoned )
			{
				from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 141 ] );
				return null;
			}

			if ( creature is BaseFamiliar )
			{
				from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 142 ] );
				return null;
			}

			if ( (creature is PackLlama || creature is PackHorse || creature is Beetle) && (creature.Backpack != null && creature.Backpack.Items.Count > 0) )
			{
				from.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 143 ] );
				return null;
			}

			return new MobileStatuette( creature );
		}

		/// <summary>
		/// Brings the creature to a player and deletes the statuette
		/// </summary>
		/// <param name="m">The player the creature should be given to</param>
		public void GiveCreatureTo( Mobile m )
		{
			m_Creature.SetControlMaster( m );
			m_Creature.MoveToWorld( m.Location, m.Map );

			m_Creature = null;
			Delete();
		}

		/// <summary>
		/// This will stable the pet inside a player's stable and delete the statuette.
		/// This function will also clear the control slots needed for the new master to control the pet
		/// as they have been assigned at bid time
		/// </summary>
		/// <param name="m">The player claiming the pet</param>
		/// <returns>True if succesful, false if player can't add to stable</returns>
		public bool Stable( Mobile m )
		{
			if ( m_Creature == null )
			{
				m.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 144 ] );
				Delete();
				return true;
			}

			if ( m.Stabled.Count > AnimalTrainer.GetMaxStabled( m ) )
			{
				m.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 178 ] );
				return false;
			}

			m_Creature.ControlMaster = m;

			m_Creature.ControlTarget = null;
			m_Creature.ControlOrder = OrderType.Stay;
			m_Creature.Internalize();

			m_Creature.SetControlMaster( null );
			m_Creature.SummonMaster = null;
			m_Creature.IsBonded = false;

			m_Creature.IsStabled = true;
			m.Stabled.Add( m_Creature );

			m_Creature = null;
			Delete();

			return true;
		}

		public void ForceDelete()
		{
			if ( null != m_Creature )
				m_Creature.Delete();

			Delete();
		}
	}
}