using System;
using Server.Items;

namespace Server.Factions
{
    public class FactionDeathKnight : BaseFactionGuard
    {
        [Constructable]
        public FactionDeathKnight()
            : base("the death knight")
        {
            this.GenerateBody(false, false);
            this.Hue = 1;

            this.SetStr(126, 150);
            this.SetDex(61, 85);
            this.SetInt(81, 95);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 30, 50);
            this.SetResistance(ResistanceType.Fire, 30, 50);
            this.SetResistance(ResistanceType.Cold, 30, 50);
            this.SetResistance(ResistanceType.Energy, 30, 50);
            this.SetResistance(ResistanceType.Poison, 30, 50);

            this.VirtualArmor = 24;

            this.SetSkill(SkillName.Swords, 100.0, 110.0);
            this.SetSkill(SkillName.Wrestling, 100.0, 110.0);
            this.SetSkill(SkillName.Tactics, 100.0, 110.0);
            this.SetSkill(SkillName.MagicResist, 100.0, 110.0);
            this.SetSkill(SkillName.Healing, 100.0, 110.0);
            this.SetSkill(SkillName.Anatomy, 100.0, 110.0);

            this.SetSkill(SkillName.Magery, 100.0, 110.0);
            this.SetSkill(SkillName.EvalInt, 100.0, 110.0);
            this.SetSkill(SkillName.Meditation, 100.0, 110.0);

            Item shroud = new Item(0x204E);
            shroud.Layer = Layer.OuterTorso;

            this.AddItem(this.Immovable(this.Rehued(shroud, 1109)));
            this.AddItem(this.Newbied(this.Rehued(new ExecutionersAxe(), 2211)));

            this.PackItem(new Bandage(Utility.RandomMinMax(30, 40)));
            this.PackStrongPotions(6, 12);
        }

        public FactionDeathKnight(Serial serial)
            : base(serial)
        {
        }

        public override GuardAI GuardAI
        {
            get
            {
                return GuardAI.Melee | GuardAI.Curse | GuardAI.Bless;
            }
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