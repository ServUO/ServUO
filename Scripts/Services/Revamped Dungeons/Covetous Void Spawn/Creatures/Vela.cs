using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.VoidPool
{
    public class VelaTheSorceress : BaseCreature
    {
        public override bool IsInvulnerable { get { return true; } }

        [Constructable]
        public VelaTheSorceress()
            : base(AIType.AI_Vendor, FightMode.None, 2, 1, 0.5, 2)
        {
            Name = "Vela";
            Title = "the sorceress";

            SetStr(110);
            SetDex(100);
            SetInt(1000);

            Hue = Utility.RandomSkinHue();
            Body = 0x191;
            HairItemID = 0x203C;
            HairHue = 0x46D;

            Item item = new FancyShirt();
            item.Hue = 1150;
            AddItem(item);

            item = new LongPants();
            item.Hue = 108;
            AddItem(item);

            item = new Shoes();
            item.Hue = 108;
            AddItem(item);

            item = new BraceletOfProtection();
            item.Movable = false;
            PackItem(item);

            item = new Hephaestus();
            item.Movable = false;
            PackItem(item);

            item = new GargishHephaestus();
            item.Movable = false;
            PackItem(item);

            item = new BlightOfTheTundra();
            item.Movable = false;
            PackItem(item);

            item = new GargishBlightOfTheTundra();
            item.Movable = false;
            PackItem(item);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile && from.InRange(this.Location, 5))
                from.SendGump(new VoidPoolRewardGump(this, from as PlayerMobile));
        }

        public VelaTheSorceress(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}