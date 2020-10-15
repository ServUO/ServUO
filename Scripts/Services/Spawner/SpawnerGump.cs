using Server.Gumps;
using System;

namespace Server.Mobiles
{
    public class SpawnerGump : BaseGump
    {
        public static readonly int MaxEntries = 13;
        public Spawner Spawner { get; set; }

        public int LabelHue => User?.NetState != null && User.NetState.IsEnhancedClient ? 0x386 : 0x384;

        public SpawnerGump(IEntity m, Spawner spawner)
            : base(m as PlayerMobile)
        {
            Spawner = spawner;
        }

        public override void AddGumpLayout()
        {
            AddPage(0);

            AddBackground(0, 0, 410, 381, 5054);

            AddLabel(75, 1, 0, "Spawn List");
            AddLabel(335, 1, 0, "Max");
            AddLabel(378, 1, 0, "Total");

            AddButton(5, 310, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
            AddLabel(38, 310, LabelHue, "Apply");

            AddButton(5, 333, 0xFA8, 0xFAB, 1025, GumpButtonType.Reply, 0);
            AddLabel(38, 333, LabelHue, "Props");

            AddButton(5, 356, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
            AddLabel(38, 356, LabelHue, "Cancel");

            AddButton(110, 310, 0xFA5, 0xFA7, 1500, GumpButtonType.Reply, 0);
            AddLabel(143, 310, LabelHue, string.Format("Running: {0}", Spawner.Running ? "Yes" : "No"));

            AddButton(110, 333, 0xFA5, 0xFA7, 1000, GumpButtonType.Reply, 0);
            AddLabel(143, 333, LabelHue, string.Format("Group: {0}", Spawner.Group ? "Yes" : "No"));

            AddButton(110, 356, 0xFB4, 0xFB6, 2, GumpButtonType.Reply, 0);
            AddLabel(143, 356, LabelHue, "Bring to Home");

            AddButton(270, 333, 0xFA8, 0xFAA, 3, GumpButtonType.Reply, 0);
            AddLabel(303, 333, LabelHue, "Total Respawn");

            AddButton(270, 356, 0xFA8, 0xFAA, 1750, GumpButtonType.Reply, 0);
            AddLabel(303, 356, LabelHue, "Total Reset");

            AddImageTiled(350, 306, 30, 23, 0xA40);
            AddImageTiled(351, 307, 28, 21, 0xBBC);

            AddLabel(270, 306, LabelHue, "Max Spawn:");
            AddTextEntry(353, 307, 28, 21, 0, 500, Spawner.MaxCount.ToString());

            AddLabel(382, 307, 0, Spawner.SpawnCount.ToString());

            for (int i = 0; i < MaxEntries; i++)
            {
                AddButton(5, (22 * i) + 20, 0xFA5, 0xFA7, 4 + (i * 2), GumpButtonType.Reply, 0);
                AddButton(38, (22 * i) + 20, 0xFA2, 0xFA4, 5 + (i * 2), GumpButtonType.Reply, 0);

                AddImageTiled(71, (22 * i) + 20, 279, 23, 0xA40);
                AddImageTiled(72, (22 * i) + 21, 277, 21, 0xBBC);

                AddImageTiled(330, (22 * i) + 20, 50, 23, 0xA40);
                AddImageTiled(331, (22 * i) + 21, 48, 21, 0xBBC);

                string str = "";
                int max = 0;

                if (i < Spawner.SpawnObjects.Count)
                {
                    SpawnObject so = Spawner.SpawnObjects[i];

                    str = so.SpawnName;
                    max = so.MaxCount;

                    int count = Spawner.CountCreatures(so);
                    AddLabel(382, (22 * i) + 20, 0, count.ToString());
                }

                AddTextEntry(75, (22 * i) + 21, 304, 21, 0, i, str);
                AddTextEntry(332, (22 * i) + 21, 28, 21, 0, i + 20, max.ToString());
            }
        }

        public void UpdateSpawnObjects(RelayInfo info, Mobile from)
        {
            TextRelay tr = info.GetTextEntry(500);

            if (tr != null && tr.Text.Length > 0)
            {
                Spawner.MaxCount = Math.Max(0, Utility.ToInt32(tr.Text));
            }

            for (int i = 0; i < MaxEntries; i++)
            {
                TextRelay te = info.GetTextEntry(i);
                TextRelay te2 = info.GetTextEntry(i + 20);

                SpawnObject so = i < Spawner.SpawnObjects.Count ? Spawner.SpawnObjects[i] : null;

                if (te != null)
                {
                    string name = te.Text;
                    string maxCount = te2?.Text;
                    int max = 0;

                    if (name.Length > 0)
                    {
                        name = name.Trim();

                        if (!string.IsNullOrEmpty(maxCount))
                        {
                            max = Utility.ToInt32(maxCount);
                        }

                        max = Math.Max(0, max);

                        string t = Spawner.ParseType(name);
                        Type type = ScriptCompiler.FindTypeByName(t);

                        if (type == null)
                        {
                            from.SendMessage("{0} is not a valid type name.", t);
                            continue;
                        }

                        if (so != null)
                        {
                            if (so.SpawnName != name)
                                so.SpawnName = name;

                            if (so.MaxCount != max)
                                so.MaxCount = max;
                        }
                        else
                        {
                            Spawner.AddSpawnObject(new SpawnObject(name, max));
                        }
                    }
                }
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (Spawner.Deleted || User.AccessLevel < AccessLevel.GameMaster)
                return;

            switch (info.ButtonID)
            {
                case 0: // Closed
                {
                    return;
                }
                case 1: // Apply
                {
                    UpdateSpawnObjects(info, User);

                    break;
                }
                case 2: // Bring to Home
                {
                    Spawner.BringToHome();

                    break;
                }
                case 3: // Total Respawn
                {
                    Spawner.Respawn();

                    break;
                }
                case 1000:
                {
                    Spawner.Group = !Spawner.Group;
                    break;
                }
                case 1025:
                {
                    User.SendGump(new PropertiesGump(User, Spawner));
                    break;
                }
                case 1500:
                {
                    Spawner.Running = !Spawner.Running;
                    break;
                }
                case 1750:
                {
                    Spawner.RemoveSpawned();
                    break;
                }
                default:
                {
                    int buttonID = info.ButtonID - 4;
                    int index = buttonID / 2;
                    int type = buttonID % 2;

                    TextRelay entry = info.GetTextEntry(index);

                    if (entry != null && entry.Text.Length > 0)
                    {
                        UpdateSpawnObjects(info, User);

                        if (type == 0) // Spawn creature
                            Spawner.Spawn(index);
                        else // Remove creatures
                            Spawner.RemoveSpawned(index);
                    }

                    break;
                }
            }

            Refresh();
        }
    }
}
