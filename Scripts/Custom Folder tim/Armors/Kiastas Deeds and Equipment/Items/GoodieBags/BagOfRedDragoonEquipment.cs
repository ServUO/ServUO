using System;
using Server;
using Server.Items;

namespace Server.Kiasta.Dragoon
{
    public class BagOfRedDragoonEquipment : BaseGoodieBag
    {
        [Constructable]
        public BagOfRedDragoonEquipment() : this(1)
        {
        }

        [Constructable]
        public BagOfRedDragoonEquipment(int amount) : base(amount)
        {
            Weight = 0.0;
            Name = "a bag of red dragoon equipment";
            LootType = Settings.Misc.BagLootType;
            for (int i = 0; i < amount; i++)
            {
                DropItem(new RedDragoonBlade());
                DropItem(new RedDragoonBracelet());
                DropItem(new RedDragoonCloak());
                DropItem(new RedDragoonCuirass());
                DropItem(new RedDragoonDress());
                DropItem(new RedDragoonEarrings());
                DropItem(new RedDragoonFemaleElvenRobe());
                DropItem(new RedDragoonGauntlets());
                DropItem(new RedDragoonGorget());
                DropItem(new RedDragoonGreaves());
                DropItem(new RedDragoonHelm());
                DropItem(new RedDragoonMaleElvenRobe());
                DropItem(new RedDragoonNecklace());
                DropItem(new RedDragoonPaulders());
                DropItem(new RedDragoonRing());
                DropItem(new RedDragoonShield());
                DropItem(new RedDragoonShroud());
                DropItem(new RedDragoonThighBoots());
            }
        }

        public BagOfRedDragoonEquipment(Serial serial)
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
