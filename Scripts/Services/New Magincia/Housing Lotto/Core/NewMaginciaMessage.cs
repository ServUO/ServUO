using Server;
using System;

namespace Server.Engines.NewMagincia
{
    public class NewMaginciaMessage
    {
        public static readonly TimeSpan ExpirePeriod = TimeSpan.FromDays(7);

        private TextDefinition m_Title;
        private TextDefinition m_Body;
        private string m_Args;
        private DateTime m_Created;

        public TextDefinition Title { get { return m_Title; } }
        public TextDefinition Body { get { return m_Body; } }
        public string Args { get { return m_Args; } }
        public DateTime Created { get { return m_Created; } }

        public bool Expired { get { return m_Created + ExpirePeriod < DateTime.UtcNow; } }

        public NewMaginciaMessage(TextDefinition title, TextDefinition body, string args)
        {
            m_Title = title;
            m_Body = body;
            m_Args = args;
            m_Created = DateTime.UtcNow;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);

            TextDefinition.Serialize(writer, m_Title);
            TextDefinition.Serialize(writer, m_Body);
            writer.Write(m_Created);
            writer.Write(m_Args);
        }

        public NewMaginciaMessage(GenericReader reader)
        {
            int v = reader.ReadInt();

            m_Title = TextDefinition.Deserialize(reader);
            m_Body = TextDefinition.Deserialize(reader);
            m_Created = reader.ReadDateTime();
            m_Args = reader.ReadString();
        }
    }
}