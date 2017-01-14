using Server;
using System;
using Server.Mobiles;
using Server.Engines.VoidPool;

namespace Server.Items
{
    public class BestWaveBoard : Item
    {
        public override bool ForceShowProperties { get { return true; } }

        [Constructable]
        public BestWaveBoard() : base(7774)
        {
            Name = "Void Pool - Best Wave";
            Movable = false;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if(m is PlayerMobile && m.InRange(this.Location, 3))
                m.SendGump(new ScoresGump(m.Map == Map.Felucca ? VoidPoolController.InstanceFel : VoidPoolController.InstanceTram, m as PlayerMobile, ScoreType.BestWave));
        }

        public BestWaveBoard(Serial serial)
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