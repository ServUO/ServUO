using System;

namespace Server.Ethics
{
    [TypeAlias("Server.Factions.EthicsPersistance")]
    public class EthicsPersistence : Item
    {
        private static EthicsPersistence m_Instance;
        [Constructable]
        public EthicsPersistence()
            : base(1)
        {
            this.Movable = false;

            if (m_Instance == null || m_Instance.Deleted)
                m_Instance = this;
            else
                base.Delete();
        }

        public EthicsPersistence(Serial serial)
            : base(serial)
        {
            m_Instance = this;
        }

        public static EthicsPersistence Instance
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
                return "Ethics Persistence - Internal";
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            for (int i = 0; i < Ethics.Ethic.Ethics.Length; ++i)
                Ethics.Ethic.Ethics[i].Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        for (int i = 0; i < Ethics.Ethic.Ethics.Length; ++i)
                            Ethics.Ethic.Ethics[i].Deserialize(reader);

                        break;
                    }
            }
        }

        public override void Delete()
        {
        }
    }
}