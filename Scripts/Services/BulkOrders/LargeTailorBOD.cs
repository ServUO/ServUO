using System;
using System.Collections.Generic;

namespace Server.Engines.BulkOrders
{
    public class LargeTailorBOD : LargeBOD
    {
        public static double[] m_TailoringMaterialChances = new double[]
        {
            0.857421875, // None
            0.125000000, // Spined
            0.015625000, // Horned
            0.001953125  // Barbed
        };
        [Constructable]
        public LargeTailorBOD()
        {
            LargeBulkEntry[] entries;
            bool useMaterials = false;

            switch ( Utility.Random(14) )
            {
                default:
                case 0:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Farmer);
                    break;
                case 1:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.FemaleLeatherSet);
                    useMaterials = true;
                    break;
                case 2:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.FisherGirl);
                    break;
                case 3:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Gypsy);
                    break;
                case 4:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.HatSet);
                    break;
                case 5:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Jester);
                    break;
                case 6:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Lady);
                    break;
                case 7:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.MaleLeatherSet);
                    useMaterials = true;
                    break;
                case 8:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Pirate);
                    break;
                case 9:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.ShoeSet);
                    useMaterials = Core.ML;
                    break;
                case 10:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.StuddedSet);
                    useMaterials = true;
                    break;
                case 11:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.TownCrier);
                    break;
                case 12:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.Wizard);
                    break;
                case 13:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.BoneSet);
                    useMaterials = true;
                    break;
            }

            int hue = 0x483;
            int amountMax = Utility.RandomList(10, 15, 20, 20);
            bool reqExceptional = (0.825 > Utility.RandomDouble());

            BulkMaterialType material;

            if (useMaterials)
                material = GetRandomMaterial(BulkMaterialType.Spined, m_TailoringMaterialChances);
            else
                material = BulkMaterialType.None;

            this.Hue = hue;
            this.AmountMax = amountMax;
            this.Entries = entries;
            this.RequireExceptional = reqExceptional;
            this.Material = material;
        }

        public LargeTailorBOD(int amountMax, bool reqExceptional, BulkMaterialType mat, LargeBulkEntry[] entries)
        {
            this.Hue = 0x483;
            this.AmountMax = amountMax;
            this.Entries = entries;
            this.RequireExceptional = reqExceptional;
            this.Material = mat;
        }

        public LargeTailorBOD(Serial serial)
            : base(serial)
        {
        }

        public override int ComputeFame()
        {
            return TailorRewardCalculator.Instance.ComputeFame(this);
        }

        public override int ComputeGold()
        {
            return TailorRewardCalculator.Instance.ComputeGold(this);
        }

        public override List<Item> ComputeRewards(bool full)
        {
            List<Item> list = new List<Item>();

            RewardGroup rewardGroup = TailorRewardCalculator.Instance.LookupRewards(TailorRewardCalculator.Instance.ComputePoints(this));

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