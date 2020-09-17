namespace Server.Items
{
    public class AquariumFishNet : SpecialFishingNet
    {
        [Constructable]
        public AquariumFishNet()
        {
            ItemID = 0xDC8;
            Hue = 0x240;
        }

        public AquariumFishNet(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074463;// An aquarium fishing net
        public override bool RequireDeepWater => false;
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

        protected override void AddNetProperties(ObjectPropertyList list)
        {
        }

        protected override void FinishEffect(Point3D p, Map map, Mobile from)
        {
            if (from.Skills.Fishing.Value < 10)
            {
                from.SendLocalizedMessage(1074487); // The creatures are too quick for you!
            }
            else
            {
                BaseFish fish = GiveFish(from);
                FishBowl bowl = Aquarium.GetEmptyBowl(from);

                if (bowl != null)
                {
                    fish.StopTimer();
                    bowl.AddItem(fish);
                    from.SendLocalizedMessage(1074489); // A live creature jumps into the fish bowl in your pack!
                    Delete();
                    return;
                }
                else
                {
                    if (from.PlaceInBackpack(fish))
                    {
                        from.PlaySound(0x5A2);
                        from.SendLocalizedMessage(1074490); // A live creature flops around in your pack before running out of air.

                        fish.Kill();
                        Delete();
                        return;
                    }
                    else
                    {
                        fish.Delete();

                        from.SendLocalizedMessage(1074488); // You could not hold the creature.
                    }
                }
            }

            InUse = false;
            Movable = true;

            if (!from.PlaceInBackpack(this))
            {
                if (from.Map == null || from.Map == Map.Internal)
                    Delete();
                else
                    MoveToWorld(from.Location, from.Map);
            }
        }

        protected virtual BaseFish GiveFish(Mobile from)
        {
            double skill = from.Skills.Fishing.Value;

            if ((skill / 100.0) >= Utility.RandomDouble())
            {
                int max = (int)skill / 5;

                if (max > 20)
                    max = 20;

                switch (Utility.Random(max))
                {
                    case 0:
                        return new MinocBlueFish();
                    case 1:
                        return new Shrimp();
                    case 2:
                        return new FandancerFish();
                    case 3:
                        return new GoldenBroadtail();
                    case 4:
                        return new RedDartFish();
                    case 5:
                        return new AlbinoCourtesanFish();
                    case 6:
                        return new MakotoCourtesanFish();
                    case 7:
                        return new NujelmHoneyFish();
                    case 8:
                        return new Jellyfish();
                    case 9:
                        return new SpeckledCrab();
                    case 10:
                        return new LongClawCrab();
                    case 11:
                        return new AlbinoFrog();
                    case 12:
                        return new KillerFrog();
                    case 13:
                        return new VesperReefTiger();
                    case 14:
                        return new PurpleFrog();
                    case 15:
                        return new BritainCrownFish();
                    case 16:
                        return new YellowFinBluebelly();
                    case 17:
                        return new SpottedBuccaneer();
                    case 18:
                        return new SpinedScratcherFish();
                    default:
                        return new SmallMouthSuckerFin();
                }
            }

            return new MinocBlueFish();
        }
    }

    // Legacy code
    public class AquariumFishingNet : Item
    {
        public AquariumFishingNet()
        {
        }

        public AquariumFishingNet(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074463;// An aquarium fishing net
        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            Item replacement = CreateReplacement();

            if (!from.PlaceInBackpack(replacement))
            {
                replacement.Delete();
                from.SendLocalizedMessage(500720); // You don't have enough room in your backpack!
            }
            else
            {
                Delete();
                from.Use(replacement);
            }
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

        private Item CreateReplacement()
        {
            Item result = new AquariumFishNet
            {
                Hue = Hue,
                LootType = LootType,
                Movable = Movable,
                Name = Name,
                QuestItem = QuestItem,
                Visible = Visible
            };

            return result;
        }
    }
}
