using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using System.Linq;
using Server.Engines.Points;
using Server.Regions;
using System.Xml;
using Server.Spells;

namespace Server.Engines.Blackthorn
{
	public class BlackthornRegion : DungeonRegion
	{
        public static readonly Point3D[] StableLocs = new Point3D[] { new Point3D(1510, 1543, 25), 
            new Point3D(1516, 1542, 25), new Point3D(1520, 1542, 25), new Point3D(1525, 1542, 25) };

        public static readonly Rectangle2D PetSafeZone = new Rectangle2D(6421, 2676, 20, 7);

        private static readonly Point3D[] Random_Locations =
        {
            new Point3D(6459, 2781, 0),
            new Point3D(6451, 2781, 0),
            new Point3D(6443, 2781, 0), 
            new Point3D(6409, 2792, 0),
            new Point3D(6356, 2781, 0), 
            new Point3D(6272, 2702, 0), 
            new Point3D(6272, 2656, 0),
            new Point3D(6456, 2623, 0),
        };

        private static readonly Type[] Random_Spawn =
        {
            typeof(Skeleton),
            typeof(FrostOoze),
            typeof(GiantSpider),
            typeof(EarthElemental),
            typeof(GiantRat),
            typeof(VampireBat),
            typeof(Lich)
        };

        private Timer _RegionTimer;

        public BlackthornRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), OnTick);
        }

        public void OnTick()
        {
            foreach (Mobile m in this.GetEnumeratedMobiles().Where(m => m is PlayerMobile && m.AccessLevel == AccessLevel.Player))
            {
                if (m.Hidden)
                    m.RevealingAction();

                if (m.LastMoveTime + 120000 < Core.TickCount)
                    MoveLocation(m);
            }
        }

        public void MoveLocation(Mobile m)
        {
            Point3D p = Random_Locations[Utility.Random(Random_Locations.Length)];

            m.MoveToWorld(p, this.Map);

            for (int x = m.X - 1; x <= m.X + 1; x++)
            {
                for (int y = m.Y - 1; y <= m.Y + 1; y++)
                {
                    Effects.SendLocationEffect(new Point3D(x, y, m.Z), m.Map, Utility.RandomList(14120, 4518, 14133), 16, 1, 1166, 0);

                    if (Utility.RandomBool() && (x != m.X || y != m.Y))
                    {
                        BaseCreature spawn = Activator.CreateInstance(Random_Spawn[Utility.Random(Random_Spawn.Length)]) as BaseCreature;

                        if (spawn != null)
                        {
                            spawn.MoveToWorld(new Point3D(x, y, m.Z), this.Map);
                            Timer.DelayCall(() => spawn.Combatant = m);
                        }
                    }
                }
            }

            Effects.PlaySound(m.Location, m.Map, 0x231);
            m.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x22, 500855); // You are enveloped by a noxious gas cloud!                
            m.ApplyPoison(m, Poison.Lethal);
        }

        public override void OnLocationChanged(Mobile m, Point3D oldLocation)
        {
            base.OnLocationChanged(m, oldLocation);

            if (PetSafeZone.Contains(m.Location) || m.AccessLevel > AccessLevel.Player)
                return;

            if (m.Mount != null)
            {
                m.Mount.Rider = null;

                if (m.Mount is BaseCreature && ((BaseCreature)m.Mount).Controlled)
                {
                    BaseCreature mount = m.Mount as BaseCreature;
                    TryAutoStable((BaseCreature)mount);
                }
            }

            if (m is BaseCreature && ((BaseCreature)m).Controlled)
                TryAutoStable((BaseCreature)m);

            base.OnLocationChanged(m, oldLocation);
        }

        public void TryAutoStable(BaseCreature pet)
        {
            Mobile owner = pet.GetMaster();

            if (!pet.Controlled || owner == null)
            {
                return;
            }
            if (pet.Body.IsHuman || pet.IsDeadPet || pet.Allured)
            {
                SendToStables(pet);
            }
            else if (owner.Stabled.Count >= AnimalTrainer.GetMaxStabled(owner))
            {
                SendToStables(pet);
            }
            else if ((pet is PackLlama || pet is PackHorse || pet is Beetle) &&
                     (pet.Backpack != null && pet.Backpack.Items.Count > 0))
            {
                SendToStables(pet);
            }
            else
            {
                pet.ControlTarget = null;
                pet.ControlOrder = OrderType.Stay;
                pet.Internalize();

                pet.SetControlMaster(null);
                pet.SummonMaster = null;

                pet.IsStabled = true;
                pet.StabledBy = owner;

                if (Core.SE)
                {
                    pet.Loyalty = AnimalTrainer.MaxLoyalty; // Wonderfully happy
                }

                owner.Stabled.Add(pet);
            }
        }

        public void SendToStables(BaseCreature bc)
        {
            Point3D p = StableLocs[Utility.Random(StableLocs.Length)];

            bc.MoveToWorld(p, this.Map);
        }

        public override bool CheckTravel(Mobile traveller, Point3D p, TravelCheckType type)
        {
            if (traveller.AccessLevel > AccessLevel.Player)
                return true;

            return type > TravelCheckType.Mark;
        }
	}
}