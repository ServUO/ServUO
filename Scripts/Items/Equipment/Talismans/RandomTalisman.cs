namespace Server.Items
{
    public class RandomTalisman : BaseTalisman
    {
        [Constructable]
        public RandomTalisman()
            : base(GetRandomItemID())
        {
            Summoner = GetRandomSummoner();

            if (Summoner.IsEmpty)
            {
                Removal = GetRandomRemoval();

                if (Removal != TalismanRemoval.None)
                {
                    MaxCharges = GetRandomCharges();
                    MaxChargeTime = 1200;
                }
            }
            else
            {
                MaxCharges = Utility.RandomMinMax(10, 50);

                if (Summoner.IsItem)
                    MaxChargeTime = 60;
                else
                    MaxChargeTime = 1800;
            }

            Blessed = GetRandomBlessed();
            Slayer = GetRandomSlayer();
            Protection = GetRandomProtection();
            Killer = GetRandomKiller();
            Skill = GetRandomSkill();
            ExceptionalBonus = GetRandomExceptional();
            SuccessBonus = GetRandomSuccessful();
            Charges = MaxCharges;
        }

        public RandomTalisman(Serial serial)
            : base(serial)
        {
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
