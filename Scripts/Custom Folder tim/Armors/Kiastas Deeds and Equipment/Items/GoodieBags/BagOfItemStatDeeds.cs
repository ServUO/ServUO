using System;
using Server;
using Server.Items;

namespace Server.Kiasta.Deeds
{
    public class BagOfItemStatDeeds : BaseGoodieBag
    {
        [Constructable]
        public BagOfItemStatDeeds() : this(1)
        {
        }

        [Constructable]
        public BagOfItemStatDeeds(int amount) : base(amount)
        {
            Weight = 0.0;
            Name = "a bag of item stat deeds";
            this.LootType = LootType.Cursed;
            for (int i = 0; i < amount; i++)
            {
                DropItem(new AttackChanceDeed());
                DropItem(new BonusDexDeed());
                DropItem(new BonusHitsDeed());
                DropItem(new BonusIntDeed());
                DropItem(new BonusManaDeed());
                DropItem(new BonusStamDeed());
                DropItem(new BonusStrDeed());
                DropItem(new CastSpeedDeed());
                DropItem(new CastRecoveryDeed());
                DropItem(new DefendChanceDeed());
                DropItem(new DurabilityBonusDeed());
                DropItem(new EnhancePotionsDeed());
                DropItem(new HitColdAreaDeed());
                DropItem(new HitDispelDeed());
                DropItem(new HitEnergyAreaDeed());
                DropItem(new HitFireAreaDeed());
                DropItem(new HitFireballDeed());
                DropItem(new HitHarmDeed());
                DropItem(new HitLeechHitsDeed());
                DropItem(new HitLightningDeed());
                DropItem(new HitLowerAttackDeed());
                DropItem(new HitLowerDefendDeed());
                DropItem(new HitMagicArrowDeed());
                DropItem(new HitLeechManaDeed());
                DropItem(new HitPhysicalAreaDeed());
                DropItem(new HitPoisonAreaDeed());
                DropItem(new HitLeechStamDeed());
                DropItem(new LowerManaCostDeed());
                DropItem(new LowerRegCostDeed());
                DropItem(new LuckDeed());
                DropItem(new ReflectPhysicalDeed());
                DropItem(new RegenHitsDeed());
                DropItem(new RegenManaDeed());
                DropItem(new RegenStamDeed());
                DropItem(new ResistColdBonusDeed());
                DropItem(new ResistEnergyBonusDeed());
                DropItem(new ResistFireBonusDeed());
                DropItem(new ResistPhysicalBonusDeed());
                DropItem(new ResistPoisonBonusDeed());
                DropItem(new SelfRepairDeed());
                DropItem(new WeaponSpeedDeed());
                DropItem(new WeaponDamageDeed());
                DropItem(new BlessDeed());
                DropItem(new MageArmorDeed());
                DropItem(new NightSightDeed());
                DropItem(new SpellChannelingDeed());
                DropItem(new SpellDamageDeed());
                DropItem(new UseBestSkillDeed());
            }
        }

        public BagOfItemStatDeeds(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}