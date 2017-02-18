using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ultima;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ultima.Tests
{
    [TestClass()]
    public class UltimaOnlineReaderFactoryTests
    {
        [TestMethod()]
        public void UltimaOnlineReaderFactoryTest()
        {
            var factory = new UltimaOnlineReaderFactory(@"C:\Ultima\OSI_seas_mul - Copia");
            Assert.IsNotNull(factory);
            factory.Dispose();
        }
    }
}