//ssusing System;
using Server.Gumps;

namespace Server.Items
{
	public class SkillGate : Item
	{
		[Constructable]
		public SkillGate() : base( 0xF6C )
		{
			Movable = false;
			Name = "-Skills 100-";
		}

		public SkillGate( Serial serial ) : base( serial )
		{
		}

		public override bool OnMoveOver( Mobile m )
		{
			if (m.Map.CanFit( m.Location, 16, false, false ) )
			{
				m.Skills[SkillName.Alchemy].Base = 100;
				m.Skills[SkillName.Anatomy].Base = 100;
				m.Skills[SkillName.AnimalLore].Base = 100;
				m.Skills[SkillName.AnimalTaming].Base = 100;
				m.Skills[SkillName.Archery].Base = 100;
                		m.Skills[SkillName.ArmsLore].Base = 100;
                		m.Skills[SkillName.Begging].Base = 100;
				m.Skills[SkillName.Blacksmith].Base = 100;
				m.Skills[SkillName.Camping].Base = 100;
				m.Skills[SkillName.Carpentry].Base = 100;
				m.Skills[SkillName.Cartography].Base = 100;
				m.Skills[SkillName.Cooking].Base = 100;
				m.Skills[SkillName.DetectHidden].Base = 100;
				m.Skills[SkillName.Discordance].Base = 100;
				m.Skills[SkillName.EvalInt].Base = 100;
				m.Skills[SkillName.Fishing].Base = 100;
				m.Skills[SkillName.Fencing].Base = 100;
				m.Skills[SkillName.Fletching].Base = 100;
				m.Skills[SkillName.Focus].Base = 100;
				m.Skills[SkillName.Forensics].Base = 100;
				m.Skills[SkillName.Healing].Base = 100;
				m.Skills[SkillName.Herding].Base = 100;
				m.Skills[SkillName.Hiding].Base = 100;
				m.Skills[SkillName.Inscribe].Base = 100;
				m.Skills[SkillName.ItemID].Base = 100;
				m.Skills[SkillName.Lockpicking].Base = 100;
				m.Skills[SkillName.Lumberjacking].Base = 100;
				m.Skills[SkillName.Macing].Base = 100;
				m.Skills[SkillName.Magery].Base = 100;
				m.Skills[SkillName.MagicResist].Base = 100;
				m.Skills[SkillName.Meditation].Base = 100;
				m.Skills[SkillName.Mining].Base = 100;
				m.Skills[SkillName.Musicianship].Base = 100;
				m.Skills[SkillName.Parry].Base = 100;
				m.Skills[SkillName.Peacemaking].Base = 100;
				m.Skills[SkillName.Poisoning].Base = 100;
				m.Skills[SkillName.Provocation].Base = 100;
				m.Skills[SkillName.RemoveTrap].Base = 100;
				m.Skills[SkillName.Snooping].Base = 100;
				m.Skills[SkillName.SpiritSpeak].Base = 100;
				m.Skills[SkillName.Stealing].Base = 100;
				m.Skills[SkillName.Stealth].Base = 100;
				m.Skills[SkillName.Swords].Base = 100;
				m.Skills[SkillName.Tactics].Base = 100;
				m.Skills[SkillName.Tailoring].Base = 100;
				m.Skills[SkillName.TasteID].Base = 100;
				m.Skills[SkillName.Tinkering].Base = 100;
				m.Skills[SkillName.Tracking].Base = 100;
				m.Skills[SkillName.Veterinary].Base = 100;
				m.Skills[SkillName.Wrestling].Base = 100;
				m.Skills[SkillName.Bushido].Base = 100;
				m.Skills[SkillName.Ninjitsu].Base = 100;
				m.Skills[SkillName.Chivalry].Base = 100;
				m.Skills[SkillName.Necromancy].Base = 100;
				m.Skills[SkillName.Spellweaving].Base = 100;
				m.Skills[SkillName.Mysticism].Base = 100;
				m.Skills[SkillName.Throwing].Base = 100;
				m.Skills[SkillName.Imbuing].Base = 100;
                m.RawStr = 75;
                m.RawDex = 75;
                m.RawInt = 75;
				return true;
			}
                        else
                        {
                        	return false;
                        }
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
