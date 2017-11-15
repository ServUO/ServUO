using Server;
using System;
using Server.Mobiles;

namespace Server.Engines.ArenaSystem
{
    public class ArenaManager : AnimalTrainer
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public PVPArena Arena { get; set; }

        [Constructable]
        public ArenaManager(PVPArena arena)
            : base("The arena manager")
        {
            Arena = arena;
            CantWalk = true;
        }

        public override void InitBody()
        {
            Female = true;
            Name = NameList.RandomName("female");

            HairItemID = Race.RandomHair(true);
            HairHue = Race.RandomHairHue();
            Hue = Race.RandomSkinHue();

            SetStr(100);
            SetInt(100);
            SetDex(100);
        }

        public override void InitOutfit()
        {
            SetWearable(new PlateHaidate(), 1173);
            SetWearable(new FemalePlateChest(), 1173);
            SetWearable(new PlateGloves(), 1173);
            SetWearable(new Bonnet(), 1173);
            SetWearable(new Sandals(), 1173);
            SetWearable(new Spellbook(), 1168);
        }

        public ArenaManager(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}