using Server.Commands;
using Server.Engines.CannedEvil;
using Server.Engines.Shadowguard;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using System;

namespace Server
{
    public static class TimeOfLegends
    {
        public static void Initialize()
        {
            CommandSystem.Register("DecorateTOL", AccessLevel.GameMaster, DecorateTOL_OnCommand);

            if (DateTime.UtcNow < _EndCurrencyWarning)
                EventSink.Login += OnLogin;

            EventSink.CreatureDeath += CheckRecipeDrop;
        }

        private static readonly DateTime _EndCurrencyWarning = new DateTime(2017, 3, 1, 1, 1, 1);

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

            ChampionSpawn sp = new ChampionSpawn
            {
                Type = ChampionSpawnType.DragonTurtle
            };
            sp.MoveToWorld(new Point3D(451, 1696, 65), Map.TerMur);
            sp.Active = true;
            WeakEntityCollection.Add("tol", sp);

            sp = new ChampionSpawn
            {
                SpawnRadius = 35,
                SpawnMod = .5,
                KillsMod = .5,
                Type = ChampionSpawnType.DragonTurtle
            };
            sp.MoveToWorld(new Point3D(7042, 1889, 60), Map.Felucca);
            sp.Active = true;
            WeakEntityCollection.Add("tol", sp);

            PublicMoongate gate = new PublicMoongate();
            gate.MoveToWorld(new Point3D(719, 1863, 40), Map.TerMur);

            ShadowguardController.SetupShadowguard(e.Mobile);
            Engines.MyrmidexInvasion.GenerateMyrmidexQuest.Generate();

            MacawSpawner.Generate();

            CommandSystem.Handle(e.Mobile, CommandSystem.Prefix + "XmlLoad Spawns/Eodon.xml");

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

        public static void CheckRecipeDrop(CreatureDeathEventArgs e)
        {
            BaseCreature bc = e.Creature as BaseCreature;
            Container c = e.Corpse;
            Mobile killer = e.Killer;

            if (SpellHelper.IsEodon(c.Map, c.Location))
            {
                double chance = (double)bc.Fame / 1000000;
                int luck = 0;

                if (killer != null)
                {
                    luck = Math.Min(1800, killer is PlayerMobile ? ((PlayerMobile)killer).RealLuck : killer.Luck);
                }

                if (luck > 0)
                    chance += (double)luck / 152000;

                if (chance > Utility.RandomDouble())
                {
                    if (0.33 > Utility.RandomDouble())
                    {
                        Item item = Loot.Construct(_ArmorDropTypes[Utility.Random(_ArmorDropTypes.Length)]);

                        if (item != null)
                            c.DropItem(item);
                    }
                    else
                    {
                        Item scroll = new RecipeScroll(_RecipeTypes[Utility.Random(_RecipeTypes.Length)]);

                        if (scroll != null)
                            c.DropItem(scroll);
                    }
                }
            }
        }

        public static Type[] ArmorDropTypes => _ArmorDropTypes;
        private static readonly Type[] _ArmorDropTypes =
        {
            typeof(AloronsBustier), typeof(AloronsGorget), typeof(AloronsHelm), typeof(AloronsLegs), typeof(AloronsLongSkirt), typeof(AloronsSkirt), typeof(AloronsTunic), typeof(AloronsShorts),
            typeof(DardensBustier), typeof(DardensHelm), typeof(DardensLegs), typeof(DardensSleeves), typeof(DardensTunic)
        };

        public static int[] RecipeTypes => _RecipeTypes;
        private static readonly int[] _RecipeTypes =
        {
            560, 561, 562, 563, 564, 565, 566,
            570, 571, 572, 573, 574, 575, 576, 577,
            580, 581, 582, 583, 584
        };
    }
}
