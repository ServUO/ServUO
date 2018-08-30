using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class KeeperOfBushido : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public KeeperOfBushido() : base( "the Keeper of Bushido" )
		{
			SetSkill( SkillName.Fencing, 75.0, 85.0 );
			SetSkill( SkillName.Macing, 75.0, 85.0 );
			SetSkill( SkillName.Swords, 75.0, 85.0 );
			SetSkill( SkillName.Bushido, 100.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBKeeperOfBushido() );
		}

		public override void InitOutfit()
		{
			AddItem( new NoDachi() );
			AddItem( new NinjaTabi() );
			AddItem( new PlateSuneate() );
			AddItem( new LightPlateJingasa() );
			AddItem( new LeatherDo() );
			AddItem( new LeatherHiroSode() );

			PackGold( 100, 200 );
		}

		public KeeperOfBushido( Serial serial ) : base( serial )
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