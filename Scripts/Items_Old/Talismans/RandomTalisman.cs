using System;

namespace Server.Items
{
    public class RandomTalisman : BaseTalisman
    {
        [Constructable]
        public RandomTalisman()
            : base(GetRandomItemID())
        {
            this.Summoner = BaseTalisman.GetRandomSummoner();

            if (this.Summoner.IsEmpty)
            {
                this.Removal = BaseTalisman.GetRandomRemoval();

                if (this.Removal != TalismanRemoval.None)
                {
                    this.MaxCharges = BaseTalisman.GetRandomCharges();
                    this.MaxChargeTime = 1200;
                }
            }
            else
            {
                this.MaxCharges = Utility.RandomMinMax(10, 50);

                if (this.Summoner.IsItem)
                    this.MaxChargeTime = 60;
                else
                    this.MaxChargeTime = 1800;
            }

            this.Blessed = BaseTalisman.GetRandomBlessed();
            this.Slayer = BaseTalisman.GetRandomSlayer();
            this.Protection = BaseTalisman.GetRandomProtection();
            this.Killer = BaseTalisman.GetRandomKiller();
            this.Skill = BaseTalisman.GetRandomSkill();
            this.ExceptionalBonus = BaseTalisman.GetRandomExceptional();
            this.SuccessBonus = BaseTalisman.GetRandomSuccessful();
            this.Charges = this.MaxCharges;
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