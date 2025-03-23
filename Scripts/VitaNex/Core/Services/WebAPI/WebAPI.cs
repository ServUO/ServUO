#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Server;
using Server.Misc;

using VitaNex.IO;
using VitaNex.Text;
#endregion

namespace VitaNex.Web
{
	public static partial class WebAPI
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		public static WebAPIOptions CSOptions { get; private set; }

		public static Dictionary<string, WebAPIHandler> Handlers { get; private set; }

		private static readonly char[] _QuerySplit = { '&' };

		private static bool _ServerStarted;
		private static bool _Listening;

		private static readonly byte[] _EmptyBuffer = new byte[0];

		private static PollTimer _ActivityTimer;

		public static TcpListener Listener { get; private set; }

		public static List<WebAPIClient> Clients { get; private set; }

		public static event WebAPIContextHandler ContextHandler;

		public static event WebAPIClientConnected ClientConnected;
		public static event WebAPIClientDisconnected ClientDisconnected;

		public static event WebAPIRequestSend<object> RequestSend;
		public static event WebAPIRequestReceive<object> RequestReceive;

		private static void HandleRoot(WebAPIContext context)
		{
			if (!CSOptions.WebServer)
			{
				context.Response.Status = HttpStatusCode.ServiceUnavailable;
				return;
			}

			var root = IOUtility.EnsureDirectory(Core.BaseDirectory + "/web");
			var path = IOUtility.GetSafeFilePath(root + "/" + context.Uri, true);

			if (Path.HasExtension(path))
			{
				try
				{
					var file = new FileInfo(path);

					if (Insensitive.StartsWith(file.FullName, root.FullName))
					{
						context.Response.FileName = file.Name;
						context.Response.Data = file.Directory;
					}
					else
					{
						context.Response.Status = HttpStatusCode.Forbidden;
					}
				}
				catch
				{
					context.Response.Status = HttpStatusCode.BadRequest;
				}
			}
			else if (Directory.Exists(path))
			{
				try
				{
					var dir = new DirectoryInfo(path);

					if (Insensitive.StartsWith(dir.FullName, root.FullName))
					{
						context.Response.Data = dir;
					}
					else
					{
						context.Response.Status = HttpStatusCode.Forbidden;
					}
				}
				catch
				{
					context.Response.Status = HttpStatusCode.BadRequest;
				}
			}
			else
			{
				context.Response.Status = HttpStatusCode.BadRequest;
			}
		}

		public static void Connect(WebAPIClient client)
		{
			try
			{
				lock (Clients)
				{
					if (Clients.Contains(client))
					{
						return;
					}

					Clients.Add(client);
				}

				var info = client.ToString();

				CSOptions.ToConsole("[{0}] Client Connected: {1}", Clients.Count, info);

				if (ClientConnected != null)
				{
					ClientConnected(client);
				}

				if (!client.IsDisposed && client.Connected)
				{
					if (!ClientUtility.HandleConnection(client) && CSOptions.ServiceDebug)
					{
						CSOptions.ToConsole("Unhandled Request: {0}", info);
					}
				}
				else
				{
					Disconnect(client);
				}
			}
			catch (Exception e)
			{
				CSOptions.ToConsole(e);

				Disconnect(client);
			}
		}

		public static void Disconnect(WebAPIClient client)
		{
			try
			{
				lock (Clients)
				{
					if (!Clients.Remove(client))
					{
						return;
					}
				}

				if (!client.IsDisposed)
				{
					var info = client.ToString();

					if (ClientDisconnected != null)
					{
						ClientDisconnected(client);
					}

					client.Close(true);

					CSOptions.ToConsole("[{0}] Client Disconnected: {1}", Clients.Count, info);
				}
			}
			catch (Exception e)
			{
				CSOptions.ToConsole(e);

				lock (Clients)
				{
					Clients.Remove(client);
				}

				client.Close(true);
			}
		}

		public static bool Register(string uri, Action<WebAPIContext> handler)
		{
			if (uri == null)
			{
				return false;
			}

			uri = uri.TrimEnd('/');

			if (!uri.StartsWith("/"))
			{
				uri = "/" + uri;
			}

			var h = Handlers.GetValue(uri);

			if (h == null)
			{
				Handlers[uri] = h = new WebAPIHandler(uri, handler);
			}
			else
			{
				h.Handler = handler;
			}

			return h.Handler == handler;
		}

		public static bool Unregister(string uri)
		{
			if (uri == null)
			{
				return false;
			}

			uri = uri.TrimEnd('/');

			if (!uri.StartsWith("/"))
			{
				uri = "/" + uri;
			}

			var h = Handlers.GetValue(uri);

			if (h != null)
			{
				h.Handler = null;
			}

			return Handlers.Remove(uri);
		}

		public static string EncodeQuery(IEnumerable<KeyValueString> queries)
		{
			return EncodeQuery(queries.Select(o => (KeyValuePair<string, string>)o));
		}

		public static string EncodeQuery(IEnumerable<KeyValuePair<string, string>> queries)
		{
			var value = "?" + String.Join("&", queries.Select(kv => String.Format("{0}={1}", kv.Key, kv.Value)));

            return WebUtility.UrlEncode(value);
        }

        public static IEnumerable<KeyValuePair<string, string>> DecodeQuery(string query)
		{
			if (String.IsNullOrWhiteSpace(query))
			{
				yield break;
			}

			query = WebUtility.UrlDecode(query);

			query = query.Substring(query.IndexOf('?') + 1);

			if (String.IsNullOrWhiteSpace(query))
			{
				yield break;
			}

			foreach (var kv in query.Split(_QuerySplit, StringSplitOptions.RemoveEmptyEntries))
			{
				if (ExtractKeyValuePair(kv, out var key, out var value))
				{
					yield return new KeyValuePair<string, string>(key, value);
				}
			}
		}

		private static bool ExtractKeyValuePair(string kv, out string key, out string value)
		{
			key = kv;
			value = String.Empty;

			var eq = kv.IndexOf('=');

			if (eq <= 0)
			{
				return !String.IsNullOrWhiteSpace(key);
			}

			key = kv.Substring(0, eq);
			value = String.Empty;

			if (++eq < kv.Length)
			{
				value = kv.Substring(eq);
			}

			return !String.IsNullOrWhiteSpace(key);
		}

		public static void SetContent(this HttpWebRequest request, string content)
		{
			SetContent(request, content, Encoding.UTF8);
		}

		public static void SetContent(this HttpWebRequest request, string content, Encoding enc)
		{
			try
			{
				request.Headers[HttpRequestHeader.ContentEncoding] = enc.WebName;

				if (String.IsNullOrEmpty(content))
				{
					return;
				}

				using (var s = request.GetRequestStream())
				{
					var buf = new byte[Math.Min(4096, content.Length * 2)];

					int idx = 0, len, cnt;

					while ((len = enc.GetBytes(content, idx, cnt = content.Length - idx, buf, 0)) > 0)
					{
						s.Write(buf, 0, len);
						s.Flush();

						if ((idx += cnt) >= content.Length)
						{
							break;
						}
					}
				}
			}
			catch (ObjectDisposedException)
			{ }
			catch (Exception e)
			{
				CSOptions.ToConsole(e);
			}
		}

		public static string GetContent(this HttpWebResponse response)
		{
			try
			{
				using (var s = response.GetResponseStream())
				{
					if (s == null)
					{
						return String.Empty;
					}

					var enc = Encoding.UTF8;

					if (!String.IsNullOrWhiteSpace(response.ContentEncoding))
					{
						enc = Encoding.GetEncoding(response.ContentEncoding);
					}

					using (var r = new StreamReader(s, enc))
					{
						char[] b = null;
						StringBuilder c = null;

						int len;

						while (r.Peek() >= 0)
						{
							if (b == null)
							{
								b = new char[4096];
								c = new StringBuilder();
							}

							while ((len = r.Read(b, 0, b.Length)) > 0)
							{
								c.Append(b, 0, len);
							}
						}

						if (c != null)
						{
							return c.ToString();
						}
					}
				}
			}
			catch (ObjectDisposedException)
			{ }
			catch (Exception e)
			{
				CSOptions.ToConsole(e);
			}

			return String.Empty;
		}

		public static void BeginRequest<T>(Uri uri, T state, WebAPIRequestSend<T> send, WebAPIRequestReceive<T> receive)
		{
			BeginRequest(uri.ToString(), state, send, receive);
		}

		public static void BeginRequest<T>(string uri, T state, WebAPIRequestSend<T> send, WebAPIRequestReceive<T> receive)
		{
			try
			{
				CSOptions.ToConsole("Requesting: {0}", uri);

				var request = (HttpWebRequest)WebRequest.Create(uri);

				request.UserAgent = "VitaNexCore/" + VitaNexCore.Version + " " + CSOptions.Service.FullName;

				// ReSharper disable once AssignNullToNotNullAttribute
				request.Proxy = null;
				request.Credentials = null;

				try
				{
					send?.Invoke(request, state);
				}
				catch (Exception e)
				{
					CSOptions.ToConsole(e);
				}

				try
				{
					RequestSend?.Invoke(request, state);
				}
				catch (Exception e)
				{
					CSOptions.ToConsole(e);
				}

				RequestUtility.BeginGetResponse(request, receive, state);
			}
			catch (Exception e)
			{
				CSOptions.ToConsole(e);
			}
		}

		private static class ListenerUtility
		{
			public static bool Resolve(string addr, out IPAddress outValue)
			{
				addr = addr.Trim();

				if (IPAddress.TryParse(addr, out outValue))
				{
					return true;
				}

				try
				{
					var iphe = Dns.GetHostEntry(addr);

					outValue = iphe.AddressList.FirstOrDefault();

					return true;
				}
				catch
				{
					outValue = IPAddress.None;

					return false;
				}
			}

			public static void AcquireListener()
			{
				if (Listener != null && ((IPEndPoint)Listener.LocalEndpoint).Port != CSOptions.Port)
				{
					ReleaseListener();
				}

				if (Listener == null)
				{
					var ipep = Server.Network.Listener.EndPoints.FirstOrDefault(ep => !ep.Address.IsPrivateNetwork());
					var ip = ipep != null ? ipep.Address : IPAddress.Any;

					Listener = new TcpListener(ip, CSOptions.Port)
					{
						ExclusiveAddressUse = false
					};
				}

				if (!Listener.Server.IsBound)
				{
					try
					{
						Listener.Start(CSOptions.MaxConnections);
					}
					catch (Exception e)
					{
						CSOptions.ToConsole(e);

						if (Insensitive.Contains(e.ToString(), "access permissions"))
						{
							CSOptions.ToConsole(
								"Another process may be bound to port {0}.\n" +
								"The WebAPI service requires port {0} to be unbound and available for use on the target IP Address.",
								CSOptions.Port);
						}
					}

					if (!Listener.Server.IsBound)
					{
						return;
					}

					var ipep = Listener.LocalEndpoint as IPEndPoint;

					if (ipep != null)
					{
						foreach (var ip in ipep.Address.FindInternal())
						{
							CSOptions.ToConsole("Listening: {0}:{1}", ip, ipep.Port);
						}
					}
				}

				_Listening = true;
			}

			public static void ReleaseListener()
			{
				if (Listener == null)
				{
					return;
				}

				try
				{
					if (Listener.Server.IsBound)
					{
						Listener.Server.Disconnect(true);
					}
				}
				catch
				{ }

				try
				{
					Listener.Stop();
				}
				catch
				{ }

				Listener = null;

				_Listening = false;
			}

			public static void ListenAsync()
			{
				AcquireListener();

				if (Listener == null)
				{
					return;
				}

				try
				{
					Listener.BeginAcceptTcpClient(EndAcceptTcpClient, null);
				}
				catch (SocketException)
				{ }
				catch (ObjectDisposedException)
				{ }
				catch (Exception e)
				{
					CSOptions.ToConsole(e);
				}
			}

			private static void EndAcceptTcpClient(IAsyncResult r)
			{
				TcpClient client;

				try
				{
					client = Listener.EndAcceptTcpClient(r);
				}
				catch (Exception e)
				{
					CSOptions.ToConsole(e);
					return;
				}
				finally
				{
					ListenAsync();
				}

				if (client.Connected && _ServerStarted)
				{
					var ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address;

					var allow = !CSOptions.UseWhitelist;

					if (allow)
					{
						allow = !CSOptions.Blacklist.Any(l => Utility.IPMatch(l, ip)) && !Firewall.IsBlocked(ip);
					}

					if (!allow && CSOptions.UseWhitelist)
					{
						allow = CSOptions.Whitelist.Any(l => Utility.IPMatch(l, ip));
					}

					if (allow)
					{
						Connect(new WebAPIClient(client));
						return;
					}
				}

				client.Close();
			}
		}

		private static class ClientUtility
		{
			private static readonly object _Lock = new object();

			public static bool HandleConnection(WebAPIClient client)
			{
				try
				{
					using (client)
					{	
						if (!client.ReceiveHeaders(out var headers))
						{
							return false;
						}
	
						var m = headers[0].Key;
	
						if (String.IsNullOrWhiteSpace(m))
						{
							return false;
						}
	
						if (!Enum.TryParse(m, out WebAPIMethod method))
						{
							return false;
						}
	
						var u = headers[0].Value;
						var i = u.LastIndexOf(' ');
	
						if (i > -1)
						{
							u = u.Substring(0, i);
						}

						u = WebUtility.UrlDecode(u);

						if (String.IsNullOrWhiteSpace(u))
						{
							u = "/";
						}
	
						using (var context = new WebAPIContext(client, method, u))
						{
							foreach (var h in headers.Skip(1))
							{
								context.Request.Headers[h.Key] = h.Value;
							}
	
							foreach (var q in DecodeQuery(u))
							{
								context.Request.Queries[q.Key] = q.Value;
							}
	
							if (!String.IsNullOrWhiteSpace(context.Request.Headers["Content-Type"]))
							{
								context.Request.ContentType = context.Request.Headers["Content-Type"];
							}
	
							var length = 0;
	
							if (!String.IsNullOrWhiteSpace(context.Request.Headers["Content-Length"]))
							{
								Int32.TryParse(context.Request.Headers["Content-Length"], out length);
							}
	
							if (Insensitive.Contains(context.Request.Headers["Accept-Encoding"], "deflate"))
							{
								context.Response.Compress = true;
							}
	
							var encoding = Encoding.UTF8;
	
							if (!String.IsNullOrWhiteSpace(context.Request.Headers["Accept-Charset"]))
							{
								var h = context.Request.Headers["Accept-Charset"].Trim();
	
								if (h.Contains(','))
								{
									foreach (var e in h.Split(','))
									{
										try
										{
											encoding = Encoding.GetEncoding(e.Trim());
										}
										catch
										{
											encoding = Encoding.UTF8;
										}
									}
								}
								else
								{
									try
									{
										encoding = Encoding.GetEncoding(h);
									}
									catch
									{
										encoding = Encoding.UTF8;
									}
								}
							}
	
							context.Request.Encoding = context.Response.Encoding = encoding;
	
							context.Response.Headers["Date"] = DateTime.UtcNow.ToSimpleString("D, d M y t@h:m:s@") + " GMT";
							context.Response.Headers["Server"] = String.Format(
								"Vita-Nex: Core/{0} [{1}/{2}] ({3})",
								VitaNexCore.Version,
								CSOptions.ServiceName,
								CSOptions.ServiceVersion,
								ServerList.ServerName);
	
							if (!context.Method.AnyFlags(WebAPIMethod.OPTIONS, WebAPIMethod.GET, WebAPIMethod.POST))
							{
								context.Response.Headers["Allow"] = "OPTIONS, GET, POST";
								context.Response.Headers["Connection"] = "close";
	
								client.Send(false, "HTTP/1.1 405 Method Not Allowed\r\n" + context.Response.Headers, Encoding.ASCII);
								return true;
							}
	
							if (context.Method == WebAPIMethod.OPTIONS)
							{
								if (!String.IsNullOrWhiteSpace(context.Request.Headers["Origin"]))
								{
									context.Response.Headers["Access-Control-Allow-Methods"] = "POST, GET, OPTIONS";
									context.Response.Headers["Access-Control-Allow-Headers"] = "Origin, X-Requested-With, Content-Type, Accept";
									context.Response.Headers["Access-Control-Allow-Origin"] = context.Request.Headers["Origin"];
								}
	
								context.Response.Headers["Vary"] = "Accept-Encoding";
								context.Response.Headers["Keep-Alive"] = "timeout=2, max=120";
								context.Response.Headers["Connection"] = "keep-alive";
	
								client.Send(false, "HTTP/1.1 200 OK\r\n" + context.Response.Headers, Encoding.ASCII);
								return true;
							}
	
							if (length > CSOptions.MaxReceiveBufferSizeBytes)
							{
								context.Response.Headers["Connection"] = "close";
	
								client.Send(false, "HTTP/1.1 413 Request Entity Too Large\r\n" + context.Response.Headers, Encoding.ASCII);
								return true;
							}
	
							var key = u.Trim();
							var idx = u.IndexOf('?');
	
							if (idx > 0)
							{
								key = u.Substring(0, idx);
							}
	
							if (key.Length > 1)
							{
								key = key.TrimEnd('/');
							}
	
							if (!Handlers.TryGetValue(key, out var handler) || handler == null)
							{
								key = "/";
							}
	
							byte[] buffer;
	
							if (handler != null || (Handlers.TryGetValue(key, out handler) && handler != null))
							{
								try
								{
									if (length > 0)
									{	
										client.Receive(false, context.Request.Encoding, out var data, out buffer, out length);
	
										context.Request.Data = data;
										context.Request.Length = length;
									}
	
									handler.Handler(context);
								}
								catch (Exception e)
								{
									CSOptions.ToConsole(e);
	
									if (e is InternalBufferOverflowException)
									{
										context.Response.Status = HttpStatusCode.RequestEntityTooLarge;
									}
									else
									{
										context.Response.Status = HttpStatusCode.InternalServerError;
									}
								}
							}
							else
							{
								context.Response.Status = HttpStatusCode.NotFound;
							}
	
							if (ContextHandler != null)
							{
								ContextHandler(context);
							}
	
							string status;
	
							if ((int)context.Response.Status >= 400)
							{
								context.Response.Headers["Connection"] = "close";
	
								status = String.Format("{0} {1}", (int)context.Response.Status, context.Response.Status.ToString().SpaceWords());
	
								client.Send(false, "HTTP/1.1 " + status + "\r\n" + context.Response.Headers, Encoding.ASCII);
								return true;
							}
	
							var encoded = false;
							var compressed = false;
	
							try
							{
								GetResponseBuffer(context, out buffer, out length, out encoded);
	
								if (length > 0 && context.Response.Compress)
								{
									client.Compress(ref buffer, ref length);
									compressed = true;
								}
							}
							catch (Exception e)
							{
								CSOptions.ToConsole(e);
	
								buffer = _EmptyBuffer;
								length = 0;
	
								if (e is InternalBufferOverflowException)
								{
									context.Response.Status = HttpStatusCode.RequestEntityTooLarge;
								}
								else
								{
									context.Response.Status = HttpStatusCode.InternalServerError;
								}
							}
	
							if (!String.IsNullOrWhiteSpace(context.Request.Headers["Origin"]))
							{
								context.Response.Headers["Access-Control-Allow-Origin"] = context.Request.Headers["Origin"];
							}
	
							if (String.IsNullOrWhiteSpace(context.Response.Headers["Vary"]))
							{
								context.Response.Headers["Vary"] = "Accept-Encoding";
							}
	
							if (length > 0)
							{
								if (compressed)
								{
									context.Response.Headers["Content-Encoding"] = "deflate";
								}
	
								if (context.Response.ContentType.IsDefault && !String.IsNullOrWhiteSpace(context.Response.FileName))
								{
									var mime = FileMime.Lookup(context.Response.FileName);
	
									if (!mime.IsDefault && mime != context.Response.ContentType)
									{
										context.Response.ContentType = mime;
									}
								}
	
								var contentType = context.Response.ContentType.MimeType;
	
								if (encoded)
								{
									contentType = String.Format("{0}; charset={1}", contentType, context.Response.Encoding.WebName);
								}
	
								context.Response.Headers["Content-Type"] = contentType;
								context.Response.Headers["Content-Length"] = length.ToString();
	
								if (!String.IsNullOrWhiteSpace(context.Response.FileName))
								{
									var inline = context.Response.ContentType.IsCommonText() || context.Response.ContentType.IsCommonImage();
	
									var disp = inline ? "inline" : "attachment";
	
									disp = String.Format("{0}; filename=\"{1}\"", disp, context.Response.FileName);
	
									context.Response.Headers["Content-Disposition"] = disp;
								}
							}
	
							if (context.Response.Cache < 0)
							{
								context.Response.Headers["Pragma"] = "no-cache";
								context.Response.Headers["Cache-Control"] = "no-cache, no-store";
							}
							else if (context.Response.Cache > 0)
							{
								context.Response.Headers["Cache-Control"] = "max-age=" + context.Response.Cache;
							}
	
							if (String.IsNullOrWhiteSpace(context.Response.Headers["Connection"]))
							{
								context.Response.Headers["Connection"] = "close";
							}
	
							status = String.Format("{0} {1}", (int)context.Response.Status, context.Response.Status.ToString().SpaceWords());
	
							client.Send(false, "HTTP/1.1 " + status + "\r\n" + context.Response.Headers, Encoding.ASCII);
	
							if (buffer.Length > 0 && length > 0)
							{
								client.Send(false, ref buffer, ref length);
							}
						}
	
						return true;
					}
				}
				catch (Exception x)
				{
					CSOptions.ToConsole(x);
					return false;
				}
			}

			private static bool FromImage(WebAPIContext context, Image image, out byte[] buffer, out int length)
			{
				buffer = _EmptyBuffer;
				length = 0;

				if (image == null)
				{
					context.Response.Status = HttpStatusCode.NotFound;
					return false;
				}

				using (var ms = new MemoryStream())
				{
					lock (_Lock)
					{
						try
						{
							image.Save(ms, ImageFormat.Png);
						}
						catch
						{
							using (var clone = new Bitmap(image))
							{
								clone.Save(ms, ImageFormat.Png);
							}
						}
					}

					buffer = ms.ToArray();
					length = buffer.Length;
				}

				if (String.IsNullOrWhiteSpace(context.Response.FileName))
				{
					context.Response.FileName = image.GetHashCode() + ".png";
				}
				else if (!Insensitive.EndsWith(context.Response.FileName, ".png"))
				{
					context.Response.FileName += ".png";
				}

				context.Response.ContentType = context.Response.FileName;
				context.Response.Status = HttpStatusCode.OK;

				return true;
			}

			private static bool FromFile(WebAPIContext c, FileInfo file, out byte[] buffer, out int length, out bool enc)
			{
				buffer = _EmptyBuffer;
				length = 0;
				enc = false;

				if (file == null)
				{
					c.Response.Status = HttpStatusCode.NotFound;
					return false;
				}

				file.Refresh();

				if (!file.Exists)
				{
					c.Response.Status = HttpStatusCode.NotFound;
					return false;
				}

				if (file.Length > CSOptions.MaxSendBufferSizeBytes)
				{
					c.Response.Status = HttpStatusCode.RequestEntityTooLarge;
					return false;
				}

				buffer = file.ReadAllBytes();
				length = buffer.Length;

				c.Response.FileName = file.Name;
				c.Response.ContentType = file;
				c.Response.Status = HttpStatusCode.OK;

				enc = c.Response.ContentType.IsCommonText();

				return true;
			}

			private static bool FromDir(WebAPIContext c, DirectoryInfo dir, out byte[] buffer, out int length, out bool enc)
			{
				buffer = _EmptyBuffer;
				length = 0;
				enc = false;

				if (dir == null)
				{
					c.Response.Status = HttpStatusCode.NotFound;
					return false;
				}

				dir.Refresh();

				if (!dir.Exists)
				{
					c.Response.Status = HttpStatusCode.NotFound;
					return false;
				}

				var name = "/" + dir.Name;

				if (Insensitive.Equals(dir.Name, "web"))
				{
					name = "/";
				}

				var html = new StringBuilder();

				html.AppendLine("<!DOCTYPE html>");
				html.AppendLine("<html>");
				html.AppendLine("\t<head>");
				html.AppendLine("\t\t<title>Index of {0}</title>", name);
				html.AppendLine("\t</head>");
				html.AppendLine("\t<body>");
				html.AppendLine("\t\t<h1>Index of {0}</h1>", name);
				html.AppendLine("\t\t<ul>");

				if (name != "/")
				{
					html.AppendLine("\t\t\t<li><a href='/'>Parent Directory</a></li>");
				}

				foreach (var o in dir.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly).OrderByNatural(o => o.Name))
				{
					if (o is FileInfo)
					{
						html.AppendLine("\t\t\t<li><a href='{0}' alt='{0}'>{0}</a></li>", o.Name);
					}
					else if (o is DirectoryInfo)
					{
						html.AppendLine("\t\t\t<li><a href='{0}/' alt='{0}'>{0}/</a></li>", o.Name);
					}
				}

				html.AppendLine("\t\t</ul>");
				html.AppendLine("\t</body>");
				html.AppendLine("</html>");

				enc = true;

				c.Client.Encode(c.Response.Encoding, html.ToString(), out buffer, out length);

				c.Response.FileName = "index.html";
				c.Response.ContentType = "html";
				c.Response.Status = HttpStatusCode.OK;

				return true;
			}

			private static void GetResponseBuffer(WebAPIContext context, out byte[] buffer, out int length, out bool encoded)
			{
				buffer = _EmptyBuffer;
				length = 0;
				encoded = false;

				if (context.Response.Data == null)
				{
					return;
				}

				if (context.Response.Data is byte[])
				{
					buffer = (byte[])context.Response.Data;
					length = buffer.Length;

					encoded = context.Response.ContentType.IsCommonText();

					return;
				}

				if (context.Response.Data is Image)
				{
					var image = (Image)context.Response.Data;

					if (FromImage(context, image, out buffer, out length))
					{
						context.Response.Status = HttpStatusCode.OK;
					}

					return;
				}

				if (context.Response.Data is DirectoryInfo)
				{
					var dir = (DirectoryInfo)context.Response.Data;

					FileInfo file = null;

					if (!String.IsNullOrWhiteSpace(context.Response.FileName))
					{
						file = new FileInfo(IOUtility.GetSafeFilePath(dir + "/" + context.Response.FileName, true));
					}

					if (file == null || !file.Exists)
					{
						file = new FileInfo(IOUtility.GetSafeFilePath(dir + "/index.html", true));
					}

					if (FromFile(context, file, out buffer, out length, out encoded) ||
						(CSOptions.DirectoryIndex && FromDir(context, dir, out buffer, out length, out encoded)))
					{
						context.Response.Status = HttpStatusCode.OK;
					}

					return;
				}

				if (context.Response.Data is FileInfo)
				{
					var file = (FileInfo)context.Response.Data;

					if (FromFile(context, file, out buffer, out length, out encoded))
					{
						context.Response.Status = HttpStatusCode.OK;
					}

					return;
				}

				string response;

				if (context.Response.Data is string || context.Response.Data is StringBuilder || context.Response.Data is ValueType)
				{
					response = context.Response.Data.ToString();

					if (!context.Response.ContentType.IsCommonText())
					{
						context.Response.ContentType = "txt";
					}
				}
				else
				{
					response = Json.Encode(context.Response.Data, out JsonException je) ?? String.Empty;

					if (je != null)
					{
						response = je.ToString();

						if (!context.Response.ContentType.IsCommonText())
						{
							context.Response.ContentType = "txt";
						}
					}
					else if (!String.IsNullOrWhiteSpace(response))
					{
						context.Response.ContentType = "json";
					}
				}

				if (String.IsNullOrWhiteSpace(context.Response.FileName))
				{
					context.Response.FileName = //
						Math.Abs(response.GetHashCode()) + "." + //
						context.Response.ContentType.Extension;
				}

				encoded = true;

				context.Client.Encode(context.Response.Encoding, response, out buffer, out length);

				context.Response.Status = HttpStatusCode.OK;
			}
		}

		private static class RequestUtility
		{
			public static void BeginGetResponse<T>(HttpWebRequest request, WebAPIRequestReceive<T> receive, T state)
			{
				if (request == null)
					return;

				try
				{
					var task = new TaskState<T>(request, receive, state);

					if (!Core.Closing)
					{
						Task.Factory.StartNew(task.GetResponse).ContinueWith(t => Timer.DelayCall(EndGetResponse, t.Result));
					}
					else
					{
						EndGetResponse(task.GetResponse());
					}
				}
				catch (Exception e)
				{
					CSOptions.ToConsole(e);
				}
			}

			public static void EndGetResponse<T>(TaskState<T> state)
			{
				if (state == null)
				{
					return;
				}

				try
				{
					if (state.Error != null)
					{
						CSOptions.ToConsole(state.Error);
						return;
					}

					try
					{
						state.Receive?.Invoke(state.Request, state.State, state.Response);
					}
					catch (Exception e)
					{
						CSOptions.ToConsole(e);
					}

					try
					{
						RequestReceive?.Invoke(state.Request, state.State, state.Response);
					}
					catch (Exception e)
					{
						CSOptions.ToConsole(e);
					}
				}
				catch (Exception e)
				{
					CSOptions.ToConsole(e);
				}
				finally
				{
					state.Dispose();
				}
			}

			public sealed class TaskState<T> : IDisposable
			{
				private bool _Handled, _Disposed;

				public HttpWebRequest Request { get; private set; }
				public HttpWebResponse Response { get; private set; }

				public WebAPIRequestReceive<T> Receive { get; private set; }

				public Exception Error { get; private set; }

				public T State { get; private set; }

				public TaskState(HttpWebRequest req, WebAPIRequestReceive<T> receive, T state)
				{
					Request = req;
					Receive = receive;
					State = state;
				}

				public TaskState<T> GetResponse()
				{
					if (_Handled)
					{
						return this;
					}

					_Handled = true;

					try
					{
						Response = (HttpWebResponse)Request.GetResponse();
					}
					catch (Exception e)
					{
						Error = e;
					}

					return this;
				}

				public void Dispose()
				{
					if (_Disposed)
					{
						return;
					}

					_Disposed = true;

					Request = null;
					Receive = null;
					State = default(T);

					if (Response != null)
					{
						Response.Close();
						Response = null;
					}
				}
			}
		}
	}
}
