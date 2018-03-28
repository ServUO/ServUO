using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines
{
    public static class GenerateForgottenPyramid
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenForgottenPyramid", AccessLevel.Administrator, Generate_ForgottenPyramid);
        }

        public static void Generate_ForgottenPyramid(CommandEventArgs e)
        {
            XmlSpawner sp = new XmlSpawner("Sphynx");
            sp.SpawnRange = 10;
            sp.HomeRange = 15;
            sp.MoveToWorld(new Point3D(1821, 1797, -110), Map.Malas);
            sp.Respawn();

            Item item = new AncientWall();
            item.MoveToWorld(new Point3D(1824, 1783, -110), Map.Malas);

            item = new SphynxFortune();
            item.MoveToWorld(new Point3D(1831, 1805, -110), Map.Malas);

            e.Mobile.SendMessage("Forgotten Pyramid generation complete.");
        }
    }
}
