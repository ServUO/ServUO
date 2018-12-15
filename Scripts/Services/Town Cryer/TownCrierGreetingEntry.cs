using System;
using Server;

namespace Server.Services.TownCryer
{
    [PropertyObject]
    public class TownCryerGreetingEntry
    {
        [CommandProperty(AccessLevel.Administrator)]
        public TextDefinition Title { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public TextDefinition Body1 { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public string Body2 { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public string Body3 { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public DateTime Created { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public DateTime Expires { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public string Link { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public string LinkText { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public bool PreLoaded { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public bool CanEdit { get; private set; }

        [CommandProperty(AccessLevel.Administrator)]
        public bool Expired { get { return Expires != DateTime.MinValue && Expires < DateTime.Now; } }

        [CommandProperty(AccessLevel.Administrator)]
        public bool Saves { get { return !PreLoaded && Expires != DateTime.MinValue; } }

        public TownCryerGreetingEntry(TextDefinition body)
            : this(null, body, null, null, -1)
        {
        }

        public TownCryerGreetingEntry(TextDefinition title, TextDefinition body, string link, string linkText)
            : this(title, body, null, null, -1, link, linkText)
        {
        }

        public TownCryerGreetingEntry(TextDefinition title, TextDefinition body, string body2, string link, string linkText)
            : this(title, body, body2, null, -1, link, linkText)
        {
        }

        public TownCryerGreetingEntry(TextDefinition title, TextDefinition body, int expires, string link, string linkText)
            : this(title, body, null, null, expires, link, linkText)
        {
        }

        public TownCryerGreetingEntry(TextDefinition title, TextDefinition body, string body2, string body3, int expires = -1, string link = null, string linkText = null, bool canEdit = false)
        {
            Title = title;

            Body1 = body;
            Body2 = body2;
            Body3 = body3;

            Created = DateTime.Now;

            Link = link;
            LinkText = linkText;

            if (expires > 0)
            {
                Expires = DateTime.Now + TimeSpan.FromHours(expires);
            }

            CanEdit = canEdit;
        }

        public TownCryerGreetingEntry(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    CanEdit = reader.ReadBool();

                    Body2 = reader.ReadString();
                    Body3 = reader.ReadString();
                    goto case 0;
                case 0:
                    Title = TextDefinition.Deserialize(reader);
                    Body1 = TextDefinition.Deserialize(reader);
                    Created = reader.ReadDateTime();
                    Expires = reader.ReadDateTime();

                    Link = reader.ReadString();
                    LinkText = reader.ReadString();
                    break;
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(1);

            writer.Write(CanEdit);

            writer.Write(Body2);
            writer.Write(Body3);

            TextDefinition.Serialize(writer, Title);
            TextDefinition.Serialize(writer, Body1);
            writer.Write(Created);
            writer.Write(Expires);

            writer.Write(Link);
            writer.Write(LinkText);
        }
    }
}