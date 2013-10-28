using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Server.Commands.Generic;
using Server.Engines.BulkOrders;
using Server.Items;

namespace Server.Commands
{
    public class Docs
    {
        public static void Initialize()
        {
            CommandSystem.Register("DocGen", AccessLevel.Administrator, new CommandEventHandler(DocGen_OnCommand));
        }

        [Usage("DocGen")]
        [Description("Generates RunUO documentation.")]
        private static void DocGen_OnCommand(CommandEventArgs e)
        {
            World.Broadcast(0x35, true, "Documentation is being generated, please wait.");
            Console.WriteLine("Documentation is being generated, please wait.");

            Network.NetState.FlushAll();
            Network.NetState.Pause();

            DateTime startTime = DateTime.UtcNow;

            bool generated = Document();

            DateTime endTime = DateTime.UtcNow;

            Network.NetState.Resume();

            if (generated)
            {
                World.Broadcast(0x35, true, "Documentation has been completed. The entire process took {0:F1} seconds.", (endTime - startTime).TotalSeconds);
                Console.WriteLine("Documentation complete.");
            }
            else
            {
                World.Broadcast(0x35, true, "Docmentation failed: Documentation directories are locked and in use. Please close all open files and directories and try again.");
                Console.WriteLine("Documentation failed.");
            }
        }

        private class MemberComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == y)
                    return 0;

                ConstructorInfo aCtor = x as ConstructorInfo;
                ConstructorInfo bCtor = y as ConstructorInfo;

                PropertyInfo aProp = x as PropertyInfo;
                PropertyInfo bProp = y as PropertyInfo;

                MethodInfo aMethod = x as MethodInfo;
                MethodInfo bMethod = y as MethodInfo;

                bool aStatic = this.GetStaticFor(aCtor, aProp, aMethod);
                bool bStatic = this.GetStaticFor(bCtor, bProp, bMethod);

                if (aStatic && !bStatic)
                    return -1;
                else if (!aStatic && bStatic)
                    return 1;

                int v = 0;

                if (aCtor != null)
                {
                    if (bCtor == null)
                        v = -1;
                }
                else if (bCtor != null)
                {
                    if (aCtor == null)
                        v = 1;
                }
                else if (aProp != null)
                {
                    if (bProp == null)
                        v = -1;
                }
                else if (bProp != null)
                {
                    if (aProp == null)
                        v = 1;
                }

                if (v == 0)
                {
                    v = this.GetNameFrom(aCtor, aProp, aMethod).CompareTo(this.GetNameFrom(bCtor, bProp, bMethod));
                }

                if (v == 0 && aCtor != null && bCtor != null)
                {
                    v = aCtor.GetParameters().Length.CompareTo(bCtor.GetParameters().Length);
                }
                else if (v == 0 && aMethod != null && bMethod != null)
                {
                    v = aMethod.GetParameters().Length.CompareTo(bMethod.GetParameters().Length);
                }

                return v;
            }

            private bool GetStaticFor(ConstructorInfo ctor, PropertyInfo prop, MethodInfo method)
            {
                if (ctor != null)
                    return ctor.IsStatic;
                else if (method != null)
                    return method.IsStatic;

                if (prop != null)
                {
                    MethodInfo getMethod = prop.GetGetMethod();
                    MethodInfo setMethod = prop.GetGetMethod();

                    return (getMethod != null && getMethod.IsStatic) || (setMethod != null && setMethod.IsStatic);
                }

                return false;
            }

            private string GetNameFrom(ConstructorInfo ctor, PropertyInfo prop, MethodInfo method)
            {
                if (ctor != null)
                    return ctor.DeclaringType.Name;
                else if (prop != null)
                    return prop.Name;
                else if (method != null)
                    return method.Name;
                else
                    return "";
            }
        }

        private class TypeComparer : IComparer<TypeInfo>
        {
            public int Compare(TypeInfo x, TypeInfo y)
            {
                if (x == null && y == null)
                    return 0;
                else if (x == null)
                    return -1;
                else if (y == null)
                    return 1;

                return x.TypeName.CompareTo(y.TypeName);
            }
        }

        private class TypeInfo
        {
            public readonly Type m_Type;

            public readonly Type m_BaseType;

            public readonly Type m_Declaring;

            public List<TypeInfo> m_Derived, m_Nested;
            public readonly Type[] m_Interfaces;
            private readonly string m_FileName;

            private readonly string m_TypeName;

            private readonly string m_LinkName;

            public TypeInfo(Type type)
            {
                this.m_Type = type;

                this.m_BaseType = type.BaseType;
                this.m_Declaring = type.DeclaringType;
                this.m_Interfaces = type.GetInterfaces();

                FormatGeneric(this.m_Type, ref this.m_TypeName, ref this.m_FileName, ref this.m_LinkName);
                //				Console.WriteLine( ">> inline typeinfo: "+m_TypeName );
                //				m_TypeName = GetGenericTypeName( m_Type );
                //				m_FileName = Docs.GetFileName( "docs/types/", GetGenericTypeName( m_Type, "-", "-" ), ".html" );
                //				m_Writer = Docs.GetWriter( "docs/types/", m_FileName );
            }

            public string FileName
            {
                get
                {
                    return this.m_FileName;
                }
            }
            public string TypeName
            {
                get
                {
                    return this.m_TypeName;
                }
            }

            public string LinkName(string dirRoot)
            {
                return this.m_LinkName.Replace("@directory@", dirRoot);
            }
        }

        #region FileSystem
        private static readonly char[] ReplaceChars = "<>".ToCharArray();

        public static string GetFileName(string root, string name, string ext)
        {
            if (name.IndexOfAny(ReplaceChars) >= 0)
            {
                StringBuilder sb = new StringBuilder(name);

                for (int i = 0; i < ReplaceChars.Length; ++i)
                {
                    sb.Replace(ReplaceChars[i], '-');
                }

                name = sb.ToString();
            }

            int index = 0;
            string file = String.Concat(name, ext);

            while (File.Exists(Path.Combine(root, file)))
            {
                file = String.Concat(name, ++index, ext);
            }

            return file;
        }

        private static readonly string m_RootDirectory = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

        private static void EnsureDirectory(string path)
        {
            path = Path.Combine(m_RootDirectory, path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private static void DeleteDirectory(string path)
        {
            path = Path.Combine(m_RootDirectory, path);

            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }

        private static StreamWriter GetWriter(string root, string name)
        {
            return new StreamWriter(Path.Combine(Path.Combine(m_RootDirectory, root), name));
        }

        private static StreamWriter GetWriter(string path)
        {
            return new StreamWriter(Path.Combine(m_RootDirectory, path));
        }

        #endregion

        #region GetPair

        private static readonly string[,] m_Aliases = new string[,]
        {
            { "System.Object", "<font color=\"blue\">object</font>" },
            { "System.String", "<font color=\"blue\">string</font>" },
            { "System.Boolean", "<font color=\"blue\">bool</font>" },
            { "System.Byte", "<font color=\"blue\">byte</font>" },
            { "System.SByte", "<font color=\"blue\">sbyte</font>" },
            { "System.Int16", "<font color=\"blue\">short</font>" },
            { "System.UInt16", "<font color=\"blue\">ushort</font>" },
            { "System.Int32", "<font color=\"blue\">int</font>" },
            { "System.UInt32", "<font color=\"blue\">uint</font>" },
            { "System.Int64", "<font color=\"blue\">long</font>" },
            { "System.UInt64", "<font color=\"blue\">ulong</font>" },
            { "System.Single", "<font color=\"blue\">float</font>" },
            { "System.Double", "<font color=\"blue\">double</font>" },
            { "System.Decimal", "<font color=\"blue\">decimal</font>" },
            { "System.Char", "<font color=\"blue\">char</font>" },
            { "System.Void", "<font color=\"blue\">void</font>" },
        };

        private static readonly int m_AliasLength = m_Aliases.GetLength(0);

        public static string GetPair(Type varType, string name, bool ignoreRef)
        {
            string prepend = "";
            StringBuilder append = new StringBuilder();

            Type realType = varType;

            if (varType.IsByRef)
            {
                if (!ignoreRef)
                    prepend = RefString;

                realType = varType.GetElementType();
            }

            if (realType.IsPointer)
            {
                if (realType.IsArray)
                {
                    append.Append('*');

                    do
                    {
                        append.Append('[');

                        for (int i = 1; i < realType.GetArrayRank(); ++i)
                            append.Append(',');

                        append.Append(']');

                        realType = realType.GetElementType();
                    }
                    while (realType.IsArray);

                    append.Append(' ');
                }
                else
                {
                    realType = realType.GetElementType();
                    append.Append(" *");
                }
            }
            else if (realType.IsArray)
            {
                do
                {
                    append.Append('[');

                    for (int i = 1; i < realType.GetArrayRank(); ++i)
                        append.Append(',');

                    append.Append(']');

                    realType = realType.GetElementType();
                }
                while (realType.IsArray);

                append.Append(' ');
            }
            else
            {
                append.Append(' ');
            }

            string fullName = realType.FullName;
            string aliased = null;// = realType.Name;

            TypeInfo info = null;
            m_Types.TryGetValue(realType, out info);

            if (info != null)
            {
                aliased = "<!-- DBG-0 -->" + info.LinkName(null);
                //aliased = String.Format( "<a href=\"{0}\">{1}</a>", info.m_FileName, info.m_TypeName );
            }
            else
            {
                //FormatGeneric( );
                if (realType.IsGenericType)
                {
                    string typeName = "";
                    string fileName = "";
                    string linkName = "";

                    FormatGeneric(realType, ref typeName, ref fileName, ref linkName);
                    linkName = linkName.Replace("@directory@", null);
                    aliased = linkName;
                }
                else
                {
                    for (int i = 0; i < m_AliasLength; ++i)
                    {
                        if (m_Aliases[i, 0] == fullName)
                        {
                            aliased = m_Aliases[i, 1];
                            break;
                        }
                    }
                }

                if (aliased == null)
                    aliased = realType.Name;
            }

            string retval = String.Concat(prepend, aliased, append, name);
            //Console.WriteLine(">> getpair: "+retval);
            return retval;
        }

        #endregion

        private static Dictionary<Type, TypeInfo> m_Types;
        private static Dictionary<string, List<TypeInfo>> m_Namespaces;

        #region Root documentation

        private static bool Document()
        {
            try
            {
                DeleteDirectory("docs/");
            }
            catch
            {
                return false;
            }

            EnsureDirectory("docs/");
            EnsureDirectory("docs/namespaces/");
            EnsureDirectory("docs/types/");
            EnsureDirectory("docs/bods/");

            GenerateStyles();
            GenerateIndex();

            DocumentCommands();
            DocumentKeywords();
            DocumentBodies();

            DocumentBulkOrders();

            m_Types = new Dictionary<Type, TypeInfo>();
            m_Namespaces = new Dictionary<string, List<TypeInfo>>();

            List<Assembly> assemblies = new List<Assembly>();

            assemblies.Add(Core.Assembly);

            foreach (Assembly asm in ScriptCompiler.Assemblies)
                assemblies.Add(asm);

            Assembly[] asms = assemblies.ToArray();

            for (int i = 0; i < asms.Length; ++i)
                LoadTypes(asms[i], asms);

            DocumentLoadedTypes();
            DocumentConstructableObjects();

            return true;
        }

        private static void AddIndexLink(StreamWriter html, string filePath, string label, string desc)
        {
            html.WriteLine("      <h2><a href=\"{0}\" title=\"{1}\">{2}</a></h2>", filePath, desc, label);
        }

        private static void GenerateStyles()
        {
            using (StreamWriter css = GetWriter("docs/", "styles.css"))
            {
                css.WriteLine("body { background-color: #FFFFFF; font-family: verdana, arial; font-size: 11px; }");
                css.WriteLine("a { color: #28435E; }");
                css.WriteLine("a:hover { color: #4878A9; }");
                css.WriteLine("td.header { background-color: #9696AA; font-weight: bold; font-size: 12px; }");
                css.WriteLine("td.lentry { background-color: #D7D7EB; width: 10%; }");
                css.WriteLine("td.rentry { background-color: #FFFFFF; width: 90%; }");
                css.WriteLine("td.entry { background-color: #FFFFFF; }");
                css.WriteLine("td { font-size: 11px; }");
                css.WriteLine(".tbl-border { background-color: #46465A; }");

                css.WriteLine("td.ir {{ background-color: #{0:X6}; }}", Iron);
                css.WriteLine("td.du {{ background-color: #{0:X6}; }}", DullCopper);
                css.WriteLine("td.sh {{ background-color: #{0:X6}; }}", ShadowIron);
                css.WriteLine("td.co {{ background-color: #{0:X6}; }}", Copper);
                css.WriteLine("td.br {{ background-color: #{0:X6}; }}", Bronze);
                css.WriteLine("td.go {{ background-color: #{0:X6}; }}", Gold);
                css.WriteLine("td.ag {{ background-color: #{0:X6}; }}", Agapite);
                css.WriteLine("td.ve {{ background-color: #{0:X6}; }}", Verite);
                css.WriteLine("td.va {{ background-color: #{0:X6}; }}", Valorite);

                css.WriteLine("td.cl {{ background-color: #{0:X6}; }}", Cloth);
                css.WriteLine("td.pl {{ background-color: #{0:X6};  }}", Plain);
                css.WriteLine("td.sp {{ background-color: #{0:X6}; }}", Core.AOS ? SpinedAOS : SpinedLBR);
                css.WriteLine("td.ho {{ background-color: #{0:X6}; }}", Core.AOS ? HornedAOS : HornedLBR);
                css.WriteLine("td.ba {{ background-color: #{0:X6}; }}", Core.AOS ? BarbedAOS : BarbedLBR);
            }
        }

        private static void GenerateIndex()
        {
            using (StreamWriter html = GetWriter("docs/", "index.html"))
            {
                html.WriteLine("<html>");
                html.WriteLine("   <head>");
                html.WriteLine("      <title>RunUO Documentation - Index</title>");
                html.WriteLine("      <link rel=\"stylesheet\" type=\"text/css\" href=\"styles.css\" />");
                html.WriteLine("   </head>");
                html.WriteLine("   <body>");

                AddIndexLink(html, "commands.html", "Commands", "Every available command. This contains command name, usage, aliases, and description.");
                AddIndexLink(html, "objects.html", "Constructable Objects", "Every constructable item or npc. This contains object name and usage. Hover mouse over parameters to see type description.");
                AddIndexLink(html, "keywords.html", "Speech Keywords", "Lists speech keyword numbers and associated match patterns. These are used in some scripts for multi-language matching of client speech.");
                AddIndexLink(html, "bodies.html", "Body List", "Every usable body number and name. Table is generated from a UO:3D client datafile. If you do not have UO:3D installed, this may be blank.");
                AddIndexLink(html, "overview.html", "Class Overview", "Scripting reference. Contains every class type and contained methods in the core and scripts.");
                AddIndexLink(html, "bods/bod_smith_rewards.html", "Bulk Order Rewards: Smithing", "Reference table for large and small smithing bulk order deed rewards.");
                AddIndexLink(html, "bods/bod_tailor_rewards.html", "Bulk Order Rewards: Tailoring", "Reference table for large and small tailoring bulk order deed rewards.");

                html.WriteLine("   </body>");
                html.WriteLine("</html>");
            }
        }

        #endregion

        #region BODs

        private const int Iron = 0xCCCCDD;
        private const int DullCopper = 0xAAAAAA;
        private const int ShadowIron = 0x777799;
        private const int Copper = 0xDDCC99;
        private const int Bronze = 0xAA8866;
        private const int Gold = 0xDDCC55;
        private const int Agapite = 0xDDAAAA;
        private const int Verite = 0x99CC77;
        private const int Valorite = 0x88AABB;

        private const int Cloth = 0xDDDDDD;
        private const int Plain = 0xCCAA88;
        private const int SpinedAOS = 0x99BBBB;
        private const int HornedAOS = 0xCC8888;
        private const int BarbedAOS = 0xAABBAA;
        private const int SpinedLBR = 0xAA8833;
        private const int HornedLBR = 0xBBBBAA;
        private const int BarbedLBR = 0xCCAA88;

        private static void DocumentBulkOrders()
        {
            using (StreamWriter html = GetWriter("docs/bods/", "bod_smith_rewards.html"))
            {
                html.WriteLine("<html>");
                html.WriteLine("   <head>");
                html.WriteLine("      <title>RunUO Documentation - Bulk Orders - Smith Rewards</title>");
                html.WriteLine("      <link rel=\"stylesheet\" type=\"text/css\" href=\"../styles.css\" />");
                html.WriteLine("   </head>");
                html.WriteLine("   <body>");

                SmallBOD sbod = new SmallSmithBOD();

                sbod.Type = typeof(Katana);
                sbod.Material = BulkMaterialType.None;
                sbod.AmountMax = 10;

                WriteSmithBODHeader(html, "(Small) Weapons");
                sbod.RequireExceptional = false;
                DocumentSmithBOD(html, sbod.ComputeRewards(true), "10, 15, 20: Normal", sbod.Material);
                sbod.RequireExceptional = true;
                DocumentSmithBOD(html, sbod.ComputeRewards(true), "10, 15, 20: Exceptional", sbod.Material);
                WriteSmithBODFooter(html);

                html.WriteLine("      <br><br>");
                html.WriteLine("      <br><br>");

                sbod.Type = typeof(PlateArms);

                WriteSmithBODHeader(html, "(Small) Armor: Normal");

                sbod.RequireExceptional = false;
                for (BulkMaterialType mat = BulkMaterialType.None; mat <= BulkMaterialType.Valorite; ++mat)
                {
                    sbod.Material = mat;
                    sbod.AmountMax = 10;
                    DocumentSmithBOD(html, sbod.ComputeRewards(true), "10, 15, 20", sbod.Material);
                }

                WriteSmithBODFooter(html);

                html.WriteLine("      <br><br>");

                WriteSmithBODHeader(html, "(Small) Armor: Exceptional");

                sbod.RequireExceptional = true;
                for (BulkMaterialType mat = BulkMaterialType.None; mat <= BulkMaterialType.Valorite; ++mat)
                {
                    sbod.Material = mat;

                    for (int amt = 15; amt <= 20; amt += 5)
                    {
                        sbod.AmountMax = amt;
                        DocumentSmithBOD(html, sbod.ComputeRewards(true), amt == 20 ? "20" : "10, 15", sbod.Material);
                    }
                }

                WriteSmithBODFooter(html);

                html.WriteLine("      <br><br>");
                html.WriteLine("      <br><br>");

                sbod.Delete();

                WriteSmithLBOD(html, "Ringmail", LargeBulkEntry.LargeRing);
                WriteSmithLBOD(html, "Chainmail", LargeBulkEntry.LargeChain);
                WriteSmithLBOD(html, "Platemail", LargeBulkEntry.LargePlate);

                html.WriteLine("   </body>");
                html.WriteLine("</html>");
            }

            using (StreamWriter html = GetWriter("docs/bods/", "bod_tailor_rewards.html"))
            {
                html.WriteLine("<html>");
                html.WriteLine("   <head>");
                html.WriteLine("      <title>RunUO Documentation - Bulk Orders - Tailor Rewards</title>");
                html.WriteLine("      <link rel=\"stylesheet\" type=\"text/css\" href=\"../styles.css\" />");
                html.WriteLine("   </head>");
                html.WriteLine("   <body>");

                SmallBOD sbod = new SmallTailorBOD();

                WriteTailorBODHeader(html, "Small Bulk Order");

                html.WriteLine("         <tr>");
                html.WriteLine("            <td width=\"850\" colspan=\"21\" class=\"entry\"><b>Regular: 10, 15</b></td>");
                html.WriteLine("         </tr>");

                sbod.AmountMax = 10;
                sbod.RequireExceptional = false;

                sbod.Type = typeof(SkullCap);
                sbod.Material = BulkMaterialType.None;
                DocumentTailorBOD(html, sbod.ComputeRewards(true), "10, 15", sbod.Material, sbod.Type);

                sbod.Type = typeof(LeatherCap);
                for (BulkMaterialType mat = BulkMaterialType.None; mat <= BulkMaterialType.Barbed; ++mat)
                {
                    if (mat >= BulkMaterialType.DullCopper && mat <= BulkMaterialType.Valorite)
                        continue;

                    sbod.Material = mat;
                    DocumentTailorBOD(html, sbod.ComputeRewards(true), "10, 15", sbod.Material, sbod.Type);
                }

                html.WriteLine("         <tr>");
                html.WriteLine("            <td width=\"850\" colspan=\"21\" class=\"entry\"><b>Regular: 20</b></td>");
                html.WriteLine("         </tr>");

                sbod.AmountMax = 20;
                sbod.RequireExceptional = false;

                sbod.Type = typeof(SkullCap);
                sbod.Material = BulkMaterialType.None;
                DocumentTailorBOD(html, sbod.ComputeRewards(true), "20", sbod.Material, sbod.Type);

                sbod.Type = typeof(LeatherCap);
                for (BulkMaterialType mat = BulkMaterialType.None; mat <= BulkMaterialType.Barbed; ++mat)
                {
                    if (mat >= BulkMaterialType.DullCopper && mat <= BulkMaterialType.Valorite)
                        continue;

                    sbod.Material = mat;
                    DocumentTailorBOD(html, sbod.ComputeRewards(true), "20", sbod.Material, sbod.Type);
                }

                html.WriteLine("         <tr>");
                html.WriteLine("            <td width=\"850\" colspan=\"21\" class=\"entry\"><b>Exceptional: 10, 15</b></td>");
                html.WriteLine("         </tr>");

                sbod.AmountMax = 10;
                sbod.RequireExceptional = true;

                sbod.Type = typeof(SkullCap);
                sbod.Material = BulkMaterialType.None;
                DocumentTailorBOD(html, sbod.ComputeRewards(true), "10, 15", sbod.Material, sbod.Type);

                sbod.Type = typeof(LeatherCap);
                for (BulkMaterialType mat = BulkMaterialType.None; mat <= BulkMaterialType.Barbed; ++mat)
                {
                    if (mat >= BulkMaterialType.DullCopper && mat <= BulkMaterialType.Valorite)
                        continue;

                    sbod.Material = mat;
                    DocumentTailorBOD(html, sbod.ComputeRewards(true), "10, 15", sbod.Material, sbod.Type);
                }

                html.WriteLine("         <tr>");
                html.WriteLine("            <td width=\"850\" colspan=\"21\" class=\"entry\"><b>Exceptional: 20</b></td>");
                html.WriteLine("         </tr>");

                sbod.AmountMax = 20;
                sbod.RequireExceptional = true;

                sbod.Type = typeof(SkullCap);
                sbod.Material = BulkMaterialType.None;
                DocumentTailorBOD(html, sbod.ComputeRewards(true), "20", sbod.Material, sbod.Type);

                sbod.Type = typeof(LeatherCap);
                for (BulkMaterialType mat = BulkMaterialType.None; mat <= BulkMaterialType.Barbed; ++mat)
                {
                    if (mat >= BulkMaterialType.DullCopper && mat <= BulkMaterialType.Valorite)
                        continue;

                    sbod.Material = mat;
                    DocumentTailorBOD(html, sbod.ComputeRewards(true), "20", sbod.Material, sbod.Type);
                }

                WriteTailorBODFooter(html);

                html.WriteLine("      <br><br>");
                html.WriteLine("      <br><br>");

                sbod.Delete();

                WriteTailorLBOD(html, "Large Bulk Order: 4-part", LargeBulkEntry.Gypsy, true, true);
                WriteTailorLBOD(html, "Large Bulk Order: 5-part", LargeBulkEntry.TownCrier, true, true);
                WriteTailorLBOD(html, "Large Bulk Order: 6-part", LargeBulkEntry.MaleLeatherSet, false, true);

                html.WriteLine("   </body>");
                html.WriteLine("</html>");
            }
        }

        #region Tailor Bods
        private static void WriteTailorLBOD(StreamWriter html, string name, SmallBulkEntry[] entries, bool expandCloth, bool expandPlain)
        {
            WriteTailorBODHeader(html, name);

            LargeBOD lbod = new LargeTailorBOD();

            lbod.Entries = LargeBulkEntry.ConvertEntries(lbod, entries);

            Type type = entries[0].Type;

            bool showCloth = !(type.IsSubclassOf(typeof(BaseArmor)) || type.IsSubclassOf(typeof(BaseShoes)));

            html.WriteLine("         <tr>");
            html.WriteLine("            <td width=\"850\" colspan=\"21\" class=\"entry\"><b>Regular</b></td>");
            html.WriteLine("         </tr>");

            lbod.RequireExceptional = false;
            lbod.AmountMax = 10;

            if (showCloth)
            {
                lbod.Material = BulkMaterialType.None;

                if (expandCloth)
                {
                    lbod.AmountMax = 10;
                    DocumentTailorBOD(html, lbod.ComputeRewards(true), "10, 15", lbod.Material, type);
                    lbod.AmountMax = 20;
                    DocumentTailorBOD(html, lbod.ComputeRewards(true), "20", lbod.Material, type);
                }
                else
                {
                    lbod.AmountMax = 10;
                    DocumentTailorBOD(html, lbod.ComputeRewards(true), "10, 15, 20", lbod.Material, type);
                }
            }

            lbod.Material = BulkMaterialType.None;

            if (expandPlain)
            {
                lbod.AmountMax = 10;
                DocumentTailorBOD(html, lbod.ComputeRewards(true), "10, 15, 20", lbod.Material, typeof(LeatherCap));
                lbod.AmountMax = 20;
                DocumentTailorBOD(html, lbod.ComputeRewards(true), "20", lbod.Material, typeof(LeatherCap));
            }
            else
            {
                lbod.AmountMax = 10;
                DocumentTailorBOD(html, lbod.ComputeRewards(true), "10, 15, 20", lbod.Material, typeof(LeatherCap));
            }

            for (BulkMaterialType mat = BulkMaterialType.Spined; mat <= BulkMaterialType.Barbed; ++mat)
            {
                lbod.Material = mat;
                lbod.AmountMax = 10;
                DocumentTailorBOD(html, lbod.ComputeRewards(true), "10, 15", lbod.Material, type);
                lbod.AmountMax = 20;
                DocumentTailorBOD(html, lbod.ComputeRewards(true), "20", lbod.Material, type);
            }

            html.WriteLine("         <tr>");
            html.WriteLine("            <td width=\"850\" colspan=\"21\" class=\"entry\"><b>Exceptional</b></td>");
            html.WriteLine("         </tr>");

            lbod.RequireExceptional = true;
            lbod.AmountMax = 10;

            if (showCloth)
            {
                lbod.Material = BulkMaterialType.None;

                if (expandCloth)
                {
                    lbod.AmountMax = 10;
                    DocumentTailorBOD(html, lbod.ComputeRewards(true), "10, 15", lbod.Material, type);
                    lbod.AmountMax = 20;
                    DocumentTailorBOD(html, lbod.ComputeRewards(true), "20", lbod.Material, type);
                }
                else
                {
                    lbod.AmountMax = 10;
                    DocumentTailorBOD(html, lbod.ComputeRewards(true), "10, 15, 20", lbod.Material, type);
                }
            }

            lbod.Material = BulkMaterialType.None;

            if (expandPlain)
            {
                lbod.AmountMax = 10;
                DocumentTailorBOD(html, lbod.ComputeRewards(true), "10, 15, 20", lbod.Material, typeof(LeatherCap));
                lbod.AmountMax = 20;
                DocumentTailorBOD(html, lbod.ComputeRewards(true), "20", lbod.Material, typeof(LeatherCap));
            }
            else
            {
                lbod.AmountMax = 10;
                DocumentTailorBOD(html, lbod.ComputeRewards(true), "10, 15, 20", lbod.Material, typeof(LeatherCap));
            }

            for (BulkMaterialType mat = BulkMaterialType.Spined; mat <= BulkMaterialType.Barbed; ++mat)
            {
                lbod.Material = mat;
                lbod.AmountMax = 10;
                DocumentTailorBOD(html, lbod.ComputeRewards(true), "10, 15", lbod.Material, type);
                lbod.AmountMax = 20;
                DocumentTailorBOD(html, lbod.ComputeRewards(true), "20", lbod.Material, type);
            }

            WriteTailorBODFooter(html);

            html.WriteLine("      <br><br>");
            html.WriteLine("      <br><br>");
        }

        private static void WriteTailorBODHeader(StreamWriter html, string title)
        {
            html.WriteLine("      <table width=\"850\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
            html.WriteLine("      <tr><td class=\"tbl-border\">");
            html.WriteLine("      <table border=\"0\" width=\"850\" cellpadding=\"0\" cellspacing=\"1\">");
            html.WriteLine("         <tr>");
            html.WriteLine("            <td width=\"250\" rowspan=\"2\" class=\"entry\"><center>{0}</center></td>", title);
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><a href=\"http://www.runuo.com/images/bodreward_cloth_full.jpg\"><img src=\"http://www.runuo.com/images/bodreward_cloth_1.jpg\" alt=\"Colored Cloth (Level 1)\" border=\"0\"></a></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><a href=\"http://www.runuo.com/images/bodreward_cloth_full.jpg\"><img src=\"http://www.runuo.com/images/bodreward_cloth_2.jpg\" alt=\"Colored Cloth (Level 2)\" border=\"0\"></a></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><a href=\"http://www.runuo.com/images/bodreward_cloth_full.jpg\"><img src=\"http://www.runuo.com/images/bodreward_cloth_3.jpg\" alt=\"Colored Cloth (Level 3)\" border=\"0\"></a></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><a href=\"http://www.runuo.com/images/bodreward_cloth_full.jpg\"><img src=\"http://www.runuo.com/images/bodreward_cloth_4.jpg\" alt=\"Colored Cloth (Level 4)\" border=\"0\"></a></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><a href=\"http://www.runuo.com/images/bodreward_cloth_full.jpg\"><img src=\"http://www.runuo.com/images/bodreward_cloth_5.jpg\" alt=\"Colored Cloth (Level 5)\" border=\"0\"></a></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><a href=\"http://www.runuo.com/images/bodreward_sandals_full.jpg\"><img src=\"http://www.runuo.com/images/bodreward_sandals.jpg\" alt=\"Colored Sandals\" border=\"0\"></a></center></td>");
            html.WriteLine("            <td width=\"100\" colspan=\"4\" class=\"entry\"><center>Power Scrolls</center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_smallhides.jpg\" alt=\"Small Stretched Hide\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_mediumhides.jpg\" alt=\"Medium Stretched Hide\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_lighttapestry.jpg\" alt=\"Light Flower Tapestry\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_darktapestry.jpg\" alt=\"Dark Flower Tapestry\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_brownbearrug.jpg\" alt=\"Brown Bear Rug\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_polarbearrug.jpg\" alt=\"Polar Bear Rug\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_clothingbless.jpg\" alt=\"Clothing Bless Deed\"></center></td>");
            html.WriteLine("            <td width=\"75\" colspan=\"3\" class=\"entry\"><center>Runic Kits</center></td>");
            html.WriteLine("         </tr>");
            html.WriteLine("         <tr>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+5</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+10</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+15</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+20</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_runic_spined.jpg\" alt=\"Runic Sewing Kit: Spined\"></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_runic_horned.jpg\" alt=\"Runic Sewing Kit: Horned\"></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_runic_barbed.jpg\" alt=\"Runic Sewing Kit: Barbed\"></center></td>");
            html.WriteLine("         </tr>");
        }

        private static void WriteTailorBODFooter(StreamWriter html)
        {
            html.WriteLine("         <tr>");
            html.WriteLine("            <td width=\"250\" rowspan=\"2\" class=\"entry\">&nbsp;</td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><a href=\"http://www.runuo.com/images/bodreward_cloth_full.jpg\"><img src=\"http://www.runuo.com/images/bodreward_cloth_1.jpg\" alt=\"Colored Cloth (Level 1)\" border=\"0\"></a></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><a href=\"http://www.runuo.com/images/bodreward_cloth_full.jpg\"><img src=\"http://www.runuo.com/images/bodreward_cloth_2.jpg\" alt=\"Colored Cloth (Level 2)\" border=\"0\"></a></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><a href=\"http://www.runuo.com/images/bodreward_cloth_full.jpg\"><img src=\"http://www.runuo.com/images/bodreward_cloth_3.jpg\" alt=\"Colored Cloth (Level 3)\" border=\"0\"></a></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><a href=\"http://www.runuo.com/images/bodreward_cloth_full.jpg\"><img src=\"http://www.runuo.com/images/bodreward_cloth_4.jpg\" alt=\"Colored Cloth (Level 4)\" border=\"0\"></a></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><a href=\"http://www.runuo.com/images/bodreward_cloth_full.jpg\"><img src=\"http://www.runuo.com/images/bodreward_cloth_5.jpg\" alt=\"Colored Cloth (Level 5)\" border=\"0\"></a></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><a href=\"http://www.runuo.com/images/bodreward_sandals_full.jpg\"><img src=\"http://www.runuo.com/images/bodreward_sandals.jpg\" alt=\"Colored Sandals\" border=\"0\"></a></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+5</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+10</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+15</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+20</small></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_smallhides.jpg\" alt=\"Small Stretched Hide\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_mediumhides.jpg\" alt=\"Medium Stretched Hide\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_lighttapestry.jpg\" alt=\"Light Flower Tapestry\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_darktapestry.jpg\" alt=\"Dark Flower Tapestry\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_brownbearrug.jpg\" alt=\"Brown Bear Rug\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_polarbearrug.jpg\" alt=\"Polar Bear Rug\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_clothingbless.jpg\" alt=\"Clothing Bless Deed\"></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_runic_spined.jpg\" alt=\"Runic Sewing Kit: Spined\"></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_runic_horned.jpg\" alt=\"Runic Sewing Kit: Horned\"></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_runic_barbed.jpg\" alt=\"Runic Sewing Kit: Barbed\"></center></td>");
            html.WriteLine("         </tr>");
            html.WriteLine("         <tr>");
            html.WriteLine("            <td width=\"100\" colspan=\"4\" class=\"entry\"><center>Power Scrolls</center></td>");
            html.WriteLine("            <td width=\"75\" colspan=\"3\" class=\"entry\"><center>Runic Kits</center></td>");
            html.WriteLine("         </tr>");
            html.WriteLine("      </table></td></tr></table>");
        }

        private static void DocumentTailorBOD(StreamWriter html, List<Item> items, string amt, BulkMaterialType material, Type type)
        {
            bool[] rewards = new bool[20];

            for (int i = 0; i < items.Count; ++i)
            {
                Item item = (Item)items[i];

                if (item is Sandals)
                    rewards[5] = true;
                else if (item is SmallStretchedHideEastDeed || item is SmallStretchedHideSouthDeed)
                    rewards[10] = rewards[11] = true;
                else if (item is MediumStretchedHideEastDeed || item is MediumStretchedHideSouthDeed)
                    rewards[10] = rewards[11] = true;
                else if (item is LightFlowerTapestryEastDeed || item is LightFlowerTapestrySouthDeed)
                    rewards[12] = rewards[13] = true;
                else if (item is DarkFlowerTapestryEastDeed || item is DarkFlowerTapestrySouthDeed)
                    rewards[12] = rewards[13] = true;
                else if (item is BrownBearRugEastDeed || item is BrownBearRugSouthDeed)
                    rewards[14] = rewards[15] = true;
                else if (item is PolarBearRugEastDeed || item is PolarBearRugSouthDeed)
                    rewards[14] = rewards[15] = true;
                else if (item is ClothingBlessDeed)
                    rewards[16] = true;
                else if (item is PowerScroll)
                {
                    PowerScroll ps = (PowerScroll)item;

                    if (ps.Value == 105.0)
                        rewards[6] = true;
                    else if (ps.Value == 110.0)
                        rewards[7] = true;
                    else if (ps.Value == 115.0)
                        rewards[8] = true;
                    else if (ps.Value == 120.0)
                        rewards[9] = true;
                }
                else if (item is UncutCloth)
                {
                    if (item.Hue == 0x483 || item.Hue == 0x48C || item.Hue == 0x488 || item.Hue == 0x48A)
                        rewards[0] = true;
                    else if (item.Hue == 0x495 || item.Hue == 0x48B || item.Hue == 0x486 || item.Hue == 0x485)
                        rewards[1] = true;
                    else if (item.Hue == 0x48D || item.Hue == 0x490 || item.Hue == 0x48E || item.Hue == 0x491)
                        rewards[2] = true;
                    else if (item.Hue == 0x48F || item.Hue == 0x494 || item.Hue == 0x484 || item.Hue == 0x497)
                        rewards[3] = true;
                    else
                        rewards[4] = true;
                }
                else if (item is RunicSewingKit)
                {
                    RunicSewingKit rkit = (RunicSewingKit)item;

                    rewards[16 + CraftResources.GetIndex(rkit.Resource)] = true;
                }

                item.Delete();
            }

            string style = null;
            string name = null;

            switch (material)
            {
                case BulkMaterialType.None:
                    {
                        if (type.IsSubclassOf(typeof(BaseArmor)) || type.IsSubclassOf(typeof(BaseShoes)))
                        {
                            style = "pl";
                            name = "Plain";
                        }
                        else
                        {
                            style = "cl";
                            name = "Cloth";
                        }

                        break;
                    }
                case BulkMaterialType.Spined:
                    style = "sp";
                    name = "Spined";
                    break;
                case BulkMaterialType.Horned:
                    style = "ho";
                    name = "Horned";
                    break;
                case BulkMaterialType.Barbed:
                    style = "ba";
                    name = "Barbed";
                    break;
            }

            html.WriteLine("         <tr>");
            html.WriteLine("            <td width=\"250\" class=\"entry\">&nbsp;- {0} <font size=\"1pt\">{1}</font></td>", name, amt);

            int index = 0;

            while (index < 20)
            {
                if (rewards[index])
                {
                    html.WriteLine("            <td width=\"25\" class=\"{0}\"><center><b>X</b></center></td>", style);
                    ++index;
                }
                else
                {
                    int count = 0;

                    while (index < 20 && !rewards[index])
                    {
                        ++count;
                        ++index;

                        if (index == 5 || index == 6 || index == 10 || index == 17)
                            break;
                    }

                    html.WriteLine("            <td width=\"{0}\"{1} class=\"entry\">&nbsp;</td>", count * 25, count == 1 ? "" : String.Format(" colspan=\"{0}\"", count));
                }
            }

            html.WriteLine("         </tr>");
        }

        #endregion

        #region Smith Bods
        private static void WriteSmithLBOD(StreamWriter html, string name, SmallBulkEntry[] entries)
        {
            LargeBOD lbod = new LargeSmithBOD();

            lbod.Entries = LargeBulkEntry.ConvertEntries(lbod, entries);

            WriteSmithBODHeader(html, String.Format("(Large) {0}: Normal", name));

            lbod.RequireExceptional = false;
            for (BulkMaterialType mat = BulkMaterialType.None; mat <= BulkMaterialType.Valorite; ++mat)
            {
                lbod.Material = mat;
                lbod.AmountMax = 10;
                DocumentSmithBOD(html, lbod.ComputeRewards(true), "10, 15, 20", lbod.Material);
            }

            WriteSmithBODFooter(html);

            html.WriteLine("      <br><br>");

            WriteSmithBODHeader(html, String.Format("(Large) {0}: Exceptional", name));

            lbod.RequireExceptional = true;
            for (BulkMaterialType mat = BulkMaterialType.None; mat <= BulkMaterialType.Valorite; ++mat)
            {
                lbod.Material = mat;

                for (int amt = 15; amt <= 20; amt += 5)
                {
                    lbod.AmountMax = amt;
                    DocumentSmithBOD(html, lbod.ComputeRewards(true), amt == 20 ? "20" : "10, 15", lbod.Material);
                }
            }

            WriteSmithBODFooter(html);

            html.WriteLine("      <br><br>");
            html.WriteLine("      <br><br>");
        }

        private static void WriteSmithBODHeader(StreamWriter html, string title)
        {
            html.WriteLine("      <table width=\"850\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
            html.WriteLine("      <tr><td class=\"tbl-border\">");
            html.WriteLine("      <table border=\"0\" width=\"850\" cellpadding=\"0\" cellspacing=\"1\">");
            html.WriteLine("         <tr>");
            html.WriteLine("            <td width=\"250\" rowspan=\"2\" class=\"entry\"><center>{0}</center></td>", title);
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_sturdytool.jpg\" alt=\"Sturdy Pickaxe/Shovel (150 uses)\"></center></td>");
            html.WriteLine("            <td width=\"75\" colspan=\"3\" class=\"entry\"><center>Gloves</center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_gargaxe.jpg\" alt=\"Gargoyles Pickaxe (100 uses)\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_prospectortool.jpg\" alt=\"Prospectors Tool (50 uses)\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_powder.jpg\" alt=\"Powder of Temperament (10 uses)\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_anvil.jpg\" alt=\"Colored Anvil\"></center></td>");
            html.WriteLine("            <td width=\"100\" colspan=\"4\" class=\"entry\"><center>Power Scrolls</center></td>");
            html.WriteLine("            <td width=\"200\" colspan=\"8\" class=\"entry\"><center>Runic Hammers</center></td>");
            html.WriteLine("            <td width=\"100\" colspan=\"4\" class=\"entry\"><center>Ancient Hammers</center></td>");
            html.WriteLine("         </tr>");
            html.WriteLine("         <tr>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+1</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+3</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+5</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+5</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+10</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+15</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+20</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"du\"><center><small>Du</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"sh\"><center><small>Sh</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"co\"><center><small>Co</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"br\"><center><small>Br</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"go\"><center><small>Go</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"ag\"><center><small>Ag</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"ve\"><center><small>Ve</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"va\"><center><small>Va</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+10</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+15</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+30</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+60</small></center></td>");
            html.WriteLine("         </tr>");
        }

        private static void WriteSmithBODFooter(StreamWriter html)
        {
            html.WriteLine("         <tr>");
            html.WriteLine("            <td width=\"250\" rowspan=\"2\" class=\"entry\">&nbsp;</td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_sturdytool.jpg\" alt=\"Sturdy Pickaxe/Shovel (150 uses)\"></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+1</small></center>&nbsp;</td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+3</small></center>&nbsp;</td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+5</small></center>&nbsp;</td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_gargaxe.jpg\" alt=\"Gargoyles Pickaxe (100 uses)\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_prospectortool.jpg\" alt=\"Prospectors Tool (50 uses)\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_powder.jpg\" alt=\"Powder of Temperament (10 uses)\"></center></td>");
            html.WriteLine("            <td width=\"25\" rowspan=\"2\" class=\"entry\"><center><img src=\"http://www.runuo.com/images/bodreward_anvil.jpg\" alt=\"Colored Anvil\"></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+5</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+10</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+15</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+20</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"du\"><center><small>Du</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"sh\"><center><small>Sh</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"co\"><center><small>Co</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"br\"><center><small>Br</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"go\"><center><small>Go</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"ag\"><center><small>Ag</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"ve\"><center><small>Ve</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"va\"><center><small>Va</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+10</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+15</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+30</small></center></td>");
            html.WriteLine("            <td width=\"25\" class=\"entry\"><center><small>+60</small></center></td>");
            html.WriteLine("         </tr>");
            html.WriteLine("         <tr>");
            html.WriteLine("            <td width=\"75\" colspan=\"3\" class=\"entry\"><center>Gloves</center></td>");
            html.WriteLine("            <td width=\"100\" colspan=\"4\" class=\"entry\"><center>Power Scrolls</center></td>");
            html.WriteLine("            <td width=\"200\" colspan=\"8\" class=\"entry\"><center>Runic Hammers</center></td>");
            html.WriteLine("            <td width=\"100\" colspan=\"4\" class=\"entry\"><center>Ancient Hammers</center></td>");
            html.WriteLine("         </tr>");
            html.WriteLine("      </table></td></tr></table>");
        }

        private static void DocumentSmithBOD(StreamWriter html, List<Item> items, string amt, BulkMaterialType material)
        {
            bool[] rewards = new bool[24];

            for (int i = 0; i < items.Count; ++i)
            {
                Item item = (Item)items[i];

                if (item is SturdyPickaxe || item is SturdyShovel)
                    rewards[0] = true;
                else if (item is LeatherGlovesOfMining)
                    rewards[1] = true;
                else if (item is StuddedGlovesOfMining)
                    rewards[2] = true;
                else if (item is RingmailGlovesOfMining)
                    rewards[3] = true;
                else if (item is GargoylesPickaxe)
                    rewards[4] = true;
                else if (item is ProspectorsTool)
                    rewards[5] = true;
                else if (item is PowderOfTemperament)
                    rewards[6] = true;
                else if (item is ColoredAnvil)
                    rewards[7] = true;
                else if (item is PowerScroll)
                {
                    PowerScroll ps = (PowerScroll)item;

                    if (ps.Value == 105.0)
                        rewards[8] = true;
                    else if (ps.Value == 110.0)
                        rewards[9] = true;
                    else if (ps.Value == 115.0)
                        rewards[10] = true;
                    else if (ps.Value == 120.0)
                        rewards[11] = true;
                }
                else if (item is RunicHammer)
                {
                    RunicHammer rh = (RunicHammer)item;

                    rewards[11 + CraftResources.GetIndex(rh.Resource)] = true;
                }
                else if (item is AncientSmithyHammer)
                {
                    AncientSmithyHammer ash = (AncientSmithyHammer)item;

                    if (ash.Bonus == 10)
                        rewards[20] = true;
                    else if (ash.Bonus == 15)
                        rewards[21] = true;
                    else if (ash.Bonus == 30)
                        rewards[22] = true;
                    else if (ash.Bonus == 60)
                        rewards[23] = true;
                }

                item.Delete();
            }

            string style = null;
            string name = null;

            switch (material)
            {
                case BulkMaterialType.None:
                    style = "ir";
                    name = "Iron";
                    break;
                case BulkMaterialType.DullCopper:
                    style = "du";
                    name = "Dull Copper";
                    break;
                case BulkMaterialType.ShadowIron:
                    style = "sh";
                    name = "Shadow Iron";
                    break;
                case BulkMaterialType.Copper:
                    style = "co";
                    name = "Copper";
                    break;
                case BulkMaterialType.Bronze:
                    style = "br";
                    name = "Bronze";
                    break;
                case BulkMaterialType.Gold:
                    style = "go";
                    name = "Gold";
                    break;
                case BulkMaterialType.Agapite:
                    style = "ag";
                    name = "Agapite";
                    break;
                case BulkMaterialType.Verite:
                    style = "ve";
                    name = "Verite";
                    break;
                case BulkMaterialType.Valorite:
                    style = "va";
                    name = "Valorite";
                    break;
            }

            html.WriteLine("         <tr>");
            html.WriteLine("            <td width=\"250\" class=\"entry\">{0} <font size=\"1pt\">{1}</font></td>", name, amt);

            int index = 0;

            while (index < 24)
            {
                if (rewards[index])
                {
                    html.WriteLine("            <td width=\"25\" class=\"{0}\"><center><b>X</b></center></td>", style);
                    ++index;
                }
                else
                {
                    int count = 0;

                    while (index < 24 && !rewards[index])
                    {
                        ++count;
                        ++index;

                        if (index == 4 || index == 8 || index == 12 || index == 20)
                            break;
                    }

                    html.WriteLine("            <td width=\"{0}\"{1} class=\"entry\">&nbsp;</td>", count * 25, count == 1 ? "" : String.Format(" colspan=\"{0}\"", count));
                }
            }

            html.WriteLine("         </tr>");
        }

        #endregion

        #endregion

        #region Bodies
        public static List<BodyEntry> LoadBodies()
        {
            List<BodyEntry> list = new List<BodyEntry>();

            string path = Core.FindDataFile("models/models.txt");

            if (File.Exists(path))
            {
                using (StreamReader ip = new StreamReader(path))
                {
                    string line;

                    while ((line = ip.ReadLine()) != null)
                    {
                        line = line.Trim();

                        if (line.Length == 0 || line.StartsWith("#"))
                            continue;

                        string[] split = line.Split('\t');

                        if (split.Length >= 9)
                        {
                            Body body = Utility.ToInt32(split[0]);
                            ModelBodyType type = (ModelBodyType)Utility.ToInt32(split[1]);
                            string name = split[8];

                            BodyEntry entry = new BodyEntry(body, type, name);

                            if (!list.Contains(entry))
                                list.Add(entry);
                        }
                    }
                }
            }

            return list;
        }

        private static void DocumentBodies()
        {
            List<BodyEntry> list = LoadBodies();

            using (StreamWriter html = GetWriter("docs/", "bodies.html"))
            {
                html.WriteLine("<html>");
                html.WriteLine("   <head>");
                html.WriteLine("      <title>RunUO Documentation - Body List</title>");
                html.WriteLine("      <link rel=\"stylesheet\" type=\"text/css\" href=\"styles.css\" />");
                html.WriteLine("   </head>");
                html.WriteLine("   <body>");
                html.WriteLine("      <a name=\"Top\" />");
                html.WriteLine("      <h4><a href=\"index.html\">Back to the index</a></h4>");

                if (list.Count > 0)
                {
                    html.WriteLine("      <h2>Body List</h2>");

                    list.Sort(new BodyEntrySorter());

                    ModelBodyType lastType = ModelBodyType.Invalid;

                    for (int i = 0; i < list.Count; ++i)
                    {
                        BodyEntry entry = list[i];
                        ModelBodyType type = entry.BodyType;

                        if (type != lastType)
                        {
                            if (lastType != ModelBodyType.Invalid)
                                html.WriteLine("      </table></td></tr></table><br>");

                            lastType = type;

                            html.WriteLine("      <a name=\"{0}\" />", type);

                            switch (type)
                            {
                                case ModelBodyType.Monsters:
                                    html.WriteLine("      <b>Monsters</b> | <a href=\"#Sea\">Sea</a> | <a href=\"#Animals\">Animals</a> | <a href=\"#Human\">Human</a> | <a href=\"#Equipment\">Equipment</a><br><br>");
                                    break;
                                case ModelBodyType.Sea:
                                    html.WriteLine("      <a href=\"#Top\">Monsters</a> | <b>Sea</b> | <a href=\"#Animals\">Animals</a> | <a href=\"#Human\">Human</a> | <a href=\"#Equipment\">Equipment</a><br><br>");
                                    break;
                                case ModelBodyType.Animals:
                                    html.WriteLine("      <a href=\"#Top\">Monsters</a> | <a href=\"#Sea\">Sea</a> | <b>Animals</b> | <a href=\"#Human\">Human</a> | <a href=\"#Equipment\">Equipment</a><br><br>");
                                    break;
                                case ModelBodyType.Human:
                                    html.WriteLine("      <a href=\"#Top\">Monsters</a> | <a href=\"#Sea\">Sea</a> | <a href=\"#Animals\">Animals</a> | <b>Human</b> | <a href=\"#Equipment\">Equipment</a><br><br>");
                                    break;
                                case ModelBodyType.Equipment:
                                    html.WriteLine("      <a href=\"#Top\">Monsters</a> | <a href=\"#Sea\">Sea</a> | <a href=\"#Animals\">Animals</a> | <a href=\"#Human\">Human</a> | <b>Equipment</b><br><br>");
                                    break;
                            }

                            html.WriteLine("      <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
                            html.WriteLine("      <tr><td class=\"tbl-border\">");
                            html.WriteLine("      <table width=\"100%\" cellpadding=\"4\" cellspacing=\"1\">");
                            html.WriteLine("         <tr><td width=\"100%\" colspan=\"2\" class=\"header\">{0}</td></tr>", type);
                        }

                        html.WriteLine("         <tr><td class=\"lentry\">{0}</td><td class=\"rentry\">{1}</td></tr>", entry.Body.BodyID, entry.Name);
                    }

                    html.WriteLine("      </table>");
                }
                else
                {
                    html.WriteLine("      This feature requires a UO:3D installation.");
                }

                html.WriteLine("   </body>");
                html.WriteLine("</html>");
            }
        }

        #endregion

        #region Speech
        private static void DocumentKeywords()
        {
            List<Dictionary<int, SpeechEntry>> tables = LoadSpeechFile();

            using (StreamWriter html = GetWriter("docs/", "keywords.html"))
            {
                html.WriteLine("<html>");
                html.WriteLine("   <head>");
                html.WriteLine("      <title>RunUO Documentation - Speech Keywords</title>");
                html.WriteLine("      <link rel=\"stylesheet\" type=\"text/css\" href=\"styles.css\" />");
                html.WriteLine("   </head>");
                html.WriteLine("   <body>");
                html.WriteLine("      <h4><a href=\"index.html\">Back to the index</a></h4>");
                html.WriteLine("      <h2>Speech Keywords</h2>");

                for (int p = 0; p < 1 && p < tables.Count; ++p)
                {
                    Dictionary<int, SpeechEntry> table = tables[p];

                    if (p > 0)
                        html.WriteLine("      <br>");

                    html.WriteLine("      <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
                    html.WriteLine("      <tr><td class=\"tbl-border\">");
                    html.WriteLine("      <table width=\"100%\" cellpadding=\"4\" cellspacing=\"1\">");
                    html.WriteLine("         <tr><td class=\"header\">Number</td><td class=\"header\">Text</td></tr>");

                    List<SpeechEntry> list = new List<SpeechEntry>(table.Values);
                    list.Sort(new SpeechEntrySorter());

                    for (int i = 0; i < list.Count; ++i)
                    {
                        SpeechEntry entry = list[i];

                        html.Write("         <tr><td class=\"lentry\">0x{0:X4}</td><td class=\"rentry\">", entry.Index);

                        entry.Strings.Sort();//( new EnglishPrioStringSorter() );

                        for (int j = 0; j < entry.Strings.Count; ++j)
                        {
                            if (j > 0)
                                html.Write("<br>");

                            string v = entry.Strings[j];

                            for (int k = 0; k < v.Length; ++k)
                            {
                                char c = v[k];

                                if (c == '<')
                                    html.Write("&lt;");
                                else if (c == '>')
                                    html.Write("&gt;");
                                else if (c == '&')
                                    html.Write("&amp;");
                                else if (c == '"')
                                    html.Write("&quot;");
                                else if (c == '\'')
                                    html.Write("&apos;");
                                else if (c >= 0x20 && c < 0x7F)
                                    html.Write(c);
                                else
                                    html.Write("&#{0};", (int)c);
                            }
                        }

                        html.WriteLine("</td></tr>");
                    }

                    html.WriteLine("      </table></td></tr></table>");
                }

                html.WriteLine("   </body>");
                html.WriteLine("</html>");
            }
        }

        private class SpeechEntry
        {
            private readonly int m_Index;
            private readonly List<string> m_Strings;

            public int Index
            {
                get
                {
                    return this.m_Index;
                }
            }
            public List<string> Strings
            {
                get
                {
                    return this.m_Strings;
                }
            }

            public SpeechEntry(int index)
            {
                this.m_Index = index;
                this.m_Strings = new List<string>();
            }
        }

        private class SpeechEntrySorter : IComparer<SpeechEntry>
        {
            public int Compare(SpeechEntry x, SpeechEntry y)
            {
                return x.Index.CompareTo(y.Index);
            }
        }

        private static List<Dictionary<int, SpeechEntry>> LoadSpeechFile()
        {
            List<Dictionary<int, SpeechEntry>> tables = new List<Dictionary<int, SpeechEntry>>();
            int lastIndex = -1;

            Dictionary<int, SpeechEntry> table = null;

            string path = Core.FindDataFile("speech.mul");

            if (File.Exists(path))
            {
                using (FileStream ip = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryReader bin = new BinaryReader(ip);

                    while (bin.PeekChar() >= 0)
                    {
                        int index = (bin.ReadByte() << 8) | bin.ReadByte();
                        int length = (bin.ReadByte() << 8) | bin.ReadByte();
                        string text = Encoding.UTF8.GetString(bin.ReadBytes(length)).Trim();

                        if (text.Length == 0)
                            continue;

                        if (table == null || lastIndex > index)
                        {
                            if (index == 0 && text == "*withdraw*")
                                tables.Insert(0, table = new Dictionary<int, SpeechEntry>());
                            else
                                tables.Add(table = new Dictionary<int, SpeechEntry>());
                        }

                        lastIndex = index;

                        SpeechEntry entry = null;
                        table.TryGetValue(index, out entry);

                        if (entry == null)
                            table[index] = entry = new SpeechEntry(index);

                        entry.Strings.Add(text);
                    }
                }
            }

            return tables;
        }

        #endregion

        #region Commands

        public class DocCommandEntry
        {
            private readonly AccessLevel m_AccessLevel;
            private readonly string m_Name;
            private readonly string[] m_Aliases;
            private readonly string m_Usage;
            private readonly string m_Description;

            public AccessLevel AccessLevel
            {
                get
                {
                    return this.m_AccessLevel;
                }
            }
            public string Name
            {
                get
                {
                    return this.m_Name;
                }
            }
            public string[] Aliases
            {
                get
                {
                    return this.m_Aliases;
                }
            }
            public string Usage
            {
                get
                {
                    return this.m_Usage;
                }
            }
            public string Description
            {
                get
                {
                    return this.m_Description;
                }
            }

            public DocCommandEntry(AccessLevel accessLevel, string name, string[] aliases, string usage, string description)
            {
                this.m_AccessLevel = accessLevel;
                this.m_Name = name;
                this.m_Aliases = aliases;
                this.m_Usage = usage;
                this.m_Description = description;
            }
        }

        public class CommandEntrySorter : IComparer<DocCommandEntry>
        {
            public int Compare(DocCommandEntry a, DocCommandEntry b)
            {
                int v = b.AccessLevel.CompareTo(a.AccessLevel);

                if (v == 0)
                    v = a.Name.CompareTo(b.Name);

                return v;
            }
        }

        private static void DocumentCommands()
        {
            using (StreamWriter html = GetWriter("docs/", "commands.html"))
            {
                html.WriteLine("<html>");
                html.WriteLine("   <head>");
                html.WriteLine("      <title>RunUO Documentation - Commands</title>");
                html.WriteLine("      <link rel=\"stylesheet\" type=\"text/css\" href=\"styles.css\" />");
                html.WriteLine("   </head>");
                html.WriteLine("   <body>");
                html.WriteLine("      <a name=\"Top\" />");
                html.WriteLine("      <h4><a href=\"index.html\">Back to the index</a></h4>");
                html.WriteLine("      <h2>Commands</h2>");

                List<CommandEntry> commands = new List<CommandEntry>(CommandSystem.Entries.Values);
                List<DocCommandEntry> list = new List<DocCommandEntry>();

                commands.Sort();
                commands.Reverse();
                Clean(commands);

                for (int i = 0; i < commands.Count; ++i)
                {
                    CommandEntry e = commands[i];

                    MethodInfo mi = e.Handler.Method;

                    object[] attrs = mi.GetCustomAttributes(typeof(UsageAttribute), false);

                    if (attrs.Length == 0)
                        continue;

                    UsageAttribute usage = attrs[0] as UsageAttribute;

                    attrs = mi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attrs.Length == 0)
                        continue;

                    DescriptionAttribute desc = attrs[0] as DescriptionAttribute;

                    if (usage == null || desc == null)
                        continue;

                    attrs = mi.GetCustomAttributes(typeof(AliasesAttribute), false);

                    AliasesAttribute aliases = (attrs.Length == 0 ? null : attrs[0] as AliasesAttribute);

                    string descString = desc.Description.Replace("<", "&lt;").Replace(">", "&gt;");

                    if (aliases == null)
                        list.Add(new DocCommandEntry(e.AccessLevel, e.Command, null, usage.Usage, descString));
                    else
                        list.Add(new DocCommandEntry(e.AccessLevel, e.Command, aliases.Aliases, usage.Usage, descString));
                }

                for (int i = 0; i < TargetCommands.AllCommands.Count; ++i)
                {
                    BaseCommand command = TargetCommands.AllCommands[i];

                    string usage = command.Usage;
                    string desc = command.Description;

                    if (usage == null || desc == null)
                        continue;

                    string[] cmds = command.Commands;
                    string cmd = cmds[0];
                    string[] aliases = new string[cmds.Length - 1];

                    for (int j = 0; j < aliases.Length; ++j)
                        aliases[j] = cmds[j + 1];

                    desc = desc.Replace("<", "&lt;").Replace(">", "&gt;");

                    if (command.Supports != CommandSupport.Single)
                    {
                        StringBuilder sb = new StringBuilder(50 + desc.Length);

                        sb.Append("Modifiers: ");

                        if ((command.Supports & CommandSupport.Global) != 0)
                            sb.Append("<i><a href=\"#Global\">Global</a></i>, ");

                        if ((command.Supports & CommandSupport.Online) != 0)
                            sb.Append("<i><a href=\"#Online\">Online</a></i>, ");

                        if ((command.Supports & CommandSupport.Region) != 0)
                            sb.Append("<i><a href=\"#Region\">Region</a></i>, ");

                        if ((command.Supports & CommandSupport.Contained) != 0)
                            sb.Append("<i><a href=\"#Contained\">Contained</a></i>, ");

                        if ((command.Supports & CommandSupport.Multi) != 0)
                            sb.Append("<i><a href=\"#Multi\">Multi</a></i>, ");

                        if ((command.Supports & CommandSupport.Area) != 0)
                            sb.Append("<i><a href=\"#Area\">Area</a></i>, ");

                        if ((command.Supports & CommandSupport.Self) != 0)
                            sb.Append("<i><a href=\"#Self\">Self</a></i>, ");

                        sb.Remove(sb.Length - 2, 2);
                        sb.Append("<br>");
                        sb.Append(desc);

                        desc = sb.ToString();
                    }

                    list.Add(new DocCommandEntry(command.AccessLevel, cmd, aliases, usage, desc));
                }

                List<BaseCommandImplementor> commandImpls = BaseCommandImplementor.Implementors;

                for (int i = 0; i < commandImpls.Count; ++i)
                {
                    BaseCommandImplementor command = commandImpls[i];

                    string usage = command.Usage;
                    string desc = command.Description;

                    if (usage == null || desc == null)
                        continue;

                    string[] cmds = command.Accessors;
                    string cmd = cmds[0];
                    string[] aliases = new string[cmds.Length - 1];

                    for (int j = 0; j < aliases.Length; ++j)
                        aliases[j] = cmds[j + 1];

                    desc = desc.Replace("<", "&lt;").Replace(">", "&gt;");

                    list.Add(new DocCommandEntry(command.AccessLevel, cmd, aliases, usage, desc));
                }

                list.Sort(new CommandEntrySorter());

                AccessLevel last = AccessLevel.Player;

                foreach (DocCommandEntry e in list)
                {
                    if (e.AccessLevel != last)
                    {
                        if (last >= AccessLevel.Counselor)
                            html.WriteLine("      </table></td></tr></table><br>");

                        last = e.AccessLevel;

                        html.WriteLine("      <a name=\"{0}\" />", last);

                        switch (last)
                        {
                            case AccessLevel.Administrator:
                                html.WriteLine("      <b>Administrator</b> | <a href=\"#GameMaster\">Game Master</a> | <a href=\"#Counselor\">Counselor</a> | <a href=\"#Player\">Player</a><br><br>");
                                break;
                            case AccessLevel.GameMaster:
                                html.WriteLine("      <a href=\"#Top\">Administrator</a> | <b>Game Master</b> | <a href=\"#Counselor\">Counselor</a> | <a href=\"#Player\">Player</a><br><br>");
                                break;
                            case AccessLevel.Seer:
                                html.WriteLine("      <a href=\"#Top\">Administrator</a> | <a href=\"#GameMaster\">Game Master</a> | <a href=\"#Counselor\">Counselor</a> | <a href=\"#Player\">Player</a><br><br>");
                                break;
                            case AccessLevel.Counselor:
                                html.WriteLine("      <a href=\"#Top\">Administrator</a> | <a href=\"#GameMaster\">Game Master</a> | <b>Counselor</b> | <a href=\"#Player\">Player</a><br><br>");
                                break;
                            case AccessLevel.Player:
                                html.WriteLine("      <a href=\"#Top\">Administrator</a> | <a href=\"#GameMaster\">Game Master</a> | <a href=\"#Counselor\">Counselor</a> | <b>Player</b><br><br>");
                                break;
                        }

                        html.WriteLine("      <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
                        html.WriteLine("      <tr><td class=\"tbl-border\">");
                        html.WriteLine("      <table width=\"100%\" cellpadding=\"4\" cellspacing=\"1\">");
                        html.WriteLine("         <tr><td colspan=\"2\" width=\"100%\" class=\"header\">{0}</td></tr>", last == AccessLevel.GameMaster ? "Game Master" : last.ToString());
                    }

                    DocumentCommand(html, e);
                }

                html.WriteLine("      </table></td></tr></table>");
                html.WriteLine("   </body>");
                html.WriteLine("</html>");
            }
        }

        public static void Clean(List<CommandEntry> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                CommandEntry e = list[i];

                for (int j = i + 1; j < list.Count; ++j)
                {
                    CommandEntry c = list[j];

                    if (e.Handler.Method == c.Handler.Method)
                    {
                        list.RemoveAt(j);
                        --j;
                    }
                }
            }
        }

        private static void DocumentCommand(StreamWriter html, DocCommandEntry e)
        {
            string usage = e.Usage;
            string desc = e.Description;
            string[] aliases = e.Aliases;

            html.Write("         <tr><a name=\"{0}\" /><td class=\"lentry\">{0}</td>", e.Name);

            if (aliases == null || aliases.Length == 0)
            {
                html.Write("<td class=\"rentry\"><b>Usage: {0}</b><br>{1}</td>", usage.Replace("<", "&lt;").Replace(">", "&gt;"), desc);
            }
            else
            {
                html.Write("<td class=\"rentry\"><b>Usage: {0}</b><br>Alias{1}: ", usage.Replace("<", "&lt;").Replace(">", "&gt;"), aliases.Length == 1 ? "" : "es");

                for (int i = 0; i < aliases.Length; ++i)
                {
                    if (i != 0)
                        html.Write(", ");

                    html.Write(aliases[i]);
                }

                html.Write("<br>{0}</td>", desc);
            }

            html.WriteLine("</tr>");
        }

        #endregion

        private static void LoadTypes(Assembly a, Assembly[] asms)
        {
            Type[] types = a.GetTypes();

            for (int i = 0; i < types.Length; ++i)
            {
                Type type = types[i];

                string nspace = type.Namespace;

                if (nspace == null || type.IsSpecialName)
                    continue;

                TypeInfo info = new TypeInfo(type);
                m_Types[type] = info;

                List<TypeInfo> nspaces = null;
                m_Namespaces.TryGetValue(nspace, out nspaces);

                if (nspaces == null)
                    m_Namespaces[nspace] = nspaces = new List<TypeInfo>();

                nspaces.Add(info);

                Type baseType = info.m_BaseType;

                if (baseType != null && InAssemblies(baseType, asms))
                {
                    TypeInfo baseInfo = null;
                    m_Types.TryGetValue(baseType, out baseInfo);

                    if (baseInfo == null)
                        m_Types[baseType] = baseInfo = new TypeInfo(baseType);

                    if (baseInfo.m_Derived == null)
                        baseInfo.m_Derived = new List<TypeInfo>();

                    baseInfo.m_Derived.Add(info);
                }

                Type decType = info.m_Declaring;

                if (decType != null)
                {
                    TypeInfo decInfo = null;
                    m_Types.TryGetValue(decType, out decInfo);

                    if (decInfo == null)
                        m_Types[decType] = decInfo = new TypeInfo(decType);

                    if (decInfo.m_Nested == null)
                        decInfo.m_Nested = new List<TypeInfo>();

                    decInfo.m_Nested.Add(info);
                }

                for (int j = 0; j < info.m_Interfaces.Length; ++j)
                {
                    Type iface = info.m_Interfaces[j];

                    if (!InAssemblies(iface, asms))
                        continue;

                    TypeInfo ifaceInfo = null;
                    m_Types.TryGetValue(iface, out ifaceInfo);

                    if (ifaceInfo == null)
                        m_Types[iface] = ifaceInfo = new TypeInfo(iface);

                    if (ifaceInfo.m_Derived == null)
                        ifaceInfo.m_Derived = new List<TypeInfo>();

                    ifaceInfo.m_Derived.Add(info);
                }
            }
        }

        private static bool InAssemblies(Type t, Assembly[] asms)
        {
            Assembly a = t.Assembly;

            for (int i = 0; i < asms.Length; ++i)
                if (a == asms[i])
                    return true;

            return false;
        }

        #region Constructable Objects
        private static readonly Type typeofItem = typeof(Item);

        private static readonly Type typeofMobile = typeof(Mobile);

        private static readonly Type typeofMap = typeof(Map);

        private static readonly Type typeofCustomEnum = typeof(CustomEnumAttribute);

        private static bool IsConstructable(Type t, out bool isItem)
        {
            if (isItem = typeofItem.IsAssignableFrom(t))
                return true;

            return typeofMobile.IsAssignableFrom(t);
        }

        private static bool IsConstructable(ConstructorInfo ctor)
        {
            return ctor.IsDefined(typeof(ConstructableAttribute), false);
        }

        private static void DocumentConstructableObjects()
        {
            List<TypeInfo> types = new List<TypeInfo>(m_Types.Values);
            types.Sort(new TypeComparer());

            ArrayList items = new ArrayList(), mobiles = new ArrayList();

            for (int i = 0; i < types.Count; ++i)
            {
                Type t = types[i].m_Type;
                bool isItem;

                if (t.IsAbstract || !IsConstructable(t, out isItem))
                    continue;

                ConstructorInfo[] ctors = t.GetConstructors();
                bool anyConstructable = false;

                for (int j = 0; !anyConstructable && j < ctors.Length; ++j)
                    anyConstructable = IsConstructable(ctors[j]);

                if (anyConstructable)
                {
                    (isItem ? items : mobiles).Add(t);
                    (isItem ? items : mobiles).Add(ctors);
                }
            }

            using (StreamWriter html = GetWriter("docs/", "objects.html"))
            {
                html.WriteLine("<html>");
                html.WriteLine("   <head>");
                html.WriteLine("      <title>RunUO Documentation - Constructable Objects</title>");
                html.WriteLine("      <link rel=\"stylesheet\" type=\"text/css\" href=\"styles.css\" />");
                html.WriteLine("   </head>");
                html.WriteLine("   <body>");
                html.WriteLine("      <h4><a href=\"index.html\">Back to the index</a></h4>");
                html.WriteLine("      <h2>Constructable <a href=\"#items\">Items</a> and <a href=\"#mobiles\">Mobiles</a></h2>");

                html.WriteLine("      <a name=\"items\" />");
                html.WriteLine("      <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
                html.WriteLine("      <tr><td class=\"tbl-border\">");
                html.WriteLine("      <table width=\"100%\" cellpadding=\"4\" cellspacing=\"1\">");
                html.WriteLine("         <tr><td class=\"header\">Item Name</td><td class=\"header\">Usage</td></tr>");

                for (int i = 0; i < items.Count; i += 2)
                    DocumentConstructableObject(html, (Type)items[i], (ConstructorInfo[])items[i + 1]);

                html.WriteLine("      </table></td></tr></table><br><br>");

                html.WriteLine("      <a name=\"mobiles\" />");
                html.WriteLine("      <table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
                html.WriteLine("      <tr><td class=\"tbl-border\">");
                html.WriteLine("      <table width=\"100%\" cellpadding=\"4\" cellspacing=\"1\">");
                html.WriteLine("         <tr><td class=\"header\">Mobile Name</td><td class=\"header\">Usage</td></tr>");

                for (int i = 0; i < mobiles.Count; i += 2)
                    DocumentConstructableObject(html, (Type)mobiles[i], (ConstructorInfo[])mobiles[i + 1]);

                html.WriteLine("      </table></td></tr></table>");

                html.WriteLine("   </body>");
                html.WriteLine("</html>");
            }
        }

        private static void DocumentConstructableObject(StreamWriter html, Type t, ConstructorInfo[] ctors)
        {
            html.Write("         <tr><td class=\"lentry\">{0}</td><td class=\"rentry\">", t.Name);

            bool first = true;

            for (int i = 0; i < ctors.Length; ++i)
            {
                ConstructorInfo ctor = ctors[i];

                if (!IsConstructable(ctor))
                    continue;

                if (!first)
                    html.Write("<br>");

                first = false;

                html.Write("{0}Add {1}", CommandSystem.Prefix, t.Name);

                ParameterInfo[] parms = ctor.GetParameters();

                for (int j = 0; j < parms.Length; ++j)
                {
                    html.Write(" <a ");

                    TypeInfo typeInfo = null;
                    m_Types.TryGetValue(parms[j].ParameterType, out typeInfo);

                    if (typeInfo != null)
                        html.Write("href=\"types/{0}\" ", typeInfo.FileName);

                    html.Write("title=\"{0}\">{1}</a>", GetTooltipFor(parms[j]), parms[j].Name);
                }
            }

            html.WriteLine("</td></tr>");
        }

        #endregion

        #region Tooltips

        private const string HtmlNewLine = "&#13;";

        private static readonly object[,] m_Tooltips = new object[,]
        {
            { typeof(Byte), "Numeric value in the range from 0 to 255, inclusive." },
            { typeof(SByte), "Numeric value in the range from negative 128 to positive 127, inclusive." },
            { typeof(UInt16), "Numeric value in the range from 0 to 65,535, inclusive." },
            { typeof(Int16), "Numeric value in the range from negative 32,768 to positive 32,767, inclusive." },
            { typeof(UInt32), "Numeric value in the range from 0 to 4,294,967,295, inclusive." },
            { typeof(Int32), "Numeric value in the range from negative 2,147,483,648 to positive 2,147,483,647, inclusive." },
            { typeof(UInt64), "Numeric value in the range from 0 through about 10^20." },
            { typeof(Int64), "Numeric value in the approximate range from negative 10^19 through 10^19." },
            { typeof(String), "Text value. To specify a value containing spaces, encapsulate the value in quote characters:{0}{0}&quot;Spaced text example&quot;" },
            { typeof(Boolean), "Boolean value which can be either True or False." },
            { typeof(Map), "Map or facet name. Possible values include:{0}{0}- Felucca{0}- Trammel{0}- Ilshenar{0}- Malas" },
            { typeof(Poison), "Poison name or level. Possible values include:{0}{0}- Lesser{0}- Regular{0}- Greater{0}- Deadly{0}- Lethal" },
            { typeof(Point3D), "Three-dimensional coordinate value. Format as follows:{0}{0}&quot;(<x value>, <y value>, <z value>)&quot;" }
        };

        private static string GetTooltipFor(ParameterInfo param)
        {
            Type paramType = param.ParameterType;

            for (int i = 0; i < m_Tooltips.GetLength(0); ++i)
            {
                Type checkType = (Type)m_Tooltips[i, 0];

                if (paramType == checkType)
                    return String.Format((string)m_Tooltips[i, 1], HtmlNewLine);
            }

            if (paramType.IsEnum)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("Enumeration value or name. Possible named values include:{0}", HtmlNewLine);

                string[] names = Enum.GetNames(paramType);

                for (int i = 0; i < names.Length; ++i)
                    sb.AppendFormat("{0}- {1}", HtmlNewLine, names[i]);

                return sb.ToString();
            }
            else if (paramType.IsDefined(typeofCustomEnum, false))
            {
                object[] attributes = paramType.GetCustomAttributes(typeofCustomEnum, false);

                if (attributes != null && attributes.Length > 0)
                {
                    CustomEnumAttribute attr = attributes[0] as CustomEnumAttribute;

                    if (attr != null)
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.AppendFormat("Enumeration value or name. Possible named values include:{0}", HtmlNewLine);

                        string[] names = attr.Names;

                        for (int i = 0; i < names.Length; ++i)
                            sb.AppendFormat("{0}- {1}", HtmlNewLine, names[i]);

                        return sb.ToString();
                    }
                }
            }
            else if (paramType == typeofMap)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("Enumeration value or name. Possible named values include:{0}", HtmlNewLine);

                string[] names = Map.GetMapNames();

                for (int i = 0; i < names.Length; ++i)
                    sb.AppendFormat("{0}- {1}", HtmlNewLine, names[i]);

                return sb.ToString();
            }

            return "";
        }

        #endregion

        #region Const Strings

        private const string RefString = "<font color=\"blue\">ref</font> ";
        private const string GetString = " <font color=\"blue\">get</font>;";
        private const string SetString = " <font color=\"blue\">set</font>;";

        private const string InString = "<font color=\"blue\">in</font> ";
        private const string OutString = "<font color=\"blue\">out</font> ";

        private const string VirtString = "<font color=\"blue\">virtual</font> ";
        private const string CtorString = "(<font color=\"blue\">ctor</font>) ";
        private const string StaticString = "(<font color=\"blue\">static</font>) ";
        #endregion

        private static void DocumentLoadedTypes()
        {
            using (StreamWriter indexHtml = GetWriter("docs/", "overview.html"))
            {
                indexHtml.WriteLine("<html>");
                indexHtml.WriteLine("   <head>");
                indexHtml.WriteLine("      <title>RunUO Documentation - Class Overview</title>");
                indexHtml.WriteLine("   </head>");
                indexHtml.WriteLine("   <body bgcolor=\"white\" style=\"font-family: Courier New\" text=\"#000000\" link=\"#000000\" vlink=\"#000000\" alink=\"#808080\">");
                indexHtml.WriteLine("      <h4><a href=\"index.html\">Back to the index</a></h4>");
                indexHtml.WriteLine("      <h2>Namespaces</h2>");

                SortedList<string, List<TypeInfo>> nspaces = new SortedList<string, List<TypeInfo>>(m_Namespaces);

                foreach (KeyValuePair<string, List<TypeInfo>> kvp in nspaces)
                {
                    kvp.Value.Sort(new TypeComparer());

                    SaveNamespace(kvp.Key, kvp.Value, indexHtml);
                }

                indexHtml.WriteLine("   </body>");
                indexHtml.WriteLine("</html>");
            }
        }

        private static void SaveNamespace(string name, List<TypeInfo> types, StreamWriter indexHtml)
        {
            string fileName = GetFileName("docs/namespaces/", name, ".html");

            indexHtml.WriteLine("      <a href=\"namespaces/{0}\">{1}</a><br>", fileName, name);

            using (StreamWriter nsHtml = GetWriter("docs/namespaces/", fileName))
            {
                nsHtml.WriteLine("<html>");
                nsHtml.WriteLine("   <head>");
                nsHtml.WriteLine("      <title>RunUO Documentation - Class Overview - {0}</title>", name);
                nsHtml.WriteLine("   </head>");
                nsHtml.WriteLine("   <body bgcolor=\"white\" style=\"font-family: Courier New\" text=\"#000000\" link=\"#000000\" vlink=\"#000000\" alink=\"#808080\">");
                nsHtml.WriteLine("      <h4><a href=\"../overview.html\">Back to the namespace index</a></h4>");
                nsHtml.WriteLine("      <h2>{0}</h2>", name);

                for (int i = 0; i < types.Count; ++i)
                    SaveType(types[i], nsHtml, fileName, name);

                nsHtml.WriteLine("   </body>");
                nsHtml.WriteLine("</html>");
            }
        }

        private static void SaveType(TypeInfo info, StreamWriter nsHtml, string nsFileName, string nsName)
        {
            if (info.m_Declaring == null)
                nsHtml.WriteLine("      <!-- DBG-ST -->" + info.LinkName("../types/") + "<br>");

            using (StreamWriter typeHtml = Docs.GetWriter(info.FileName))
            {
                typeHtml.WriteLine("<html>");
                typeHtml.WriteLine("   <head>");
                typeHtml.WriteLine("      <title>RunUO Documentation - Class Overview - {0}</title>", info.TypeName);
                typeHtml.WriteLine("   </head>");
                typeHtml.WriteLine("   <body bgcolor=\"white\" style=\"font-family: Courier New\" text=\"#000000\" link=\"#000000\" vlink=\"#000000\" alink=\"#808080\">");
                typeHtml.WriteLine("      <h4><a href=\"../namespaces/{0}\">Back to {1}</a></h4>", nsFileName, nsName);

                if (info.m_Type.IsEnum)
                    WriteEnum(info, typeHtml);
                else
                    WriteType(info, typeHtml);

                typeHtml.WriteLine("   </body>");
                typeHtml.WriteLine("</html>");
            }
        }

        #region Write[...]
        private static void WriteEnum(TypeInfo info, StreamWriter typeHtml)
        {
            Type type = info.m_Type;

            typeHtml.WriteLine("      <h2>{0} (Enum)</h2>", info.TypeName);

            string[] names = Enum.GetNames(type);

            bool flags = type.IsDefined(typeof(FlagsAttribute), false);
            string format;

            if (flags)
                format = "      {0:G} = 0x{1:X}{2}<br>";
            else
                format = "      {0:G} = {1:D}{2}<br>";

            for (int i = 0; i < names.Length; ++i)
            {
                object value = Enum.Parse(type, names[i]);

                typeHtml.WriteLine(format, names[i], value, i < (names.Length - 1) ? "," : "");
            }
        }

        private static void WriteType(TypeInfo info, StreamWriter typeHtml)
        {
            Type type = info.m_Type;

            typeHtml.Write("      <h2>");

            Type decType = info.m_Declaring;

            if (decType != null)
            {
                // We are a nested type
                typeHtml.Write('(');

                TypeInfo decInfo = null;
                m_Types.TryGetValue(decType, out decInfo);

                if (decInfo == null)
                    typeHtml.Write(decType.Name);
                else
                //typeHtml.Write( "<a href=\"{0}\">{1}</a>", decInfo.m_FileName, decInfo.m_TypeName );
                    typeHtml.Write(decInfo.LinkName(null));

                typeHtml.Write(") - ");
            }

            typeHtml.Write(info.TypeName);

            Type[] ifaces = info.m_Interfaces;
            Type baseType = info.m_BaseType;

            int extendCount = 0;

            if (baseType != null && baseType != typeof(object) && baseType != typeof(ValueType) && !baseType.IsPrimitive)
            {
                typeHtml.Write(" : ");

                TypeInfo baseInfo = null;
                m_Types.TryGetValue(baseType, out baseInfo);

                if (baseInfo == null)
                    typeHtml.Write(baseType.Name);
                else
                {
                    typeHtml.Write("<!-- DBG-1 -->" + baseInfo.LinkName(null));
                }

                ++extendCount;
            }

            if (ifaces.Length > 0)
            {
                if (extendCount == 0)
                    typeHtml.Write(" : ");

                for (int i = 0; i < ifaces.Length; ++i)
                {
                    Type iface = ifaces[i];
                    TypeInfo ifaceInfo = null;
                    m_Types.TryGetValue(iface, out ifaceInfo);

                    if (extendCount != 0)
                        typeHtml.Write(", ");

                    ++extendCount;

                    if (ifaceInfo == null)
                    {
                        string typeName = "";
                        string fileName = "";
                        string linkName = "";
                        FormatGeneric(iface, ref typeName, ref fileName, ref linkName);
                        linkName = linkName.Replace("@directory@", null);
                        typeHtml.Write("<!-- DBG-2.1 -->" + linkName);
                    }
                    else
                    {
                        typeHtml.Write("<!-- DBG-2.2 -->" + ifaceInfo.LinkName(null));
                    }
                }
            }

            typeHtml.WriteLine("</h2>");

            List<TypeInfo> derived = info.m_Derived;

            if (derived != null)
            {
                typeHtml.Write("<h4>Derived Types: ");

                derived.Sort(new TypeComparer());

                for (int i = 0; i < derived.Count; ++i)
                {
                    TypeInfo derivedInfo = derived[i];

                    if (i != 0)
                        typeHtml.Write(", ");

                    //typeHtml.Write( "<a href=\"{0}\">{1}</a>", derivedInfo.m_FileName, derivedInfo.m_TypeName );
                    typeHtml.Write("<!-- DBG-3 -->" + derivedInfo.LinkName(null));
                }

                typeHtml.WriteLine("</h4>");
            }

            List<TypeInfo> nested = info.m_Nested;

            if (nested != null)
            {
                typeHtml.Write("<h4>Nested Types: ");

                nested.Sort(new TypeComparer());

                for (int i = 0; i < nested.Count; ++i)
                {
                    TypeInfo nestedInfo = nested[i];

                    if (i != 0)
                        typeHtml.Write(", ");

                    //typeHtml.Write( "<a href=\"{0}\">{1}</a>", nestedInfo.m_FileName, nestedInfo.m_TypeName );
                    typeHtml.Write("<!-- DBG-4 -->" + nestedInfo.LinkName(null));
                }

                typeHtml.WriteLine("</h4>");
            }

            MemberInfo[] membs = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            Array.Sort(membs, new MemberComparer());

            for (int i = 0; i < membs.Length; ++i)
            {
                MemberInfo mi = membs[i];

                if (mi is PropertyInfo)
                    WriteProperty((PropertyInfo)mi, typeHtml);
                else if (mi is ConstructorInfo)
                    WriteCtor(info.TypeName, (ConstructorInfo)mi, typeHtml);
                else if (mi is MethodInfo)
                    WriteMethod((MethodInfo)mi, typeHtml);
            }
        }

        private static void WriteProperty(PropertyInfo pi, StreamWriter html)
        {
            html.Write("      ");

            MethodInfo getMethod = pi.GetGetMethod();
            MethodInfo setMethod = pi.GetSetMethod();

            if ((getMethod != null && getMethod.IsStatic) || (setMethod != null && setMethod.IsStatic))
                html.Write(StaticString);

            html.Write(GetPair(pi.PropertyType, pi.Name, false));
            html.Write('(');

            if (pi.CanRead)
                html.Write(GetString);

            if (pi.CanWrite)
                html.Write(SetString);

            html.WriteLine(" )<br>");
        }

        private static void WriteCtor(string name, ConstructorInfo ctor, StreamWriter html)
        {
            if (ctor.IsStatic)
                return;

            html.Write("      ");
            html.Write(CtorString);
            html.Write(name);
            html.Write('(');

            ParameterInfo[] parms = ctor.GetParameters();

            if (parms.Length > 0)
            {
                html.Write(' ');

                for (int i = 0; i < parms.Length; ++i)
                {
                    ParameterInfo pi = parms[i];

                    if (i != 0)
                        html.Write(", ");

                    if (pi.IsIn)
                        html.Write(InString);
                    else if (pi.IsOut)
                        html.Write(OutString);

                    html.Write(GetPair(pi.ParameterType, pi.Name, pi.IsOut));
                }

                html.Write(' ');
            }

            html.WriteLine(")<br>");
        }

        private static void WriteMethod(MethodInfo mi, StreamWriter html)
        {
            if (mi.IsSpecialName)
                return;

            html.Write("      ");

            if (mi.IsStatic)
                html.Write(StaticString);

            if (mi.IsVirtual)
                html.Write(VirtString);

            html.Write(GetPair(mi.ReturnType, mi.Name, false));
            html.Write('(');

            ParameterInfo[] parms = mi.GetParameters();

            if (parms.Length > 0)
            {
                html.Write(' ');

                for (int i = 0; i < parms.Length; ++i)
                {
                    ParameterInfo pi = parms[i];

                    if (i != 0)
                        html.Write(", ");

                    if (pi.IsIn)
                        html.Write(InString);
                    else if (pi.IsOut)
                        html.Write(OutString);

                    html.Write(GetPair(pi.ParameterType, pi.Name, pi.IsOut));
                }

                html.Write(' ');
            }

            html.WriteLine(")<br>");
        }

        #endregion

        public static void FormatGeneric(Type type, ref string typeName, ref string fileName, ref string linkName)
        {
            string name = null;
            string fnam = null;
            string link = null;

            if (type.IsGenericType)
            {
                int index = type.Name.IndexOf('`');

                if (index > 0)
                {
                    string rootType = type.Name.Substring(0, index);

                    StringBuilder nameBuilder = new StringBuilder(rootType);
                    StringBuilder fnamBuilder = new StringBuilder("docs/types/" + Docs.SanitizeType(rootType));
                    StringBuilder linkBuilder;
                    if (DontLink(type))//if( DontLink( rootType ) )
                        linkBuilder = new StringBuilder("<font color=\"blue\">" + rootType + "</font>");
                    else
                        linkBuilder = new StringBuilder("<a href=\"" + "@directory@" + rootType + "-T-.html\">" + rootType + "</a>");

                    nameBuilder.Append("&lt;");
                    fnamBuilder.Append("-");
                    linkBuilder.Append("&lt;");

                    Type[] typeArguments = type.GetGenericArguments();

                    for (int i = 0; i < typeArguments.Length; i++)
                    {
                        if (i != 0)
                        {
                            nameBuilder.Append(',');
                            fnamBuilder.Append(',');
                            linkBuilder.Append(',');
                        }

                        string sanitizedName = Docs.SanitizeType(typeArguments[i].Name);
                        string aliasedName = Docs.AliasForName(sanitizedName);

                        nameBuilder.Append(sanitizedName);
                        fnamBuilder.Append("T");
                        if (DontLink(typeArguments[i]))//if( DontLink( typeArguments[i].Name ) )
                            linkBuilder.Append("<font color=\"blue\">" + aliasedName + "</font>");
                        else
                            linkBuilder.Append("<a href=\"" + "@directory@" + aliasedName + ".html\">" + aliasedName + "</a>");
                    }

                    nameBuilder.Append("&gt;");
                    fnamBuilder.Append("-");
                    linkBuilder.Append("&gt;");

                    name = nameBuilder.ToString();
                    fnam = fnamBuilder.ToString();
                    link = linkBuilder.ToString();
                }
            }
            if (name == null)
                typeName = type.Name;
            else
                typeName = name;

            if (fnam == null)
                fileName = "docs/types/" + Docs.SanitizeType(type.Name) + ".html";
            else
                fileName = fnam + ".html";

            if (link == null)
            {
                if (DontLink(type)) //if( DontLink( type.Name ) )
                    linkName = "<font color=\"blue\">" + Docs.SanitizeType(type.Name) + "</font>";
                else
                    linkName = "<a href=\"" + "@directory@" + Docs.SanitizeType(type.Name) + ".html\">" + Docs.SanitizeType(type.Name) + "</a>";
            }
            else
                linkName = link;
            //Console.WriteLine( typeName+":"+fileName+":"+linkName );
        }

        public static string SanitizeType(string name)
        {
            bool anonymousType = false;
            if (name.Contains("<"))
                anonymousType = true;
            StringBuilder sb = new StringBuilder(name);
            for (int i = 0; i < ReplaceChars.Length; ++i)
            {
                sb.Replace(ReplaceChars[i], '-');
            }

            if (anonymousType)
                return "(Anonymous-Type)" + sb.ToString();
            else
                return sb.ToString();
        }

        public static string AliasForName(string name)
        {
            for (int i = 0; i < m_AliasLength; ++i)
            {
                if (m_Aliases[i, 0] == name)
                {
                    return m_Aliases[i, 1];
                }
            }
            return name;
        }

        /*
        // For stuff we don't want to links to
        private static string[] m_DontLink = new string[]
        {
        "List",
        "Stack",
        "Queue",
        "Dictionary",
        "LinkedList",
        "SortedList",
        "SortedDictionary",
        "IComparable",
        "IComparer",
        "ICloneable",
        "Type"
        };

        public static bool DontLink( string name )
        {
        foreach( string dontLink in m_DontLink )
        if( dontLink == name ) return true;
        return false;
        }
        */
        public static bool DontLink(Type type)
        {
            // MONO: type.Namespace is null/empty for generic arguments
            if (type.Name == "T" || String.IsNullOrEmpty(type.Namespace) || m_Namespaces == null)
                return true;

            if (type.Namespace.StartsWith("Server"))
                return false;

            return !m_Namespaces.ContainsKey(type.Namespace);
        }
    }

    #region BodyEntry & BodyType
    public enum ModelBodyType
    {
        Invalid = -1,
        Monsters,
        Sea,
        Animals,
        Human,
        Equipment
    }

    public class BodyEntry
    {
        private readonly Body m_Body;
        private readonly ModelBodyType m_BodyType;
        private readonly string m_Name;

        public Body Body
        {
            get
            {
                return this.m_Body;
            }
        }
        public ModelBodyType BodyType
        {
            get
            {
                return this.m_BodyType;
            }
        }
        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }

        public BodyEntry(Body body, ModelBodyType bodyType, string name)
        {
            this.m_Body = body;
            this.m_BodyType = bodyType;
            this.m_Name = name;
        }

        public override bool Equals(object obj)
        {
            BodyEntry e = (BodyEntry)obj;

            return (this.m_Body == e.m_Body && this.m_BodyType == e.m_BodyType && this.m_Name == e.m_Name);
        }

        public override int GetHashCode()
        {
            return this.m_Body.BodyID ^ (int)this.m_BodyType ^ this.m_Name.GetHashCode();
        }
    }

    public class BodyEntrySorter : IComparer<BodyEntry>
    {
        public int Compare(BodyEntry a, BodyEntry b)
        {
            int v = a.BodyType.CompareTo(b.BodyType);

            if (v == 0)
                v = a.Body.BodyID.CompareTo(b.Body.BodyID);

            if (v == 0)
                v = a.Name.CompareTo(b.Name);

            return v;
        }
    }
    #endregion
}