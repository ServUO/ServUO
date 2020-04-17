using Server.Accounting;
using Server.Engines.VeteranRewards;
using Server.Multis;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class LighthouseAddon : BaseAddon
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public string Account { get; set; }

        public Account LinkedAccount => Account == null ? null : Accounts.GetAccount(Account) as Account;

        public override BaseAddonDeed Deed => new LighthouseAddonDeed(Account);

        [Constructable]
        public LighthouseAddon(string account)
        {
            Account = account;
            Lighthouses.Add(this);

            AddComponent(new AddonComponent(18212), 0, 0, 0);
            AddComponent(new AddonComponent(18223), -1, 0, 0);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (Lighthouses.Contains(this))
                Lighthouses.Remove(this);
        }

        public override AddonFitResult CouldFit(IPoint3D p, Map map, Mobile from, ref BaseHouse house)
        {
            AddonFitResult result = base.CouldFit(p, map, from, ref house);

            if (result == AddonFitResult.Valid)
            {
                // Requires no roof being over it
                StaticTile[] staticTiles = map.Tiles.GetStaticTiles(p.X, p.Y, true);

                foreach (StaticTile tile in staticTiles)
                {
                    if (tile.Z > p.Z)
                        return AddonFitResult.Blocked;
                }

                if (from != null)
                    from.SendLocalizedMessage(1154596); // Ships placed by this account will now be linked to this lighthouse when they decay. Lost ships will be  found in your house moving crate.
            }

            return result;
        }

        public void DockBoat(BaseBoat boat, BaseHouse house)
        {
            foreach (IEntity entity in boat.GetEntitiesOnBoard())
            {
                if (!(entity is Item) || entity == this || boat.IsComponentItem(entity) || entity is EffectItem || entity == boat.TillerMan)
                    continue;

                Item item = entity as Item;

                if (!item.Deleted && boat.Contains(item))
                {
                    if (item is AddonComponent || item is BaseAddon)
                    {
                        BaseAddon addon = item is AddonComponent ? ((AddonComponent)item).Addon : (BaseAddon)item;
                        Item deed = addon.Deed;

                        bool retainDeedHue = false;
                        int hue = 0;

                        if (addon != null && addon.RetainDeedHue)
                        {
                            retainDeedHue = true;

                            for (int j = 0; hue == 0 && j < addon.Components.Count; ++j)
                            {
                                AddonComponent c = addon.Components[j];

                                if (c.Hue != 0)
                                    hue = c.Hue;
                            }
                        }

                        if (deed != null)
                        {
                            if (retainDeedHue)
                                deed.Hue = hue;

                            house.DropToMovingCrate(deed);
                        }
                    }
                    else
                    {
                        item.Delete();
                    }
                }

                Container cont;

                if (boat is BaseGalleon)
                    cont = ((BaseGalleon)boat).GalleonHold;
                else
                    cont = boat.Hold;

                if (cont != null)
                {
                    cont.Items.ForEach(i =>
                        {
                            if (i is BaseWeapon)
                                house.DropToMovingCrate(i);
                            else
                                i.Delete();
                        });
                }
            }

            BaseDockedBoat model = boat.BoatItem;

            if (model == null || model.Deleted)
                model = boat.DockedBoat;

            if (model == null)
                return;

            model.BoatItem = boat;

            if (boat.IsClassicBoat && boat.Owner != null)
                boat.RemoveKeys(boat.Owner);

            house.DropToMovingCrate(model);

            boat.OnDryDock(null);

            boat.Refresh();
            boat.Internalize();
        }

        public static void Configure()
        {
            Lighthouses = new List<LighthouseAddon>();
        }

        public static List<LighthouseAddon> Lighthouses { get; set; }

        public static LighthouseAddon GetLighthouse(Mobile m)
        {
            if (m == null)
                return null;

            Account a = m.Account as Account;

            return Lighthouses.FirstOrDefault(l => l != null && !l.Deleted && l.LinkedAccount == a && a != null);
        }

        public LighthouseAddon(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
            writer.Write(Account);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            Account = reader.ReadString();

            Lighthouses.Add(this);
        }
    }

    public class LighthouseAddonDeed : BaseAddonDeed, IRewardItem
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public string Account { get; set; }

        public Account LinkedAccount => Account == null ? null : Accounts.GetAccount(Account) as Account;

        public bool IsRewardItem
        {
            get { return true; }
            set { }
        }

        public override BaseAddon Addon => new LighthouseAddon(Account);
        public override int LabelNumber => 1154582;  // Deed for a Lighthouse

        [Constructable]
        public LighthouseAddonDeed() : this(null)
        {
        }

        public LighthouseAddonDeed(string account)
        {
            Account = account;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (LinkedAccount == null || LinkedAccount != from.Account as Account)
            {
                from.SendLocalizedMessage(1154595); // Your account is not linked to this lighthouse.
            }
            else if (RewardSystem.CheckIsUsableBy(from, this, null))
            {
                base.OnDoubleClick(from);
            }
        }

        public LighthouseAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write(Account);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Account = reader.ReadString();
        }
    }
}
