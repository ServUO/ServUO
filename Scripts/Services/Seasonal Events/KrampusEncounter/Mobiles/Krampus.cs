using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a skeletal corpse")]
    public class Krampus : BaseCreature
    {
        public override bool TeleportsTo { get { return true; } }

        public List<BaseCreature> SummonedHelpers { get; set; }
        private DateTime _NextSummon;
        public const int MaxSummons = 12;

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D SpawnLocation { get; set; }

        [Constructable]
        public Krampus()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Krampus";
            Body = 1484;

            SetStr(700, 750);
            SetDex(800, 900);
            SetInt(500, 600);

            SetHits(40000);

            SetDamage(28, 35);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 80, 90);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 80, 90);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 110, 120);
            SetSkill(SkillName.Wrestling, 120.0, 125.0);
            SetSkill(SkillName.DetectHidden, 60.0, 70.0);
            SetSkill(SkillName.ResistSpells, 150);

            Fame = 3000;
            Karma = -3000;

            switch (Utility.Random(3))
            {
                case 0:
                    SetMagicAbility(MagicalAbility.Piercing); break;
                case 1:
                    SetMagicAbility(MagicalAbility.Bashing); break;
                case 2:
                    SetMagicAbility(MagicalAbility.Slashing); break;
            }

            _NextSummon = DateTime.UtcNow;
        }

        public int TotalSummons()
        {
            return SummonedHelpers.Where(bc => bc != null && bc.Alive).Count();
        }

        public void Summon(Mobile target)
        {
            if (target == null)
                return;

            int max = MaxSummons;
            var map = Map;

            if (map == null || TotalSummons() > max)
                return;

            MovingEffect(target, 0xA271, 10, 0, false, false, 0, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(2), com =>
            {
                if (!com.Alive)
                    return;

                int count = Utility.RandomList(1, 2, 2, 2, 3, 3, 4, 5);

                for (int i = 0; i < count; i++)
                {
                    Point3D p = com.Location;

                    for (int j = 0; j < 10; j++)
                    {
                        int x = Utility.RandomMinMax(p.X - 3, p.X + 3);
                        int y = Utility.RandomMinMax(p.Y - 3, p.Y + 3);
                        int z = map.GetAverageZ(x, y);

                        if (map.CanSpawnMobile(x, y, z))
                        {
                            p = new Point3D(x, y, z);
                            break;
                        }
                    }

                    BaseCreature spawn = new KrampusMinion();

                    if (spawn != null)
                    {
                        spawn.MoveToWorld(p, map);
                        spawn.Team = this.Team;
                        spawn.SummonMaster = this;

                        Timer.DelayCall(TimeSpan.FromMilliseconds(250), minion =>
                        {
                            if (minion.Combatant != null)
                            {
                                if (!(minion.Combatant is PlayerMobile) || !((PlayerMobile)minion.Combatant).HonorActive)
                                    minion.Combatant = com;
                            }
                            else
                            {
                                minion.Combatant = com;
                            }

                        }, spawn);

                        AddHelper(spawn);
                    }
                }
            }, target);

            _NextSummon = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(20, 40));
        }

        protected virtual void AddHelper(BaseCreature bc)
        {
            if (SummonedHelpers == null)
                SummonedHelpers = new List<BaseCreature>();

            if (!SummonedHelpers.Contains(bc))
                SummonedHelpers.Add(bc);
        }

        private Mobile _LastTeleported;

        public override void OnAfterTeleport(Mobile m)
        {
            m.SendLocalizedMessage(1072195); // Without warning, you are magically transported closer to your enemy!
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (defender is _LastTeleported)
            {
                BoneBreakerContext.OnHit(this, defender);
            }

            _LastTeleported = null;
        }

        public override int GetIdleSound()
        {
            return 1589;
        }

        public override int GetAngerSound()
        {
            return 1586;
        }

        public override int GetHurtSound()
        {
            return 1588;
        }

        public override int GetDeathSound()
        {
            return 1587;
        }

        public Krampus(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 5);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (_NextSummon < DateTime.UtcNow && 0.25 > Utility.RandomDouble())
            {
                var target = GetTeleportTarget();

                if (target != null)
                {
                    Summon(target);

                    _NextSummon = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(20, 40));
                }
            }
            else if (_NextArea < DateTime.UtcNow && 0.25 > Utility.RandomDouble())
            {

            }
        }

        public override void Delete()
        {
            base.Delete();

            if (SummonedHelpers != null)
            {
                ColUtility.Free(SummonedHelpers);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(SpawnLocation);

            writer.Write(SummonedHelpers == null ? 0 : SummonedHelpers.Count);

            if (SummonedHelpers != null)
                SummonedHelpers.ForEach(m => writer.Write(m));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            SpawnLocation = reader.ReadPoint3D();

            int count = reader.ReadInt();

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    BaseCreature summon = reader.ReadMobile() as BaseCreature;

                    if (summon != null)
                    {
                        if (SummonedHelpers == null)
                            SummonedHelpers = new List<BaseCreature>();

                        SummonedHelpers.Add(summon);
                    }
                }
            }

            _NextSummon = DateTime.UtcNow;
        }
    }
}
