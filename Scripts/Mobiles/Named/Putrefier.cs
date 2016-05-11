using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Putrefier corpse")]
    public class Putrefier : Balron
    {
        [Constructable]
        public Putrefier()
        {
            this.Name = "Putrefier";
            this.Hue = 63;

            this.SetStr(1057, 1400);
            this.SetDex(232, 560);
            this.SetInt(201, 440);

            this.SetHits(3010, 4092);

            this.SetDamage(27, 34);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Fire, 0);
            this.SetDamageType(ResistanceType.Poison, 50);
            this.SetDamageType(ResistanceType.Energy, 0);

            this.SetResistance(ResistanceType.Physical, 65, 80);
            this.SetResistance(ResistanceType.Fire, 65, 80);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.Wrestling, 111.2, 128.0);
            this.SetSkill(SkillName.Tactics, 115.2, 125.2);
            this.SetSkill(SkillName.MagicResist, 143.4, 170.0);
            this.SetSkill(SkillName.Anatomy, 44.6, 67.0);
            this.SetSkill(SkillName.Magery, 117.6, 118.8);
            this.SetSkill(SkillName.EvalInt, 113.0, 128.8);
            this.SetSkill(SkillName.Meditation, 41.4, 85.0);
            this.SetSkill(SkillName.Poisoning, 45.0, 50.0);

            this.Fame = 24000;
            this.Karma = -24000;

            for (int i = 0; i < Utility.RandomMinMax(0, 2); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Putrefier(Serial serial)
            : base(serial)
        {
        }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            c.DropItem( new SpleenOfThePutrefier() );

            if ( Utility.RandomDouble() < 0.6 )
            c.DropItem( new ParrotItem() );

            if ( Paragon.ChestChance > Utility.RandomDouble() )
            c.DropItem( new ParagonChest( Name, TreasureMapLevel ) );
        }

        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Deadly;
            }
        }// Becomes Lethal with Paragon bonus
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 3);
            this.AddLoot(LootPack.MedScrolls, 2);
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