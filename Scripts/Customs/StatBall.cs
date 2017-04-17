//Version 1.1

using System;
using System.Collections;
using Server;
using Server.Prompts;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using Server.Items;

namespace Server.Items
{
    public class StatBall : Item
    {
        [Constructable]
        public StatBall() : base(0xE73)
        {
            Weight = 1.0;
            Hue = 39;
            Name = "a stat ball";
            LootType = LootType.Blessed;
            Movable = true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }
            else if (from is PlayerMobile)
            {
                from.SendGump(new StatBallGump((PlayerMobile)from, this));
            }
        }

        public override bool DisplayLootType { get { return false; } }

        public StatBall(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}

namespace Server.Items
{
    public class StatBallGump : Gump
    {
        //-----------------
        // Edit to your liking or leave blank for default values
        //------------------------
        private int statcap;
        private int defaultStr;
        private int defaultDex;
        private int defaultInt;
        private int maxStr;
        private int maxDex;
        private int maxInt;

        //-----------------------

        private PlayerMobile m_From;
        private StatBall m_Ball;

        private int str;
        private int dex;
        private int intel;


        public StatBallGump(PlayerMobile from, StatBall ball) : base(50, 50)
        {
            m_From = from;
            m_Ball = ball;

            if (statcap <= 0)
            {
                statcap = m_From.StatCap;
            }
            if (defaultStr <= 0)
            {
                defaultStr = 50;
            }
            if (defaultDex <= 0)
            {
                defaultDex = 50;
            }
            if (defaultInt <= 0)
            {
                defaultInt = 50;
            }
            if (maxStr <= 0)
            {
                maxStr = 100;
            }
            if (maxDex <= 0)
            {
                maxDex = 100;
            }
            if (maxInt <= 0)
            {
                maxInt = 100;
            }

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(50, 50, 437, 215, 9200);
            this.AddLabel(200, 67, 1160, "Stat Ball Selection");
            this.AddLabel(114, 96, 1160, "Choose your Strength, Dexterity, and Intelligence");
            this.AddLabel(69, 156, 1152, "STR");
            this.AddLabel(213, 156, 1152, "DEX");
            this.AddLabel(353, 156, 1152, "INT");
            this.AddTextEntry(109, 156, 32, 20, 1359, 0, defaultStr.ToString());
            this.AddTextEntry(253, 156, 32, 20, 1359, 1, defaultDex.ToString());
            this.AddTextEntry(393, 156, 32, 20, 1359, 2, defaultInt.ToString());
            this.AddLabel(139, 156, 1152, " / " + maxStr.ToString());
            this.AddLabel(283, 156, 1152, " / " + maxDex.ToString());
            this.AddLabel(423, 156, 1152, " / " + maxInt.ToString());
            this.AddButton(405, 221, 238, 240, 4, GumpButtonType.Reply, 0);
            this.AddLabel(140, 200, 1152, "* Stat totals should equal " + statcap + " *");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Ball.Deleted)
                return;


            TextRelay s = info.GetTextEntry(0);
            try
            {
                str = Convert.ToInt32(s.Text);
            }
            catch
            {
                m_From.SendMessage("Bad strength entry. A number was expected.");
            }

            TextRelay d = info.GetTextEntry(1);
            try
            {
                dex = Convert.ToInt32(d.Text);
            }
            catch
            {
                m_From.SendMessage("Bad dexterity entry. A number was expected.");
            }

            TextRelay i = info.GetTextEntry(2);
            try
            {
                intel = Convert.ToInt32(i.Text);
            }
            catch
            {
                m_From.SendMessage("Bad intelligence entry. A number was expected.");
            }

            if (str > 0 && dex > 0 && intel > 0)
            {
                //  Uncomment the line line below, and add a comment to the line under it to use a defined number instead of the standard Stat Cap
                //              if ( ( ( str + dex + intel ) > Cap ) || ( ( str + dex + intel ) < Cap ) || ( str < 10 ) || ( dex < 10 ) || ( intel < 10 ) )
                if (((str + dex + intel) > statcap) || ((str + dex + intel) < statcap) || (str < 10) || (dex < 10) || (intel < 10) || (str > maxStr) || (dex > maxDex) || (intel > maxInt))
                    m_From.SendMessage("Your choice totals are invalid.  Please try again!");

                else
                {
                    m_From.RawStr = str;
                    m_From.RawDex = dex;
                    m_From.RawInt = intel;

                    m_Ball.Delete();
                }
            }
        }
    }
}