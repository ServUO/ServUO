using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Misc
{
    public class VirtueArtifactsSystem
    {
        private static bool m_Enabled = (Core.Expansion >= Expansion.ML);
        public static bool Enabled { get { return m_Enabled; } }

        private static Type[] m_VirtueArtifacts = new Type[]
            {
                typeof( KatrinasCrook ), typeof( JaanasStaff ), typeof( DragonsEnd ), typeof( AnkhPendant ),
                typeof( SentinelsGuard ), typeof( LordBlackthornsExemplar ), typeof( MapOfTheKnownWorld ), typeof( TenthAnniversarySculpture ),
                typeof( CompassionArms ), typeof( JusticeBreastplate ), typeof( ValorGauntlets ), typeof( HonestyGorget ),
                typeof( SpiritualityHelm ), typeof( HonorLegs ), typeof( SacrificeSollerets )
            };

        public static Type[] VirtueArtifacts { get { return m_VirtueArtifacts; } }

        private static bool CheckLocation(Mobile m)
        {
            Region r = m.Region;

            if (r.IsPartOf(typeof(Server.Regions.HouseRegion)) || Server.Multis.BaseBoat.FindBoatAt(m, m.Map) != null)
                return false;

            return (r.IsPartOf("Covetous") || r.IsPartOf("Deceit") || r.IsPartOf("Despise") || r.IsPartOf("Destard") ||
                r.IsPartOf("Hythloth") || r.IsPartOf("Shame") || r.IsPartOf("Wrong"));
        }

        public static void HandleKill(Mobile victim, Mobile killer)
        {
            PlayerMobile pm = killer as PlayerMobile;
            BaseCreature bc = victim as BaseCreature;

            if (!Enabled || pm == null || bc == null || !CheckLocation(bc) || !CheckLocation(pm) || !killer.InRange(victim, 18))
                return;

            if (bc.Controlled || bc.Owners.Count > 0 || bc.Fame <= 0)
                return;

            //25000 for 1/100 chance, 10 hyrus
            //1500, 1/1000 chance, 20 lizard men for that chance.

            pm.VASTotalMonsterFame += (int)(bc.Fame * (1 + Math.Sqrt(pm.Luck) / 100));

            //This is the Exponentional regression with only 2 datapoints.
            //A log. func would also work, but it didn't make as much sense.
            //This function isn't OSI exact beign that I don't know OSI's func they used ;p
            int x = pm.VASTotalMonsterFame;

            //const double A = 8.63316841 * Math.Pow( 10, -4 );
            const double A = 0.000863316841;
            //const double B = 4.25531915 * Math.Pow( 10, -6 );
            const double B = 0.00000425531915;

            double chance = A * Math.Pow(10, B * x);

            double roll = Utility.RandomDouble();

            killer.PlaySound(0x5B4);

            if (chance > roll)
            {
                Item i = null;

                try
                {
                    i = Activator.CreateInstance(m_VirtueArtifacts[Utility.Random(m_VirtueArtifacts.Length)]) as Item;
                }
                catch
                { }

                if (i != null)
                {
                    pm.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.

                    if (!pm.PlaceInBackpack(i))
                    {
                        if (pm.BankBox != null && pm.BankBox.TryDropItem(killer, i, false))
                            pm.SendLocalizedMessage(1079730); // The item has been placed into your bank box.
                        else
                        {
                            pm.SendLocalizedMessage(1072523); // You find an artifact, but your backpack and bank are too full to hold it.
                            i.MoveToWorld(pm.Location, pm.Map);
                        }
                    }

                    pm.VASTotalMonsterFame = 0;                   
                }
            }
        }
    }
}