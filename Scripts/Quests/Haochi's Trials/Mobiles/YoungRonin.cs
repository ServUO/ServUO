using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Samurai
{
    [CorpseName("a young ronin's corpse")]
    public class YoungRonin : BaseCreature
    {
        [Constructable]
        public YoungRonin()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.InitStats(45, 30, 5);
            this.SetHits(10, 20);

            this.Hue = Utility.RandomSkinHue();
            this.Body = 0x190;
            this.Name = "a young ronin";

            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this);

            this.AddItem(new LeatherDo());
            this.AddItem(new LeatherHiroSode());
            this.AddItem(new SamuraiTabi());

            switch ( Utility.Random(3) )
            {
                case 0:
                    this.AddItem(new StuddedHaidate());
                    break;
                case 1:
                    this.AddItem(new PlateSuneate());
                    break;
                default:
                    this.AddItem(new LeatherSuneate());
                    break;
            }

            this.AddItem(new Bandana(Utility.RandomNondyedHue()));

            switch ( Utility.Random(3) )
            {
                case 0:
                    this.AddItem(new NoDachi());
                    break;
                case 1:
                    this.AddItem(new Lajatang());
                    break;
                default:
                    this.AddItem(new Wakizashi());
                    break;
            }

            this.SetSkill(SkillName.Swords, 50.0);
            this.SetSkill(SkillName.Tactics, 50.0);
        }

        public YoungRonin(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}