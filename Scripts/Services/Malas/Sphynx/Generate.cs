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
            Generate(e.Mobile);
        }

        public static void Generate(Mobile m)
        {
            if (Map.Malas.FindItem<XmlSpawner>(new Point3D(1821, 1797, -110)) == null)
            {
                XmlSpawner sp = new XmlSpawner("Sphynx");
                sp.SpawnRange = 10;
                sp.HomeRange = 15;
                sp.MoveToWorld(new Point3D(1821, 1797, -110), Map.Malas);
                sp.Respawn();
            }

            if (Map.Malas.FindItem<AncientWall>(new Point3D(1824, 1783, -110)) == null)
            {
                Item item = new AncientWall();
                item.MoveToWorld(new Point3D(1824, 1783, -110), Map.Malas);
            }

            if (m != null)
            {
                m.SendMessage("Forgotten Pyramid generation complete.");
            }
        }
    }
}
