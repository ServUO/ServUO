//ssusing System;
using Server.Gumps;

namespace Server.Items
{
	public class SkillGate60 : Item
	{
		[Constructable]
		public SkillGate60() : base( 0xF6C )
		{
			Movable = false;
			Name = "Skill and Stats 100 Gate";
		}

		public SkillGate60( Serial serial ) : base( serial )
		{
		}

		public override bool OnMoveOver( Mobile m )
		{
			if (m.Map.CanFit( m.Location, 16, false, false ) )
			{
				m.Skills[SkillName.Alchemy].Base = 60;
				m.Skills[SkillName.Anatomy].Base = 60;
				m.Skills[SkillName.AnimalLore].Base = 60;
				m.Skills[SkillName.AnimalTaming].Base = 60;
				m.Skills[SkillName.Archery].Base = 60;
                		m.Skills[SkillName.ArmsLore].Base = 60;
                		m.Skills[SkillName.Begging].Base = 60;
				m.Skills[SkillName.Blacksmith].Base = 60;
				m.Skills[SkillName.Camping].Base = 60;
				m.Skills[SkillName.Carpentry].Base = 60;
				m.Skills[SkillName.Cartography].Base = 60;
				m.Skills[SkillName.Cooking].Base = 60;
				m.Skills[SkillName.DetectHidden].Base = 60;
				m.Skills[SkillName.Discordance].Base = 60;
				m.Skills[SkillName.EvalInt].Base = 60;
				m.Skills[SkillName.Fishing].Base = 60;
				m.Skills[SkillName.Fencing].Base = 60;
				m.Skills[SkillName.Fletching].Base = 60;
				m.Skills[SkillName.Focus].Base = 60;
				m.Skills[SkillName.Forensics].Base = 60;
				m.Skills[SkillName.Healing].Base = 60;
				m.Skills[SkillName.Herding].Base = 60;
				m.Skills[SkillName.Hiding].Base = 60;
				m.Skills[SkillName.Inscribe].Base = 60;
				m.Skills[SkillName.ItemID].Base = 60;
				m.Skills[SkillName.Lockpicking].Base = 60;
				m.Skills[SkillName.Lumberjacking].Base = 60;
				m.Skills[SkillName.Macing].Base = 60;
				m.Skills[SkillName.Magery].Base = 60;
				m.Skills[SkillName.MagicResist].Base = 60;
				m.Skills[SkillName.Meditation].Base = 60;
				m.Skills[SkillName.Mining].Base = 60;
				m.Skills[SkillName.Musicianship].Base = 60;
				m.Skills[SkillName.Parry].Base = 60;
				m.Skills[SkillName.Peacemaking].Base = 60;
				m.Skills[SkillName.Poisoning].Base = 60;
				m.Skills[SkillName.Provocation].Base = 60;
				m.Skills[SkillName.RemoveTrap].Base = 60;
				m.Skills[SkillName.Snooping].Base = 60;
				m.Skills[SkillName.SpiritSpeak].Base = 60;
				m.Skills[SkillName.Stealing].Base = 60;
				m.Skills[SkillName.Stealth].Base = 60;
				m.Skills[SkillName.Swords].Base = 60;
				m.Skills[SkillName.Tactics].Base = 60;
				m.Skills[SkillName.Tailoring].Base = 60;
				m.Skills[SkillName.TasteID].Base = 60;
				m.Skills[SkillName.Tinkering].Base = 60;
				m.Skills[SkillName.Tracking].Base = 60;
				m.Skills[SkillName.Veterinary].Base = 60;
				m.Skills[SkillName.Wrestling].Base = 60;
				m.Skills[SkillName.Bushido].Base = 60;
				m.Skills[SkillName.Ninjitsu].Base = 60;
				m.Skills[SkillName.Chivalry].Base = 60;
				m.Skills[SkillName.Necromancy].Base = 60;
				m.Skills[SkillName.Spellweaving].Base = 60;
				m.Skills[SkillName.Mysticism].Base = 60;
				m.Skills[SkillName.Throwing].Base = 60;
				m.Skills[SkillName.Imbuing].Base = 60;
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
