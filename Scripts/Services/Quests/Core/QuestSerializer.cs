using System;

namespace Server.Engines.Quests
{
    public class QuestSerializer
    {
        public static object Construct(Type type)
        {
            try
            {
                return Activator.CreateInstance(type);
            }
            catch
            {
                return null;
            }
        }

        public static void Write(Type type, Type[] referenceTable, GenericWriter writer)
        {
            if (type == null)
            {
                writer.WriteEncodedInt((int)0x00);
            }
            else
            {
                for (int i = 0; i < referenceTable.Length; ++i)
                {
                    if (referenceTable[i] == type)
                    {
                        writer.WriteEncodedInt((int)0x01);
                        writer.WriteEncodedInt((int)i);
                        return;
                    }
                }

                writer.WriteEncodedInt((int)0x02);
                writer.Write(type.FullName);
            }
        }

        public static Type ReadType(Type[] referenceTable, GenericReader reader)
        {
            int encoding = reader.ReadEncodedInt();

            switch ( encoding )
            {
                default:
                case 0x00: // null
                    {
                        return null;
                    }
                case 0x01: // indexed
                    {
                        int index = reader.ReadEncodedInt();

                        if (index >= 0 && index < referenceTable.Length)
                            return referenceTable[index];

                        return null;
                    }
                case 0x02: // by name
                    {
                        string fullName = reader.ReadString();

                        if (fullName == null)
                            return null;

                        return ScriptCompiler.FindTypeByFullName(fullName, false);
                    }
            }
        }

        public static QuestSystem DeserializeQuest(GenericReader reader)
        {
            int encoding = reader.ReadEncodedInt();

            switch ( encoding )
            {
                default:
                case 0x00: // null
                    {
                        return null;
                    }
                case 0x01:
                    {
                        Type type = ReadType(QuestSystem.QuestTypes, reader);

                        QuestSystem qs = Construct(type) as QuestSystem;

                        if (qs != null)
                            qs.BaseDeserialize(reader);

                        return qs;
                    }
            }
        }

        public static void Serialize(QuestSystem qs, GenericWriter writer)
        {
            if (qs == null)
            {
                writer.WriteEncodedInt(0x00);
            }
            else
            {
                writer.WriteEncodedInt(0x01);

                Write(qs.GetType(), QuestSystem.QuestTypes, writer);

                qs.BaseSerialize(writer);
            }
        }

        public static QuestObjective DeserializeObjective(Type[] referenceTable, GenericReader reader)
        {
            int encoding = reader.ReadEncodedInt();

            switch ( encoding )
            {
                default:
                case 0x00: // null
                    {
                        return null;
                    }
                case 0x01:
                    {
                        Type type = ReadType(referenceTable, reader);

                        QuestObjective obj = Construct(type) as QuestObjective;

                        if (obj != null)
                            obj.BaseDeserialize(reader);

                        return obj;
                    }
            }
        }

        public static void Serialize(Type[] referenceTable, QuestObjective obj, GenericWriter writer)
        {
            if (obj == null)
            {
                writer.WriteEncodedInt(0x00);
            }
            else
            {
                writer.WriteEncodedInt(0x01);

                Write(obj.GetType(), referenceTable, writer);

                obj.BaseSerialize(writer);
            }
        }

        public static QuestConversation DeserializeConversation(Type[] referenceTable, GenericReader reader)
        {
            int encoding = reader.ReadEncodedInt();

            switch ( encoding )
            {
                default:
                case 0x00: // null
                    {
                        return null;
                    }
                case 0x01:
                    {
                        Type type = ReadType(referenceTable, reader);

                        QuestConversation conv = Construct(type) as QuestConversation;

                        if (conv != null)
                            conv.BaseDeserialize(reader);

                        return conv;
                    }
            }
        }

        public static void Serialize(Type[] referenceTable, QuestConversation conv, GenericWriter writer)
        {
            if (conv == null)
            {
                writer.WriteEncodedInt(0x00);
            }
            else
            {
                writer.WriteEncodedInt(0x01);

                Write(conv.GetType(), referenceTable, writer);

                conv.BaseSerialize(writer);
            }
        }
    }
}