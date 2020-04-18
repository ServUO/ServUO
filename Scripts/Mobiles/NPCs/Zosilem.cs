using Server.Engines.BulkOrders;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
    public class Zosilem : MondainQuester, ITierQuester
    {
        public TierQuestInfo TierInfo => TierQuestInfo.Zosilem;

        [Constructable]
        public Zosilem()
            : base("Zosilem", "the Alchemist")
        {
            SetSkill(SkillName.Alchemy, 85.0, 100.0);
            SetSkill(SkillName.TasteID, 65.0, 88.0);
        }

        public Zosilem(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[] { };

        #region Bulk Orders
        public override BODType BODType => BODType.Alchemy;

        public override bool IsValidBulkOrder(Item item)
        {
            return (item is SmallAlchemyBOD || item is LargeAlchemyBOD);
        }

        public override bool SupportsBulkOrders(Mobile from)
        {
            return BulkOrderSystem.NewSystemEnabled && from is PlayerMobile && from.Skills[SkillName.Alchemy].Base > 0;
        }

        public override void OnSuccessfulBulkOrderReceive(Mobile from)
        {
            if (from is PlayerMobile)
                ((PlayerMobile)from).NextAlchemyBulkOrder = TimeSpan.Zero;
        }

        #endregion

        public override bool IsActiveVendor => true;

        protected override List<SBInfo> SBInfos => m_SBInfos;

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBAlchemist(this));
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Gargoyle;

            Hue = 0x86EA;
            HairItemID = 0x4273;
            HairHue = 0x323;
        }

        public override void InitOutfit()
        {
            AddItem(new FemaleGargishClothLegs(0x736));
            AddItem(new FemaleGargishClothKilt(0x73D));
            AddItem(new FemaleGargishClothChest(0x38B));
            AddItem(new FemaleGargishClothArms(0x711));
        }

        private static readonly Type[][] m_PileTypes = new Type[][]
            {
                new Type[] {typeof(DullCopperIngot),  typeof(PileofInspectedDullCopperIngots) },
                new Type[] {typeof(ShadowIronIngot),  typeof(PileofInspectedShadowIronIngots) },
                new Type[] {typeof(BronzeIngot),      typeof(PileofInspectedBronzeIngots) },
                new Type[] {typeof(GoldIngot),        typeof(PileofInspectedGoldIngots) },
                new Type[] {typeof(AgapiteIngot),     typeof(PileofInspectedAgapiteIngots) },
                new Type[] {typeof(VeriteIngot),      typeof(PileofInspectedVeriteIngots) },
                new Type[] {typeof(ValoriteIngot),    typeof(PileofInspectedValoriteIngots) }
            };

        private static readonly object[][] m_KegTypes = new object[][]
            {
                new object[] {PotionEffect.RefreshTotal,  typeof(InspectedKegofTotalRefreshment) },
                new object[] {PotionEffect.PoisonGreater, typeof(InspectedKegofGreaterPoison) }
            };

        private const int NeededIngots = 20;

        private Type GetPileType(Item item)
        {
            Type itemType = item.GetType();

            for (int i = 0; i < m_PileTypes.Length; i++)
            {
                Type[] pair = m_PileTypes[i];

                if (itemType == pair[0])
                    return pair[1];
            }

            return null;
        }

        private Type GetKegType(PotionEffect effect)
        {
            for (int i = 0; i < m_KegTypes.Length; i++)
            {
                object[] pair = m_KegTypes[i];

                if (effect == (PotionEffect)pair[0])
                    return (Type)pair[1];
            }

            return null;
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            Type inspectedType = null;
            bool success = false;

            inspectedType = GetPileType(item);

            if (inspectedType != null)
            {
                if (item.Amount > NeededIngots)
                {
                    SayTo(from, 1113037); // That's too many.
                }
                else if (item.Amount < NeededIngots)
                {
                    SayTo(from, 1113036); // That's not enough.
                }
                else
                {
                    success = true;
                }
            }
            else if (item is PotionKeg)
            {
                PotionKeg keg = (PotionKeg)item;

                inspectedType = GetKegType(keg.Type);

                if (inspectedType == null)
                {
                    SayTo(from, 1113039); // It is the wrong type.
                }
                else if (keg.Held < 100)
                {
                    SayTo(from, 1113038); // It is not full.
                }
                else
                {
                    success = true;
                }
            }
            else
            {
                if (item is Gold)
                {
                    base.CheckGold(from, item);
                }
                else
                {
                    SayTo(from, 1113035); // Oooh, shiney. I have no use for this, though.
                }
            }

            if (success)
            {
                SayTo(from, 1113040); // Good. I can use 

                from.AddToBackpack(Activator.CreateInstance(inspectedType) as Item);
                from.SendLocalizedMessage(1113041); // Now mark the inspected item as a quest item to turn it in.
            }

            return success;
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
