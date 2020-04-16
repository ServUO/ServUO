using Server.Gumps;

namespace Server.Items
{
    public class SpikeHead : Item
    {
        public int GumpID { get; set; }

        [Constructable]
        public SpikeHead()
            : this(Utility.Random(SpikeHeadType.Length))
        {
        }

        [Constructable]
        public SpikeHead(int type)
            : base(0x9955 + type)
        {
            GumpID = 30522 + type;

            Name = SpikeHeadType[type];
        }

        private static readonly string[] SpikeHeadType = new string[]
        {
            "MrsTroubleMaker’s Head On A Spike",
            "Brutrin’s Head On A Spike",
            "Stethun’s Head On A Spike",
            "Rakban’s Head On A Spike",
            "Thatblok’s Head On A Spike",
            "Gryphon’s Head On A Spike",
            "Kyronix’s Head On A Spike",
            "Misk’s Head On A Spike",
            "Bleak’s Head On A Spike",
            "Onifrk’s Head On A Spike",
            "Mesanna’s Head On A Spike"
        };

        public SpikeHead(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!m.InRange(GetWorldLocation(), 3))
            {
                m.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else
            {
                Gump g = new Gump(100, 100);
                g.AddImage(0, 0, GumpID);

                m.SendGump(g);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(GumpID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            GumpID = reader.ReadInt();
        }
    }
}
