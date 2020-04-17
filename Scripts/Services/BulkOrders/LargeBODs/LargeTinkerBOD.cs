using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Engines.BulkOrders
{
    public class LargeTinkerBOD : LargeBOD
    {
        public override BODType BODType => BODType.Tinkering;

        private GemType _GemType;

        [CommandProperty(AccessLevel.GameMaster)]
        public GemType GemType
        {
            get { return _GemType; }
            set
            {
                if (Entries.Length > 0 && Entries[0].Details != null && Entries[0].Details.Type != null && Entries[0].Details.Type.IsSubclassOf(typeof(BaseJewel)))
                {
                    _GemType = value;
                    AssignGemNumbers();

                    InvalidateProperties();
                }
            }
        }

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
            bool jewelry = false;

            int rand = Utility.Random(4);

            switch (rand)
            {
                default:
                case 0:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeDining);
                    break;
                case 1:
                    entries = LargeBulkEntry.ConvertEntries(this, LargeBulkEntry.LargeJewelry);
                    useMaterials = false;
                    jewelry = true;
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

            Hue = 1109;
            AmountMax = amountMax;
            Entries = entries;
            RequireExceptional = reqExceptional;
            Material = material;

            if (jewelry)
            {
                AssignGemType();
            }
        }

        public LargeTinkerBOD(int amountMax, bool reqExceptional, BulkMaterialType mat, LargeBulkEntry[] entries, GemType gemType)
        {
            Hue = 1109;
            AmountMax = amountMax;
            Entries = entries;
            RequireExceptional = reqExceptional;
            Material = mat;
            _GemType = gemType;
        }

        public override bool CheckType(SmallBOD small, Type type)
        {
            if (_GemType != GemType.None && (!(small is SmallTinkerBOD) || ((SmallTinkerBOD)small).GemType != _GemType))
            {
                return false;
            }

            return base.CheckType(small, type);
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

        public void AssignGemType()
        {
            _GemType = (GemType)Utility.RandomMinMax(1, 9);
            AssignGemNumbers();
        }

        public void AssignGemNumbers()
        {
            foreach (LargeBulkEntry entry in Entries)
            {
                Type jewelType = entry.Details.Type;

                int offset = (int)GemType - 1;
                int loc = 0;

                if (jewelType == typeof(GoldRing) || jewelType == typeof(SilverRing))
                {
                    loc = 1044176;
                }
                else if (jewelType == typeof(GoldBracelet) || jewelType == typeof(SilverBracelet))
                {
                    loc = 1044221;
                }
                else
                {
                    loc = 1044203;
                }

                entry.Details.Number = loc + offset;
            }

            //this.Number = loc + (int)gemType - 1;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write((int)_GemType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    _GemType = (GemType)reader.ReadInt();
                    break;
            }

            if (version < 1 && Entries != null && Entries.Length > 0 && Entries[0].Details != null)
            {
                Type t = Entries[0].Details.Type;

                if (SmallTinkerBOD.CannotAssignMaterial(t) && Material != BulkMaterialType.None)
                {
                    Material = BulkMaterialType.None;
                }

                if (t.IsSubclassOf(typeof(BaseJewel)))
                {
                    AssignGemType();
                }
            }
        }
    }
}