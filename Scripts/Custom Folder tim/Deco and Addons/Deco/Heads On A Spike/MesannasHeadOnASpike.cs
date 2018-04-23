using System;
using Server.Gumps;

namespace Server.Items
{
    public class MesannasHeadOnASpike : Item
    {
        [Constructable]
        public MesannasHeadOnASpike()
            : base(0x995F)
        {
            this.Name = "Mesanna’s Head On A Spike";
        }

        public MesannasHeadOnASpike(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.HasGump(typeof(MesannasHeadPic)))
            {
                from.CloseGump(typeof(MesannasHeadPic));
                from.SendGump(new MesannasHeadPic());
                return;
            }
            else
            {
                from.SendGump(new MesannasHeadPic());
            }
        }

        private class MesannasHeadPic : Gump
        {
            public MesannasHeadPic()
                : base(0, 0)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
                this.AddImage(0, 0, 30532);
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