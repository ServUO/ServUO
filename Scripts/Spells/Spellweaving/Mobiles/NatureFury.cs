using System;

namespace Server.Mobiles
{
    public class NatureFury : BaseCreature
    {
        [Constructable]
        public NatureFury()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a nature's fury";
            this.Body = 0x33;
            this.Hue = 0x4001;

            this.SetStr(150);
            this.SetDex(150);
            this.SetInt(100);

            this.SetHits(80);
            this.SetStam(250);
            this.SetMana(0);

            this.SetDamage(6, 8);

            this.SetDamageType(ResistanceType.Poison, 100);
            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetResistance(ResistanceType.Physical, 90);

            this.SetSkill(SkillName.Wrestling, 90.0);
            this.SetSkill(SkillName.MagicResist, 70.0);
            this.SetSkill(SkillName.Tactics, 100.0);

            this.Fame = 0;
            this.Karma = 0;

            this.ControlSlots = 1;
        }

        public NatureFury(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteCorpseOnDeath
        {
            get
            {
                return Core.AOS;
            }
        }
        public override bool IsHouseSummonable
        {
            get
            {
                return true;
            }
        }
        public override double DispelDifficulty
        {
            get
            {
                return 125.0;
            }
        }
        public override double DispelFocus
        {
            get
            {
                return 90.0;
            }
        }
        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override void MoveToWorld(Point3D loc, Map map)
        {
            base.MoveToWorld(loc, map);
            Timer.DelayCall(TimeSpan.Zero, DoEffects);
        }

        public void DoEffects()
        {
            this.FixedParticles(0x91C, 10, 180, 0x2543, 0, 0, EffectLayer.Waist);
            this.PlaySound(0xE);
            this.PlaySound(0x1BC);

            if (this.Alive && !this.Deleted)
                Timer.DelayCall(TimeSpan.FromSeconds(7.0), DoEffects);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.Delete();
        }
    }
}