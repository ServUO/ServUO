#region References
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Server.Guilds;
using Server.Network;
#endregion

namespace Server.Misc
{
	public static class StatusPage
	{
		public static bool Enabled { get; set; }

		private static HttpListener _Listener;

		private static string _StatusPage = String.Empty;
		private static byte[] _StatusBuffer = new byte[0];

		private static readonly Timer _RefreshTimer;

		static StatusPage()
		{
			_RefreshTimer = Timer.DelayCall(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(60.0), OnTick);
		}

		public static void Initialize()
		{
			Listen();
		}

		private static void Listen()
		{
			if (!Enabled || !HttpListener.IsSupported)
			{
				return;
			}

			if (_Listener == null)
			{
				_Listener = new HttpListener();
				_Listener.Prefixes.Add("http://*:80/status/");
				_Listener.Start();
			}
			else if (!_Listener.IsListening)
			{
				_Listener.Start();
			}

			if (_Listener.IsListening)
			{
				_ = _Listener.BeginGetContext(ListenerCallback, null);
			}
		}

		private static void ListenerCallback(IAsyncResult result)
		{
			try
			{
				var context = _Listener.EndGetContext(result);

				context.Response.ContentLength64 = _StatusBuffer.Length;

				context.Response.OutputStream.Write(_StatusBuffer, 0, _StatusBuffer.Length);
				context.Response.OutputStream.Close();
			}
			catch (Exception e)
			{
				Diagnostics.ExceptionLogging.LogException(e);
			}

			Listen();
		}

		private static string Encode(string input)
		{
			var sb = new StringBuilder(input);

			_ = sb.Replace("&", "&amp;");
			_ = sb.Replace("<", "&lt;");
			_ = sb.Replace(">", "&gt;");
			_ = sb.Replace("\"", "&quot;");
			_ = sb.Replace("'", "&apos;");

			return sb.ToString();
		}

		private static void OnTick()
		{
			if (!Enabled)
			{
				return;
			}

			_ = Directory.CreateDirectory("web");

			using (var op = new StreamWriter("web/status.html"))
			{
				op.WriteLine("<!DOCTYPE html>");
				op.WriteLine("<html>");
				op.WriteLine("   <head>");
				op.WriteLine("      <title>" + ServerList.ServerName + " Server Status</title>");
				op.WriteLine("   </head>");
				op.WriteLine("   <style type=\"text/css\">");
				op.WriteLine("   body { background: #999; }");
				op.WriteLine("   table { width: 100%; }");
				op.WriteLine("   tr.ruo-header td { background: #000; color: #FFF; }");
				op.WriteLine("   tr.odd td { background: #222; color: #DDD; }");
				op.WriteLine("   tr.even td { background: #DDD; color: #222; }");
				op.WriteLine("   </style>");
				op.WriteLine("   <body>");
				op.WriteLine("      <h1>" + ServerList.ServerName + "Server Status</h1>");
				op.WriteLine("      <h3>Online clients</h3>");
				op.WriteLine("      <table cellpadding=\"0\" cellspacing=\"0\">");
				op.WriteLine("         <tr class=\"ruo-header\"><td>Name</td><td>Location</td><td>Kills</td><td>Karma/Fame</td></tr>");

				var index = 0;

				foreach (var m in NetState.Instances.Where(state => state.Mobile != null).Select(state => state.Mobile))
				{
					++index;

					op.Write("         <tr class=\"ruo-result " + (index % 2 == 0 ? "even" : "odd") + "\"><td>");

					if (m.Guild is Guild g)
					{
						op.Write(Encode(m.Name));
						op.Write(" [");

						var title = m.GuildTitle?.Trim();

						if (title?.Length > 0)
						{
							op.Write(Encode(title));
							op.Write(", ");
						}

						op.Write(Encode(g.Abbreviation));

						op.Write(']');
					}
					else
					{
						op.Write(Encode(m.Name));
					}

					op.Write("</td><td>");
					op.Write(m.X);
					op.Write(", ");
					op.Write(m.Y);
					op.Write(", ");
					op.Write(m.Z);
					op.Write(" (");
					op.Write(m.Map);
					op.Write(")</td><td>");
					op.Write(m.Kills);
					op.Write("</td><td>");
					op.Write(m.Karma);
					op.Write(" / ");
					op.Write(m.Fame);
					op.WriteLine("</td></tr>");
				}

				op.WriteLine("         <tr>");
				op.WriteLine("      </table>");
				op.WriteLine("   </body>");
				op.WriteLine("</html>");
			}

			_StatusPage = File.ReadAllText("web/status.html");
			_StatusBuffer = Encoding.UTF8.GetBytes(_StatusPage);
		}
	}
}
