using System;
using Server;
using Server.Items;

namespace Server.Kiasta.Dragoon
{
    public class BagOfRandomColorDragoonEquipment : BaseGoodieBag
    {
        [Constructable]
        public BagOfRandomColorDragoonEquipment() : this(1)
        {
        }

        [Constructable]
        public BagOfRandomColorDragoonEquipment(int amount) : base (amount)
        {
            Weight = 0.0;
            Name = "a bag of random color dragoon equipment";
            LootType = Settings.Misc.BagLootType;
            for (int i = 0; i < amount; i++)
            {
                DropItem(new RandomColorDragoonBlade());
                DropItem(new RandomColorDragoonBracelet());
                DropItem(new RandomColorDragoonCloak());
                DropItem(new RandomColorDragoonCuirass());
                DropItem(new RandomColorDragoonDress());
                DropItem(new RandomColorDragoonEarrings());
                DropItem(new RandomColorDragoonFemaleElvenRobe());
                DropItem(new RandomColorDragoonGauntlets());
                DropItem(new RandomColorDragoonGorget());
                DropItem(new RandomColorDragoonGreaves());
                DropItem(new RandomColorDragoonHelm());
                DropItem(new RandomColorDragoonMaleElvenRobe());
                DropItem(new RandomColorDragoonNecklace());
                DropItem(new RandomColorDragoonPaulders());
                DropItem(new RandomColorDragoonRing());
                DropItem(new RandomColorDragoonShield());
                DropItem(new RandomColorDragoonShroud());
                DropItem(new RandomColorDragoonThighBoots());
            }
        }

        public BagOfRandomColorDragoonEquipment(Serial serial) : base(serial)
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
