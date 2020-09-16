using Server.Mobiles;
using Server.Regions;
using Server.Spells;
using Server.Spells.Bushido;
using Server.Spells.Chivalry;
using Server.Spells.Ninjitsu;
using System;
using System.Linq;
using System.Xml;

namespace Server.Engines.Blackthorn
{
    public class BlackthornDungeon : DungeonRegion
    {
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

        public BlackthornDungeon(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), OnTick);
        }

        public void OnTick()
        {
            if (!Fellowship.ForsakenFoesEvent.Instance.Running)
            {
                foreach (Mobile m in GetEnumeratedMobiles().Where(m => m is PlayerMobile && m.AccessLevel == AccessLevel.Player))
                {
                    if (m.Hidden)
                        m.RevealingAction();

                    if (m.Y > 2575 && m.LastMoveTime + 120000 < Core.TickCount)
                        MoveLocation(m);
                }
            }
        }

        public void MoveLocation(Mobile m)
        {
            Point3D p = Random_Locations[Utility.Random(Random_Locations.Length)];

            m.MoveToWorld(p, Map);

            for (int x = m.X - 1; x <= m.X + 1; x++)
            {
                for (int y = m.Y - 1; y <= m.Y + 1; y++)
                {
                    Effects.SendLocationEffect(new Point3D(x, y, m.Z), m.Map, Utility.RandomList(14120, 4518, 14133), 16, 1, 1166, 0);
                }
            }

            Effects.PlaySound(m.Location, m.Map, 0x231);
            m.LocalOverheadMessage(Network.MessageType.Regular, 0x22, 500855); // You are enveloped by a noxious gas cloud!                
            m.ApplyPoison(m, Poison.Lethal);

            IPooledEnumerable eable = Map.GetMobilesInRange(m.Location, 12);

            foreach (Mobile mob in eable)
            {
                if (mob.Combatant == null && mob is BaseCreature && ((BaseCreature)mob).GetMaster() == null && mob.CanBeHarmful(m))
                {
                    if (mob.InLOS(m))
                        Timer.DelayCall(() => mob.Combatant = m);
                    else
                        ((BaseCreature)mob).AIObject.MoveTo(mob, true, 1);
                }
            }

            eable.Free();

            m.LastMoveTime = Core.TickCount;
        }

        public override bool CheckTravel(Mobile traveller, Point3D p, TravelCheckType type)
        {
            if (traveller.AccessLevel > AccessLevel.Player)
                return true;

            return type > TravelCheckType.Mark;
        }

        public override void OnDeath(Mobile m)
        {
            if (m is BaseCreature && Map == Map.Trammel && InvasionController.TramInstance != null)
                InvasionController.TramInstance.OnDeath(m as BaseCreature);

            if (m is BaseCreature && Map == Map.Felucca && InvasionController.FelInstance != null)
                InvasionController.FelInstance.OnDeath(m as BaseCreature);
        }
    }

    public class BlackthornCastle : GuardedRegion
    {
        public static readonly Point3D[] StableLocs = new Point3D[] { new Point3D(1510, 1543, 25),
            new Point3D(1516, 1542, 25), new Point3D(1520, 1542, 25), new Point3D(1525, 1542, 25) };

        public BlackthornCastle(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
            if (m.AccessLevel > AccessLevel.Player)
                return base.OnBeginSpellCast(m, s);

            int loc;

            if (s is PaladinSpell)
                loc = 1062075; // You cannot use a Paladin ability here.
            else if (s is NinjaMove || s is NinjaSpell || s is SamuraiSpell || s is SamuraiMove)
                loc = 1062938; // That ability does not seem to work here.
            else
                loc = 502629;

            m.SendLocalizedMessage(loc);
            return false;
        }

        public override void OnLocationChanged(Mobile m, Point3D oldLocation)
        {
            base.OnLocationChanged(m, oldLocation);

            if (m is PlayerMobile && m.X <= 1525 && m.X >= 1520 && m.Y <= 1485 && oldLocation.Y > 1485)
            {
                Quests.AVisitToCastleBlackthornQuest.CheckLocation((PlayerMobile)m, oldLocation);
            }

            if (m.AccessLevel > AccessLevel.Player)
                return;

            if (m.Mount != null)
            {
                if (m is PlayerMobile)
                {
                    (m as PlayerMobile).SetMountBlock(BlockMountType.DismountRecovery, TimeSpan.FromSeconds(30), true);
                }
                else
                {
                    m.Mount.Rider = null;
                }

                m.SendLocalizedMessage(1153052); // Mounts and flying are not permitted in this area.

                if (m.Mount is BaseCreature && ((BaseCreature)m.Mount).Controlled)
                {
                    BaseCreature mount = m.Mount as BaseCreature;
                    TryAutoStable(mount);
                }
            }

            if (m is BaseCreature && ((BaseCreature)m).Controlled)
                TryAutoStable((BaseCreature)m);
        }

        public void TryAutoStable(BaseCreature pet)
        {
            if (pet == null)
                return;

            Mobile owner = pet.GetMaster();

            if (!pet.Controlled || owner == null)
            {
                return;
            }
            if (pet.Body.IsHuman || pet.IsDeadPet || pet.Allured)
            {
                SendToStables(pet, owner);
            }
            else if (owner.Stabled.Count >= AnimalTrainer.GetMaxStabled(owner))
            {
                SendToStables(pet, owner);
            }
            else if ((pet is PackLlama || pet is PackHorse || pet is Beetle) &&
                     (pet.Backpack != null && pet.Backpack.Items.Count > 0))
            {
                SendToStables(pet, owner);
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

                pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy
                owner.Stabled.Add(pet);
                owner.SendLocalizedMessage(1153050, pet.Name); // Pets are not permitted in this location. Your pet named ~1_NAME~ has been sent to the stables.
            }
        }

        public void SendToStables(BaseCreature bc, Mobile m = null)
        {
            Point3D p = StableLocs[Utility.Random(StableLocs.Length)];
            bc.MoveToWorld(p, Map);

            if (m != null)
                m.SendLocalizedMessage(1153053, bc.Name); // Pets are not permitted in this area. Your pet named ~1_NAME~ could not be sent to the stables, so has been teleported outside the event area.
        }

        public override bool CheckTravel(Mobile traveller, Point3D p, TravelCheckType type)
        {
            if (traveller.AccessLevel > AccessLevel.Player)
                return true;

            return type > TravelCheckType.Mark;
        }
    }
}
