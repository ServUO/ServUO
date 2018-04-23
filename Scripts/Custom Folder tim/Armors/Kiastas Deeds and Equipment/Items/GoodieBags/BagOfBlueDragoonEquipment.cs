using System;
using Server;
using Server.Items;

namespace Server.Kiasta.Dragoon
{
    public class BagOfBlueDragoonEquipment : BaseGoodieBag
    {
        [Constructable]
        public BagOfBlueDragoonEquipment() : this(1)
        {
        }

        [Constructable]
        public BagOfBlueDragoonEquipment(int amount) : base(amount)
        {
            Weight = 0.0;
            Name = "a bag of blue dragoon equipment";
            LootType = Settings.Misc.BagLootType;
            for (int i = 0; i < amount; i++)
            {
                DropItem(new BlueDragoonBlade());
                DropItem(new BlueDragoonBracelet());
                DropItem(new BlueDragoonCloak());
                DropItem(new BlueDragoonCuirass());
                DropItem(new BlueDragoonDress());
                DropItem(new BlueDragoonEarrings());
                DropItem(new BlueDragoonFemaleElvenRobe());
                DropItem(new BlueDragoonGauntlets());
                DropItem(new BlueDragoonGorget());
                DropItem(new BlueDragoonGreaves());
                DropItem(new BlueDragoonHelm());
                DropItem(new BlueDragoonMaleElvenRobe());
                DropItem(new BlueDragoonNecklace());
                DropItem(new BlueDragoonPaulders());
                DropItem(new BlueDragoonRing());
                DropItem(new BlueDragoonShield());
                DropItem(new BlueDragoonShroud());
                DropItem(new BlueDragoonThighBoots());
            }
        }

        public BagOfBlueDragoonEquipment(Serial serial)
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
