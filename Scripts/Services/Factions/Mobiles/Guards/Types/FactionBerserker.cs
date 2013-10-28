using System;
using Server.Items;

namespace Server.Factions
{
    public class FactionBerserker : BaseFactionGuard
    {
        [Constructable]
        public FactionBerserker()
            : base("the berserker")
        {
            this.GenerateBody(false, false);

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

            this.AddItem(this.Immovable(this.Rehued(new BodySash(), 1645)));
            this.AddItem(this.Immovable(this.Rehued(new Kilt(), 1645)));
            this.AddItem(this.Immovable(this.Rehued(new Sandals(), 1645)));
            this.AddItem(this.Newbied(new DoubleAxe()));

            this.HairItemID = 0x2047; // Afro
            this.HairHue = 0x29;

            this.FacialHairItemID = 0x204B; // Medium Short Beard
            this.FacialHairHue = 0x29;

            this.PackItem(new Bandage(Utility.RandomMinMax(30, 40)));
            this.PackStrongPotions(6, 12);
        }

        public FactionBerserker(Serial serial)
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