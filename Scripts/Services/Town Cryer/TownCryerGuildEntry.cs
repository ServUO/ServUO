using Server.Guilds;
using System;

namespace Server.Services.TownCryer
{
    public class TownCryerGuildEntry
    {
        public string Title { get; set; }
        public string FullTitle { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public Guild Guild { get; set; }
        public DateTime EventTime { get; set; }
        public DateTime Expires { get; private set; }
        public string EventLocation { get; set; }

        public bool Expired => DateTime.Now + TimeSpan.FromDays(32) < DateTime.Now;

        public TownCryerGuildEntry(Mobile m, DateTime eventTime, string eventLocation, string title, string body)
        {
            Guild = m.Guild as Guild;
            Title = title;
            FullTitle = string.Format("{0}-{1} [{2}] {3}", eventTime.Month, eventTime.Day, Guild.Abbreviation, title);
            Body = body;
            Author = m.Name;
            EventTime = eventTime;
            EventLocation = eventLocation;
        }

        public void GetExpiration()
        {
            DateTime dt = DateTime.Now.AddMonths(1);

            Expires = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Title);
            writer.Write(FullTitle);
            writer.Write(Body);
            writer.Write(Author);
            writer.Write(Guild);
            writer.Write(EventTime);
            writer.Write(EventLocation);
            writer.Write(Expires);
        }

        public TownCryerGuildEntry(GenericReader reader)
        {
            int version = reader.ReadInt();

            Title = reader.ReadString();
            FullTitle = reader.ReadString();
            Body = reader.ReadString();
            Author = reader.ReadString();
            Guild = reader.ReadGuild() as Guild;
            EventTime = reader.ReadDateTime();
            EventLocation = reader.ReadString();
            Expires = reader.ReadDateTime();
        }
    }
}