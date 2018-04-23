using System;
using Server;
using Server.Items;

namespace Server.Kiasta.Dragoon
{
    public class BagOfWhiteDragoonEquipment : BaseGoodieBag
    {
        [Constructable]
        public BagOfWhiteDragoonEquipment() : this(1)
        {
        }

        [Constructable]
        public BagOfWhiteDragoonEquipment(int amount) : base(amount)
        {
            Weight = 0.0;
            Name = "a bag of white dragoon equipment";
            LootType = Settings.Misc.BagLootType;
            for (int i = 0; i < amount; i++)
            {
                DropItem(new WhiteDragoonBlade());
                DropItem(new WhiteDragoonBracelet());
                DropItem(new WhiteDragoonCloak());
                DropItem(new WhiteDragoonCuirass());
                DropItem(new WhiteDragoonDress());
                DropItem(new WhiteDragoonEarrings());
                DropItem(new WhiteDragoonFemaleElvenRobe());
                DropItem(new WhiteDragoonGauntlets());
                DropItem(new WhiteDragoonGorget());
                DropItem(new WhiteDragoonGreaves());
                DropItem(new WhiteDragoonHelm());
                DropItem(new WhiteDragoonMaleElvenRobe());
                DropItem(new WhiteDragoonNecklace());
                DropItem(new WhiteDragoonPaulders());
                DropItem(new WhiteDragoonRing());
                DropItem(new WhiteDragoonShield());
                DropItem(new WhiteDragoonShroud());
                DropItem(new WhiteDragoonThighBoots());
            }
        }

        public BagOfWhiteDragoonEquipment(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
