using System;
using Server.Engines.Points;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class EtherealSoulcleanser : BaseContainer
    {
        public override int LabelNumber { get { return 1159196; } } // Ethereal Soulcleanser

        public override int DefaultGumpID { get { return 0x10C; } }
        public override bool DisplaysContent { get { return false; } }

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
            return CheckGain(from, dropped);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            return CheckGain(from, item);
        }

        public bool CheckGain(Mobile from, Item item)
        {
            if (from == null || from.Deleted)
            {
                return false;
            }

            if (item != null && item is EtherealSoulbinder && ((EtherealSoulbinder)item).SoulPoint > 0)
            {
                from.SendLocalizedMessage(1159170); // The machine only accepts filled ethereal soulbinders.
                return false;
            }

            int amount = Calculate((EtherealSoulbinder)item);
            PointsSystem.FellowshipData.AwardPoints((PlayerMobile)from, amount);

            Effects.SendPacket(from.Location, from.Map, new GraphicalEffect(EffectType.FixedXYZ, from.Serial, Serial.Zero, 0x373A, from.Location, from.Location, 10, 15, true, true));
            Effects.PlaySound(from.Location, from.Map, 0x1F2);

            item.Delete();

            return true;
        }

        private int Calculate(EtherealSoulbinder item)
        {
            int perc = item.GetPercent();

            if (perc <= 0)
                return 0;
            else
                return 10000 * (perc / 100);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
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
