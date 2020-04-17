using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.TreasuresOfDoom
{
    public class Elizabeth : BaseVendor
    {
        protected readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override bool IsActiveVendor => false;

        public static Elizabeth Instance { get; set; }
        public static readonly Point3D SpawnLocation = new Point3D(2364, 1284, -90);

        public override void InitSBInfo()
        {
        }

        [Constructable]
        public Elizabeth() : base("the Professor")
        {
            Instance = this;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(Location, 5))
            {
                BaseGump.SendGump(new StoryGump(m as PlayerMobile,
                    1155648,
                    new PageData(1, 1155649, new SelectionEntry(1155650, 2)),
                    new PageData(2, 1155651, new SelectionEntry(1155652, 3), new SelectionEntry(1155653, 4)),
                    new PageData(3, 1155654, new SelectionEntry(1155653, 5), new SelectionEntry(1155655, 6)),
                    new PageData(4, 1155657, new SelectionEntry(1155652, 5)),
                    new PageData(5, 1155654, new SelectionEntry(1155655, 6)),
                    new PageData(6, 1155656)));
            }
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = "Elizabeth";
            Female = true;

            SpeechHue = 0x3B2;
            Hue = Utility.RandomSkinHue();
            Body = 0x191;
        }

        public override void InitOutfit()
        {
            SetWearable(new FancyShirt(), 1255);
            SetWearable(new JinBaori(), 2722);
            SetWearable(new Kilt(), 2012);
            SetWearable(new ThighBoots(), 1908);
        }

        public Elizabeth(Serial serial)
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

            Instance = this;
        }
    }
}