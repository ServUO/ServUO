using System;
using Server.Gumps;

namespace Server.Items
{
    public class BrutrinsHeadOnASpike : Item
    {
        [Constructable]
        public BrutrinsHeadOnASpike()
            : base(0x9956)
        {
            this.Name = "Brutrin’s Head On A Spike";
        }

        public BrutrinsHeadOnASpike(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.HasGump(typeof(HeadPic1)))
            {
                from.CloseGump(typeof(HeadPic1));
                from.SendGump(new HeadPic1());
                return;
            }
            else
            {
                from.SendGump(new HeadPic1());
            }
        }

        private class HeadPic1 : Gump
        {
            public HeadPic1()
                : base(0, 0)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
                this.AddImage(0, 0, 30523);
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