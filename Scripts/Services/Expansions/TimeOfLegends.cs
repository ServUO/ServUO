using System;
using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Quests;
using Server.Engines.CannedEvil;
using Server.Engines.Shadowguard;
using Server.Gumps;

namespace Server
{
	public static class TimeOfLegends
	{
        public static void Initialize()
        {
            CommandSystem.Register("DecorateTOL", AccessLevel.GameMaster, new CommandEventHandler(DecorateTOL_OnCommand));

            if (DateTime.UtcNow < _EndCurrencyWarning)
                EventSink.Login += new LoginEventHandler(OnLogin);
        }

        private  static readonly DateTime _EndCurrencyWarning = new DateTime(2017, 3, 1, 1, 1, 1);

		private static Type[] m_PigmentList = new Type[]
		{
			typeof(AnonsBoots),					typeof(AnonsBootsGargoyle),			typeof(AnonsSpellbook),			typeof(BalakaisShamanStaff),
			typeof(BalakaisShamanStaffGargoyle),typeof(EnchantressCameo),			typeof(GrugorsShield),			typeof(GrugorsShieldGargoyle),
			typeof(HalawasHuntingBow),			typeof(HalawasHuntingBowGargoyle),	typeof(HawkwindsRobe),			typeof(JumusSacredHide),
			typeof(JumusSacredHideGargoyle), 	typeof(JuonarsGrimoire), 			typeof(LereisHuntingSpear), 	typeof(LereisHuntingSpearGargoyle), 
			typeof(MinaxsSandles), 				typeof(MinaxsSandlesGargoyle), 		typeof(MocapotilsObsidianSword),typeof(OzymandiasObi),
			typeof(OzymandiasObiGargoyle), 		typeof(ShantysWaders), 				typeof(ShantysWadersGargoyle), 	typeof(TotemOfTheTribe),
			typeof(WamapsBoneEarrings), 		typeof(WamapsBoneEarringsGargoyle), typeof(UnstableTimeRift)
		};
		
		public static Type[] PigmentList{ get{ return m_PigmentList; } }

        public static bool FindItem(int x, int y, int z, Map map, Item test)
        {
            return FindItem(new Point3D(x, y, z), map, test);
        }

        public static bool FindItem(Point3D p, Map map, Item test)
        {
            IPooledEnumerable eable = map.GetItemsInRange(p);

            foreach (Item item in eable)
            {
                if (item.Z == p.Z && item.ItemID == test.ItemID)
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }

        [Usage("DecorateTOL")]
        [Description("Generates Time of Legends world decoration.")]
        private static void DecorateTOL_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Generating Time Of Legends world decoration, please wait.");

            Decorate.Generate("tol", "Data/Decoration/TimeOfLegends/TerMur", Map.TerMur);
            Decorate.Generate("tol", "Data/Decoration/TimeOfLegends/Felucca", Map.Felucca);

            ChampionSpawn sp = new ChampionSpawn();

            sp.Type = ChampionSpawnType.DragonTurtle;
            sp.MoveToWorld(new Point3D(451, 1696, 65), Map.TerMur);
            sp.Active = true;
            WeakEntityCollection.Add("tol", sp);

            sp = new ChampionSpawn();
            sp.SpawnRadius = 35;
            sp.Type = ChampionSpawnType.DragonTurtle;
            sp.MoveToWorld(new Point3D(7042, 1889, 60), Map.Felucca);
            sp.Active = true;
            WeakEntityCollection.Add("tol", sp);

            PublicMoongate gate = new PublicMoongate();
            gate.MoveToWorld(new Point3D(719, 1863, 40), Map.TerMur);

            ShadowguardController.SetupShadowguard(e.Mobile);
            Server.Engines.MyrmidexInvasion.GenerateMyrmidexQuest.Generate();

            MacawSpawner.Generate();

            CommandSystem.Handle(e.Mobile, Server.Commands.CommandSystem.Prefix + "XmlLoad Spawns/Eodon.xml");

            e.Mobile.SendMessage("Time Of Legends world generating complete.");
        }

        public static void OnLogin(LoginEventArgs e)
        {
            if (e.Mobile is PlayerMobile && e.Mobile.AccessLevel == AccessLevel.Player)
                Timer.DelayCall(TimeSpan.FromSeconds(5), () =>
                    {
                        if (!e.Mobile.HasGump(typeof(NewCurrencyHelpGump)))
                            e.Mobile.SendGump(new NewCurrencyHelpGump());
                    });
        }
	}
}