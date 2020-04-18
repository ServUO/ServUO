using Server.Engines.Quests;
using Server.Mobiles;
using Server.Network;
using Server.SkillHandlers;

namespace Server.Engines.Khaldun
{
    public class MysteriousBook : Item, IForensicTarget
    {
        public override int LabelNumber => 1158583;  // mysterious book
        public static readonly Point3D SpawnLocation = new Point3D(6240, 2885, 7);

        [CommandProperty(AccessLevel.GameMaster)]
        public TrapDoor Door { get; set; }

        public MysteriousBook(TrapDoor door)
            : base(0x42b8)
        {
            Movable = false;
            Door = door;
            Hue = 1950;
        }

        public void OnForensicEval(Mobile m)
        {
            if (!m.Player)
                return;

            GoingGumshoeQuest2 quest = QuestHelper.GetQuest<GoingGumshoeQuest2>((PlayerMobile)m);

            if (quest != null)
            {
                if (HasFoundClue1(quest))
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
                    m.SendGump(new GumshoeItemGump(m, ItemID, Hue, "book", 1158577, null));

                    SetFoundClue1(quest);
                }
            }
        }

        public void OnInscribeTarget(Mobile m)
        {
            GoingGumshoeQuest2 quest = QuestHelper.GetQuest<GoingGumshoeQuest2>((PlayerMobile)m);

            if (quest != null)
            {
                if (HasFoundClue2(quest))
                {
                    m.SendLocalizedMessage(1158613); // You have already documented this clue.
                }
                else
                {
                    m.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1158582, m.NetState); // *You copy several pages onto some parchment and roll it up...*
                    m.SendLocalizedMessage(1158612, null, 0x23); // You have identified a clue! This item seems pertinent to the investigation!
                    m.AddToBackpack(new RolledParchment(GetPage()));
                    m.PlaySound(0x249);

                    m.SendSound(quest.UpdateSound);

                    SetFoundClue2(quest);
                }
            }
        }

        private void SetFoundClue1(GoingGumshoeQuest2 quest)
        {
            if (Door == null)
            {
                return;
            }

            switch (Door.Keyword.ToLower())
            {
                case "boreas": quest.ClueBook1_1 = true; break;
                case "moriens": quest.ClueBook2_1 = true; break;
                case "carthax": quest.ClueBook3_1 = true; break;
                case "tenebrae": quest.ClueBook4_1 = true; break;
            }
        }

        private void SetFoundClue2(GoingGumshoeQuest2 quest)
        {
            if (Door == null)
            {
                return;
            }

            switch (Door.Keyword.ToLower())
            {
                case "boreas": quest.ClueBook1_2 = true; break;
                case "moriens": quest.ClueBook2_2 = true; break;
                case "carthax": quest.ClueBook3_2 = true; break;
                case "tenebrae": quest.ClueBook4_2 = true; break;
            }
        }

        private bool HasFoundClue1(GoingGumshoeQuest2 quest)
        {
            if (Door == null)
            {
                return false;
            }

            switch (Door.Keyword.ToLower())
            {
                case "boreas": return quest.ClueBook1_1;
                case "moriens": return quest.ClueBook2_1;
                case "carthax": return quest.ClueBook3_1;
                case "tenebrae": return quest.ClueBook4_1;
            }

            return false;
        }

        private bool HasFoundClue2(GoingGumshoeQuest2 quest)
        {
            if (Door == null)
            {
                return false;
            }

            switch (Door.Keyword.ToLower())
            {
                case "boreas": return quest.ClueBook1_2;
                case "moriens": return quest.ClueBook2_2;
                case "carthax": return quest.ClueBook3_2;
                case "tenebrae": return quest.ClueBook4_2;
            }

            return false;
        }

        private int GetPage()
        {
            if (Door == null)
            {
                return 0;
            }

            switch (Door.Keyword.ToLower())
            {
                case "boreas": return 1158629;
                case "moriens": return 1158630;
                case "carthax": return 1158631;
                case "tenebrae": return 1158632;
            }

            return 0;
        }

        public MysteriousBook(Serial serial)
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
