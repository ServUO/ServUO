using System;
using System.Collections.Generic;

using Server;
using Server.Prompts;
using Server.Mobiles;
using Server.Items;
using Server.SkillHandlers;
using Server.Gumps;
using Server.Network;
using Server.Engines.Quests;

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

            var quest = QuestHelper.GetQuest<GoingGumshoeQuest2>((PlayerMobile)m);

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

                    var gump = new Gump(50, 50);
                    gump.AddBackground(0, 0, 500, 500, 9380);

                    gump.AddItem(84, 130, ItemID, Hue);
                    gump.AddHtml(167, 50, 310, 20, "<center><basefont color=#B22222>a dust pile</center>", false, false);
                    gump.AddHtmlLocalized(167, 70, 310, 380, 1158617, true, false);

                    m.SendGump(gump);

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

            writer.Write((int)0); // version
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
