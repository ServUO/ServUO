using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a crystal vortex corpse")]
    public class CrystalVortex : BaseCreature
    {
        [Constructable]
        public CrystalVortex()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a crystal vortex";
            this.Body = 0xD;
            this.Hue = 0x2B2;
            this.BaseSoundID = 0x107;

            this.SetStr(800, 900);
            this.SetDex(500, 600);
            this.SetInt(200);

            this.SetHits(350, 400);
            this.SetMana(0);

            this.SetDamage(15, 20);

            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetDamageType(ResistanceType.Cold, 50);
            this.SetDamageType(ResistanceType.Energy, 50);

            this.SetResistance(ResistanceType.Physical, 60, 80);
            this.SetResistance(ResistanceType.Fire, 0, 10);
            this.SetResistance(ResistanceType.Cold, 70, 80);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 60, 90);

            this.SetSkill(SkillName.MagicResist, 120.0);
            this.SetSkill(SkillName.Tactics, 120.0);
            this.SetSkill(SkillName.Wrestling, 120.0);

            this.Fame = 17000;
            this.Karma = -17000;

            for (int i = 0; i < Utility.RandomMinMax(0, 2); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public CrystalVortex(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 2);
            AddLoot( LootPack.Parrot );
            AddLoot(LootPack.MedScrolls);
            AddLoot(LootPack.HighScrolls);
        }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if ( Utility.RandomDouble() < 0.75 )
            c.DropItem( new CrystallineFragments() );

            if ( Utility.RandomDouble() < 0.06 )
            c.DropItem( new JaggedCrystals() );
        }

        public override int GetAngerSound()
        {
            return 0x15;
        }

        public override int GetAttackSound()
        {
            return 0x28;
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