using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Mystic;

namespace Server.Gumps
{
	public class SpellTriggerGump : Gump
	{
        private Mobile m_From;
        private SpellTriggerSpell m_Spell;

		public SpellTriggerGump( Mobile m, SpellTriggerSpell spell ) : base( 0, 0 )
		{
            m_From = m;
            m_Spell = spell;
            int font = 0x7FFF;

			int skill = (int)m.Skills[SkillName.Mysticism].Value;

			AddPage( 0 );
			AddBackground( 0, 0, 170, 400, 9270 );
			AddAlphaRegion( 10, 10, 150, 380 );

			// Listing them in skill order would have been easier, all well.

            if (skill >= 33 && HasSpell(683)) // Animated Weapon
            {
                AddHtmlLocalized(40, 15, 200, 16, 1022313, font, false, false);
                AddButton(15, 15, 9702, 9703, 683, GumpButtonType.Reply, 0);
            }
            else
            {
                AddHtmlLocalized(40, 15, 200, 16, 1022313, false, false);
                AddImage(15, 15, 9702, 995);
            }

            if (skill >= 58 && HasSpell(688))  // Bombard
			{
				AddHtmlLocalized( 40, 40, 200, 16, 1031689, font, false, false );
				AddButton( 15, 40, 9702, 9703, 688, GumpButtonType.Reply, 0 );
			}
			else
			{
                AddHtmlLocalized(40, 40, 200, 16, 1031689, false, false);
				AddImage( 15, 40, 9702, 995 );
			}

            if (skill >= 58 && HasSpell(687)) // Cleansing Winds
			{
				AddHtmlLocalized( 40, 65, 200, 16, 1031688, font, false, false );
				AddButton( 15, 65, 9702, 9703, 687, GumpButtonType.Reply, 0 );
			}
			else
			{
                AddHtmlLocalized(40, 65, 200, 16, 1031688, false, false);
				AddImage( 15, 65, 9702, 995 );
			}

            if (skill >= 20 && HasSpell(682)) // Eagle Strike
            {
                AddHtmlLocalized(40, 90, 200, 16, 1031683, font, false, false);
                AddButton(15, 90, 9702, 9703, 682, GumpButtonType.Reply, 0); // Eagle Strike
            }
            else
            {
                AddHtmlLocalized(40, 90, 200, 16, 1031683, false, false);
                AddImage(15, 90, 9702, 995);
            }

            if (skill >= 8 && HasSpell(680)) // Enchant
            {
                AddHtmlLocalized(40, 115, 200, 16, 1031681, font, false, false);
                AddButton(15, 115, 9702, 9703, 680, GumpButtonType.Reply, 0);
            }
            else
            {
                AddHtmlLocalized(40, 115, 200, 16, 1031681, false, false);
                AddImage(15, 115, 9702, 995);
            }

            if (skill >= 70 && HasSpell(690))// Hail Storm
			{
                AddHtmlLocalized(40, 140, 200, 16, 1031691, font, false, false);
				AddButton( 15, 140, 9702, 9703, 690, GumpButtonType.Reply, 0 ); 
			}
			else
			{
                AddHtmlLocalized(40, 140, 200, 16, 1031691, false, false);
				AddImage( 15, 140, 9702, 995 );
			}

            if (HasSpell(678))
            {
                AddHtmlLocalized(40, 165, 200, 16, 1031679, font, false, false);// Healing Stone
                AddButton(15, 165, 9702, 9703, 678, GumpButtonType.Reply, 0);
            }
            else
            {
                AddHtmlLocalized(40, 165, 200, 16, 1031679, false, false);
                AddImage(15, 165, 9702, 995);
            }

            if (skill >= 45 && HasSpell(686)) // Mass Sleep
            {
                AddHtmlLocalized(40, 190, 200, 16, 1031687, font, false, false);
                AddButton(15, 190, 9702, 9703, 686, GumpButtonType.Reply, 0);
            }
            else
            {
                AddHtmlLocalized(40, 190, 200, 16, 1031687, false, false);
                AddImage(15, 190, 9702, 995);
            }

            if (HasSpell(677)) // Nether Bolt
            {
                AddHtmlLocalized(40, 215, 200, 16, 1031678, font, false, false);
                AddButton(15, 215, 9702, 9703, 677, GumpButtonType.Reply, 0);
            }
            else
            {
                AddHtmlLocalized(40, 215, 200, 16, 1031678, false, false);
                AddImage(15, 215, 9702, 995);
            }

            if (skill >= 83 && HasSpell(691)) // Nether Cyclone
			{
                AddHtmlLocalized(40, 240, 200, 16, 1031692, font, false, false);
				AddButton( 15, 240, 9702, 9703, 691, GumpButtonType.Reply, 0 );
			}
			else
			{
                AddHtmlLocalized(40, 240, 200, 16, 1031692, false, false);
                AddImage(15, 240, 9702, 995);
			}

            if (skill >= 8 && HasSpell(679)) // Purge Magic
            {
                AddHtmlLocalized(40, 265, 200, 16, 1031680, font, false, false);
                AddButton(15, 265, 9702, 9703, 679, GumpButtonType.Reply, 0);
            }
            else
            {
                AddHtmlLocalized(40, 265, 200, 16, 1031680, false, false);
                AddImage(15, 265, 9702, 995);
            }

            if (skill >= 83 && HasSpell(692)) // Rising Colossus
			{
                AddHtmlLocalized(40, 290, 200, 16, 1031693, font, false, false);
				AddButton( 15, 290, 9702, 9703, 692, GumpButtonType.Reply, 0 );
			}
			else
			{
                AddHtmlLocalized(40, 290, 200, 16, 1031693, false, false);
                AddImage(15, 290, 9702, 995);
			}

            if (skill >= 20 && HasSpell(681)) // Sleep
            {
                AddHtmlLocalized(40, 315, 200, 16, 1031682, font, false, false);
                AddButton(15, 315, 9702, 9703, 681, GumpButtonType.Reply, 0);
            }
            else
            {
                AddHtmlLocalized(40, 315, 200, 16, 1031682, false, false);
                AddImage(15, 315, 9702, 995);
            }

            if (skill >= 70 && HasSpell(689)) // Spell Plague
			{
                AddHtmlLocalized(40, 340, 200, 16, 1031690, font, false, false);
				AddButton( 15, 340, 9702, 9703, 689, GumpButtonType.Reply, 0 ); // Spell Plague
			}
			else
			{
                AddHtmlLocalized(40, 340, 200, 16, 1031690, false, false);
                AddImage(15, 340, 9702, 995);
			}

            if (skill >= 33 && HasSpell(684))
            {
                AddHtmlLocalized(40, 365, 200, 16, 1031685, font, false, false);
                AddButton(15, 365, 9702, 9703, 684, GumpButtonType.Reply, 0); // Stone Form
            }
            else
            {
                AddHtmlLocalized(40, 365, 200, 16, 1031685, false, false);
                AddImage(15, 365, 9702, 995);
            }
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

            if (info.ButtonID == 0)
                return;

			if ( !(info.ButtonID >= 677 && info.ButtonID <= 692) )
				from.SendMessage( "There was an error in your spell choice, please try again or page if you have." );
			else if ( from.Backpack != null && HasSpell(info.ButtonID) && m_Spell.CheckSequence())
			{
				Item[] stones = from.Backpack.FindItemsByType( typeof( SpellStone ) );

				for ( int i = 0; i < stones.Length; i++ )
					stones[i].Delete();

				from.PlaySound( 0x659 );
				from.Backpack.DropItem( new SpellStone( from, info.ButtonID ) );
				from.SendLocalizedMessage( 1080165 ); // A Spell Stone appears in your backpack
			}

            m_Spell.FinishSequence();
		}

        public bool HasSpell(int id)
        {
            Spellbook book = Spellbook.Find(m_From, id);
            return book != null && book.HasSpell(id);
        }
	}
}