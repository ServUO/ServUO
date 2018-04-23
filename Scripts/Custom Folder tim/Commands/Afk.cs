using Server.Commands;
using System;
using System.Collections;
using Server;

namespace Server.Commands
{
   /// <summary>
   /// Summary description for AFK.
   /// </summary>
   public class AFK : Timer
   {
      private static Hashtable m_AFK = new Hashtable();
      private Mobile who;
      private Point3D where;
      private DateTime when;
      public string what="";

      public static void Initialize()
      {
         CommandSystem.Register( "afk", AccessLevel.Player, new CommandEventHandler( AFK_OnCommand ) );
         EventSink.Logout += new LogoutEventHandler( OnLogout );
         EventSink.Speech += new SpeechEventHandler( OnSpeech );
         EventSink.PlayerDeath += new PlayerDeathEventHandler( OnDeath);
      }
      public static void OnDeath( PlayerDeathEventArgs e )
      {
         if ( m_AFK.Contains( e.Mobile.Serial.Value ) )
         {
            AFK afk=(AFK)m_AFK[e.Mobile.Serial.Value];
            if (afk==null)
            {
               e.Mobile.SendMessage("Afk object missing!");
               return;
            }
            e.Mobile.PlaySound(  e.Mobile.Female ? 814 : 1088 );
            afk.wakeUp();
         }
      }
      public static void OnLogout( LogoutEventArgs e )
      {
         if ( m_AFK.Contains( e.Mobile.Serial.Value ) )
         {
            AFK afk=(AFK)m_AFK[e.Mobile.Serial.Value];
            if (afk==null)
            {
               e.Mobile.SendMessage("Afk object missing!");
               return;
            }
            afk.wakeUp();
         }
      }
      public static void OnSpeech( SpeechEventArgs e )
      {
         if ( m_AFK.Contains( e.Mobile.Serial.Value ) )
         {
            AFK afk=(AFK)m_AFK[e.Mobile.Serial.Value];
            if (afk==null)
            {
               e.Mobile.SendMessage("Afk object missing!");
               return;
            }
            afk.wakeUp();
         }
      }
      public static void AFK_OnCommand( CommandEventArgs e )
      {
         if ( m_AFK.Contains( e.Mobile.Serial.Value ) )
         {
            AFK afk=(AFK)m_AFK[e.Mobile.Serial.Value];
            if (afk==null)
            {
               e.Mobile.SendMessage("Afk object missing!");
               return;
            }
            afk.wakeUp();
         }
         else
         {
            m_AFK.Add( e.Mobile.Serial.Value,new AFK(e.Mobile,e.ArgString.Trim()) );
            e.Mobile.SendMessage( "AFK enabled." );
         }
      }
      public void wakeUp()
      {
         m_AFK.Remove( who.Serial.Value );
         who.Emote("*is no longer AFK*");
         who.SendMessage( "AFK deactivated." );
         this.Stop();
      }
      public AFK(Mobile afker, string message) : base(TimeSpan.FromSeconds(10),TimeSpan.FromSeconds  (10))
      {
         if ((message==null)||(message=="")) message="is AFK";
         what=message;
         who=afker;
         when=DateTime.Now;
         where=who.Location;
         this.Start();
      }
      protected override void OnTick()
      {
         if (!(who.Location==where) )
         {
            this.wakeUp();
            return;
         }
         who.Say("zZz");
         TimeSpan ts=DateTime.Now.Subtract(when);
         who.Emote("*{0} ({1}:{2}:{3})*",what,ts.Hours,ts.Minutes,ts.Seconds);
         who.PlaySound(  who.Female ? 819 : 1093);
      }
   }
}