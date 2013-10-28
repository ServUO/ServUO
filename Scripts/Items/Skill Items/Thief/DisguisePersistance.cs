using System;
using System.Collections;

namespace Server.Items
{
    public class DisguisePersistance : Item
    {
        private static DisguisePersistance m_Instance;
        public DisguisePersistance()
            : base(1)
        {
            this.Movable = false;
			
            if (m_Instance == null || m_Instance.Deleted)
                m_Instance = this;
            else
                base.Delete();
        }

        public DisguisePersistance(Serial serial)
            : base(serial)
        {
            m_Instance = this;
        }

        public static DisguisePersistance Instance
        {
            get
            {
                return m_Instance;
            }
        }
        public override string DefaultName
        {
            get
            {
                return "Disguise Persistance - Internal";
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
			
            int timerCount = DisguiseTimers.Timers.Count;
			
            writer.Write(timerCount);
				
            foreach (DictionaryEntry entry in DisguiseTimers.Timers)
            {
                Mobile m = (Mobile)entry.Key;
				
                writer.Write(m);
                writer.Write(((Timer)entry.Value).Next - DateTime.UtcNow);
                writer.Write(m.NameMod);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        int count = reader.ReadInt();
									
                        for (int i = 0; i < count; ++i)
                        {
                            Mobile m = reader.ReadMobile();
                            DisguiseTimers.CreateTimer(m, reader.ReadTimeSpan());
                            m.NameMod = reader.ReadString();
                        }

                        break;
                    }
            }
        }

        public override void Delete()
        {
        }
    }
}