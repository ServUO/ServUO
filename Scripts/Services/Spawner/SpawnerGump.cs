using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;

namespace Server.Mobiles
{
    public class SpawnerGump : Gump
    {
        private readonly Spawner m_Spawner;
        public SpawnerGump(Spawner spawner)
            : base(50, 50)
        {
            this.m_Spawner = spawner;

            this.AddPage(0);

            this.AddBackground(0, 0, 410, 371, 5054);

            this.AddLabel(160, 1, 0, "Creatures List");

            this.AddButton(5, 347, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
            this.AddLabel(38, 347, 0x384, "Cancel");

            this.AddButton(5, 325, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
            this.AddLabel(38, 325, 0x384, "Apply");

            this.AddButton(110, 325, 0xFB4, 0xFB6, 2, GumpButtonType.Reply, 0);
            this.AddLabel(143, 325, 0x384, "Bring to Home");

            this.AddButton(110, 347, 0xFA8, 0xFAA, 3, GumpButtonType.Reply, 0);
            this.AddLabel(143, 347, 0x384, "Total Respawn");

            for (int i = 0; i < 13; i++)
            {
                this.AddButton(5, (22 * i) + 20, 0xFA5, 0xFA7, 4 + (i * 2), GumpButtonType.Reply, 0);
                this.AddButton(38, (22 * i) + 20, 0xFA2, 0xFA4, 5 + (i * 2), GumpButtonType.Reply, 0);

                this.AddImageTiled(71, (22 * i) + 20, 309, 23, 0xA40);
                this.AddImageTiled(72, (22 * i) + 21, 307, 21, 0xBBC);

                string str = "";

                if (i < spawner.SpawnNames.Count)
                {
                    str = (string)spawner.SpawnNames[i];
                    int count = this.m_Spawner.CountCreatures(str);

                    this.AddLabel(382, (22 * i) + 20, 0, count.ToString());
                }

                this.AddTextEntry(75, (22 * i) + 21, 304, 21, 0, i, str);
            }
        }

        public List<string> CreateArray(RelayInfo info, Mobile from)
        {
            List<string> creaturesName = new List<string>();

            for (int i = 0; i < 13; i++)
            {
                TextRelay te = info.GetTextEntry(i);

                if (te != null)
                {
                    string str = te.Text;

                    if (str.Length > 0)
                    {
                        str = str.Trim();

                        string t = Spawner.ParseType(str);

                        Type type = ScriptCompiler.FindTypeByName(t);

                        if (type != null)
                            creaturesName.Add(str);
                        else
                            from.SendMessage("{0} is not a valid type name.", t);
                    }
                }
            }

            return creaturesName;
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (this.m_Spawner.Deleted || state.Mobile.AccessLevel < AccessLevel.GameMaster)
                return;

            switch ( info.ButtonID )
            {
                case 0: // Closed
                    {
                        return;
                    }
                case 1: // Apply
                    {
                        this.m_Spawner.SpawnNames = this.CreateArray(info, state.Mobile);

                        break;
                    }
                case 2: // Bring to Home
                    {
                        this.m_Spawner.BringToHome();

                        break;
                    }
                case 3: // Total Respawn
                    {
                        this.m_Spawner.Respawn();

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
                            if (type == 0) // Spawn creature
                                this.m_Spawner.Spawn(entry.Text);
                            else // Remove creatures
                                this.m_Spawner.RemoveSpawned(entry.Text);

                            this.m_Spawner.SpawnNames = this.CreateArray(info, state.Mobile);
                        }

                        break;
                    }
            }

            state.Mobile.SendGump(new SpawnerGump(this.m_Spawner));
        }
    }
}