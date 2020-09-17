using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.TombOfKings
{
    public class Generator
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenToK", AccessLevel.Developer, GenToK_Command);
        }

        [Usage("GenToK")]
        private static void GenToK_Command(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Creating Tomb of Kings...");

            // Bridge
            Static st = new Static(16880);
            WeakEntityCollection.Add("sa", st);
            st.MoveToWorld(new Point3D(36, 36, 0), Map.TerMur);

            st = new Static(16882);
            WeakEntityCollection.Add("sa", st);
            st.MoveToWorld(new Point3D(37, 36, 0), Map.TerMur);

            st = new Static(16883);
            st.MoveToWorld(new Point3D(38, 36, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(16878);
            st.MoveToWorld(new Point3D(36, 35, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(16884);
            st.MoveToWorld(new Point3D(37, 35, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(16884);
            st.MoveToWorld(new Point3D(38, 35, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(16878);
            st.MoveToWorld(new Point3D(36, 34, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(16884);
            st.MoveToWorld(new Point3D(37, 34, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(16884);
            st.MoveToWorld(new Point3D(38, 34, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(16878);
            st.MoveToWorld(new Point3D(36, 33, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(16884);
            st.MoveToWorld(new Point3D(37, 33, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(16884);
            st.MoveToWorld(new Point3D(38, 33, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(16878);
            st.MoveToWorld(new Point3D(36, 32, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(16884);
            st.MoveToWorld(new Point3D(37, 32, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(16884);
            st.MoveToWorld(new Point3D(38, 32, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(16872);
            st.MoveToWorld(new Point3D(36, 31, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(16873);
            st.MoveToWorld(new Point3D(37, 31, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(16874);
            st.MoveToWorld(new Point3D(38, 31, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            //Sacred Quest Blocker
            SacredQuestBlocker sq = new SacredQuestBlocker();
            sq.MoveToWorld(new Point3D(35, 38, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", sq);

            sq = new SacredQuestBlocker();
            sq.MoveToWorld(new Point3D(36, 38, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", sq);

            sq = new SacredQuestBlocker();
            sq.MoveToWorld(new Point3D(37, 38, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", sq);

            sq = new SacredQuestBlocker();
            sq.MoveToWorld(new Point3D(38, 38, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", sq);

            sq = new SacredQuestBlocker();
            sq.MoveToWorld(new Point3D(39, 38, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", sq);

            // Guardian
            XmlSpawner spawner = new XmlSpawner(1, 300, 600, 0, 0, 0, "GargoyleDestroyer, /blessed/true/Frozen/true/Direction/West/Paralyzed/true/Hue/2401/Name/Guardian")
            {
                SmartSpawning = true
            };
            spawner.MoveToWorld(new Point3D(42, 38, 13), Map.TerMur);
            WeakEntityCollection.Add("sa", spawner);

            spawner = new XmlSpawner(1, 300, 600, 0, 0, 0, "GargoyleDestroyer, /blessed/true/Frozen/true/Direction/East/Paralyzed/true/Hue/2401/Name/Guardian")
            {
                SmartSpawning = true
            };
            spawner.MoveToWorld(new Point3D(33, 38, 13), Map.TerMur);
            WeakEntityCollection.Add("sa", spawner);

            // Teleporter
            ToKTeleporter t = new ToKTeleporter();
            t.MoveToWorld(new Point3D(21, 99, 1), Map.TerMur);
            WeakEntityCollection.Add("sa", t);

            st = new Static(14186); // sparkle
            st.MoveToWorld(new Point3D(21, 99, 1), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(18304); // door
            st.MoveToWorld(new Point3D(18, 99, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            TombOfKingsSecretDoor door = new TombOfKingsSecretDoor(18304);
            door.MoveToWorld(new Point3D(52, 99, 0), Map.TerMur);
            WeakEntityCollection.Add("sa", door);

            // Serpent's Breath
            Item flame = new FlameOfOrder(new Point3D(28, 212, 3), Map.TerMur);
            WeakEntityCollection.Add("sa", flame);

            flame = new FlameOfChaos(new Point3D(43, 212, 3), Map.TerMur);
            WeakEntityCollection.Add("sa", flame);

            st = new Static(3025)
            {
                Name = "Order Shall Steal The Serpent's Strength"
            };
            st.MoveToWorld(new Point3D(28, 208, 4), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            st = new Static(3025)
            {
                Name = "Chaos Shall Quell The Serpent's Wrath"
            };
            st.MoveToWorld(new Point3D(28, 208, 4), Map.TerMur);
            WeakEntityCollection.Add("sa", st);

            // Kings' Chambers
            ChamberLever.Generate();
            Chamber.Generate();
            ChamberSpawner.Generate();

            CommandSystem.Handle(e.Mobile, CommandSystem.Prefix + "ArisenGenerate");

            e.Mobile.SendMessage("Generation completed!");
        }
    }
}
