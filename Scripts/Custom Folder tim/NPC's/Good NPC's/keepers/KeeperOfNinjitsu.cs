using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class KeeperOfNinjitsu : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public KeeperOfNinjitsu() : base( "the Keeper of Ninjitsu" )
		{
			SetSkill( SkillName.Fencing, 75.0, 85.0 );
			SetSkill( SkillName.Macing, 75.0, 85.0 );
			SetSkill( SkillName.Swords, 75.0, 85.0 );
			SetSkill( SkillName.Ninjitsu, 100.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBKeeperOfNinjitsu() );
		}

		public override void InitOutfit()
		{
			AddItem( new SamuraiTabi() );
			AddItem( new LeatherNinjaPants() );
			AddItem( new LeatherNinjaHood() );
			AddItem( new LeatherNinjaBelt() );
			AddItem( new LeatherNinjaMitts() );
			AddItem( new LeatherNinjaJacket() );

		        PackGold( 100, 200 );
		}

		public KeeperOfNinjitsu( Serial serial ) : base( serial )
		{
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