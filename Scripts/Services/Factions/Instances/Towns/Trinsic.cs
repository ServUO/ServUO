using System;

namespace Server.Factions
{
    public class Trinsic : Town
    {
        public Trinsic()
        {
            this.Definition =
                new TownDefinition(
                    1,
                    0x186A,
                    "Trinsic",
                    "Trinsic",
                    new TextDefinition(1011434, "TRINSIC"),
                    new TextDefinition(1011562, "TOWN STONE FOR TRINSIC"),
                    new TextDefinition(1041035, "The Faction Sigil Monolith of Trinsic"),
                    new TextDefinition(1041405, "The Faction Town Sigil Monolith of Trinsic"),
                    new TextDefinition(1041414, "Faction Town Stone of Trinsic"),
                    new TextDefinition(1041396, "Faction Town Sigil of Trinsic"),
                    new TextDefinition(1041387, "Corrupted Faction Town Sigil of Trinsic"),
                    new Point3D(1914, 2717, 20),
                    new Point3D(1909, 2720, 20));
        }
    }
}