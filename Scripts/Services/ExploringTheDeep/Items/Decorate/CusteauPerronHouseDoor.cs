using Server.Engines.Quests;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class CusteauPerronHouseDoor : DarkWoodDoor
    {
        [Constructable]
        public CusteauPerronHouseDoor() : base(DoorFacing.WestCCW)
        {
        }

        public CusteauPerronHouseDoor(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile m)
            {
                if (m.ExploringTheDeepQuest == ExploringTheDeepQuestChain.HeplerPaulsonComplete || m.Region.Name == "Custeau Perron House")
                {
                    base.OnDoubleClick(m);
                    m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1154223); // *You knock on the door but there is no answer.  You decide to let yourself in...*
                }
                else
                {
                    m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502503); // That is locked.
                }
            }
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
