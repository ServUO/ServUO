using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Quests;
using Server.Engines.Points;

namespace Server.Mobiles
{
    [PropertyObject]
    public class PointsSystemProps
    {
        public override string ToString()
        {
            return "...";
        }

        public PlayerMobile Player { get; set; }

        public PointsSystemProps(PlayerMobile pm)
        {
            Player = pm;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Blackthorn
        {
            get
            {
                return (int)PointsSystem.Blackthorn.GetPoints(Player);
            }
            set
            {
                PointsSystem.Blackthorn.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double CleanUpBrit
        {
            get
            {
                return (int)PointsSystem.CleanUpBritannia.GetPoints(Player);
            }
            set
            {
                PointsSystem.CleanUpBritannia.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double VoidPool
        {
            get
            {
                return (int)PointsSystem.VoidPool.GetPoints(Player);
            }
            set
            {
                PointsSystem.VoidPool.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Casino
        {
            get
            {
                return (int)PointsSystem.CasinoData.GetPoints(Player);
            }
            set
            {
                PointsSystem.CasinoData.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double QueensLoyalty
        {
            get
            {
                return (int)PointsSystem.QueensLoyalty.GetPoints(Player);
            }
            set
            {
                PointsSystem.QueensLoyalty.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double ShameCrystals
        {
            get
            {
                return (int)PointsSystem.ShameCrystals.GetPoints(Player);
            }
            set
            {
                PointsSystem.ShameCrystals.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double DespiseCrystals
        {
            get
            {
                return (int)PointsSystem.DespiseCrystals.GetPoints(Player);
            }
            set
            {
                PointsSystem.DespiseCrystals.SetPoints(Player, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double ViceVsVirtue
        {
            get
            {
                return (int)PointsSystem.ViceVsVirtue.GetPoints(Player);
            }
            set
            {
                PointsSystem.ViceVsVirtue.SetPoints(Player, value);
            }
        }
    }
}