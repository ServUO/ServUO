using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Engines.PartySystem;
using Server.Items;
using System.Linq;

namespace Server.Engines.Shadowguard
{
    [PropertyObject]
    public class ShadowguardInstance
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Center { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ShadowguardEncounter Encounter { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Index { get; set; }

        public ShadowguardRegion Region { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ShadowguardController Controller { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool InUse { get { return Encounter != null; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRoof { get { return Index >= 13; } }

        public ShadowguardInstance(ShadowguardController controller, Point3D center, Rectangle2D bounds, int index)
        {
            Controller = controller;
            Center = center;
            Index = index;

            Region = new ShadowguardRegion(bounds, index >= 13 ? String.Format("Roof {0}", (index - 12).ToString()) : index.ToString(), this);
            Region.Register();
        }

        public bool TryBeginEncounter(Mobile m, bool fromQueue, EncounterType type)
        {
            Party p = Party.Get(m);

            if (!fromQueue && p != null)
            {
                foreach (PartyMemberInfo info in p.Members)
                {
                    if (!Controller.Lobby.Contains(new Point2D(info.Mobile.X, info.Mobile.Y)))
                    {
                        m.SendLocalizedMessage(1156186); // All members of your party must remain in the lobby of Shadowguard 
                        // while your encounter is prepared. Make sure all members of your party 
                        // are in the lobby and try again. 
                        return false;
                    }
                }
            }

            Encounter = ConstructEncounter(type);
            Controller.AddEncounter(Encounter);
            Encounter.OnBeforeBegin(m);

            return true;
        }

        private ShadowguardEncounter ConstructEncounter(EncounterType type)
        {
            switch (type)
            {
                default:
                case EncounterType.Bar: return new BarEncounter(this);
                case EncounterType.Orchard: return new OrchardEncounter(this);
                case EncounterType.Armory: return new ArmoryEncounter(this);
                case EncounterType.Fountain: return new FountainEncounter(this);
                case EncounterType.Belfry: return new BelfryEncounter(this);
                case EncounterType.Roof: return new RoofEncounter(this);
            }
        }

        public void CompleteEncounter()
        {
            if (InUse)
            {
                Encounter = null;
            }
        }

        public void ClearRegion()
        {
            foreach (Item item in this.Region.GetEnumeratedItems())
            {
                Corpse corpse = item as Corpse;

                if (corpse != null)
                {
                    if (corpse.Owner is PlayerMobile)
                        corpse.MoveToWorld(Controller.KickLocation, Map.TerMur);
                    else
                        corpse.Delete();
                }
                else if (item is BaseAddon)
                {
                    ((BaseAddon)item).Internalize();
                }
                else if (item.Movable || IsInDeleteList(item))
                    item.Delete();
            }

            foreach (Mobile m in this.Region.GetEnumeratedMobiles())
            {
                if (m is BaseCreature && !(((BaseCreature)m).GetMaster() is PlayerMobile))
                    m.Delete();
            }
        }

        private bool IsInDeleteList(Item item)
        {
            if (item == null)
                return false;

            foreach (Type t in DeleteList)
            {
                if (item.GetType() == t)
                    return true;
            }

            return false;
        }

        private Type[] DeleteList =
        {
            typeof(ShadowguardCanal)
        };

        public static void Initialize()
        {
            if (ShadowguardController.Instance == null)
                return;

            ColUtility.ForEach(ShadowguardController.Instance.Addons.Where(addon => addon.Map != Map.Internal), addon =>
                {
                    ShadowguardInstance instance = ShadowguardController.GetInstance(addon.Location, addon.Map);

                    if (instance != null && !instance.InUse)
                        instance.ClearRegion();
                });
        }
    }
}