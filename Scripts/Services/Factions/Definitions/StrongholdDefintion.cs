using System;

namespace Server.Factions
{
    public class StrongholdDefinition
    {
        private readonly Rectangle2D[] m_Area;
        private readonly Point3D m_JoinStone;
        private readonly Point3D m_FactionStone;
        private readonly Point3D[] m_Monoliths;
        private readonly Point3D m_CollectionBox;

        public StrongholdDefinition(Rectangle2D[] area, Point3D joinStone, Point3D factionStone, Point3D[] monoliths, Point3D collectionBox)
        {
            m_Area = area;
            m_JoinStone = joinStone;
            m_FactionStone = factionStone;
            m_Monoliths = monoliths;
            m_CollectionBox = collectionBox;
        }

        public Rectangle2D[] Area
        {
            get
            {
                return m_Area;
            }
        }
        public Point3D JoinStone
        {
            get
            {
                return m_JoinStone;
            }
        }
        public Point3D FactionStone
        {
            get
            {
                return m_FactionStone;
            }
        }
        public Point3D[] Monoliths
        {
            get
            {
                return m_Monoliths;
            }
        }

        public Point3D CollectionBox
        {
            get
            {
                return m_CollectionBox;
            }
        }
    }
}