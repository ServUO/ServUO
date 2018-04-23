using System;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;
using Server.Commands;

namespace Server.Items
{
	public class AddonEditorTool : Item
	{

		[Constructable]
        public AddonEditorTool() : base( 4033 )
        {
			Name = "Addon Editor Tool";
			Hue = 1287;
		}

        public AddonEditorTool(Serial serial) : base(serial)
        {
        }
		
		public override void OnDoubleClick(Mobile from)
		{
			if (from.AccessLevel == AccessLevel.Player)
			{
				from.SendMessage("That is for Staff use only!");
				return;
			}

			AddOnEditor_Att addoneditor = (AddOnEditor_Att)XmlAttach.FindAttachment(from, typeof(AddOnEditor_Att));
			
			if( addoneditor == null ) {
				XmlAttach.AttachTo(from, new AddOnEditor_Att());
			}
			
			addoneditor.CallCommand(from);

			base.OnDoubleClick(from);
		}

		public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
		}

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
		}
	}
}