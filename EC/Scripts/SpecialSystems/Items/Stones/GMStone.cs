#if false
using System;
using Server.Network;
using Server.Commands;

namespace Server.Items
{
	public class GMStone : Item
	{
		public override string DefaultName
		{
			get { return "a GM stone"; }
		}

		[Constructable]
		public GMStone() : base( 0xED4 )
		{
			Movable = false;
			Hue = 0x489;
		}

		public GMStone( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.AccessLevel < AccessLevel.GameMaster )
			{
				from.AccessLevel = AccessLevel.GameMaster;

				from.SendAsciiMessage( 0x482, "The command prefix is \"{0}\"", CommandSystem.Prefix );
				CommandHandlers.Help_OnCommand( new CommandEventArgs( from, "help", "", new string[0] ) );
			}
			else
			{
				from.SendMessage( "The stone has no effect." );
			}
		}
	}
}
#endif