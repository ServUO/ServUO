namespace Server.Items
{
    [Flipable(0xA489, 0xA48A)]
    public class CowPie : Item
    {
        public override int LabelNumber => 1126237;  // cow pie

        [Constructable]
        public CowPie()
            : base(0xA4E5)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (IsLockedDown && (!m.Hidden || m.IsPlayer()) && Utility.InRange(m.Location, Location, 2) && !Utility.InRange(oldLocation, Location, 2))
            {
                Effects.PlaySound(Location, Map, 1064);
            }

            base.OnMovement(m, oldLocation);
        }

        public CowPie(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
