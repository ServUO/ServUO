using System;
using Server.Gumps;

namespace Server.Items
{
    public class MrsTroubleMakersHeadOnASpike : Item
    {
        [Constructable]
        public MrsTroubleMakersHeadOnASpike()
            : base(0x9955)
        {
            this.Name = "MrsTroubleMaker’s Head On A Spike";
        }

        public MrsTroubleMakersHeadOnASpike(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.HasGump(typeof(MrsTroubleMakersHeadPic)))
            {
                from.CloseGump(typeof(MrsTroubleMakersHeadPic));
                from.SendGump(new MrsTroubleMakersHeadPic());
                return;
            }
            else
            {
                from.SendGump(new MrsTroubleMakersHeadPic());
            }
        }

        private class MrsTroubleMakersHeadPic : Gump
        {
            public MrsTroubleMakersHeadPic()
                : base(0, 0)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
                this.AddImage(0, 0, 30522);
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