using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a crystal daemon corpse")]
    public class CrystalDaemon : BaseCreature
    {
        [Constructable]
        public CrystalDaemon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a crystal daemon";
            this.Body = 0x310;
            this.Hue = 0x3E8;
            this.BaseSoundID = 0x47D;

            this.SetStr(140, 200);
            this.SetDex(120, 150);
            this.SetInt(800, 850);

            this.SetHits(200, 220);

            this.SetDamage(16, 20);

            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetDamageType(ResistanceType.Cold, 40);
            this.SetDamageType(ResistanceType.Energy, 60);

            this.SetResistance(ResistanceType.Physical, 20, 40);
            this.SetResistance(ResistanceType.Fire, 0, 20);
            this.SetResistance(ResistanceType.Cold, 60, 80);
            this.SetResistance(ResistanceType.Poison, 20, 40);
            this.SetResistance(ResistanceType.Energy, 65, 75);

            this.SetSkill(SkillName.Wrestling, 60.0, 80.0);
            this.SetSkill(SkillName.Tactics, 70.0, 80.0);
            this.SetSkill(SkillName.MagicResist, 100.0, 110.0);
            this.SetSkill(SkillName.Magery, 120.0, 130.0);
            this.SetSkill(SkillName.EvalInt, 100.0, 110.0);
            this.SetSkill(SkillName.Meditation, 100.0, 110.0);

            this.Fame = 15000;
            this.Karma = -15000;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if ( Utility.RandomDouble() < 0.4 )
            c.DropItem( new ScatteredCrystals() );
        }

        public CrystalDaemon(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 3);
            this.AddLoot(LootPack.HighScrolls);
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