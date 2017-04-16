using System;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("a frost mite corpse")]
    public class FrostMite : BaseCreature
    {
        [Constructable]
        public FrostMite() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Frost Mite";
            this.Body = 0x590;
            this.Female = true;

            this.SetStr(1017);
            this.SetDex(164);
            this.SetInt(283);

            this.SetHits(862);

            this.SetDamage(21, 28);

            this.SetDamageType(ResistanceType.Cold, 100);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Fire, 15, 25);
            this.SetResistance(ResistanceType.Cold, 90, 100);
            this.SetResistance(ResistanceType.Poison, 50, 70);
            this.SetResistance(ResistanceType.Energy, 40, 45);

            this.SetSkill(SkillName.MagicResist, 50.0, 85.0);
            this.SetSkill(SkillName.Tactics, 70.0, 105.0);
            this.SetSkill(SkillName.Wrestling, 70.0, 110.0);
            this.SetSkill(SkillName.DetectHidden, 60.0, 80.0);
            this.SetSkill(SkillName.Focus, 100.0, 115.0);

            this.Tamable = true;
            this.ControlSlots = 3;
            this.MinTameSkill = 102.0;
        }

        public override int Meat { get { return 5; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override bool HasAura { get { return true; } }
        public override int AuraRange { get { return 2; } }
        public override int AuraBaseDamage { get { return 15; } }
        public override int AuraFireDamage { get { return 0; } }
        public override int AuraColdDamage { get { return 100; } }
        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }

        public override void AuraEffect(Mobile m)
        {
            m.SendLocalizedMessage(1008111, false, m.Name); //  : The intense cold is damaging you!
        }

        public FrostMite(Serial serial) : base(serial)
        {
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