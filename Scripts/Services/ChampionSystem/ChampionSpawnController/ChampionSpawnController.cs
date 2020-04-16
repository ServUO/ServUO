using System.Collections;

namespace Server.Engines.CannedEvil
{
    // This class is only here to remove legacy controller items
    public class ChampionSpawnController : Item
    {
        public ChampionSpawnController(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            // Do nothing
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        reader.ReadItemList();
                        goto case 0;
                    }
                case 0:
                    {
                        reader.ReadBool();
                        reader.ReadItemList();
                        reader.ReadItemList();
                        reader.ReadItemList();
                        reader.ReadItemList();
                        reader.ReadItemList();
                        reader.ReadTimeSpan();
                        reader.ReadInt();
                        break;
                    }
            }
            DeleteAll();
            Delete();
        }

        private void DeleteAll()
        {
            ArrayList list = new ArrayList();

            foreach (Item item in World.Items.Values)
            {
                if (item is ChampionSpawn && !item.Deleted)
                    list.Add(item);
            }

            foreach (ChampionSpawn cs in list)
            {
                cs.Delete();
            }
        }
    }
}