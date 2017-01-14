using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.XmlSpawner2
{
    public class XmlFactionMastery : XmlAttachment
    {
        private int m_Chance = 20;// 20% hitchance by default
        private double m_FactionScaling = 0.002;// faction multiplier for damage bonus. By default, 0.002% increase in damage per total faction level
        private int m_PercentCap = 200;// maximum total percent damage bonus
        private int m_PercentIncrease = 50;// base percentage damage increase
        private string m_Enemy;
        private XmlMobFactions.GroupTypes m_EnemyType = XmlMobFactions.GroupTypes.End_Unused;
        // These are the various ways in which the message attachment can be constructed.
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments

        // a serial constructor is REQUIRED
        public XmlFactionMastery(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlFactionMastery(string enemyfaction)
        {
            this.EnemyFaction = enemyfaction;
        }

        [Attachable]
        public XmlFactionMastery(string enemyfaction, int basepercentincrease)
        {
            this.m_PercentIncrease = basepercentincrease;
            this.EnemyFaction = enemyfaction;
        }

        [Attachable]
        public XmlFactionMastery(string enemyfaction, int hitchance, int basepercentincrease)
        {
            this.m_Chance = hitchance;
            this.m_PercentIncrease = basepercentincrease;
            this.EnemyFaction = enemyfaction;
        }

        [Attachable]
        public XmlFactionMastery(string enemyfaction, int hitchance, int basepercentincrease, double expiresin)
        {
            this.m_Chance = hitchance;
            this.m_PercentIncrease = basepercentincrease;
            this.Expiration = TimeSpan.FromMinutes(expiresin);
            this.EnemyFaction = enemyfaction;
        }

        [Attachable]
        public XmlFactionMastery(string enemyfaction, int hitchance, int basepercentincrease, int percentcap, double expiresin)
        {
            this.m_Chance = hitchance;
            this.m_PercentIncrease = basepercentincrease;
            this.Expiration = TimeSpan.FromMinutes(expiresin);
            this.EnemyFaction = enemyfaction;
            this.PercentCap = percentcap;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Chance
        {
            get
            {
                return this.m_Chance;
            }
            set
            {
                this.m_Chance = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int PercentIncrease
        {
            get
            {
                return this.m_PercentIncrease;
            }
            set
            {
                this.m_PercentIncrease = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int PercentCap
        {
            get
            {
                return this.m_PercentCap;
            }
            set
            {
                this.m_PercentCap = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public double FactionScaling
        {
            get
            {
                return this.m_FactionScaling;
            }
            set
            {
                this.m_FactionScaling = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string EnemyFaction
        {
            get
            {
                return this.m_Enemy;
            }
            set
            {
                // look up the group type
                try
                {
                    if (value == "?")
                    {
                        // randomly pick one
                        this.m_EnemyType = (XmlMobFactions.GroupTypes)Utility.Random((int)XmlMobFactions.GroupTypes.End_Unused);
                    }
                    else
                        this.m_EnemyType = (XmlMobFactions.GroupTypes)Enum.Parse(typeof(XmlMobFactions.GroupTypes), value, true);
                }
                catch
                {
                }

                this.m_Enemy = this.m_EnemyType.ToString();
            }
        }
        public override void OnAttach()
        {
            base.OnAttach();

            if (this.AttachedTo is Mobile)
            {
                Mobile m = this.AttachedTo as Mobile;
                Effects.PlaySound(m, m.Map, 516);
                m.SendMessage(String.Format("You gain the power of Faction Mastery over {0}", this.EnemyFaction));
            }
        }

        // note that this method will be called when attached to either a mobile or a weapon
        // when attached to a weapon, only that weapon will do additional damage
        // when attached to a mobile, any weapon the mobile wields will do additional damage
        public override void OnWeaponHit(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven)
        {
            if (this.m_Chance <= 0 || Utility.Random(100) > this.m_Chance)
                return;

            if (defender != null && attacker != null && this.m_EnemyType != XmlMobFactions.GroupTypes.End_Unused)
            {
                // check to the owner's faction levels
                List<XmlAttachment> list = XmlAttach.FindAttachments(XmlAttach.MobileAttachments, attacker, typeof(XmlMobFactions), "Standard");

                if (list != null && list.Count > 0)
                {
                    XmlMobFactions x = list[0] as XmlMobFactions;

                    double increase = 0;

                    // go through all of the factions the defender might belong to
                    List<XmlMobFactions.GroupTypes> glist = XmlMobFactions.FindGroups(defender);

                    if (glist != null && glist.Count > 0)
                    {
                        foreach (XmlMobFactions.GroupTypes targetgroup in glist)
                        {
                            // found the group that matches the enemy type for this attachment
                            if (targetgroup == this.m_EnemyType)
                            {
                                // get the percent damage increase based upon total faction level of opponent groups
                                int totalfac = 0;
                                
                                // get the target enemy group
                                XmlMobFactions.Group g = XmlMobFactions.FindGroup(this.m_EnemyType);
                
                                if (g.Opponents != null)
                                {
                                    // go through all of the opponents of this group
                                    for (int i = 0; i < g.Opponents.Length; i++)
                                    {
                                        // and sum the faction levels
                                        try
                                        {
                                            totalfac += (int)(x.GetFactionLevel(g.Opponents[i].GroupType) * g.OpponentGain[i]);
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                                
                                // what is the damage increase based upon the total opponent faction level
                                increase = (double)this.ComputeIncrease(totalfac) / 100.0;

                                break;
                            }
                        }
                    }

                    if (increase > 0)
                    {
                        // apply the additional damage if any
                        defender.Damage((int)(damageGiven * increase), attacker);
                    }
                }
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if (this.AttachedTo is Mobile)
            {
                Mobile m = this.AttachedTo as Mobile;
                if (!m.Deleted)
                {
                    Effects.PlaySound(m, m.Map, 958);
                    m.SendMessage(String.Format("Your power of Faction Mastery over {0} fades..", this.EnemyFaction));
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write(this.m_PercentIncrease);
            writer.Write(this.m_PercentCap);
            writer.Write(this.m_Chance);
            writer.Write(this.m_Enemy);
            writer.Write(this.m_FactionScaling);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            this.m_PercentIncrease = reader.ReadInt();
            this.m_PercentCap = reader.ReadInt();
            this.m_Chance = reader.ReadInt();
            this.EnemyFaction = reader.ReadString();
            this.m_FactionScaling = reader.ReadDouble();
        }

        public override string OnIdentify(Mobile from)
        {
            if (from == null)
            {
                if (this.Expiration > TimeSpan.Zero)
                {
                    return String.Format("Faction Mastery : increased damage vs {0}, {1}% hitchance, expires in {2} mins",
                        this.m_Enemy, this.Chance, this.Expiration.TotalMinutes);
                }
                else
                {
                    return String.Format("Faction Mastery : increased damage vs {0}, {1}% hitchance", this.m_Enemy, this.Chance);
                }
            }

            string msg = null;
            string raisestr = "improve ";
            int percentincrease = 0;

            // compute the damage increase based upon the owner's faction level for the specified enemy type
            List<XmlAttachment> list = XmlAttach.FindAttachments(XmlAttach.MobileAttachments, from, typeof(XmlMobFactions), "Standard");
            if (list != null && list.Count > 0)
            {
                XmlMobFactions x = list[0] as XmlMobFactions;

                // get the percent damage increase based upon total faction level of opponent groups
                int totalfac = 0;
                
                // get the target enemy group
                XmlMobFactions.Group g = XmlMobFactions.FindGroup(this.m_EnemyType);

                if (g != null && g.Opponents != null)
                {
                    // go through all of the opponents
                    for (int i = 0; i < g.Opponents.Length; i++)
                    {
                        try
                        {
                            totalfac += (int)(x.GetFactionLevel(g.Opponents[i].GroupType) * g.OpponentGain[i]);
                            raisestr = String.Format("{0}{1}, ", raisestr, g.Opponents[i].GroupType);
                        }
                        catch
                        {
                        }
                    }
                }

                percentincrease = this.ComputeIncrease(totalfac);
            }
            
            if (this.Expiration > TimeSpan.Zero)
            {
                msg = String.Format("Faction Mastery : +{3}% damage vs {0} ({4}% max), {1}% hitchance, expires in {2} mins",
                    this.m_Enemy, this.Chance, this.Expiration.TotalMinutes, percentincrease, this.PercentCap);
            }
            else
            {
                msg = String.Format("Faction Mastery : +{2}% damage vs {0} ({3}% max), {1}% hitchance", this.m_Enemy, this.Chance, percentincrease, this.PercentCap);
            }

            msg = String.Format("{0} : {1}faction to enhance damage.", msg, raisestr);

            return msg;
        }

        private int ComputeIncrease(int faclevel)
        {
            // calculate the additional damage
            int val = (int)(this.PercentIncrease + faclevel * this.FactionScaling);
              
            if (val > this.PercentCap)
                val = this.PercentCap;
              
            return val;
        }
    }
}