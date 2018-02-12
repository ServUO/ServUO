using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Factions
{
    public static class FactionEquipment
    {
        public static bool CanUse(IFactionItem item, Mobile m, int failmessage = 500294)
        {
            if (item == null)
                return true;

            var state = PlayerState.Find(m);

            if (state != null && state.Faction == item.FactionItemState.Faction)
            {
                return true;
            }

            if (failmessage > 0)
            {
                m.SendLocalizedMessage(failmessage); // You cannot use that.
            }

            return false;
        }

        public static bool AddFactionProperties(IFactionItem item, ObjectPropertyList list)
        {
            if (item.FactionItemState != null)
            {
                list.Add(1041350); // faction item

                if (item.FactionItemState.MinRank > 0)
                    list.Add(1094805, item.FactionItemState.MinRank.ToString()); // Faction Rank: ~1_VALUE~

                return true;
            }

            return false;
        }

        public static void CheckProperties(Item item)
        {
            if (item is PrimerOnArmsTalisman && ((PrimerOnArmsTalisman)item).Attributes.AttackChance != 10)
            {
                ((PrimerOnArmsTalisman)item).Attributes.AttackChance = 10;
            }

            if (item is ClaininsSpellbook && ((ClaininsSpellbook)item).Attributes.LowerManaCost != 10)
            {
                ((ClaininsSpellbook)item).Attributes.LowerManaCost = 10;
            }

            if (item is CrimsonCincture && ((CrimsonCincture)item).Attributes.BonusDex != 10)
            {
                ((CrimsonCincture)item).Attributes.BonusDex = 10;
            }

            if (item is CrystallineRing && ((CrystallineRing)item).Attributes.CastRecovery != 3)
            {
                ((CrystallineRing)item).Attributes.CastRecovery = 3;
            }

            if (item is FeyLeggings)
            {
                if (((FeyLeggings)item).PhysicalBonus != 3)
                    ((FeyLeggings)item).PhysicalBonus = 3;

                if (((FeyLeggings)item).FireBonus != 3)
                    ((FeyLeggings)item).FireBonus = 3;

                if (((FeyLeggings)item).ColdBonus != 3)
                    ((FeyLeggings)item).ColdBonus = 3;

                if (((FeyLeggings)item).EnergyBonus != 3)
                    ((FeyLeggings)item).EnergyBonus = 3;
            }

            if (item is FoldedSteelGlasses && ((FoldedSteelGlasses)item).Attributes.DefendChance != 25)
            {
                ((FoldedSteelGlasses)item).Attributes.DefendChance = 25;
            }

            if (item is HeartOfTheLion)
            {
                if (((HeartOfTheLion)item).PhysicalBonus != 5)
                    ((HeartOfTheLion)item).PhysicalBonus = 5;

                if (((HeartOfTheLion)item).FireBonus != 5)
                    ((HeartOfTheLion)item).FireBonus = 5;

                if (((HeartOfTheLion)item).ColdBonus != 5)
                    ((HeartOfTheLion)item).ColdBonus = 5;

                if (((HeartOfTheLion)item).PoisonBonus != 5)
                    ((HeartOfTheLion)item).PoisonBonus = 5;

                if (((HeartOfTheLion)item).EnergyBonus != 5)
                    ((HeartOfTheLion)item).EnergyBonus = 5;
            }

            if (item is HuntersHeaddress)
            {
                if (((HuntersHeaddress)item).Resistances.Physical != 8)
                    ((HuntersHeaddress)item).Resistances.Physical = 8;

                if (((HuntersHeaddress)item).Resistances.Fire != 4)
                    ((HuntersHeaddress)item).Resistances.Fire = 4;

                if (((HuntersHeaddress)item).Resistances.Cold != -8)
                    ((HuntersHeaddress)item).Resistances.Cold = -8;

                if (((HuntersHeaddress)item).Resistances.Poison != 9)
                    ((HuntersHeaddress)item).Resistances.Poison = 9;

                if (((HuntersHeaddress)item).Resistances.Energy != 3)
                    ((HuntersHeaddress)item).Resistances.Energy = 3;
            }

            if (item is KasaOfTheRajin && ((KasaOfTheRajin)item).Attributes.DefendChance != 10)
            {
                ((KasaOfTheRajin)item).Attributes.DefendChance = 10;
            }

            if (item is MaceAndShieldGlasses && ((MaceAndShieldGlasses)item).Attributes.WeaponDamage != 10)
            {
                ((MaceAndShieldGlasses)item).Attributes.WeaponDamage = 10;
            }

            if (item is VesperOrderShield && ((VesperOrderShield)item).Attributes.CastSpeed != 0)
            {
                ((VesperOrderShield)item).Attributes.CastSpeed = 0;

                if (item.Name != "Order Shield")
                    item.Name = "Order Shield";
            }

            if (item is OrnamentOfTheMagician && ((OrnamentOfTheMagician)item).Attributes.RegenMana != 3)
            {
                ((OrnamentOfTheMagician)item).Attributes.RegenMana = 3;
            }

            if (item is RingOfTheVile && ((RingOfTheVile)item).Attributes.AttackChance != 25)
            {
                ((RingOfTheVile)item).Attributes.AttackChance = 25;
            }

            if (item is RuneBeetleCarapace)
            {
                if (((RuneBeetleCarapace)item).PhysicalBonus != 3)
                    ((RuneBeetleCarapace)item).PhysicalBonus = 3;

                if (((RuneBeetleCarapace)item).FireBonus != 3)
                    ((RuneBeetleCarapace)item).FireBonus = 3;

                if (((RuneBeetleCarapace)item).ColdBonus != 3)
                    ((RuneBeetleCarapace)item).ColdBonus = 3;

                if (((RuneBeetleCarapace)item).PoisonBonus != 3)
                    ((RuneBeetleCarapace)item).PoisonBonus = 3;

                if (((RuneBeetleCarapace)item).EnergyBonus != 3)
                    ((RuneBeetleCarapace)item).EnergyBonus = 3;
            }

            if (item is SpiritOfTheTotem)
            {
                if (((SpiritOfTheTotem)item).Resistances.Fire != 7)
                    ((SpiritOfTheTotem)item).Resistances.Fire = 7;

                if (((SpiritOfTheTotem)item).Resistances.Cold != 2)
                    ((SpiritOfTheTotem)item).Resistances.Cold = 2;

                if (((SpiritOfTheTotem)item).Resistances.Poison != 6)
                    ((SpiritOfTheTotem)item).Resistances.Poison = 6;

                if (((SpiritOfTheTotem)item).Resistances.Energy != 6)
                    ((SpiritOfTheTotem)item).Resistances.Energy = 6;
            }

            if (item is Stormgrip && ((Stormgrip)item).Attributes.AttackChance != 10)
            {
                ((Stormgrip)item).Attributes.AttackChance = 10;
            }

            if (item is InquisitorsResolution)
            {
                if (((InquisitorsResolution)item).PhysicalBonus != 5)
                    ((InquisitorsResolution)item).PhysicalBonus = 5;

                if (((InquisitorsResolution)item).FireBonus != 7)
                    ((InquisitorsResolution)item).FireBonus = 7;

                if (((InquisitorsResolution)item).ColdBonus != -2)
                    ((InquisitorsResolution)item).ColdBonus = -2;

                if (((InquisitorsResolution)item).PoisonBonus != 7)
                    ((InquisitorsResolution)item).PoisonBonus = 7;

                if (((InquisitorsResolution)item).EnergyBonus != -7)
                    ((InquisitorsResolution)item).EnergyBonus = -7;
            }

            if (item is TomeOfLostKnowledge && ((TomeOfLostKnowledge)item).Attributes.RegenMana != 3)
            {
                ((TomeOfLostKnowledge)item).Attributes.RegenMana = 3;
            }

            if (item is WizardsCrystalGlasses)
            {
                if (((WizardsCrystalGlasses)item).PhysicalBonus != 5)
                    ((WizardsCrystalGlasses)item).PhysicalBonus = 5;

                if (((WizardsCrystalGlasses)item).FireBonus != 5)
                    ((WizardsCrystalGlasses)item).FireBonus = 5;

                if (((WizardsCrystalGlasses)item).ColdBonus != 5)
                    ((WizardsCrystalGlasses)item).ColdBonus = 5;

                if (((WizardsCrystalGlasses)item).PoisonBonus != 5)
                    ((WizardsCrystalGlasses)item).PoisonBonus = 5;

                if (((WizardsCrystalGlasses)item).EnergyBonus != 5)
                    ((WizardsCrystalGlasses)item).EnergyBonus = 5;
            }
        }
    }

    public class FactionCollectionItem : CollectionItem
    {
        public int MinRank { get; private set; }
        public Faction Faction { get; private set; }

        public FactionCollectionItem(Type type, int id, int tooltip, int hue, Faction faction, double points, int minRank)
            : base(type, id, tooltip, hue, points)
        {
            Faction = faction;
            MinRank = minRank;
        }

        public override void OnGiveReward(PlayerMobile to, Item item, IComunityCollection collection, int hue)
        {
            if(this.Faction != null)
            {
                FactionEquipment.CheckProperties(item);
                FactionItem.Imbue(item, Faction, false, -1, MinRank);

                if (!(item is Spellbook || item is ShrineGem))
                    item.LootType = LootType.Regular;

                if (item is IWearableDurability)
                {
                    ((IWearableDurability)item).MaxHitPoints = 255;
                    ((IWearableDurability)item).HitPoints = 255;
                }

                if (item is IOwnerRestricted)
                {
                    ((IOwnerRestricted)item).Owner = to;
                    to.SendLocalizedMessage(1094803); // This faction reward is bound to you, and cannot be traded.
                }

                item.InvalidateProperties();
            }

            base.OnGiveReward(to, item, collection, hue);
        }

        public override bool CanSelect(PlayerMobile from)
        {
            PlayerState state = PlayerState.Find(from);


            return from.AccessLevel > AccessLevel.Player || (state != null && state.Rank.Rank >= MinRank);
        }
    }
}