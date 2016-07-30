using Server;
using System;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Commands;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
    public static class NewMaginciaCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("ViewLottos", AccessLevel.GameMaster, new CommandEventHandler(ViewLottos_OnCommand));

            CommandSystem.Register("GenNewMagincia", AccessLevel.GameMaster, new CommandEventHandler(GenNewMagincia_OnCommand));
            CommandSystem.Register("DeleteNewMagincia", AccessLevel.Administrator, Delete);
        }

        private static void Delete(CommandEventArgs e)
        {
        }

        public static void GenNewMagincia_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.SendMessage("Generating New Magincia Bazaar System...");

            if (MaginciaBazaar.Instance == null)
            {
                MaginciaBazaar.Instance = new MaginciaBazaar();
                MaginciaBazaar.Instance.MoveToWorld(new Point3D(3729, 2058, 5), Map.Trammel);
                Console.WriteLine("Generated {0} New Magincia Bazaar Stalls.", MaginciaBazaar.Plots.Count);
            }
            else
                Console.WriteLine("Magincia Bazaar System already exists!");

            Console.WriteLine("Generating New Magincia Housing Lotty System..");

            if (MaginciaLottoSystem.Instance == null)
            {
                MaginciaLottoSystem.Instance = new MaginciaLottoSystem();
                MaginciaLottoSystem.Instance.MoveToWorld(new Point3D(3718, 2049, 5), Map.Trammel);

                Console.WriteLine("Generated {0} New Magincia Housing Plots.", MaginciaLottoSystem.Plots.Count);
            }
            else
                Console.WriteLine("Magincia Housing Lotto System already exists!");
        }

        public static void ViewLottos_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile.AccessLevel > AccessLevel.Player)
            {
                e.Mobile.CloseGump(typeof(LottoTrackingGump));
                e.Mobile.SendGump(new LottoTrackingGump());
            }
        }
    }
}