using Server;
using System;

namespace Server.Services.TownCryer
{
    public class TownCryerNewsEntry
    {
        public TextDefinition Title { get; set; }
        public TextDefinition Body { get; set; }
        public int GumpImage { get; set; }
        public Type QuestType { get; set; }
        public string InfoUrl { get; set; }

        public TownCryerNewsEntry(TextDefinition title, TextDefinition body, int gumpImage, Type questType, string url)
        {
            Title = title;
            Body = body;
            GumpImage = gumpImage;
            QuestType = questType;
            InfoUrl = url;
        }
    }
}