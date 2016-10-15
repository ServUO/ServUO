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

namespace Server.Engines.Blackthorn
{
	public class BlackthornRegion : DungeonRegion
	{
        public static readonly Point3D[] StableLocs = new Point3D[] { new Point3D(1510, 1543, 25), 
            new Point3D(1516, 1542, 25), new Point3D(1520, 1542, 25), new Point3D(1525, 1542, 25) };

        public static readonly Rectangle2D PetSafeZone = new Rectangle2D(6421, 2676, 20, 7);

        public BlackthornRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        public override void OnLocationChanged(Mobile m, Point3D oldLocation)
        {
            base.OnLocationChanged(m, oldLocation);

            if (PetSafeZone.Contains(m.Location))
                return;

            // Ethereals too???
            if (m.Mount is BaseCreature && ((BaseCreature)m.Mount).Controlled)
            {
                BaseCreature mount = m.Mount as BaseCreature;
                m.Mount.Rider = null;

                TryAutoStable((BaseCreature)mount);
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
	}
}