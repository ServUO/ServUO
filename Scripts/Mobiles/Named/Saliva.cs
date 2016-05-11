using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a saliva corpse")]
    public class Saliva : Harpy
    {
        [Constructable]
        public Saliva()
            : base()
        {
            this.Name = "a saliva";
            this.Hue = 0x11E;

            this.SetStr(136, 206);
            this.SetDex(123, 222);
            this.SetInt(118, 127);

            this.SetHits(409, 842);

            this.SetDamage(19, 28);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 46, 47);
            this.SetResistance(ResistanceType.Fire, 32, 40);
            this.SetResistance(ResistanceType.Cold, 34, 49);
            this.SetResistance(ResistanceType.Poison, 40, 48);
            this.SetResistance(ResistanceType.Energy, 35, 39);

            this.SetSkill(SkillName.Wrestling, 106.4, 128.8);
            this.SetSkill(SkillName.Tactics, 129.9, 141.0);
            this.SetSkill(SkillName.MagicResist, 84.3, 90.1);

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Saliva(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosUltraRich, 2);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);		
			
            c.DropItem(new SalivasFeather());
			
            if (Utility.RandomDouble() < 0.1)				
                c.DropItem(new ParrotItem());
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