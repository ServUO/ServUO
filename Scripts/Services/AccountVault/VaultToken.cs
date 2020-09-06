
using Server.AccountVault;
using Server.Mobiles;
using Server.Engines.UOStore;

namespace Server.Items
{
    public class VaultToken : Item, IAccountRestricted
    {
        public override int LabelNumber => 1070997;// A promotional token

        public string Account { get; set; }

        [Constructable]
        public VaultToken()
            : this(null)
        {
        }

        public VaultToken(string account)
            : base(0x2AAA)
        {
            Account = account;
        }

        public VaultToken(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (!SystemSettings.UseTokens)
                {
                    from.SendMessage("Account Vaults are not set up on this shard for Tokens. Contact staff regarding a refund.");
                }
                else
                {
                    var pm = from as PlayerMobile;

                    if (pm != null)
                    {
                        if (Account != null && (pm.Account == null || pm.Account.Username != Account))
                        {
                            pm.SendLocalizedMessage(1071296); // This item is Account Bound and your character is not bound to it. You cannot use this item.
                        }
                        else
                        {
                            var storeProf = UltimaStore.GetProfile(pm, true);

                            if (storeProf.VaultTokens >= SystemSettings.MaxTokenBalance)
                            {
                                pm.SendLocalizedMessage(1158194); // You cannot increase the number of vault credits on this character.
                            }
                            else
                            {
                                storeProf.VaultTokens++;

                                pm.SendLocalizedMessage(1158193, storeProf.VaultTokens.ToString());

                                Delete();
                                // You have increased your storage vault credit on this character. Your total credit count is now ~1_VAL~.
                                // Visit the nearest Vault Manager to rent a storage vault.
                            }
                        }
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1070998, "#1158192"); // Use this to redeem<br>your Storage Vault Credit (Account-Bound)
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
            reader.ReadInt();

            Account = reader.ReadString();
        }
    }
}
