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
        public TextDefinition Body { get; set; }

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
        public bool Expired { get { return Expires != DateTime.MinValue && Expires < DateTime.Now; } }

        [CommandProperty(AccessLevel.Administrator)]
        public bool Saves { get { return !PreLoaded && Expires != DateTime.MinValue; } }

        public TownCryerGreetingEntry(TextDefinition body)
            : this(null, body, -1)
        {
        }

        public TownCryerGreetingEntry(TextDefinition title, TextDefinition body, string link, string linkText)
            : this(title, body, -1, link, linkText)
        {
        }

        public TownCryerGreetingEntry(TextDefinition title, TextDefinition body, int expires = -1, string link = null, string linkText = null)
        {
            Title = title;
            Body = body;
            Created = DateTime.Now;

            Link = link;
            LinkText = linkText;

            if (expires > 0)
            {
                Expires = DateTime.Now + TimeSpan.FromHours(expires);
            }
        }

        public TownCryerGreetingEntry(GenericReader reader)
        {
            int version = reader.ReadInt();

            Title = TextDefinition.Deserialize(reader);
            Body = TextDefinition.Deserialize(reader);
            Created = reader.ReadDateTime();
            Expires = reader.ReadDateTime();

            Link = reader.ReadString();
            LinkText = reader.ReadString();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            TextDefinition.Serialize(writer, Title);
            TextDefinition.Serialize(writer, Body);
            writer.Write(Created);
            writer.Write(Expires);

            writer.Write(Link);
            writer.Write(LinkText);
        }
    }
}