using Server;
using System;
using Server.Engines.Quests;
using System.Linq;
using System.Collections.Generic;
using Server.Items;
using Server.Network;
using Server.Movement;
using Server.Spells;
using Server.Spells.SkillMasteries;
using Server.Misc;

namespace Server.Mobiles
{
    public class MyrmidexQueen : BaseCreature
    {
        private DateTime _NextCombo1;
        private DateTime _NextCombo2;
        private DateTime _NextEggThrow;

        private List<BaseCreature> _Spawn;

        public override bool AlwaysMurderer { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Parasitic; } }
        public override Poison HitPoison { get { return Poison.Parasitic; } }
        public override bool Unprovokable { get { return true; } }

        [Constructable]
        public MyrmidexQueen()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.3)
        {
            Body = 1404;
            Name = "Myrmidex Queen";
            BaseSoundID = 959;

            SetHits(60000);
            SetStr(900, 1000);
            SetDex(535);
            SetInt(1000, 1200);

            SetDamage(18, 24);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Poison, 60);

            SetResistance(ResistanceType.Physical, 80, 90);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.Wrestling, 110, 120);
            SetSkill(SkillName.Tactics, 120, 130);
            SetSkill(SkillName.MagicResist, 120, 130);
            SetSkill(SkillName.Anatomy, 0, 10);

            _NextCombo1 = DateTime.UtcNow;
            _NextCombo2 = DateTime.UtcNow;
            _NextEggThrow = DateTime.UtcNow;

            _Spawn = new List<BaseCreature>();

            Fame = 35000;
            Karma = -35000;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 5);
        }	

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null)
                return;

            if (_NextCombo1 < DateTime.UtcNow && 0.1 > Utility.RandomDouble())
            {
                SpitOoze();
                _NextCombo1 = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(15, 30));
            }

            if (_NextCombo2 < DateTime.UtcNow && 0.1 > Utility.RandomDouble())
            {
                if (0.5 > Utility.RandomDouble())
                    DropRocks();
                else
                    RaiseRocks();

                _NextCombo2 = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 40));
            }

            if (_NextEggThrow < DateTime.UtcNow && 0.1 > Utility.RandomDouble())
            {
                ThrowEggs();
                _NextEggThrow = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(90, 120));
            }
        }

        public void ThrowEggs()
        {
            if (this.Map == null)
                return;

            int delay = 0;

            for (byte i = 0; i <= 0x7; i++)
            {
                Direction d = (Direction)i;
                int xOffset = 0;
                int yOffset = 0;

                Movement.Movement.Offset(d, ref xOffset, ref yOffset);

                int x = this.X + (27 * xOffset);
                int y = this.Y + (27 * yOffset);

                Point3D p = new Point3D(x, y, this.Map.GetAverageZ(x, y));

                if (!this.Map.CanFit(p, 16, false, false))
                    continue;

                Timer.DelayCall(TimeSpan.FromSeconds(delay), () =>
                    {
                        Entity e = new Entity(Serial.Zero, p, this.Map);
                        this.MovingParticles(e, 4313, 10, 0, false, true, 1371, 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0);

                        Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(2, 3)), () =>
                            {
                                Type t = Utility.RandomList<Type>(typeof(MyrmidexWarrior), typeof(MyrmidexDrone), typeof(MyrmidexLarvae));
                                BaseCreature bc = Activator.CreateInstance(t) as BaseCreature;

                                if (bc != null)
                                {
                                    bc.MoveToWorld(p, this.Map);
                                    _Spawn.Add(bc);
                                }
                            });

                        delay++;
                    });
            }
        }

        public void SpitOoze()
        {
            if (this.Map == null)
                return;

            IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 7);

            foreach (Mobile m in eable)
            {
                if (m != this && SpellHelper.ValidIndirectTarget(this, m) && this.CanBeHarmful(m, false))
                {
                    List<OozeItem> list = new List<OozeItem>();

                    var ooze1 = new OozeItem(this, 40222);
                    ooze1.MoveToWorld(m.Location, m.Map);

                    var ooze2 = new OozeItem(this, Utility.Random(40214, 2));
                    ooze2.MoveToWorld(new Point3D(m.X - 1, m.Y, m.Z), m.Map);

                    var ooze3 = new OozeItem(this, Utility.Random(40216, 2));
                    ooze3.MoveToWorld(new Point3D(m.X, m.Y + 1, m.Z), m.Map);

                    var ooze4 = new OozeItem(this, Utility.Random(40218, 2));
                    ooze4.MoveToWorld(new Point3D(m.X, m.Y - 1, m.Z), m.Map);

                    var ooze5 = new OozeItem(this, Utility.Random(40220, 2));
                    ooze5.MoveToWorld(new Point3D(m.X + 1, m.Y, m.Z), m.Map);

                    var ooze6 = new OozeItem(this, 40210);
                    ooze6.MoveToWorld(new Point3D(m.X - 1, m.Y + 1, m.Z), m.Map);

                    var ooze7 = new OozeItem(this, 40211);
                    ooze7.MoveToWorld(new Point3D(m.X + 1, m.Y + 1, m.Z), m.Map);

                    var ooze8 = new OozeItem(this, 40212);
                    ooze8.MoveToWorld(new Point3D(m.X - 1, m.Y - 1, m.Z), m.Map);

                    var ooze9 = new OozeItem(this, 40213);
                    ooze9.MoveToWorld(new Point3D(m.X + 1, m.Y - 1, m.Z), m.Map);

                    Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(20, 30)), () =>
                    {
                        ooze1.Delete(); ooze2.Delete(); ooze3.Delete(); ooze4.Delete(); ooze5.Delete(); 
                        ooze6.Delete(); ooze7.Delete(); ooze8.Delete(); ooze9.Delete();
                    });

                    ooze1.OnMoveOver(m);
                }
            }

            eable.Free();
        }

        public void DropRocks()
        {
            if (this.Map == null)
                return;

            IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 12);
            List<Mobile> random = new List<Mobile>();

            foreach (Mobile m in eable)
            {
                if (m.Alive && m is PlayerMobile && SpellHelper.ValidIndirectTarget(this, m) && this.CanBeHarmful(m, false))
                    random.Add(m);
            }

            eable.Free();
            Mobile target = null;
            
            if(random.Count > 0)
                target = random[Utility.Random(random.Count)];

            if (target != null)
            {
                Entity e = new Entity(Serial.Zero, new Point3D(target.X, target.Y, target.Z + 40), target.Map);

                for (int i = 0; i < 5; i++)
                    Timer.DelayCall(TimeSpan.FromMilliseconds(100 * i), () =>
                        {
                            Effects.SendMovingParticles(e, target, 40136, 3, 60, false, true, 0, 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0);
                        });

                Timer.DelayCall(TimeSpan.FromMilliseconds(250), () =>
                    {
                        Effects.SendLocationEffect(target.Location, this.Map, 40136, 120);
                        target.PrivateOverheadMessage(MessageType.Regular, 0x21, 1156835, target.NetState); // *Crunch Crunch Crunch* 
                    });
 
                AOS.Damage(target, this, Utility.RandomMinMax(80, 100), 100, 0, 0, 0, 0);
                target.SendSpeedControl(SpeedControlType.WalkSpeed);

                Timer.DelayCall(TimeSpan.FromSeconds(5), () => target.SendSpeedControl(SpeedControlType.Disable));
            }

            ColUtility.Free(random);
        }

        public void RaiseRocks()
        {
            if (this.Map == null)
                return;

            IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 12);
            List<Mobile> random = new List<Mobile>();

            foreach (Mobile m in eable)
            {
                if (m.Alive && m is PlayerMobile && SpellHelper.ValidIndirectTarget(this, m) && this.CanBeHarmful(m, false))
                    random.Add(m);
            }

            eable.Free();
            Mobile target = null;

            if (random.Count > 0)
                target = random[Utility.Random(random.Count)];

            if (target != null)
            {
                Direction d = Utility.GetDirection(this, target);
                Rectangle2D r = new Rectangle2D(target.X - 8, target.Y - 2, 17, 5);

                switch (d)
                {
                    case Direction.West:
                        r = new Rectangle2D(this.X - 24, this.Y - 2, 20, 5); break;
                    case Direction.North:
                        r = new Rectangle2D(this.X - 2, this.Y - 24, 5, 20); break;
                    case Direction.East:
                        r = new Rectangle2D(this.X + 4, this.Y - 2, 20, 5); break;
                    case Direction.South:
                        r = new Rectangle2D(this.X - 4, this.Y + 4, 20, 5); break;
                }

                for (int x = r.X; x <= r.X + r.Width; x++)
                {
                    for (int y = r.Y; y <= r.Y + r.Height; y++)
                    {
                        if (x > this.X - 4 && x < this.X + 4 && y > this.Y - 4 && y < this.Y + 4)
                            continue;

                        if (0.75 > Utility.RandomDouble())
                        {
                            int id = Utility.RandomList<int>(2282, 2273, 2277, 40106, 40107, 40108, 40106, 40107, 40108, 40106, 40107, 40108);
                            Effects.SendLocationEffect(new Point3D(x, y, this.Map.GetAverageZ(x, y)), this.Map, id, 60);
                        }
                    }
                }

                IPooledEnumerable eable2 = this.Map.GetMobilesInBounds(r);

                foreach (Mobile m in eable2)
                {
                    if (m.Alive && m is PlayerMobile && SpellHelper.ValidIndirectTarget(this, m) && this.CanBeHarmful(m, false))
                    {
                        if (m.X > this.X - 4 && m.X < this.X + 4 && m.Y > this.Y - 4 && m.Y < this.Y + 4)
                            continue;

                        m.Freeze(TimeSpan.FromSeconds(2));
                        BleedAttack.BeginBleed(m, this, false);

                        AOS.Damage(target, this, Utility.RandomMinMax(100, 110), 100, 0, 0, 0, 0);
                        m.PrivateOverheadMessage(MessageType.Regular, 0x21, 1156849, m.NetState); // *Rising columns of rock rip through your flesh and concuss you!*
                    }
                }

                eable2.Free();
            }
        }

        public class OozeItem : Item
        {
            public override int LabelNumber { get { return 1156831; } } // Noxious Goo

            public BaseCreature Owner { get; set; }

            public OozeItem(BaseCreature bc, int id)
                : base(id)
            {
                Owner = bc;
                Movable = false;
                Hue = 2966;
            }

            public override bool OnMoveOver(Mobile m)
            {
                if (m != Owner && SpellHelper.ValidIndirectTarget(Owner, m) && Owner.CanBeHarmful(m, false))
                {
                    if (0.60 > Utility.RandomDouble())
                    {
                        m.PrivateOverheadMessage(MessageType.Regular, 0x21, 1156832, m.NetState); // *The noxious goo has poisoned you!*
                        m.Poison = Poison.Parasitic;
                    }
                    else
                        m.PrivateOverheadMessage(MessageType.Regular, 0x21, 1156830, m.NetState); // *You are drenched in a noxious goo!*

                    AOS.Damage(m, Owner, Utility.RandomMinMax(40, 60), 0, 0, 0, 100, 0);
                }

                return true;
            }

            public OozeItem(Serial serial)
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
                int v = reader.ReadInt();

                Delete();
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new MyrmidexEggsac(Utility.RandomMinMax(5, 10)));
        }

        public override void Delete()
        {
            base.Delete();

            Timer.DelayCall(TimeSpan.FromSeconds(30), () =>
            {
                ColUtility.ForEach(_Spawn.Where(sp => sp != null && sp.Alive), sp => sp.Kill());
                ColUtility.Free(_Spawn);
            });
        }

        public MyrmidexQueen(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);

            writer.Write(_Spawn.Count);
            _Spawn.ForEach(sp => writer.Write(sp));
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();

            _Spawn = new List<BaseCreature>();

            int c = reader.ReadInt();
            for (int i = 0; i < c; i++)
            {
                BaseCreature bc = reader.ReadMobile() as BaseCreature;

                if (bc != null)
                    _Spawn.Add(bc);
            }

            _NextCombo1 = DateTime.UtcNow;
            _NextCombo2 = DateTime.UtcNow;
            _NextEggThrow = DateTime.UtcNow;
		}
    }

    public class Zipactriotl : BaseCreature
    {
        public bool IsQuest { get; set; }

        public override bool AlwaysMurderer { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Parasitic; } }
        public override bool Unprovokable { get { return true; } }

        private DateTime _NextMastery;
        private DateTime _NextSpecial;

        [Constructable]
        public Zipactriotl() : this(false) { }

        public Zipactriotl(bool quest)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.3)
        {
            Body = 1405;
            Name = "Zipactriotl";
            BaseSoundID = 609;

            IsQuest = quest;

            SetHits(60000);
            SetStr(900, 1000);
            SetDex(800, 850);
            SetInt(1400, 1600);

            SetDamage(22, 26);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 80, 90);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.Wrestling, 110, 120);
            SetSkill(SkillName.Tactics, 110, 120);
            SetSkill(SkillName.MagicResist, 120, 130);
            SetSkill(SkillName.Anatomy, 40, 60);

            _NextMastery = DateTime.UtcNow;
            _NextSpecial = DateTime.UtcNow;

            Fame = 35000;
            Karma = -35000;

            SetWeaponAbility(WeaponAbility.Disarm);
            SetWeaponAbility(WeaponAbility.ParalyzingBlow);
        }

        public override bool TeleportsTo { get { return true; } }
        public override TimeSpan TeleportDuration { get { return TimeSpan.FromSeconds(15); } }
        public override double TeleportProb { get { return 0.33; } }

        public override Mobile GetTeleportTarget()
        {
            IPooledEnumerable eable = this.GetMobilesInRange(TeleportRange);
            List<Mobile> list = new List<Mobile>();

            foreach (Mobile m in eable)
            {
                if (m != this && m is PlayerMobile && CanBeHarmful(m) && CanSee(m) && ((PlayerMobile)m).AllFollowers.Where(pet => !(pet is IMount) || ((IMount)pet).Rider == null).Count() > 0)
                {
                    list.Add(m);
                }
            }

            eable.Free();
            Mobile mob = null;

            if (list.Count > 0)
                mob = list[Utility.Random(list.Count)];

            ColUtility.Free(list);
            return mob;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null)
                return;

            if (_NextMastery < DateTime.UtcNow)
            {
                if (SkillMasterySpell.HasSpell(this, typeof(RampageSpell)) || Utility.RandomDouble() > 0.5)
                {
                    SpecialMove.SetCurrentMove(this, SpellRegistry.GetSpecialMove(740));
                }
                else
                {
                    SkillMasterySpell spell = new RampageSpell(this, null);
                    spell.Cast();
                }

                _NextMastery = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(45, 70));
            }

            if (_NextSpecial < DateTime.UtcNow)
            {
                DoSpecial();
                _NextSpecial = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(45, 60));
            }
        }

        public void DoSpecial()
        {
            Map map = this.Map;

            if (map == null || map == Map.Internal)
                return;

            int counter = 0;
            int dist = 4;
            Point3D p = Point3D.Zero;

            IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, dist);

            foreach (Mobile m in eable)
            {
                if (m is PlayerMobile)
                {
                    m.Mana = 0;
                    m.PrivateOverheadMessage(MessageType.Regular, 0x21, 1156856, m.NetState); // *Your mana is converted to pure energy!*
                }
            }
            eable.Free();

            for ( int i = 0; i < _Offsets.Length; i += 2 )
            {
                int tarx = this.X + (int)(_Offsets[i] * dist);
                int tary = this.Y + (int)(_Offsets[i + 1] * dist);
                int tarz = this.Map.GetAverageZ(tarx, tary);

                if (tarx == p.X && tary == p.Y)
                    continue;

                p = new Point3D(tarx, tary, tarz);

                Timer.DelayCall<Point3D>(TimeSpan.FromMilliseconds(350 * counter), (point) =>
                    {
                        //Point3D point = new Point3D(tarx, tary, tarz);

                        Entity e = new Entity(Serial.Zero, point, map);
                        this.MovingParticles(e, 0x3818, 10, 0, false, false, 1150, 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0);

                        Timer.DelayCall<Point3D>(TimeSpan.FromMilliseconds(250), (pnt) =>
                        {
                            Effects.SendLocationEffect(pnt, this.Map, 14089, 30, 1150, 4); // TODO: Check
                        }, point);
                    }, p);

                counter++;
            }

            Timer.DelayCall(TimeSpan.FromMilliseconds(Utility.RandomMinMax(300, 350) * (_Offsets.Length / 2)), () =>
                {
                    eable = this.Map.GetMobilesInRange(this.Location, dist);

                    foreach (Mobile m in eable)
                    {
                        if (m != this && m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile) && CanBeHarmful(m))
                        {
                            Direction d = Utility.GetDirection(this, m);

                            int range = 0;
                            int x = m.X;
                            int y = m.Y;
                            int orx = x;
                            int ory = y;

                            while (range < 15)
                            {
                                range++;
                                int lastx = x;
                                int lasty = y;

                                Movement.Movement.Offset(d, ref x, ref y);
                                int z = map.GetAverageZ(x, y);

                                if (!this.Map.CanFit(new Point3D(x, y, z), 16, false, false))
                                {
                                    m.MoveToWorld(new Point3D(lastx, lasty, this.Map.GetAverageZ(lastx, lasty)), this.Map);
                                    AOS.Damage(m, this, Utility.RandomMinMax(100, 150), 0, 0, 0, 0, 100);
                                    break;
                                }

                                if (range >= 15 && (orx != x || ory != y))
                                {
                                    m.MoveToWorld(new Point3D(x, y, z), this.Map);
                                    AOS.Damage(m, this, Utility.RandomMinMax(100, 150), 0, 0, 0, 0, 100);
                                }
                            }
                        }
                    }
                    eable.Free();
                });
        }

        private static readonly double[] _Offsets = new double[]
			{
                Math.Cos( 300.0 / 180.0 * Math.PI ), Math.Sin( 300.0 / 180.0 * Math.PI ),
				Math.Cos( 320.0 / 180.0 * Math.PI ), Math.Sin( 320.0 / 180.0 * Math.PI ),
                Math.Cos( 340.0 / 180.0 * Math.PI ), Math.Sin( 340.0 / 180.0 * Math.PI ),
                Math.Cos( 000.0 / 180.0 * Math.PI ), Math.Sin( 000.0 / 180.0 * Math.PI ),
                Math.Cos( 020.0 / 180.0 * Math.PI ), Math.Sin( 020.0 / 180.0 * Math.PI ),
				Math.Cos( 040.0 / 180.0 * Math.PI ), Math.Sin( 040.0 / 180.0 * Math.PI ),
                Math.Cos( 060.0 / 180.0 * Math.PI ), Math.Sin( 060.0 / 180.0 * Math.PI ),
				Math.Cos( 080.0 / 180.0 * Math.PI ), Math.Sin( 080.0 / 180.0 * Math.PI ),
                Math.Cos( 100.0 / 180.0 * Math.PI ), Math.Sin( 100.0 / 180.0 * Math.PI ),
				Math.Cos( 120.0 / 180.0 * Math.PI ), Math.Sin( 120.0 / 180.0 * Math.PI ),
				Math.Cos( 140.0 / 180.0 * Math.PI ), Math.Sin( 140.0 / 180.0 * Math.PI ),
                Math.Cos( 160.0 / 180.0 * Math.PI ), Math.Sin( 160.0 / 180.0 * Math.PI ),
				Math.Cos( 180.0 / 180.0 * Math.PI ), Math.Sin( 180.0 / 180.0 * Math.PI ),
                Math.Cos( 200.0 / 180.0 * Math.PI ), Math.Sin( 200.0 / 180.0 * Math.PI ),
                Math.Cos( 220.0 / 180.0 * Math.PI ), Math.Sin( 220.0 / 180.0 * Math.PI ),
				Math.Cos( 240.0 / 180.0 * Math.PI ), Math.Sin( 240.0 / 180.0 * Math.PI ),
                Math.Cos( 260.0 / 180.0 * Math.PI ), Math.Sin( 260.0 / 180.0 * Math.PI ),
				Math.Cos( 280.0 / 180.0 * Math.PI ), Math.Sin( 280.0 / 180.0 * Math.PI ),

                Math.Cos( 260.0 / 180.0 * Math.PI ), Math.Sin( 260.0 / 180.0 * Math.PI ),
                Math.Cos( 240.0 / 180.0 * Math.PI ), Math.Sin( 240.0 / 180.0 * Math.PI ),
                Math.Cos( 220.0 / 180.0 * Math.PI ), Math.Sin( 220.0 / 180.0 * Math.PI ),
                Math.Cos( 200.0 / 180.0 * Math.PI ), Math.Sin( 200.0 / 180.0 * Math.PI ),
                Math.Cos( 180.0 / 180.0 * Math.PI ), Math.Sin( 180.0 / 180.0 * Math.PI ),
                Math.Cos( 160.0 / 180.0 * Math.PI ), Math.Sin( 160.0 / 180.0 * Math.PI ),
                Math.Cos( 140.0 / 180.0 * Math.PI ), Math.Sin( 140.0 / 180.0 * Math.PI ),
                Math.Cos( 120.0 / 180.0 * Math.PI ), Math.Sin( 120.0 / 180.0 * Math.PI ),
                Math.Cos( 100.0 / 180.0 * Math.PI ), Math.Sin( 100.0 / 180.0 * Math.PI ),
                Math.Cos( 080.0 / 180.0 * Math.PI ), Math.Sin( 080.0 / 180.0 * Math.PI ),
                Math.Cos( 060.0 / 180.0 * Math.PI ), Math.Sin( 060.0 / 180.0 * Math.PI ),
                Math.Cos( 040.0 / 180.0 * Math.PI ), Math.Sin( 040.0 / 180.0 * Math.PI ),
                Math.Cos( 020.0 / 180.0 * Math.PI ), Math.Sin( 020.0 / 180.0 * Math.PI ),
                Math.Cos( 000.0 / 180.0 * Math.PI ), Math.Sin( 000.0 / 180.0 * Math.PI ),
                Math.Cos( 340.0 / 180.0 * Math.PI ), Math.Sin( 340.0 / 180.0 * Math.PI ),
                Math.Cos( 320.0 / 180.0 * Math.PI ), Math.Sin( 320.0 / 180.0 * Math.PI ),
                Math.Cos( 300.0 / 180.0 * Math.PI ), Math.Sin( 300.0 / 180.0 * Math.PI ),
			};

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 5);
        }	

        public override void Delete()
        {
            base.Delete();

            if (IsQuest)
                Timer.DelayCall(TimeSpan.FromMinutes(30), () => MoonstonePowerGeneratorAddon.ResetGenerators());
        }

        public Zipactriotl(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(IsQuest);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            _NextMastery = DateTime.UtcNow;
            _NextSpecial = DateTime.UtcNow;

            IsQuest = reader.ReadBool();
        }
    }

    public class IgnisFatalis : BaseCreature
    {
        public override bool AlwaysMurderer { get { return true; } }

        [Constructable]
        public IgnisFatalis()
            : base(AIType.AI_Spellweaving, FightMode.Closest, 10, 1, 0.15, 0.3)
        {
            Body = 0x105;
            Name = "Ignis Fatalis";
            BaseSoundID = 0x56B;

            SetHits(500);
            SetStr(350, 360);
            SetDex(100, 150);
            SetInt(580, 620);

            SetDamage(15, 22);

            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Energy, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 100);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);
            SetSkill(SkillName.MagicResist, 100);
            SetSkill(SkillName.DetectHidden, 100.0);
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Meditation, 100.0);
            SetSkill(SkillName.Focus, 100.0);
            SetSkill(SkillName.Spellweaving, 100.0);
        }

        public override bool HasAura { get { return true; } }
        public override int AuraEnergyDamage { get { return 100; } }

        public IgnisFatalis(Serial serial)
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
            int v = reader.ReadInt();
        }
    }
}