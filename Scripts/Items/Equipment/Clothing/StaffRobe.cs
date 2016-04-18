using System;

namespace Server.Items
{
    public class StaffRobe : BaseSuit
    {
        private int _DecoratorHue = 0x0;
        private int _OwnerHue, _CoOwnerHue = 0x481;
        private int _DeveloperHue = 0x497;
        private int _AdminHue = 0x47E;
        private int _SeerHue = 0x1D3;
        private int _GamemasterHue = 0x26;
        private int _CounselorHue, _SpawnerHue = 0x3;
        private int _PlayerHue = 0x0;
        [Constructable]
        public StaffRobe()
            : base(AccessLevel.Player, 0x0, 0x2683)
        {
            this.Name = "an elder robe";
        }

        public StaffRobe(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.Owner)]
        public int OwnerHue
        {
            get
            {
                return this._OwnerHue;
            }
            set
            {
                this._OwnerHue = value;
            }
        }
        [CommandProperty(AccessLevel.CoOwner)]
        public int CoOwnerHue
        {
            get
            {
                return this._CoOwnerHue;
            }
            set
            {
                this._CoOwnerHue = value;
            }
        }
        [CommandProperty(AccessLevel.Developer)]
        public int DeveloperHue
        {
            get
            {
                return this._DeveloperHue;
            }
            set
            {
                this._DeveloperHue = value;
            }
        }
        [CommandProperty(AccessLevel.Administrator)]
        public int AdminHue
        {
            get
            {
                return this._AdminHue;
            }
            set
            {
                this._AdminHue = value;
            }
        }
        [CommandProperty(AccessLevel.Seer)]
        public int SeerHue
        {
            get
            {
                return this._SeerHue;
            }
            set
            {
                this._SeerHue = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int GamemasterHue
        {
            get
            {
                return this._GamemasterHue;
            }
            set
            {
                this._GamemasterHue = value;
            }
        }
        [CommandProperty(AccessLevel.Spawner)]
        public int SpawnerHue
        {
            get
            {
                return this._SpawnerHue;
            }
            set
            {
                this._SpawnerHue = value;
            }
        }
        [CommandProperty(AccessLevel.Decorator)]
        public int DecoratorHue
        {
            get
            {
                return this._DecoratorHue;
            }
            set
            {
                this._DecoratorHue = value;
            }
        }
        [CommandProperty(AccessLevel.Counselor)]
        public int CounselorHue
        {
            get
            {
                return this._CounselorHue;
            }
            set
            {
                this._CounselorHue = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int PlayerHue
        {
            get
            {
                return this._PlayerHue;
            }
            set
            {
                this._PlayerHue = value;
            }
        }
        public override void Serialize(GenericWriter writer)
        { // No need to serialize level hues, robe meant for staff only.
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void OnRemoved(object parent)
        {
            if (this.ItemID == 0x204F)
                this.ItemID = 0x2683;

            this.Hue = 0x0;
            this.Name = "An Elder Robe";
        }

        public override bool OnEquip(Mobile from)
        {
            if ((this.ItemID == 0x2683) && (from.IsStaff()))
                this.ItemID = 0x204F;

            switch (from.AccessLevel)
            {
                case AccessLevel.Owner:
                    this.Name = "The Owner Robe";
                    this.Hue = this._OwnerHue;
                    break;
                case AccessLevel.CoOwner:
                    this.Name = "A Co-Owner Robe";
                    break;
                case AccessLevel.Administrator:
                    this.Name = "An Administrator Robe";
                    this.Hue = this._AdminHue;
                    break;
                case AccessLevel.Developer:
                    this.Name = "a Developer robe";
                    this.Hue = this._DeveloperHue;
                    break;
                case AccessLevel.Seer:
                    this.Name = " a Seer Robe";
                    this.Hue = this._SeerHue;
                    break;
                case AccessLevel.GameMaster:
                    this.Name = "a GameMaster Robe";
                    this.Hue = this._GamemasterHue;
                    break;
                case AccessLevel.Counselor:
                    this.Name = "a Counselor Robe";
                    this.Hue = this._CounselorHue;
                    break;
                default:
                    this.Name = "an elder robe";
                    this.Hue = this._PlayerHue;
                    break;
            }

            return true;
        }
    }
}