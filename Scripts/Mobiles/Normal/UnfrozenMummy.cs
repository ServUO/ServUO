using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an unfrozen mummy corpse")]
    public class UnfrozenMummy : BaseCreature
    {
        [Constructable]
        public UnfrozenMummy()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.4, 0.8)
        {
            this.Name = "an unfrozen mummy";
            this.Body = 0x9B;
            this.Hue = 0x480;
            this.BaseSoundID = 0x1D7;

            this.SetStr(450, 500);
            this.SetDex(200, 250);
            this.SetInt(800, 850);

            this.SetHits(1500);

            this.SetDamage(16, 20);

            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetDamageType(ResistanceType.Energy, 50);
            this.SetDamageType(ResistanceType.Cold, 50);

            this.SetResistance(ResistanceType.Physical, 35, 40);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 60, 80);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 70, 80);

            this.SetSkill(SkillName.Wrestling, 90.0, 100.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.MagicResist, 250.0);
            this.SetSkill(SkillName.Magery, 50.0, 60.0);
            this.SetSkill(SkillName.EvalInt, 50.0, 60.0);
            this.SetSkill(SkillName.Meditation, 80.0);

            this.Fame = 25000;
            this.Karma = -25000;

            for (int i = 0; i < Utility.RandomMinMax(0, 2); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if ( Utility.RandomDouble() < 0.6 )
            c.DropItem( new BrokenCrystals() );

            if ( Utility.RandomDouble() < 0.1 )
            c.DropItem( new ParrotItem() );
        }

        public UnfrozenMummy(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 2);
            AddLoot( LootPack.Parrot );
            AddLoot(LootPack.HighScrolls, 2);
            AddLoot(LootPack.MedScrolls);
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