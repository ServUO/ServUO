using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SpawnObject
    {
        public string SpawnName { get; set; }
        public int MaxCount { get; set; }

        public int CurrentCount => SpawnedObjects.Count;

        public List<ISpawnable> SpawnedObjects { get; set; }

        public SpawnObject(string name)
            : this(name, 1)
        {
        }

        public SpawnObject(string name, int count)
        {
            SpawnName = name;
            MaxCount = count;

            SpawnedObjects = new List<ISpawnable>();
        }

        public SpawnObject(GenericReader reader)
        {
            int version = reader.ReadInt();

            SpawnedObjects = new List<ISpawnable>();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                ISpawnable e = World.FindEntity(reader.ReadInt()) as ISpawnable;

                if (e != null)
                    SpawnedObjects.Add(e);
            }

            SpawnName = reader.ReadString();
            MaxCount = reader.ReadInt();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(SpawnedObjects.Count);
            foreach (ISpawnable sp in SpawnedObjects)
            {
                if (sp is Item item)
                    writer.Write(item);
                else if (sp is Mobile mobile)
                    writer.Write(mobile);
                else
                    writer.Write(Serial.MinusOne);
            }

            writer.Write(SpawnName);
            writer.Write(MaxCount);
        }
    }
}
