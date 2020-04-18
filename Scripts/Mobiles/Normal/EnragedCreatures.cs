namespace Server.Mobiles
{
    [CorpseName("a hare corpse")]
    public class EnragedRabbit : BaseEnraged
    {
        public EnragedRabbit(Mobile summoner)
            : base(summoner)
        {
            Name = "a rabbit";
            Body = 0xcd;
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
            writer.Write(0);
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
            Name = "a great hart";
            Body = 0xea;
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
            writer.Write(0);
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
            Name = "a hind";
            Body = 0xed;
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
            writer.Write(0);
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
            Name = "a black bear";
            Body = 0xd3;
            BaseSoundID = 0xa3;
        }

        public EnragedBlackBear(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
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
            Name = "an eagle";
            Body = 0x5;
            BaseSoundID = 0x2ee;
        }

        public EnragedEagle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
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
            SetStr(50, 200);
            SetDex(50, 200);
            SetHits(50, 200);
            SetStam(50, 200);

            /* 
            On OSI, all stats are random 50-200, but
            str is never less than hits, and dex is never
            less than stam.
            */

            if (Str < Hits)
                Str = Hits;
            if (Dex < Stam)
                Dex = Stam;

            Karma = -1000;
            Tamable = false;

            SummonMaster = summoner;
        }

        public BaseEnraged(Serial serial)
            : base(serial)
        {
        }

        public override void OnThink()
        {
            if (SummonMaster == null || SummonMaster.Deleted)
            {
                Delete();
            }
            /*
            On OSI, without combatant, they behave as if they have been
            given "come" command, ie they wander towards their summoner,
            but never actually "follow".
            */
            else if (!Combat(this))
            {
                if (AIObject != null)
                {
                    AIObject.MoveTo(SummonMaster, false, 5);
                }
            }
            /*
            On OSI, if the summon attacks a mobile, the summoner meer also
            attacks them, regardless of karma, etc. as long as the combatant
            is a player or controlled/summoned, and the summoner is not already
            engaged in combat.
            */
            else if (!Combat(SummonMaster))
            {
                BaseCreature bc = null;
                if (Combatant is BaseCreature)
                {
                    bc = (BaseCreature)Combatant;
                }
                if (Combatant is PlayerMobile || (bc != null && (bc.Controlled || bc.SummonMaster != null)))
                {
                    SummonMaster.Combatant = Combatant;
                }
            }
            else
            {
                base.OnThink();
            }
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
            list.Add(1060768); // enraged
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        private bool Combat(Mobile mobile)
        {
            Mobile combatant = mobile.Combatant as Mobile;

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
