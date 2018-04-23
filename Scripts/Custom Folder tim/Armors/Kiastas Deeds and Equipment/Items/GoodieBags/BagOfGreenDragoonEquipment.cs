using System;
using Server;
using Server.Items;

namespace Server.Kiasta.Dragoon
{
    public class BagOfGreenDragoonEquipment : BaseGoodieBag
    {
        [Constructable]
        public BagOfGreenDragoonEquipment() : this(1)
        {
        }

        [Constructable]
        public BagOfGreenDragoonEquipment(int amount) : base(amount)
        {
            Weight = 0.0;
            Name = "a bag of green dragoon equipment";
            LootType = Settings.Misc.BagLootType;
            for (int i = 0; i < amount; i++)
            {
                DropItem(new GreenDragoonBlade());
                DropItem(new GreenDragoonBracelet());
                DropItem(new GreenDragoonCloak());
                DropItem(new GreenDragoonCuirass());
                DropItem(new GreenDragoonDress());
                DropItem(new GreenDragoonEarrings());
                DropItem(new GreenDragoonFemaleElvenRobe());
                DropItem(new GreenDragoonGauntlets());
                DropItem(new GreenDragoonGorget());
                DropItem(new GreenDragoonGreaves());
                DropItem(new GreenDragoonHelm());
                DropItem(new GreenDragoonMaleElvenRobe());
                DropItem(new GreenDragoonNecklace());
                DropItem(new GreenDragoonPaulders());
                DropItem(new GreenDragoonRing());
                DropItem(new GreenDragoonShield());
                DropItem(new GreenDragoonShroud());
                DropItem(new GreenDragoonThighBoots());
            }
        }

        public BagOfGreenDragoonEquipment(Serial serial)
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
