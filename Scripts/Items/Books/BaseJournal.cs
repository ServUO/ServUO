using System;
using Server;
using Server.Gumps;

namespace Server.Items
{
    public class BaseJournalGump : Gump
    {
        public BaseJournalGump(TextDefinition title, TextDefinition body)
            : base(10, 10)
        {
            AddImage(0, 0, 0x761C);
            int y = 20;

            if (title != null)
            {
                if (title.Number > 0)
                {
                    AddHtmlLocalized(50, y, 450, 20, 1154645, String.Format("#{0}", title.Number.ToString()), 0, false, false);
                    y += 30;
                }
                else
                {
                    AddHtml(50, y, 450, 20, String.Format("<CENTER>{0}</CENTER>", title.String), false, false);
                    y += 30;
                }
            }

            if (body.Number > 0)
            {
                AddHtmlLocalized(95, y, 380, 600, body.Number, 1, false, true);
            }
            else
            {
                AddHtml(95, y, 380, 600, body.String, false, true);
            }
        }
    }

    public abstract class BaseJournal : Item
    {
        public abstract TextDefinition Title { get; }
        public abstract TextDefinition Body { get; }

        public BaseJournal()
            : base(0xFBE)
        {
        }

        public BaseJournal(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                m.SendGump(new BaseJournalGump(Title, Body));
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Title != null)
            {
                if (Title.Number > 0)
                {
                    list.Add(Title.Number);
                }
                else
                {
                    list.Add(Title.String);
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}