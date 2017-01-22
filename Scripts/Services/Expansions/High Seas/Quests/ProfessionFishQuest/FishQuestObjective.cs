using Server;
using System;
using System.Collections.Generic;
using Server.Multis;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class FishQuestObjective : BaseObjective
    {
        public Dictionary<Type, int[]> Line { get; set; }

        public bool IsCompleted
        {
            get
            {
                foreach (KeyValuePair<Type, int[]> kvp in Line)
                {
                    if (kvp.Value[0] < kvp.Value[1])
                        return false;
                }

                return true;
            }
        }

        public FishQuestObjective()
        {
        }

        public FishQuestObjective(Dictionary<Type, int> line)
            : base(500, 0)
        {
            Line = new Dictionary<Type, int[]>();

            foreach (KeyValuePair<Type, int> kvp in line)
            {
                Line[kvp.Key] = new int[] { 0, kvp.Value };
            }
        }

        public override bool Update(object obj)
        {
            if (obj is Item)
            {
                Item item = (Item)obj;

                foreach (KeyValuePair<Type, int[]> kvp in Line)
                {
                    if (item.GetType() == kvp.Key)
                    {
                        kvp.Value[0] += item.Amount;

                        if (IsCompleted && Quest.Owner != null)
                        {
                            CurProgress = 500;
                            Quest.Owner.SendLocalizedMessage(1072273, null, 0x23); // You've completed a quest!  Don't forget to collect your reward.							
                            Quest.Owner.SendSound(Quest.CompleteSound);
                        }
                        else
                            Quest.Owner.SendSound(Quest.UpdateSound);

                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckLift(Item item)
        {
            return item != null && Line.ContainsKey(item.GetType());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(Line.Count);
            foreach (KeyValuePair<Type, int[]> kvp in Line)
            {
                writer.Write(FishQuestHelper.GetIndexForType(kvp.Key));
                writer.Write(kvp.Value[0]);
                writer.Write(kvp.Value[1]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            int count = reader.ReadInt();
            Line = new Dictionary<Type, int[]>();

            for (int i = 0; i < count; i++)
            {
                Type type = FishQuestHelper.GetTypeFromIndex(reader.ReadInt());
                int[] line = new int[] { reader.ReadInt(), reader.ReadInt() };

                if (type != null)
                    Line[type] = line;
            }
        }
    }
}