using System;
using Server.Gumps;

namespace Server.Items
{
    public class RakbansHeadOnASpike : Item
    {
        [Constructable]
        public RakbansHeadOnASpike()
            : base(0x9958)
        {
            this.Name = "Rakban’s Head On A Spike";
        }

        public RakbansHeadOnASpike(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.HasGump(typeof(RakbansHeadPic)))
            {
                from.CloseGump(typeof(RakbansHeadPic));
                from.SendGump(new RakbansHeadPic());
                return;
            }
            else
            {
                from.SendGump(new RakbansHeadPic());
            }
        }

        private class RakbansHeadPic : Gump
        {
            public RakbansHeadPic()
                : base(0, 0)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
                this.AddImage(0, 0, 30525);
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