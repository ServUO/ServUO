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
            this.SetStr(340, 360);
            this.SetDex(200, 215);
            this.SetInt(500, 515);

            this.SetHits(800);

            this.SetDamage(15, 20);

            this.Fame = 25000;
            this.Karma = -25000;

            this.VirtualArmor = 60;
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
			
                c.DropItem(new DragonFlameKey());

            if (Utility.RandomDouble() < 0.5)
                c.DropItem(new DragonFlameSectBadge());
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