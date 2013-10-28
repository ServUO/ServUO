using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a black order master corpse")] 
    public class TigersClawMaster : TigersClawThief
    {
        [Constructable]
        public TigersClawMaster()
            : base()
        {
            this.Name = "Black Order Master";
            this.Title = "of the Serpent's Fang Sect";
        }

        public TigersClawMaster(Serial serial)
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
			
            if (Utility.RandomDouble() < 0.2)
                c.DropItem(new TigerClawKey());
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