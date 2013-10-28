using System;
using System.Reflection;
using Server.Items;

namespace Server.Engines.XmlSpawner2
{
    public class XmlWeaponAbility : XmlAttachment
    {
        private WeaponAbility m_Ability = null;// default data

        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments

        // a serial constructor is REQUIRED
        public XmlWeaponAbility(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlWeaponAbility(string weaponability)
        {
            this.Ability = weaponability;
        }

        [Attachable]
        public XmlWeaponAbility(string name, string weaponability)
        {
            this.Name = name;
            this.Ability = weaponability;
        }

        [Attachable]
        public XmlWeaponAbility(string name, string weaponability, double expiresin)
        {
            this.Name = name;
            this.Ability = weaponability;
            this.Expiration = TimeSpan.FromMinutes(expiresin);
        }

        public WeaponAbility WeaponAbility
        {
            get
            {
                return this.m_Ability;
            }
            set
            {
                this.m_Ability = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Ability 
        { 
            get 
            {
                if (this.m_Ability != null)
                {
                    return this.m_Ability.GetType().Name;
                }
                else
                {
                    return null;
                }
            }
            set 
            {
                if (value != null)
                {
                    FieldInfo finfo = typeof(WeaponAbility).GetField(value);
                    if (finfo != null && finfo.IsStatic && finfo.FieldType == typeof(WeaponAbility))
                    {
                        try
                        {
                            this.m_Ability = (WeaponAbility)finfo.GetValue(null);
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    this.m_Ability = null;
                }
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write(this.Ability);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            this.Ability = reader.ReadString();
        }

        public override string OnIdentify(Mobile from)
        {
            if (from == null || from.IsPlayer())
                return null;

            if (this.Expiration > TimeSpan.Zero)
            {
                return String.Format("{2}: Weapon ability {0} expires in {1} mins", this.Ability, this.Expiration.TotalMinutes, this.Name);
            }
            else
            {
                return String.Format("{1}: Weapon ability {0}", this.Ability, this.Name);
            }
        }
    }
}