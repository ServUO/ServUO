using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class WeaponRenamingTool : Item, IRewardItem
	{				

		private bool m_IsRewardItem;
		
	
		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get{ return m_IsRewardItem; }
			set{ m_IsRewardItem = value; InvalidateProperties(); }
		}
		
		[Constructable]
		public WeaponRenamingTool() : base( 0x32F8 )
		{
			LootType = LootType.Blessed;
			Weight = 1.0;
			Name = "Weapon Renaming Tool";
			Hue = 38;
		}

		public WeaponRenamingTool( Serial serial ) : base( serial )
		{
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( m_IsRewardItem && !RewardSystem.CheckIsUsableBy( from, this, null ) )
				return;

				from.SendMessage( "Select an object to rename" ); // Select an object to engrave.
				from.Target = new TargetWeapon( this );
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
			
			writer.Write( (bool) m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
			
		m_IsRewardItem = reader.ReadBool();
		}
				
		public static WeaponRenamingTool Find( Mobile from )
		{
			if ( from.Backpack != null )
				return from.Backpack.FindItemByType( typeof( WeaponRenamingTool ) ) as WeaponRenamingTool;
				
			return null;
		}
		
		private class TargetWeapon : Target
		{
			private WeaponRenamingTool m_Tool;
		
			public TargetWeapon( WeaponRenamingTool tool ) : base( -1, true, TargetFlags.None )
			{
				m_Tool = tool;
			}
			
			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Tool == null || m_Tool.Deleted )
					return;
					
				if ( targeted is BaseWeapon )
				{
					BaseWeapon item = (BaseWeapon) targeted;
					
					from.CloseGump( typeof( InternalGump ) );
					from.SendGump( new InternalGump( m_Tool, item ) );
				}
				else
					from.SendMessage( "The selected item cannobt be renamed using this tool" ); 
			}
		}
		
		private class InternalGump : Gump
		{
			private WeaponRenamingTool m_Tool;
			private BaseWeapon m_Target;
		
			private enum Buttons
			{
				Cancel,
				Okay,
				Text
			}
		
			public InternalGump( WeaponRenamingTool tool, BaseWeapon target ) : base( 0, 0 )
			{
				m_Tool = tool;
				m_Target = target;
			
				Closable = true;
				Disposable = true;
				Dragable = true;
				Resizable = false;
				
				AddBackground( 50, 50, 400, 300, 0xA28 );

				AddPage( 0 );

				AddHtmlLocalized( 50, 70, 400, 20, 1072359, 0x0, false, false ); // <CENTER>Renaming Tool</CENTER>
				AddHtmlLocalized( 75, 95, 350, 145, 1076229, 0x0, true, true ); // Please enter the text to add to the selected object. Leave the text area blank to remove any existing text.  Removing text does not use a charge.
				AddButton( 125, 300, 0x81A, 0x81B, (int) Buttons.Okay, GumpButtonType.Reply, 0 );
				AddButton( 320, 300, 0x819, 0x818, (int) Buttons.Cancel, GumpButtonType.Reply, 0 );
				AddImageTiled( 75, 245, 350, 40, 0xDB0 );
				AddImageTiled( 76, 245, 350, 2, 0x23C5 );
				AddImageTiled( 75, 245, 2, 40, 0x23C3 );
				AddImageTiled( 75, 285, 350, 2, 0x23C5 );
				AddImageTiled( 425, 245, 2, 42, 0x23C3 );
				
				AddTextEntry( 75, 245, 350, 40, 0x0, (int) Buttons.Text, "" );
			}
			
			public override void OnResponse( Server.Network.NetState state, RelayInfo info )
			{		
				if ( m_Tool == null || m_Tool.Deleted || m_Target == null || m_Target.Deleted )
					return;
			
				if ( info.ButtonID == (int) Buttons.Okay )
				{
					TextRelay relay = info.GetTextEntry( (int) Buttons.Text );
					
					if ( relay != null )
					{
						if ( String.IsNullOrEmpty( relay.Text ) )
						{
							m_Target.Name = null;
							state.Mobile.SendLocalizedMessage( 1072362 ); // You remove the engraving from the object.
						}
						else
						{
							if ( relay.Text.Length > 64 )
								m_Target.Name = relay.Text.Substring( 0, 64 );
							else
								m_Target.Name = relay.Text;
						
							state.Mobile.SendLocalizedMessage( 1072361 ); // You engraved the object.						
							m_Tool.Delete();
						}
					}
				}
				else
					state.Mobile.SendLocalizedMessage( 1072363 ); // The object was not engraved.
			}
		}
	}
}	
