using System;
using System.IO;
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
        public static string FilePath = Path.Combine("Saves/PointsSystem", "Persistence.bin");

        public List<PointsEntry> PlayerTable { get; set; }

        public abstract TextDefinition Name { get; }
        public abstract PointsType Loyalty { get; }
        public abstract bool AutoAdd { get; }
        public abstract double MaxPoints { get; }

        public virtual bool ShowOnLoyaltyGump { get { return true; } }

        public PointsSystem()
        {
            PlayerTable = new List<PointsEntry>();

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
            PointsEntry entry = GetEntry(from, false);

            if (entry == null)
            {
                if (points > MaxPoints)
                    points = MaxPoints;

                AddEntry(from);
                GetEntry(from).Points = points;

                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine("Converted {0} points for {1} to {2}!", (int)points, from.Name, this.GetType().Name);
                Utility.PopColor();
            }
        }

        public virtual void AwardPoints(Mobile from, double points, bool quest = false, bool message = true)
        {
            if (!(from is PlayerMobile) || points <= 0)
                return;

            PointsEntry entry = GetEntry(from);

            if (entry != null)
            {
                double old = entry.Points;
                SendMessage((PlayerMobile)from, old, points, quest);

                SetPoints((PlayerMobile)from, Math.Min(MaxPoints, entry.Points + points));
            }
        }

        public void SetPoints(PlayerMobile pm, double points)
        {
            PointsEntry entry = GetEntry(pm);

            if(entry != null)
                entry.Points = points;
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
            PointsEntry entry = GetEntry(from);

            if (entry == null || entry.Points < points)
            {
                return false;
            }
            else
            {
                entry.Points -= points;

                if (message)
                    from.SendLocalizedMessage(1115921, String.Format("{0}\t{1}", Name.ToString(), ((int)points).ToString()));  // Your loyalty to ~1_GROUP~ has decreased by ~2_AMOUNT~;Original
            }

            return true;
        }

        public virtual void OnPlayerAdded(PlayerMobile pm)
        {
        }

        public virtual PointsEntry AddEntry(PlayerMobile pm)
        {
            PointsEntry entry = GetSystemEntry(pm);

            if (!PlayerTable.Contains(entry))
            {
                PlayerTable.Add(entry);
                OnPlayerAdded(pm);
            }

            return entry;
        }

        public double GetPoints(Mobile from)
        {
            PointsEntry entry = GetEntry(from);

            if (entry != null)
                return entry.Points;

            return 0.0;
        }

        public virtual TextDefinition GetTitle(PlayerMobile from)
        {
            return null;
        }

        public PointsEntry GetEntry(Mobile from, bool create = false)
		{
            if (from == null)
                return null;

            PointsEntry entry = PlayerTable.FirstOrDefault(e => e.Player == from);

            if (entry == null && AutoAdd && from is PlayerMobile)
                entry = AddEntry((PlayerMobile)from);
				
			return entry;
		}

        public TEntry GetPlayerEntry<TEntry>(PlayerMobile pm) where TEntry : PointsEntry
        {
            if (pm == null)
                return null;

            return PlayerTable.FirstOrDefault(p => p.Player == pm) as TEntry;
        }

        /// <summary>
        /// Override this if you are going to derive Points Entry into a bigger and badder class!
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        public virtual PointsEntry GetSystemEntry(PlayerMobile pm)
        {
            return new PointsEntry(pm);
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write((int)1);

            writer.Write(PlayerTable.Count);

            PlayerTable.ForEach(entry =>
                {
                    writer.Write(entry.Player);
                    entry.Serialize(writer);
                });
            // Old version 0, now is stored in PointsEntry for more flexibility in newer systems
            /*writer.Write(PlayerTable.Count);

            foreach (KeyValuePair<PlayerMobile, double> kvp in PlayerTable)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }*/
        }

        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                case 1:
                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        PlayerMobile player = reader.ReadMobile() as PlayerMobile;
 
                        PointsEntry entry = GetSystemEntry(player);

                        if (version > 0)
                            entry.Deserialize(reader);
                        else
                            entry.Points = reader.ReadDouble();

                        if (player != null)
                            PlayerTable.Add(entry);
                    }
                    break;
            }

            //Old player data, not uses the above
            /*PlayerTable = new Dictionary<PlayerMobile, double>();

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                PlayerMobile pm = reader.ReadMobile() as PlayerMobile;
                double points = reader.ReadDouble();

                if (pm != null)
                    PlayerTable.Add(pm, points);
            }*/
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

    public class PointsEntry
	{
		public PlayerMobile Player { get; set; }
		public double Points { get; set ; }

        public PointsEntry(PlayerMobile pm)
        {
            Player = pm;
        }

		public PointsEntry(PlayerMobile pm, double points)
		{
			Player = pm;
			Points = points;
		}
		
		public virtual void Serialize(GenericWriter writer)
		{
			writer.Write(0);
			writer.Write(Player);
			writer.Write(Points);
		}
		
		public virtual void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();
			Player = reader.ReadMobile() as PlayerMobile;
			Points = reader.ReadDouble();
		}
	}
}