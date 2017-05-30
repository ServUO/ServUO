using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Guilds;
using System.Collections.Generic;

namespace Server.Engines.VvV
{
    public class SilverTrader : BaseVendor
    {
        public override bool IsActiveVendor { get { return false; } }
        public override bool DisallowAllMoves { get { return true; } }
        public override bool ClickTitle { get { return true; } }
        public override bool CanTeach { get { return false; } }

        protected List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return this.m_SBInfos; } }
        public override void InitSBInfo() { }

        [Constructable]
        public SilverTrader() : base("the Silver Trader")
        {
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = NameList.RandomName("male");

            SpeechHue = 0x3B2;
            Hue = Utility.RandomSkinHue();
            Body = 0x190;
        }

        public override void InitOutfit()
        {
            Robe robe = new Robe();
            robe.ItemID = 0x2684;
            robe.Name = "a robe";

            SetWearable(robe, 1109);

            Timer.DelayCall(TimeSpan.FromSeconds(10), StockInventory);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1155513); // Vice vs Virtue Reward Vendor
        }

        private DateTime _NextSpeak;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (_NextSpeak < DateTime.UtcNow && ViceVsVirtueSystem.IsVvV(m) && InRange(m.Location, 6) && m.Race == Race.Gargoyle)
            {
                SayTo(m, 1155534); // I will convert your human artifacts to gargoyle versions if you hand them to me.
                _NextSpeak = DateTime.UtcNow + TimeSpan.FromSeconds(25);
            }
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (ViceVsVirtueSystem.Enabled && m is PlayerMobile && InRange(m.Location, 3))
            {
                if (ViceVsVirtueSystem.IsVvV(m))
                {
                    m.SendGump(new VvVRewardGump(this, (PlayerMobile)m));
                }
                else
                {
                    SayTo(m, 1155585); // You have no silver to trade with. Join Vice vs Virtue and return to me.
                }
            }
        }

        public void StockInventory()
        {
            if (Backpack == null)
                AddItem(new Backpack());

            foreach (CollectionItem item in VvVRewards.Rewards)
            {
                if (item.Tooltip == 0)
                {
                    if (Backpack.GetAmount(item.Type) > 0)
                    {
                        Item itm = Backpack.FindItemByType(item.Type);

                        if (itm is IVvVItem)
                            ((IVvVItem)itm).IsVvVItem = true;

                        continue;
                    }

                    Item i = Activator.CreateInstance(item.Type) as Item;

                    if (i != null)
                    {
                        if (i is IOwnerRestricted)
                            ((IOwnerRestricted)i).OwnerName = "Your Player Name";

                        if (i is IVvVItem)
                            ((IVvVItem)i).IsVvVItem = true;

                        NegativeAttributes neg = RunicReforging.GetNegativeAttributes(i);

                        if (neg != null)
                        {
                            neg.Antique = 1;

                            if (i is IDurability && ((IDurability)i).MaxHitPoints == 0)
                            {
                                ((IDurability)i).MaxHitPoints = 255;
                                ((IDurability)i).HitPoints = 255;
                            }
                        }

                        ViceVsVirtueSystem.Instance.AddVvVItem(i);

                        Backpack.DropItem(i);
                    }
                }
            }
        }

        private Type[][] _Table =
        {
            new Type[] { typeof(CrimsonCincture), typeof(GargishCrimsonCincture) },
            new Type[] { typeof(MaceAndShieldGlasses), typeof(GargishMaceAndShieldGlasses) },
            new Type[] { typeof(WizardsCrystalGlasses), typeof(GargishWizardsCrystalGlasses) },
            new Type[] { typeof(FoldedSteelGlasses), typeof(GargishFoldedSteelGlasses) },
        };

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (ViceVsVirtueSystem.IsVvV(from))
            {
                if (!(dropped is IOwnerRestricted) || ((IOwnerRestricted)dropped).Owner == from)
                {
                    if (dropped is IVvVItem && from.Race == Race.Gargoyle)
                    {
                        foreach (var t in _Table)
                        {
                            if (dropped.GetType() == t[0])
                            {
                                IDurability dur = dropped as IDurability;

                                if (dur != null && dur.MaxHitPoints == 255 && dur.HitPoints == 255)
                                {
                                    var item = Loot.Construct(t[1]);

                                    if (item != null)
                                    {
                                        VvVRewards.OnRewardItemCreated(from, item);

                                        if (item is GargishCrimsonCincture)
                                        {
                                            ((GargishCrimsonCincture)item).Attributes.BonusDex = 10;
                                        }

                                        if (item is GargishMaceAndShieldGlasses)
                                        {
                                            ((GargishMaceAndShieldGlasses)item).Attributes.WeaponDamage = 10;
                                        }

                                        if (item is GargishFoldedSteelGlasses)
                                        {
                                            ((GargishFoldedSteelGlasses)item).Attributes.DefendChance = 25;
                                        }

                                        if (item is GargishWizardsCrystalGlasses)
                                        {
                                            ((GargishWizardsCrystalGlasses)item).PhysicalBonus = 5;                                            
                                            ((GargishWizardsCrystalGlasses)item).FireBonus = 5;                                            
                                            ((GargishWizardsCrystalGlasses)item).ColdBonus = 5;                                            
                                            ((GargishWizardsCrystalGlasses)item).PoisonBonus = 5;                                            
                                            ((GargishWizardsCrystalGlasses)item).EnergyBonus = 5;
                                        }

                                        from.AddToBackpack(item);
                                        dropped.Delete();

                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            SayTo(from, 1157365); // I'm sorry, I cannot accept this item.
            return false;
        }

        public SilverTrader(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Timer.DelayCall(TimeSpan.FromSeconds(5), StockInventory);
        }
    }
}