using System;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
    public class QuestWriter
    { 
        public static void Quests(GenericWriter writer, List<BaseQuest> quests)
        {
            if (quests == null)
            {
                writer.Write((int)0);
                return;
            }
			
            writer.Write((int)quests.Count);
			
            for (int i = 0; i < quests.Count; i ++)
            {
                BaseQuest quest = quests[i];
				
                Type(writer, quest.GetType());
				
                quest.Serialize(writer);
            }
        }

        public static void Chains(GenericWriter writer, Dictionary<QuestChain, BaseChain> chains)
        {
            if (chains == null)
            {
                writer.Write((int)0);
                return;
            }
			
            writer.Write((int)chains.Count);
			
            foreach (KeyValuePair<QuestChain, BaseChain> pair in chains)
            { 
                writer.Write((int)pair.Key);
				
                Type(writer, pair.Value.CurrentQuest);
                Type(writer, pair.Value.Quester);
            }
        }

        public static void Object(GenericWriter writer, object obj)
        {
            if (writer == null)
                return;
				
            if (obj is int)
            {
                writer.Write((byte)0x1);
                writer.Write((int)obj);
            }
            else if (obj is String)
            {
                writer.Write((byte)0x2);
                writer.Write((String)obj);
            }
            else if (obj is Item)
            {
                writer.Write((byte)0x3);				
                writer.Write((Item)obj);
            }
            else if (obj is Mobile)
            {
                writer.Write((byte)0x4);				
                writer.Write((Mobile)obj);
            }
            else
            {
                writer.Write((byte)0x0); // invalid
            }
        }

        public static void Type(GenericWriter writer, Type type)
        {
            if (writer == null)
                return;
				
            writer.Write(type == null ? null : type.FullName);
        }
    }
}