using System;

namespace Server.Factions
{
    public class StrongholdDefinition
    {
        private readonly Rectangle2D[] m_Area;
        private readonly Point3D m_JoinStone;
        private readonly Point3D m_FactionStone;
        private readonly Point3D[] m_Monoliths;
        public StrongholdDefinition(Rectangle2D[] area, Point3D joinStone, Point3D factionStone, Point3D[] monoliths)
        {
            this.m_Area = area;
            this.m_JoinStone = joinStone;
            this.m_FactionStone = factionStone;
            this.m_Monoliths = monoliths;
        }

        public Rectangle2D[] Area
        {
            get
            {
                return this.m_Area;
            }
        }
        public Point3D JoinStone
        {
            get
            {
                return this.m_JoinStone;
            }
        }
        public Point3D FactionStone
        {
            get
            {
                return this.m_FactionStone;
            }
        }
        public Point3D[] Monoliths
        {
            get
            {
                return this.m_Monoliths;
            }
        }
    }
}