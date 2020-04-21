#define CLIENT6017

using Server.Items;
using Server.Network;
using System;

namespace Server.Engines.XmlSpawner2
{
    public class PacketHandlerOverrides
    {
        public static void Initialize()
        {
            //
            // this will replace the default packet handlers with XmlSpawner2 versions.
            // The delay call is to make sure they are assigned after the core default assignments.
            //
            // If you dont want these packet handler overrides to be applied, just comment them out here.
            //

            // This will replace the default packet handler for basebooks content change.  This allows the
            // use of the text entry book interface for editing spawner entries.
            // Regular BaseBooks will still call their default handlers for ContentChange and HeaderChange
            Timer.DelayCall(TimeSpan.Zero, ContentChangeOverride);
        }

        public static void ContentChangeOverride()
        {
            PacketHandlers.Register(0x66, 0, true, XmlTextEntryBook.ContentChange);
#if(CLIENT6017)
            PacketHandlers.Register6017(0x66, 0, true, XmlTextEntryBook.ContentChange);
#endif
        }
    }
}
