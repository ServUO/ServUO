using System;

namespace Server.Factions
{
    public class Moonglow : Town
    {
        public Moonglow()
        {
            this.Definition =
                new TownDefinition(
                    3,
                    0x186C,
                    "Moonglow",
                    "Moonglow",
                    new TextDefinition(1011435, "MOONGLOW"),
                    new TextDefinition(1011563, "TOWN STONE FOR MOONGLOW"),
                    new TextDefinition(1041037, "The Faction Sigil Monolith of Moonglow"),
                    new TextDefinition(1041407, "The Faction Town Sigil Monolith of Moonglow"),
                    new TextDefinition(1041416, "Faction Town Stone of Moonglow"),
                    new TextDefinition(1041398, "Faction Town Sigil of Moonglow"),
                    new TextDefinition(1041389, "Corrupted Faction Town Sigil of Moonglow"),
                    new Point3D(4436, 1083, 0),
                    new Point3D(4432, 1086, 0));
        }
    }
}