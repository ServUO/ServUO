using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Moug-Guur corpse")]
    public class MougGuur : Ettin
    {
        [Constructable]
        public MougGuur()
        {
            this.Name = "Moug-Guur";

            this.SetStr(556, 575);
            this.SetDex(84, 94);
            this.SetInt(59, 73);

            this.SetHits(400, 415);

            this.SetDamage(12, 20);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 61, 65);
            this.SetResistance(ResistanceType.Fire, 16, 19);
            this.SetResistance(ResistanceType.Cold, 41, 46);
            this.SetResistance(ResistanceType.Poison, 21, 24);
            this.SetResistance(ResistanceType.Energy, 19, 25);

            this.SetSkill(SkillName.MagicResist, 70.2, 75.0);
            this.SetSkill(SkillName.Tactics, 80.8, 81.7);
            this.SetSkill(SkillName.Wrestling, 93.9, 99.4);

            this.Fame = 3000;
            this.Karma = -3000;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public MougGuur(Serial serial)
            : base(serial)
        {
        }
		public override bool CanBeParagon { get { return false; } }
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