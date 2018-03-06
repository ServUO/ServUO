using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;

namespace Server.Mobiles
{
    public class SpawnerGump : Gump
    {
        public static readonly int MaxEntries = 13;

        private readonly Spawner m_Spawner;

        public SpawnerGump(Spawner spawner)
            : base(50, 50)
        {
            m_Spawner = spawner;

            AddPage(0);

            AddBackground(0, 0, 410, 381, 5054);

            AddLabel(75, 1, 0, "Spawn List");
            AddLabel(335, 1, 0, "Max");
            AddLabel(378, 1, 0, "Total");

            AddButton(5, 310, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
            AddLabel(38, 310, 0x384, "Apply");

            AddButton(5, 333, 0xFA8, 0xFAB, 1025, GumpButtonType.Reply, 0);
            AddLabel(38, 333, 0x384, "Props");

            AddButton(5, 356, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
            AddLabel(38, 356, 0x384, "Cancel");

            AddButton(110, 310, 0xFA5, 0xFA7, 1500, GumpButtonType.Reply, 0);
            AddLabel(143, 310, 0x384, String.Format("Running: {0}", m_Spawner.Running ? "Yes" : "No"));

            AddButton(110, 333, 0xFA5, 0xFA7, 1000, GumpButtonType.Reply, 0);
            AddLabel(143, 333, 0x384, String.Format("Group: {0}", m_Spawner.Group ? "Yes" : "No"));

            AddButton(110, 356, 0xFB4, 0xFB6, 2, GumpButtonType.Reply, 0);
            AddLabel(143, 356, 0x384, "Bring to Home");

            AddButton(270, 333, 0xFA8, 0xFAA, 3, GumpButtonType.Reply, 0);
            AddLabel(303, 333, 0x384, "Total Respawn");

            AddButton(270, 356, 0xFA8, 0xFAA, 1750, GumpButtonType.Reply, 0);
            AddLabel(303, 356, 0x384, "Total Reset");

            AddImageTiled(350, 306, 30, 23, 0xA40);
            AddImageTiled(351, 307, 28, 21, 0xBBC);

            AddLabel(270, 306, 0x384, "Max Spawn:");
            AddTextEntry(353, 307, 28, 21, 0, 500, m_Spawner.MaxCount.ToString());

            AddLabel(382, 307, 0, m_Spawner.SpawnCount.ToString());

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

                if (i < spawner.SpawnObjects.Count)
                {
                    var so = spawner.SpawnObjects[i];

                    str = so.SpawnName;
                    max = so.MaxCount;

                    int count = m_Spawner.CountCreatures(so);
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
                m_Spawner.MaxCount = Math.Max(0, Utility.ToInt32(tr.Text));
            }

            for (int i = 0; i < MaxEntries; i++)
            {
                TextRelay te = info.GetTextEntry(i);
                TextRelay te2 = info.GetTextEntry(i + 20);

                SpawnObject so = i < m_Spawner.SpawnObjects.Count ? m_Spawner.SpawnObjects[i] : null;

                if (te != null)
                {
                    string name = te.Text;
                    string maxCount = te2 != null ? te2.Text : null;
                    int max = 0;

                    if (name.Length > 0)
                    {
                        name = name.Trim();

                        if (!String.IsNullOrEmpty(maxCount))
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
                            m_Spawner.AddSpawnObject(new SpawnObject(name, max));
                        }
                    }
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Spawner.Deleted || state.Mobile.AccessLevel < AccessLevel.GameMaster)
                return;

            switch ( info.ButtonID )
            {
                case 0: // Closed
                    {
                        return;
                    }
                case 1: // Apply
                    {
                        UpdateSpawnObjects(info, state.Mobile);

                        break;
                    }
                case 2: // Bring to Home
                    {
                        m_Spawner.BringToHome();

                        break;
                    }
                case 3: // Total Respawn
                    {
                        m_Spawner.Respawn();

                        break;
                    }
                case 1000:
                    {
                        if (m_Spawner.Group)
                            m_Spawner.Group = false;
                        else
                            m_Spawner.Group = true;
                        break;
                    }
                case 1025:
                    {
                        state.Mobile.SendGump(new PropertiesGump(state.Mobile, m_Spawner));
                        break;
                    }
                case 1500:
                    {
                        if (m_Spawner.Running)
                            m_Spawner.Running = false;
                        else
                            m_Spawner.Running = true;
                        break;
                    }
                case 1750:
                    {
                        m_Spawner.RemoveSpawned();
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
                            UpdateSpawnObjects(info, state.Mobile);

                            if (type == 0) // Spawn creature
                                m_Spawner.Spawn(index);
                            else // Remove creatures
                                m_Spawner.RemoveSpawned(index);
                        }

                        break;
                    }
            }

            state.Mobile.SendGump(new SpawnerGump(m_Spawner));
        }
    }
}