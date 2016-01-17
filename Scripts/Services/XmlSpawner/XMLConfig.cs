namespace Server.XmlConfiguration
{
    class XmlConfig
    {
        /// <summary>
        /// The extra systems from XMLSpawner can be enabled or disabled here.
        /// </summary>

        // This will enable the XMLPoints System for tracking kills, deaths, etc.
        public static bool XmlPointsEnabled = false;

        // This will enable the XMLMobFactions System which allows players to show favoritism to certain classes of creatures.
        public static bool XmlMobFactionsEnabled = false;

        // This will enable the XMLSockets system which allows armor and weapons to be enhanced using a Diablo style socket system
        public static bool XmlSocketsEnabled = false;
        // This will determine if Artifacts will be included in the Socket system. True means Artifacts will have sockets. Sockets must be enabled for this to work.
        public static bool XmlSocketedArtifacts = false;
    }
}
