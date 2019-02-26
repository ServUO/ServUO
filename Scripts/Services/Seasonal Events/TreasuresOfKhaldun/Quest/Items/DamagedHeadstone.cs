using System;

using Server;
using Server.Mobiles;
using Server.Items;
using Server.Network;
using Server.Gumps;
using Server.SkillHandlers;
using Server.Engines.Quests;

// 1158607 => brit
// 1158608 => vesper
// 1158609 => moonglow
// 1158610 => yew

namespace Server.Engines.Khaldun
{
    public class DamagedHeadstone : Item, IForensicTarget
    {
        public override int LabelNumber { get { return 1158561; } } // damaged headstone

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

            var quest = QuestHelper.GetQuest<GoingGumshoeQuest2>((PlayerMobile)m);

            if (quest != null)
            {
                m.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1158571, m.NetState); // *You examine the headstone...*
                m.SendLocalizedMessage(1158562, null, 0x23); // The damage to the epitaph seems deliberate.  Using your training from Inspector Jasper you have found a hidden message among the scratches. You recreate the original epitaph in your mind's eye...

                m.SendSound(quest.UpdateSound);

                var gump = new Gump(50, 50);

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

            writer.Write((int)0); // version

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
