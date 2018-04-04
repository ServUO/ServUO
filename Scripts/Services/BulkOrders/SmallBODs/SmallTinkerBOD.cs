using System;
using System.Collections.Generic;
using Server.Engines.Craft;
using Server.Items;

namespace Server.Engines.BulkOrders
{
    public class SmallTinkerBOD : SmallBOD
    {
        public override BODType BODType { get { return BODType.Tinkering; } }

        public static double[] m_TinkerMaterialChances = new double[]
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
        public SmallTinkerBOD()
        {
            SmallBulkEntry[] entries;
            bool useMaterials;

            if (useMaterials = 0.75 > Utility.RandomDouble())
                entries = SmallBulkEntry.TinkeringSmalls;
            else
                entries = SmallBulkEntry.TinkeringSmallsRegular;

            if (entries.Length > 0)
            {
                int amountMax = Utility.RandomList(10, 15, 20);

                BulkMaterialType material;

                if (useMaterials)
                    material = GetRandomMaterial(BulkMaterialType.DullCopper, m_TinkerMaterialChances);
                else
                    material = BulkMaterialType.None;

                bool reqExceptional = useMaterials ? Utility.RandomBool() : false;

                SmallBulkEntry entry = entries[Utility.Random(entries.Length)];

                this.Hue = 1109;
                this.AmountMax = amountMax;
                this.Type = entry.Type;
                this.Number = entry.Number;
                this.Graphic = entry.Graphic;
                this.RequireExceptional = reqExceptional;
                this.Material = material;
                this.GraphicHue = entry.Hue;
            }
        }

        public SmallTinkerBOD(int amountCur, int amountMax, Type type, int number, int graphic, bool reqExceptional, BulkMaterialType mat, int hue)
        {
            this.Hue = 1109;
            this.AmountMax = amountMax;
            this.AmountCur = amountCur;
            this.Type = type;
            this.Number = number;
            this.Graphic = graphic;
            this.RequireExceptional = reqExceptional;
            this.Material = mat;
            this.GraphicHue = hue;
        }

        public SmallTinkerBOD(Serial serial)
            : base(serial)
        {
        }

        private SmallTinkerBOD(SmallBulkEntry entry, BulkMaterialType material, int amountMax, bool reqExceptional)
        {
            this.Hue = 1109;
            this.AmountMax = amountMax;
            this.Type = entry.Type;
            this.Number = entry.Number;
            this.Graphic = entry.Graphic;
            this.RequireExceptional = reqExceptional;
            this.Material = material;
        }

        public static SmallTinkerBOD CreateRandomFor(Mobile m)
        {
            SmallBulkEntry[] entries;
            bool useMaterials;

            if (useMaterials = 0.75 > Utility.RandomDouble())
                entries = SmallBulkEntry.TinkeringSmalls;
            else
                entries = SmallBulkEntry.TinkeringSmallsRegular;

            if (entries.Length > 0)
            {
                double theirSkill = BulkOrderSystem.GetBODSkill(m, SkillName.Tinkering);
                int amountMax;

                if (theirSkill >= 70.1)
                    amountMax = Utility.RandomList(10, 15, 20, 20);
                else if (theirSkill >= 50.1)
                    amountMax = Utility.RandomList(10, 15, 15, 20);
                else
                    amountMax = Utility.RandomList(10, 10, 15, 20);

                BulkMaterialType material = BulkMaterialType.None;

                if (useMaterials && theirSkill >= 70.1)
                {
                    for (int i = 0; i < 20; ++i)
                    {
                        BulkMaterialType check = GetRandomMaterial(BulkMaterialType.DullCopper, m_TinkerMaterialChances);
                        double skillReq = GetRequiredSkill(check);

                        if (theirSkill >= skillReq)
                        {
                            material = check;
                            break;
                        }
                    }
                }

                double excChance = 0.0;

                if (useMaterials && theirSkill >= 70.1)
                    excChance = (theirSkill + 80.0) / 200.0;

                bool reqExceptional = (excChance > Utility.RandomDouble());

                CraftSystem system = DefTinkering.CraftSystem;

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
                            if (reqExceptional)
                                chance = item.GetExceptionalChance(system, chance, m);

                            if (chance > 0.0)
                                validEntries.Add(entries[i]);
                        }
                    }
                }

                if (validEntries.Count > 0)
                {
                    SmallBulkEntry entry = validEntries[Utility.Random(validEntries.Count)];

                    if (entry.Type.IsSubclassOf(typeof(BaseTool)) && material != BulkMaterialType.None)
                        material = BulkMaterialType.None;

                    return new SmallTinkerBOD(entry, material, amountMax, reqExceptional);
                }
            }

            return null;
        }

        public override bool CheckType(Type itemType)
        {
            bool check = base.CheckType(itemType);

            if (!check)
            {
                check = CheckTinkerType(itemType, this.Type);
            }

            return check;
        }

        /* Tinkering needs conditional check for combining:
        * SpoonLeft/SpoonRight, ForkLeft/ForkRight, KnifeLeft/KnifeRight, ClockRight/ClockLeft
        */
        private static Type[][] _TinkerTypeTable =
        {
            new Type[] { typeof(Spoon), typeof(SpoonRight), typeof(SpoonLeft) },
            new Type[] { typeof(Fork), typeof(ForkRight), typeof(ForkLeft) },
            new Type[] { typeof(Knife), typeof(KnifeRight), typeof(KnifeLeft) },
            new Type[] { typeof(Clock), typeof(ClockRight), typeof(ClockLeft) },
            new Type[] { typeof(GoldRing), typeof(SilverRing) },
            new Type[] { typeof(GoldBracelet), typeof(SilverBracelet) },
            new Type[] { typeof(SmithHammer), typeof(SmithyHammer) }
        };

        public static bool CheckTinkerType(Type actual, Type lookingfor)
        {
            foreach (Type[] types in _TinkerTypeTable)
            {
                foreach (Type t in types)
                {
                    if (t == lookingfor) // found the list, lets see if the actual is here
                    {
                        foreach (Type t2 in types)
                        {
                            if (t2 == actual)
                            {
                                return true;
                            }
                        }
                    }
                }

                /*if (types[0] == lookingfor)
                {
                    foreach (Type t in types)
                    {
                        if (actual == t)
                            return true;
                    }
                }*/
            }

            return false;
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