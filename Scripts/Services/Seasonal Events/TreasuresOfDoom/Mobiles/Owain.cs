using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.TreasuresOfDoom
{
    public class Owain : BaseVendor
    {
        protected readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override bool IsActiveVendor => false;

        public static Owain Instance { get; set; }
        public static readonly Point3D SpawnLocation = new Point3D(86, 223, -1);

        public override void InitSBInfo()
        {
        }

        [Constructable]
        public Owain() : base("the blind")
        {
            Instance = this;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(Location, 5))
            {
                BaseGump.SendGump(new StoryGump(m as PlayerMobile,
                    1155664,
                    new PageData(1, 1155665, new SelectionEntry(1155666, 2)),
                    new PageData(2, 1155667, new SelectionEntry(1155668, 3), new SelectionEntry(1155669, 4)),
                    new PageData(3, 1155670, new SelectionEntry(1155671, 5)),
                    new PageData(4, 1155672, new SelectionEntry(1155673, 6)),
                    new PageData(5, 1155674, new SelectionEntry(1155675, 7)),
                    new PageData(6, 1155676, new SelectionEntry(1155677, 8)),
                    new PageData(7, 1155678),
                    new PageData(8, 1155679)));
            }
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = "Owain";

            SpeechHue = 0x3B2;
            Hue = Utility.RandomSkinHue();
            Body = 0x190;
        }

        public override void InitOutfit()
        {
            SetWearable(new Robe(), 1255);
            SetWearable(new ThighBoots(), 1908);
        }

        public Owain(Serial serial)
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