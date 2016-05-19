using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Items;

namespace Server.Commands
{
    class GenerateValidTiles
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenerateValidTiles", AccessLevel.Administrator, new CommandEventHandler(GenerateValidTiles_OnCommand));
        }

        [Usage("GenerateValidTiles")]
        [Description("Generates data file containing valid tiles where honor virtue items and treasure chests may spawn in felucca and trammel.")]
        private static void GenerateValidTiles_OnCommand(CommandEventArgs e)
        {
            Rectangle2D rect = new Rectangle2D(0, 0, 5119, 4095);

            List<Point2D> fel = new List<Point2D>();
            List<Point2D> tram = new List<Point2D>();

            int felCount = 0;
            int tramCount = 0;
            Console.WriteLine("Generating valid tiles");
            for (int i = 0; i < 5119; i++)
            {
                for (int j = 0; j < 4095; j++)
                {
                    if (TreasureMap.ValidateLocation(i, j, Map.Felucca))
                    {
                        fel.Add(new Point2D(i, j));
                        felCount++;
                    }

                    if (TreasureMap.ValidateLocation(i, j, Map.Trammel))
                    {
                        tram.Add(new Point2D(i, j));
                        tramCount++;
                    }
                }
            }


            try
            {
                using (BinaryWriter bw = new BinaryWriter(File.Open(Path.Combine("Data", "Felucca.MapPoints"), FileMode.Create)))
                {
                    bw.Write(felCount);
                    for (int i = 0; i < felCount; i++)
                    {
                        bw.Write(fel[i].X);
                        bw.Write(fel[i].Y);
                    }
                }

                Compress(Path.Combine("Data", "Felucca.MapPoints"));

                using (BinaryWriter bw = new BinaryWriter(File.Open(Path.Combine("Data", "Trammel.MapPoints"), FileMode.Create)))
                {
                    bw.Write(tramCount);
                    for (int i = 0; i < tramCount; i++)
                    {
                        bw.Write(tram[i].X);
                        bw.Write(tram[i].Y);
                    }
                }

                Compress(Path.Combine("Data", "Trammel.MapPoints"));
            }
            catch
            {
            }

            Console.WriteLine("Finished generating valid tiles");
        }

        private static void Compress(string path)
        {
            var b = File.ReadAllBytes(path);
            using (FileStream f = new FileStream(path, FileMode.Create))
            using (GZipStream gz = new GZipStream(f, CompressionMode.Compress, false))
            {
                gz.Write(b, 0, b.Length);
                gz.Close();
            }
        }

    }
}
