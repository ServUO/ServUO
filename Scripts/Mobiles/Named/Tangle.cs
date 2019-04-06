using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a tangle corpse")]
    public class Tangle : BogThing
    {
        [Constructable]
        public Tangle()
            : base()
        {
            this.Name = "Tangle";
            this.Hue = 0x21;

            this.SetStr(843, 943);
            this.SetDex(58, 74);
            this.SetInt(46, 58);

            this.SetHits(2468, 2733);

            this.SetDamage(15, 28);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Poison, 40);

            this.SetResistance(ResistanceType.Physical, 50, 57);
            this.SetResistance(ResistanceType.Fire, 40, 43);
            this.SetResistance(ResistanceType.Cold, 30, 35);
            this.SetResistance(ResistanceType.Poison, 61, 69);
            this.SetResistance(ResistanceType.Energy, 41, 45);

            this.SetSkill(SkillName.Wrestling, 80.8, 94.6);
            this.SetSkill(SkillName.Tactics, 90.6, 100.4);
            this.SetSkill(SkillName.MagicResist, 108.4, 114.0);

            this.Fame = 16000;
            this.Karma = -16000;

            this.VirtualArmor = 54;

            for (int i = 0; i < Utility.RandomMinMax(1, 3); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Tangle(Serial serial)
            : base(serial)
        {
        }

        public override bool GivesMLMinorArtifact
        {
            get { return true; }
        }

        public override bool BardImmune
        {
            get
            {
                return !Core.AOS;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosUltraRich, 3);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.3)
                c.DropItem(new TaintedSeeds());
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