using NUnit.Framework;
using Ultima;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ultima.Tests
{
    [TestFixture()]
    public class UltimaOnlineReaderFactoryTests
    {
        [Test()]
        public void UltimaOnlineReaderFactoryTest()
        {
            var factory = new UltimaOnlineReaderFactory("");
            Assert.NotNull(factory);
        }

        [Test()]
        public void InitTest()
        {
            var factory = new UltimaOnlineReaderFactory("");
            factory.Init();
            Assert.NotNull(factory);
        }

        [Test()]
        public void DisposeTest()
        {
            var factory = new UltimaOnlineReaderFactory("");
            factory.Init();
            factory.Dispose();
            Assert.NotNull(factory);
        }

        [Test()]
        public void ArtTest()
        {
            var factory = new UltimaOnlineReaderFactory("");
            factory.Init();
            var legalItemId = factory.Art.GetLegalItemID(100);
            Assert.IsTrue(legalItemId == 100);
            factory.Dispose();
        }

        [Test()]
        public void ArtTest2()
        {
            var factory = new UltimaOnlineReaderFactory(@"C:\Ultima\OSI_seas_mul - Copia");
            factory.Init();
            var raw = factory.Art.GetRaw(100);
            Assert.IsTrue(raw != null);
            factory.Dispose();
        }

        [Test()]
        public void TileData()
        {
            var factory = new UltimaOnlineReaderFactory(@"C:\Ultima\OSI_seas_mul - Copia");
            factory.Init();
            var raw = factory.TileData.ItemTable[100];
            Assert.IsTrue(raw.Height > 0);
            factory.Dispose();
        }

        [Test()]
        public void LandData()
        {
            var factory = new UltimaOnlineReaderFactory(@"C:\Ultima\OSI_seas_mul - Copia");
            factory.Init();
            var raw = factory.TileData.LandTable[100];
            Assert.IsTrue(raw.TextureID > 0);
            factory.Dispose();
        }

        [Test()]
        public void Gump()
        {
            var factory = new UltimaOnlineReaderFactory(@"C:\Ultima\OSI_seas_mul - Copia");
            factory.Init();
            var raw = factory.Gumps.GetCount();
            Assert.IsTrue(raw > 0);
            factory.Dispose();
        }
        [Test()]
        public void Maps()
        {
            var factory = new UltimaOnlineReaderFactory(@"C:\Ultima\OSI_seas_mul - Copia");
            factory.Init();
            List<Tile[]> tileList = factory.Maps.Select(mapEntry => mapEntry.Value.Tiles.GetLandBlock(10, 10)).ToList();

            foreach (var element in tileList)
            {
                Assert.IsTrue(element != null);
            }


         
            factory.Dispose();
        }
    }
}