using System.Collections.Generic;

using Server.Mobiles;
using Server.Items;
using Server.ContextMenus;
using Server.Gumps;

namespace Server.AccountVault
{
    public class VaultManager : Banker
    {
        public override bool IsActiveVendor => false;
        public override bool ChangeRace => false;

        [Constructable]
        public VaultManager()
        {
            Title = "the Vault Manager";
            CantWalk = true;
        }

        public VaultManager(Serial serial)
            : base(serial)
        {
        }

        public override void InitOutfit()
        {
            if (Backpack == null)
            {
                Item backpack = new Backpack
                {
                    Movable = false
                };
                AddItem(backpack);
            }

            SetWearable(new ElvenShirt());
            SetWearable(new GuildedKilt(), 1436);
            SetWearable(new Sandals());
            SetWearable(new GoldNecklace());

            int hairHue = GetHairHue();

            Utility.AssignRandomHair(this, hairHue);
            Utility.AssignRandomFacialHair(this, hairHue);

            Utility.AssignRandomHair(this, hairHue);
            Utility.AssignRandomFacialHair(this, hairHue);

            if (Body == 0x191)
            {
                FacialHairItemID = 0;
            }
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.AddCustomContextEntries(from, list);

            if (SystemSettings.Enabled && from.Alive && from.InRange(Location, 8))
            {
                var vault = AccountVault.GetVault(from);
                var inRange = vault != null && from.Region.IsPartOf(this.Region);

                var open = new OpenVaultEntry(this)
                {
                    Enabled = vault != null && inRange && !from.Criminal
                };
                list.Add(open);

                var rent = new RentVaultEntry(this)
                {
                    Enabled = vault == null
                };
                list.Add(rent);

                var claim = new ClaimVaultEntry(this)
                {
                    Enabled = vault != null && inRange
                };
                list.Add(claim);

                var actions = new VaultActionsEntry(this, vault)
                {
                    Enabled = vault != null && inRange
                };
                list.Add(actions);

                var locations = new VaultLocationsEntry(this);
                list.Add(locations);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version
        }

        public class OpenVaultEntry : ContextMenuEntry
        {
            public VaultManager Manager { get; private set; }

            public OpenVaultEntry(VaultManager manager)
                : base(1157734, 8)
            {
                Manager = manager;
            }

            public override void OnClick()
            {
                var from = Owner.From;

                if (from != null)
                {
                    var vault = AccountVault.GetVault(from);

                    if (vault != null && from.Region.IsPartOf(Manager.Region))
                    {
                        if (from.Criminal)
                        {
                            from.SendLocalizedMessage(1158195); // Thou art a criminal and cannot access thy vault.
                        }
                        else if (!from.InRange(vault.GetWorldLocation(), 2))
                        {
                            vault.MoveTo(from);
                        }
                        else
                        {
                            vault.OnDoubleClick(from);
                        }
                    }
                }
            }
        }

        public class RentVaultEntry : ContextMenuEntry
        {
            public VaultManager Manager { get; private set; }

            public RentVaultEntry(VaultManager manager)
                 : base(1157733, 8)
            {
                Manager = manager;
            }

            public override void OnClick()
            {
                var pm = Owner.From as PlayerMobile;

                if (pm != null)
                {
                    if (!AccountVault.HasVault(pm))
                    {
                        AccountVault.TryRentVault(pm, Manager);
                    }
                    else
                    {
                        pm.SendMessage("Your account already has an account vault.");
                    }
                }
            }
        }

        public class ClaimVaultEntry : ContextMenuEntry
        {
            public VaultManager Manager { get; private set; }

            public ClaimVaultEntry(VaultManager manager)
                : base(1157735, 8)
            {
                Manager = manager;
            }

            public override void OnClick()
            {
                var pm = Owner.From as PlayerMobile;

                if (pm != null)
                {
                    var vault = AccountVault.GetVault(pm);

                    if (vault != null)
                    {
                        /*Are you sure you want to claim this vault? Doing so will forfeit any rental
                         * time remaining and place your vault contents in your backpack.*/

                        BaseGump.SendGump(new PetTrainingStyleConfirmGump(pm, 1074974, 1158037, () =>
                        {
                            vault.ClaimVault(pm);
                        }));
                    }
                    else
                    {
                        pm.SendLocalizedMessage(1158034); // You don’t have a vault to claim.
                    }
                }
            }
        }

        public class VaultActionsEntry : ContextMenuEntry
        {
            public VaultManager Manager { get; private set; }
            public AccountVault Vault { get; private set; }

            public VaultActionsEntry(VaultManager manager, AccountVault vault)
                : base(1157978, 8)
            {
                Manager = manager;
                Vault = vault;
            }

            public override void OnClick()
            {
                var pm = Owner.From as PlayerMobile;

                if (pm != null && Vault != null)
                {
                    BaseGump.SendGump(new VaultActionsGump(pm, Vault));
                }
            }
        }

        public class VaultLocationsEntry : ContextMenuEntry
        {
            public VaultManager Manager { get; private set; }

            public VaultLocationsEntry(VaultManager manager)
                : base(1158143, 8)
            {
                Manager = manager;
            }

            public override void OnClick()
            {
                var pm = Owner.From as PlayerMobile;

                if (pm != null)
                {
                    BaseGump.SendGump(new VaultLocationsGump(pm));
                }
            }
        }
    }
}
