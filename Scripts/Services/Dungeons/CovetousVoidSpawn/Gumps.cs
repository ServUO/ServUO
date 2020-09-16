using Server.Engines.Points;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.VoidPool
{
    public enum ScoreType
    {
        Current,
        BestSingle,
        OverallTotal,
        BestWave
    }

    public class VoidPoolGump : Gump
    {
        public static readonly int Red = 0x4800;
        public static readonly int Orange = 0xB104;

        public PlayerMobile User { get; set; }
        public VoidPoolController Controller { get; set; }

        public VoidPoolGump(VoidPoolController controller, PlayerMobile pm) : base(50, 50)
        {
            Controller = controller;
            User = pm;

            AddGumpLayout();
        }

        public void AddGumpLayout()
        {
            AddBackground(0, 0, 400, 565, 9350);

            AddHtmlLocalized(10, 10, 200, 16, 1152531, Red, false, false); // The Void Pool
            AddHtmlLocalized(10, 30, 200, 16, Controller.Map == Map.Felucca ? 1012001 : 1012000, Red, false, false); // FEl/Tram

            if (Controller.OnGoing)
            {
                AddHtmlLocalized(10, 50, 200, 16, 1152914, Orange, false, false); // Current Battle:
                AddHtmlLocalized(180, 50, 200, 16, 1152915, Controller.Wave.ToString(), Orange, false, false); // Wave ~1_WAVE~
            }
            else
            {
                AddHtmlLocalized(10, 50, 200, 16, 1152916, Orange, false, false); // Next Battle:

                if (Controller.NextStart > DateTime.UtcNow)
                    AddHtmlLocalized(180, 50, 200, 16, 1152917, ((int)(Controller.NextStart - DateTime.UtcNow).TotalMinutes).ToString(), Orange, false, false); // Starts in ~1_MIN~ min.
            }

            AddButton(140, 70, 4005, 4006, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(180, 70, 200, 16, 1152535, Orange, false, false); 		//Current Battle Scoreboard

            AddButton(140, 90, 4005, 4006, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(180, 90, 200, 16, 1152536, Orange, false, false); 		//Best Single Battle Scoreboard

            AddButton(140, 110, 4005, 4006, 3, GumpButtonType.Reply, 0);
            AddHtmlLocalized(180, 110, 200, 16, 1152537, Orange, false, false); 		//Overall Total Scores

            VoidPoolStats stats = VoidPoolStats.GetStats(Controller);

            if (stats.BestWave != null)
            {
                AddButton(140, 130, 4005, 4006, 4, GumpButtonType.Reply, 0);
                AddHtml(180, 130, 200, 16, string.Format("<basefont color=#A52A2A>Best Wave: {0}", stats.BestWave.Waves.ToString()), false, false);
            }

            AddHtmlLocalized(10, 150, 400, 16, 1152552, Orange, false, false);          // See Loyalty Menu for Reward Points
            AddHtmlLocalized(10, 170, 400, 16, 1152553, Orange, false, false);          // See Vela in Cove for rewards

            AddHtmlLocalized(10, 190, 380, 175, 1152533, Orange, true, true);
            AddHtmlLocalized(10, 375, 380, 175, 1152534, Orange, true, true);
        }

        public override void OnResponse(Network.NetState state, RelayInfo info)
        {
            VoidPoolStats stats = VoidPoolStats.GetStats(Controller);

            switch (info.ButtonID)
            {
                default: break;
                case 1:
                    User.SendGump(new ScoresGump(Controller, User, ScoreType.Current));
                    break;
                case 2:
                    User.SendGump(new ScoresGump(Controller, User, ScoreType.BestSingle));
                    break;
                case 3:
                    User.SendGump(new ScoresGump(Controller, User, ScoreType.OverallTotal));
                    break;
                case 4:
                    if (stats.BestWave != null)
                        User.SendGump(new ScoresGump(Controller, User, ScoreType.BestWave));
                    break;
            }
        }
    }

    public class ScoresGump : Gump
    {
        public static readonly int Red = 0x4800;
        public static readonly int Orange = 0xB104;

        public PlayerMobile User { get; private set; }
        public VoidPoolController Controller { get; private set; }
        public Dictionary<Mobile, long> Score { get; private set; }
        public ScoreType ScoreType { get; set; }

        public ScoresGump(VoidPoolController controller, PlayerMobile pm, ScoreType type)
            : base(50, 50)
        {
            Controller = controller;
            User = pm;
            ScoreType = type;

            AddGumpLayout();
        }

        public void AddGumpLayout()
        {
            int page = 0;
            object loc;
            AddBackground(0, 0, 500, 620, 9350);
            AddPage(page);

            VoidPoolStats stats = VoidPoolStats.GetStats(Controller);

            if (ScoreType == ScoreType.BestWave && stats.BestWave == null)
                return;

            switch (ScoreType)
            {
                default:
                case ScoreType.Current: Score = Controller.CurrentScore; loc = 1152535; break;
                case ScoreType.BestSingle: Score = stats.BestSingle; loc = 1152536; break;
                case ScoreType.OverallTotal: Score = stats.OverallTotal; loc = 1152537; break;
                case ScoreType.BestWave: Score = stats.BestWave.Score; loc = "Best Wave Scoreboard"; break;
            }

            AddHtmlLocalized(10, 10, 200, 16, 1152531, Red, false, false); // The Void Pool
            AddHtmlLocalized(10, 30, 200, 16, Controller.Map == Map.Felucca ? 1012001 : 1012000, Red, false, false); // FEl/Tram

            if (loc is int)
                AddHtmlLocalized(10, 50, 200, 16, (int)loc, Red, false, false);
            else if (loc is string)
                AddHtml(10, 50, 200, 16, string.Format("<basefont color=#8B0000>{0}", (string)loc), false, false);

            if (ScoreType == ScoreType.BestWave)
            {
                AddHtml(200, 30, 200, 16, string.Format("<basefont color=#8B0000>Total Waves: {0}", stats.BestWave.Waves.ToString()), false, false);
                AddHtml(200, 50, 200, 16, string.Format("<basefont color=#8B0000>Total Score: {0}", stats.BestWave.TotalScore.ToString()), false, false);
            }

            AddHtmlLocalized(10, 90, 100, 16, 1152541, Orange, false, false); // RANK
            AddHtmlLocalized(100, 90, 300, 16, 1152542, Orange, false, false); // PLAYER
            AddHtmlLocalized(400, 90, 100, 16, 1152543, Orange, false, false); // SCORE

            page++;
            AddPage(page);

            if (Score != null)
            {
                int index = 0;
                int yOffset = 0;

                foreach (KeyValuePair<Mobile, long> table in Score.OrderBy(kvp => -kvp.Value))
                {
                    AddHtml(10, 130 + (20 * yOffset), 50, 16, (index + 1).ToString(), false, false);
                    AddHtml(100, 130 + (20 * yOffset), 300, 16, table.Key.Name, false, false);
                    AddHtml(400, 130 + (20 * yOffset), 100, 16, table.Value.ToString(), false, false);

                    index++;
                    yOffset++;

                    if (index > 0 && index % 19 == 0 && index < Score.Count - 1)
                    {
                        page++;
                        yOffset = 0;

                        AddHtmlLocalized(320, 570, 100, 16, 1044045, Orange, false, false); // NEXT PAGE
                        AddButton(404, 570, 4005, 4006, 0, GumpButtonType.Page, page);

                        AddPage(page);

                        AddHtmlLocalized(120, 570, 100, 16, 1044044, Orange, false, false); // PREV PAGE
                        AddButton(80, 570, 4014, 4015, 0, GumpButtonType.Page, page - 1);
                    }
                }
            }
        }
    }

    public class VoidPoolRewardGump : BaseRewardGump
    {
        public VoidPoolRewardGump(Mobile owner, PlayerMobile user)
            : base(owner, user, VoidPoolRewards.Rewards, 1152531)
        {
        }

        public override double GetPoints(Mobile m)
        {
            return PointsSystem.VoidPool.GetPoints(m);
        }

        public override int GetItemHue(Item i, CollectionItem item)
        {
            int hue;

            if (i != null)
                hue = Points >= item.Points ? i.Hue : 0x3E9;
            else
                hue = Points >= item.Points ? CraftResources.GetHue((CraftResource)item.Hue) : 0x3E9;

            return hue;
        }

        public override void OnConfirmed(CollectionItem citem, int index)
        {
            Item item;

            if (index >= 0 && index <= 23)
            {
                item = Activator.CreateInstance(citem.Type, (CraftResource)citem.Hue) as Item;
            }
            else
                item = Activator.CreateInstance(citem.Type) as Item;

            if (item != null)
            {
                if (User.Backpack == null || !User.Backpack.TryDropItem(User, item, false))
                {
                    User.SendLocalizedMessage(1074361); // The reward could not be given.  Make sure you have room in your pack.
                    item.Delete();
                }
                else
                {
                    PointsSystem.VoidPool.DeductPoints(User, citem.Points);

                    User.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
                    User.PlaySound(0x5A7);
                }
            }
        }
    }
}
