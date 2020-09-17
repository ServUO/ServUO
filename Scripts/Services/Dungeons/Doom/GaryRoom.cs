using Server.ContextMenus;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.Doom
{
    [PropertyObject]
    public class GaryRegion : BaseRegion
    {
        public static void Initialize()
        {
            new GaryRegion();
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public GaryTheDungeonMaster Gary { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Sapphired20 Dice { get; set; }

        public DisplayStatue[] Statues { get; set; }

        public Timer Timer { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseDoor DoorOne { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseDoor DoorTwo { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextRoll { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseCreature Spawn { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ForceRoll { get; set; }

        private static Point3D _GaryLoc = new Point3D(389, 8, 0);
        private static Point3D _DiceLoc = new Point3D(390, 8, 5);
        private static Point3D _RulesLoc = new Point3D(390, 9, 6);
        private static Point3D _SpawnLoc = new Point3D(396, 8, 4);
        private static Point3D _DoorOneLoc = new Point3D(395, 15, -1);
        private static Point3D _DoorTwoLoc = new Point3D(396, 15, -1);
        private static readonly Point3D[] _StatueLocs = new Point3D[]
        {
            new Point3D(393, 4, 5),
            new Point3D(395, 4 ,5),
            new Point3D(397, 4, 5)
        };
        private static readonly Rectangle2D[] _Bounds =
        {
            new Rectangle2D(388, 3, 16, 12)
        };

        private readonly Type[] _MonsterList =
        {
            typeof(BoneDemon), typeof(SkeletalKnight), typeof(SkeletalMage), typeof(DarkGuardian), typeof(Devourer),
            typeof(FleshGolem), typeof(Gibberling), typeof(AncientLich), typeof(Lich), typeof(LichLord),
            typeof(Mummy), typeof(PatchworkSkeleton), typeof(Ravager), typeof(RestlessSoul), typeof(Dragon),
            typeof(SkeletalDragon), typeof(VampireBat), typeof(WailingBanshee), typeof(WandererOfTheVoid)
        };

        public TimeSpan RollDelay => TimeSpan.FromMinutes(Utility.RandomMinMax(12, 15));

        public GaryRegion()
            : base("Gary Region", Map.Malas, Find(_GaryLoc, Map.Malas), _Bounds)
        {
            Register();
            CheckStuff();
        }

        public override void OnRegister()
        {
            NextRoll = DateTime.UtcNow;
            Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), OnTick);

            ForceRoll = -1;
        }

        public override void OnUnregister()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }
        }

        public void OnTick()
        {
            if (NextRoll < DateTime.UtcNow /*&& (Spawn == null || !Spawn.Alive)*/ && GetEnumeratedMobiles().OfType<PlayerMobile>().Where(p => p.Alive).Count() > 0)
            {
                DoRoll();
                NextRoll = DateTime.UtcNow + RollDelay;
            }
        }

        public void DoRoll()
        {
            GaryTheDungeonMaster g = GetGary();
            Sapphired20 d = GetDice();
            int roll = ForceRoll >= 0 && ForceRoll < 20 ? ForceRoll : Utility.Random(20);

            g.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1080099); // *Gary rolls the sapphire d20*

            BaseDoor door1 = GetDoor1();
            BaseDoor door2 = GetDoor2();

            door1.Locked = true;
            door2.Locked = true;

            Timer.DelayCall(TimeSpan.FromMinutes(2), () =>
                {
                    door1.Locked = false;
                    door2.Locked = false;
                });

            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                {
                    if (d != null)
                    {
                        d.Roll(roll);
                    }
                    else
                    {
                        foreach (PlayerMobile m in GetEnumeratedMobiles().OfType<PlayerMobile>())
                        {
                            m.SendMessage("- {0} -", (roll + 1).ToString());
                        }
                    }
                });

            Timer.DelayCall(TimeSpan.FromSeconds(2), () =>
                {
                    if (roll == 19)
                    {
                        DoStaffSpawn();
                    }
                    else
                    {
                        Spawn = Activator.CreateInstance(_MonsterList[roll]) as BaseCreature;
                        Spawn.Kills = 100;

                        if (Spawn is Dragon)
                        {
                            Spawn.Body = 155;
                            Spawn.CorpseNameOverride = "a rotting corpse";
                        }

                        Spawn.MoveToWorld(_SpawnLoc, Map.Malas);
                        Spawn.Home = _SpawnLoc;
                        Spawn.RangeHome = 7;
                    }

                    ChangeStatues();
                });

            ForceRoll = -1;
        }

        public void ChangeStatues()
        {
            foreach (DisplayStatue statue in GetStatues())
            {
                statue.AssignRandom();
            }
        }

        public void DoStaffSpawn()
        {
            GameMaster gm1 = new GameMaster();
            GameMaster gm2 = new GameMaster();

            Point3D p = _SpawnLoc;

            gm1.MoveToWorld(new Point3D(p.X + 1, p.Y - 1, p.Z), Map.Malas);
            gm2.MoveToWorld(new Point3D(p.X + 1, p.Y + 1, p.Z), Map.Malas);

            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                {
                    gm1.Say(1080100); // What the heck?

                    Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                        {
                            gm2.Say(1080101); // Did we just get summoned?

                            Timer.DelayCall(TimeSpan.FromSeconds(2), () =>
                                {
                                    gm1.Say(1080102); // Wow. We did!

                                    Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                                        {
                                            gm2.Say(1080103); // What're the odds of that!?

                                            Timer.DelayCall(TimeSpan.FromSeconds(2), () =>
                                                {
                                                    gm1.Say(1080104); // ...about 1 in 20?

                                                    Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                                                        {
                                                            gm2.Say(1080105); // *looks at the d20*

                                                            Timer.DelayCall(TimeSpan.FromSeconds(2), () =>
                                                                {
                                                                    gm1.Say(1080106);// Ah, right.

                                                                    Timer.DelayCall(TimeSpan.FromMinutes(2), () =>
                                                                        {
                                                                            DeleteStaff(gm1, gm2);
                                                                        });
                                                                });
                                                        });
                                                });
                                        });
                                });
                        });
                });
        }

        private void DeleteStaff(BaseCreature one, BaseCreature two)
        {
            GMHidingStone.SendStoneEffects((StoneEffect)Utility.Random(19), 0, one);
            GMHidingStone.SendStoneEffects((StoneEffect)Utility.Random(19), 0, two);

            one.Delete();
            two.Delete();
        }

        public override void OnDeath(Mobile m)
        {
            if (m == Spawn)
            {
                BaseDoor door1 = GetDoor1();
                BaseDoor door2 = GetDoor2();

                door1.Locked = false;
                door2.Locked = false;

                Spawn = null;
            }

            base.OnDeath(m);
        }

        public override void OnEnter(Mobile m)
        {
            GaryTheDungeonMaster g = GetGary();

            g.SayTo(m, 1080098); // Ah... visitors!
        }

        public override bool CheckTravel(Mobile traveller, Point3D p, Spells.TravelCheckType type)
        {
            switch (type)
            {
                case Spells.TravelCheckType.Mark:
                case Spells.TravelCheckType.RecallTo:
                case Spells.TravelCheckType.RecallFrom:
                case Spells.TravelCheckType.GateTo:
                case Spells.TravelCheckType.GateFrom:
                    return false;
            }

            return base.CheckTravel(traveller, p, type);
        }

        private GaryTheDungeonMaster GetGary()
        {
            if (Gary == null || Gary.Deleted)
            {
                GaryTheDungeonMaster gary = null;
                IPooledEnumerable eable = Map.GetMobilesInBounds(_Bounds[0]);

                foreach (Mobile m in eable)
                {
                    if (m is GaryTheDungeonMaster)
                    {
                        gary = (GaryTheDungeonMaster)m;
                        break;
                    }
                }

                eable.Free();

                if (gary != null)
                {
                    Gary = gary;
                    Gary.MoveToWorld(_GaryLoc, Map.Malas);
                }
                else
                {
                    Gary = new GaryTheDungeonMaster();
                    Gary.MoveToWorld(_GaryLoc, Map.Malas);
                }
            }

            return Gary;
        }

        private Sapphired20 GetDice()
        {
            if (Dice == null || Dice.Deleted)
            {
                Sapphired20 dice = GetEnumeratedItems().OfType<Sapphired20>().FirstOrDefault(i => !i.Deleted);

                if (dice != null)
                {
                    Dice = dice;
                    Dice.MoveToWorld(_DiceLoc, Map.Malas);
                }
                else
                {
                    Dice = new Sapphired20
                    {
                        Movable = false
                    };
                    Dice.MoveToWorld(_DiceLoc, Map.Malas);
                }
            }

            return Dice;
        }

        private DisplayStatue[] GetStatues()
        {
            if (Statues == null || Statues.Length != 3)
            {
                Statues = new DisplayStatue[3];
            }

            for (int i = 0; i < 3; i++)
            {
                if (Statues[i] == null || Statues[i].Deleted)
                {
                    DisplayStatue s = GetEnumeratedItems().OfType<DisplayStatue>().FirstOrDefault(st => Array.IndexOf(Statues, st) == -1);

                    if (s == null)
                    {
                        Statues[i] = new DisplayStatue
                        {
                            Movable = false
                        };
                        Statues[i].MoveToWorld(_StatueLocs[i], Map.Malas);
                    }
                    else
                    {
                        Statues[i] = s;
                        Statues[i].MoveToWorld(_StatueLocs[i], Map.Malas);
                    }
                }
            }

            return Statues;
        }

        private BaseDoor GetDoor1()
        {
            if (DoorOne == null || DoorOne.Deleted)
            {
                //BaseDoor door = this.GetEnumeratedItems().OfType<DarkWoodDoor>().FirstOrDefault(d => d.Location == _DoorOneLoc);
                Point3D p = _DoorOneLoc;
                BaseDoor door = GetDoor(p);

                if (door == null)
                {
                    door = GetDoor(new Point3D(p.X - 1, p.Y + 1, p.Z));
                }

                if (door == null)
                {
                    DoorOne = new DarkWoodDoor(DoorFacing.WestCW);
                    DoorOne.MoveToWorld(_DoorOneLoc, Map.Malas);
                }
                else
                {
                    DoorOne = door;
                }

                DoorOne.Locked = false;
            }

            return DoorOne;
        }

        private BaseDoor GetDoor2()
        {
            if (DoorTwo == null || DoorTwo.Deleted)
            {
                //BaseDoor door = this.GetEnumeratedItems().OfType<DarkWoodDoor>().FirstOrDefault(d => d.Location == _DoorOneLoc);
                Point3D p = _DoorTwoLoc;
                BaseDoor door = GetDoor(p);

                if (door == null)
                {
                    door = GetDoor(new Point3D(p.X + 1, p.Y + 1, p.Z));
                }

                if (door == null)
                {
                    DoorTwo = new DarkWoodDoor(DoorFacing.EastCCW);
                    DoorTwo.MoveToWorld(_DoorTwoLoc, Map.Malas);
                }
                else
                {
                    DoorTwo = door;
                }

                DoorTwo.Locked = false;
            }

            return DoorTwo;
        }

        private void CheckStuff()
        {
            GetGary();
            GetStatues();
            GetDice();
            GetDoor1();
            GetDoor2();

            if (!FindObject(typeof(UOBoard), _RulesLoc))
            {
                UOBoard rules = new UOBoard
                {
                    Movable = false
                };
                rules.MoveToWorld(_RulesLoc, Map.Malas);
            }

            Point3D p = new Point3D(390, 7, 2);

            if (!FindObject(typeof(Static), p))
            {
                Static books = new Static(0x1E22);
                books.MoveToWorld(p, Map.Malas);
            }

            if (!FindObject(typeof(ScribesPen), p))
            {
                ScribesPen pen = new ScribesPen
                {
                    ItemID = 4032,
                    Movable = false
                };
                pen.MoveToWorld(p, Map.Malas);
            }
        }

        private bool FindObject(Type t, Point3D p)
        {
            IPooledEnumerable eable = Map.GetObjectsInRange(p, 0);

            foreach (object o in eable)
            {
                if (o.GetType() == t)
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }

        private BaseDoor GetDoor(Point3D p)
        {
            IPooledEnumerable eable = Map.GetItemsInRange(p, 0);

            foreach (Item item in eable)
            {
                if (item is BaseDoor)
                {
                    eable.Free();
                    return (BaseDoor)item;
                }
            }

            eable.Free();
            return null;
        }
    }

    public class Sapphired20 : Item
    {
        public override int LabelNumber => 1080096;  // Star Sapphire d20

        [Constructable]
        public Sapphired20()
            : base(0x3192)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (GetRegion().IsPartOf<GaryRegion>())
            {
                m.SendLocalizedMessage(1080097); // You're blasted back in a blaze of light! This d20 is not yours to roll...

                m.Damage(Utility.RandomMinMax(20, 30));
            }
            else
            {
                Roll(Utility.Random(20));
            }
        }

        public void Roll(int roll)
        {
            PublicOverheadMessage(MessageType.Regular, 0x3B2, false, string.Format("- {0} -", (roll + 1).ToString()));
        }

        public Sapphired20(Serial serial)
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

    public class DisplayStatue : Item
    {
        private MonsterStatuetteInfo _Info;

        [CommandProperty(AccessLevel.GameMaster)]
        public MonsterStatuetteInfo Info
        {
            get { return _Info; }
            set
            {
                _Info = value;

                if (ItemID != _Info.ItemID)
                {
                    ItemID = _Info.ItemID;
                }

                InvalidateProperties();
            }
        }

        public override int LabelNumber
        {
            get
            {
                if (_Info == null)
                    return base.LabelNumber;

                return _Info.LabelNumber;
            }
        }

        [Constructable]
        public DisplayStatue()
        {
            AssignRandom();

            Hue = 2958;
        }

        public void AssignRandom()
        {
            MonsterStatuetteInfo info;

            do
            {
                info = MonsterStatuetteInfo.Table[Utility.Random(MonsterStatuetteInfo.Table.Length)];
            }
            while (ItemID == info.ItemID);

            Info = info;
        }

        public DisplayStatue(Serial serial)
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

            AssignRandom();
        }
    }

    public class UOBoard : Item
    {
        private int _Index;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Index
        {
            get { return _Index; }
            set
            {
                _Index = value;

                if (_Index < 0)
                    _Index = 0;

                if (_Index > 9)
                    _Index = 0;
            }
        }

        public override int LabelNumber => 1080085;  // The Rulebook

        [Constructable]
        public UOBoard() : base(0xFAA)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 3))
            {
                int cliloc;

                if (_Index == 0)
                {
                    cliloc = 1080095;
                }
                else
                {
                    cliloc = 1080086 + _Index;
                }

                from.SendLocalizedMessage(cliloc);
                Index++;
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> entries)
        {
            base.GetContextMenuEntries(from, entries);

            entries.Add(new SimpleContextMenuEntry(from, 3006162, m =>
                {
                    Index = 0;
                }, 2));
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1062703); // Spectator Vision
        }

        public UOBoard(Serial serial)
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

    public class GaryTheDungeonMaster : BaseCreature
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public GaryRegion RegionProps
        {
            get { return Region as GaryRegion; }
            set { }
        }

        public GaryTheDungeonMaster()
            : base(AIType.AI_Vendor, FightMode.None, 10, 1, .2, .4)
        {
            Blessed = true;
            Body = 0x190;
            Name = "Gary";
            Title = "the Dungeon Master";

            SetStr(150);
            SetInt(150);
            SetDex(150);

            SetWearable(new ShortPants(), 1024);
            SetWearable(new FancyShirt(), 680);
            SetWearable(new JinBaori());
            SetWearable(new Shoes());

            HairItemID = 8253;
            FacialHairItemID = 8267;
            Hue = Race.RandomSkinHue();

            CantWalk = true;
            SpeechHue = 0x3B2;
        }

        public GaryTheDungeonMaster(Serial serial)
            : base(serial)
        {
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

            if (version == 0)
                Delete();
        }
    }

    public class GameMaster : BaseCreature
    {
        public GameMaster()
            : base(AIType.AI_Vendor, FightMode.None, 10, 1, .2, .4)
        {
            Blessed = true;
            Body = 0x190;
            Name = "Game Master";

            SetStr(150);
            SetInt(150);
            SetDex(150);

            SetWearable(new Robe(0x204F), 0x85);

            HairItemID = 8253;
            FacialHairItemID = 8267;
            Hue = Race.RandomSkinHue();

            CantWalk = true;
            SpeechHue = 0x3B2;
        }

        public GameMaster(Serial serial)
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

            Timer.DelayCall(TimeSpan.FromSeconds(10), Delete);
        }
    }
}
