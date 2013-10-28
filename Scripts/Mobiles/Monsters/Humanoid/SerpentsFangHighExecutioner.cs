using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a black order high executioner corpse")] 
    public class SerpentsFangHighExecutioner : SerpentsFangAssassin
    {
        [Constructable]
        public SerpentsFangHighExecutioner()
            : base()
        {
            this.Name = "Black Order High Executioner";
            this.Title = "of the Serpent's Fang Sect";
        }

        public SerpentsFangHighExecutioner(Serial serial)
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

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            if (from != null)
                from.Damage(damage / 2, from);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);	
			
            if (Utility.RandomDouble() < 0.2)
                c.DropItem(new SerpentFangKey());
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