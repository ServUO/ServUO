using System;

namespace Server.Items
{
    public class RandomTalisman : BaseTalisman
    {
        [Constructable]
        public RandomTalisman()
            : base(GetRandomItemID())
        {
            Summoner = BaseTalisman.GetRandomSummoner();

            if (Summoner.IsEmpty)
            {
                Removal = BaseTalisman.GetRandomRemoval();

                if (Removal != TalismanRemoval.None)
                {
                    MaxCharges = BaseTalisman.GetRandomCharges();
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

            Blessed = BaseTalisman.GetRandomBlessed();
            Slayer = BaseTalisman.GetRandomSlayer();
            Protection = BaseTalisman.GetRandomProtection();
            Killer = BaseTalisman.GetRandomKiller();
            Skill = BaseTalisman.GetRandomSkill();
            ExceptionalBonus = BaseTalisman.GetRandomExceptional();
            SuccessBonus = BaseTalisman.GetRandomSuccessful();
            Charges = MaxCharges;
        }

        public RandomTalisman(Serial serial)
            : base(serial)
        {
        }

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