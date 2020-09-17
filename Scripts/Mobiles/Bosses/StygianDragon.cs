using Server.Items;
using Server.Network;
using Server.Spells;
using System;
using System.Collections.Generic;

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

            Tamable = false;

            SetWeaponAbility(WeaponAbility.Bladeweave);
            SetWeaponAbility(WeaponAbility.TalonStrike);
            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public StygianDragon(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList => new[]
                {
                    typeof(BurningAmber), typeof(DraconisWrath), typeof(DragonHideShield), typeof(FallenMysticsSpellbook),
                    typeof(LifeSyphon), typeof(GargishSignOfOrder), typeof(HumanSignOfOrder), typeof(VampiricEssence)
                };
        public override Type[] SharedSAList => new[]
                {
                    typeof(AxesOfFury), typeof(SummonersKilt), typeof(GiantSteps),
                    typeof(TokenOfHolyFavor)
                };

        public override bool AlwaysMurderer => true;
        public override bool Unprovokable => false;
        public override bool BardImmune => false;
        public override bool AutoDispel => !Controlled;
        public override int Meat => 19;
        public override int Hides => 30;
        public override HideType HideType => HideType.Barbed;
        public override int Scales => 7;
        public override ScaleType ScaleType => (Body == 12 ? ScaleType.Yellow : ScaleType.Red);
        public override int DragonBlood => 48;
        public override bool CanFlee => false;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 4);
            AddLoot(LootPack.Gems, 8);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null || !(Combatant is Mobile))
                return;

            if (DateTime.UtcNow > m_Delay)
            {
                switch (Utility.Random(3))
                {
                    case 0: CrimsonMeteor(this, (Mobile)Combatant, 70, 125); break;
                    case 1: DoStygianFireball(); break;
                    case 2: DoFireColumn(); break;
                }

                m_Delay = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60));
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new StygianDragonHead());

            if (Paragon.ChestChance > Utility.RandomDouble())
                c.DropItem(new ParagonChest(Name, 5));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        #region Crimson Meteor
        public static void CrimsonMeteor(Mobile owner, Mobile combatant, int minDamage, int maxDamage)
        {
            if (!combatant.Alive || combatant.Map == null || combatant.Map == Map.Internal)
                return;

            new CrimsonMeteorTimer(owner, combatant.Location, minDamage, maxDamage).Start();
        }

        public class CrimsonMeteorTimer : Timer
        {
            private readonly Mobile m_From;
            private readonly Map m_Map;
            private int m_Count;
            private readonly int m_MaxCount;
            private bool m_DoneDamage;
            private Point3D m_LastTarget;
            private Rectangle2D m_ShowerArea;
            private readonly List<Mobile> m_ToDamage;

            private readonly int m_MinDamage, m_MaxDamage;

            public CrimsonMeteorTimer(Mobile from, Point3D loc, int min, int max)
                : base(TimeSpan.FromMilliseconds(250.0), TimeSpan.FromMilliseconds(250.0))
            {
                m_From = from;
                m_Map = from.Map;
                m_Count = 0;
                m_MaxCount = 25; // in ticks
                m_LastTarget = loc;
                m_DoneDamage = false;
                m_ShowerArea = new Rectangle2D(loc.X - 2, loc.Y - 2, 4, 4);

                m_MinDamage = min;
                m_MaxDamage = max;

                m_ToDamage = new List<Mobile>();

                IPooledEnumerable eable = m_Map.GetMobilesInBounds(m_ShowerArea);

                foreach (Mobile m in eable)
                {
                    if (m != from && m_From.CanBeHarmful(m))
                        m_ToDamage.Add(m);
                }

                eable.Free();
            }

            protected override void OnTick()
            {
                if (m_From == null || m_From.Deleted || m_Map == null || m_Map == Map.Internal)
                {
                    Stop();
                    return;
                }

                if (0.33 > Utility.RandomDouble())
                {
                    FireField field = new FireField(m_From, 25, Utility.RandomBool());
                    field.MoveToWorld(m_LastTarget, m_Map);
                }

                Point3D start = new Point3D();
                Point3D finish = new Point3D
                {
                    X = m_ShowerArea.X + Utility.Random(m_ShowerArea.Width),
                    Y = m_ShowerArea.Y + Utility.Random(m_ShowerArea.Height),
                    Z = m_From.Z
                };

                SpellHelper.AdjustField(ref finish, m_Map, 16, false);

                //objects move from upper right/right to left as per OSI
                start.X = finish.X + Utility.RandomMinMax(-4, 4);
                start.Y = finish.Y - 15;
                start.Z = finish.Z + 50;

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

        #region Fire Column
        public void DoFireColumn()
        {
            Map map = Map;

            if (map == null)
                return;

            Direction columnDir = Utility.GetDirection(this, Combatant);

            Packet flash = ScreenLightFlash.Instance;
            IPooledEnumerable e = Map.GetClientsInRange(Location, Core.GlobalUpdateRange);

            foreach (NetState ns in e)
            {
                if (ns.Mobile != null)
                    ns.Mobile.Send(flash);
            }

            e.Free();

            int x = X;
            int y = Y;
            bool south = columnDir == Direction.East || columnDir == Direction.West;

            Movement.Movement.Offset(columnDir, ref x, ref y);
            Point3D p = new Point3D(x, y, Z);
            SpellHelper.AdjustField(ref p, map, 16, false);

            FireField fire = new FireField(this, Utility.RandomMinMax(25, 32), south);
            fire.MoveToWorld(p, map);

            for (int i = 0; i < 7; i++)
            {
                Movement.Movement.Offset(columnDir, ref x, ref y);

                p = new Point3D(x, y, Z);
                SpellHelper.AdjustField(ref p, map, 16, false);

                fire = new FireField(this, Utility.RandomMinMax(25, 32), south);
                fire.MoveToWorld(p, map);
            }
        }
        #endregion

        #region Fire Field
        public class FireField : Item
        {
            private readonly Mobile m_Owner;
            private readonly Timer m_Timer;
            private readonly DateTime m_Destroy;

            [Constructable]
            public FireField(Mobile owner, int duration, bool south)
                : base(GetItemID(south))
            {
                Movable = false;
                m_Destroy = DateTime.UtcNow + TimeSpan.FromSeconds(duration);

                m_Owner = owner;
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
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

            private void OnTick()
            {
                if (DateTime.UtcNow > m_Destroy)
                {
                    Delete();
                }
                else
                {
                    IPooledEnumerable eable = GetMobilesInRange(0);
                    List<Mobile> list = new List<Mobile>();

                    foreach (Mobile m in eable)
                    {
                        if (m == null)
                        {
                            continue;
                        }

                        if (m_Owner == null || CanTargetMob(m))
                        {
                            list.Add(m);
                        }
                    }

                    eable.Free();

                    foreach (Mobile mob in list)
                    {
                        DealDamage(mob);
                    }

                    ColUtility.Free(list);
                }
            }

            public override bool OnMoveOver(Mobile m)
            {
                DealDamage(m);

                return true;
            }

            public void DealDamage(Mobile m)
            {
                if (m != m_Owner && (m_Owner == null || CanTargetMob(m)))
                    AOS.Damage(m, m_Owner, Utility.RandomMinMax(2, 4), 0, 100, 0, 0, 0);
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
            private readonly StygianDragon m_Dragon;
            private readonly Mobile m_Combatant;
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

                    DelayCall(TimeSpan.FromSeconds(.20), new TimerStateCallback(DoDamage_Callback), new object[] { m_Combatant, m_Dragon, damage });

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
