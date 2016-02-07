using System;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a hare corpse")]
    public class EnragedRabbit : BaseEnraged
    {
        public EnragedRabbit(Mobile summoner)
            : base(summoner)
        {
            this.Name = "a rabbit";
            this.Body = 0xcd;
        }

        public EnragedRabbit(Serial serial)
            : base(serial)
        {
        }

        public override int GetAttackSound() 
        { 
            return 0xC9; 
        }

        public override int GetHurtSound() 
        { 
            return 0xCA; 
        }

        public override int GetDeathSound() 
        { 
            return 0xCB; 
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a deer corpse")]
    public class EnragedHart : BaseEnraged
    {
        public EnragedHart(Mobile summoner)
            : base(summoner)
        {
            this.Name = "a great hart";
            this.Body = 0xea;
        }

        public EnragedHart(Serial serial)
            : base(serial)
        {
        }

        public override int GetAttackSound() 
        { 
            return 0x82; 
        }

        public override int GetHurtSound() 
        { 
            return 0x83; 
        }

        public override int GetDeathSound() 
        { 
            return 0x84; 
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a deer corpse")]
    public class EnragedHind : BaseEnraged
    {
        public EnragedHind(Mobile summoner)
            : base(summoner)
        {
            this.Name = "a hind";
            this.Body = 0xed;
        }

        public EnragedHind(Serial serial)
            : base(serial)
        {
        }

        public override int GetAttackSound() 
        { 
            return 0x82; 
        }

        public override int GetHurtSound() 
        { 
            return 0x83; 
        }

        public override int GetDeathSound() 
        { 
            return 0x84; 
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a bear corpse")]
    public class EnragedBlackBear : BaseEnraged
    {
        public EnragedBlackBear(Mobile summoner)
            : base(summoner)
        {
            this.Name = "a black bear";
            this.Body = 0xd3;
            this.BaseSoundID = 0xa3;
        }

        public EnragedBlackBear(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("an eagle corpse")]
    public class EnragedEagle : BaseEnraged
    {
        public EnragedEagle(Mobile summoner)
            : base(summoner)
        {
            this.Name = "an eagle";
            this.Body = 0x5;
            this.BaseSoundID = 0x2ee;
        }

        public EnragedEagle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BaseEnraged : BaseCreature
    {
        public BaseEnraged(Mobile summoner)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.SetStr(50, 200);
            this.SetDex(50, 200);
            this.SetHits(50, 200);
            this.SetStam(50, 200);

            /* 
            On OSI, all stats are random 50-200, but
            str is never less than hits, and dex is never
            less than stam.
            */

            if (this.Str < this.Hits)
                this.Str = this.Hits;
            if (this.Dex < this.Stam)
                this.Dex = this.Stam;

            this.Karma = -1000;
            this.Tamable = false;

            this.SummonMaster = summoner;
        }

        public BaseEnraged(Serial serial)
            : base(serial)
        {
        }

        public override void OnThink()
        {
            if (this.SummonMaster == null || this.SummonMaster.Deleted)
            {
                this.Delete();
            }
            /*
            On OSI, without combatant, they behave as if they have been
            given "come" command, ie they wander towards their summoner,
            but never actually "follow".
            */
            else if (!this.Combat(this))
            {
                if (this.AIObject != null)
                {
                    this.AIObject.MoveTo(this.SummonMaster, false, 5);
                }
            }
            /*
            On OSI, if the summon attacks a mobile, the summoner meer also
            attacks them, regardless of karma, etc. as long as the combatant
            is a player or controlled/summoned, and the summoner is not already
            engaged in combat.
            */
            else if (!this.Combat(this.SummonMaster))
            {
                BaseCreature bc = null;
                if (this.Combatant is BaseCreature)
                {
                    bc = (BaseCreature)this.Combatant;
                }
                if (this.Combatant.Player || (bc != null && (bc.Controlled || bc.SummonMaster != null)))
                {
                    this.SummonMaster.Combatant = this.Combatant;
                }
            }
            else
            {
                base.OnThink();
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            this.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1060768, from.NetState); // enraged
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
            list.Add(1060768); // enraged
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        private bool Combat(Mobile mobile)
        {
            Mobile combatant = mobile.Combatant;
            if (combatant == null || combatant.Deleted)
            { 
                return false;
            }
            else if (combatant.IsDeadBondedPet || !combatant.Alive)
            {
                return false;
            }
            return true;
        }
    }
}