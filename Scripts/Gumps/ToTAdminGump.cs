using System;
using Server.Commands;
using Server.Misc;
using Server.Network;

namespace Server.Gumps
{
    public class ToTAdminGump : Gump
    {
        private static readonly string[] m_ToTInfo =
        {
            //Opening Message
            "<center>Treasures of Tokuno Admin</center><br>" +
            "-Use the gems to switch eras<br>" +
            "-Drop era and Reward era can be changed seperately<br>" +
            "-Drop era can be deactivated, Reward era is always activated",
            //Treasures of Tokuno 1 message
                                                                           "<center>Treasures of Tokuno 1</center><br>" +
                                                                           "-10 charge Bleach Pigment can drop as a Lesser Artifact<br>" +
                                                                           "-50 charge Neon Pigments available as a reward<br>",
            //Treasures of Tokuno 2 message
                                                                                                                                "<center>Treasures of Tokuno 2</center><br>" +
                                                                                                                                "-30 types of 1 charge Metallic Pigments drop as Lesser Artifacts<br>" +
                                                                                                                                "-1 charge Bleach Pigment can drop as a Lesser Artifact<br>" +
                                                                                                                                "-10 charge Greater Metallic Pigments available as a reward",
            //Treasures of Tokuno 3 message
                                                                                                                                                                                             "<center>Treasures of Tokuno 3</center><br>" +
                                                                                                                                                                                             "-10 types of 1 charge Fresh Pigments drop as Lesser Artifacts<br>" +
                                                                                                                                                                                             "-1 charge Bleach Pigment can drop as a Lesser Artifact<br>" +
                                                                                                                                                                                             "-Leurocian's Mempo Of Fortune can drop as a Lesser Artifact"
        };
        private readonly int m_ToTEras;
        public ToTAdminGump()
            : base(30, 50)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
			
            this.m_ToTEras = Enum.GetValues(typeof(TreasuresOfTokunoEra)).Length - 1;
			
            this.AddPage(0);
            this.AddBackground(0, 0, 320, 75 + (this.m_ToTEras * 25), 9200);
            this.AddImageTiled(25, 18, 270, 10, 9267);
            this.AddLabel(75, 5, 54, "Treasures of Tokuno Admin");
            this.AddLabel(10, 25, 54, "ToT Era");
            this.AddLabel(90, 25, 54, "Drop Era");
            this.AddLabel(195, 25, 54, "Reward Era");
            this.AddLabel(287, 25, 54, "Info");
			
            this.AddBackground(320, 0, 200, 150, 9200);		
            this.AddImageTiled(325, 5, 190, 140, 2624);
            this.AddAlphaRegion(325, 5, 190, 140);
			
            this.SetupToTEras();
        }

        public static void Initialize()
        {
            CommandSystem.Register("ToTAdmin", AccessLevel.Administrator, new CommandEventHandler(ToTAdmin_OnCommand));
        }

        [Usage("ToTAdmin")]
        [Description("Displays a menu to configure Treasures of Tokuno.")]
        public static void ToTAdmin_OnCommand(CommandEventArgs e)
        {
            ToTAdminGump tg;
			
            tg = new ToTAdminGump();
            e.Mobile.CloseGump(typeof(ToTAdminGump));
            e.Mobile.SendGump(tg);
        }

        public void SetupToTEras()
        {
            bool isActivated = TreasuresOfTokuno.DropEra != TreasuresOfTokunoEra.None;
            this.AddButton(75, 50, isActivated ? 2361 : 2360, isActivated ? 2361 : 2360, 1, GumpButtonType.Reply, 0);
            this.AddLabel(90, 45, isActivated ? 167 : 137, isActivated ? "Activated" : "Deactivated");
			
            for (int i = 0; i < this.m_ToTEras; i++)
            {
                int yoffset = (i * 25);
				
                bool isThisDropEra = ((int)TreasuresOfTokuno.DropEra - 1) == i;
                bool isThisRewardEra = ((int)TreasuresOfTokuno.RewardEra - 1) == i;
                int dropButtonID = isThisDropEra ? 2361 : 2360;
                int rewardButtonID = isThisRewardEra ? 2361 : 2360;
				
                this.AddLabel(10, 70 + yoffset, 2100, "ToT " + (i + 1));
                this.AddButton(75, 75 + yoffset, dropButtonID, dropButtonID, 2 + (i * 2), GumpButtonType.Reply, 0);
                this.AddLabel(90, 70 + yoffset, isThisDropEra ? 167 : 137, isThisDropEra ? "Active" : "Inactive");
                this.AddButton(180, 75 + yoffset, rewardButtonID, rewardButtonID, 2 + (i * 2) + 1, GumpButtonType.Reply, 0);
                this.AddLabel(195, 70 + yoffset, isThisRewardEra ? 167 : 137, isThisRewardEra ? "Active" : "Inactive");
				
                this.AddButton(285, 70 + yoffset, 4005, 4006, i, GumpButtonType.Page, 2 + i);
            }
			
            for (int i = 0; i < m_ToTInfo.Length; i++)
            {
                this.AddPage(1 + i);
                this.AddHtml(330, 10, 180, 130, m_ToTInfo[i], false, true);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int button = info.ButtonID;
            Mobile from = sender.Mobile;

            if (button == 1)
            {
                TreasuresOfTokuno.DropEra = TreasuresOfTokunoEra.None;
                from.SendMessage("Treasures of Tokuno Drops have been deactivated");
            }
            else if (button >= 2)
            {
                int selectedToT;
                if (button % 2 == 0)
                {
                    selectedToT = button / 2;
                    TreasuresOfTokuno.DropEra = (TreasuresOfTokunoEra)(selectedToT);
                    from.SendMessage("Treasures of Tokuno " + selectedToT + " Drops have been enabled");
                }
                else
                {
                    selectedToT = (button - 1) / 2;
                    TreasuresOfTokuno.RewardEra = (TreasuresOfTokunoEra)(selectedToT);
                    from.SendMessage("Treasures of Tokuno " + selectedToT + " Rewards have been enabled");
                }
            }
        }
    }
}