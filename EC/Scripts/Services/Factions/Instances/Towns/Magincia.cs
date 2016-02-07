using System;

namespace Server.Factions
{
    public class Magincia : Town
    {
        public Magincia()
        {
            this.Definition =
                new TownDefinition(
                    7,
                    0x1870,
                    "Magincia",
                    "Magincia",
                    new TextDefinition(1011440, "MAGINCIA"),
                    new TextDefinition(1011568, "TOWN STONE FOR MAGINCIA"),
                    new TextDefinition(1041041, "The Faction Sigil Monolith of Magincia"),
                    new TextDefinition(1041411, "The Faction Town Sigil Monolith of Magincia"),
                    new TextDefinition(1041420, "Faction Town Stone of Magincia"),
                    new TextDefinition(1041402, "Faction Town Sigil of Magincia"),
                    new TextDefinition(1041393, "Corrupted Faction Town Sigil of Magincia"),
                    new Point3D(3714, 2235, 20),
                    new Point3D(3712, 2230, 20));
        }
    }
}