using System;
using Server.Gumps;

namespace Server.Items
{
    public class OnifrksHeadOnASpike : Item
    {
        [Constructable]
        public OnifrksHeadOnASpike()
            : base(0x995E)
        {
            this.Name = "Onifrk’s Head On A Spike";
        }

        public OnifrksHeadOnASpike(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.HasGump(typeof(OnifrksHeadPic)))
            {
                from.CloseGump(typeof(OnifrksHeadPic));
                from.SendGump(new OnifrksHeadPic());
                return;
            }
            else
            {
                from.SendGump(new OnifrksHeadPic());
            }
        }

        private class OnifrksHeadPic : Gump
        {
            public OnifrksHeadPic()
                : base(0, 0)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
                this.AddImage(0, 0, 30531);
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