using Server;
using System;
using Server.Mobiles;

namespace Server.Items
{
    public class StoneFootwear : BaseShoes
    {
        public static void Initialize()
        {
            EventSink.Movement += new MovementEventHandler(EventSink_Movement);
        }

        [Constructable]
        public StoneFootwear() : this(Utility.Random(5899, 8))
        {
        }

        [Constructable]
        public StoneFootwear(int itemID) : base(itemID)
        {
            Weight = 3.0;
        }

        public string GetNameInfo()
        {
            string name = "foot wear";
            switch (ItemID)
            {
                case 5899:
                case 5900: name = "boots"; break;
                case 5901:
                case 5902: name = "sandals"; break;
                case 5903:
                case 5904: name = "shoes"; break;
                case 5905:
                case 5906: name = "thigh boots"; break;
            }
            return name;
        }

        public override void OnAdded(object parent)
        {
            if (parent is Mobile)
            {
                if (((Mobile)parent).Frozen && Navrey.Table.ContainsKey((Mobile)parent))
                {
                    ((Mobile)parent).Frozen = false;
                    Navrey.RemoveFromTable((Mobile)parent);
                    ((Mobile)parent).SendMessage("You have been released from Navrey's web!");
                }
            }
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            list.Add(1151095, GetNameInfo());
        }

        public static void EventSink_Movement(MovementEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from != null && from.Alive)
            {
                Item item = from.FindItemOnLayer(Layer.Shoes);

                if (item is StoneFootwear)
                    e.Blocked = true;
            }
        }

        public StoneFootwear(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class CrackedLavaRockSouth : Item
    {
        public override int LabelNumber { get { return 1098151; } }

        [Constructable]
        public CrackedLavaRockSouth() : base(19279)
        {
        }

        public virtual void OnCrack(Mobile from)
        {
            Item item;

            switch (Utility.Random(5))
            {
                default:
                case 0: item = new GeodeEast(); break;
                case 1: item = new GeodeSouth(); break;
                case 2: item = new GeodeShardEast(); break;
                case 3: item = new GeodeShardSouth(); break;
                case 4: item = new LavaRock(); break;
            }

            if (item != null)
            {
                from.AddToBackpack(item);
                from.SendMessage("You have split the lava rock!");
                Delete();
            }
        }

        public CrackedLavaRockSouth(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class CrackedLavaRockEast : Item
    {
        public override int LabelNumber { get { return 1098151; } }

        [Constructable]
        public CrackedLavaRockEast() : base(19275)
        {
        }

        public virtual void OnCrack(Mobile from)
        {
            Item item;
            from.SendSound(0x3B3);

            if (from.RawStr < Utility.Random(150))
            {
                from.SendMessage("You swing, but fail to crack the rock any further.");
                return;
            }

            switch (Utility.Random(5))
            {
                default:
                case 0: item = new GeodeEast(); break;
                case 1: item = new GeodeSouth(); break;
                case 2: item = new GeodeShardEast(); break;
                case 3: item = new GeodeShardSouth(); break;
                case 4: item = new LavaRock(); break;
            }

            if (item != null)
            {
                from.AddToBackpack(item);
                from.SendMessage("You have split the lava rock!");
                Delete();
            }
        }

        public CrackedLavaRockEast(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GeodeSouth : Item
    {
        public override int LabelNumber { get { return 1098145; } }

        [Constructable]
        public GeodeSouth() : base(Utility.Random(19277, 2))
        {
            switch (Utility.Random(4))
            {
                case 0: Hue = 2658; break;
                case 1: Hue = 2659; break;
                case 2: Hue = 2660; break;
                case 3: Hue = 2654; break;
            }
        }

        public GeodeSouth(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GeodeEast : Item
    {
        public override int LabelNumber { get { return 1098145; } }

        [Constructable]
        public GeodeEast() : base(Utility.Random(19273, 2))
        {
            switch (Utility.Random(4))
            {
                case 0: Hue = 2658; break;
                case 1: Hue = 2659; break;
                case 2: Hue = 2660; break;
                case 3: Hue = 2654; break;
            }
        }

        public GeodeEast(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GeodeShardSouth : Item
    {
        public override int LabelNumber { get { return 1098148; } }

        [Constructable]
        public GeodeShardSouth() : base(19276)
        {
            switch (Utility.Random(4))
            {
                case 0: Hue = 2658; break;
                case 1: Hue = 2659; break;
                case 2: Hue = 2660; break;
                case 3: Hue = 2654; break;
            }
        }

        public GeodeShardSouth(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GeodeShardEast : Item
    {
        public override int LabelNumber { get { return 1098148; } }

        [Constructable]
        public GeodeShardEast() : base(19272)
        {
            switch (Utility.Random(4))
            {
                case 0: Hue = 2658; break;
                case 1: Hue = 2659; break;
                case 2: Hue = 2660; break;
                case 3: Hue = 2654; break;
            }
        }

        public GeodeShardEast(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LavaRock : Item
    {
        public override int LabelNumber { get { return 1151166; } }

        [Constructable]
        public LavaRock() : base(Utility.Random(4964, 6))
        {
            Hue = 1175;
        }

        public LavaRock(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class StonePaver : Item
    {
        public override int LabelNumber { get { return 1097277; } }

        [Constructable]
        public StonePaver()
            : base(Utility.RandomList(18396, 18397, 18398, 18399, 18400, 18405, 18652, 18653, 18654, 18655))
        {
            Weight = 5;
        }

        public StonePaver(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}