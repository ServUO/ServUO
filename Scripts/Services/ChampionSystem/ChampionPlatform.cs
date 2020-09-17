using Server.Items;
using System;

namespace Server.Engines.CannedEvil
{
    public class ChampionPlatform : BaseAddon
    {
        private ChampionSpawn m_Spawn;
        public ChampionPlatform(ChampionSpawn spawn)
        {
            m_Spawn = spawn;

            for (int x = -2; x <= 2; ++x)
                for (int y = -2; y <= 2; ++y)
                    AddComponent(0x3EE, x, y, -5);

            for (int x = -1; x <= 1; ++x)
                for (int y = -1; y <= 1; ++y)
                    AddComponent(0x3EE, x, y, 0);

            for (int i = -1; i <= 1; ++i)
            {
                AddComponent(0x3EF, i, 2, 0);
                AddComponent(0x3F0, 2, i, 0);

                AddComponent(0x3F1, i, -2, 0);
                AddComponent(0x3F2, -2, i, 0);
            }

            AddComponent(0x03F7, -2, -2, 0);
            AddComponent(0x03F8, 2, 2, 0);
            AddComponent(0x03F9, -2, 2, 0);
            AddComponent(0x03FA, 2, -2, 0);
        }

        public ChampionPlatform(Serial serial)
            : base(serial)
        {
        }

        public void AddComponent(int id, int x, int y, int z)
        {
            AddonComponent ac = new AddonComponent(id)
            {
                Hue = 0x452
            };

            AddComponent(ac, x, y, z);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Spawn != null)
                m_Spawn.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version

            writer.Write(m_Spawn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                case 1:
                case 0:
                    {
                        m_Spawn = reader.ReadItem() as ChampionSpawn;

                        if (m_Spawn == null)
                            Delete();

                        break;
                    }
            }

            if (version < 2)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(10), FixComponents);
            }
        }

        private void FixComponents()
        {
            foreach (AddonComponent comp in Components)
            {
                comp.Hue = 0x452;

                if (comp.ItemID == 0x750)
                    comp.ItemID = 0x3EE;

                if (comp.ItemID == 0x751)
                    comp.ItemID = 0x3EF;

                if (comp.ItemID == 0x752)
                    comp.ItemID = 0x3F0;

                if (comp.ItemID == 0x753)
                    comp.ItemID = 0x3F1;

                if (comp.ItemID == 0x754)
                    comp.ItemID = 0x3F2;

                if (comp.ItemID == 0x759)
                    comp.ItemID = 0x3F7;

                if (comp.ItemID == 0x75A)
                    comp.ItemID = 0x3F8;

                if (comp.ItemID == 0x75B)
                    comp.ItemID = 0x3F9;

                if (comp.ItemID == 0x75C)
                    comp.ItemID = 0x3FA;
            }
        }
    }
}