using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class QuestReader
    { 
        public static List<BaseQuest> Quests(GenericReader reader, PlayerMobile player)
        {
            List<BaseQuest> quests = new List<BaseQuest>();
			
            int count = reader.ReadInt();
			
            for (int i = 0; i < count; i ++)
            {
                BaseQuest quest = Construct(reader) as BaseQuest;
				
                if (quest == null)
                    continue;
					
                quest.Owner = player;
                quest.Deserialize(reader);
				
                quests.Add(quest);
            }
			
            return quests;
        }

        public static Dictionary<QuestChain, BaseChain> Chains(GenericReader reader)
        {
            Dictionary<QuestChain, BaseChain> chains = new Dictionary<QuestChain, BaseChain>();
			
            int count = reader.ReadInt();
			
            for (int i = 0; i < count; i ++)
                chains.Add((QuestChain)reader.ReadInt(), new BaseChain(Type(reader), Type(reader)));
			
            return chains;
        }

        public static object Object(GenericReader reader)
        { 
            if (reader == null)
                return null;
					
            byte type = reader.ReadByte();
			
            switch ( type )
            {
                case 0x0:
                    return null; // invalid
                case 0x1:
                    return reader.ReadInt();
                case 0x2:
                    return reader.ReadString();
                case 0x3:
                    return reader.ReadItem();
                case 0x4:
                    return reader.ReadMobile();
            }
			
            return null;
        }

        public static Type Type(GenericReader reader)
        {
            if (reader == null)
                return null;
				
            string type = reader.ReadString();

            if (type != null)
                return ScriptCompiler.FindTypeByFullName(type, false);
            else
                return null;
        }

        public static object Construct(GenericReader reader)
        {
            Type type = Type(reader);
			
            try
            {
                return Activator.CreateInstance(type);
            }
            catch
            {
                return null;
            }
        }
    }
}