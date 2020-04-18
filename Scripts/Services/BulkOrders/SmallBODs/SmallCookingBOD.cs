using Server.Engines.Craft;
using System;
using System.Collections.Generic;

namespace Server.Engines.BulkOrders
{
    public class SmallCookingBOD : SmallBOD
    {
        public override BODType BODType => BODType.Cooking;

        [Constructable]
        public SmallCookingBOD()
        {
            SmallBulkEntry[] entries;
            bool nonexceptional = false;

            if (0.20 > Utility.RandomDouble())
            {
                nonexceptional = true;
                entries = SmallBulkEntry.CookingSmallsRegular;
            }
            else
            {
                entries = SmallBulkEntry.CookingSmalls;
            }

            if (entries.Length > 0)
            {
                int amountMax = Utility.RandomList(10, 15, 20);

                BulkMaterialType material;
                material = BulkMaterialType.None;

                SmallBulkEntry entry = entries[Utility.Random(entries.Length)];

                Hue = 1169;
                AmountMax = amountMax;
                Type = entry.Type;
                Number = entry.Number;
                Graphic = entry.Graphic;
                Material = material;
                RequireExceptional = !nonexceptional && Utility.RandomBool();
                GraphicHue = entry.Hue;
            }
        }

        public SmallCookingBOD(int amountCur, int amountMax, Type type, int number, int graphic, bool reqExceptional, BulkMaterialType mat, int hue)
        {
            Hue = 1169;
            AmountMax = amountMax;
            AmountCur = amountCur;
            Type = type;
            Number = number;
            Graphic = graphic;
            RequireExceptional = reqExceptional;
            Material = mat;
            GraphicHue = hue;
        }

        public SmallCookingBOD(Serial serial)
            : base(serial)
        {
        }

        private SmallCookingBOD(SmallBulkEntry entry, int amountMax)
        {
            Hue = 1169;
            AmountMax = amountMax;
            Type = entry.Type;
            Number = entry.Number;
            Graphic = entry.Graphic;
            GraphicHue = entry.Hue;
        }

        private SmallCookingBOD(SmallBulkEntry entry, int amountMax, bool reqExceptional)
        {
            Hue = 1169;
            AmountMax = amountMax;
            Type = entry.Type;
            Number = entry.Number;
            Graphic = entry.Graphic;
            GraphicHue = entry.Hue;
            RequireExceptional = reqExceptional;
        }

        public static SmallCookingBOD CreateRandomFor(Mobile m)
        {
            SmallBulkEntry[] entries;

            double theirSkill = BulkOrderSystem.GetBODSkill(m, SkillName.Cooking);
            bool nonexceptional = false;

            if (0.20 > Utility.RandomDouble())
            {
                nonexceptional = true;
                entries = SmallBulkEntry.CookingSmallsRegular;
            }
            else
            {
                entries = SmallBulkEntry.CookingSmalls;
            }

            if (entries.Length > 0)
            {
                int amountMax;

                if (theirSkill >= 70.1)
                    amountMax = Utility.RandomList(10, 15, 20, 20);
                else if (theirSkill >= 50.1)
                    amountMax = Utility.RandomList(10, 15, 15, 20);
                else
                    amountMax = Utility.RandomList(10, 10, 15, 20);

                double excChance = 0.0;

                if (theirSkill >= 70.1)
                    excChance = (theirSkill + 80.0) / 200.0;

                bool reqExceptional = !nonexceptional && excChance > Utility.RandomDouble();

                CraftSystem system = DefCooking.CraftSystem;

                List<SmallBulkEntry> validEntries = new List<SmallBulkEntry>();

                for (int i = 0; i < entries.Length; ++i)
                {
                    CraftItem item = system.CraftItems.SearchFor(entries[i].Type);

                    if (item != null)
                    {
                        bool allRequiredSkills = true;
                        double chance = item.GetSuccessChance(m, null, system, false, ref allRequiredSkills);

                        if (allRequiredSkills && chance >= 0.0)
                        {
                            if (chance > 0.0)
                                validEntries.Add(entries[i]);
                        }
                    }
                }

                if (validEntries.Count > 0)
                {
                    SmallBulkEntry entry = validEntries[Utility.Random(validEntries.Count)];
                    return new SmallCookingBOD(entry, amountMax, reqExceptional);
                }
            }

            return null;
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

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}