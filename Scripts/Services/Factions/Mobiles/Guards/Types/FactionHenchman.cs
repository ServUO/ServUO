using System;
using Server.Items;

namespace Server.Factions
{
    public class FactionHenchman : BaseFactionGuard
    {
        [Constructable]
        public FactionHenchman()
            : base("the henchman")
        {
            this.GenerateBody(false, true);

            this.SetStr(91, 115);
            this.SetDex(61, 85);
            this.SetInt(81, 95);

            this.SetDamage(10, 14);

            this.SetResistance(ResistanceType.Physical, 10, 30);
            this.SetResistance(ResistanceType.Fire, 10, 30);
            this.SetResistance(ResistanceType.Cold, 10, 30);
            this.SetResistance(ResistanceType.Energy, 10, 30);
            this.SetResistance(ResistanceType.Poison, 10, 30);

            this.VirtualArmor = 8;

            this.SetSkill(SkillName.Fencing, 80.0, 90.0);
            this.SetSkill(SkillName.Wrestling, 80.0, 90.0);
            this.SetSkill(SkillName.Tactics, 80.0, 90.0);
            this.SetSkill(SkillName.MagicResist, 80.0, 90.0);
            this.SetSkill(SkillName.Healing, 80.0, 90.0);
            this.SetSkill(SkillName.Anatomy, 80.0, 90.0);

            this.AddItem(new StuddedChest());
            this.AddItem(new StuddedLegs());
            this.AddItem(new StuddedArms());
            this.AddItem(new StuddedGloves());
            this.AddItem(new StuddedGorget());
            this.AddItem(new Boots());
            this.AddItem(this.Newbied(new Spear()));

            this.PackItem(new Bandage(Utility.RandomMinMax(10, 20)));
            this.PackWeakPotions(1, 4);
        }

        public FactionHenchman(Serial serial)
            : base(serial)
        {
        }

        public override GuardAI GuardAI
        {
            get
            {
                return GuardAI.Melee;
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