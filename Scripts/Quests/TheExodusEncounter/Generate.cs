using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using System.Linq;
using Server.Commands;

namespace Server.Engines.Blackthorn
{
    public static class GenerateTheExodusEncounter
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenExodusQuest", AccessLevel.Administrator, Generate);
        }

        public static void Generate(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

            Generate(Map.Ilshenar);

            CommandSystem.Handle(m, Server.Commands.CommandSystem.Prefix + "XmlLoad Spawns/TheExodusEncounterQuest.xml");
        }

        public static void Generate(Map map)
        {
            PublicMoongate gate = new PublicMoongate();
            gate.MoveToWorld(new Point3D(761, 641, 0), map);

            AnkhOfSacrificeAddon ankh = new AnkhOfSacrificeAddon(true);
            ankh.MoveToWorld(new Point3D(757, 641, 0), map);
        }
    }
}