using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Engines.MyrmidexInvasion
{
    public enum Allegiance
    {
        None = 0,
        Myrmidex = 1156634,
        Tribes = 1156635
    }

    public class MyrmidexInvasionSystem
    {
        public static readonly bool Active = true;

        public static string FilePath = Path.Combine("Saves", "MyrmidexInvasion.bin");
        public static MyrmidexInvasionSystem System { get; set; }

        public static List<AllianceEntry> AllianceEntries { get; set; }

        public MyrmidexInvasionSystem()
        {
            AllianceEntries = new List<AllianceEntry>();
        }

        public void Join(PlayerMobile pm, Allegiance type)
        {
            AllianceEntry entry = GetEntry(pm);

            if (entry != null)
                AllianceEntries.Remove(entry);

            pm.SendLocalizedMessage(1156636, string.Format("#{0}", ((int)type).ToString())); // You have declared allegiance to the ~1_SIDE~!  You may only change your allegiance once every 2 hours.

            AllianceEntries.Add(new AllianceEntry(pm, type));
        }

        public static bool IsAlliedWith(Mobile a, Mobile b)
        {
            return (IsAlliedWithMyrmidex(a) && IsAlliedWithMyrmidex(b)) || (IsAlliedWithEodonTribes(a) && IsAlliedWithEodonTribes(b));
        }

        public static bool AreEnemies(Mobile a, Mobile b)
        {
            if ((IsAlliedWithEodonTribes(a) && !IsAlliedWithMyrmidex(b)) || (IsAlliedWithEodonTribes(b) && !IsAlliedWithMyrmidex(a)) ||
                (IsAlliedWithMyrmidex(a) && !IsAlliedWithEodonTribes(b)))
                return false;

            return !IsAlliedWith(a, b);
        }

        public static bool IsAlliedWith(Mobile m, Allegiance allegiance)
        {
            return allegiance == Allegiance.Myrmidex ? IsAlliedWithMyrmidex(m) : IsAlliedWithEodonTribes(m);
        }

        public static bool IsAlliedWithMyrmidex(Mobile m)
        {
            if (m is BaseCreature)
            {
                BaseCreature bc = m as BaseCreature;

                if (bc.GetMaster() != null)
                    return IsAlliedWithMyrmidex(bc.GetMaster());

                return m is MyrmidexLarvae || m is MyrmidexWarrior || m is MyrmidexQueen || m is MyrmidexDrone || (m is BaseEodonTribesman && ((BaseEodonTribesman)m).TribeType == EodonTribe.Barrab);
            }

            AllianceEntry entry = GetEntry(m as PlayerMobile);

            return entry != null && entry.Allegiance == Allegiance.Myrmidex;
        }

        public static bool IsAlliedWithEodonTribes(Mobile m)
        {
            if (m is BaseCreature)
            {
                BaseCreature bc = m as BaseCreature;

                if (bc.GetMaster() != null)
                    return IsAlliedWithEodonTribes(bc.GetMaster());

                return (m is BaseEodonTribesman && ((BaseEodonTribesman)m).TribeType != EodonTribe.Barrab) || m is BritannianInfantry;
            }

            AllianceEntry entry = GetEntry(m as PlayerMobile);

            return entry != null && entry.Allegiance == Allegiance.Tribes;
        }

        public static bool CanRecieveQuest(PlayerMobile pm, Allegiance allegiance)
        {
            AllianceEntry entry = GetEntry(pm);

            return entry != null && entry.Allegiance == allegiance && entry.CanRecieveQuest;
        }

        public static AllianceEntry GetEntry(PlayerMobile pm)
        {
            if (pm == null)
                return null;

            return AllianceEntries.FirstOrDefault(e => e.Player == pm);
        }

        public static void Configure()
        {
            System = new MyrmidexInvasionSystem();

            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;

            CommandSystem.Register("GetAllianceEntry", AccessLevel.GameMaster, e =>
            {
                e.Mobile.BeginTarget(10, false, TargetFlags.None, (from, targeted) =>
                    {
                        if (targeted is PlayerMobile)
                        {
                            AllianceEntry entry = GetEntry((PlayerMobile)targeted);

                            if (entry != null)
                            {
                                ((PlayerMobile)targeted).SendGump(new PropertiesGump((PlayerMobile)targeted, entry));
                            }
                            else
                                e.Mobile.SendMessage("They don't belong to an alliance.");
                        }
                    });
            });
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0);

                    writer.Write(AllianceEntries.Count);
                    AllianceEntries.ForEach(entry => entry.Serialize(writer));

                    writer.Write(MoonstonePowerGeneratorAddon.Boss);
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
               FilePath,
               reader =>
               {
                   int version = reader.ReadInt();

                   int count = reader.ReadInt();
                   for (int i = 0; i < count; i++)
                   {
                       AllianceEntry entry = new AllianceEntry(reader);

                       if (entry.Player != null)
                           AllianceEntries.Add(entry);
                   }

                   MoonstonePowerGeneratorAddon.Boss = reader.ReadMobile() as Zipactriotl;
               });
        }
    }

    [PropertyObject]
    public class AllianceEntry
    {
        public PlayerMobile Player { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Allegiance Allegiance { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime JoinTime { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanRecieveQuest { get; set; }

        public AllianceEntry(PlayerMobile pm, Allegiance allegiance)
        {
            Player = pm;
            Allegiance = allegiance;
            JoinTime = DateTime.UtcNow;
        }

        public AllianceEntry(GenericReader reader)
        {
            int version = reader.ReadInt();

            Player = reader.ReadMobile() as PlayerMobile;
            Allegiance = (Allegiance)reader.ReadInt();
            JoinTime = reader.ReadDateTime();
            CanRecieveQuest = reader.ReadBool();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Player);
            writer.Write((int)Allegiance);
            writer.Write(JoinTime);
            writer.Write(CanRecieveQuest);
        }
    }
}