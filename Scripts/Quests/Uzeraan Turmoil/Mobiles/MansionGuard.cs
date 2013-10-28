using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Haven
{
    public class MansionGuard : BaseQuester
    {
        [Constructable]
        public MansionGuard()
            : base("the Mansion Guard")
        {
        }

        public MansionGuard(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Hue = Utility.RandomSkinHue();

            this.Female = false;
            this.Body = 0x190;
            this.Name = NameList.RandomName("male");
        }

        public override void InitOutfit()
        {
            this.AddItem(new PlateChest());
            this.AddItem(new PlateArms());
            this.AddItem(new PlateGloves());
            this.AddItem(new PlateLegs());

            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this, this.HairHue);

            Bardiche weapon = new Bardiche();
            weapon.Movable = false;
            this.AddItem(weapon);
        }

        public override int GetAutoTalkRange(PlayerMobile pm)
        {
            return 3;
        }

        public override bool CanTalkTo(PlayerMobile to)
        {
            return (to.Quest == null && QuestSystem.CanOfferQuest(to, typeof(UzeraanTurmoilQuest)));
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            if (player.Quest == null && QuestSystem.CanOfferQuest(player, typeof(UzeraanTurmoilQuest)))
            {
                this.Direction = this.GetDirectionTo(player);

                new UzeraanTurmoilQuest(player).SendOffer();
            }
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