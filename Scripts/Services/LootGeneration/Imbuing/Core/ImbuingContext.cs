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

        public ImbuingContext(Mobile mob)
        {
            Player = mob;
        }

        public ImbuingContext(Mobile owner, GenericReader reader)
        {
            int v = reader.ReadInt();

            Player = owner;

            switch (v)
            {
                case 1:
                    LastImbued = reader.ReadItem();
                    Imbue_Mod = reader.ReadInt();
                    Imbue_ModInt = reader.ReadInt();
                    Imbue_ModVal = reader.ReadInt();
                    ImbMenu_Cat = reader.ReadInt();
                    break;
                case 0:
                    LastImbued = reader.ReadItem();
                    Imbue_Mod = reader.ReadInt();
                    Imbue_ModInt = reader.ReadInt();
                    Imbue_ModVal = reader.ReadInt();
                    reader.ReadInt();
                    ImbMenu_Cat = reader.ReadInt();
                    reader.ReadInt();
                    break;
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(1);

            writer.Write(LastImbued);
            writer.Write(Imbue_Mod);
            writer.Write(Imbue_ModInt);
            writer.Write(Imbue_ModVal);
            writer.Write(ImbMenu_Cat);
        }
    }
}
