using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Samurai
{
    public class HaochisGuardsman : BaseQuester
    {
        [Constructable]
        public HaochisGuardsman()
            : base("the Guardsman of Daimyo Haochi")
        {
        }

        public HaochisGuardsman(Serial serial)
            : base(serial)
        {
        }

        public override int TalkNumber
        {
            get
            {
                return -1;
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Hue = Race.Human.RandomSkinHue();

            this.Female = false;
            this.Body = 0x190;
            this.Name = NameList.RandomName("male");
        }

        public override void InitOutfit()
        {
            Utility.AssignRandomHair(this);

            this.AddItem(new LeatherDo());
            this.AddItem(new LeatherHiroSode());
            this.AddItem(new SamuraiTabi(Utility.RandomNondyedHue()));

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

            switch ( Utility.Random(4) )
            {
                case 0:
                    this.AddItem(new DecorativePlateKabuto());
                    break;
                case 1:
                    this.AddItem(new ChainHatsuburi());
                    break;
                case 2:
                    this.AddItem(new LightPlateJingasa());
                    break;
                default:
                    this.AddItem(new LeatherJingasa());
                    break;
            }

            Item weapon;
            switch ( Utility.Random(3) )
            {
                case 0:
                    weapon = new NoDachi();
                    break;
                case 1:
                    weapon = new Lajatang();
                    break;
                default:
                    weapon = new Wakizashi();
                    break;
            }
            weapon.Movable = false;
            this.AddItem(weapon);
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
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