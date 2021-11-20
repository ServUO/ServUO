namespace Server.Items
{
	public class XmlTextEntryBook : BaseBook
	{
		[Constructable]
		public XmlTextEntryBook(int itemID, string title, string author, int pageCount, bool writable)
			: base(itemID, title, author, pageCount, writable)
		{
		}

		public XmlTextEntryBook(Serial serial) : base(serial)
		{
		}

		public void Fill(string text)
		{
			int pagenum = 0;
			int current = 0;

			// break up the text into single line length pieces
			while (text != null && current < text.Length)
			{
				int lineCount = 10;
				string[] lines = new string[lineCount];

				// place the line on the page
				for (int i = 0; i < lineCount; i++)
				{
					if (current < text.Length)
					{
						// make each line 25 chars long
						int length = text.Length - current;

						if (length > 20) 
							length = 20;

						lines[i] = text.Substring(current, length);
						current += length;
					}
					else
					{
						// fill up the remaining lines
						lines[i] = string.Empty;
					}
				}

				if (pagenum >= PagesCount)
					return;

				Pages[pagenum].Lines = lines;
				pagenum++;
			}

			// empty the remaining contents
			for (int j = pagenum; j < PagesCount; j++)
			{
				if (Pages[j].Lines.Length > 0)
				{
					for (int i = 0; i < Pages[j].Lines.Length; i++)
					{
						Pages[j].Lines[i] = string.Empty;
					}
				}
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();

			Delete();
		}
	}
}

