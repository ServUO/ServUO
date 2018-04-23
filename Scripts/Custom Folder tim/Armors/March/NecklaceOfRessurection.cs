using System;
///////This is Lord Greywolf's script. I hereby proclaim he kicks ass on RunUO forums. Signed Stygian Stalker.

using Server;

namespace Server.Items
{
	public class NecklaceOfRessurection : BaseEarrings
	{
		[Constructable]
		public NecklaceOfRessurection() : base( 0x3BB5 )
		{
			Name = "Necklace Of Ressurection";
			SkillBonuses.SetValues( 0, m_PossibleBonusSkills[Utility.Random(m_PossibleBonusSkills.Length)], (Utility.Random( 5 ) == 0 ? 15.0 : Utility.RandomMinMax( 5, 10 )) );
			SkillBonuses.SetValues( 1, m_PossibleBonusSkills[Utility.Random(m_PossibleBonusSkills.Length)], (Utility.Random( 5 ) == 0 ? 15.0 : Utility.RandomMinMax( 5, 10 )) );
		}

		private static SkillName[] m_PossibleBonusSkills = new SkillName[]
		{
			SkillName.Alchemy,
			SkillName.Anatomy,
			SkillName.AnimalLore,
			SkillName.ItemID,
			SkillName.ArmsLore,
			SkillName.Parry,
			SkillName.Begging,
			SkillName.Blacksmith,
			SkillName.Fletching,
			SkillName.Peacemaking,

			SkillName.Carpentry,
			SkillName.Cartography,
			SkillName.Cooking,
			SkillName.DetectHidden,
			SkillName.Discordance,
			SkillName.EvalInt,
			SkillName.Healing,
			SkillName.Fishing,
			SkillName.Forensics,
			SkillName.Herding,
			SkillName.Hiding,
			SkillName.Provocation,
			SkillName.Inscribe,
			SkillName.Lockpicking,
			SkillName.Magery,
			SkillName.MagicResist,
			SkillName.Tactics,
			SkillName.Snooping,
			SkillName.Musicianship,
			SkillName.Poisoning,
			SkillName.Archery,
			SkillName.SpiritSpeak,
			SkillName.Stealing,
			SkillName.Tailoring,
			SkillName.AnimalTaming,

			SkillName.Tinkering,
			SkillName.Tracking,
			SkillName.Veterinary,
			SkillName.Swords,
			SkillName.Macing,
			SkillName.Fencing,
			SkillName.Wrestling,
			SkillName.Lumberjacking,
			SkillName.Mining,
			SkillName.Meditation,
			SkillName.Stealth,
			SkillName.RemoveTrap,

			SkillName.Necromancy,
			SkillName.Focus,
			SkillName.Chivalry,
			SkillName.Bushido,
			SkillName.Ninjitsu
		};

		public NecklaceOfRessurection(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}
}