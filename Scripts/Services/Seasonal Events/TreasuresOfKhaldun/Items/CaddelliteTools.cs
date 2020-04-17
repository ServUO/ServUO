namespace Server.Items
{
    public interface ICaddelliteTool
    {
    }

    public class CaddellitePickaxe : Pickaxe, ICaddelliteTool
    {
        public override int LabelNumber => 1158689;  // Caddellite Pickaxe

        [Constructable]
        public CaddellitePickaxe()
        {
        }

        public CaddellitePickaxe(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1158692); // * Can Harvest Caddellite Infused Resources in the Lost Lands *
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
        }
    }

    public class CaddelliteHatchet : Hatchet, ICaddelliteTool
    {
        public override int LabelNumber => 1158690;  // Caddellite Hatchet

        [Constructable]
        public CaddelliteHatchet()
        {
        }

        public CaddelliteHatchet(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1158692); // * Can Harvest Caddellite Infused Resources in the Lost Lands *
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
        }
    }

    public class CaddelliteFishingPole : FishingPole, ICaddelliteTool
    {
        public override int LabelNumber => 1158691;  // Caddellite Fishing Pole

        [Constructable]
        public CaddelliteFishingPole()
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1158692); // * Can Harvest Caddellite Infused Resources in the Lost Lands *
        }

        public CaddelliteFishingPole(Serial serial)
            : base(serial)
        {
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
        }
    }
}
