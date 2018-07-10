using Server;
using System;
using Server.Engines.CityLoyalty;

namespace Server.Services.TownCryer
{
    public class TownCryerCityEntry
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public DateTime Expires { get; set; }
        public City City { get; set; }

        public bool Expired { get { return Expires < DateTime.Now; } }

        public TownCryerCityEntry(Mobile author, City city, int duration, string title, string body)
        {
            Author = author.Name;
            Expires = DateTime.Now + TimeSpan.FromDays(duration);
            Title = title;
            Body = body;
            City = city;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Title);
            writer.Write(Body);
            writer.Write(Expires);
            writer.Write(Author);
            writer.Write((int)City);
        }

        public TownCryerCityEntry(GenericReader reader)
        {
            int version = reader.ReadInt();

            Title = reader.ReadString();
            Body = reader.ReadString();
            Expires = reader.ReadDateTime();
            Author = reader.ReadString();
            City = (City)reader.ReadInt();
        }
    }
}