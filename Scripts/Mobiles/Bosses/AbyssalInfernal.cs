/* 	Based on Rikktor, still to get detailed information on the Abyssal Infernal */
using System;
using System.Collections;
using Server.Engines.CannedEvil;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an abyssal infernal corpse")]
    public class AbyssalInfernal : BaseChampion
    {
        private DateTime m_Delay;
        [Constructable]
        public AbyssalInfernal()
            : base(AIType.AI_Mage)
        {
            this.Body = 713;// 730;
            this.Name = "The Abyssal Infernal";

            this.SetStr(701, 900);
            this.SetDex(201, 350);
            this.SetInt(51, 100);

            this.SetHits(50000);
            this.SetStam(203, 650);

            this.SetDamage(28, 35);

            this.SetDamageType(ResistanceType.Physical, 25);
            this.SetDamageType(ResistanceType.Fire, 50);
            this.SetDamageType(ResistanceType.Energy, 25);

            this.SetResistance(ResistanceType.Physical, 80, 90);
            this.SetResistance(ResistanceType.Fire, 80, 90);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 80, 90);
            this.SetResistance(ResistanceType.Energy, 80, 90);

            this.SetSkill(SkillName.Anatomy, 100.0);
            this.SetSkill(SkillName.MagicResist, 140.2, 160.0);
            this.SetSkill(SkillName.Tactics, 100.0);

            this.Fame = 22500;
            this.Karma = -22500;

            this.VirtualArmor = 40;
            this.QLPoints = 150;
        }

        public AbyssalInfernal(Serial serial)
            : base(serial)
        {
        }

        public override ChampionSkullType SkullType
        {
            get
            {
                return ChampionSkullType.Terror;
            }
        }
        public override Type[] UniqueList
        {
            get
            {
                return new Type[] { typeof(TongueoftheBeast) };
            }
        }
        public override Type[] SharedList
        {
            get
            {
                return new Type[] { typeof(RoyalGuardInvestigatorsCloak), typeof(JadeArmband), typeof(DetectiveBoots) };
            }
        }
        public override Type[] DecorativeList
        {
            get
            {
                return new Type[] { typeof(MagicalDoor), typeof(MonsterStatuette) };
            }
        }
        public override MonsterStatuetteType[] StatueTypes
        {
            get
            {
                return new MonsterStatuetteType[] { MonsterStatuetteType.ArchDemon };
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return !Core.SE;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return Core.SE;
            }
        }
        public override bool Uncalmable
        {
            get
            {
                return Core.SE;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override ScaleType ScaleType
        {
            get
            {
                return ScaleType.All;
            }
        }
        public override int Scales
        {
            get
            {
                return 20;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 4);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.50)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        this.AddToBackpack(new HornAbyssalInferno());
                        break;
                    case 1:
                        this.AddToBackpack(new NetherCycloneScroll());
                        break;
                }
            }
        }

        public override int GetIdleSound()
        {
            return 1495;
        }

        public override int GetAngerSound()
        {
            return 1492;
        }

        public override int GetHurtSound()
        {
            return 1494;
        }

        public override int GetDeathSound()
        {
            return 1493;
        }

        public override void OnDamagedBySpell(Mobile caster)
        {
            if (this.Map != null && caster != this && caster is PlayerMobile && 0.20 > Utility.RandomDouble())
            {
                this.Combatant = caster;
                this.Map = caster.Map;
                this.Location = caster.Location;

                switch (Utility.Random(5))
                {
                    case 0:
                        caster.Location = new Point3D(6949, 701, 32);
                        break;
                    case 1:
                        caster.Location = new Point3D(6941, 761, 32);
                        break;
                    case 2:
                        caster.Location = new Point3D(7015, 688, 32);
                        break;
                    case 3:
                        caster.Location = new Point3D(7043, 751, 32);
                        break;
                    case 4:
                        caster.Location = new Point3D(6999, 798, 32);
                        break;
                }

                caster.SendMessage("You are being burned by the heat from the Lava!");
                AOS.Damage(caster, Utility.RandomMinMax(75, 85), 0, 100, 0, 0, 0);
                caster.MoveToWorld(caster.Location, caster.Map);
                caster.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                caster.PlaySound(0x1FE);
            }

            base.OnDamagedBySpell(caster);
        }

        public void DrainLife()
        {
            if (this.Map == null)
                return;

            ArrayList list = new ArrayList();

            foreach (Mobile m in this.GetMobilesInRange(8))
            {
                if (m == this || !this.CanBeHarmful(m))
                    continue;

                if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team))
                    list.Add(m);
                else if (m.Player)
                    list.Add(m);
            }

            foreach (Mobile m in list)
            {
                this.DoHarmful(m);

                m.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
                m.PlaySound(0x231);////////////////

                m.SendMessage("You feel the life drain out of you!");

                int toDrain = Utility.RandomMinMax(25, 30);

                this.Hits += toDrain;
                m.Damage(toDrain, this);
            }
        }

        public void SpawnFollowers(Mobile target)
        {
            Map map = this.Map;

            if (map == null)
                return;

            int followers = 0;

            foreach (Mobile m in this.GetMobilesInRange(10))
            {
                if (m is EarthElemental || m is FireElemental || m is AirElemental || m is FireSteed || m is WaterElemental)
                    ++followers;
            }

            if (this.Followers < 9)
            {
                this.PlaySound(0x218);//////////

                int newFollowers = Utility.RandomMinMax(1, 3);

                for (int i = 0; i < newFollowers; ++i)
                {
                    BaseCreature follower;

                    switch (Utility.Random(5))
                    {
                        default:
                        case 0:
                            follower = new WaterElemental();
                            break;
                        case 1:
                            follower = new EarthElemental();
                            break;
                        case 2:
                            follower = new FireSteed();
                            break;
                        case 3:
                            follower = new FireElemental();
                            break;
                        case 4:
                            follower = new AirElemental();
                            break;
                    }

                    follower.Team = this.Team;

                    bool validLocation = false;
                    Point3D loc = this.Location;

                    for (int j = 0; !validLocation && j < 9; ++j)
                    {
                        int x = this.X + Utility.Random(3) - 1;
                        int y = this.Y + Utility.Random(3) - 1;
                        int z = map.GetAverageZ(x, y);

                        if (validLocation = map.CanFit(x, y, this.Z, 16, false, false))
                            loc = new Point3D(x, y, this.Z);
                        else if (validLocation = map.CanFit(x, y, z, 16, false, false))
                            loc = new Point3D(x, y, z);
                    }

                    follower.MoveToWorld(loc, map);
                    follower.Combatant = target;
                }
            }
        }

        public void DoSpecialAbility(Mobile target)
        {
            if (target == null || target.Deleted) //sanity
                return;        

            if (0.05 >= Utility.RandomDouble()) // 20% chance to more ratmen
                this.SpawnFollowers(target);           
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.25 >= Utility.RandomDouble())
                this.DrainLife();

            this.DoSpecialAbility(defender);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.25 >= Utility.RandomDouble())
                this.DrainLife();

            this.DoSpecialAbility(attacker);
        }

        public override void OnActionCombat()
        {
            if (DateTime.UtcNow > this.m_Delay)
            {
                Ability.CrimsonMeteor(this, 100);
                this.m_Delay = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(25, 35));
            }

            base.OnActionCombat();
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