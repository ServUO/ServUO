using System;
using System.Collections.Generic;

namespace Server.Engines.BulkOrders
{
    public class LargeCookingBOD : LargeBOD
    {
        public override BODType BODType { get { return BODType.Cooking; } }

        [Constructable]
        public LargeCookingBOD()
        {
            LargeBulkEntry[] entries;
            bool nonexceptional = false;

            switch (Utility.Random(7))
            {
                default:
                case 0:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeBarbeque);
                    nonexceptional = true;
                    break;
                case 1:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeDough);
                    break;
                case 2:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeFruits);
                    nonexceptional = true;
                    break;
                case 3:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeMiso);
                    break;
                case 4:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeSushi);
                    break;
                case 5:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeSweets);
                    break;
                case 6:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeUnbakedPies);
                    break;
            }

            this.Hue = 1169;
            this.AmountMax = Utility.RandomList(10, 15, 20, 20);
            this.Entries = entries;
            this.RequireExceptional = !nonexceptional && 0.825 > Utility.RandomDouble();
        }

        public LargeCookingBOD(int amountMax, bool reqExceptional, BulkMaterialType mat, LargeBulkEntry[] entries)
        {
            this.Hue = 1169;
            this.AmountMax = amountMax;
            this.Entries = entries;
            this.RequireExceptional = reqExceptional;
            this.Material = mat;
        }

        public LargeCookingBOD(Serial serial)
            : base(serial)
        {
        }

        public override int ComputeFame()
        {
            return CookingRewardCalculator.Instance.ComputeFame(this);
        }

        public override int ComputeGold()
        {
            return CookingRewardCalculator.Instance.ComputeGold(this);
        }

        public override List<Item> ComputeRewards(bool full)
        {
            List<Item> list = new List<Item>();

            RewardGroup rewardGroup = CookingRewardCalculator.Instance.LookupRewards(CookingRewardCalculator.Instance.ComputePoints(this));

            if (rewardGroup != null)
            {
                if (full)
                {
                    for (int i = 0; i < rewardGroup.Items.Length; ++i)
                    {
                        Item item = rewardGroup.Items[i].Construct();

                        if (item != null)
                            list.Add(item);
                    }
                }
                else
                {
                    RewardItem rewardItem = rewardGroup.AcquireItem();

                    if (rewardItem != null)
                    {
                        Item item = rewardItem.Construct();

                        if (item != null)
                            list.Add(item);
                    }
                }
            }

            return list;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}