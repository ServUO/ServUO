using System;
using Server;
using Server.Items;

namespace Server.Kiasta.Dragoon
{
    public class BagOfBlackDragoonEquipment : BaseGoodieBag
    {
        [Constructable]
        public BagOfBlackDragoonEquipment()
            : this(1)
        {
        }

        [Constructable]
        public BagOfBlackDragoonEquipment(int amount)
            : base(amount)
        {
            Weight = 0.0;
            Name = "a bag of blue dragoon equipment";
            LootType = Settings.Misc.BagLootType;
            for (int i = 0; i < amount; i++)
            {
                DropItem(new BlackDragoonBlade());
                DropItem(new BlackDragoonBracelet());
                DropItem(new BlackDragoonCloak());
                DropItem(new BlackDragoonCuirass());
                DropItem(new BlackDragoonDress());
                DropItem(new BlackDragoonEarrings());
                DropItem(new BlackDragoonFemaleElvenRobe());
                DropItem(new BlackDragoonGauntlets());
                DropItem(new BlackDragoonGorget());
                DropItem(new BlackDragoonGreaves());
                DropItem(new BlackDragoonHelm());
                DropItem(new BlackDragoonMaleElvenRobe());
                DropItem(new BlackDragoonNecklace());
                DropItem(new BlackDragoonPaulders());
                DropItem(new BlackDragoonRing());
                DropItem(new BlackDragoonShield());
                DropItem(new BlackDragoonShroud());
                DropItem(new BlackDragoonThighBoots());
            }
        }

        public BagOfBlackDragoonEquipment(Serial serial)
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
