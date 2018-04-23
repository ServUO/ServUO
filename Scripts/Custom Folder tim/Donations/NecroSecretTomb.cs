/**********************************************************
RunUO AoV C# script file
Official Age of Valor Script :: www.uovalor.com
Last modified by Red Squirrel on Aug-08-2011 09:29:13pm
Filepath: scripts\Items\artifacts\Artifacts of Assimilation\lessers\TheSecretBook.cs
Lines of code: 36

Description: 

***********************************************************/




//******************************************************************
//NOTE:  This artifact has been converted over to LifelessWhisper
//******************************************************************

using System;
using Server;

namespace Server.Items
{
	public class NecroSecretTomb : NecromancerSpellbook
	{

		[Constructable]
		public NecroSecretTomb() : base()
		{
			Name = "The Secret Book of the Evil Math Triangle";
			Hue = 1109;
			Attributes.RegenHits = 5;
			Attributes.LowerManaCost = 8;
			Attributes.SpellDamage = 12;
			SkillBonuses.SetValues( 1, SkillName.Magery, 15.0 );
			SkillBonuses.SetValues( 2, SkillName.Necromancy, 15.0 );
		}

		public NecroSecretTomb( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
		
			
	}
}