using System;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Engines.XmlSpawner2;
using Server.Items;

namespace Server.Engines.XmlSpawner2
{ 
   
   public class AddOnEditor_Att : XmlAttachment
   {
		[CommandProperty(AccessLevel.GameMaster)]
        public BaseAddon SelectedAddon 
		{ get; set; }
	 

		public static void Initialize() {
			CommandSystem.Register( "AddonEdit", AccessLevel.Player, new CommandEventHandler( AddonEdit_OnCommand ) );
		}

		[Usage("AddonEdit")]
		[Description("Allows you to edit AddOns")]
		public static void AddonEdit_OnCommand( CommandEventArgs e )
		{	
			AddOnEditor_Att addoneditor = (AddOnEditor_Att)XmlAttach.FindAttachment(e.Mobile, typeof(AddOnEditor_Att));
		
			if( addoneditor == null ) {
				XmlAttach.AttachTo(e.Mobile, new AddOnEditor_Att());
				AddonEdit_OnCommand(e);
			}
			else
			{
				if( e.Mobile.HasGump(typeof(AddOnEditor)) ) {
					e.Mobile.CloseGump(typeof(AddOnEditor));
				}
				e.Mobile.SendGump( new AddOnEditor( e.Mobile, addoneditor) );
			}
		}
		
		public void CallCommand( Mobile from )
		{
			AddOnEditor_Att addoneditor = (AddOnEditor_Att)XmlAttach.FindAttachment(from, typeof(AddOnEditor_Att));
			
			CommandEventArgs e = new CommandEventArgs(from, "", "", new string[0]);
			AddonEdit_OnCommand(e);
		}
		
		public void Resend( Mobile from )
		{
			AddOnEditor_Att addoneditor = (AddOnEditor_Att)XmlAttach.FindAttachment(from, typeof(AddOnEditor_Att));
			
			if( from.HasGump(typeof(AddOnEditor)) ) {
				from.CloseGump(typeof(AddOnEditor));
			}
			from.SendGump( new AddOnEditor( from, addoneditor) );
		}
	  
		public AddOnEditor_Att( ASerial serial ) : base( serial )
        {}
		
		[Attachable]
        public AddOnEditor_Att()
        {}
		
		public override void Serialize( GenericWriter writer )
		{
            base.Serialize( writer );
            writer.Write( (int) 0 ); // version
			
			writer.Write( (BaseAddon) SelectedAddon);
		}
		
		public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
			
			SelectedAddon = ( BaseAddon )reader.ReadItem();
			
		}
		
   }
}

