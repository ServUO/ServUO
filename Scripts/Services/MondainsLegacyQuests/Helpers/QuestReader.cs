#region References
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Server.Engines.Quests
{
    public class QuestReader
    {
        public static int Version(GenericReader reader)
        {
            if (reader == null)
            {
                return -1;
            }

            if (reader.PeekInt() == 0x7FFFFFFF)
            {
                reader.ReadInt(); // Preamble 0x7FFFFFFF

                return reader.ReadInt();
            }

            return -1;
        }

        public static bool SubRead(GenericReader reader, Action<GenericReader> deserializer)
        {
            if (reader == null)
            {
                return false;
            }

            using (MemoryStream s = new MemoryStream())
            {
                long length = reader.ReadLong();

                while (s.Length < length)
                {
                    s.WriteByte(reader.ReadByte());
                }

                if (deserializer != null)
                {
                    s.Position = 0;

                    BinaryFileReader r = new BinaryFileReader(new BinaryReader(s));

                    try
                    {
                        deserializer(r);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Quest Load Failure: {0}", Utility.FormatDelegate(deserializer));
                        Diagnostics.ExceptionLogging.LogException(e);

                        return false;
                    }
                    finally
                    {
                        r.Close();
                    }
                }
            }

            return true;
        }

        public static List<BaseQuest> Quests(GenericReader reader, PlayerMobile player)
        {
            List<BaseQuest> quests = new List<BaseQuest>();

            if (reader == null)
            {
                return quests;
            }

            int version = Version(reader);

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                BaseQuest quest = Construct(reader) as BaseQuest;

                if (quest == null)
                {
                    if (version >= 0)
                    {
                        SubRead(reader, null);
                    }

                    continue;
                }

                quest.Owner = player;

                if (version < 0)
                {
                    quest.Deserialize(reader);
                }
                else if (!SubRead(reader, quest.Deserialize))
                {
                    continue;
                }

                quests.Add(quest);
            }

            return quests;
        }

        public static Dictionary<QuestChain, BaseChain> Chains(GenericReader reader)
        {
            Dictionary<QuestChain, BaseChain> chains = new Dictionary<QuestChain, BaseChain>();

            if (reader == null)
            {
                return chains;
            }

            Version(reader);

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                int chain = reader.ReadInt();
                Type quest = Type(reader);
                Type quester = Type(reader);

                if (Enum.IsDefined(typeof(QuestChain), chain) && quest != null && quester != null)
                {
                    chains[(QuestChain)chain] = new BaseChain(quest, quester);
                }
            }

            return chains;
        }

        public static object Object(GenericReader reader)
        {
            if (reader == null)
            {
                return null;
            }

            Version(reader);

            byte type = reader.ReadByte();

            switch (type)
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
            {
                return null;
            }

            string type = reader.ReadString();

            if (type != null)
            {
                return ScriptCompiler.FindTypeByFullName(type, false);
            }

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
