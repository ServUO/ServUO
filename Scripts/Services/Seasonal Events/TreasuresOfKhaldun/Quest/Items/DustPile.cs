using Server.Engines.Quests;
using Server.Mobiles;
using Server.Network;
using Server.SkillHandlers;

namespace Server.Engines.Khaldun
{
    public class DustPile : Item, IForensicTarget
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public TrapDoor Door { get; set; }

        public DustPile(TrapDoor door)
            : base(0x573D)
        {
            Movable = false;
            Hue = 2044;
            Name = "";
            Door = door;
        }

        public void OnForensicEval(Mobile m)
        {
            if (!m.Player)
                return;

            GoingGumshoeQuest2 quest = QuestHelper.GetQuest<GoingGumshoeQuest2>((PlayerMobile)m);

            if (quest != null)
            {
                if (HasFoundClue(quest))
                {
                    m.SendLocalizedMessage(1158613); // You have already documented this clue.
                }
                else
                {
                    m.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1157722, "Forensics", m.NetState); // *Your proficiency in ~1_SKILL~ reveals more about the item*
                    m.SendLocalizedMessage(1158612, null, 0x23); // You have identified a clue! This item seems pertinent to the investigation!

                    m.SendSound(quest.UpdateSound);
                    m.SendSound(m.Female ? 0x30B : 0x41A);

                    m.CloseGump(typeof(GumshoeItemGump));
                    m.SendGump(new GumshoeItemGump(m, ItemID, Hue, "a dust pile", 1158617, null));

                    /*The dust seems to have have settled in a distinct pattern around whatever once was placed at this location.
                     * Whatever it was, it was certainly small enough to be taken away in a hurry.*/

                    SetFoundClue(quest);
                }
            }
        }

        private void SetFoundClue(GoingGumshoeQuest2 quest)
        {
            if (Door == null)
            {
                return;
            }

            switch (Door.Keyword.ToLower())
            {
                case "boreas": quest.ClueDust1 = true; break;
                case "moriens": quest.ClueDust2 = true; break;
                case "carthax": quest.ClueDust3 = true; break;
                case "tenebrae": quest.ClueDust4 = true; break;
            }
        }

        private bool HasFoundClue(GoingGumshoeQuest2 quest)
        {
            if (Door == null)
            {
                return false;
            }

            switch (Door.Keyword.ToLower())
            {
                case "boreas": return quest.ClueDust1;
                case "moriens": return quest.ClueDust2;
                case "carthax": return quest.ClueDust3;
                case "tenebrae": return quest.ClueDust4;
            }

            return false;
        }

        public DustPile(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
            writer.Write(Door);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            Door = reader.ReadItem() as TrapDoor;
        }
    }
}
