using System;
using System.Collections.Generic;

namespace Server.Engines.BulkOrders
{
    public class LargeTinkerBOD : LargeBOD
    {
        public override BODType BODType { get { return BODType.Tinkering; } }

        public static double[] m_BlackTinkerMaterialChances = new double[]
        {
            0.501953125, // None
            0.250000000, // Dull Copper
            0.125000000, // Shadow Iron
            0.062500000, // Copper
            0.031250000, // Bronze
            0.015625000, // Gold
            0.007812500, // Agapite
            0.003906250, // Verite
            0.001953125  // Valorite
        };

        [Constructable]
        public LargeTinkerBOD()
        {
            LargeBulkEntry[] entries;
            bool useMaterials = true;
			
            int rand = Utility.Random(4);

            switch ( rand )
            {
                default:
                case 0:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeDining);
                    break;
                case 1:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeJewelry);
                    break;
                case 2:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeKeyGlobe);
                    break;
                case 3:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeTools);
                    useMaterials = false;
                    break;
            }

            int amountMax = Utility.RandomList(10, 15, 20, 20);
            bool reqExceptional = (0.825 > Utility.RandomDouble());

            BulkMaterialType material;

            if (useMaterials)
                material = GetRandomMaterial(BulkMaterialType.DullCopper, m_BlackTinkerMaterialChances);
            else
                material = BulkMaterialType.None;

            this.Hue = 1109;
            this.AmountMax = amountMax;
            this.Entries = entries;
            this.RequireExceptional = reqExceptional;
            this.Material = material;
        }

        public LargeTinkerBOD(int amountMax, bool reqExceptional, BulkMaterialType mat, LargeBulkEntry[] entries)
        {
            this.Hue = 1109;
            this.AmountMax = amountMax;
            this.Entries = entries;
            this.RequireExceptional = reqExceptional;
            this.Material = mat;
        }

        public LargeTinkerBOD(Serial serial)
            : base(serial)
        {
        }

        public override int ComputeFame()
        {
            return TinkeringRewardCalculator.Instance.ComputeFame(this);
        }

        public override int ComputeGold()
        {
            return TinkeringRewardCalculator.Instance.ComputeGold(this);
        }

        public override List<Item> ComputeRewards(bool full)
        {
            List<Item> list = new List<Item>();

            RewardGroup rewardGroup = TinkeringRewardCalculator.Instance.LookupRewards(TinkeringRewardCalculator.Instance.ComputePoints(this));

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