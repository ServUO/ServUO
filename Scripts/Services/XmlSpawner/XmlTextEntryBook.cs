using System;

using Server.Mobiles;

namespace Server.Items
{
	public delegate void XmlTextEntryBookCallback(Mobile from, XmlTextEntryBook book);

	public sealed class XmlTextEntryBook : BaseEntryBook
	{
		public XmlSpawner SpawnerInstance { get; }
		public int SpawnerEntryIndex { get; }

		public XmlTextEntryBookCallback Callback { get; }

		public XmlTextEntryBook(XmlSpawner spawner, int entryIndex, string title, string author, string content, XmlTextEntryBookCallback callback)
			: base(title, author, content)
		{
			SpawnerInstance = spawner;
			SpawnerEntryIndex = entryIndex;

			Callback = callback;
		}

		public XmlTextEntryBook(Serial serial)
			: base(serial)
		{
		}

		protected override void OnContentChange(Mobile from)
		{
			base.OnContentChange(from);

			Callback?.Invoke(from, this);
		}

		public override void Serialize(GenericWriter writer)
		{
		}

		public override void Deserialize(GenericReader reader)
		{
			Delete();
		}
	}

	public abstract class BaseEntryBook : BaseBook
	{
		public BaseEntryBook(string title, string author, string content)
			: base(0, title, author, 20, true)
		{
			Visible = false;
			Movable = false;

			int pagenum = 0;
			int current = 0;

			// break up the text into single line length pieces
			while (content != null && current < content.Length)
			{
				string[] lines = new string[10];

				// place the line on the page
				for (int i = 0; i < lines.Length; i++)
				{
					if (current < content.Length)
					{
						// make each line 25 chars long
						int length = content.Length - current;

						if (length > 20)
						{
							length = 20;
						}

						lines[i] = content.Substring(current, length);

						current += length;
					}
					else
					{
						// fill up the remaining lines
						lines[i] = string.Empty;
					}
				}

				if (pagenum >= PagesCount)
				{
					return;
				}

				Pages[pagenum].Lines = lines;

				++pagenum;
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

		public BaseEntryBook(Serial serial) 
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
		}

		public override void Deserialize(GenericReader reader)
		{
			Delete();
		}
	}
}
