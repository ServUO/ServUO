using System;
using Server.Gumps;

namespace Server.Items
{
    public class BleaksHeadOnASpike : Item
    {
        [Constructable]
        public BleaksHeadOnASpike()
            : base(0x995D)
        {
            this.Name = "Bleak’s Head On A Spike";
        }

        public BleaksHeadOnASpike(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.HasGump(typeof(HeadPic)))
            {
                from.CloseGump(typeof(HeadPic));
                from.SendGump(new HeadPic());
                return;
            }
            else
            {
                from.SendGump(new HeadPic());
            }
        }

        private class HeadPic : Gump
        {
            public HeadPic()
                : base(0, 0)
            {
                 this.Closable = true;
                 this.Disposable = true;
                 this.Dragable = true;
                 this.Resizable = false;
                 this.AddImage(0, 0, 30530);
            }
        }

        //public override int Title
        //{
        //    get
         //   {
        //        return 1154355;
        //    }
       // }// Head on a spike

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}