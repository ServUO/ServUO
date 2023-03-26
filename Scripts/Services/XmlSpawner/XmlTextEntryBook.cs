using System;

namespace Server.Items
{
	public sealed class XmlTextEntryBook : BaseEntryBook
	{
		public Action<string> Callback { get; }

		public XmlTextEntryBook(string title, string author, string content, Action<string> callback)
			: base(title, author, content)
		{
			Callback = callback;
		}

		public XmlTextEntryBook(Serial serial)
			: base(serial)
		{
		}

		protected override void OnContentChange(Mobile from)
		{
			base.OnContentChange(from);

			Callback?.Invoke(ContentAsStringNoBreaks);
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
			: base(0, title, author, DefaultPageCount, true)
		{
			Visible = false;
			Movable = false;

			var pagenum = 0;
			var current = 0;

			// break up the text into single line length pieces
			while (content != null && current < content.Length)
			{
				var lines = new string[LineLimit];

				// place the line on the page
				for (var i = 0; i < lines.Length; i++)
				{
					if (current < content.Length)
					{
						var length = Math.Min(20, content.Length - current);

						lines[i] = content.Substring(current, length);

						current += length;
					}
					else
					{
						// fill up the remaining lines
						lines[i] = String.Empty;
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
			for (var j = pagenum; j < PagesCount; j++)
			{
				if (Pages[j].Lines.Length > 0)
				{
					for (var i = 0; i < Pages[j].Lines.Length; i++)
					{
						Pages[j].Lines[i] = String.Empty;
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
