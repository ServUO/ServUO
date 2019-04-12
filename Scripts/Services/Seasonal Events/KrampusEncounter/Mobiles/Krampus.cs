using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Items;
using Server.Engines.SeasonalEvents;

namespace Server.Mobiles
{
    [CorpseName("the corpse of krampus")]
    public class Krampus : BaseCreature
    {
        public override bool TeleportsTo { get { return true; } }

        public List<BaseCreature> SummonedHelpers { get; set; }
        private DateTime _NextSummon;
        private DateTime _NextArea;
        public const int MaxSummons = 12;

        private DateTime _LastActivity;

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

            SetSkill(SkillName.MagicResist, 150);
            SetSkill(SkillName.Tactics, 110, 120);
            SetSkill(SkillName.Wrestling, 120.0, 125.0);
            SetSkill(SkillName.DetectHidden, 60.0, 70.0);
            SetSkill(SkillName.Parry, 60, 70);

            Fame = 30000;
            Karma = -30000;

            switch (Utility.Random(3))
            {
                case 0:
                    SetMagicalAbility(MagicalAbility.Piercing); break;
                case 1:
                    SetMagicalAbility(MagicalAbility.Bashing); break;
                case 2:
                    SetMagicalAbility(MagicalAbility.Slashing); break;
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

                int count = Utility.RandomMinMax(3, 5);

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

                        if (spawn.Combatant != null)
                        {
                            if (!(spawn.Combatant is PlayerMobile) || !((PlayerMobile)spawn.Combatant).HonorActive)
                                spawn.Combatant = com;
                        }
                        else
                        {
                            spawn.Combatant = com;
                        }

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

            if (defender == _LastTeleported)
            {
                BoneBreakerContext.CheckHit(this, defender);
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

            if (Combatant == null)
            {
                if (_LastActivity + TimeSpan.FromHours(2) < DateTime.UtcNow)
                {
                    Delete();
                }

                return;
            }

            _LastActivity = DateTime.UtcNow;

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

            if (Map != null && SpawnLocation != Point3D.Zero && !Utility.InRange(Location, SpawnLocation, 25))
            {
                MoveToWorld(SpawnLocation, Map);
            }
        }

        public override void OnDeath(Container c)
        {
            if (KrampusEncounter.Encounter != null && KrampusEncounter.Encounter.Krampus == this)
            {
                var rights = GetLootingRights();

                foreach (var ds in rights.Where(s => s.m_Mobile is PlayerMobile && s.m_HasRight))
                {
                    var m = ds.m_Mobile as PlayerMobile;
                    int ordersComplete = 0;

                    if (KrampusEncounter.Encounter.CompleteTable.ContainsKey(m))
                    {
                        ordersComplete = KrampusEncounter.Encounter.CompleteTable[m];
                    }

                    if (ordersComplete >= 3 || Utility.RandomMinMax(0, 8) <= ordersComplete)
                    {
                        Item item;

                        switch (Utility.Random(8))
                        {
                            case 0: item = new KrampusCoinPurse(m); break;
                            case 1: item = new CardOfSemidar(Utility.RandomMinMax(0, 6)); break;
                            case 2: item = new NiceTitleDeed(); break;
                            case 3: item = new NaughtyTitleDeed(); break;
                            case 4: item = new PunisherTitleDeed(); break;
                            case 5: item = new RecipeScroll(586); break; // minion hat
                            case 6: item = new RecipeScroll(587); break; // minion boots
                            case 7: item = new KrampusCoinPurse(463); break; // minion talons
                            case 8: item = new KrampusCoinPurse(588); break; // minion earrings
                            case 9: item = new KrampusPunishinList(m.Name); break;
                            case 10: item = new SpikedWhip(); break;
                            case 11: item = new BarbedWhip(); break;
                            case 12: item = new BladedWhip(); break;
                        }
                    }
                }
            }

            base.OnDeath(c);
        }

        public override void Delete()
        {
            base.Delete();

            if (SpawnLocation != Point3D.Zero && KrampusEncounter.Encounter != null)
            {
                KrampusEncounter.Encounter.OnKrampusKilled();
            }

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
