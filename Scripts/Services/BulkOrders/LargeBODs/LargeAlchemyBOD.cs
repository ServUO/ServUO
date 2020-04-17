using System.Collections.Generic;

namespace Server.Engines.BulkOrders
{
    public class LargeAlchemyBOD : LargeBOD
    {
        public override BODType BODType => BODType.Alchemy;

        [Constructable]
        public LargeAlchemyBOD()
        {
            LargeBulkEntry[] entries;

            switch (Utility.Random(5))
            {
                default:
                case 0:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeExplosive);
                    break;
                case 1:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeGreater);
                    break;
                case 2:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeLesser);
                    break;
                case 3:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeRegular);
                    break;
                case 4:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeToxic);
                    break;
            }

            int amountMax = Utility.RandomList(10, 15, 20, 20);

            Hue = 2505;
            AmountMax = amountMax;
            Entries = entries;
        }

        public LargeAlchemyBOD(int amountMax, bool reqExceptional, BulkMaterialType mat, LargeBulkEntry[] entries)
        {
            Hue = 2505;
            AmountMax = amountMax;
            Entries = entries;
            RequireExceptional = reqExceptional;
            Material = mat;
        }

        public LargeAlchemyBOD(Serial serial)
            : base(serial)
        {
        }

        public override int ComputeFame()
        {
            return AlchemyRewardCalculator.Instance.ComputeFame(this);
        }

        public override int ComputeGold()
        {
            return AlchemyRewardCalculator.Instance.ComputeGold(this);
        }

        public override List<Item> ComputeRewards(bool full)
        {
            List<Item> list = new List<Item>();

            RewardGroup rewardGroup = AlchemyRewardCalculator.Instance.LookupRewards(AlchemyRewardCalculator.Instance.ComputePoints(this));

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

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}