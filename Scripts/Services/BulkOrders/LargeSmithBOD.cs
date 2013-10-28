using System;
using System.Collections.Generic;

namespace Server.Engines.BulkOrders
{
    [TypeAlias("Scripts.Engines.BulkOrders.LargeSmithBOD")]
    public class LargeSmithBOD : LargeBOD
    {
        public static double[] m_BlacksmithMaterialChances = new double[]
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
        public LargeSmithBOD()
        {
            LargeBulkEntry[] entries;
            bool useMaterials = true;
			
            int rand = Utility.Random(8);

            switch ( rand )
            {
                default:
                case 0:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeRing);
                    break;
                case 1:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargePlate);
                    break;
                case 2:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeChain);
                    break;
                case 3:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeAxes);
                    break;
                case 4:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeFencing);
                    break;
                case 5:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeMaces);
                    break;
                case 6:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargePolearms);
                    break;
                case 7:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeSwords);
                    break;
            }
			
            if (rand > 2 && rand < 8)
                useMaterials = false;

            int hue = 0x44E;
            int amountMax = Utility.RandomList(10, 15, 20, 20);
            bool reqExceptional = (0.825 > Utility.RandomDouble());

            BulkMaterialType material;

            if (useMaterials)
                material = GetRandomMaterial(BulkMaterialType.DullCopper, m_BlacksmithMaterialChances);
            else
                material = BulkMaterialType.None;

            this.Hue = hue;
            this.AmountMax = amountMax;
            this.Entries = entries;
            this.RequireExceptional = reqExceptional;
            this.Material = material;
        }

        public LargeSmithBOD(int amountMax, bool reqExceptional, BulkMaterialType mat, LargeBulkEntry[] entries)
        {
            this.Hue = 0x44E;
            this.AmountMax = amountMax;
            this.Entries = entries;
            this.RequireExceptional = reqExceptional;
            this.Material = mat;
        }

        public LargeSmithBOD(Serial serial)
            : base(serial)
        {
        }

        public override int ComputeFame()
        {
            return SmithRewardCalculator.Instance.ComputeFame(this);
        }

        public override int ComputeGold()
        {
            return SmithRewardCalculator.Instance.ComputeGold(this);
        }

        public override List<Item> ComputeRewards(bool full)
        {
            List<Item> list = new List<Item>();

            RewardGroup rewardGroup = SmithRewardCalculator.Instance.LookupRewards(SmithRewardCalculator.Instance.ComputePoints(this));

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