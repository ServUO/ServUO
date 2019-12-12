using System;
using Server;

namespace Server
{
    public class ImbuingContext
    {
        public Mobile Player { get; private set; }       
        public Item LastImbued { get; set; }
        public int Imbue_Mod { get; set; }
        public int Imbue_ModInt { get; set; }
        public int Imbue_ModVal { get; set; }
        public int ImbMenu_Cat { get; set; }
        public int ImbMenu_ModInc { get; set; }
        public int Imbue_IWmax { get; set; }

        public ImbuingContext(Mobile mob)
        {
            Player = mob;
        }

        public ImbuingContext(Mobile owner, GenericReader reader)
        {
            int v = reader.ReadInt();

            Player = owner;
            LastImbued = reader.ReadItem();
            Imbue_Mod = reader.ReadInt();
            Imbue_ModInt = reader.ReadInt();
            Imbue_ModVal = reader.ReadInt();
            Imbue_IWmax = reader.ReadInt();
            ImbMenu_Cat = reader.ReadInt();
            ImbMenu_ModInc = reader.ReadInt();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);

            writer.Write(LastImbued);
            writer.Write(Imbue_Mod);
            writer.Write(Imbue_ModInt);
            writer.Write(Imbue_ModVal);
            writer.Write(Imbue_IWmax);
            writer.Write(ImbMenu_Cat);
            writer.Write(ImbMenu_ModInc);
        }
    }
}
