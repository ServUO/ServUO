using System;

namespace Server.Mobiles
{
    public class NatureFury : BaseCreature
    {
        [Constructable]
        public NatureFury()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a nature's fury";
            Body = 0x33;
            Hue = 0x4001;

            SetStr(150);
            SetDex(150);
            SetInt(100);

            SetHits(80);
            SetStam(250);
            SetMana(0);

            SetDamage(6, 8);

            SetDamageType(ResistanceType.Poison, 100);
            SetDamageType(ResistanceType.Physical, 0);
            SetResistance(ResistanceType.Physical, 90);

            SetSkill(SkillName.Wrestling, 90.0);
            SetSkill(SkillName.MagicResist, 70.0);
            SetSkill(SkillName.Tactics, 100.0);

            Fame = 0;
            Karma = 0;

            ControlSlots = 1;
        }

        public NatureFury(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteCorpseOnDeath => true;
        public override bool IsHouseSummonable => true;
        public override double DispelDifficulty => 125.0;
        public override double DispelFocus => 90.0;
        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override void MoveToWorld(Point3D loc, Map map)
        {
            base.MoveToWorld(loc, map);
            Timer.DelayCall(TimeSpan.Zero, DoEffects);
        }

        public void DoEffects()
        {
            FixedParticles(0x91C, 10, 180, 0x2543, 0, 0, EffectLayer.Waist);
            PlaySound(0xE);
            PlaySound(0x1BC);

            if (Alive && !Deleted)
                Timer.DelayCall(TimeSpan.FromSeconds(7.0), DoEffects);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Delete();
        }
    }
}
