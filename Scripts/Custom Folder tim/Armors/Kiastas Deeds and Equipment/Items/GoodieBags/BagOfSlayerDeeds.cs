using System;
using Server;
using Server.Items;

namespace Server.Kiasta.Deeds
{
    public class BagOfSlayerDeeds : BaseGoodieBag
    {
        [Constructable]
        public BagOfSlayerDeeds() : this(1)
        {
        }

        [Constructable]
        public BagOfSlayerDeeds(int amount) : base(amount)
        {
            Weight = 0.0;
            Name = "a bag of slayer deeds";
            this.LootType = LootType.Cursed;
            for (int i = 0; i < amount; i++)
            {
                DropItem(new ArachnidDoomDeed());
                DropItem(new BalronDamnationDeed());
                DropItem(new BloodDrinkingDeed());
                DropItem(new DaemonDismissalDeed());
                DropItem(new DragonSlayingDeed());
                DropItem(new EarthShatterDeed());
                DropItem(new ElementalBanDeed());
                DropItem(new ElementalHealthDeed());
                DropItem(new ExorcismDeed());
                DropItem(new FeyDeed());
                DropItem(new FlameDousingDeed());
                DropItem(new GargoylesFoeDeed());
                DropItem(new LizardmanSlaughterDeed());
                DropItem(new OgreTrashingDeed());
                DropItem(new OphidianDeed());
                DropItem(new OrcSlayingDeed());
                DropItem(new RepondDeed());
                DropItem(new ReptilianDeathDeed());
                DropItem(new ScorpionsBaneDeed());
                DropItem(new SilverDeed());
                DropItem(new SlayerRemovalDeed());
                DropItem(new SnakesBaneDeed());
                DropItem(new SpidersDeathDeed());
                DropItem(new SummerWindDeed());
                DropItem(new TerathanDeed());
                DropItem(new TrollSlaughterDeed());
                DropItem(new VacuumDeed());
                DropItem(new WaterDissipationDeed());
            }
        }

        public BagOfSlayerDeeds(Serial serial) : base(serial)
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