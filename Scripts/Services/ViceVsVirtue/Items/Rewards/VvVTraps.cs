using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Necromancy;
using System;
using System.Collections.Generic;

namespace Server.Engines.VvV
{
    public enum VvVTrapType
    {
        Explosion = 1015027, // Explosion
        Poison = 1028000, // Poison
        Cold = 1113466, // Freezing
        Energy = 1154942, // Shocking
        Blade = 1154943, // Blades
    }

    public enum DeploymentType
    {
        Proximaty = 1154939,
        Tripwire = 1154940
    }

    public class VvVTrap : Item, IRevealableItem
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DeploymentType DeploymentType { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public VvVTrap ParentTrap { get; set; }

        public List<VvVTrap> Links { get; set; }

        public override bool HandlesOnMovement => true;
        public bool CheckWhenHidden => true;

        public virtual int MinDamage => 0;
        public virtual int MaxDamage => 0;
        public virtual VvVTrapType TrapType => VvVTrapType.Explosion;

        public static int HiddenID = 8600;
        public static int VisibleID = 39818;

        public VvVTrap(Mobile owner, DeploymentType type) : base(HiddenID)
        {
            Owner = owner;
            DeploymentType = type;

            Movable = false;
            Hue = 0x3D8;
        }

        public bool SetTripwire(VvVTrapKit deed, Point3D myLocation, Point3D wireLocation, Map map)
        {
            Links = new List<VvVTrap>();

            MovementPath path = new MovementPath(myLocation, wireLocation, map);
            int x = myLocation.X;
            int y = myLocation.Y;

            if (path.Success)
            {
                for (int i = 0; i < path.Directions.Length; ++i)
                {
                    Movement.Movement.Offset(path.Directions[i], ref x, ref y);

                    Point3D p = new Point3D(x, y, Map.GetAverageZ(x, y));

                    if (p == myLocation)
                        continue;

                    VvVTrap trap = deed.ConstructTrap(Owner);
                    Links.Add(trap);
                    trap.ParentTrap = this;

                    trap.MoveToWorld(p, map);
                }

                return true;
            }

            return false;
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (IsEnemy(m) && DeploymentType == DeploymentType.Proximaty && m.InRange(Location, 3) && ViceVsVirtueSystem.IsEnemy(m, Owner))
            {
                Detonate(m);
            }
        }

        public bool CheckReveal(Mobile m)
        {
            if (!ViceVsVirtueSystem.IsVvV(m) || ItemID != HiddenID)
                return false;

            return Utility.Random(100) <= m.Skills[SkillName.DetectHidden].Value;
        }

        public void OnRevealed(Mobile m)
        {
            ItemID = VisibleID;

            if (Links != null)
            {
                Links.ForEach(l =>
                    {
                        if (!l.Deleted && l.ItemID == HiddenID)
                            l.ItemID = VisibleID;
                    });
            }

            if (ParentTrap != null)
            {
                if (ParentTrap.ItemID == HiddenID)
                    ParentTrap.ItemID = VisibleID;

                ParentTrap.OnRevealed(m);
            }
        }

        public bool CheckPassiveDetect(Mobile m)
        {
            if (m.InRange(Location, 6))
            {
                int skill = (int)m.Skills[SkillName.DetectHidden].Value;

                if (skill >= 80 && Utility.Random(600) < skill)
                    PrivateOverheadMessage(Network.MessageType.Regular, 0x21, 500813, m.NetState); // [trapped]
            }

            return false;
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (IsEnemy(m))
            {
                Detonate(m);
            }

            return base.OnMoveOver(m);
        }

        public bool IsEnemy(Mobile m)
        {
            if (Owner == null)
                return true;

            return ViceVsVirtueSystem.IsVvV(m) && ViceVsVirtueSystem.IsVvV(Owner) && ViceVsVirtueSystem.IsEnemy(m, Owner);
        }

        public virtual void Detonate(Mobile m)
        {
            if (Owner != null)
                Owner.DoHarmful(m);

            Delete();
        }

        public override void Delete()
        {
            base.Delete();

            if (Links != null)
            {
                Links.ForEach(l =>
                    {
                        if (!l.Deleted)
                            l.Delete();
                    });
            }

            if (ParentTrap != null && !ParentTrap.Deleted)
                ParentTrap.Delete();
        }

        public VvVTrap(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Owner);
            writer.Write((int)DeploymentType);

            writer.Write(Links != null ? Links.Count : 0);

            if (Links != null)
            {
                Links.ForEach(l => writer.Write(l));
            }

            writer.Write(ParentTrap);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Owner = reader.ReadMobile();
            DeploymentType = (DeploymentType)reader.ReadInt();

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                if (Links == null)
                    Links = new List<VvVTrap>();

                VvVTrap link = reader.ReadItem() as VvVTrap;

                if (link != null)
                    Links.Add(link);
            }

            ParentTrap = reader.ReadItem() as VvVTrap;
        }
    }

    public class VvVExplosionTrap : VvVTrap
    {
        public override int MinDamage => 40;
        public override int MaxDamage => 50;

        public VvVExplosionTrap(Mobile owner, DeploymentType type)
            : base(owner, type)
        {
        }

        public override void Detonate(Mobile m)
        {
            int dam = Utility.RandomMinMax(MinDamage, MaxDamage);

            if (DeploymentType == DeploymentType.Tripwire)
                dam *= 2;

            AOS.Damage(m, Owner, dam, 50, 50, 0, 0, 0);

            Effects.SendLocationEffect(GetWorldLocation(), Map, 0x36BD, 15, 10);
            Effects.PlaySound(GetWorldLocation(), Map, 0x307);

            base.Detonate(m);
        }

        public VvVExplosionTrap(Serial serial)
            : base(serial)
        {
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
        }
    }

    public class VvVPoisonTrap : VvVTrap
    {
        public override int MinDamage => 25;
        public override int MaxDamage => 35;
        public override VvVTrapType TrapType => VvVTrapType.Poison;

        public VvVPoisonTrap(Mobile owner, DeploymentType type)
            : base(owner, type)
        {
        }

        public override void Detonate(Mobile m)
        {
            int dam = Utility.RandomMinMax(MinDamage, MaxDamage);

            if (DeploymentType == DeploymentType.Tripwire)
                dam *= 2;

            AOS.Damage(m, Owner, dam, 0, 0, 0, 100, 0);
            m.ApplyPoison(Owner, Poison.Deadly);

            Effects.SendTargetEffect(m, 0x1145, 3, 16);
            Effects.PlaySound(GetWorldLocation(), Map, 0x230);

            base.Detonate(m);
        }

        public VvVPoisonTrap(Serial serial) : base(serial)
        {
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
        }
    }

    public class VvVColdTrap : VvVTrap
    {
        public override int MinDamage => 25;
        public override int MaxDamage => 35;
        public override VvVTrapType TrapType => VvVTrapType.Cold;

        public VvVColdTrap(Mobile owner, DeploymentType type)
            : base(owner, type)
        {
        }

        public override void Detonate(Mobile m)
        {
            int dam = Utility.RandomMinMax(MinDamage, MaxDamage);

            if (DeploymentType == DeploymentType.Tripwire)
                dam *= 2;

            AOS.Damage(m, Owner, dam, 0, 0, 100, 0, 0);
            m.FixedParticles(0x374A, 1, 15, 9502, 97, 3, (EffectLayer)255);

            m.Paralyze(TimeSpan.FromSeconds(5));

            Effects.SendLocationParticles(m, 0x374A, 1, 30, 97, 3, 9502, 0);
            Effects.PlaySound(GetWorldLocation(), Map, 0x1FB);

            base.Detonate(m);
        }

        public VvVColdTrap(Serial serial) : base(serial)
        {
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
        }
    }

    public class VvVEnergyTrap : VvVTrap
    {
        public override int MinDamage => 25;
        public override int MaxDamage => 35;
        public override VvVTrapType TrapType => VvVTrapType.Energy;

        public VvVEnergyTrap(Mobile owner, DeploymentType type)
            : base(owner, type)
        {
        }

        public override void Detonate(Mobile m)
        {
            int dam = Utility.RandomMinMax(MinDamage, MaxDamage);

            if (DeploymentType == DeploymentType.Tripwire)
                dam *= 2;

            Effects.SendBoltEffect(m, true, 0);
            AOS.Damage(m, Owner, dam, 0, 0, 100, 0, 0);

            MortalStrike.BeginWound(m, TimeSpan.FromSeconds(3));

            base.Detonate(m);
        }

        public VvVEnergyTrap(Serial serial) : base(serial)
        {
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
        }
    }

    public class VvVBladeTrap : VvVTrap
    {
        public override int MinDamage => 25;
        public override int MaxDamage => 35;
        public override VvVTrapType TrapType => VvVTrapType.Blade;

        public VvVBladeTrap(Mobile owner, DeploymentType type)
            : base(owner, type)
        {
        }

        public override void Detonate(Mobile m)
        {
            int dam = Utility.RandomMinMax(MinDamage, MaxDamage);

            if (DeploymentType == DeploymentType.Tripwire)
                dam *= 2;

            AOS.Damage(m, Owner, dam, 100, 0, 0, 0, 0);
            Effects.SendLocationEffect(m.Location, m.Map, 0x11AD, 25, 10);
            Effects.PlaySound(m.Location, m.Map, 0x218);

            TransformContext context = TransformationSpellHelper.GetContext(m);

            if ((context != null && (context.Type == typeof(LichFormSpell) || context.Type == typeof(WraithFormSpell))) ||
                (m is BaseCreature && ((BaseCreature)m).BleedImmune))
                return;

            m.SendLocalizedMessage(1060160); // You are bleeding!
            BleedAttack.BeginBleed(m, Owner, false);

            base.Detonate(m);
        }

        public VvVBladeTrap(Serial serial) : base(serial)
        {
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
        }
    }
}