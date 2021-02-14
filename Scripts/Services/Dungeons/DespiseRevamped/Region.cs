using Server.Items;
using Server.Mobiles;
using Server.Regions;
using Server.Spells;
using System;
using System.Collections.Generic;

namespace Server.Engines.Despise
{
    public class DespiseRegion : BaseRegion
    {
        private readonly bool m_LowerLevel;

        public DespiseRegion(string name, Rectangle2D[] bounds) : this(name, bounds, false)
        {
        }

        public DespiseRegion(string name, Rectangle2D[] bounds, bool lowerLevel) : base(name, Map.Trammel, DefaultPriority, bounds)
        {
            m_LowerLevel = lowerLevel;
            Register();
        }

        private Rectangle2D m_KickBounds = new Rectangle2D(5576, 626, 6, 10);

        public bool IsInGoodRegion(Point3D loc)
        {
            foreach (Rectangle2D rec in DespiseController.GoodBounds)
                if (rec.Contains(loc))
                    return true;

            return false;
        }

        public bool IsInEvilRegion(Point3D loc)
        {
            foreach (Rectangle2D rec in DespiseController.EvilBounds)
                if (rec.Contains(loc))
                    return true;

            return false;
        }

        public bool IsInLowerRegion(Point3D loc)
        {
            foreach (Rectangle2D rec in DespiseController.LowerLevelBounds)
                if (rec.Contains(loc))
                    return true;

            return false;
        }

        public bool IsInStartRegion(Point3D loc)
        {
            return !IsInLowerRegion(loc) && !IsInEvilRegion(loc) && !IsInGoodRegion(loc);
        }

        public override void OnDeath(Mobile m)
        {
            base.OnDeath(m);

            if (m is DespiseBoss boss)
            {
                DespiseController controller = DespiseController.Instance;

                if (controller != null && controller.Boss == m)
                {
                    Quests.WhisperingWithWispsQuest.OnBossSlain(boss);

                    controller.OnBossSlain();
                }
            }
            else if (m is PlayerMobile && m_LowerLevel)
            {
                KickFromRegion(m, false);
            }
        }

        public override bool OnBeforeDeath(Mobile m)
        {
            if (m is DespiseCreature despiseCreature && despiseCreature.Region != null && despiseCreature.Region.IsPartOf(GetType()) && !despiseCreature.Controlled && despiseCreature.Orb == null)
            {
                Dictionary<DespiseCreature, int> creatures = new Dictionary<DespiseCreature, int>();

                foreach (DamageEntry de in despiseCreature.DamageEntries)
                {
                    if (de.Damager is DespiseCreature creat)
                    {
                        if (!creat.Controlled || creat.Orb == null)
                            continue;

                        if (creatures.ContainsKey(creat))
                            creatures[creat] += de.DamageGiven;
                        else
                            creatures[creat] = de.DamageGiven;
                    }
                }

                if (creatures.Count > 0)
                {
                    DespiseCreature topdam = null;
                    int highest = 0;

                    foreach (KeyValuePair<DespiseCreature, int> kvp in creatures)
                    {
                        if (topdam == null || kvp.Value > highest)
                        {
                            topdam = kvp.Key;
                            highest = kvp.Value;
                        }
                    }

                    if (topdam != null && highest > 0)
                    {
                        int mobKarma = Math.Abs(despiseCreature.Karma);
                        int karma = (int) (((double) mobKarma / 10) * highest / despiseCreature.HitsMax);

                        if (karma < 1)
                            karma = 1;

                        if (despiseCreature.Karma > 0)
                            karma *= -1;

                        Mobile master = topdam.GetMaster();
                        Alignment oldAlign = topdam.Alignment;
                        int power = topdam.Power;
                        topdam.Karma += karma;
                        Alignment newAlign = topdam.Alignment;

                        if (master != null && karma > 0)
                            master.SendLocalizedMessage(1153281); // Your possessed creature has gained karma!
                        else if (master != null && karma < 0)
                            master.SendLocalizedMessage(1153282); // Your possessed creature has lost karma!

                        if (power < topdam.MaxPower)
                        {
                            topdam.Progress += despiseCreature.Power;

                            if (topdam.Power > power && master != null)
                                master.SendLocalizedMessage(1153294,
                                    topdam.Name); // ~1_NAME~ has achieved a new threshold in power!
                        }
                        else if (master != null)
                            master.SendLocalizedMessage(1153309); // Your controlled creature cannot gain further power.

                        if (oldAlign != newAlign && newAlign != Alignment.Neutral && topdam.MaxPower < 15)
                        {
                            topdam.MaxPower = 15;

                            if (master != null)
                                master.SendLocalizedMessage(1153293, topdam.Name); // ~1_NAME~ is growing in strength.

                            topdam.Delta(MobileDelta.Noto);

                            topdam.FixedEffect(0x373A, 10, 30);
                            topdam.PlaySound(0x209);
                        }

                        if (master != null && master.Map != null && master.Map != Map.Internal &&
                            master.Backpack != null)
                        {
                            PutridHeart heart = new PutridHeart(Utility.RandomMinMax(despiseCreature.Power * 8,
                                despiseCreature.Power * 10));

                            if (!master.Backpack.TryDropItem(master, heart, false))
                            {
                                heart.MoveToWorld(master.Location, master.Map);
                            }
                        }
                    }
                }
            }

            return base.OnBeforeDeath(m);
        }

        public override bool OnDoubleClick(Mobile m, object o)
        {
            if (o is BallOfSummoning || o is BraceletOfBinding)
                return false;

            if (o is Corpse c && m.AccessLevel == AccessLevel.Player && (c.Owner == null || c.Owner is DespiseCreature))
            {
                m.SendLocalizedMessage(1152684); // There is no loot on the corpse.
                return false;
            }

            return base.OnDoubleClick(m, o);
        }

        #region Boss Enconter

        public static void GetArmyPower(ref int good, ref int evil)
        {
            foreach (WispOrb orb in WispOrb.Orbs)
            {
                if (orb.Alignment == Alignment.Good)
                    good += orb.GetArmyPower();
                else if (orb.Alignment == Alignment.Evil)
                    evil += orb.GetArmyPower();
            }
        }

        #endregion

        public override bool CheckTravel(Mobile from, Point3D p, TravelCheckType type)
        {
            if (from.AccessLevel > AccessLevel.Player)
                return true;

            switch (type)
            {
                case TravelCheckType.RecallFrom: return true;
                case TravelCheckType.RecallTo: return false;
                case TravelCheckType.GateFrom: return false;
                case TravelCheckType.GateTo: return false;
                case TravelCheckType.Mark: return false;
                case TravelCheckType.TeleportFrom: return true;
                case TravelCheckType.TeleportTo: return true;
            }

            return false;
        }

        public override void OnEnter(Mobile m)
        {
            if (m.AccessLevel > AccessLevel.Player)
                return;

            if (!IsInStartRegion(m.Location) && m is BaseCreature bc && !(bc is DespiseCreature) && !(bc is CorruptedWisp) && !(bc is EnsorcledWisp) && (bc.Controlled || bc.Summoned))
            {
                KickPet(bc);
            }

            if (m is PlayerMobile && IsInLowerRegion(m.Location))
            {
                WispOrb orb = DespiseController.GetWispOrb(m);

                if (orb == null)
                    Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerStateCallback(Kick_Callback), m);
            }
        }

        public override void OnLocationChanged(Mobile m, Point3D oldLocation)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(1.5), () =>
            {
                if (!IsInStartRegion(m.Location) && m is BaseCreature bc && !(bc is DespiseCreature) && !(bc is CorruptedWisp) && !(bc is EnsorcledWisp) && (bc.Controlled || bc.Summoned))
                {
                    if (bc.Summoned)
                        bc.Delete();
                    else
                        KickFromRegion(bc, false);
                }
            });

            base.OnLocationChanged(m, oldLocation);
        }

        private void KickPet(Mobile m)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(0.5), bc =>
            {
                if (bc.Summoned)
                    bc.Delete();
                else
                    KickFromRegion(bc, false);

                if (bc.GetMaster() != null)
                    bc.GetMaster().SendLocalizedMessage(bc.Summoned ? 1153193 : 1153192); // Your pet has been teleported outside the Despise dungeon entrance.
            }, (BaseCreature)m);
        }

        public void Kick_Callback(object o)
        {
            Mobile m = o as Mobile;

            if (m != null)
            {
                KickFromRegion(m, true);
                m.SendLocalizedMessage(1153347); // Without the presence of a Wisp Orb, strong magical forces send you back to whence you came...
            }
        }

        private void KickFromRegion(Mobile m, bool telepet)
        {
            while (true)
            {
                int x = Utility.RandomMinMax(m_KickBounds.X, m_KickBounds.X + m_KickBounds.Width);
                int y = Utility.RandomMinMax(m_KickBounds.Y, m_KickBounds.Y + m_KickBounds.Height);
                int z = Map.Trammel.GetAverageZ(x, y);
                Point3D p = new Point3D(x, y, z);

                if (Map.CanSpawnMobile(p))
                {
                    if (m.Corpse != null)
                        m.Corpse.MoveToWorld(p, Map.Trammel);

                    m.MoveToWorld(p, Map.Trammel);

                    if (telepet)
                        WispOrb.TeleportPet(m);
                    else
                    {
                        WispOrb orb = DespiseController.GetWispOrb(m);

                        if (orb != null && orb.Pet != null)
                            orb.Pet.Kill();
                    }

                    break;
                }
            }
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            return false;
        }

        public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
        {
            global = LightCycle.DungeonLevel;
        }
    }
}
