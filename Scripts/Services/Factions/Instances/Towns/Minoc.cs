using System;

namespace Server.Factions
{
    public class Minoc : Town
    {
        public Minoc()
        {
            this.Definition =
                new TownDefinition(
                    2,
                    0x186B,
                    "Minoc",
                    "Minoc",
                    new TextDefinition(1011437, "MINOC"),
                    new TextDefinition(1011564, "TOWN STONE FOR MINOC"),
                    new TextDefinition(1041036, "The Faction Sigil Monolith of Minoc"),
                    new TextDefinition(1041406, "The Faction Town Sigil Monolith Minoc"),
                    new TextDefinition(1041415, "Faction Town Stone of Minoc"),
                    new TextDefinition(1041397, "Faction Town Sigil of Minoc"),
                    new TextDefinition(1041388, "Corrupted Faction Town Sigil of Minoc"),
                    new Point3D(2471, 439, 15),
                    new Point3D(2469, 445, 15));
        }
    }
}