#region Header
// **********
// ServUO - Paladin.cs
// **********
#endregion

#region References
using Server.Items;
#endregion

namespace Server.Mobiles
{
    public class Paladin : BaseCreature
    {
        [Constructable]
        public Paladin()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, .2, .4)
        {
            SetStr(100);
            SetDex(150);
            SetInt(40);

            Body = 0x190;
            Name = NameList.RandomName("male");
            Title = "the Paladin";
            Hue = Race.RandomSkinHue();

            switch (Utility.Random(5))
            {
                case 0: SetWearable(new Helmet()); break;
                case 1: SetWearable(new NorseHelm()); break;
                case 2: SetWearable(new PlateHelm()); break;
                case 3: SetWearable(new Bascinet()); break;
                case 4: SetWearable(new ChainCoif()); break;
            }

            SetWearable(new PlateLegs());
            SetWearable(new PlateArms());
            SetWearable(new PlateGloves());
            SetWearable(new PlateChest());
            SetWearable(new StuddedGorget());
            SetWearable(new VikingSword());
            SetWearable(new MetalKiteShield(), 1158);

            switch (Utility.Random(3))
            {
                case 0: SetWearable(new Tunic(), GetRandomHue()); break;
                case 1: SetWearable(new Doublet(), GetRandomHue()); break;
                case 2: SetWearable(new BodySash(), GetRandomHue()); break;
            }

            SetSkill(SkillName.Swords, 120);
            SetSkill(SkillName.Tactics, 120);
            SetSkill(SkillName.Anatomy, 120);
            SetSkill(SkillName.MagicResist, 120);
        }

        public virtual int GetRandomHue()
        {
            switch (Utility.Random(5))
            {
                default:
                case 0:
                    return Utility.RandomBlueHue();
                case 1:
                    return Utility.RandomGreenHue();
                case 2:
                    return Utility.RandomRedHue();
                case 3:
                    return Utility.RandomYellowHue();
                case 4:
                    return Utility.RandomNeutralHue();
            }
        }

        public Paladin(Serial serial)
            : base(serial)
        { }

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