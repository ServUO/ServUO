
using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Gnaw corpse")]
    public class Gnaw : DireWolf
    {
        [Constructable]
        public Gnaw()
        {

            this.Name = "Gnaw";
            this.Hue = 0x130;

            this.SetStr(142, 169);
            this.SetDex(102, 145);
            this.SetInt(44, 69);

            this.SetHits(786, 837);
            this.SetStam(102, 145);
            this.SetMana(44, 69);

            this.SetDamage(16, 22);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 23, 40);

            this.SetSkill(SkillName.Wrestling, 96.3, 119.7);
            this.SetSkill(SkillName.Tactics, 89.5, 107.7);
            this.SetSkill(SkillName.MagicResist, 93.6, 112.8);

            this.Fame = 17500;
            this.Karma = -17500;

            Tamable = false;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }
        public override bool CanBeParagon { get { return false; } }
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.3)
                c.DropItem(new GnawsFang());
        }

        public Gnaw(Serial serial)
            : base(serial)
        {
        }

        /*public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }*/
        public override int Hides
        {
            get
            {
                return 28;
            }
        }
        public override int Meat
        {
            get
            {
                return 4;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 2);
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
