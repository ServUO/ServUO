using System;

namespace Server.Services.TownCryer
{
    public class TownCryerModeratorEntry
    {
        public string Title { get; set; }
        public string Body1 { get; set; }
        public string Body2 { get; set; }
        public string Body3 { get; set; }
        public DateTime Expires { get; set; }
        public string ModeratorName { get; set; }

        public bool Expired => Expires < DateTime.Now;

        public TownCryerModeratorEntry(Mobile m, int duration, string title, string body1, string body2 = null, string body3 = null)
        {
            Title = title;
            Body1 = body1;
            Body2 = body2;
            Body3 = body3;
            ModeratorName = "EM " + m.Name;
            Expires = DateTime.Now + TimeSpan.FromDays(duration);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);
            writer.Write(Title);
            writer.Write(Body1);
            writer.Write(Body2);
            writer.Write(Body3);
            writer.Write(Expires);
            writer.Write(ModeratorName);
        }

        public TownCryerModeratorEntry(GenericReader reader)
        {
            int version = reader.ReadInt();

            Title = reader.ReadString();
            Body1 = reader.ReadString();
            Body2 = reader.ReadString();
            Body3 = reader.ReadString();
            Expires = reader.ReadDateTime();
            ModeratorName = reader.ReadString();
        }
    }
}