using System;
using Server;

namespace Server.Items
{
	public class BlackBookOfSpells : Spellbook
	{
		public override string DefaultName{ get{ return "black book of spells"; } }
		public override int LabelNumber{ get{ return 0; } }

		[Constructable]
		public BlackBookOfSpells() : this( (ulong)0 )
		{
		}

		[Constructable]
		public BlackBookOfSpells( ulong content ) : base( content )
		{
			LootType = LootType.Regular;
			Hue = 1175;
		}

		public BlackBookOfSpells( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}