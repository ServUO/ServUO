using System;
using Server.Gumps;

namespace Server.Items
{
    public class StethunsHeadOnASpike : Item
    {
        [Constructable]
        public StethunsHeadOnASpike()
            : base(0x9957)
        {
            this.Name = "Stethun’s Head On A Spike";
        }

        public StethunsHeadOnASpike(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.HasGump(typeof(StethunsHeadPic)))
            {
                from.CloseGump(typeof(StethunsHeadPic));
                from.SendGump(new StethunsHeadPic());
                return;
            }
            else
            {
                from.SendGump(new StethunsHeadPic());
            }
        }

        private class StethunsHeadPic : Gump
        {
            public StethunsHeadPic()
                : base(0, 0)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
                this.AddImage(0, 0, 30524);
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