using System;
using Server.Engines.ClassSystem;

namespace Server.Engines.ClassSystem
{
    /// <summary>
    /// Single entry point for the entire class system.
    ///
    /// ServUO automatically calls any public static Initialize() method at startup.
    /// To avoid conflicts with ClassRegistry.Configure() (which must NOT be named
    /// Initialize() or ServUO would call it twice), all bootstrapping is done here
    /// in the correct order:
    ///   1. Register all classes.
    ///   2. Call ClassRegistry.Configure() to register their commands.
    /// </summary>
    public static class ClassSystemBootstrap
    {
        public static void Initialize()
        {
            // 1. Register all playable classes.
            AvengerClass.Register();
            // NinjaClass.Register();
            // ArchmageClass.Register();

            // 2. Register commands for all classes.
            ClassRegistry.Configure();
        }
    }
}
