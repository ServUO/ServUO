using Server.Engines.Quests;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.SkillHandlers;

// 1158607 => brit
// 1158608 => vesper
// 1158609 => moonglow
// 1158610 => yew

namespace Server.Engines.Khaldun
{
    public class DamagedHeadstone : Item, IForensicTarget
    {
        public override int LabelNumber => 1158561;  // damaged headstone

        [CommandProperty(AccessLevel.GameMaster)]
        public int GumpLocalization { get; private set; }

        public DamagedHeadstone(int gumpLoc)
            : base(0x1180)
        {
            GumpLocalization = gumpLoc;
            Movable = false;
        }

        public void OnForensicEval(Mobile m)
        {
            if (!m.Player)
                return;

            GoingGumshoeQuest2 quest = QuestHelper.GetQuest<GoingGumshoeQuest2>((PlayerMobile)m);

            if (quest != null)
            {
                m.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1158571, m.NetState); // *You examine the headstone...*
                m.SendLocalizedMessage(1158562, null, 0x23); // The damage to the epitaph seems deliberate.  Using your training from Inspector Jasper you have found a hidden message among the scratches. You recreate the original epitaph in your mind's eye...

                m.SendSound(quest.UpdateSound);

                Gump gump = new Gump(50, 50);

                gump.AddImage(0, 0, 0x66);
                gump.AddHtmlLocalized(47, 60, 146, 160, GumpLocalization, false, false);

                m.SendGump(gump);

                SetPrerequisite(quest);
            }
            else
            {
                m.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1158563, m.NetState); // *It appears to be a normal, yet oddly damaged, headstone. The epitaph is illegible..*
            }
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(GetWorldLocation(), 2))
            {
                m.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1158563, m.NetState); // *It appears to be a normal, yet oddly damaged, headstone. The epitaph is illegible..*
            }
            else
                m.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1019045, m.NetState); // I can't reach that.
        }

        public void SetPrerequisite(GoingGumshoeQuest2 quest)
        {
            switch (GumpLocalization)
            {
                case 1158607: quest.VisitedHeastone1 = true; break;
                case 1158608: quest.VisitedHeastone2 = true; break;
                case 1158609: quest.VisitedHeastone3 = true; break;
                case 1158610: quest.VisitedHeastone4 = true; break;
            }
        }

        public DamagedHeadstone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(GumpLocalization);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            GumpLocalization = reader.ReadInt();
        }
    }
}
