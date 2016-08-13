using System;
using System.Collections.Generic;

namespace Server.Engines.Harvest
{
    public class HarvestDefinition
    {
        private int m_BankWidth, m_BankHeight;
        private int m_MinTotal, m_MaxTotal;
        private int[] m_Tiles;
        private int[] m_SpecialTiles;
        private bool m_RangedTiles;
        private TimeSpan m_MinRespawn, m_MaxRespawn;
        private int m_MaxRange;
        private int m_ConsumedPerHarvest, m_ConsumedPerFeluccaHarvest;
        private bool m_PlaceAtFeetIfFull;
        private SkillName m_Skill;
        private int[] m_EffectActions;
        private int[] m_EffectCounts;
        private int[] m_EffectSounds;
        private TimeSpan m_EffectSoundDelay;
        private TimeSpan m_EffectDelay;
        private object m_NoResourcesMessage, m_OutOfRangeMessage, m_TimedOutOfRangeMessage, m_DoubleHarvestMessage, m_FailMessage, m_PackFullMessage, m_ToolBrokeMessage;
        private HarvestResource[] m_Resources;
        private HarvestVein[] m_Veins;
        private BonusHarvestResource[] m_BonusResources;
        private bool m_RaceBonus;
        private bool m_RandomizeVeins;
        private Dictionary<Map, Dictionary<Point2D, HarvestBank>> m_BanksByMap;
        public HarvestDefinition()
        {
            this.m_BanksByMap = new Dictionary<Map, Dictionary<Point2D, HarvestBank>>();
        }

        public int BankWidth
        {
            get
            {
                return this.m_BankWidth;
            }
            set
            {
                this.m_BankWidth = value;
            }
        }
        public int BankHeight
        {
            get
            {
                return this.m_BankHeight;
            }
            set
            {
                this.m_BankHeight = value;
            }
        }
        public int MinTotal
        {
            get
            {
                return this.m_MinTotal;
            }
            set
            {
                this.m_MinTotal = value;
            }
        }
        public int MaxTotal
        {
            get
            {
                return this.m_MaxTotal;
            }
            set
            {
                this.m_MaxTotal = value;
            }
        }
        public int[] Tiles
        {
            get
            {
                return this.m_Tiles;
            }
            set
            {
                this.m_Tiles = value;
            }
        }
        public int[] SpecialTiles 
        {
            get
            { 
                return m_SpecialTiles;
            } 
            set
            { 
                m_SpecialTiles = value;
            }
        }
        public bool RangedTiles
        {
            get
            {
                return this.m_RangedTiles;
            }
            set
            {
                this.m_RangedTiles = value;
            }
        }
        public TimeSpan MinRespawn
        {
            get
            {
                return this.m_MinRespawn;
            }
            set
            {
                this.m_MinRespawn = value;
            }
        }
        public TimeSpan MaxRespawn
        {
            get
            {
                return this.m_MaxRespawn;
            }
            set
            {
                this.m_MaxRespawn = value;
            }
        }
        public int MaxRange
        {
            get
            {
                return this.m_MaxRange;
            }
            set
            {
                this.m_MaxRange = value;
            }
        }
        public int ConsumedPerHarvest
        {
            get
            {
                return this.m_ConsumedPerHarvest;
            }
            set
            {
                this.m_ConsumedPerHarvest = value;
            }
        }
        public int ConsumedPerFeluccaHarvest
        {
            get
            {
                return this.m_ConsumedPerFeluccaHarvest;
            }
            set
            {
                this.m_ConsumedPerFeluccaHarvest = value;
            }
        }
        public bool PlaceAtFeetIfFull
        {
            get
            {
                return this.m_PlaceAtFeetIfFull;
            }
            set
            {
                this.m_PlaceAtFeetIfFull = value;
            }
        }
        public SkillName Skill
        {
            get
            {
                return this.m_Skill;
            }
            set
            {
                this.m_Skill = value;
            }
        }
        public int[] EffectActions
        {
            get
            {
                return this.m_EffectActions;
            }
            set
            {
                this.m_EffectActions = value;
            }
        }
        public int[] EffectCounts
        {
            get
            {
                return this.m_EffectCounts;
            }
            set
            {
                this.m_EffectCounts = value;
            }
        }
        public int[] EffectSounds
        {
            get
            {
                return this.m_EffectSounds;
            }
            set
            {
                this.m_EffectSounds = value;
            }
        }
        public TimeSpan EffectSoundDelay
        {
            get
            {
                return this.m_EffectSoundDelay;
            }
            set
            {
                this.m_EffectSoundDelay = value;
            }
        }
        public TimeSpan EffectDelay
        {
            get
            {
                return this.m_EffectDelay;
            }
            set
            {
                this.m_EffectDelay = value;
            }
        }
        public object NoResourcesMessage
        {
            get
            {
                return this.m_NoResourcesMessage;
            }
            set
            {
                this.m_NoResourcesMessage = value;
            }
        }
        public object OutOfRangeMessage
        {
            get
            {
                return this.m_OutOfRangeMessage;
            }
            set
            {
                this.m_OutOfRangeMessage = value;
            }
        }
        public object TimedOutOfRangeMessage
        {
            get
            {
                return this.m_TimedOutOfRangeMessage;
            }
            set
            {
                this.m_TimedOutOfRangeMessage = value;
            }
        }
        public object DoubleHarvestMessage
        {
            get
            {
                return this.m_DoubleHarvestMessage;
            }
            set
            {
                this.m_DoubleHarvestMessage = value;
            }
        }
        public object FailMessage
        {
            get
            {
                return this.m_FailMessage;
            }
            set
            {
                this.m_FailMessage = value;
            }
        }
        public object PackFullMessage
        {
            get
            {
                return this.m_PackFullMessage;
            }
            set
            {
                this.m_PackFullMessage = value;
            }
        }
        public object ToolBrokeMessage
        {
            get
            {
                return this.m_ToolBrokeMessage;
            }
            set
            {
                this.m_ToolBrokeMessage = value;
            }
        }
        public HarvestResource[] Resources
        {
            get
            {
                return this.m_Resources;
            }
            set
            {
                this.m_Resources = value;
            }
        }
        public HarvestVein[] Veins
        {
            get
            {
                return this.m_Veins;
            }
            set
            {
                this.m_Veins = value;
            }
        }
        public BonusHarvestResource[] BonusResources
        {
            get
            {
                return this.m_BonusResources;
            }
            set
            {
                this.m_BonusResources = value;
            }
        }
        public bool RaceBonus
        {
            get
            {
                return this.m_RaceBonus;
            }
            set
            {
                this.m_RaceBonus = value;
            }
        }
        public bool RandomizeVeins
        {
            get
            {
                return this.m_RandomizeVeins;
            }
            set
            {
                this.m_RandomizeVeins = value;
            }
        }
        public Dictionary<Map, Dictionary<Point2D, HarvestBank>> Banks
        {
            get
            {
                return this.m_BanksByMap;
            }
            set
            {
                this.m_BanksByMap = value;
            }
        }
        public void SendMessageTo(Mobile from, object message)
        {
            if (message is int)
                from.SendLocalizedMessage((int)message);
            else if (message is string)
                from.SendMessage((string)message);
        }

        public HarvestBank GetBank(Map map, int x, int y)
        {
            if (map == null || map == Map.Internal)
                return null;

            x /= this.m_BankWidth;
            y /= this.m_BankHeight;

            Dictionary<Point2D, HarvestBank> banks = null;
            this.m_BanksByMap.TryGetValue(map, out banks);

            if (banks == null)
                this.m_BanksByMap[map] = banks = new Dictionary<Point2D, HarvestBank>();

            Point2D key = new Point2D(x, y);
            HarvestBank bank = null;
            banks.TryGetValue(key, out bank);

            if (bank == null)
                banks[key] = bank = new HarvestBank(this, this.GetVeinAt(map, x, y));

            return bank;
        }

        public HarvestVein GetVeinAt(Map map, int x, int y)
        {
            if (this.m_Veins.Length == 1)
                return this.m_Veins[0];

            double randomValue;

            if (this.m_RandomizeVeins)
            {
                randomValue = Utility.RandomDouble();
            }
            else
            {
                Random random = new Random((x * 17) + (y * 11) + (map.MapID * 3));
                randomValue = random.NextDouble();
            }

            return this.GetVeinFrom(randomValue);
        }

        public HarvestVein GetVeinFrom(double randomValue)
        {
            if (this.m_Veins.Length == 1)
                return this.m_Veins[0];

            randomValue *= 100;

            for (int i = 0; i < this.m_Veins.Length; ++i)
            {
                if (randomValue <= this.m_Veins[i].VeinChance)
                    return this.m_Veins[i];

                randomValue -= this.m_Veins[i].VeinChance;
            }

            return null;
        }

        public BonusHarvestResource GetBonusResource()
        {
            if (this.m_BonusResources == null)
                return null;

            double randomValue = Utility.RandomDouble() * 100;

            for (int i = 0; i < this.m_BonusResources.Length; ++i)
            {
                if (randomValue <= this.m_BonusResources[i].Chance)
                    return this.m_BonusResources[i];

                randomValue -= this.m_BonusResources[i].Chance;
            }

            return null;
        }

        public bool Validate(int tileID)
        {
            if (this.m_RangedTiles)
            {
                bool contains = false;

                for (int i = 0; !contains && i < this.m_Tiles.Length; i += 2)
                    contains = (tileID >= this.m_Tiles[i] && tileID <= this.m_Tiles[i + 1]);

                return contains;
            }
            else
            {
                int dist = -1;

                for (int i = 0; dist < 0 && i < this.m_Tiles.Length; ++i)
                    dist = (this.m_Tiles[i] - tileID);

                return (dist == 0);
            }
        }

        #region High Seas
        public bool ValidateSpecial(int tileID)
        {
            //No Special tiles were initiated so always true
            if (m_SpecialTiles == null || m_SpecialTiles.Length == 0)
                return true;

            for (int i = 0; i < m_SpecialTiles.Length; i++)
            {
                if (tileID == m_SpecialTiles[i])
                    return true;
            }

            return false;
        }
        #endregion
    }
}