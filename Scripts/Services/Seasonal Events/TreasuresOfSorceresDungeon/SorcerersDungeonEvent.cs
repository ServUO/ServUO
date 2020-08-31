using Server.Items;
using Server.Engines.SeasonalEvents;

namespace Server.Engines.SorcerersDungeon
{
    public class SorcerersDungeonEvent : SeasonalEvent
    {
        public static SorcerersDungeonEvent Instance { get; set; }

        public SorcerersDungeonEvent(EventType type, string name, EventStatus status)
            : base(type, name, status)
        {
            Instance = this;
        }

        public SorcerersDungeonEvent(EventType type, string name, EventStatus status, int month, int day, int duration)
            : base(type, name, status, month, day, duration)
        {
            Instance = this;
        }

        protected override void Generate()
        {
            Map map = Map.Ilshenar;

            if (SorcerersDungeonResearcher.Instance == null)
            {
                SorcerersDungeonResearcher.Instance = new SorcerersDungeonResearcher();
                SorcerersDungeonResearcher.Instance.MoveToWorld(new Point3D(536, 456, -53), map);
            }

            if (map.FindItem<Static>(new Point3D(546, 460, 6)) == null)
            {
                Static st = new Static(0x9D2B);
                st.MoveToWorld(new Point3D(546, 460, 6), map);

                st = new Static(0x9D2C);
                st.MoveToWorld(new Point3D(548, 460, 6), map);

                st = new Static(0x9D2D);
                st.MoveToWorld(new Point3D(548, 458, 6), map);
            }

            if (map.FindItem<Static>(new Point3D(545, 462, -53)) == null)
            {
                Static st = new Static(0x9F34);
                st.MoveToWorld(new Point3D(545, 462, -53), map);
            }

            if (map.FindItem<Static>(new Point3D(550, 462, -53)) == null)
            {
                Static st = new Static(0x9F34);
                st.MoveToWorld(new Point3D(550, 462, -53), map);
            }

            if (map.FindItem<Static>(new Point3D(545, 463, -55)) == null)
            {
                Static st = new Static(0x9F28);
                st.MoveToWorld(new Point3D(545, 463, -55), map);
            }

            if (map.FindItem<Static>(new Point3D(550, 463, -55)) == null)
            {
                Static st = new Static(0x9F24);
                st.MoveToWorld(new Point3D(550, 463, -55), map);
            }

            if (TOSDSpawner.Instance == null)
            {
                TOSDSpawner spawner = new TOSDSpawner();
                spawner.BeginTimer();
            }
        }

        protected override void Remove()
        {
            if (TOSDSpawner.Instance != null)
            {
                TOSDSpawner.Instance.Deactivate();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = InheritInsertion ? 0 : reader.ReadInt();
        }
    }
}
