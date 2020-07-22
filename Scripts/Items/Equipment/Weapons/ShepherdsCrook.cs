using Server.Engines.CannedEvil;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System;

namespace Server.Items
{
    [FlipableAttribute(0xE81, 0xE82)]
    public class ShepherdsCrook : BaseStaff
    {
        [Constructable]
        public ShepherdsCrook()
            : base(0xE81)
        {
            Weight = 4.0;
        }

        public ShepherdsCrook(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.CrushingBlow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Disarm;
        public override int StrengthReq => 20;
        public override int MinDamage => 13;
        public override int MaxDamage => 16;
        public override float Speed => 2.75f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 50;

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

        public override void OnDoubleClick(Mobile from)
        {
            from.SendLocalizedMessage(502464); // Target the animal you wish to herd.
            from.Target = new HerdingTarget(this);
        }

        private class HerdingTarget : Target
        {
            private static readonly Type[] m_ChampTamables = new Type[]
            {
                typeof(StrongMongbat), typeof(Imp), typeof(Scorpion), typeof(GiantSpider),
                typeof(Snake), typeof(LavaLizard), typeof(Drake), typeof(Dragon),
                typeof(Kirin), typeof(Unicorn), typeof(GiantRat), typeof(Slime),
                typeof(DireWolf), typeof(HellHound), typeof(DeathwatchBeetle),
                typeof(LesserHiryu), typeof(Hiryu)
            };

            private readonly ShepherdsCrook m_Crook;

            public HerdingTarget(ShepherdsCrook crook)
                : base(10, false, TargetFlags.None)
            {
                m_Crook = crook;
            }

            protected override void OnTarget(Mobile from, object targ)
            {
                if (targ is BaseCreature)
                {
                    BaseCreature bc = (BaseCreature)targ;

                    if (IsHerdable(bc))
                    {
                        if (bc.Controlled)
                        {
                            bc.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502467, from.NetState); // That animal looks tame already.
                        }
                        else
                        {
                            from.SendLocalizedMessage(502475); // Click where you wish the animal to go.
                            from.Target = new InternalTarget(bc, m_Crook);
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(502468); // That is not a herdable animal.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502472); // You don't seem to be able to persuade that to move.
                }
            }

            private bool IsHerdable(BaseCreature bc)
            {
                if (bc.IsParagon)
                    return false;

                if (bc.Tamable)
                    return true;

                Map map = bc.Map;

                ChampionSpawnRegion region = Region.Find(bc.Home, map) as ChampionSpawnRegion;

                if (region != null)
                {
                    ChampionSpawn spawn = region.ChampionSpawn;

                    if (spawn != null && spawn.IsChampionSpawn(bc))
                    {
                        Type t = bc.GetType();

                        foreach (Type type in m_ChampTamables)
                            if (type == t)
                                return true;
                    }
                }

                return false;
            }

            private class InternalTarget : Target
            {
                private readonly BaseCreature m_Creature;
                private readonly ShepherdsCrook m_Crook;

                public InternalTarget(BaseCreature c, ShepherdsCrook crook)
                    : base(10, true, TargetFlags.None)
                {
                    m_Creature = c;
                    m_Crook = crook;
                }

                protected override void OnTarget(Mobile from, object targ)
                {
                    if (targ is IPoint2D)
                    {
                        double min = m_Creature.CurrentTameSkill - 30;
                        double max = m_Creature.CurrentTameSkill + 30 + Utility.Random(10);

                        if (max <= from.Skills[SkillName.Herding].Value)
                            m_Creature.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502471, from.NetState); // That wasn't even challenging.

                        if (from.CheckTargetSkill(SkillName.Herding, m_Creature, min, max))
                        {
                            IPoint2D p = (IPoint2D)targ;

                            if (targ != from)
                                p = new Point2D(p.X, p.Y);

                            m_Creature.TargetLocation = p;
                            from.SendLocalizedMessage(502479); // The animal walks where it was instructed to.

                            if (Siege.SiegeShard && m_Crook is IUsesRemaining)
                            {
                                Siege.CheckUsesRemaining(from, m_Crook);
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(502472); // You don't seem to be able to persuade that to move.
                        }
                    }
                }
            }
        }
    }
}
