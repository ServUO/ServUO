using System;
using Server.Gumps;

namespace Server.Items
{
    public class KyronixsHeadOnASpike : Item
    {
        [Constructable]
        public KyronixsHeadOnASpike()
            : base(0x995B)
        {
            this.Name = "Kyronix’s Head On A Spike";
        }

        public KyronixsHeadOnASpike(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.HasGump(typeof(KyronixsHeadPic)))
            {
                from.CloseGump(typeof(KyronixsHeadPic));
                from.SendGump(new KyronixsHeadPic());
                return;
            }
            else
            {
                from.SendGump(new KyronixsHeadPic());
            }
        }

        private class KyronixsHeadPic : Gump
        {
            public KyronixsHeadPic()
                : base(0, 0)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
                this.AddImage(0, 0, 30528);
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