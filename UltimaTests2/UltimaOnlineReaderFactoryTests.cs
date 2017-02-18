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
            UltimaOnlineReaderFactory factory = new UltimaOnlineReaderFactory("");
        }

        [TestMethod()]
        public void DisposeTest()
        {

        }
    }
}