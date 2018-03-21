using System;
using Server;
using Server.Items;
using System.Collections.Generic;
using Server.Engines.Quests;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("a tyrannosaurus rex corpse")]
    public class TRex : BaseCreature
    {
        private DateTime _NextFreeze;

        [Constructable]
        public TRex()
            : base(AIType.AI_Melee, FightMode.Weakest, 10, 1, .08, .17)
        {
            Name = "Tyrannosaurus Rex";
            Body = 1400;
            BaseSoundID = 362;

            SetStr(500, 700);
            SetDex(500, 700);
            SetInt(100, 180);

            SetHits(15000);

            SetDamage(33, 55);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 80, 90);
            SetResistance(ResistanceType.Fire, 60, 80);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 80, 100);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.Anatomy, 100.0);
            SetSkill(SkillName.MagicResist, 140.0, 150.0);
            SetSkill(SkillName.Tactics, 110.0, 130.0);
            SetSkill(SkillName.Wrestling, 130.0, 150.0);
            SetSkill(SkillName.Poisoning, 60.0, 70.0);
            SetSkill(SkillName.Parry, 100);

            Fame = 24000;
            Karma = -24000;

            _NextFreeze = DateTime.UtcNow;

            CanSwim = true;
            SetWeaponAbility(WeaponAbility.ArmorIgnore);
            SetSpecialAbility(SpecialAbility.TailSwipe);
        }

        public override bool AutoDispel { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool UseSmartAI { get { return true; } }
        public override bool ReacquireOnMovement { get { return true; } }
        public override bool AttacksFocus { get { return true; } }

        // Missing Tail Swipe Ability

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null)
                return;

            if (_NextFreeze < DateTime.UtcNow)
                DoFreeze();
        }

        public void DoFreeze()
        {
            DoEffects(Direction.North);
            DoEffects(Direction.West);
            DoEffects(Direction.South);
            DoEffects(Direction.East);

            _NextFreeze = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(45, 60));
        }

        private bool DoEffects(Direction d)
        {
            int x = X;
            int y = Y;
            int z = Z;
            int range = 10;
            int offset = 8;

            switch (d)
            {
                case Direction.North:
                    x = X + Utility.RandomMinMax(-offset, offset);
                    y = Y - range;
                    break;
                case Direction.West:
                    x = X - range;
                    y = Y + Utility.RandomMinMax(-offset, offset);
                    break;
                case Direction.South:
                    x = X + Utility.RandomMinMax(-offset, offset);
                    y = Y + range;
                    break;
                case Direction.East:
                    x = X + range;
                    y = Y + Utility.RandomMinMax(-offset, offset);
                    break;
            }

            for (int i = 0; i < range; i++)
            {
                switch (d)
                {
                    case Direction.North: y += i; break;
                    case Direction.West: x += i; break;
                    case Direction.South: y -= i; break;
                    case Direction.East: x -= i; break;
                }

                z = Map.GetAverageZ(x, y);
                Point3D p = new Point3D(x, y, z);

                if (Server.Spells.SpellHelper.AdjustField(ref p, Map, 12, false))/*Map.CanFit(x, y, z, 16, false, false, true))/*Map.CanSpawnMobile(x, y, z)*/
                {
                    MovementPath path = new MovementPath(this, p);

                    if (path.Success)
                    {
                        DropCrack(path);
                        return true;
                    }
                }
            }

            return false;
        }

        private void DropCrack(MovementPath path)
        {
            int time = 10;
            int x = X;
            int y = Y;

            for (int i = 0; i < path.Directions.Length; ++i)
            {
                Movement.Movement.Offset(path.Directions[i], ref x, ref y);
                IPoint3D p = new Point3D(x, y, Map.GetAverageZ(x, y)) as IPoint3D;

                Timer.DelayCall(TimeSpan.FromMilliseconds(time), new TimerStateCallback(ManaDrainEffects_Callback), new object[] { p, Map });

                time += 200;
            }
        }

        private void ManaDrainEffects_Callback(object o)
        {
            object[] objs = o as object[];
            IPoint3D p = objs[0] as IPoint3D;
            Map map = objs[1] as Map;

            var item = new FreezeItem(Utility.RandomList(6913, 6915, 6917, 6919), this);
            Spells.SpellHelper.GetSurfaceTop(ref p);

            item.MoveToWorld(new Point3D(p), Map);
        }

        private class FreezeItem : Item
        {
            public Item Static { get; private set; }
            public BaseCreature Owner { get; private set; }

            public FreezeItem(int id, BaseCreature owner)
                : base(id)
            {
                Owner = owner;

                Movable = false;
                Hue = 1152;
                Timer.DelayCall(TimeSpan.FromSeconds(5), ChangeHue);
            }

            private void ChangeHue()
            {
                Hue = 1153;
                Static.Hue = 1153;

                Timer.DelayCall(TimeSpan.FromSeconds(0.5), Delete);
            }

            public override void Delete()
            {
                if (Static != null)
                    Static.Delete();

                Static = null;
                base.Delete();
            }

            public override void OnLocationChange(Point3D oldLocation)
            {
                Static = new Static(ItemID + 1);
                Static.MoveToWorld(Location, Map);

                IPooledEnumerable eable = Map.GetMobilesInRange(Location, 0);

                foreach (Mobile m in eable)
                {
                    OnMoveOver(m);
                }
                eable.Free();
            }

            public override bool OnMoveOver(Mobile m)
            {
                if ((m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)) && m.CanBeHarmful(Owner, false))
                {
                    m.Freeze(TimeSpan.FromSeconds(Utility.RandomMinMax(5, 10)));

                    if (Owner.CanBeHarmful(m))
                    {
                        m.AggressiveAction(Owner);
                        Owner.InitialFocus = m;
                        Owner.Combatant = m;
                    }
                }

                return true;
            }

            public FreezeItem(Serial serial)
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

                Delete();
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 2);
        }

        public TRex(Serial serial)
            : base(serial)
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

    [CorpseName("the corpse of a great ape")]
    public class GreatApe : BaseCreature
    {
        private DateTime _NextSpecial;
        private DateTime _NextBarrelThrow;

        private int _LastTeleport;

        private Point3D[] _TeleList =
        {
            new Point3D(874, 1439, 0),
            new Point3D(847, 1425, 0),
            new Point3D(847, 1389, 0),
            new Point3D(866, 1366, 0),
            new Point3D(887, 1390, 0),
            new Point3D(889, 1407, 0),
            new Point3D(882, 1426, 0),
            new Point3D(858, 1412, 21),
            new Point3D(868, 1412, 20),
            new Point3D(869, 1409, 20),
            new Point3D(869, 1404, 20),
            new Point3D(858, 1412, 21),
            new Point3D(868, 1412, 20),
            new Point3D(869, 1409, 20),
            new Point3D(869, 1404, 20),
        };

        private Point3D[] _PlayerTeleList =
        {
            new Point3D(875, 1380, -20),
            new Point3D(855, 1442, -20)
        };

        private int[] _BarrelIDs =
        {
            3703,   4014,   5453,   7861,
            17650
        };

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Teleports { get; set; }

        [Constructable]
        public GreatApe(bool teleports = false)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .08, .17)
        {
            Teleports = teleports;
            Name = "a great ape";
            Body = 1308;
            BaseSoundID = 0x9E;

            SetStr(986, 1185);
            SetDex(177, 255);
            SetInt(151, 250);

            SetHits(12500);

            SetDamage(20, 33);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 80);
            SetResistance(ResistanceType.Fire, 60, 80);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Anatomy, 25.1, 50.0);
            SetSkill(SkillName.MagicResist, 150.0);
            SetSkill(SkillName.Tactics, 100);
            SetSkill(SkillName.Wrestling, 100);

            Fame = 35000;
            Karma = -35000;

            _NextSpecial = DateTime.UtcNow;
            _NextBarrelThrow = DateTime.UtcNow;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 2);
        }

        public override bool AutoDispel { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool UseSmartAI { get { return true; } }

        public GreatApe(Serial serial)
            : base(serial)
        {
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null)
                return;

            if (_NextSpecial < DateTime.UtcNow)
            {
                _NextSpecial = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(45, 60));

                switch(Utility.Random(3))
                {
                    case 0:
                        IPooledEnumerable eable = Map.GetMobilesInRange(Location, 10);

                        foreach (Mobile m in eable)
                        {
                            if (m.Alive && m.AccessLevel == AccessLevel.Player && m is PlayerMobile && .75 > Utility.RandomDouble())
                                DoDismount(m);
                        }

                        eable.Free();
                        break;
                    case 1:
                        int ran = -1;

                        while (ran < 0 || ran > _TeleList.Length || ran == _LastTeleport)
                        {
                            ran = Utility.Random(_TeleList.Length);
                        }

                        _LastTeleport = ran;
                        Point3D p = _TeleList[ran];
                        Point3D old = Location;

                        MoveToWorld(p, Map);
                        ProcessDelta();

                        Effects.SendLocationParticles(EffectItem.Create(old, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                        Effects.SendLocationParticles(EffectItem.Create(p, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
                        break;
                    case 2:
                        IPooledEnumerable eable2 = Map.GetMobilesInRange(Location, 10);
                        List<Mobile> mobiles = new List<Mobile>();

                        foreach (Mobile m in eable2)
                        {
                            if (m.Alive && m.AccessLevel == AccessLevel.Player && m is PlayerMobile)
                                mobiles.Add(m);
                        }

                        eable2.Free();

                        if (mobiles.Count > 0)
                        {
                            Mobile m = mobiles[Utility.Random(mobiles.Count)];
                            Point3D old2 = m.Location;
                            Point3D p2 = _PlayerTeleList[Utility.Random(_PlayerTeleList.Length)];

                            m.MoveToWorld(p2, Map);
                            m.ProcessDelta();

                            Effects.SendLocationParticles(EffectItem.Create(old2, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                            Effects.SendLocationParticles(EffectItem.Create(p2, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
                        }

                        ColUtility.Free(mobiles);
                        break;
                }
            }
            else if (_NextBarrelThrow < DateTime.UtcNow && .25 > Utility.RandomDouble())
            {
                _NextBarrelThrow = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(5, 10));
                int barrel = CheckBarrel();

                if (barrel >= 0)
                {
                    IPooledEnumerable eable = Map.GetMobilesInRange(Location, 10);
                    List<Mobile> mobiles = new List<Mobile>();

                    foreach (Mobile m in eable)
                    {
                        if (m.Alive && m.AccessLevel == AccessLevel.Player && m is PlayerMobile)
                            mobiles.Add(m);
                    }

                    eable.Free();

                    if (mobiles.Count > 0)
                    {
                        Mobile m = mobiles[Utility.Random(mobiles.Count)];
                        DoHarmful(m);

                        MovingParticles(m, barrel, 10, 0, false, true, 0, 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0);

                        Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                        {
                            m.PlaySound(0x11D);
                            AOS.Damage(m, this, Utility.RandomMinMax(70, 120), 100, 0, 0, 0, 0);
                        });
                    }

                    ColUtility.Free(mobiles);
                }
            }
        }

        public void DoDismount(Mobile m)
        {
            MovingParticles(m, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
            PlaySound(0x15E);

            double range = m.GetDistanceToSqrt(this);

            Timer.DelayCall(TimeSpan.FromMilliseconds(250 * range), () =>
            {
                IMount mount = m.Mount;

                if (mount != null)
                {
                    if (m is PlayerMobile)
                        ((PlayerMobile)m).SetMountBlock(BlockMountType.Dazed, TimeSpan.FromSeconds(10), true);
                    else
                        mount.Rider = null;
                }
                else if (m.Flying)
                {
                    ((PlayerMobile)m).SetMountBlock(BlockMountType.Dazed, TimeSpan.FromSeconds(10), true);
                }

                PlaySound(0x220);
                AOS.Damage(m, this, Utility.RandomMinMax(15, 25), 100, 0, 0, 0, 0);
                m.SendLocalizedMessage(1156495); // *The shaking ground damages and disorients you!*
            });
        }

        public int CheckBarrel()
        {
            IPooledEnumerable eable = Map.GetItemsInRange(Location, 2);
            List<Item> items = new List<Item>();

            foreach (Item item in eable)
            {
                if (!item.Movable && IsInList(item.ItemID))
                    items.Add(item);
            }

            eable.Free();
            int itemid;

            if (items.Count > 0)
                itemid = items[Utility.Random(items.Count)].ItemID;
            else
                itemid = -1;

            ColUtility.Free(items);
            return itemid;
        }

        private bool IsInList(int itemid)
        {
            foreach (int i in _BarrelIDs)
            {
                if (i == itemid)
                    return true;
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write(Teleports);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            Teleports = reader.ReadBool();

            _NextSpecial = DateTime.UtcNow;
            _NextBarrelThrow = DateTime.UtcNow;
        }
    }

    [CorpseName("the corpse of a tiger cub")]
    public class TigerCub : BaseCreature
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Protector { get; set; }

        [Constructable]
        public TigerCub()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a tiger cub";
            Body = 1309;
            BaseSoundID = 0x69;

            SetStr(100);
            SetDex(100);
            SetInt(20);

            SetHits(50, 60);
            SetMana(0);

            SetDamage(10, 12);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 5, 10);

            SetSkill(SkillName.MagicResist, 5.0);
            SetSkill(SkillName.Tactics, 40.0);
            SetSkill(SkillName.Wrestling, 50.0);

            Fame = 0;
            Karma = 500;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Protector != null && Protector is PlayerMobile && InRange(Home, 2))
            {
                PrideOfTheAmbushQuest quest = QuestHelper.GetQuest((PlayerMobile)Protector, typeof(PrideOfTheAmbushQuest)) as PrideOfTheAmbushQuest;

                if (quest != null && !quest.Completed)
                    quest.Update(this);

                Protector.PrivateOverheadMessage(Server.Network.MessageType.Regular, 0x35, 1156501, Protector.NetState); // *You watch as the Tiger Cub safely returns to the Kurak Tribe*
                
                Timer.DelayCall(TimeSpan.FromSeconds(.25), Delete);
                Protector = null;
            }
        }

        public override void Damage(int amount, Mobile from, bool informMount, bool checkfizzle)
        {
            if(from is BaseCreature)
                from = ((BaseCreature)from).GetMaster();

            if (from is PlayerMobile)
            {
                PrivateOverheadMessage(Server.Network.MessageType.Regular, 0x35, 1156500, from.NetState); // *The cub looks at you playfully. Your attack fails as you are overwhelmed by its cuteness*
                return;
            }

            base.Damage(amount, from, informMount, checkfizzle);
        }

        public override int Meat { get { return 1; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat | FoodType.Fish; } }
        public override PackInstinct PackInstinct { get { return PackInstinct.Feline; } }

        public TigerCub(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write(Protector);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            Protector = reader.ReadMobile();
        }
    }

    public class Trapper : Brigand
    {
        [Constructable]
        public Trapper()
        {
            Title = "the trapper";

            SetHits(2500);

            SetStr(125, 150);
            SetDex(200);
            SetInt(61, 75);

            SetDamage(20, 32);

            SetSkill(SkillName.Fencing, 100);
            SetSkill(SkillName.Macing, 100);
            SetSkill(SkillName.MagicResist, 120);
            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Tactics, 100);
            SetSkill(SkillName.Wrestling, 100);

            Fame = 12000;
            Karma = -12000;

            Timer.DelayCall(TimeSpan.FromMinutes(10), Delete);
            
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 3);
        }

        public Trapper(Serial serial)
            : base(serial)
        {
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

            Delete();
        }
    }

    public class Poacher : Trapper
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public NestWithEgg Nest { get; set; }

        [Constructable]
        public Poacher(NestWithEgg nest)
        {
            Title = "the poacher";
            Nest = nest;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 3);
        }

        public Poacher(Serial serial)
            : base(serial)
        {
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Nest != null && !Nest.Deleted)
                Nest.OnPoacherKilled(this);
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
        }
    }

    [CorpseName("a lava elemental corpse")]
    public class VolcanoElemental : BaseCreature
    {
        [Constructable]
        public VolcanoElemental()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a volcano elemental";
            Body = 15; 
            Hue = 2726;

            SetStr(446, 510);
            SetDex(173, 191);
            SetInt(369, 397);

            SetHits(800, 1200);

            SetDamage(18, 24);

            SetDamageType(ResistanceType.Physical, 10);
            SetDamageType(ResistanceType.Fire, 90);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Anatomy, 0.0, 12.8);
            SetSkill(SkillName.EvalInt, 84.8, 92.6);
            SetSkill(SkillName.Magery, 90.1, 92.7);
            SetSkill(SkillName.Meditation, 97.8, 102.8);
            SetSkill(SkillName.MagicResist, 101.9, 106.2);
            SetSkill(SkillName.Tactics, 80.3, 94.0);
            SetSkill(SkillName.Wrestling, 71.7, 85.4);

            Fame = 12500;
            Karma = -12500;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.Gems, 2);
            AddLoot(LootPack.MedScrolls);
        }

        public VolcanoElemental(Serial serial)
            : base(serial)
        {
        }

        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }

        public override int GetIdleSound()
        {
            return 1549;
        }

        public override int GetAngerSound()
        {
            return 1546;
        }

        public override int GetHurtSound()
        {
            return 1548;
        }

        public override int GetDeathSound()
        {
            return 1547;
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
        }
    }
}
