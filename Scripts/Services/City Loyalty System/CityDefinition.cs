using System;
using System.Linq;

namespace Server.Engines.CityLoyalty
{
    [PropertyObject]
    public class CityDefinition
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public City City { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D StoneLocation { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D TradeMinisterLocation { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D HeraldLocation { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D GuardsmanLocation { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D BoardLocation { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Name { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LocalizedName { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BattleMessage { get; }

        private Region _Region;

        [CommandProperty(AccessLevel.GameMaster)]
        public Region Region
        {
            get
            {
                if (_Region == null)
                {
                    _Region = Region.Regions.FirstOrDefault(r => r.Name == Name && r.Map == CityLoyaltySystem.SystemMap);

                    if (_Region == null)
                        Console.WriteLine("WARNING: Region for {0} not found!", Name);
                }

                return _Region;
            }
        }

        public override string ToString()
        {
            return "...";
        }

        public CityDefinition(City city, Point3D stoneLoc, Point3D tradeloc, Point3D heraldLoc, Point3D guardsmanloc, Point3D boardloc, string name, int locname, int battleloc)
        {
            City = city;
            StoneLocation = stoneLoc;
            TradeMinisterLocation = tradeloc;
            HeraldLocation = heraldLoc;
            GuardsmanLocation = guardsmanloc;
            BoardLocation = boardloc;
            Name = name;
            LocalizedName = locname;
            BattleMessage = battleloc;
        }
    }
}
