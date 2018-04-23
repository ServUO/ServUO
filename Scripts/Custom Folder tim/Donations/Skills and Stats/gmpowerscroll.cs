/**********************************************************
RunUO 2.0 AoV C# script file
Official Age of Valor Script :: www.uovalor.com
Last modified by 
Filepath: scripts\_custom\items\misc\staff\gmpowerscroll.cs
Lines of code: 103
***********************************************************/


using Server;
using System;

namespace Server.Items
{
	
	public class gmpowerscroll : Item
	{
	
			public static SkillName[] AllSkills = new SkillName[]
			{
				SkillName.Alchemy,
				SkillName.Blacksmith,
				SkillName.Tailoring,
				SkillName.Swords,
				SkillName.Fencing,
				SkillName.Macing,
				SkillName.Archery,
				SkillName.Wrestling,
				SkillName.Parry,
				SkillName.Tactics,
				SkillName.Anatomy,
				SkillName.Healing,
				SkillName.Magery,
				SkillName.Meditation,
				SkillName.EvalInt,
				SkillName.MagicResist,
				SkillName.AnimalTaming,
				SkillName.AnimalLore,
				SkillName.Veterinary,
				SkillName.Musicianship,
				SkillName.Provocation,
				SkillName.Discordance,
				SkillName.Peacemaking,
				SkillName.Chivalry,
				SkillName.Focus,
				SkillName.Necromancy,
				SkillName.Stealing,
				SkillName.Stealth,
				SkillName.SpiritSpeak,
				SkillName.Carpentry,
				SkillName.Tinkering,
				SkillName.Mining,
				SkillName.Fletching,
				SkillName.Inscribe,
				SkillName.Spellweaving,
				SkillName.ItemID,
				SkillName.ArmsLore,
				SkillName.Begging,
				SkillName.Camping,
				SkillName.Cartography,
				SkillName.Cooking,
				SkillName.DetectHidden,
				SkillName.Forensics,
				SkillName.Lumberjacking,
				SkillName.RemoveTrap,
				SkillName.TasteID,
				SkillName.Bushido,
				SkillName.Ninjitsu,
				SkillName.Fishing,
				SkillName.Herding,
				SkillName.Lockpicking,
				SkillName.Snooping,
				SkillName.Hiding,
				SkillName.Poisoning,
				SkillName.Tracking
			};	
	

		[Constructable]
		public gmpowerscroll() : base(0x14F0)
		{
		Name="GM Power Scroll";
		LootType = LootType.Blessed;
		Hue=1150;
		}
		
		
		public override void OnDoubleClick( Mobile from )
		{
			if(from.AccessLevel<=AccessLevel.Player)return;
			
			
			
			for(int i=0;i<AllSkills.Length;i++)
			{
			SkillName skill = AllSkills[i];
			
			from.Skills[skill].Cap=0xFFFF;		
			}
			
			
			from.SendMessage("You feel a surge of magic as the scroll enhances all your skills!"); 

			Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0, 0, 0, 0, 0, 5060, 0 );
			Effects.PlaySound( from.Location, from.Map, 0x243 );

			Effects.SendMovingParticles( new Entity( Serial.Zero, new Point3D( from.X - 6, from.Y - 6, from.Z + 15 ), from.Map ), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100 );
			Effects.SendMovingParticles( new Entity( Serial.Zero, new Point3D( from.X - 4, from.Y - 6, from.Z + 15 ), from.Map ), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100 );
			Effects.SendMovingParticles( new Entity( Serial.Zero, new Point3D( from.X - 6, from.Y - 4, from.Z + 15 ), from.Map ), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100 );

			Effects.SendTargetParticles( from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100 );

			Delete();		
		
		}
		

		

		public gmpowerscroll( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );		
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
    	}
	}
	
}