using System;
using Server;
using Server.Items;

namespace Server.Kiasta.Dragoon
{
    public class BagOfYellowDragoonEquipment : BaseGoodieBag
    {
        [Constructable]
        public BagOfYellowDragoonEquipment() : this(1)
        {
        }

        [Constructable]
        public BagOfYellowDragoonEquipment(int amount) : base(amount)
        {
            Weight = 0.0;
            Name = "a bag of yellow dragoon equipment";
            LootType = Settings.Misc.BagLootType;
            for (int i = 0; i < amount; i++)
            {
                DropItem(new YellowDragoonBlade());
                DropItem(new YellowDragoonBracelet());
                DropItem(new YellowDragoonCloak());
                DropItem(new YellowDragoonCuirass());
                DropItem(new YellowDragoonDress());
                DropItem(new YellowDragoonEarrings());
                DropItem(new YellowDragoonFemaleElvenRobe());
                DropItem(new YellowDragoonGauntlets());
                DropItem(new YellowDragoonGorget());
                DropItem(new YellowDragoonGreaves());
                DropItem(new YellowDragoonHelm());
                DropItem(new YellowDragoonMaleElvenRobe());
                DropItem(new YellowDragoonNecklace());
                DropItem(new YellowDragoonPaulders());
                DropItem(new YellowDragoonRing());
                DropItem(new YellowDragoonShield());
                DropItem(new YellowDragoonShroud());
                DropItem(new YellowDragoonThighBoots());
            }
        }

        public BagOfYellowDragoonEquipment(Serial serial)
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
