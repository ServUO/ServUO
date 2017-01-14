using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Guile corpse")]
    public class Guile : Changeling
    {
        [Constructable]
        public Guile()
        {


            this.Hue = this.DefaultHue;

            this.SetStr(53, 214);
            this.SetDex(243, 367);
            this.SetInt(369, 586);

            this.SetHits(1013, 1058);
            this.SetStam(243, 367);
            this.SetMana(369, 586);

            this.SetDamage(14, 20);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 80, 90);
            this.SetResistance(ResistanceType.Fire, 43, 46);
            this.SetResistance(ResistanceType.Cold, 42, 44);
            this.SetResistance(ResistanceType.Poison, 42, 50);
            this.SetResistance(ResistanceType.Energy, 47, 50);

            this.SetSkill(SkillName.Wrestling, 12.8, 16.7);
            this.SetSkill(SkillName.Tactics, 102.6, 131.0);
            this.SetSkill(SkillName.MagicResist, 141.2, 161.6);
            this.SetSkill(SkillName.Magery, 108.4, 120.0);
            this.SetSkill(SkillName.EvalInt, 108.4, 120.0);
            this.SetSkill(SkillName.Meditation, 109.2, 120.0);

            this.Fame = 21000;
            this.Karma = -21000;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Guile(Serial serial)
            : base(serial)
        {
        }
		public override bool CanBeParagon { get { return false; } }
        public override string DefaultName
        {
            get
            {
                return "Guile";
            }
        }
        public override int DefaultHue
        {
            get
            {
                return 0x3F;
            }
        }
        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomBool())
            {
                if (!Kappa.IsBeingDrained(defender) && this.Mana > 14)
                {
                    defender.SendLocalizedMessage(1070848); // You feel your life force being stolen away.
                    Kappa.BeginLifeDrain(defender, this);
                    this.Mana -= 15;
                }
            }
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 2);
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