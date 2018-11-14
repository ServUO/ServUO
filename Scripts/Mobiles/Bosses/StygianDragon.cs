using System;
using System.Collections.Generic;

using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a stygian dragon corpse")]
    public class StygianDragon : BaseSABoss
    {
        private DateTime m_Delay;

        [Constructable]
        public StygianDragon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.3, 0.5)
        {
            Name = "Stygian Dragon";
            Body = 826;
            BaseSoundID = 362;

            SetStr(702);
            SetDex(250);
            SetInt(180);

            SetHits(30000);
            SetStam(431);
            SetMana(180);

            SetDamage(33, 55);

            SetDamageType(ResistanceType.Physical, 25);
            SetDamageType(ResistanceType.Fire, 50);
            SetDamageType(ResistanceType.Energy, 25);

            SetResistance(ResistanceType.Physical, 80, 90);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 80, 90);

            SetSkill(SkillName.Anatomy, 100.0);
            SetSkill(SkillName.MagicResist, 150.0, 155.0);
            SetSkill(SkillName.Tactics, 120.7, 125.0);
            SetSkill(SkillName.Wrestling, 115.0, 117.7);

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 60;

            Tamable = false;

            SetWeaponAbility(WeaponAbility.Bladeweave);
            SetWeaponAbility(WeaponAbility.TalonStrike);
        }

        public StygianDragon(Serial serial)
            : base(serial)
        {
        }        

        public override Type[] UniqueSAList
        {
            get
            {
                return new Type[]
                {
                    typeof(BurningAmber), typeof(DraconisWrath), typeof(DragonHideShield), typeof(FallenMysticsSpellbook),
                    typeof(LifeSyphon), typeof(GargishSignOfOrder), typeof(HumanSignOfOrder), typeof(VampiricEssence)
                };
            }
        }
        public override Type[] SharedSAList
        {
            get
            {
                return new Type[]
                {
                    typeof(AxesOfFury), typeof(SummonersKilt), typeof(GiantSteps), typeof(StoneDragonsTooth),
                    typeof(TokenOfHolyFavor)
                };
            }
        }

        public override bool CausesTrueFear { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool Unprovokable { get { return false; } }
        public override bool BardImmune { get { return false; } }
        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override bool AutoDispel { get { return !Controlled; } }
        public override int Meat { get { return 19; } }
        public override int Hides { get { return 30; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Scales { get { return 7; } }
        public override ScaleType ScaleType { get { return (Body == 12 ? ScaleType.Yellow : ScaleType.Red); } }
        public override int DragonBlood { get { return 48; } }
        public override bool CanFlee { get { return false; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.AosSuperBoss, 4);
            AddLoot(LootPack.Gems, 8);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null)
                return;

            if (DateTime.UtcNow > m_Delay)
            {
                if (Utility.RandomBool())
                    CrimsonMeteor(this, Combatant.Location, 75, 120, true, false, true, 15, 15, 25);
                else
                    DoStygianFireball();

                m_Delay = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 45));
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new StygianDragonHead());
			
			if ( Paragon.ChestChance > Utility.RandomDouble() )
            	c.DropItem( new ParagonChest( Name, TreasureMapLevel ) );
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        #region Crimson Meteor
        public static void CrimsonMeteor(Mobile from, int damage)
        {
            CrimsonMeteor(from, from.Location, damage, damage, true, false, false, 5, 5, 30);
        }

        public static void CrimsonMeteor(Mobile from, Point3D loc, int minDam, int maxDam, bool doField, bool fromTop, bool damage, int width, int height, int duration)
        {
            if (!from.Alive || !from.Frozen || !from.Paralyzed || from.Map == null || from.Map == Map.Internal)
                return;

            //from.Say("*Shooting Meteor !!*");  

            new CrimsonMeteorTimer(from, loc, minDam, maxDam, doField, fromTop, damage, width, height, duration).Start();
        }

        public class CrimsonMeteorTimer : Timer
        {
            private Mobile m_From;
            private Map m_Map;
            private int m_MinDamage, m_MaxDamage;
            private int m_Count;
            private int m_MaxCount;
            private bool m_Field, m_FromTop, m_DoneDamage;
            private Point3D m_LastTarget;
            private Rectangle2D m_ShowerArea;
            private List<Mobile> m_ToDamage;

            public CrimsonMeteorTimer(Mobile from, Point3D loc, int minDam, int maxDam, bool doField, bool fromTop, bool damage, int width, int height, int duration)
                : base(TimeSpan.FromMilliseconds(250.0), TimeSpan.FromMilliseconds(250.0))
            {
                m_From = from;
                m_Map = from.Map;
                m_MinDamage = minDam;
                m_MaxDamage = maxDam;
                m_Count = 0;
                m_MaxCount = duration; // in ticks
                m_LastTarget = loc;
                m_Field = doField;
                m_FromTop = fromTop;
                m_DoneDamage = false;
                m_ShowerArea = new Rectangle2D(loc.X - (width / 2), loc.Y - (height / 2), width, height);

                if (damage)
                {
                    m_ToDamage = new List<Mobile>();

                    IPooledEnumerable eable = m_Map.GetMobilesInBounds(m_ShowerArea);

                    foreach (Mobile m in eable)
                    {
                        if (m != from && m_From.CanBeHarmful(m))
                            m_ToDamage.Add(m);
                    }

                    eable.Free();
                }
            }

            protected override void OnTick()
            {
                if (m_From == null || m_From.Deleted || m_Map == null || m_Map == Map.Internal)
                {
                    Stop();
                    return;
                }

                if (m_Field && 0.33 > Utility.RandomDouble())
                    new FireField(m_From, 25, 10, 30, Utility.RandomBool(), m_LastTarget, m_Map);

                Point3D start = new Point3D();
                Point3D finish = new Point3D();

                finish.X = m_ShowerArea.X + Utility.Random(m_ShowerArea.Width);
                finish.Y = m_ShowerArea.Y + Utility.Random(m_ShowerArea.Height);
                finish.Z = m_Map.GetAverageZ(finish.X, finish.Y);

                if (m_FromTop)
                {
                    start.X = m_ShowerArea.X + Utility.Random(m_ShowerArea.Width);
                    start.Y = m_ShowerArea.Y + Utility.Random(m_ShowerArea.Height);
                    start.Z = finish.Z + 50;
                }
                else
                {
                    //objects move from upper right/right to left as per OSI
                    start.X = finish.X + Utility.RandomMinMax(-8, 8);
                    start.Y = finish.Y - 15;
                    start.Z = finish.Z + 50;
                }

                Effects.SendMovingParticles(
                    new Entity(Serial.Zero, start, m_Map),
                    new Entity(Serial.Zero, finish, m_Map),
                    0x36D4, 15, 0, false, false, 0, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                Effects.PlaySound(finish, m_Map, 0x11D);

                m_LastTarget = finish;
                m_Count++;

                if (m_Count >= m_MaxCount / 2 && !m_DoneDamage)
                {
                    if (m_ToDamage != null && m_ToDamage.Count > 0)
                    {
                        int damage;

                        foreach (Mobile mob in m_ToDamage)
                        {
                            damage = Utility.RandomMinMax(m_MinDamage, m_MaxDamage);

                            m_From.DoHarmful(mob);
                            AOS.Damage(mob, m_From, damage, 0, 100, 0, 0, 0);

                            mob.FixedParticles(0x36BD, 1, 15, 9502, 0, 3, (EffectLayer)255);
                        }
                    }

                    m_DoneDamage = true;
                    return;
                }

                if (m_Count >= m_MaxCount)
                    Stop();
            }
        }
        #endregion

        #region Fire Field
        public class FireField : Item
        {
            private Mobile m_Owner;
            private int m_MinDamage;
            private int m_MaxDamage;
            private DateTime m_Destroy;
            private Point3D m_MoveToPoint;
            private Map m_MoveToMap;
            private Timer m_Timer;
            private List<Mobile> m_List;

            [Constructable]
            public FireField(int duration, int min, int max, bool south, Point3D point, Map map)
                : this(null, duration, min, max, south, point, map)
            {
            }

            [Constructable]
            public FireField(Mobile owner, int duration, int min, int max, bool south, Point3D point, Map map)
                : base(GetItemID(south))
            {
                Movable = false;

                m_Owner = owner;
                m_MinDamage = min;
                m_MaxDamage = max;
                m_Destroy = DateTime.Now + TimeSpan.FromSeconds((double)duration + 1.5);
                m_MoveToPoint = point;
                m_MoveToMap = map;
                m_List = new List<Mobile>();
                m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), new TimerCallback(OnTick));
                Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1.5), new TimerCallback(Move));
            }

            private static int GetItemID(bool south)
            {
                if (south)
                    return 0x398C;
                else
                    return 0x3996;
            }

            public override void OnAfterDelete()
            {
                if (m_Timer != null)
                    m_Timer.Stop();
            }

            private void Move()
            {
                if (!Visible)
                    ItemID = 0x36FE;

                MoveToWorld(m_MoveToPoint, m_MoveToMap);
            }

            private void OnTick()
            {
                if (DateTime.Now > m_Destroy)
                    Delete();
                else if (m_MinDamage != 0)
                {
                    IPooledEnumerable eable = GetMobilesInRange(15);
                    foreach (Mobile m in eable)
                    {
                        if (m == null)
                            continue;
                        else if (m_Owner != null)
                        {
                            if (CanTargetMob(m))
                                m_List.Add(m);
                        }
                        else
                            m_List.Add(m);
                    }
                    eable.Free();

                    for (int i = 0; i < m_List.Count; i++)
                    {
                        if (m_List[i] != null)
                            DealDamage(m_List[i]);
                    }

                    m_List.Clear();
                    m_List = new List<Mobile>();
                }
            }

            public override bool OnMoveOver(Mobile m)
            {
                if (m_MinDamage != 0)
                    DealDamage(m);

                return true;
            }

            public void DealDamage(Mobile m)
            {
                if (m != m_Owner && (m_Owner == null || CanTargetMob(m)))
                    AOS.Damage(m, (m_Owner == null) ? m : m_Owner, Utility.RandomMinMax(m_MinDamage, m_MaxDamage), 0, 100, 0, 0, 0);
            }

            public bool CanTargetMob(Mobile m)
            {
                return m != m_Owner && m_Owner.CanBeHarmful(m, false) && (m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile));
            }

            public FireField(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                // Unsaved.
            }

            public override void Deserialize(GenericReader reader)
            {
            }
        }
        #endregion

        #region Stygian Fireball
        public void DoStygianFireball()
        {
            if (!(Combatant is Mobile) || !InRange(Combatant.Location, 10))
                return;

            new StygianFireballTimer(this, (Mobile)Combatant);
            PlaySound(0x1F3);
        }

        private class StygianFireballTimer : Timer
        {
            private StygianDragon m_Dragon;
            private Mobile m_Combatant;
            private int m_Ticks;

            public StygianFireballTimer(StygianDragon dragon, Mobile combatant)
                : base(TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(200))
            {
                m_Dragon = dragon;
                m_Combatant = combatant;
                m_Ticks = 0;
                Start();
            }

            protected override void OnTick()
            {
                m_Dragon.MovingParticles(m_Combatant, 0x46E6, 7, 0, false, true, 1265, 0, 9502, 4019, 0x026, 0);

                if (m_Ticks >= 10)
                {
                    int damage = Utility.RandomMinMax(120, 150);

                    Timer.DelayCall(TimeSpan.FromSeconds(.20), new TimerStateCallback(DoDamage_Callback), new object[] { m_Combatant, m_Dragon, damage });

                    Stop();
                }

                m_Ticks++;
            }

            public void DoDamage_Callback(object state)
            {
                object[] obj = (object[])state;

                Mobile c = (Mobile)obj[0];
                Mobile d = (Mobile)obj[1];
                int dam = (int)obj[2];

                d.DoHarmful(c);
                AOS.Damage(c, d, dam, false, 0, 0, 0, 0, 0, 100, 0, false);
            }
        }
        #endregion
    }
}
