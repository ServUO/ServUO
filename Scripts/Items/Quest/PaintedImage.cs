using System;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.Quests.Collector
{
    public class PaintedImage : Item
    {
        private ImageType m_Image;
        [Constructable]
        public PaintedImage(ImageType image)
            : base(0xFF3)
        {
            this.Weight = 1.0;
            this.Hue = 0x8FD;

            this.m_Image = image;
        }

        public PaintedImage(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ImageType Image
        {
            get
            {
                return this.m_Image;
            }
            set
            {
                this.m_Image = value;
                this.InvalidateProperties();
            }
        }
        public override void AddNameProperty(ObjectPropertyList list)
        {
            ImageTypeInfo info = ImageTypeInfo.Get(this.m_Image);
            list.Add(1060847, "#1055126\t#" + info.Name); // a painted image of:
        }

        public override void OnSingleClick(Mobile from)
        {
            ImageTypeInfo info = ImageTypeInfo.Get(this.m_Image);
            this.LabelTo(from, 1060847, "#1055126\t#" + info.Name); // a painted image of:
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            from.SendGump(new InternalGump(this.m_Image));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.WriteEncodedInt((int)this.m_Image);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Image = (ImageType)reader.ReadEncodedInt();
        }

        private class InternalGump : Gump
        {
            public InternalGump(ImageType image)
                : base(75, 25)
            {
                ImageTypeInfo info = ImageTypeInfo.Get(image);

                this.AddBackground(45, 20, 100, 100, 0xA3C);
                this.AddBackground(52, 29, 86, 82, 0xBB8);

                this.AddItem(info.X, info.Y, info.Figurine);
            }
        }
    }
}