using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Grobu corpse")]
    public class Grobu : BlackBear
    {
        [Constructable]
        public Grobu()
        {

            this.Name = "Grobu";
            this.Hue = 0x455;

            this.AI = AIType.AI_Melee;
            this.FightMode = FightMode.Closest;

            this.SetStr(192, 210);
            this.SetDex(132, 150);
            this.SetInt(50, 52);

            this.SetHits(1235, 1299);
            this.SetStam(132, 150);
            this.SetMana(9);

            this.SetDamage(15, 18);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 45);
            this.SetResistance(ResistanceType.Fire, 20, 40);
            this.SetResistance(ResistanceType.Cold, 32, 35);
            this.SetResistance(ResistanceType.Poison, 25, 30);
            this.SetResistance(ResistanceType.Energy, 22, 34);

            this.SetSkill(SkillName.Wrestling, 96.4, 119.0);
            this.SetSkill(SkillName.Tactics, 96.2, 116.5);
            this.SetSkill(SkillName.MagicResist, 66.2, 83.7);

            this.Fame = 1000;
            this.Karma = 1000;

            Tamable = false;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Grobu(Serial serial)
            : base(serial)
        {
        }
		public override bool CanBeParagon { get { return false; } }
        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            c.DropItem( new GrobusFur() );
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