using System;
using Server.Gumps;

namespace Server.Items
{
    public class MisksHeadOnASpike : Item
    {
        [Constructable]
        public MisksHeadOnASpike()
            : base(0x995C)
        {
            this.Name = "Misk’s Head On A Spike";
        }

        public MisksHeadOnASpike(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.HasGump(typeof(MisksHeadPic)))
            {
                from.CloseGump(typeof(MisksHeadPic));
                from.SendGump(new MisksHeadPic());
                return;
            }
            else
            {
                from.SendGump(new MisksHeadPic());
            }
        }

        private class MisksHeadPic : Gump
        {
            public MisksHeadPic()
                : base(0, 0)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
                this.AddImage(0, 0, 30529);
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