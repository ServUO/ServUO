using Server.Engines.Points;
using Server.Engines.SeasonalEvents;
using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Misc
{
    public class VirtueArtifactsSystem : PointsSystem
    {
        public static bool Enabled => SeasonalEventSystem.IsActive(EventType.VirtueArtifacts);

        private static readonly Type[] m_VirtueArtifacts = new Type[]
            {
                typeof( KatrinasCrook ), typeof( JaanasStaff ), typeof( DragonsEnd ), typeof( AnkhPendant ),
                typeof( SentinelsGuard ), typeof( LordBlackthornsExemplar ), typeof( MapOfTheKnownWorld ), typeof( TenthAnniversarySculpture ),
                typeof( CompassionArms ), typeof( JusticeBreastplate ), typeof( ValorGauntlets ), typeof( HonestyGorget ),
                typeof( SpiritualityHelm ), typeof( HonorLegs ), typeof( SacrificeSollerets )
            };

        public static Type[] Artifacts => m_VirtueArtifacts;

        public override PointsType Loyalty => PointsType.VAS;
        public override TextDefinition Name => m_Name;
        public override bool AutoAdd => true;
        public override double MaxPoints => double.MaxValue;
        public override bool ShowOnLoyaltyGump => false;

        private readonly TextDefinition m_Name = new TextDefinition("Virtue Artifact System");

        private bool CheckLocation(Mobile m)
        {
            Region r = m.Region;

            if (m is BaseCreature && ((BaseCreature)m).IsChampionSpawn)
                return false;

            if (r.IsPartOf<Regions.HouseRegion>() || Multis.BaseBoat.FindBoatAt(m, m.Map) != null)
                return false;

            return (r.IsPartOf("Covetous") || r.IsPartOf("Deceit") || r.IsPartOf("Despise") || r.IsPartOf("Destard") ||
                r.IsPartOf("Hythloth") || r.IsPartOf("Shame") || r.IsPartOf("Wrong"));
        }

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
            // No message here
        }

        public override TextDefinition GetTitle(PlayerMobile from)
        {
            return new TextDefinition("Virtue Artifact System");
        }

        public override void ProcessKill(Mobile victim, Mobile damager)
        {
            PlayerMobile pm = damager as PlayerMobile;
            BaseCreature bc = victim as BaseCreature;

            if (!Enabled || pm == null || bc == null || !CheckLocation(bc) || !CheckLocation(pm) || !damager.InRange(victim, 18) || !damager.Alive || bc.GivenSpecialArtifact)
                return;

            if (bc.Controlled || bc.Owners.Count > 0 || bc.Fame <= 0)
                return;

            int luck = Math.Max(0, pm.RealLuck);
            AwardPoints(pm, (int)Math.Max(0, (bc.Fame * (1 + Math.Sqrt(luck) / 100))));

            double vapoints = GetPoints(pm);
            const double A = 0.000863316841;
            const double B = 0.00000425531915;

            double chance = A * Math.Pow(10, B * vapoints);

            double roll = Utility.RandomDouble();

            if (chance > roll)
            {
                Item i = null;

                try
                {
                    i = Activator.CreateInstance(m_VirtueArtifacts[Utility.Random(m_VirtueArtifacts.Length)]) as Item;
                }
                catch (Exception e)
                {
                    Diagnostics.ExceptionLogging.LogException(e);
                }

                if (i != null)
                {
                    damager.PlaySound(0x5B4);
                    pm.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.

                    if (!pm.PlaceInBackpack(i))
                    {
                        if (pm.BankBox != null && pm.BankBox.TryDropItem(damager, i, false))
                            pm.SendLocalizedMessage(1079730); // The item has been placed into your bank box.
                        else
                        {
                            pm.SendLocalizedMessage(1072523); // You find an artifact, but your backpack and bank are too full to hold it.
                            i.MoveToWorld(pm.Location, pm.Map);
                        }
                    }

                    bc.GivenSpecialArtifact = true;
                    SetPoints(pm, 0);
                }
            }
        }
    }
}
