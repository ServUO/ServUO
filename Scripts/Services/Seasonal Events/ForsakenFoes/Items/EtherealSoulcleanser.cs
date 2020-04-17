using Server.Engines.Points;
using Server.Network;

namespace Server.Items
{
    public class EtherealSoulcleanser : BaseContainer
    {
        public override int LabelNumber => 1159196;  // Ethereal Soulcleanser

        public override int DefaultGumpID => 0x10C;
        public override bool DisplaysContent => false;

        public static EtherealSoulcleanser InstanceTram { get; set; }
        public static EtherealSoulcleanser InstanceFel { get; set; }

        [Constructable]
        public EtherealSoulcleanser()
            : base(0x2DF4)
        {
            Hue = 2591;
            Movable = false;
        }

        public EtherealSoulcleanser(Serial serial)
            : base(serial)
        {
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            return Check(from, dropped);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            return Check(from, item);
        }

        public bool Check(Mobile from, Item item)
        {
            if (from == null || from.Deleted || item == null)
            {
                return false;
            }

            if (!(item is EtherealSoulbinder) || item is EtherealSoulbinder && ((EtherealSoulbinder)item).SoulPoint <= 0)
            {
                from.SendLocalizedMessage(1159170); // The machine only accepts filled ethereal soulbinders.
                return false;
            }

            double amount = 100 * ((EtherealSoulbinder)item).SoulPoint;
            PointsSystem.FellowshipData.AwardPoints(from, amount);

            Effects.SendPacket(from.Location, from.Map, new GraphicalEffect(EffectType.FixedXYZ, from.Serial, Serial.Zero, 0x373A, from.Location, from.Location, 10, 15, true, true));
            from.PlaySound(0x1F2);

            item.Delete();

            return true;
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

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }

            if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }
        }
    }
}
