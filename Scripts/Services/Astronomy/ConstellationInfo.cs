using System;

namespace Server.Engines.Astronomy
{
    [PropertyObject]
    public class ConstellationInfo
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public int Identifier { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeCoordinate TimeCoordinate { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CoordRA { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double CoordDEC { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Name { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile DiscoveredBy { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DiscoveredOn { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasBeenDiscovered => DiscoveredOn != DateTime.MinValue;

        public StarPosition[] StarPositions { get; set; }

        public ConstellationInfo(TimeCoordinate p, int coordRA, double coordDEC, StarPosition[] starInfo)
        {
            TimeCoordinate = p;
            CoordRA = coordRA;
            CoordDEC = coordDEC;
            StarPositions = starInfo;

            Identifier = -1;
        }

        public override string ToString()
        {
            return "...";
        }

        public static StarPosition[] RandomStarPositions()
        {
            int amount = Utility.RandomMinMax(4, 7);
            StarPosition[] positions = new StarPosition[amount];

            for (int i = 0; i < amount; i++)
            {
                StarPosition star = new StarPosition
                {
                    ImageID = Utility.RandomMinMax(0x668, 0x67D),
                    X = Utility.RandomMinMax(180, 450),
                    Y = Utility.RandomMinMax(150, 400)
                };

                positions[i] = star;
            }

            return positions;
        }

        public struct StarPosition
        {
            public int ImageID { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        public ConstellationInfo(GenericReader reader)
        {
            reader.ReadInt();

            Identifier = reader.ReadInt();
            TimeCoordinate = (TimeCoordinate)reader.ReadInt();

            Name = reader.ReadString();
            DiscoveredBy = reader.ReadMobile();
            DiscoveredOn = reader.ReadDateTime();
            CoordRA = reader.ReadInt();
            CoordDEC = reader.ReadDouble();

            int count = reader.ReadInt();
            StarPositions = new StarPosition[count];

            for (int i = 0; i < count; i++)
            {
                StarPosition pos = new StarPosition
                {
                    ImageID = reader.ReadInt(),
                    X = reader.ReadInt(),
                    Y = reader.ReadInt()
                };

                StarPositions[i] = pos;
            }

            if (HasBeenDiscovered)
            {
                AstronomySystem.AddDiscovery(this);
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Identifier);
            writer.Write((int)TimeCoordinate);
            writer.Write(Name);
            writer.Write(DiscoveredBy);
            writer.Write(DiscoveredOn);
            writer.Write(CoordRA);
            writer.Write(CoordDEC);

            writer.Write(StarPositions.Length);

            foreach (StarPosition pos in StarPositions)
            {
                writer.Write(pos.ImageID);
                writer.Write(pos.X);
                writer.Write(pos.Y);
            }
        }
    }
}