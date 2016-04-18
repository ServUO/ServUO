using System;
using Server.Guilds;
using Server.Items;

namespace Server.Mobiles
{
    public abstract class BaseShieldGuard : BaseCreature
    {
        public BaseShieldGuard()
            : base(AIType.AI_Melee, FightMode.Aggressor, 14, 1, 0.8, 1.6)
        {
            this.InitStats(1000, 1000, 1000);
            this.Title = "the guard";

            this.SpeechHue = Utility.RandomDyedHue();

            this.Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");

                this.AddItem(new FemalePlateChest());
                this.AddItem(new PlateArms());
                this.AddItem(new PlateLegs());

                switch( Utility.Random(2) )
                {
                    case 0:
                        this.AddItem(new Doublet(Utility.RandomNondyedHue()));
                        break;
                    case 1:
                        this.AddItem(new BodySash(Utility.RandomNondyedHue()));
                        break;
                }

                switch( Utility.Random(2) )
                {
                    case 0:
                        this.AddItem(new Skirt(Utility.RandomNondyedHue()));
                        break;
                    case 1:
                        this.AddItem(new Kilt(Utility.RandomNondyedHue()));
                        break;
                }
            }
            else
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");

                this.AddItem(new PlateChest());
                this.AddItem(new PlateArms());
                this.AddItem(new PlateLegs());

                switch( Utility.Random(3) )
                {
                    case 0:
                        this.AddItem(new Doublet(Utility.RandomNondyedHue()));
                        break;
                    case 1:
                        this.AddItem(new Tunic(Utility.RandomNondyedHue()));
                        break;
                    case 2:
                        this.AddItem(new BodySash(Utility.RandomNondyedHue()));
                        break;
                }
            }

            Utility.AssignRandomHair(this);
            if (Utility.RandomBool())
                Utility.AssignRandomFacialHair(this, this.HairHue);

            VikingSword weapon = new VikingSword();
            weapon.Movable = false;
            this.AddItem(weapon);

            BaseShield shield = this.Shield;
            shield.Movable = false;
            this.AddItem(shield);

            this.PackGold(250, 500);

            this.Skills[SkillName.Anatomy].Base = 120.0;
            this.Skills[SkillName.Tactics].Base = 120.0;
            this.Skills[SkillName.Swords].Base = 120.0;
            this.Skills[SkillName.MagicResist].Base = 120.0;
            this.Skills[SkillName.DetectHidden].Base = 100.0;
        }

        public BaseShieldGuard(Serial serial)
            : base(serial)
        {
        }

        public abstract int Keyword { get; }
        public abstract BaseShield Shield { get; }
        public abstract int SignupNumber { get; }
        public abstract GuildType Type { get; }
        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(this.Location, 2))
                return true;

            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (!e.Handled && e.HasKeyword(this.Keyword) && e.Mobile.InRange(this.Location, 2))
            {
                e.Handled = true;

                Mobile from = e.Mobile;
                Guild g = from.Guild as Guild;

                if (g == null || g.Type != this.Type)
                {
                    this.Say(this.SignupNumber);
                }
                else
                {
                    Container pack = from.Backpack;
                    BaseShield shield = this.Shield;
                    Item twoHanded = from.FindItemOnLayer(Layer.TwoHanded);

                    if ((pack != null && pack.FindItemByType(shield.GetType()) != null) || (twoHanded != null && shield.GetType().IsAssignableFrom(twoHanded.GetType())))
                    {
                        this.Say(1007110); // Why dost thou ask about virtue guards when thou art one?
                        shield.Delete();
                    }
                    else if (from.PlaceInBackpack(shield))
                    {
                        this.Say(Utility.Random(1007101, 5));
                        this.Say(1007139); // I see you are in need of our shield, Here you go.
                        from.AddToBackpack(shield);
                    }
                    else
                    {
                        from.SendLocalizedMessage(502868); // Your backpack is too full.
                        shield.Delete();
                    }
                }
            }

            base.OnSpeech(e);
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