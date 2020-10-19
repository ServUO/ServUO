using Server.Engines.CannedEvil;
using Server.Items;
using Server.Multis;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class BaseSeaChampion : BaseChampion
    {
        public override Type[] UniqueList => new Type[] { };
        public override Type[] SharedList => new Type[] { };
        public override Type[] DecorativeList => new Type[] { };
        public override MonsterStatuetteType[] StatueTypes => new MonsterStatuetteType[] { };

        public override ChampionSkullType SkullType => ChampionSkullType.None;

        private DateTime m_NextBoatDamage;
        private bool m_InDamageMode;
        private readonly Mobile m_Fisher;

        public virtual bool CanDamageBoats => false;
        public virtual TimeSpan BoatDamageCooldown => TimeSpan.MaxValue;
        public virtual DateTime NextBoatDamage => m_NextBoatDamage;
        public virtual int MinBoatDamage => 0;
        public virtual int MaxBoatDamage => 0;
        public virtual int DamageRange => 15;

        public override double BonusPetDamageScalar => 1.75;

        public BaseSeaChampion(Mobile fisher, AIType ai, FightMode fm)
            : base(ai, fm)
        {
            m_NextBoatDamage = DateTime.UtcNow;
            m_InDamageMode = false;
            m_Fisher = fisher;

            m_DamageEntries = new Dictionary<Mobile, int>();
        }

        public override void OnThink()
        {
            base.OnThink();

            if (m_InDamageMode)
                TryDamageBoat();

            else if (CanDamageBoats && DateTime.UtcNow >= NextBoatDamage)
                m_InDamageMode = true;
        }

        public override bool OnBeforeDeath()
        {
            RegisterDamageTo(this);
            AwardArtifact(GetArtifact());

            return base.OnBeforeDeath();
        }

        Dictionary<Mobile, int> m_DamageEntries;

        public virtual void RegisterDamageTo(Mobile m)
        {
            if (m == null)
                return;

            foreach (DamageEntry de in m.DamageEntries)
            {
                Mobile damager = de.Damager;

                Mobile master = damager.GetDamageMaster(m);

                if (master != null)
                    damager = master;

                RegisterDamage(damager, de.DamageGiven);
            }
        }

        public void RegisterDamage(Mobile from, int amount)
        {
            if (from == null || !from.Player)
                return;

            if (m_DamageEntries.ContainsKey(from))
                m_DamageEntries[from] += amount;
            else
                m_DamageEntries.Add(from, amount);
        }

        public void AwardArtifact(Item artifact)
        {
            if (artifact == null)
                return;

            int totalDamage = 0;

            Dictionary<Mobile, int> validEntries = new Dictionary<Mobile, int>();

            foreach (KeyValuePair<Mobile, int> kvp in m_DamageEntries)
            {
                if (IsEligible(kvp.Key, artifact))
                {
                    validEntries.Add(kvp.Key, kvp.Value);
                    totalDamage += kvp.Value;
                }
            }

            int randomDamage = Utility.RandomMinMax(1, totalDamage);

            totalDamage = 0;

            foreach (KeyValuePair<Mobile, int> kvp in m_DamageEntries)
            {
                totalDamage += kvp.Value;

                if (totalDamage > randomDamage)
                {
                    GiveArtifact(kvp.Key, artifact);
                    return;
                }
            }

            artifact.Delete();
        }

        public void GiveArtifact(Mobile to, Item artifact)
        {
            if (to == null || artifact == null)
                return;

            Container pack = to.Backpack;

            if (pack == null || !pack.TryDropItem(to, artifact, false))
                artifact.Delete();
            else
                to.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.
        }

        public bool IsEligible(Mobile m, Item Artifact)
        {
            return m.Player && m.Alive && m.InRange(Location, 32) && m.Backpack != null && m.Backpack.CheckHold(m, Artifact, false);
        }

        public void TryDamageBoat()
        {
            Mobile focusMob = m_Fisher;

            if (focusMob == null || !focusMob.Alive)
                focusMob = Combatant as Mobile;

            if (focusMob == null || focusMob.AccessLevel > AccessLevel.Player || !InRange(focusMob.Location, DamageRange) || BaseBoat.FindBoatAt(focusMob, focusMob.Map) == null)
                return;

            BaseBoat boat = BaseBoat.FindBoatAt(focusMob, focusMob.Map);

            if (boat != null)
            {
                int range = DamageRange;
                for (int x = X - range; x <= X + range; x++)
                {
                    for (int y = Y - range; y <= Y + range; y++)
                    {
                        if (BaseBoat.FindBoatAt(new Point2D(x, y), Map) == boat)
                        {
                            DoDamageBoat(boat);
                            m_NextBoatDamage = DateTime.UtcNow + BoatDamageCooldown;
                            m_InDamageMode = false;
                            return;
                        }
                    }
                }
            }
        }

        public virtual void DoDamageBoat(BaseBoat boat)
        {
            int damage = Utility.RandomMinMax(MinBoatDamage, MaxBoatDamage);

            boat.OnTakenDamage(this, damage);

            for (int x = X - 2; x <= X + 2; x++)
            {
                for (int y = Y - 2; y <= Y + 2; y++)
                {
                    BaseBoat b = BaseBoat.FindBoatAt(new Point2D(x, y), Map);

                    if (b != null && boat == b)
                    {
                        Direction toPush = Direction.North;
                        if (X < x && x - X > 1)
                            toPush = Direction.West;
                        else if (X > x && X - x > 1)
                            toPush = Direction.East;
                        else if (Y < y)
                            toPush = Direction.South;
                        else if (Y > y)
                            toPush = Direction.North;

                        boat.StartMove(toPush, 1, 0x2, boat.SlowDriftInterval, true, false);
                        //TODO: Message and Sound?
                    }
                }
            }
        }

        public Point3D GetValidPoint(BaseBoat boat, Map map, int distance)
        {
            if (boat == null || map == null || map == Map.Internal)
                return new Point3D(X + Utility.RandomMinMax(-1, 1), Y + Utility.RandomMinMax(-1, 1), Z);

            if (distance < 5) distance = 5;
            if (distance > 15) distance = 15;

            int x = boat.X;
            int y = boat.Y;
            int z = boat.Z;
            int size = boat is BritannianShip ? 4 : 3;

            int range = distance - size;

            switch (boat.Facing)
            {
                default:
                case Direction.South:
                case Direction.North:
                    x = Utility.RandomBool() ? Utility.RandomMinMax(x -= distance, x -= (distance - range)) : Utility.RandomMinMax(x += (distance - range), x += distance);
                    y = Utility.RandomMinMax(y - 8, y + 8);
                    z = map.GetAverageZ(x, y);
                    break;
                case Direction.East:
                case Direction.West:
                    x = Utility.RandomMinMax(x - 8, x + 8);
                    y = Utility.RandomBool() ? Utility.RandomMinMax(y -= distance, y -= (distance - range)) : Utility.RandomMinMax(y += (distance - range), y += distance);
                    z = map.GetAverageZ(x, y);
                    break;
            }
            return new Point3D(x, y, z);
        }

        public BaseSeaChampion(Serial serial)
            : base(serial)
        {
        }

        public virtual void OnHitByCannon(IShipCannon cannon, int damage)
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

            m_NextBoatDamage = DateTime.UtcNow;
            m_DamageEntries = new Dictionary<Mobile, int>();
        }
    }
}
