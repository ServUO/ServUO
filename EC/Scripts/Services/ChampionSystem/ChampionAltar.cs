using System;
using Server.Items;

namespace Server.Engines.CannedEvil
{
    public class ChampionAltar : PentagramAddon
    {
        private ChampionSpawn m_Spawn;
        public ChampionAltar(ChampionSpawn spawn)
        {
            this.m_Spawn = spawn;
        }

        public ChampionAltar(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_Spawn != null)
                this.m_Spawn.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Spawn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Spawn = reader.ReadItem() as ChampionSpawn;

                        if (this.m_Spawn == null)
                            this.Delete();

                        break;
                    }
            }
        }
    }
}