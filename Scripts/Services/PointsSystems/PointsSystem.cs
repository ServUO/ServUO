using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Linq;
using System.Collections.Generic;

namespace Server.Engines.Points
{
    public enum PointsType
    {
        QueensLoyalty,
        VoidPool,
        DespiseCrystals,
        ShameCrystals,
        CasinoData
    }

    public abstract class PointsSystem
    {
        public const string FilePath = @"Saves\\PointsSystem\\Persistence.bin";

        public Dictionary<PlayerMobile, double> PlayerTable { get; set; }

        public abstract TextDefinition Name { get; }
        public abstract PointsType Loyalty { get; }
        public abstract bool AutoAdd { get; }
        public abstract double MaxPoints { get; }

        public virtual bool ShowOnLoyaltyGump { get { return true; } }

        public PointsSystem()
        {
            PlayerTable = new Dictionary<PlayerMobile, double>();

            Systems.Add(this);
        }

        public virtual void ProcessKill(BaseCreature victim, Mobile damager, int index)
        {
        }

        public virtual void ProcessQuest(Mobile from, Server.Engines.Quests.BaseQuest quest)
        {
        }

        public virtual void ConvertFromOldSystem(PlayerMobile from, double points)
        {
            if (!PlayerTable.ContainsKey(from))
            {
                if (points > MaxPoints)
                    points = MaxPoints;

                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine("Converted {0} points for {1} to {2}!", (int)points, from.Name, this.GetType().Name);
                Utility.PopColor();

                PlayerTable[from] = points;
            }
        }

        public virtual void AwardPoints(Mobile from, double points, bool quest = false, bool message = true)
        {
            if (!(from is PlayerMobile) || points <= 0)
                return;

            PlayerMobile pm = from as PlayerMobile;

            if (!PlayerTable.ContainsKey(pm))
            {
                if (!AutoAdd)
                    return;

                PlayerTable[pm] = 0;
                OnPlayerAdded(pm);
            }

            double old = PlayerTable[pm];
            SendMessage(pm, old, points, quest);

            SetPoints(pm, Math.Min(MaxPoints, PlayerTable[pm] + points));
        }

        public void SetPoints(PlayerMobile pm, double points)
        {
            if(PlayerTable.ContainsKey(pm))
                PlayerTable[pm] = points;
        }

        public virtual void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
            if (quest)
                from.SendLocalizedMessage(1113719, ((int)points).ToString(), 0x26); //You have received ~1_val~ loyalty points as a reward for completing the quest. 
            else
                from.SendLocalizedMessage(1115920, String.Format("{0}\t{1}", Name.ToString(), ((int)points).ToString()));  // Your loyalty to ~1_GROUP~ has increased by ~2_AMOUNT~;Original
        }

        public virtual bool DeductPoints(Mobile from, double points, bool message = false)
        {
            if (!(from is PlayerMobile) || !PlayerTable.ContainsKey((PlayerMobile)from) || PlayerTable[(PlayerMobile)from] < points)
            {
                return false;
            }
            else
            {
                PlayerTable[(PlayerMobile)from] -= points;

                if (message)
                    from.SendLocalizedMessage(1115921, String.Format("{0}\t{1}", Name.ToString(), ((int)points).ToString()));  // Your loyalty to ~1_GROUP~ has decreased by ~2_AMOUNT~;Original
            }

            return true;
        }

        public virtual void OnPlayerAdded(PlayerMobile pm)
        {
        }

        public double GetPoints(Mobile from)
        {
            if (from is PlayerMobile && PlayerTable.ContainsKey(from as PlayerMobile))
                return PlayerTable[(PlayerMobile)from];

            return 0.0;
        }

        public virtual TextDefinition GetTitle(PlayerMobile from)
        {
            return null;
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);

            writer.Write(PlayerTable.Count);

            foreach (KeyValuePair<PlayerMobile, double> kvp in PlayerTable)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            PlayerTable = new Dictionary<PlayerMobile, double>();

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                PlayerMobile pm = reader.ReadMobile() as PlayerMobile;
                double points = reader.ReadDouble();

                if (pm != null)
                    PlayerTable.Add(pm, points);
            }
        }

        #region Static Methods and Accessors
        public static PointsSystem GetSystemInstance(PointsType t)
        {
            return Systems.FirstOrDefault(s => s.Loyalty == t);
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write((int)0);

                    writer.Write(Systems.Count);
                    Systems.ForEach(s =>
                    {
                        writer.Write((int)s.Loyalty);
                        s.Serialize(writer);
                    });
                });
        }

        public static void OnLoad()
		{
			Persistence.Deserialize(
				FilePath,
				reader =>
				{
					int version = reader.ReadInt();
				
					int count = reader.ReadInt();
					for(int i = 0; i < count; i++)
					{
						PointsType type = (PointsType)reader.ReadInt();
						PointsSystem s = GetSystemInstance(type);
						s.Deserialize(reader);
					}	
				});
		}

        public static List<PointsSystem> Systems { get; set; }

        public static QueensLoyalty QueensLoyalty { get; set; }
        public static VoidPool VoidPool { get; set; }
        public static DespiseCrystals DespiseCrystals { get; set; }
        public static ShameCrystals ShameCrystals { get; set; }
        public static CasinoData CasinoData { get; set; }

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;

            Systems = new List<PointsSystem>();

            QueensLoyalty = new QueensLoyalty();
            VoidPool = new VoidPool();
            DespiseCrystals = new DespiseCrystals();
            ShameCrystals = new ShameCrystals();
            CasinoData = new CasinoData();
        }

        public static void HandleKill(BaseCreature victim, Mobile damager, int index)
        {
            Systems.ForEach(s => s.ProcessKill(victim, damager, index));
        }

        public static void HandleQuest(Mobile from, Server.Engines.Quests.BaseQuest quest)
        {
            Systems.ForEach(s => s.ProcessQuest(from, quest));
        }
        #endregion
    }
}