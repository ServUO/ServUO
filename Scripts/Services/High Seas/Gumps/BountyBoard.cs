using Server;
using System;
using Server.Mobiles;
using Server.Gumps;
using System.Collections.Generic;
using Server.Network;
using Server.Engines.Quests;

namespace Server.Items
{
    public class ProfessionalBountyBoard : Item
    {
        [Constructable]
        public ProfessionalBountyBoard() : base(7774)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(BountyBoardGump)))
                from.SendGump(new BountyBoardGump());
        }

        public ProfessionalBountyBoard(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BountyBoardGump : Gump
    {
        public BountyBoardGump() : this(0) { }

        public BountyBoardGump(int index) : base(20, 20)
        {
            int darkHue = 19686;
            int lightHue = 19884;

            AddAlphaRegion(50, 50, 50, 50);
            AddImage(0, 0, 5400);

            AddHtmlLocalized(172, 37, 200, 16, 1116689, darkHue, false, false);   // WANTED FOR PIRACY
            AddHtmlLocalized(166, 320, 200, 16, 1116703, darkHue, false, false);  // WANTED DEAD OR ALIVE

            AddHtmlLocalized(180, 135, 200, 16, 1116704, lightHue, false, false); //Notice to all sailors
            AddHtmlLocalized(130, 150, 300, 16, 1116705, lightHue, false, false); //There be a bounty on these lowlifes!
            AddHtmlLocalized(150, 170, 300, 16, 1116706, lightHue, false, false); //See officers fer information.
            AddHtmlLocalized(195, 190, 300, 16, 1116707, lightHue, false, false); //********

            if (index < 0) 
                index = 0;
            if(index >= BountyQuestSpawner.Bounties.Count)
                index = BountyQuestSpawner.Bounties.Count - 1;

            List<Mobile> mobs = new List<Mobile>(BountyQuestSpawner.Bounties.Keys);

            if (mobs.Count == 0)
                return;

            int y = 210;
            int idx = 0;
            int reward = 1116696; //Reward: ~1_val~
            int cliloc = 1116690; //~1_val~ ~2_val~ ~3_val~
            for (int i = index; i < mobs.Count; i++)
            {
                if(idx++ > 4)
                    break;

                Mobile mob = mobs[i];
                int toReward = 1000;

                BountyQuestSpawner.Bounties.TryGetValue(mob, out toReward);
                PirateCaptain capt = mob as PirateCaptain;

                if (capt == null)
                    continue;

                string args;

                if (capt.PirateName > 0)
                    args = String.Format("#{0}\t#{1}\t#{2}", capt.Adjective, capt.Noun, capt.PirateName);
                else
                    args = String.Format("#{0}\t#{1}\t{2}", capt.Adjective, capt.Noun, capt.Name);

                AddHtmlLocalized(110, y, 160, 16, cliloc, args, lightHue, false, false);
                AddHtmlLocalized(280, y, 125, 16, reward, toReward.ToString(), lightHue, false, false);

                reward++;
                cliloc++;
                y += 16;
            }

            AddButton(362, 115, 2084, 2084, 1 + index, GumpButtonType.Reply, 0);
            AddButton(362, 342, 2085, 2085, 500 + index, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0)
                return;

            if (info.ButtonID < 500)
            {
                int idx = info.ButtonID - 1;
                from.SendGump(new BountyBoardGump(idx - 1));
            }
            else
            {
                int idx = info.ButtonID - 500;
                from.SendGump(new BountyBoardGump(idx + 1));
            }
        }
    }
}