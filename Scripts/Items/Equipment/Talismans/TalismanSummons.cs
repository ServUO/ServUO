#region References
using Server.ContextMenus;
using Server.Items;
using Server.Regions;
using System;
using System.Collections.Generic;
#endregion

namespace Server.Mobiles
{
    public class BaseTalismanSummon : BaseCreature
    {
        private long m_NextMove;

        private DateTime m_SeperationStart;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime SeperationStart
        {
            get { return m_SeperationStart; }
            set { m_SeperationStart = value; }
        }

        public BaseTalismanSummon()
            : base(AIType.AI_Melee, FightMode.None, 10, 1, 0.1, 0.2)
        {
            SetHits(100);
            SetInt(100);
            SetDex(100);
        }

        public BaseTalismanSummon(Serial serial)
            : base(serial)
        { }

        public override bool Commandable => false;
        public override bool InitialInnocent => true;

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive && ControlMaster == from)
            {
                list.Add(new TalismanReleaseEntry(this));
            }
        }

        public virtual bool RangeCheck()
        {
            Mobile master = ControlMaster;

            if (Deleted || master == null || master.Deleted)
                return false;

            int dist = (int)master.GetDistanceToSqrt(Location);

            if (master.Map != Map || dist > 15)
            {
                if (m_SeperationStart == DateTime.MinValue)
                {
                    m_SeperationStart = DateTime.UtcNow + TimeSpan.FromMinutes(10);
                }
                else if (m_SeperationStart < DateTime.UtcNow)
                {
                    Delete();
                }

                return false;
            }

            if (m_SeperationStart != DateTime.MinValue)
            {
                m_SeperationStart = DateTime.MinValue;
            }

            int range = 4;

            if (!InRange(ControlMaster.Location, RangeHome) && InLOS(ControlMaster))
            {
                Point3D loc = Point3D.Zero;

                if (Map == master.Map)
                {
                    int x = (X > master.X) ? (master.X + range) : (master.X - range);
                    int y = (Y > master.Y) ? (master.Y + range) : (master.Y - range);

                    for (int i = 0; i < 10; i++)
                    {
                        loc.X = x + Utility.RandomMinMax(-1, 1);
                        loc.Y = y + Utility.RandomMinMax(-1, 1);

                        loc.Z = Map.GetAverageZ(loc.X, loc.Y);

                        if (Map.CanSpawnMobile(loc))
                        {
                            break;
                        }

                        loc = master.Location;
                    }

                    if (!Deleted)
                    {
                        SetLocation(loc, true);
                    }
                }

                return false;
            }

            return true;
        }

        public override void OnThink()
        {
            if (Deleted || Map == null)
            {
                return;
            }

            Mobile master = ControlMaster;

            if (master == null || master.Deleted)
            {
                Delete();
                return;
            }

            if (RangeCheck())
            {
                if (AIObject != null && AIObject.WalkMobileRange(master, 5, true, 1, 1))
                {
                    if (master.Combatant != null && master.InRange(master.Combatant, 1) && Core.TickCount > m_NextMove)
                    {
                        IDamageable combatant = master.Combatant;

                        if (!InRange(combatant.Location, 1))
                        {
                            for (int x = combatant.X - 1; x <= combatant.X + 1; x++)
                            {
                                for (int y = combatant.Y - 1; y <= combatant.Y + 1; y++)
                                {
                                    if (x == combatant.X && y == combatant.Y)
                                    {
                                        continue;
                                    }

                                    Point2D p = new Point2D(x, y);

                                    if (InRange(p, 1) && master.InRange(p, 1) && Map != null)
                                    {
                                        CurrentSpeed = .01;
                                        AIObject.MoveTo(new Point3D(x, y, Map.GetAverageZ(x, y)), false, 0);
                                        m_NextMove = Core.TickCount + 500;
                                    }
                                }
                            }
                        }
                        else
                        {
                            CurrentSpeed = .1;
                        }
                    }
                    else if (master.Combatant == null)
                    {
                        CurrentSpeed = .1;
                    }
                }
                else
                {
                    CurrentSpeed = .1;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        private class TalismanReleaseEntry : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;

            public TalismanReleaseEntry(Mobile m)
                : base(6118, 3)
            {
                m_Mobile = m;
            }

            public override void OnClick()
            {
                Effects.SendLocationParticles(
                    EffectItem.Create(m_Mobile.Location, m_Mobile.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(m_Mobile, m_Mobile.Map, 0x201);

                m_Mobile.Delete();
            }
        }
    }

    public class SummonedAntLion : BaseTalismanSummon
    {
        [Constructable]
        public SummonedAntLion()
        {
            Name = "an ant lion";
            Body = 787;
            BaseSoundID = 1006;

            SetStr(296, 320);
            SetDex(81, 105);
            SetInt(36, 60);

            SetHits(151, 162);

            SetDamage(7, 21);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Poison, 30);

            SetResistance(ResistanceType.Physical, 45, 60);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 30, 35);

            SetSkill(SkillName.MagicResist, 70.0);
            SetSkill(SkillName.Tactics, 90.0);
            SetSkill(SkillName.Wrestling, 90.0);
        }

        public SummonedAntLion(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SummonedArcticOgreLord : BaseTalismanSummon
    {
        [Constructable]
        public SummonedArcticOgreLord()
        {
            Name = "an arctic ogre lord";
            Body = 135;
            BaseSoundID = 427;

            SetStr(767, 945);
            SetDex(66, 75);
            SetInt(46, 70);

            SetHits(476, 552);

            SetDamage(20, 25);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Cold, 70);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 125.1, 140.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);
        }

        public SummonedArcticOgreLord(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SummonedBakeKitsune : BaseTalismanSummon
    {
        [Constructable]
        public SummonedBakeKitsune()
        {
            Name = "a bake kitsune";
            Body = 246;

            SetStr(171, 220);
            SetDex(126, 145);
            SetInt(376, 425);

            SetHits(301, 350);

            SetDamage(15, 22);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Energy, 30);

            SetResistance(ResistanceType.Physical, 40, 60);
            SetResistance(ResistanceType.Fire, 70, 90);
            SetResistance(ResistanceType.Cold, 40, 60);
            SetResistance(ResistanceType.Poison, 40, 60);
            SetResistance(ResistanceType.Energy, 40, 60);

            SetSkill(SkillName.EvalInt, 80.1, 90.0);
            SetSkill(SkillName.Magery, 80.1, 90.0);
            SetSkill(SkillName.MagicResist, 80.1, 100.0);
            SetSkill(SkillName.Tactics, 70.1, 90.0);
            SetSkill(SkillName.Wrestling, 50.1, 55.0);
        }

        public SummonedBakeKitsune(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SummonedBogling : BaseTalismanSummon
    {
        [Constructable]
        public SummonedBogling()
        {
            Name = "a bogling";
            Body = 779;
            BaseSoundID = 422;

            SetStr(96, 120);
            SetDex(91, 115);
            SetInt(21, 45);

            SetHits(58, 72);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 20, 25);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 15, 25);
            SetResistance(ResistanceType.Energy, 15, 25);

            SetSkill(SkillName.MagicResist, 75.1, 100.0);
            SetSkill(SkillName.Tactics, 55.1, 80.0);
            SetSkill(SkillName.Wrestling, 55.1, 75.0);
        }

        public SummonedBogling(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SummonedBullFrog : BaseTalismanSummon
    {
        [Constructable]
        public SummonedBullFrog()
        {
            Name = "a bull frog";
            Body = 81;
            Hue = Utility.RandomList(0x5AC, 0x5A3, 0x59A, 0x591, 0x588, 0x57F);
            BaseSoundID = 0x266;

            SetStr(46, 70);
            SetDex(6, 25);
            SetInt(11, 20);

            SetHits(28, 42);
            SetMana(0);

            SetDamage(1, 2);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 5, 10);

            SetSkill(SkillName.MagicResist, 25.1, 40.0);
            SetSkill(SkillName.Tactics, 40.1, 60.0);
            SetSkill(SkillName.Wrestling, 40.1, 60.0);
        }

        public SummonedBullFrog(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SummonedChicken : BaseTalismanSummon
    {
        [Constructable]
        public SummonedChicken()
        {
            Name = "a chicken";
            Body = 0xD0;
            BaseSoundID = 0x6E;

            SetStr(5);
            SetDex(15);
            SetInt(5);

            SetHits(3);
            SetMana(0);

            SetDamage(1);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 1, 5);

            SetSkill(SkillName.MagicResist, 4.0);
            SetSkill(SkillName.Tactics, 5.0);
            SetSkill(SkillName.Wrestling, 5.0);
        }

        public SummonedChicken(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SummonedCow : BaseTalismanSummon
    {
        [Constructable]
        public SummonedCow()
        {
            Name = "a cow";
            Body = Utility.RandomList(0xD8, 0xE7);
            BaseSoundID = 0x78;

            SetStr(30);
            SetDex(15);
            SetInt(5);

            SetHits(18);
            SetMana(0);

            SetDamage(1, 4);

            SetDamage(1, 4);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 5, 15);

            SetSkill(SkillName.MagicResist, 5.5);
            SetSkill(SkillName.Tactics, 5.5);
            SetSkill(SkillName.Wrestling, 5.5);
        }

        public SummonedCow(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SummonedDoppleganger : BaseTalismanSummon
    {
        [Constructable]
        public SummonedDoppleganger()
        {
            Name = "a doppleganger";
            Body = 0x309;
            BaseSoundID = 0x451;

            SetStr(81, 110);
            SetDex(56, 75);
            SetInt(81, 105);

            SetHits(101, 120);

            SetDamage(8, 12);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 75.1, 85.0);
            SetSkill(SkillName.Tactics, 70.1, 80.0);
            SetSkill(SkillName.Wrestling, 80.1, 90.0);
        }

        public SummonedDoppleganger(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SummonedFrostSpider : BaseTalismanSummon
    {
        [Constructable]
        public SummonedFrostSpider()
        {
            Name = "a frost spider";
            Body = 20;
            BaseSoundID = 0x388;

            SetStr(76, 100);
            SetDex(126, 145);
            SetInt(36, 60);

            SetHits(46, 60);
            SetMana(0);

            SetDamage(6, 16);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 80);

            SetResistance(ResistanceType.Physical, 25, 30);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 25.1, 40.0);
            SetSkill(SkillName.Tactics, 35.1, 50.0);
            SetSkill(SkillName.Wrestling, 50.1, 65.0);
        }

        public SummonedFrostSpider(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SummonedGreatHart : BaseTalismanSummon
    {
        [Constructable]
        public SummonedGreatHart()
        {
            Name = "a great hart";
            Body = 0xEA;

            SetStr(41, 71);
            SetDex(47, 77);
            SetInt(27, 57);

            SetHits(27, 41);
            SetMana(0);

            SetDamage(5, 9);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 20, 25);
            SetResistance(ResistanceType.Cold, 5, 10);

            SetSkill(SkillName.MagicResist, 26.8, 44.5);
            SetSkill(SkillName.Tactics, 29.8, 47.5);
            SetSkill(SkillName.Wrestling, 29.8, 47.5);
        }

        public SummonedGreatHart(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SummonedLavaSerpent : BaseTalismanSummon
    {
        private DateTime m_NextWave;

        [Constructable]
        public SummonedLavaSerpent()
        {
            Name = "a lava serpent";
            Body = 90;
            BaseSoundID = 219;

            SetStr(386, 415);
            SetDex(56, 80);
            SetInt(66, 85);

            SetHits(232, 249);
            SetMana(0);

            SetDamage(10, 22);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 80);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 25.3, 70.0);
            SetSkill(SkillName.Tactics, 65.1, 70.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);
        }

        public SummonedLavaSerpent(Serial serial)
            : base(serial)
        { }

        public override void OnThink()
        {
            if (m_NextWave < DateTime.UtcNow)
            {
                AreaHeatDamage();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        public void AreaHeatDamage()
        {
            Mobile mob = ControlMaster;

            if (mob != null)
            {
                if (mob.InRange(Location, 2))
                {
                    if (mob.IsStaff())
                    {
                        AOS.Damage(mob, Utility.Random(2, 3), 0, 100, 0, 0, 0);
                        mob.SendLocalizedMessage(1008112); // The intense heat is damaging you!
                    }
                }

                GuardedRegion r = Region as GuardedRegion;

                if (r != null && mob.Alive)
                {
                    IPooledEnumerable eable = GetMobilesInRange(2);
                    foreach (Mobile m in eable)
                    {
                        if (!mob.CanBeHarmful(m))
                        {
                            mob.CriminalAction(false);
                        }
                    }

                    eable.Free();
                }
            }

            m_NextWave = DateTime.UtcNow + TimeSpan.FromSeconds(3);
        }
    }

    public class SummonedOrcBrute : BaseTalismanSummon
    {
        [Constructable]
        public SummonedOrcBrute()
        {
            Body = 189;

            Name = "an orc brute";
            BaseSoundID = 0x45A;

            SetStr(767, 945);
            SetDex(66, 75);
            SetInt(46, 70);

            SetHits(476, 552);

            SetDamage(20, 25);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.Macing, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 125.1, 140.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);
        }

        public SummonedOrcBrute(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SummonedPanther : BaseTalismanSummon
    {
        [Constructable]
        public SummonedPanther()
        {
            Name = "a panther";
            Body = 0xD6;
            Hue = 0x901;
            BaseSoundID = 0x462;

            SetStr(61, 85);
            SetDex(86, 105);
            SetInt(26, 50);

            SetHits(37, 51);
            SetMana(0);

            SetDamage(4, 12);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 20, 25);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Cold, 10, 15);
            SetResistance(ResistanceType.Poison, 5, 10);

            SetSkill(SkillName.MagicResist, 15.1, 30.0);
            SetSkill(SkillName.Tactics, 50.1, 65.0);
            SetSkill(SkillName.Wrestling, 50.1, 65.0);
        }

        public SummonedPanther(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SummonedSheep : BaseTalismanSummon
    {
        [Constructable]
        public SummonedSheep()
        {
            Name = "a sheep";
            Body = 0xCF;
            BaseSoundID = 0xD6;

            SetStr(19);
            SetDex(25);
            SetInt(5);

            SetHits(12);
            SetMana(0);

            SetDamage(1, 2);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 5, 10);

            SetSkill(SkillName.MagicResist, 5.0);
            SetSkill(SkillName.Tactics, 6.0);
            SetSkill(SkillName.Wrestling, 5.0);
        }

        public SummonedSheep(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SummonedSkeletalKnight : BaseTalismanSummon
    {
        [Constructable]
        public SummonedSkeletalKnight()
        {
            Name = "a skeletal knight";
            Body = 147;
            BaseSoundID = 451;

            SetStr(196, 250);
            SetDex(76, 95);
            SetInt(36, 60);

            SetHits(118, 150);

            SetDamage(8, 18);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Cold, 60);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 65.1, 80.0);
            SetSkill(SkillName.Tactics, 85.1, 100.0);
            SetSkill(SkillName.Wrestling, 85.1, 95.0);
        }

        public SummonedSkeletalKnight(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SummonedVorpalBunny : BaseTalismanSummon
    {
        [Constructable]
        public SummonedVorpalBunny()
        {
            Name = "a vorpal bunny";
            Body = 205;
            Hue = 0x480;

            SetStr(15);
            SetDex(2000);
            SetInt(1000);

            SetHits(2000);
            SetStam(500);
            SetMana(0);

            SetDamage(1);

            SetDamageType(ResistanceType.Physical, 100);

            SetSkill(SkillName.MagicResist, 200.0);
            SetSkill(SkillName.Tactics, 5.0);
            SetSkill(SkillName.Wrestling, 5.0);

            Timer.DelayCall(TimeSpan.FromMinutes(30.0), BeginTunnel);
        }

        public SummonedVorpalBunny(Serial serial)
            : base(serial)
        { }

        public virtual void BeginTunnel()
        {
            if (Deleted)
            {
                return;
            }

            new VorpalBunny.BunnyHole().MoveToWorld(Location, Map);

            Frozen = true;
            Say("* The bunny begins to dig a tunnel back to its underground lair *");
            PlaySound(0x247);

            Timer.DelayCall(TimeSpan.FromSeconds(5.0), Delete);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SummonedWailingBanshee : BaseTalismanSummon
    {
        [Constructable]
        public SummonedWailingBanshee()
        {
            Name = "a wailing banshee";
            Body = 310;
            BaseSoundID = 0x482;

            SetStr(126, 150);
            SetDex(76, 100);
            SetInt(86, 110);

            SetHits(76, 90);

            SetDamage(10, 14);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 60);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 25, 30);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 70.1, 95.0);
            SetSkill(SkillName.Tactics, 45.1, 70.0);
            SetSkill(SkillName.Wrestling, 50.1, 70.0);
        }

        public SummonedWailingBanshee(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}
