using System;
using System.Collections.Generic;
using Server.Engines.Craft;

namespace Server.Engines.BulkOrders
{
    // carp 1512
    // bow 1425
    // cook 1169
    // scribe 2598
    // tink 1109
    public class SmallAlchemyBOD : SmallBOD
    {
        public override BODType BODType { get { return BODType.Alchemy; } }

        [Constructable]
        public SmallAlchemyBOD()
        {
            SmallBulkEntry[] entries = SmallBulkEntry.AlchemySmalls;

            if (entries.Length > 0)
            {
                int amountMax = Utility.RandomList(10, 15, 20);

                BulkMaterialType material;
                material = BulkMaterialType.None;

                SmallBulkEntry entry = entries[Utility.Random(entries.Length)];

                this.Hue = 2505;
                this.AmountMax = amountMax;
                this.Type = entry.Type;
                this.Number = entry.Number;
                this.Graphic = entry.Graphic;
                this.Material = material;
                this.GraphicHue = entry.Hue;
            }
        }

        public SmallAlchemyBOD(int amountCur, int amountMax, Type type, int number, int graphic, bool reqExceptional, BulkMaterialType mat, int hue)
        {
            this.Hue = 2505;
            this.AmountMax = amountMax;
            this.AmountCur = amountCur;
            this.Type = type;
            this.Number = number;
            this.Graphic = graphic;
            this.RequireExceptional = reqExceptional;
            this.Material = mat;
            this.GraphicHue = hue;
        }

        public SmallAlchemyBOD(Serial serial)
            : base(serial)
        {
        }

        private SmallAlchemyBOD(SmallBulkEntry entry, int amountMax)
        {
            this.Hue = 2505;
            this.AmountMax = amountMax;
            this.Type = entry.Type;
            this.Number = entry.Number;
            this.Graphic = entry.Graphic;
            this.GraphicHue = entry.Hue;
        }

        public static SmallAlchemyBOD CreateRandomFor(Mobile m)
        {
            SmallBulkEntry[] entries;

            double theirSkill = m.Skills[SkillName.Alchemy].Base;

            entries = SmallBulkEntry.AlchemySmalls;

            if (entries.Length > 0)
            {
                int amountMax;

                if (theirSkill >= 70.1)
                    amountMax = Utility.RandomList(10, 15, 20, 20);
                else if (theirSkill >= 50.1)
                    amountMax = Utility.RandomList(10, 15, 15, 20);
                else
                    amountMax = Utility.RandomList(10, 10, 15, 20);

                CraftSystem system = DefAlchemy.CraftSystem;

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
                    return new SmallAlchemyBOD(entry, amountMax);
                }
            }

            return null;
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}