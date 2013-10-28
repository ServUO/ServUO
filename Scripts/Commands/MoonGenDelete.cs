using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Commands
{
    public class PublicMoongateDelete
    {
        public PublicMoongateDelete()
        {
        }

        public static void Initialize()
        {
            CommandSystem.Register("MoonGenDelete", AccessLevel.Administrator, new CommandEventHandler(MoonGenDelete_OnCommand));
        }

        [Usage("MoonGenDelete")]
        [Description("Removes created moongates with MoonGen command.")]
        public static void MoonGenDelete_OnCommand(CommandEventArgs e)
        {
            DeleteAll();
            //int count = 0;
            //count += MoonGen(PMList.Trammel);
            //count += MoonGen(PMList.Felucca);
            //count += MoonGen(PMList.Ilshenar);
            //count += MoonGen(PMList.Malas);
            //count += MoonGen(PMList.Tokuno);
            //World.Broadcast(0x35, true, "{0} moongates deleted.", count);
        }

        private static void DeleteAll()
        {
            List<Item> list = new List<Item>();

            foreach (Item item in World.Items.Values)
            {
                if (item is PublicMoongate)
                    list.Add(item);
            }

            foreach (Item item in list)
                item.Delete();

            if (list.Count > 0)
                World.Broadcast(0x35, true, "{0} moongates removed.", list.Count);
        }

        private static int DeleteMoonGate(Map map, Point3D p)
        {
            Queue<Item> m_Queue = new Queue<Item>();

            IPooledEnumerable eable = map.GetItemsInRange(p, 0);

            foreach (Item item in eable)
            {
                if (item is PublicMoongate)
                {
                    int delta = item.Z - p.Z;

                    if (delta >= -12 && delta <= 12)
                        m_Queue.Enqueue(item);
                }
            }

            eable.Free();

            int m_Count = m_Queue.Count;

            while (m_Queue.Count > 0)
                (m_Queue.Dequeue()).Delete();

            return m_Count;
        }
    }
}