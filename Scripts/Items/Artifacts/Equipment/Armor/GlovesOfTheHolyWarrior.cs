using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class GlovesOfTheHolyWarrior : PlateGloves
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GlovesOfTheHolyWarrior()
        {
            Name = "Gloves of the Holy Warrior";
            Hue = 1153;

            SkillBonuses.SetValues(0, SkillName.Chivalry, 15.0);
            SkillBonuses.SetValues(1, SkillName.Necromancy, -30.0);

            Attributes.BonusStr = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusStam = 10;
            Attributes.BonusMana = 15;
            Attributes.LowerManaCost = 8;
        }

        public GlovesOfTheHolyWarrior(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 15; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        public override int BaseEnergyResistance { get { return 15; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int OnHit(BaseWeapon weapon, int damageTaken)
        {
            Mobile wearer = Parent as Mobile;
            Mobile attacker = weapon.Parent as Mobile;

            if (wearer != null && attacker != null && attacker is BaseCreature && 0.10 > Utility.RandomDouble())
            {
                ReactiveHolyLight(wearer);
            }

            return base.OnHit(weapon, damageTaken);
        }

        public static void ReactiveHolyLight(Mobile wearer)
        {
            List<Mobile> targets = new List<Mobile>();

            foreach (Mobile m in wearer.GetMobilesInRange(3))
            {
                if (m != wearer && m is BaseCreature && wearer.CanBeHarmful(m))
                    targets.Add(m);
            }

            if (targets.Count > 0)
            {
                wearer.PlaySound(0x212);
                wearer.PlaySound(0x206);

                Effects.SendLocationParticles(
                    EffectItem.Create(wearer.Location, wearer.Map, EffectItem.DefaultDuration),
                    0x376A, 1, 29, 0x47D, 2, 9962, 0);
                Effects.SendLocationParticles(
                    EffectItem.Create(new Point3D(wearer.X, wearer.Y, wearer.Z - 7), wearer.Map, EffectItem.DefaultDuration),
                    0x37C4, 1, 29, 0x47D, 2, 9502, 0);

                foreach (Mobile target in targets)
                {
                    wearer.DoHarmful(target);

                    int damage = Utility.RandomMinMax(15, 25);

                    AOS.Damage(target, wearer, damage, 0, 0, 0, 0, 100);
                }
            }

            targets.Clear();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class GargishKiltOfTheHolyWarrior : GargishPlateKilt
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishKiltOfTheHolyWarrior()
        {
            Name = "Gargish Kilt of the Holy Warrior";
            Hue = 1153;

            SkillBonuses.SetValues(0, SkillName.Chivalry, 15.0);
            SkillBonuses.SetValues(1, SkillName.Necromancy, -30.0);

            Attributes.BonusStr = 5;
            Attributes.BonusInt = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusStam = 10;
            Attributes.BonusMana = 15;
            Attributes.LowerManaCost = 8;
        }

        public GargishKiltOfTheHolyWarrior(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 15; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        public override int BaseEnergyResistance { get { return 15; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int OnHit(BaseWeapon weapon, int damageTaken)
        {
            Mobile wearer = Parent as Mobile;
            Mobile attacker = weapon.Parent as Mobile;

            if (wearer != null && attacker != null && attacker is BaseCreature && 0.10 > Utility.RandomDouble())
            {
                GlovesOfTheHolyWarrior.ReactiveHolyLight(wearer);
            }

            return base.OnHit(weapon, damageTaken);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
