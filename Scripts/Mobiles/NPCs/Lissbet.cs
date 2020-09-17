using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
    public class Lissbet : BaseEscort
    {
        public static void Initialize()
        {
            Spawn();
        }

        public static Point3D HomeLocation => new Point3D(1569, 1041, -7);
        public static int HomeRange => 5;

        public override Type[] Quests => new Type[] { typeof(ResponsibilityQuest) };

        public static List<Lissbet> Instances { get; set; }

        [Constructable]
        public Lissbet()
            : base()
        {
            Name = "Lissbet";
            Title = "The Flower Girl";

            if (Instances == null)
                Instances = new List<Lissbet>();

            Instances.Add(this);
        }

        public Lissbet(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(1074222); // Could I trouble you for some assistance?
        }

        public override void InitBody()
        {
            Female = true;
            Race = Race.Human;

            Hue = 0x8411;
            HairItemID = 0x203D;
            HairHue = 0x1BB;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Sandals());
            AddItem(new FancyShirt(0x6BF));
            AddItem(new Kilt(0x6AA));
        }

        public override void OnDelete()
        {
            if (Instances != null && Instances.Contains(this))
                Instances.Remove(this);

            Timer.DelayCall(TimeSpan.FromSeconds(3), delegate
            {
                Spawn();
            });

            base.OnDelete();
        }

        public static void Spawn()
        {
            if (Instances != null && Instances.Count > 0)
                return;

            Lissbet creature = new Lissbet
            {
                Home = HomeLocation,
                RangeHome = HomeRange
            };
            creature.MoveToWorld(HomeLocation, Map.Ilshenar);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Instances == null)
                Instances = new List<Lissbet>();

            Instances.Add(this);
        }
    }
}
