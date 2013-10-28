using System;
using Server.Items;

namespace Server.Factions
{
    public class FactionWizard : BaseFactionGuard
    {
        [Constructable]
        public FactionWizard()
            : base("the wizard")
        {
            this.GenerateBody(false, false);

            this.SetStr(151, 175);
            this.SetDex(61, 85);
            this.SetInt(151, 175);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 60);
            this.SetResistance(ResistanceType.Fire, 40, 60);
            this.SetResistance(ResistanceType.Cold, 40, 60);
            this.SetResistance(ResistanceType.Energy, 40, 60);
            this.SetResistance(ResistanceType.Poison, 40, 60);

            this.VirtualArmor = 32;

            this.SetSkill(SkillName.Macing, 110.0, 120.0);
            this.SetSkill(SkillName.Wrestling, 110.0, 120.0);
            this.SetSkill(SkillName.Tactics, 110.0, 120.0);
            this.SetSkill(SkillName.MagicResist, 110.0, 120.0);
            this.SetSkill(SkillName.Healing, 110.0, 120.0);
            this.SetSkill(SkillName.Anatomy, 110.0, 120.0);

            this.SetSkill(SkillName.Magery, 110.0, 120.0);
            this.SetSkill(SkillName.EvalInt, 110.0, 120.0);
            this.SetSkill(SkillName.Meditation, 110.0, 120.0);

            this.AddItem(this.Immovable(this.Rehued(new WizardsHat(), 1325)));
            this.AddItem(this.Immovable(this.Rehued(new Sandals(), 1325)));
            this.AddItem(this.Immovable(this.Rehued(new Robe(), 1310)));
            this.AddItem(this.Immovable(this.Rehued(new LeatherGloves(), 1325)));
            this.AddItem(this.Newbied(this.Rehued(new GnarledStaff(), 1310)));

            this.PackItem(new Bandage(Utility.RandomMinMax(30, 40)));
            this.PackStrongPotions(6, 12);
        }

        public FactionWizard(Serial serial)
            : base(serial)
        {
        }

        public override GuardAI GuardAI
        {
            get
            {
                return GuardAI.Magic | GuardAI.Smart | GuardAI.Bless | GuardAI.Curse;
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