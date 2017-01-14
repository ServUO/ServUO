using System;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
	public class LevelItemManager
	{
		/// <summary>
		/// The Number of levels our items can go to. If you
		/// change this, be sure the Exp table has the correct
		/// number of Integer values in it.
		/// </summary>

		#region Level calculation method

		private static int[] m_Table;

		public static int[] ExpTable
		{
			get{ return m_Table; }
		}

		public static void Initialize()
		{
			// The following will build the Level experence table */
			m_Table = new int[LevelItems.MaxLevelsCap];
			m_Table[0] = 0;

			for ( int i = 1; i < LevelItems.MaxLevelsCap; ++i )
			{
				m_Table[i] = ExpToLevel( i );
			}
		}

		public static int ExpToLevel( int currentlevel )
		{
			double req = ( currentlevel + 1 ) * 10;

			req = Math.Pow( req, 2 );

			req -= 100.0;

			return ( (int)Math.Round( req ) );
		}

		#endregion

		#region Exp calculation methods

		private static bool IsMageryCreature( BaseCreature bc )
		{
			return ( bc != null && bc.AI == AIType.AI_Mage && bc.Skills[SkillName.Magery].Base > 5.0 );
		}

		private static bool IsFireBreathingCreature( BaseCreature bc )
		{
			if ( bc == null )
				return false;

			return bc.HasBreath;
		}

		private static bool IsPoisonImmune( BaseCreature bc )
		{
			return ( bc != null && bc.PoisonImmune != null );
		}

		private static int GetPoisonLevel( BaseCreature bc )
		{
			if ( bc == null )
				return 0;

			Poison p = bc.HitPoison;

			if ( p == null )
				return 0;

			return p.RealLevel + 1;
		}

		public static int CalcExp( Mobile targ )
		{
			double val = targ.Hits + targ.Stam + targ.Mana;

			for ( int i = 0; i < targ.Skills.Length; i++ )
				val += targ.Skills[i].Base;

			if ( val > 700 )
				val = 700 + ((val - 700) / 3.66667);

			BaseCreature bc = targ as BaseCreature;

			if ( IsMageryCreature( bc ) )
				val += 100;

			if ( IsFireBreathingCreature( bc ) )
				val += 100;

			if ( IsPoisonImmune( bc ) )
				val += 100;

			if ( targ is VampireBat || targ is VampireBatFamiliar )
				val += 100;

			val += GetPoisonLevel( bc ) * 20;

			val /= 10;

			return (int)val;
		}

		public static int CalcExpCap( int level )
		{
			int req = ExpToLevel( level + 1 );

			return ( req / 20 );
		}

		#endregion

		public static void CheckItems( Mobile killer, Mobile killed )
		{
			if ( killer != null )
			{
				for( int i = 0; i < 25; ++i )
				{
					Item item = killer.FindItemOnLayer( (Layer)i );

                    XmlLevelItem levitem = XmlAttach.FindAttachment(item, typeof(XmlLevelItem)) as XmlLevelItem;

					//if ( item != null && item is ILevelable )
                    if (item != null && levitem != null)
                        CheckLevelable(levitem, killer, killed);
				}
			}
		}

		//public static void InvalidateLevel( ILevelable item )
        public static void InvalidateLevel(XmlLevelItem item)
		{
			for( int i = 0; i < ExpTable.Length; ++i )
			{
				if ( item.Experience < ExpTable[i] )
					return;

				item.Level = i + 1;
			}
		}

		//public static void CheckLevelable( ILevelable item, Mobile killer, Mobile killed )
        public static void CheckLevelable(XmlLevelItem item, Mobile killer, Mobile killed)
		{
			if ( (item.Level >= LevelItems.MaxLevelsCap) || (item.Level >= item.MaxLevel) )
				return;

			int exp = CalcExp( killed );
			int oldLevel = item.Level;
			int expcap = CalcExpCap( oldLevel );

			if ( LevelItems.EnableExpCap && exp > expcap )
				exp = expcap;

			item.Experience += exp;

			InvalidateLevel( item );

			if ( item.Level != oldLevel )
				OnLevel( item, oldLevel, item.Level, killer );

            //if ( item is Item )
            //    ((Item)item).InvalidateProperties();
            if (item != null)
                item.InvalidateParentProperties();
		}

        //public static void OnLevel(ILevelable item, int oldLevel, int newLevel, Mobile from)
        public static void OnLevel(XmlLevelItem item, int oldLevel, int newLevel, Mobile from)
        {
            /* This is where we control all our props
             * and their maximum value. */
            int index;
            string itemdesc;

            index = newLevel % 10;
            if (index == 0)
            {
                item.Points += LevelItems.PointsPerLevel*2;
            }
            else
            {
                item.Points += LevelItems.PointsPerLevel;
            }

			from.PlaySound( 0x20F );
			from.FixedParticles( 0x376A, 1, 31, 9961, 1160, 0, EffectLayer.Waist );
			from.FixedParticles( 0x37C4, 1, 31, 9502, 43, 2, EffectLayer.Waist );

			if ( item.AttachedTo is BaseWeapon )
				itemdesc = "weapon";
			else if ( item.AttachedTo is BaseArmor )
				itemdesc = "armor";
			else if (item.AttachedTo is BaseJewel )
				itemdesc = "jewelry";
			else if (item.AttachedTo is BaseClothing )
				itemdesc = "clothing";
			else
				itemdesc = "item";

			from.SendMessage( "Your "+itemdesc+" has gained a level. It is now level {0}.", newLevel );
        }
	}
}
