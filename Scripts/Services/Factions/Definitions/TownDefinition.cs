using System;

namespace Server.Factions
{
    public class TownDefinition
    {
        private readonly int m_Sort;
        private readonly int m_SigilID;
        private readonly string m_Region;
        private readonly string m_FriendlyName;
        private readonly TextDefinition m_TownName;
        private readonly TextDefinition m_TownStoneHeader;
        private readonly TextDefinition m_StrongholdMonolithName;
        private readonly TextDefinition m_TownMonolithName;
        private readonly TextDefinition m_TownStoneName;
        private readonly TextDefinition m_SigilName;
        private readonly TextDefinition m_CorruptedSigilName;
        private readonly Point3D m_Monolith;
        private readonly Point3D m_TownStone;
        public TownDefinition(int sort, int sigilID, string region, string friendlyName, TextDefinition townName, TextDefinition townStoneHeader, TextDefinition strongholdMonolithName, TextDefinition townMonolithName, TextDefinition townStoneName, TextDefinition sigilName, TextDefinition corruptedSigilName, Point3D monolith, Point3D townStone)
        {
            this.m_Sort = sort;
            this.m_SigilID = sigilID;
            this.m_Region = region;
            this.m_FriendlyName = friendlyName;
            this.m_TownName = townName;
            this.m_TownStoneHeader = townStoneHeader;
            this.m_StrongholdMonolithName = strongholdMonolithName;
            this.m_TownMonolithName = townMonolithName;
            this.m_TownStoneName = townStoneName;
            this.m_SigilName = sigilName;
            this.m_CorruptedSigilName = corruptedSigilName;
            this.m_Monolith = monolith;
            this.m_TownStone = townStone;
        }

        public int Sort
        {
            get
            {
                return this.m_Sort;
            }
        }
        public int SigilID
        {
            get
            {
                return this.m_SigilID;
            }
        }
        public string Region
        {
            get
            {
                return this.m_Region;
            }
        }
        public string FriendlyName
        {
            get
            {
                return this.m_FriendlyName;
            }
        }
        public TextDefinition TownName
        {
            get
            {
                return this.m_TownName;
            }
        }
        public TextDefinition TownStoneHeader
        {
            get
            {
                return this.m_TownStoneHeader;
            }
        }
        public TextDefinition StrongholdMonolithName
        {
            get
            {
                return this.m_StrongholdMonolithName;
            }
        }
        public TextDefinition TownMonolithName
        {
            get
            {
                return this.m_TownMonolithName;
            }
        }
        public TextDefinition TownStoneName
        {
            get
            {
                return this.m_TownStoneName;
            }
        }
        public TextDefinition SigilName
        {
            get
            {
                return this.m_SigilName;
            }
        }
        public TextDefinition CorruptedSigilName
        {
            get
            {
                return this.m_CorruptedSigilName;
            }
        }
        public Point3D Monolith
        {
            get
            {
                return this.m_Monolith;
            }
        }
        public Point3D TownStone
        {
            get
            {
                return this.m_TownStone;
            }
        }
    }
}