using System;

namespace Server.Items
{
    public class AncientAquariumFishNet : AquariumFishNet
    {
        [Constructable]
        public AncientAquariumFishNet()
        {
            ItemID = 0xDC8;
            Hue = 2967;
        }

        public AncientAquariumFishNet(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1159078;// Ancient Aquarium Net

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

        protected override BaseFish GiveFish(Mobile from)
        {
            double skill = from.Skills.Fishing.Value;


            if (skill < 50)
            {
                return base.GiveFish(from);
            }

            int amount;

            if (skill < 80)
            {
                amount = 10;
            }
            else if (skill < 100)
            {
                amount = 20;
            }
            else if (skill < 105)
            {
                amount = 31;
            }
            else if (skill < 115)
            {
                amount = 42;
            }
            else
            {
                amount = 53;
            }

            return Loot.Construct(_AncientAquariumFish[Utility.Random(amount)]) as BaseFish;
        }

        private Type[] _AncientAquariumFish = new[]
        {
            // Common
            typeof(ArrowCrab), typeof(RockBeauty), typeof(SpottedParrotfish), typeof(BlackDurgeon), typeof(SeaUrchin),
            typeof(Squirrelfish), typeof(FrenchAngelfish), typeof(SergeantMajor), typeof(HumpbackGrouper), typeof(Spadefish),

            // Uncommon
            typeof(Beaugregory), typeof(HorseshoeCrab), typeof(SpinyLobster), typeof(ClownTriggerfish), typeof(MoorishIdol),
            typeof(YellowBoxfish), typeof(FalseClownfish), typeof(QueenTrigger), typeof(GrayAngelfish), typeof(Scorpionfish),

            // Rare
            typeof(BandedCoralShrimp), typeof(FeatherDuster), typeof(Octopus), typeof(BangaiCardinalfish), typeof(Fireclam),
            typeof(SpottedBlueRay), typeof(BarrelSponge), typeof(LionsManeJelly), typeof(TreeCoral), typeof(BlueHermitCrab),
            typeof(MantisShrimp),

            // Exceedingly Rare
            typeof(FireCoral), typeof(PJCardinalfish), typeof(MagnificentShrimpgoby), typeof(Frogfish), typeof(Porcupinefish),
            typeof(Filefish), typeof(GiantAnemone), typeof(PygmySeahorse), typeof(Squid), typeof(PicassoTriggerfish),
            typeof(SeaFan),

            // Exotic
            typeof(ChironexJelly), typeof(ElkhornCoral), typeof(Mandarinfish), typeof(ChocolateChipSeastar), typeof(FlamingoTongue),
            typeof(OrangeElephantEarSponge), typeof(CleanerShrimp), typeof(HippoTang), typeof(StaghornCoral), typeof(EagleRay),
            typeof(LeafySeaDragon)
        };
    }
}
