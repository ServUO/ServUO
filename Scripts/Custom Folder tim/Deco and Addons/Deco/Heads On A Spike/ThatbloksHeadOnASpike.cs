using System;
using Server.Gumps;

namespace Server.Items
{
    public class ThatbloksHeadOnASpike : Item
    {
        [Constructable]
        public ThatbloksHeadOnASpike()
            : base(0x9959)
        {
            this.Name = "Thatblok’s Head On A Spike";
        }

        public ThatbloksHeadOnASpike(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.HasGump(typeof(ThatbloksHeadPic)))
            {
                from.CloseGump(typeof(ThatbloksHeadPic));
                from.SendGump(new ThatbloksHeadPic());
                return;
            }
            else
            {
                from.SendGump(new ThatbloksHeadPic());
            }
        }

        private class ThatbloksHeadPic : Gump
        {
            public ThatbloksHeadPic()
                : base(0, 0)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
                this.AddImage(0, 0, 30526);
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