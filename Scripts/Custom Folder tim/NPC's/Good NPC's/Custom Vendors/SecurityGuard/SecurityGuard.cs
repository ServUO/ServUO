/*
 * Created by Mr. Bill Creations
 * Date: 2/16/2008
 * Time: 11:03 PM
 * 
*/
using System;
using System.Collections.Generic;
using Server;

namespace Server.Mobiles
{
	public class SecurityGuard : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }
		
		[Constructable]
		public SecurityGuard() : base( "the security guard" )
		{
			SetSkill( SkillName.EvalInt, 65.0, 88.0 );
			SetSkill( SkillName.Tactics, 36.0, 68.0 );
			SetSkill( SkillName.Macing, 45.0, 68.0 );
			SetSkill( SkillName.MagicResist, 65.0, 88.0 );
			SetSkill( SkillName.Wrestling, 36.0, 68.0 );
		}
		
		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBSecurityGuard() );
		}
		
		public override void InitOutfit()
		{
			AddItem( new Server.Items.MonksRobe() );
		    AddItem( new Server.Items.WarHammer() );
            AddItem( new Server.Items.Sandals() );
		}
		
		public SecurityGuard( Serial serial ) : base( serial )
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


