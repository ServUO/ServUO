/*                                                             .---.
/  .  \
|\_/|   |
|   |  /|
.----------------------------------------------------------------' |
/  .-.                                                              |
|  /   \         Contribute To The Orbsydia SA Project               |
| |\_.  |                                                            |
|\|  | /|                        By Lotar84                          |
| `---' |                                                            |
|       |       (Orbanised by Orb SA Core Development Team)          | 
|       |                                                           /
|       |----------------------------------------------------------'
\       |
\     /
`---'
*/
using System;

namespace Server.Items
{
    public class EssenceBox : WoodenBox
    {
        [Constructable]
        public EssenceBox()
            : base()
        {
            this.Movable = true;
            this.Hue = 1161;

            switch (Utility.Random(42))
            {
                case 0:
                    this.DropItem(new EssencePrecision());
                    break;
                case 1:
                    this.DropItem(new EssenceAchievement());
                    break;
                case 2:
                    this.DropItem(new EssenceBalance());
                    break;
                case 3:
                    this.DropItem(new EssenceControl());
                    break;
                case 4:
                    this.DropItem(new EssenceDiligence());
                    break;
                case 5:
                    this.DropItem(new EssenceDirection());
                    break;
                case 6:
                    this.DropItem(new EssenceFeeling());
                    break;
                case 7:
                    this.DropItem(new EssenceOrder());
                    break;
                case 8:
                    this.DropItem(new EssencePassion());
                    break;
                case 9:
                    this.DropItem(new EssencePersistence());
                    break;
                case 10:
                    this.DropItem(new EssenceSingularity());
                    break;
                case 11:
                    this.DropItem(new FeyWings());
                    break;
                case 12:
                    this.DropItem(new FaeryDust());
                    break;
                case 13:
                    this.DropItem(new Fur());
                    break;
                case 14:
                    this.DropItem(new GoblinBlood());
                    break;
                case 15:
                    this.DropItem(new HornAbyssalInferno());
                    break;
                case 16:
                    this.DropItem(new KepetchWax());
                    break;
                case 17:
                    this.DropItem(new LavaSerpentCrust());
                    break;
                case 18:
                    this.DropItem(new MedusaBlood());
                    break;
                case 19:
                    this.DropItem(new PowderedIron());
                    break;
                case 20:
                    this.DropItem(new PrimalLichDust());
                    break;
                case 21:
                    this.DropItem(new RaptorTeeth());
                    break;
                case 22:
                    this.DropItem(new ReflectiveWolfEye());
                    break;
                case 23:
                    this.DropItem(new SeedRenewal());
                    break;
                case 24:
                    this.DropItem(new SilverSerpentVenom());
                    break;
                case 25:
                    this.DropItem(new SilverSnakeSkin());
                    break;
                case 26:
                    this.DropItem(new SlithEye());
                    break;
                case 27:
                    this.DropItem(new SlithTongue());
                    break;
                case 28:
                    this.DropItem(new SpiderCarapace());
                    break;
                case 29:
                    this.DropItem(new ScouringToxin());
                    break;
                case 30:
                    this.DropItem(new ToxicVenomSac());
                    break;
                case 31:
                    this.DropItem(new UndyingFlesh());
                    break;
                case 32:
                    this.DropItem(new VialVitirol());
                    break;
                case 33:
                    this.DropItem(new DelicateScales());
                    break;
                case 34:
                    this.DropItem(new VoidCore());
                    break;
                case 35:
                    this.DropItem(new VoidEssence());
                    break;
                case 36:
                    this.DropItem(new BottleIchor());
                    break;
                case 37:
                    this.DropItem(new ChagaMushroom());
                    break;
                case 38:
                    this.DropItem(new CrushedGlass());
                    break;
                case 39:
                    this.DropItem(new CrystalShards());
                    break;
                case 40:
                    this.DropItem(new CrystallineBlackrock());
                    break;
                case 41:
                    this.DropItem(new DaemonClaw());
                    break;
            }
        }

        public EssenceBox(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113770;
            }
        }//Essence Box
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