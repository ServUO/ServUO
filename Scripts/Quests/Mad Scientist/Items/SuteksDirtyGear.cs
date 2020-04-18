using Server.Items.MusicBox;

namespace Server.Items
{
    public class SuteksDirtyGear : Item
    {
        public override int LabelNumber => 1115722;  // Sutek's Dirty Gear

        [Constructable]
        public SuteksDirtyGear()
            : this(1)
        {
        }

        [Constructable]
        public SuteksDirtyGear(int amount)
            : base(0x1053)
        {
            Hue = 1102;
        }

        public SuteksDirtyGear(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Utility.RandomDouble() < 0.05)
            {
                from.AddToBackpack(MusicBoxGears.RandomMusixBoxGears(TrackRarity.Rare));
            }
            else
            {
                if (Utility.RandomBool())
                {
                    from.AddToBackpack(MusicBoxGears.RandomMusixBoxGears(TrackRarity.Common));
                }
                else
                {
                    from.AddToBackpack(MusicBoxGears.RandomMusixBoxGears(TrackRarity.UnCommon));
                }
            }

            from.SendLocalizedMessage(1115723); // You have polished the dirty gear...

            Delete();
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