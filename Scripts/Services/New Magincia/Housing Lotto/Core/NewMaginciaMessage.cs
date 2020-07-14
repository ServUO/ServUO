using System;

namespace Server.Engines.NewMagincia
{
    public class NewMaginciaMessage
    {
        public static readonly TimeSpan DefaultExpirePeriod = TimeSpan.FromDays(7);

        private readonly TextDefinition m_Title;
        private readonly TextDefinition m_Body;
        private readonly string m_Args;
        private readonly DateTime m_Expires;
        private readonly bool m_AccountBound;

        public TextDefinition Title => m_Title;
        public TextDefinition Body => m_Body;
        public string Args => m_Args;
        public DateTime Expires => m_Expires;

        public bool AccountBound => m_AccountBound;

        public bool Expired => m_Expires < DateTime.UtcNow;

        public NewMaginciaMessage(TextDefinition title, TextDefinition body)
            : this(title, body, DefaultExpirePeriod, null, false)
        {
        }

        public NewMaginciaMessage(TextDefinition title, TextDefinition body, bool accountBound)
            : this(title, body, DefaultExpirePeriod, null, accountBound)
        {
        }

        public NewMaginciaMessage(TextDefinition title, TextDefinition body, string args)
            : this(title, body, DefaultExpirePeriod, args, false)
        {
        }

        public NewMaginciaMessage(TextDefinition title, TextDefinition body, string args, bool accountBound)
            : this(title, body, DefaultExpirePeriod, args, accountBound)
        {
        }

        public NewMaginciaMessage(TextDefinition title, TextDefinition body, TimeSpan expires)
            : this(title, body, expires, null, false)
        {
        }

        public NewMaginciaMessage(TextDefinition title, TextDefinition body, TimeSpan expires, bool accountBound)
            : this(title, body, expires, null, accountBound)
        {
        }

        public NewMaginciaMessage(TextDefinition title, TextDefinition body, TimeSpan expires, string args)
            : this(title, body, expires, args, false)
        {
        }

        public NewMaginciaMessage(TextDefinition title, TextDefinition body, TimeSpan expires, string args, bool accountBound)
        {
            m_Title = title;
            m_Body = body;
            m_Args = args;
            m_Expires = DateTime.UtcNow + expires;

            m_AccountBound = accountBound;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(1);

            writer.Write(m_AccountBound);
            TextDefinition.Serialize(writer, m_Title);
            TextDefinition.Serialize(writer, m_Body);
            writer.Write(m_Expires);
            writer.Write(m_Args);
        }

        public NewMaginciaMessage(GenericReader reader)
        {
            int v = reader.ReadInt();

            switch (v)
            {
                case 1:
                    m_AccountBound = reader.ReadBool();
                    goto case 0;
                case 0:
                    m_Title = TextDefinition.Deserialize(reader);
                    m_Body = TextDefinition.Deserialize(reader);
                    m_Expires = reader.ReadDateTime();
                    m_Args = reader.ReadString();
                    break;
            }
        }
    }
}
