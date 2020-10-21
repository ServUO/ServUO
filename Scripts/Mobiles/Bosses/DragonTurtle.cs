using Server.Engines.CannedEvil;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("a dragon turtle corpse")]
    public class DragonTurtle : BaseChampion
    {
        public override Type[] UniqueList => new Type[] { };
        public override Type[] SharedList => new Type[] { };
        public override Type[] DecorativeList => new Type[] { };
        public override MonsterStatuetteType[] StatueTypes => new MonsterStatuetteType[] { };

        public override ChampionSkullType SkullType => ChampionSkullType.None;

        [Constructable]
        public DragonTurtle() : base(AIType.AI_Mage)
        {
            Name = "a dragon turtle";
            Body = 1288;
            BaseSoundID = 362;

            SetStr(750, 800);
            SetDex(185, 240);
            SetInt(487, 562);

            SetDamage(25, 37);

            SetHits(60000);

            SetResistance(ResistanceType.Physical, 75, 85);
            SetResistance(ResistanceType.Fire, 65, 75);
            SetResistance(ResistanceType.Cold, 70, 75);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 65, 75);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetSkill(SkillName.MagicResist, 90, 120);
            SetSkill(SkillName.Tactics, 200, 110);
            SetSkill(SkillName.Wrestling, 225, 227);

            Fame = 11000;
            Karma = -11000;

            SetWeaponAbility(WeaponAbility.Dismount);
            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 3);
        }

        public override int Meat => 1;
        public override int Hides => 33;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies;
        public override bool TeleportsTo => true;
        public override TimeSpan TeleportDuration => TimeSpan.FromSeconds(30);
        public override int TeleportRange => 10;
        public override bool ReacquireOnMovement => true;
        public override int TreasureMapLevel => 5;

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            if (corpse != null && !corpse.Carved)
            {
                from.SendLocalizedMessage(1156198); // You cut away some scoots, but they remain on the corpse.
                corpse.DropItem(new DragonTurtleScute(18));
            }

            base.OnCarve(from, corpse, with);
        }

        public override Item GetArtifact()
        {
            return new DragonTurtleEgg();
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (!_DoingBubbles || _BubbleLocs == null)
                return;

            List<Tuple<Point3D, int>> copy = new List<Tuple<Point3D, int>>(_BubbleLocs.Where(tup => tup.Item1 == m.Location));

            foreach (Tuple<Point3D, int> t in copy)
            {
                Point3D p = m.Location;
                int hue = 0;

                for (int i = 0; i < _BubbleLocs.Count; i++)
                {
                    if (_BubbleLocs[i].Item1 == p)
                    {
                        hue = _BubbleLocs[i].Item2;
                        _BubbleLocs[i] = new Tuple<Point3D, int>(Point3D.Zero, hue);
                    }

                    Effects.SendTargetEffect(m, 13920, 10, 60, hue == 0 ? 0 : hue - 1, 5);
                    ApplyMod(m, hue);
                }
            }

            ColUtility.Free(copy);
        }

        private long _NextBubbleWander;
        private long _NextBubbleAttack;
        private bool _DoingBubbles;
        public List<Tuple<Point3D, int>> _BubbleLocs { get; set; }
        public Dictionary<Mobile, int> _Affected { get; set; }

        private readonly Direction[] _Directions = { Direction.North, Direction.Right, Direction.East, Direction.Down, Direction.South, Direction.Left, Direction.West, Direction.Up };
        private readonly int[] _Hues = { 0, 33, 44, 9, 63, 53, 117 };

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant != null && Core.TickCount - _NextBubbleWander >= 0)
            {
                DoBubble();
                _NextBubbleWander = Core.TickCount + Utility.RandomMinMax(25000, 35000);
            }

            if (Combatant != null && Core.TickCount - _NextBubbleAttack >= 0)
            {
                DoBubbleAttack();
                _NextBubbleAttack = Core.TickCount + Utility.RandomMinMax(40000, 60000);
            }
        }

        public void DoBubble()
        {
            if (!Alive || Map == Map.Internal || Map == null)
                return;

            int pathLength = Utility.RandomMinMax(5, 11);
            _DoingBubbles = true;
            _BubbleLocs = new List<Tuple<Point3D, int>>();

            for (int i = 0; i < 8; i++)
            {
                _BubbleLocs.Add(new Tuple<Point3D, int>(Location, _Hues[Utility.Random(_Hues.Length)]));
            }

            for (int i = 0; i < pathLength; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(i + 1.0), () =>
                {
                    for (int j = 0; j < _BubbleLocs.Count; j++)
                    {
                        Map map = Map;

                        if (!Alive || map == null || (i > 0 && _BubbleLocs[j].Item1 == Point3D.Zero))
                            continue;

                        Direction d = _Directions[j];

                        int hue = _BubbleLocs[j].Item2;
                        int x = _BubbleLocs[j].Item1.X;
                        int y = _BubbleLocs[j].Item1.Y;

                        if (i > 2 && 0.4 > Utility.RandomDouble())
                        {
                            if (d == Direction.Up)
                                d = Direction.North;
                            else
                                d += 1;

                            Movement.Movement.Offset(d, ref x, ref y);
                        }
                        else
                            Movement.Movement.Offset(d, ref x, ref y);

                        IPoint3D p = new Point3D(x, y, Map.GetAverageZ(x, y));
                        Spells.SpellHelper.GetSurfaceTop(ref p);

                        var newLoc = new Point3D(p);

                        bool hasMobile = false;
                        IPooledEnumerable eable = Map.GetMobilesInRange(newLoc, 0);

                        foreach (Mobile m in eable)
                        {
                            if (m != this && CanBeHarmful(m) && (m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)))
                            {
                                hasMobile = true;

                                Effects.SendTargetEffect(m, 13920, 10, 60, hue == 0 ? 0 : hue - 1, 5);
                                _BubbleLocs[j] = new Tuple<Point3D, int>(Point3D.Zero, hue);

                                ApplyMod(m, hue);
                            }
                        }

                        if (!hasMobile)
                        {
                            Effects.SendLocationEffect(newLoc, Map, 13920, 20, 10, hue == 0 ? 0 : hue - 1, 5);
                            _BubbleLocs[j] = new Tuple<Point3D, int>(newLoc, hue);
                        }
                    }

                    if (i == pathLength - 1)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                        {
                            _DoingBubbles = false;
                            _BubbleLocs.Clear();
                            _BubbleLocs = null;
                        });
                    }
                });
            }
        }

        public void DoBubbleAttack()
        {
            if (!Alive || Map == Map.Internal || Map == null)
                return;

            List<Mobile> toget = new List<Mobile>();

            IPooledEnumerable eable = Map.GetMobilesInRange(Location, 11);

            foreach (Mobile m in eable)
            {
                if (m != this && CanBeHarmful(m) && InLOS(m) && (m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)))
                    toget.Add(m);
            }

            eable.Free();

            toget.ForEach(mob =>
            {
                int hue;

                if (_Affected != null && _Affected.ContainsKey(mob))
                    hue = _Affected[mob];
                else
                    hue = _Hues[Utility.Random(_Hues.Length)];

                MovingParticles(mob, 13920, 10, 0, false, true, hue == 0 ? 0 : hue - 1, 5, 9502, 14120, 0, 0);

                Timer.DelayCall(TimeSpan.FromSeconds(.7), DoAttack_Callback, new object[] { mob, hue });
            });
        }

        private void ApplyMod(Mobile m, int hue)
        {
            ResistanceType type = GetResistanceFromHue(hue);

            if (_Affected == null)
                _Affected = new Dictionary<Mobile, int>();

            _Affected[m] = hue;

            ResistanceMod mod = new ResistanceMod(type, -25);
            m.AddResistanceMod(mod);

            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.DragonTurtleDebuff, 1156192, 1156192));

            Timer.DelayCall(TimeSpan.FromSeconds(30), RemoveMod_Callback, new object[] { m, mod });
        }

        private void RemoveMod_Callback(object obj)
        {
            object[] o = obj as object[];
            Mobile m = o[0] as Mobile;
            ResistanceMod mod = (ResistanceMod)o[1];

            m.RemoveResistanceMod(mod);
            BuffInfo.RemoveBuff(m, BuffIcon.DragonTurtleDebuff);

            if (_Affected != null && _Affected.ContainsKey(m))
                _Affected.Remove(m);
        }

        private void DoAttack_Callback(object obj)
        {
            if (!Alive)
                return;

            object[] o = obj as object[];
            Mobile mob = o[0] as Mobile;
            int hue = (int)o[1];

            ResistanceType type = GetResistanceFromHue(hue);
            int damage = Utility.RandomMinMax(60, 80);

            switch (type)
            {
                case ResistanceType.Physical: AOS.Damage(mob, this, damage, 100, 0, 0, 0, 0); break;
                case ResistanceType.Fire: AOS.Damage(mob, this, damage, 0, 100, 0, 0, 0); break;
                case ResistanceType.Cold: AOS.Damage(mob, this, damage, 0, 0, 100, 0, 0); break;
                case ResistanceType.Poison: AOS.Damage(mob, this, damage, 0, 0, 0, 100, 0); break;
                case ResistanceType.Energy: AOS.Damage(mob, this, damage, 0, 0, 0, 0, 100); break;
            }
        }

        private ResistanceType GetResistanceFromHue(int hue)
        {
            switch (hue)
            {
                case 0: return ResistanceType.Physical;
                case 33:
                case 44: return ResistanceType.Fire;
                case 9: return ResistanceType.Cold;
                case 63: return ResistanceType.Poison;
                case 53:
                case 126: return ResistanceType.Energy;
            }

            return ResistanceType.Physical;
        }

        public DragonTurtle(Serial serial) : base(serial)
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
