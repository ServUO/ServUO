using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Spells;
using Server.Spells.Ninjitsu;
using Server.Targeting;

namespace Server.Items
{
    public class BallOfSummoning : Item, TranslocationItem
    {
        private int m_Charges;
        private int m_Recharges;
        private BaseCreature m_Pet;
        private string m_PetName;
        [Constructable]
        public BallOfSummoning()
            : base(0xE2E)
        {
            this.Weight = 10.0;
            this.Light = LightType.Circle150;

            this.m_Charges = Utility.RandomMinMax(3, 9);

            this.m_PetName = "";
        }

        public BallOfSummoning(Serial serial)
            : base(serial)
        {
        }

        private delegate void BallCallback(Mobile from);
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get
            {
                return this.m_Charges;
            }
            set
            {
                if (value > this.MaxCharges)
                    this.m_Charges = this.MaxCharges;
                else if (value < 0)
                    this.m_Charges = 0;
                else
                    this.m_Charges = value;

                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Recharges
        {
            get
            {
                return this.m_Recharges;
            }
            set
            {
                if (value > this.MaxRecharges)
                    this.m_Recharges = this.MaxRecharges;
                else if (value < 0)
                    this.m_Recharges = 0;
                else
                    this.m_Recharges = value;

                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharges
        {
            get
            {
                return 20;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxRecharges
        {
            get
            {
                return 255;
            }
        }
        public string TranslocationItemName
        {
            get
            {
                return "crystal ball of pet summoning";
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseCreature Pet
        {
            get
            {
                if (this.m_Pet != null && this.m_Pet.Deleted)
                {
                    this.m_Pet = null;
                    this.InternalUpdatePetName();
                }

                return this.m_Pet;
            }
            set
            {
                this.m_Pet = value;
                this.InternalUpdatePetName();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string PetName
        {
            get
            {
                return this.m_PetName;
            }
        }
        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1054131, this.m_Charges.ToString() + (this.m_PetName.Length == 0 ? "\t " : "\t" + this.m_PetName)); // a crystal ball of pet summoning: [charges: ~1_charges~] : [linked pet: ~2_petName~]
        }

        public override void OnSingleClick(Mobile from)
        {
            this.LabelTo(from, 1054131, this.m_Charges.ToString() + (this.m_PetName.Length == 0 ? "\t " : "\t" + this.m_PetName)); // a crystal ball of pet summoning: [charges: ~1_charges~] : [linked pet: ~2_petName~]
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive && this.RootParent == from)
            {
                if (this.Pet == null)
                {
                    list.Add(new BallEntry(new BallCallback(LinkPet), 6180));
                }
                else
                {
                    list.Add(new BallEntry(new BallCallback(CastSummonPet), 6181));
                    list.Add(new BallEntry(new BallCallback(UpdatePetName), 6183));
                    list.Add(new BallEntry(new BallCallback(UnlinkPet), 6182));
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.RootParent != from) // TODO: Previous implementation allowed use on ground, without house protection checks. What is the correct behavior?
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1042001); // That must be in your pack for you to use it.
                return;
            }

            AnimalFormContext animalContext = AnimalForm.GetContext(from);

            if (Core.ML && animalContext != null)
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1080073); // You cannot use a Crystal Ball of Pet Summoning while in animal form.
                return;
            }

            if (this.Pet == null)
            {
                this.LinkPet(from);
            }
            else
            {
                this.CastSummonPet(from);
            }
        }

        public void LinkPet(Mobile from)
        {
            BaseCreature pet = this.Pet;

            if (this.Deleted || pet != null || this.RootParent != from)
                return;

            from.SendLocalizedMessage(1054114); // Target your pet that you wish to link to this Crystal Ball of Pet Summoning.
            from.Target = new PetLinkTarget(this);
        }

        public void CastSummonPet(Mobile from)
        {
            BaseCreature pet = this.Pet;

            if (this.Deleted || pet == null || this.RootParent != from)
                return;

            if (this.Charges == 0)
            {
                this.SendLocalizedMessageTo(from, 1054122); // The Crystal Ball darkens. It must be charged before it can be used again.
            }
            else if (pet is BaseMount && ((BaseMount)pet).Rider == from)
            {
                MessageHelper.SendLocalizedMessageTo(this, from, 1054124, 0x36); // The Crystal Ball fills with a yellow mist. Why would you summon your pet while riding it?
            }
            else if (pet.Map == Map.Internal && (!pet.IsStabled || (from.Followers + pet.ControlSlots) > from.FollowersMax))
            {
                MessageHelper.SendLocalizedMessageTo(this, from, 1054125, 0x5); // The Crystal Ball fills with a blue mist. Your pet is not responding to the summons.
            }
            else if ((!pet.Controlled || pet.ControlMaster != from) && !from.Stabled.Contains(pet))
            {
                MessageHelper.SendLocalizedMessageTo(this, from, 1054126, 0x8FD); // The Crystal Ball fills with a grey mist. You are not the owner of the pet you are attempting to summon.
            }
            else if (!pet.IsBonded)
            {
                MessageHelper.SendLocalizedMessageTo(this, from, 1054127, 0x22); // The Crystal Ball fills with a red mist. You appear to have let your bond to your pet deteriorate.
            }
            else if (from.Map == Map.Ilshenar || from.Region.IsPartOf<DungeonRegion>() || from.Region.IsPartOf<Jail>())
            {
                from.Send(new AsciiMessage(this.Serial, this.ItemID, MessageType.Regular, 0x22, 3, "", "You cannot summon your pet to this location."));
            }
            else if (Core.ML && from is PlayerMobile && DateTime.UtcNow < ((PlayerMobile)from).LastPetBallTime.AddSeconds(15.0))
            {
                MessageHelper.SendLocalizedMessageTo(this, from, 1080072, 0x22); // You must wait a few seconds before you can summon your pet.
            }
            else
            {
                if (Core.ML)
                    new PetSummoningSpell(this, from).Cast();
                else
                    this.SummonPet(from);
            }
        }

        public void SummonPet(Mobile from)
        {
            BaseCreature pet = this.Pet;

            if (pet == null)
                return;

            this.Charges--;

            if (pet.IsStabled)
            {
                pet.SetControlMaster(from);

                if (pet.Summoned)
                    pet.SummonMaster = from;

                pet.ControlTarget = from;
                pet.ControlOrder = OrderType.Follow;

                pet.IsStabled = false;
                pet.StabledBy = null;
                from.Stabled.Remove(pet);

                if (from is PlayerMobile)
                    ((PlayerMobile)from).AutoStabled.Remove(pet);
            }

            pet.MoveToWorld(from.Location, from.Map);

            MessageHelper.SendLocalizedMessageTo(this, from, 1054128, 0x43); // The Crystal Ball fills with a green mist. Your pet has been summoned.

            if (from is PlayerMobile)
            {
                ((PlayerMobile)from).LastPetBallTime = DateTime.UtcNow;
            }
        }

        public void UnlinkPet(Mobile from)
        {
            if (!this.Deleted && this.Pet != null && this.RootParent == from)
            {
                this.Pet = null;

                this.SendLocalizedMessageTo(from, 1054120); // This crystal ball is no longer linked to a pet.
            }
        }

        public void UpdatePetName(Mobile from)
        {
            this.InternalUpdatePetName();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)1); // version

            writer.WriteEncodedInt((int)this.m_Recharges);

            writer.WriteEncodedInt((int)this.m_Charges);
            writer.Write((Mobile)this.Pet);
            writer.Write((string)this.m_PetName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Recharges = reader.ReadEncodedInt();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Charges = Math.Min(reader.ReadEncodedInt(), this.MaxCharges);
                        this.Pet = (BaseCreature)reader.ReadMobile();
                        this.m_PetName = reader.ReadString();
                        break;
                    }
            }
        }

        private void InternalUpdatePetName()
        {
            BaseCreature pet = this.Pet;

            if (pet == null)
                this.m_PetName = "";
            else
                this.m_PetName = pet.Name;

            this.InvalidateProperties();
        }

        private class BallEntry : ContextMenuEntry
        {
            private readonly BallCallback m_Callback;
            public BallEntry(BallCallback callback, int number)
                : base(number, 2)
            {
                this.m_Callback = callback;
            }

            public override void OnClick()
            {
                Mobile from = this.Owner.From;

                if (from.CheckAlive())
                    this.m_Callback(from);
            }
        }

        private class PetLinkTarget : Target
        {
            private readonly BallOfSummoning m_Ball;
            public PetLinkTarget(BallOfSummoning ball)
                : base(-1, false, TargetFlags.None)
            {
                this.m_Ball = ball;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Ball.Deleted || this.m_Ball.Pet != null)
                    return;

                if (this.m_Ball.RootParent != from)
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1042001); // That must be in your pack for you to use it.
                }
                else if (targeted is BaseCreature)
                {
                    BaseCreature creature = (BaseCreature)targeted;

                    if (!creature.Controlled || creature.ControlMaster != from)
                    {
                        MessageHelper.SendLocalizedMessageTo(this.m_Ball, from, 1054117, 0x59); // You may only link your own pets to a Crystal Ball of Pet Summoning.
                    }
                    else if (!creature.IsBonded)
                    {
                        MessageHelper.SendLocalizedMessageTo(this.m_Ball, from, 1054118, 0x59); // You must bond with your pet before it can be linked to a Crystal Ball of Pet Summoning.
                    }
                    else
                    {
                        MessageHelper.SendLocalizedMessageTo(this.m_Ball, from, 1054119, 0x59); // Your pet is now linked to this Crystal Ball of Pet Summoning.

                        this.m_Ball.Pet = creature;
                    }
                }
                else if (targeted == this.m_Ball)
                {
                    MessageHelper.SendLocalizedMessageTo(this.m_Ball, from, 1054115, 0x59); // The Crystal Ball of Pet Summoning cannot summon itself.
                }
                else
                {
                    MessageHelper.SendLocalizedMessageTo(this.m_Ball, from, 1054116, 0x59); // Only pets can be linked to this Crystal Ball of Pet Summoning.
                }
            }
        }

        private class PetSummoningSpell : Spell
        {
            private static readonly SpellInfo m_Info = new SpellInfo("Ball Of Summoning", "", 230);
            private readonly BallOfSummoning m_Ball;
            private readonly Mobile m_Caster;
            private bool m_Stop;
            public PetSummoningSpell(BallOfSummoning ball, Mobile caster)
                : base(caster, null, m_Info)
            {
                this.m_Caster = caster;
                this.m_Ball = ball;
            }

            public override bool ClearHandsOnCast
            {
                get
                {
                    return false;
                }
            }
            public override bool RevealOnCast
            {
                get
                {
                    return true;
                }
            }
            public override double CastDelayFastScalar
            {
                get
                {
                    return 0;
                }
            }
            public override TimeSpan CastDelayBase
            {
                get
                {
                    return TimeSpan.FromSeconds(2.0);
                }
            }
            public override TimeSpan GetCastRecovery()
            {
                return TimeSpan.Zero;
            }

            public override int GetMana()
            {
                return 0;
            }

            public override bool ConsumeReagents()
            {
                return true;
            }

            public override bool CheckFizzle()
            {
                return true;
            }

            public void Stop()
            {
                this.m_Stop = true;
                this.Disturb(DisturbType.Hurt, false, false);
            }

            public override bool CheckDisturb(DisturbType type, bool checkFirst, bool resistable)
            {
                if (type == DisturbType.EquipRequest || type == DisturbType.UseRequest/* || type == DisturbType.Hurt*/)
                    return false;

                return true;
            }

            public override void DoHurtFizzle()
            {
                if (!this.m_Stop)
                    base.DoHurtFizzle();
            }

            public override void DoFizzle()
            {
                if (!this.m_Stop)
                    base.DoFizzle();
            }

            public override void OnDisturb(DisturbType type, bool message)
            {
                if (message && !this.m_Stop)
                    this.Caster.SendLocalizedMessage(1080074); // You have been disrupted while attempting to summon your pet!
            }

            public override void OnCast()
            {
                this.m_Ball.SummonPet(this.m_Caster);

                this.FinishSequence();
            }
        }
    }
}
