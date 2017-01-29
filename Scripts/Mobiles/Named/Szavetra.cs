using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Szavetra corpse")]
    public class Szavetra : Succubus
    {
        [Constructable]
        public Szavetra()
        {
            this.Name = "Szavetra";

            this.SetStr(627, 655);
            this.SetDex(164, 193);
            this.SetInt(566, 595);

            this.SetHits(312, 415);

            this.SetDamage(20, 30);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Energy, 25);

            this.SetResistance(ResistanceType.Physical, 83, 90);
            this.SetResistance(ResistanceType.Fire, 72, 80);
            this.SetResistance(ResistanceType.Cold, 40, 49);
            this.SetResistance(ResistanceType.Poison, 51, 60);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.EvalInt, 90.3, 99.8);
            this.SetSkill(SkillName.Magery, 100.1, 100.6); // 10.1-10.6 on OSI, bug?
            this.SetSkill(SkillName.Meditation, 90.1, 110.0);
            this.SetSkill(SkillName.MagicResist, 112.2, 127.2);
            this.SetSkill(SkillName.Tactics, 91.2, 92.8);
            this.SetSkill(SkillName.Wrestling, 80.2, 86.4);

            this.Fame = 24000;
            this.Karma = -24000;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Szavetra(Serial serial)
            : base(serial)
        {
        }

		public override bool CanBeParagon { get { return false; } }
        public override bool DrainsLife { get { return true; } }

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