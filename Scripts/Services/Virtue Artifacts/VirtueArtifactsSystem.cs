using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using Server.Engines.CannedEvil;

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

	        if (r is ChampionSpawnRegion)
		        return false;
	        

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
            
            double vapoints = pm.VASTotalMonsterFame;

            pm.VASTotalMonsterFame += (int)(bc.Fame * (1 + Math.Sqrt(pm.Luck) / 100));

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
                catch
                { }

                if (i != null)
                {
                    killer.PlaySound(0x5B4);
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
