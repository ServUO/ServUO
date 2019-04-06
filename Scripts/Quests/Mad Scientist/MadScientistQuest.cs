using System;
using System.Collections.Generic;
using Server.Items;
using Server.Network;
using Server.Commands;
using Server.Mobiles;
using System.Linq;

namespace Server.Engines.Quests
{
    public class MadScientistQuest
    {
        #region Generation
        public static void Initialize()
        {
            CommandSystem.Register("GenSutek", AccessLevel.Developer, new CommandEventHandler(GenQuest_Command));
        }

        private static void GenQuest_Command(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Creating Mad Scientist Quest...");

            foreach (SutekIngredientInfo def in m_Ingredients)
            {
                WeakEntityCollection.Add("sa", new SutekIngredientItem(def));
            }

            XmlSpawner sp = new XmlSpawner("Sutek");
            sp.SpawnRange = 5;
            sp.HomeRange = 5;
            sp.MoveToWorld(new Point3D(917, 594, -14), Map.TerMur);
            sp.Respawn();
            WeakEntityCollection.Add("sa", sp);

            List<Item> toDelete = new List<Item>(World.Items.Values.Where(i => i is XmlSpawner && (i.Name == "PerfectTimingSpawner" || i.Name == "PerfectTimingSpawner2")));

            foreach (var item in toDelete)
            {
                item.Delete();
            }

            e.Mobile.SendMessage("Generation completed, deleted {0} spawners!", toDelete.Count);
            ColUtility.Free(toDelete);
        }
        #endregion

        #region Definitions
        public static readonly int NeededIngredients = 20;
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15.0);

        public static readonly SutekIngredientInfo[] m_Ingredients = new SutekIngredientInfo[]
        {
            new SutekIngredientInfo(SutekIngredient.Feathers,      new Point3D(921, 598, -8),    0x1BD3, 1023578),
            new SutekIngredientInfo(SutekIngredient.Shafts,        new Point3D(918, 591, -14),   0x1BD6, 1027125),
            new SutekIngredientInfo(SutekIngredient.PowerCrystal,  new Point3D(917, 589, -5),    0x1F1C, 1112811),
            new SutekIngredientInfo(SutekIngredient.PowerCrystal,  new Point3D(926, 597, -4),    0x1F19, 1112811),
            new SutekIngredientInfo(SutekIngredient.YellowPotion,  new Point3D(926, 593, -8),    0x183B, 1023852),
            new SutekIngredientInfo(SutekIngredient.BarrelHoops,   new Point3D(931, 601, -14),   0x1DB7, 1011228),
            new SutekIngredientInfo(SutekIngredient.BarrelHoops,   new Point3D(931, 601, -12),   0x1DB7, 1011228),
            new SutekIngredientInfo(SutekIngredient.BarrelStaves,  new Point3D(932, 600, -14),   0x1EB2, 1027857),
            new SutekIngredientInfo(SutekIngredient.BarrelStaves,  new Point3D(933, 600, -14),   0x1EB3, 1027857),
            new SutekIngredientInfo(SutekIngredient.Bones,         new Point3D(925, 605, -14),   0x21FC, 1023786),
            new SutekIngredientInfo(SutekIngredient.FetidEssence,  new Point3D(926, 599, -5),    0x2D92, 1031066),
            new SutekIngredientInfo(SutekIngredient.VoidEssence,   new Point3D(926, 589, -3),    0x2D92, 1112327, 0x835),
            new SutekIngredientInfo(SutekIngredient.SpiritEssence, new Point3D(916, 602, -4),    0x2D87, 1055029, 0x481),
            new SutekIngredientInfo(SutekIngredient.Rope,          new Point3D(911, 589, -13),   0x14F8, 1020934),
            new SutekIngredientInfo(SutekIngredient.Rope,          new Point3D(919, 581, -14),   0x14FA, 1020934),
            new SutekIngredientInfo(SutekIngredient.WoodenLogs,    new Point3D(910, 590, -13),   0x1BDF, 1021217),
            new SutekIngredientInfo(SutekIngredient.WoodenLogs,    new Point3D(916, 609, -14),   0x1BE1, 1021217),
            new SutekIngredientInfo(SutekIngredient.WoodenLogs,    new Point3D(939, 593, -14),   0x1BDE, 1021217),
            new SutekIngredientInfo(SutekIngredient.WoodenLogs,    new Point3D(911, 602, -14),   0x1BE2, 1021217),
            new SutekIngredientInfo(SutekIngredient.PurplePotion,  new Point3D(916, 601, -6),    0x1841, 1023853),
            new SutekIngredientInfo(SutekIngredient.Scales,        new Point3D(921, 588, -8),    0x26B5, 1029905),
            new SutekIngredientInfo(SutekIngredient.Scales,        new Point3D(923, 603, -6),    0x26B2, 1029905),
            new SutekIngredientInfo(SutekIngredient.WhiteStone,    new Point3D(927, 605, -14),   0x177A, 1112813),
            new SutekIngredientInfo(SutekIngredient.DarkStone,     new Point3D(931, 583, -14),   0x1776, 1112866),
            new SutekIngredientInfo(SutekIngredient.Beeswax,       new Point3D(926, 600, -6),    0x1425, 1025154),
            new SutekIngredientInfo(SutekIngredient.Beeswax,       new Point3D(925, 603, -8),    0x1426, 1025154),
            new SutekIngredientInfo(SutekIngredient.Thorns,        new Point3D(928, 595, -14),   0x3022, 1112818),
            new SutekIngredientInfo(SutekIngredient.Thorns,        new Point3D(928, 596, -14),   0x3022, 1112818),
            new SutekIngredientInfo(SutekIngredient.WoodenBoards,  new Point3D(939, 592, -14),   0x1BDB, 1021189),
            new SutekIngredientInfo(SutekIngredient.WoodenBoards,  new Point3D(928, 590, -14),   0x1BDC, 1021189),
            new SutekIngredientInfo(SutekIngredient.WoodenBoards,  new Point3D(911, 591, -14),   0x1BD8, 1021189),
            new SutekIngredientInfo(SutekIngredient.WoodenBoards,  new Point3D(916, 598, -14),   0x1BD9, 1021189),
            new SutekIngredientInfo(SutekIngredient.BrownStone,    new Point3D(922, 577, -14),   0x1772, 1112814),
            new SutekIngredientInfo(SutekIngredient.RedPotion,     new Point3D(925, 603, -8),    0x183E, 1023851),
            new SutekIngredientInfo(SutekIngredient.PurplePotion,  new Point3D(921, 597, -10),   0x1839, 1023853),
            new SutekIngredientInfo(SutekIngredient.BluePotion,    new Point3D(921, 596, -7),    0x1844, 1023848),
            new SutekIngredientInfo(SutekIngredient.Gears,         new Point3D(920, 597, -8),    0x1051, 1024177),
            new SutekIngredientInfo(SutekIngredient.MeltedWax,     new Point3D(916, 600, -6),    0x142B, 1025162),
            new SutekIngredientInfo(SutekIngredient.MeltedWax,     new Point3D(926, 595, -7),    0x142A, 1025162),
            new SutekIngredientInfo(SutekIngredient.Leather,       new Point3D(924, 588, -14),   0x1078, 1024216),
            new SutekIngredientInfo(SutekIngredient.CopperWire,    new Point3D(910, 589, -8),    0x1879, 1026265),
            new SutekIngredientInfo(SutekIngredient.GoldWire,      new Point3D(910, 589, -13),   0x1878, 1026264),
            new SutekIngredientInfo(SutekIngredient.SilverWire,    new Point3D(910, 589, -10),   0x1877, 1026263),
            new SutekIngredientInfo(SutekIngredient.IronWire,      new Point3D(910, 589, -6),    0x1876, 1026262),
            new SutekIngredientInfo(SutekIngredient.SilverIngots,  new Point3D(911, 588, -14),   0x1BFA, 1027158),
            new SutekIngredientInfo(SutekIngredient.SilverIngots,  new Point3D(928, 603, -14),   0x1BF7, 1027158),
            new SutekIngredientInfo(SutekIngredient.GoldIngots,    new Point3D(914, 581, -14),   0x1BEE, 1027146),
            new SutekIngredientInfo(SutekIngredient.GoldIngots,    new Point3D(922, 594, -14),   0x1BEB, 1027146),
            new SutekIngredientInfo(SutekIngredient.CopperIngots,  new Point3D(914, 580, -14),   0x1BE5, 1027140),
            new SutekIngredientInfo(SutekIngredient.CopperIngots,  new Point3D(916, 590, -11),   0x1BE8, 1027140),
            new SutekIngredientInfo(SutekIngredient.IronIngots,    new Point3D(915, 580, -14),   0x1BF1, 1027152),
            new SutekIngredientInfo(SutekIngredient.IronIngots,    new Point3D(912, 602, -15),   0x1BF4, 1027152),
            new SutekIngredientInfo(SutekIngredient.OilOfVitriol,  new Point3D(916, 601, -8),    0x098D, 1077482),
            new SutekIngredientInfo(SutekIngredient.BlackPowder,   new Point3D(920, 597, -6),    0x0B48, 1112815, 0x497),
            new SutekIngredientInfo(SutekIngredient.WhitePowder,   new Point3D(926, 602, -6),    0x241D, 1112816),
            new SutekIngredientInfo(SutekIngredient.BluePowder,    new Point3D(926, 594, -4),    0x241E, 1112817),
            new SutekIngredientInfo(SutekIngredient.Nails,         new Point3D(915, 589, -14),   0x102E, 1024142),
        };

        private static Dictionary<Mobile, QuestContext> m_Table = new Dictionary<Mobile, QuestContext>();
        #endregion

        public static bool QuestStarted(Mobile from)
        {
            return m_Table.ContainsKey(from);
        }

        public static void StartQuest(Mobile from, ClockworkMechanism mechanism)
        {
            if (QuestStarted(from))
                return;

            QuestContext context = m_Table[from] = new QuestContext(from, mechanism);
            context.StartTimer();
        }

        public static void OnDoubleClickIngredient(Mobile from, SutekIngredient ingredient)
        {
            if (!QuestStarted(from))
                return;

            QuestContext context = m_Table[from];

            if (ingredient == context.CurrentIngredient)
            {
                from.SendLocalizedMessage(1112819); // You've successfully added this ingredient.

                context.OnIngredientAcquired();
            }
            else
            {
                from.SendLocalizedMessage(1112820); // That is not the right ingredient.

                BarkIngredient(from);
            }
        }

        public static void BarkIngredient(Mobile from)
        {
            if (!QuestStarted(from))
                return;

            QuestContext context = m_Table[from];

            from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1112821, String.Format("#{0}", (int)context.CurrentIngredient)); // I need to add some ~1_INGREDIENT~.
        }

        public class QuestContext
        {
            private Mobile m_Owner;
            private int m_IngredientsLeft = NeededIngredients;
            private SutekIngredient m_CurrentIngredient;
            private Timer m_ExpireTimer;
            private ClockworkMechanism m_Mechanism;

            public Mobile Owner { get { return m_Owner; } }
            public int IngredientsLeft { get { return m_IngredientsLeft; } }
            public SutekIngredient CurrentIngredient { get { return m_CurrentIngredient; } }
            public Timer ExpireTimer { get { return m_ExpireTimer; } }
            public ClockworkMechanism Mechanism { get { return m_Mechanism; } }

            public QuestContext(Mobile from, ClockworkMechanism mechanism)
            {
                m_Owner = from;
                m_Mechanism = mechanism;
            }

            public void StartTimer()
            {
                if (m_ExpireTimer != null)
                    m_ExpireTimer.Stop();

                m_ExpireTimer = Timer.DelayCall(Timeout, new TimerCallback(OnExpired));

                SutekIngredient[] ingredients = (SutekIngredient[])Enum.GetValues(typeof(SutekIngredient));
                m_CurrentIngredient = ingredients[Utility.Random(ingredients.Length)];

                m_Owner.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1112821, String.Format("#{0}", (int)m_CurrentIngredient)); // I need to add some ~1_INGREDIENT~.

                m_IngredientsLeft--;
            }

            public void OnExpired()
            {
                m_ExpireTimer.Stop();

                m_Owner.SendLocalizedMessage(1112822); // You fail to find the next ingredient in time. Your clockwork assembly crumbles.

                m_Mechanism.Delete();
                m_Table.Remove(m_Owner);
            }

            public void OnIngredientAcquired()
            {
                if (m_IngredientsLeft == 0)
                {
                    m_ExpireTimer.Stop();
                    m_Mechanism.OnCompleted(m_Owner);
                    m_Mechanism.Delete();

                    m_Owner.AddToBackpack(new CompletedClockworkAssembly());

                    m_Table.Remove(m_Owner);
                }
                else
                {
                    StartTimer();
                }
            }
        }        
    }
}