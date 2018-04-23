using System;
using Server.Gumps;

namespace Server.Items
{
    public class GryphonsHeadOnASpike : Item
    {
        [Constructable]
        public GryphonsHeadOnASpike()
            : base(0x995A)
        {
            this.Name = "Gryphon's Head On A Spike";
        }

        public GryphonsHeadOnASpike(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.HasGump(typeof(GryphonsHeadPic)))
            {
                from.CloseGump(typeof(GryphonsHeadPic));
                from.SendGump(new GryphonsHeadPic());
                return;
            }
            else
            {
                from.SendGump(new GryphonsHeadPic());
            }
        }

        private class GryphonsHeadPic : Gump
        {
            public GryphonsHeadPic()
                : base(0, 0)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
                this.AddImage(0, 0, 30527);
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