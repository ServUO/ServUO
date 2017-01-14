using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
/*
** QuestNote
** ArteGordon
**
** Version 1.02
** updated 9/14/04
** - changed the QuestNote to open the status gump directly, and removed the serialized properties related to the scroll gump
** - added the OriginalQuestNote that has the scroll gump and behaves like the old QuestNote
**
** Version 1.01
** updated 3/25/04
** - Moved the TitleString and NoteString properties from the QuestNote class to the XmlQuestToken class.
**
** Version 1.0
** updated 1/07/04
** adds a item that displays text messages in a scroll gump and maintains quest state information.  The size can be varied and the note text and text-color can be specified.
** The title of the note and its color can also be set.
*/
namespace Server.Items
{
	public class QuestNote : XmlQuestToken
	{

		[Constructable]
		public QuestNote() : base( 0x14EE )
		{
			Name = "A quest note";
			TitleString = "A quest note";
		}

		public QuestNote( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version
			// Version 2 has no serialized variables

			// Version 0
			//writer.Write( this.m_NoteString );    // moved to the XmlQuestToken class in version 1
			//writer.Write( this.m_TitleString );   // moved to the XmlQuestToken class in version 1
			// Version 1
			//writer.Write( this.m_TextColor );   // no longer used
			//writer.Write( this.m_TitleColor );  // no longer used
			//writer.Write( this.m_size );        // no longer used
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
         
			switch ( version )
			{
				case 1:
				{
					reader.ReadInt();
					reader.ReadInt();
					reader.ReadInt();
					//this.m_TextColor = reader.ReadInt();
					//this.m_TitleColor = reader.ReadInt();
					//this.m_size = reader.ReadInt();
				}
					break;
				case 0:
				{
					reader.ReadString();
					reader.ReadString();
					reader.ReadInt();
					reader.ReadInt();
					reader.ReadInt();
					//this.NoteString = reader.ReadString();
					//this.TitleString = reader.ReadString();
					//this.m_TextColor = reader.ReadInt();
					//this.m_TitleColor = reader.ReadInt();
					//this.m_size = reader.ReadInt();
				}
					break;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			base.OnDoubleClick(from);
			from.CloseGump( typeof( XmlQuestStatusGump ) );

			from.SendGump( new XmlQuestStatusGump(this, this.TitleString) );
		}
	}


	public class OriginalQuestNote : XmlQuestToken
	{
		private int m_size = 1;

		private int m_TextColor = 0x3e8;
		private int m_TitleColor = 0xef0000;  // cyan 0xf70000, black 0x3e8, brown 0xef0000 darkblue 0x7fff

		[Constructable]
		public OriginalQuestNote() : base( 0x14EE )
		{
			Name = "A quest note";
			TitleString = "A quest note";
		}

		public OriginalQuestNote( Serial serial ) : base( serial )
		{
		}


		[CommandProperty( AccessLevel.GameMaster )]
		public int Size
		{
				get{ return m_size; }
			set 
			{
				m_size = value;
				if(m_size < 1) m_size = 1;
				//InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int TextColor
		{
				get{ return m_TextColor; }
			set 
			{
				m_TextColor = value; 
				//InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int TitleColor
		{
				get{ return m_TitleColor; }
			set 
			{ 
				m_TitleColor = value; 
				//InvalidateProperties();
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			// Version 1
			writer.Write( this.m_TextColor );
			writer.Write( this.m_TitleColor );
			writer.Write( this.m_size );
			// Version 0
			//writer.Write( this.m_NoteString );    // moved to the XmlQuestToken class in version 1
			//writer.Write( this.m_TitleString );   // moved to the XmlQuestToken class in version 1
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			switch ( version )
			{
				case 1:
				{
					this.m_TextColor = reader.ReadInt();
					this.m_TitleColor = reader.ReadInt();
					this.m_size = reader.ReadInt();
				}
					break;
				case 0:
				{
					this.NoteString = reader.ReadString();
					this.TitleString = reader.ReadString();
					this.m_TextColor = reader.ReadInt();
					this.m_TitleColor = reader.ReadInt();
					this.m_size = reader.ReadInt();
				}
					break;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			base.OnDoubleClick(from);
			from.CloseGump( typeof( QuestNoteGump ) );
			from.SendGump( new QuestNoteGump( this ) );
		}
	}

	public class QuestNoteGump : Gump
	{
		private OriginalQuestNote m_Note;

		public static string HtmlFormat( string text, int color )
		{
			return String.Format( "<BASEFONT COLOR=#{0}>{1}</BASEFONT>", color, text);
		}

		public QuestNoteGump( OriginalQuestNote note ) : base( 0, 0 )
		{
			m_Note = note;

			AddPage( 0 );
			AddAlphaRegion( 40, 41, 225, /*371*/70*note.Size );
			// scroll top
			AddImageTiled( 3, 5, 300, 37, 0x820 );
			// scroll middle, upper portion
			AddImageTiled( 19, 41, 263, 70, 0x821 );
			for(int i=1;i<note.Size;i++)
			{
				// scroll middle , lower portion
				AddImageTiled( 19, 41+70*i, 263, 70, 0x822 );
			}
			// scroll bottom
			AddImageTiled( 20, 111+70*(note.Size-1), 273, 34, 0x823 );

			// title string
			AddHtml( 55, 10, 200, 37, QuestNoteGump.HtmlFormat( note.TitleString, note.TitleColor), false , false );
			// text string
			AddHtml( 40, 41, 225, 70*note.Size, QuestNoteGump.HtmlFormat( note.NoteString, note.TextColor ), false , false );
            
			// add the quest status gump button
			AddButton( 40, 50+ note.Size*70, 0x037, 0x037, 1, GumpButtonType.Reply, 0 );

		}
        
		public override void OnResponse( Server.Network.NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			if ( info.ButtonID == 1 )
			{
				XmlQuestStatusGump g = new XmlQuestStatusGump(m_Note, m_Note.TitleString);
				from.SendGump( g );
			}
		}
	}
    

}
