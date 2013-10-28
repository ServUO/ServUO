using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a black order grand mage corpse")] 
    public class DragonsFlameGrandMage : DragonsFlameMage
    {
        [Constructable]
        public DragonsFlameGrandMage()
            : base()
        {
            this.Name = "Black Order Grand Mage";
            this.Title = "of the Dragon's Flame Sect";
        }

        public DragonsFlameGrandMage(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool ShowFameTitle
        {
            get
            {
                return false;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosFilthyRich, 6);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);	
			
            if (Utility.RandomDouble() < 0.3)
                c.DropItem(new DragonFlameKey());
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