using System;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Gumps;
namespace Server.Engines.Quests
{
    public class CollectionsObtainObjective : ObtainObjective
    {
        private bool m_HasObtained;

        public bool HasObtained 
        { 
            get { return m_HasObtained; } 
            set { m_HasObtained = true; } 
        }

        public CollectionsObtainObjective(Type obtain, string name, int amount) : base(obtain, name, amount)
        {
            m_HasObtained = false;
        }

        public override bool Update(object o)
        {
            if (this.Quest == null || this.Quest.Owner == null)
                return false;

            if (m_HasObtained)
                return base.Update(o);

            return false;
        }

        public static void CheckReward(PlayerMobile pm, Item item)
        {
            if (pm.Quests != null)
            {
                foreach (BaseQuest q in pm.Quests)
                {
                    foreach (BaseObjective obj in q.Objectives)
                    {
                        if (obj is CollectionsObtainObjective && ((CollectionsObtainObjective)obj).Obtain == item.GetType())
                        {
                            ((CollectionsObtainObjective)obj).HasObtained = true;
                            pm.SendSound(q.UpdateSound);
                            return;
                        }
                    }
                }
            }
        }
 
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write(m_HasObtained);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_HasObtained = reader.ReadBool();
        }
    }
}