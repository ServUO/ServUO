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
using System.IO;
using System.Linq;

using Server;
#endregion

namespace VitaNex.IO
{
	public struct FileMime : IEquatable<FileMime>, IComparable<FileMime>
	{
		private static readonly Dictionary<string, FileMime> _Registry;

		public static IEnumerable<FileMime> Registry => _Registry.Values;

		public static readonly FileMime Default = new FileMime(null, null);

		static FileMime()
		{
			_Registry = new Dictionary<string, FileMime>(StringComparer.OrdinalIgnoreCase);

			#region Definitions
			Register("323", "text/h323");
			Register("3g2", "video/3gpp2");
			Register("3gp", "video/3gpp");
			Register("3gp2", "video/3gpp2");
			Register("3gpp", "video/3gpp");
			Register("7z", "application/x-7z-compressed");
			Register("aa", "audio/audible");
			Register("AAC", "audio/aac");
			Register("aaf", "application/octet-stream");
			Register("aax", "audio/vnd.audible.aax");
			Register("ac3", "audio/ac3");
			Register("aca", "application/octet-stream");
			Register("accda", "application/msaccess.addin");
			Register("accdb", "application/msaccess");
			Register("accdc", "application/msaccess.cab");
			Register("accde", "application/msaccess");
			Register("accdr", "application/msaccess.runtime");
			Register("accdt", "application/msaccess");
			Register("accdw", "application/msaccess.webapplication");
			Register("accft", "application/msaccess.ftemplate");
			Register("acx", "application/internet-property-stream");
			Register("AddIn", "text/xml");
			Register("ade", "application/msaccess");
			Register("adobebridge", "application/x-bridge-url");
			Register("adp", "application/msaccess");
			Register("ADT", "audio/vnd.dlna.adts");
			Register("ADTS", "audio/aac");
			Register("afm", "application/octet-stream");
			Register("ai", "application/postscript");
			Register("aif", "audio/x-aiff");
			Register("aifc", "audio/aiff");
			Register("aiff", "audio/aiff");
			Register("air", "application/vnd.adobe.air-application-installer-package+zip");
			Register("amc", "application/x-mpeg");
			Register("application", "application/x-ms-application");
			Register("art", "image/x-jg");
			Register("asa", "application/xml");
			Register("asax", "application/xml");
			Register("ascx", "application/xml");
			Register("asd", "application/octet-stream");
			Register("asf", "video/x-ms-asf");
			Register("ashx", "application/xml");
			Register("asi", "application/octet-stream");
			Register("asm", "text/plain");
			Register("asmx", "application/xml");
			Register("aspx", "application/xml");
			Register("asr", "video/x-ms-asf");
			Register("asx", "video/x-ms-asf");
			Register("atom", "application/atom+xml");
			Register("au", "audio/basic");
			Register("avi", "video/x-msvideo");
			Register("axs", "application/olescript");
			Register("bas", "text/plain");
			Register("bcpio", "application/x-bcpio");
			Register("bin", "application/octet-stream");
			Register("bmp", "image/bmp");
			Register("c", "text/plain");
			Register("cab", "application/octet-stream");
			Register("caf", "audio/x-caf");
			Register("calx", "application/vnd.ms-office.calx");
			Register("cat", "application/vnd.ms-pki.seccat");
			Register("cc", "text/plain");
			Register("cd", "text/plain");
			Register("cdda", "audio/aiff");
			Register("cdf", "application/x-cdf");
			Register("cer", "application/x-x509-ca-cert");
			Register("chm", "application/octet-stream");
			Register("class", "application/x-java-applet");
			Register("clp", "application/x-msclip");
			Register("cmx", "image/x-cmx");
			Register("cnf", "text/plain");
			Register("cod", "image/cis-cod");
			Register("config", "application/xml");
			Register("contact", "text/x-ms-contact");
			Register("coverage", "application/xml");
			Register("cpio", "application/x-cpio");
			Register("cpp", "text/plain");
			Register("crd", "application/x-mscardfile");
			Register("crl", "application/pkix-crl");
			Register("crt", "application/x-x509-ca-cert");
			Register("cs", "text/plain");
			Register("csdproj", "text/plain");
			Register("csh", "application/x-csh");
			Register("csproj", "text/plain");
			Register("css", "text/css");
			Register("csv", "text/csv");
			Register("cur", "application/octet-stream");
			Register("cxx", "text/plain");
			Register("dat", "application/octet-stream");
			Register("datasource", "application/xml");
			Register("dbproj", "text/plain");
			Register("dcr", "application/x-director");
			Register("def", "text/plain");
			Register("deploy", "application/octet-stream");
			Register("der", "application/x-x509-ca-cert");
			Register("dgml", "application/xml");
			Register("dib", "image/bmp");
			Register("dif", "video/x-dv");
			Register("dir", "application/x-director");
			Register("disco", "text/xml");
			Register("dll", "application/x-msdownload");
			Register("dll.config", "text/xml");
			Register("dlm", "text/dlm");
			Register("doc", "application/msword");
			Register("docm", "application/vnd.ms-word.document.macroEnabled.12");
			Register("docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
			Register("dot", "application/msword");
			Register("dotm", "application/vnd.ms-word.template.macroEnabled.12");
			Register("dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template");
			Register("dsp", "application/octet-stream");
			Register("dsw", "text/plain");
			Register("dtd", "text/xml");
			Register("dtsConfig", "text/xml");
			Register("dv", "video/x-dv");
			Register("dvi", "application/x-dvi");
			Register("dwf", "drawing/x-dwf");
			Register("dwp", "application/octet-stream");
			Register("dxr", "application/x-director");
			Register("eml", "message/rfc822");
			Register("emz", "application/octet-stream");
			Register("eot", "application/octet-stream");
			Register("eps", "application/postscript");
			Register("etl", "application/etl");
			Register("etx", "text/x-setext");
			Register("evy", "application/envoy");
			Register("exe", "application/octet-stream");
			Register("exe.config", "text/xml");
			Register("fdf", "application/vnd.fdf");
			Register("fif", "application/fractals");
			Register("filters", "Application/xml");
			Register("fla", "application/octet-stream");
			Register("flr", "x-world/x-vrml");
			Register("flv", "video/x-flv");
			Register("form", "application/x-www-form-urlencoded");
			Register("fsscript", "application/fsharp-script");
			Register("fsx", "application/fsharp-script");
			Register("generictest", "application/xml");
			Register("gif", "image/gif");
			Register("group", "text/x-ms-group");
			Register("gsm", "audio/x-gsm");
			Register("gtar", "application/x-gtar");
			Register("gz", "application/x-gzip");
			Register("h", "text/plain");
			Register("hash", "text/plain");
			Register("hdf", "application/x-hdf");
			Register("hdml", "text/x-hdml");
			Register("hhc", "application/x-oleobject");
			Register("hhk", "application/octet-stream");
			Register("hhp", "application/octet-stream");
			Register("hlp", "application/winhlp");
			Register("hpp", "text/plain");
			Register("hqx", "application/mac-binhex40");
			Register("hta", "application/hta");
			Register("htc", "text/x-component");
			Register("htm", "text/html");
			Register("html", "text/html");
			Register("htt", "text/webviewhtml");
			Register("hxa", "application/xml");
			Register("hxc", "application/xml");
			Register("hxd", "application/octet-stream");
			Register("hxe", "application/xml");
			Register("hxf", "application/xml");
			Register("hxh", "application/octet-stream");
			Register("hxi", "application/octet-stream");
			Register("hxk", "application/xml");
			Register("hxq", "application/octet-stream");
			Register("hxr", "application/octet-stream");
			Register("hxs", "application/octet-stream");
			Register("hxt", "text/html");
			Register("hxv", "application/xml");
			Register("hxw", "application/octet-stream");
			Register("hxx", "text/plain");
			Register("i", "text/plain");
			Register("ico", "image/x-icon");
			Register("ics", "application/octet-stream");
			Register("idl", "text/plain");
			Register("ief", "image/ief");
			Register("iii", "application/x-iphone");
			Register("inc", "text/plain");
			Register("inf", "application/octet-stream");
			Register("inl", "text/plain");
			Register("ins", "application/x-internet-signup");
			Register("ipa", "application/x-itunes-ipa");
			Register("ipg", "application/x-itunes-ipg");
			Register("ipproj", "text/plain");
			Register("ipsw", "application/x-itunes-ipsw");
			Register("iqy", "text/x-ms-iqy");
			Register("isp", "application/x-internet-signup");
			Register("ite", "application/x-itunes-ite");
			Register("itlp", "application/x-itunes-itlp");
			Register("itms", "application/x-itunes-itms");
			Register("itpc", "application/x-itunes-itpc");
			Register("IVF", "video/x-ivf");
			Register("jar", "application/java-archive");
			Register("java", "application/octet-stream");
			Register("jck", "application/liquidmotion");
			Register("jcz", "application/liquidmotion");
			Register("jfif", "image/pjpeg");
			Register("jnlp", "application/x-java-jnlp-file");
			Register("jpb", "application/octet-stream");
			Register("jpe", "image/jpeg");
			Register("jpeg", "image/jpeg");
			Register("jpg", "image/jpeg");
			Register("js", "application/x-javascript");
			Register("json", "application/json");
			Register("jsx", "text/jscript");
			Register("jsxbin", "text/plain");
			Register("latex", "application/x-latex");
			Register("library-ms", "application/windows-library+xml");
			Register("lit", "application/x-ms-reader");
			Register("loadtest", "application/xml");
			Register("lpk", "application/octet-stream");
			Register("lsf", "video/x-la-asf");
			Register("lst", "text/plain");
			Register("lsx", "video/x-la-asf");
			Register("lzh", "application/octet-stream");
			Register("m13", "application/x-msmediaview");
			Register("m14", "application/x-msmediaview");
			Register("m1v", "video/mpeg");
			Register("m2t", "video/vnd.dlna.mpeg-tts");
			Register("m2ts", "video/vnd.dlna.mpeg-tts");
			Register("m2v", "video/mpeg");
			Register("m3u", "audio/x-mpegurl");
			Register("m3u8", "audio/x-mpegurl");
			Register("m4a", "audio/m4a");
			Register("m4b", "audio/m4b");
			Register("m4p", "audio/m4p");
			Register("m4r", "audio/x-m4r");
			Register("m4v", "video/x-m4v");
			Register("mac", "image/x-macpaint");
			Register("mak", "text/plain");
			Register("man", "application/x-troff-man");
			Register("manifest", "application/x-ms-manifest");
			Register("map", "text/plain");
			Register("master", "application/xml");
			Register("mda", "application/msaccess");
			Register("mdb", "application/x-msaccess");
			Register("mde", "application/msaccess");
			Register("mdp", "application/octet-stream");
			Register("me", "application/x-troff-me");
			Register("mfp", "application/x-shockwave-flash");
			Register("mht", "message/rfc822");
			Register("mhtml", "message/rfc822");
			Register("mid", "audio/mid");
			Register("midi", "audio/mid");
			Register("mix", "application/octet-stream");
			Register("mk", "text/plain");
			Register("mmf", "application/x-smaf");
			Register("mno", "text/xml");
			Register("mny", "application/x-msmoney");
			Register("mod", "video/mpeg");
			Register("mov", "video/quicktime");
			Register("movie", "video/x-sgi-movie");
			Register("mp2", "video/mpeg");
			Register("mp2v", "video/mpeg");
			Register("mp3", "audio/mpeg");
			Register("mp4", "video/mp4");
			Register("mp4v", "video/mp4");
			Register("mpa", "video/mpeg");
			Register("mpe", "video/mpeg");
			Register("mpeg", "video/mpeg");
			Register("mpf", "application/vnd.ms-mediapackage");
			Register("mpg", "video/mpeg");
			Register("mpp", "application/vnd.ms-project");
			Register("mpv2", "video/mpeg");
			Register("mqv", "video/quicktime");
			Register("ms", "application/x-troff-ms");
			Register("msi", "application/octet-stream");
			Register("mso", "application/octet-stream");
			Register("mts", "video/vnd.dlna.mpeg-tts");
			Register("mtx", "application/xml");
			Register("mul", "application/octet-stream");
			Register("mvb", "application/x-msmediaview");
			Register("mvc", "application/x-miva-compiled");
			Register("mxp", "application/x-mmxp");
			Register("nc", "application/x-netcdf");
			Register("nsc", "video/x-ms-asf");
			Register("nws", "message/rfc822");
			Register("ocx", "application/octet-stream");
			Register("oda", "application/oda");
			Register("odc", "text/x-ms-odc");
			Register("odh", "text/plain");
			Register("odl", "text/plain");
			Register("odp", "application/vnd.oasis.opendocument.presentation");
			Register("ods", "application/oleobject");
			Register("odt", "application/vnd.oasis.opendocument.text");
			Register("one", "application/onenote");
			Register("onea", "application/onenote");
			Register("onepkg", "application/onenote");
			Register("onetmp", "application/onenote");
			Register("onetoc", "application/onenote");
			Register("onetoc2", "application/onenote");
			Register("orderedtest", "application/xml");
			Register("osdx", "application/opensearchdescription+xml");
			Register("p10", "application/pkcs10");
			Register("p12", "application/x-pkcs12");
			Register("p7b", "application/x-pkcs7-certificates");
			Register("p7c", "application/pkcs7-mime");
			Register("p7m", "application/pkcs7-mime");
			Register("p7r", "application/x-pkcs7-certreqresp");
			Register("p7s", "application/pkcs7-signature");
			Register("pbm", "image/x-portable-bitmap");
			Register("pcast", "application/x-podcast");
			Register("pct", "image/pict");
			Register("pcx", "application/octet-stream");
			Register("pcz", "application/octet-stream");
			Register("pdf", "application/pdf");
			Register("pfb", "application/octet-stream");
			Register("pfm", "application/octet-stream");
			Register("pfx", "application/x-pkcs12");
			Register("pgm", "image/x-portable-graymap");
			Register("pic", "image/pict");
			Register("pict", "image/pict");
			Register("pkgdef", "text/plain");
			Register("pkgundef", "text/plain");
			Register("pko", "application/vnd.ms-pki.pko");
			Register("pls", "audio/scpls");
			Register("pma", "application/x-perfmon");
			Register("pmc", "application/x-perfmon");
			Register("pml", "application/x-perfmon");
			Register("pmr", "application/x-perfmon");
			Register("pmw", "application/x-perfmon");
			Register("png", "image/png");
			Register("pnm", "image/x-portable-anymap");
			Register("pnt", "image/x-macpaint");
			Register("pntg", "image/x-macpaint");
			Register("pnz", "image/png");
			Register("post", "application/x-www-form-urlencoded");
			Register("pot", "application/vnd.ms-powerpoint");
			Register("potm", "application/vnd.ms-powerpoint.template.macroEnabled.12");
			Register("potx", "application/vnd.openxmlformats-officedocument.presentationml.template");
			Register("ppa", "application/vnd.ms-powerpoint");
			Register("ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12");
			Register("ppm", "image/x-portable-pixmap");
			Register("pps", "application/vnd.ms-powerpoint");
			Register("ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12");
			Register("ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow");
			Register("ppt", "application/vnd.ms-powerpoint");
			Register("pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12");
			Register("pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
			Register("prf", "application/pics-rules");
			Register("prm", "application/octet-stream");
			Register("prx", "application/octet-stream");
			Register("ps", "application/postscript");
			Register("psc1", "application/PowerShell");
			Register("psd", "application/octet-stream");
			Register("psess", "application/xml");
			Register("psm", "application/octet-stream");
			Register("psp", "application/octet-stream");
			Register("pub", "application/x-mspublisher");
			Register("pwz", "application/vnd.ms-powerpoint");
			Register("qht", "text/x-html-insertion");
			Register("qhtm", "text/x-html-insertion");
			Register("qt", "video/quicktime");
			Register("qti", "image/x-quicktime");
			Register("qtif", "image/x-quicktime");
			Register("qtl", "application/x-quicktimeplayer");
			Register("qxd", "application/octet-stream");
			Register("ra", "audio/x-pn-realaudio");
			Register("ram", "audio/x-pn-realaudio");
			Register("rar", "application/octet-stream");
			Register("ras", "image/x-cmu-raster");
			Register("rat", "application/rat-file");
			Register("rc", "text/plain");
			Register("rc2", "text/plain");
			Register("rct", "text/plain");
			Register("rdlc", "application/xml");
			Register("resx", "application/xml");
			Register("rf", "image/vnd.rn-realflash");
			Register("rgb", "image/x-rgb");
			Register("rgs", "text/plain");
			Register("rm", "application/vnd.rn-realmedia");
			Register("rmi", "audio/mid");
			Register("rmp", "application/vnd.rn-rn_music_package");
			Register("roff", "application/x-troff");
			Register("rpm", "audio/x-pn-realaudio-plugin");
			Register("rqy", "text/x-ms-rqy");
			Register("rtf", "application/rtf");
			Register("rtx", "text/richtext");
			Register("ruleset", "application/xml");
			Register("s", "text/plain");
			Register("safariextz", "application/x-safari-safariextz");
			Register("scd", "application/x-msschedule");
			Register("sct", "text/scriptlet");
			Register("sd2", "audio/x-sd2");
			Register("sdp", "application/sdp");
			Register("sea", "application/octet-stream");
			Register("searchConnector-ms", "application/windows-search-connector+xml");
			Register("setpay", "application/set-payment-initiation");
			Register("setreg", "application/set-registration-initiation");
			Register("settings", "application/xml");
			Register("sgimb", "application/x-sgimb");
			Register("sgml", "text/sgml");
			Register("sh", "application/x-sh");
			Register("shar", "application/x-shar");
			Register("shtml", "text/html");
			Register("sit", "application/x-stuffit");
			Register("sitemap", "application/xml");
			Register("skin", "application/xml");
			Register("sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12");
			Register("sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide");
			Register("slk", "application/vnd.ms-excel");
			Register("sln", "text/plain");
			Register("slupkg-ms", "application/x-ms-license");
			Register("smd", "audio/x-smd");
			Register("smi", "application/octet-stream");
			Register("smx", "audio/x-smd");
			Register("smz", "audio/x-smd");
			Register("snd", "audio/basic");
			Register("snippet", "application/xml");
			Register("snp", "application/octet-stream");
			Register("sol", "text/plain");
			Register("sor", "text/plain");
			Register("spc", "application/x-pkcs7-certificates");
			Register("spl", "application/futuresplash");
			Register("src", "application/x-wais-source");
			Register("srf", "text/plain");
			Register("SSISDeploymentManifest", "text/xml");
			Register("ssm", "application/streamingmedia");
			Register("sst", "application/vnd.ms-pki.certstore");
			Register("stl", "application/vnd.ms-pki.stl");
			Register("sv4cpio", "application/x-sv4cpio");
			Register("sv4crc", "application/x-sv4crc");
			Register("svc", "application/xml");
			Register("swf", "application/x-shockwave-flash");
			Register("t", "application/x-troff");
			Register("tar", "application/x-tar");
			Register("tcl", "application/x-tcl");
			Register("testrunconfig", "application/xml");
			Register("testsettings", "application/xml");
			Register("tex", "application/x-tex");
			Register("texi", "application/x-texinfo");
			Register("texinfo", "application/x-texinfo");
			Register("tgz", "application/x-compressed");
			Register("thmx", "application/vnd.ms-officetheme");
			Register("thn", "application/octet-stream");
			Register("tif", "image/tiff");
			Register("tiff", "image/tiff");
			Register("tlh", "text/plain");
			Register("tli", "text/plain");
			Register("toc", "application/octet-stream");
			Register("tr", "application/x-troff");
			Register("trm", "application/x-msterminal");
			Register("trx", "application/xml");
			Register("ts", "video/vnd.dlna.mpeg-tts");
			Register("tsv", "text/tab-separated-values");
			Register("ttf", "application/octet-stream");
			Register("tts", "video/vnd.dlna.mpeg-tts");
			Register("txt", "text/plain");
			Register("u32", "application/octet-stream");
			Register("uls", "text/iuls");
			Register("uoo", "application/octet-stream");
			Register("uop", "application/octet-stream");
			Register("user", "text/plain");
			Register("ustar", "application/x-ustar");
			Register("vb", "text/plain");
			Register("vbdproj", "text/plain");
			Register("vbk", "video/mpeg");
			Register("vbproj", "text/plain");
			Register("vbs", "text/vbscript");
			Register("vcf", "text/x-vcard");
			Register("vcproj", "application/xml");
			Register("vcs", "text/plain");
			Register("vcxproj", "application/xml");
			Register("vddproj", "text/plain");
			Register("vdp", "text/plain");
			Register("vdproj", "text/plain");
			Register("vdx", "application/vnd.ms-visio.viewer");
			Register("vml", "text/xml");
			Register("vscontent", "application/xml");
			Register("vsct", "text/xml");
			Register("vsd", "application/vnd.visio");
			Register("vsi", "application/ms-vsi");
			Register("vsix", "application/vsix");
			Register("vsixlangpack", "text/xml");
			Register("vsixmanifest", "text/xml");
			Register("vsmdi", "application/xml");
			Register("vspscc", "text/plain");
			Register("vss", "application/vnd.visio");
			Register("vsscc", "text/plain");
			Register("vssettings", "text/xml");
			Register("vssscc", "text/plain");
			Register("vst", "application/vnd.visio");
			Register("vstemplate", "text/xml");
			Register("vsto", "application/x-ms-vsto");
			Register("vsw", "application/vnd.visio");
			Register("vsx", "application/vnd.visio");
			Register("vtx", "application/vnd.visio");
			Register("wav", "audio/wav");
			Register("wave", "audio/wav");
			Register("wax", "audio/x-ms-wax");
			Register("wbk", "application/msword");
			Register("wbmp", "image/vnd.wap.wbmp");
			Register("wcm", "application/vnd.ms-works");
			Register("wdb", "application/vnd.ms-works");
			Register("wdp", "image/vnd.ms-photo");
			Register("webarchive", "application/x-safari-webarchive");
			Register("webtest", "application/xml");
			Register("wiq", "application/xml");
			Register("wiz", "application/msword");
			Register("wks", "application/vnd.ms-works");
			Register("WLMP", "application/wlmoviemaker");
			Register("wlpginstall", "application/x-wlpg-detect");
			Register("wlpginstall3", "application/x-wlpg3-detect");
			Register("wm", "video/x-ms-wm");
			Register("wma", "audio/x-ms-wma");
			Register("wmd", "application/x-ms-wmd");
			Register("wmf", "application/x-msmetafile");
			Register("wml", "text/vnd.wap.wml");
			Register("wmlc", "application/vnd.wap.wmlc");
			Register("wmls", "text/vnd.wap.wmlscript");
			Register("wmlsc", "application/vnd.wap.wmlscriptc");
			Register("wmp", "video/x-ms-wmp");
			Register("wmv", "video/x-ms-wmv");
			Register("wmx", "video/x-ms-wmx");
			Register("wmz", "application/x-ms-wmz");
			Register("wpl", "application/vnd.ms-wpl");
			Register("wps", "application/vnd.ms-works");
			Register("wri", "application/x-mswrite");
			Register("wrl", "x-world/x-vrml");
			Register("wrz", "x-world/x-vrml");
			Register("wsc", "text/scriptlet");
			Register("wsdl", "text/xml");
			Register("wvx", "video/x-ms-wvx");
			Register("x", "application/directx");
			Register("xaf", "x-world/x-vrml");
			Register("xaml", "application/xaml+xml");
			Register("xap", "application/x-silverlight-app");
			Register("xbap", "application/x-ms-xbap");
			Register("xbm", "image/x-xbitmap");
			Register("xdr", "text/plain");
			Register("xht", "application/xhtml+xml");
			Register("xhtml", "application/xhtml+xml");
			Register("xla", "application/vnd.ms-excel");
			Register("xlam", "application/vnd.ms-excel.addin.macroEnabled.12");
			Register("xlc", "application/vnd.ms-excel");
			Register("xld", "application/vnd.ms-excel");
			Register("xlk", "application/vnd.ms-excel");
			Register("xll", "application/vnd.ms-excel");
			Register("xlm", "application/vnd.ms-excel");
			Register("xls", "application/vnd.ms-excel");
			Register("xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12");
			Register("xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12");
			Register("xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			Register("xlt", "application/vnd.ms-excel");
			Register("xltm", "application/vnd.ms-excel.template.macroEnabled.12");
			Register("xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template");
			Register("xlw", "application/vnd.ms-excel");
			Register("xml", "text/xml");
			Register("xmta", "application/xml");
			Register("xof", "x-world/x-vrml");
			Register("XOML", "text/plain");
			Register("xpm", "image/x-xpixmap");
			Register("xps", "application/vnd.ms-xpsdocument");
			Register("xrm-ms", "text/xml");
			Register("xsc", "application/xml");
			Register("xsd", "text/xml");
			Register("xsf", "text/xml");
			Register("xsl", "text/xml");
			Register("xslt", "text/xml");
			Register("xsn", "application/octet-stream");
			Register("xss", "application/xml");
			Register("xtp", "application/octet-stream");
			Register("xwd", "image/x-xwindowdump");
			Register("z", "application/x-compress");
			Register("zip", "application/x-zip-compressed");
			#endregion
		}

		private static readonly string[] _TextTypes =
		{
			"text/", "/xml", "+xml", "/html", "+html", "/javascript", "/x-javascript", "+x-javascript", "/json", "+json",
			"/vbscript", "+vbscript", "/css", "+css"
		};

		private static readonly string[] _ImageTypes =
		{
			"image/", "/bmp", "/png", "/jpeg", "/pjpeg", "/tiff", "/pict", "/x-icon", "/x-portable-anymap", "/x-portable-pixmap",
			"/x-portable-bitmap", "/x-xpixmap", "/x-xbitmap", "/x-xwindowdump", "/x-macpaint"
		};

		public static bool IsCommonText(FileMime mime)
		{
			return mime.MimeType.ContainsAny(true, _TextTypes);
		}

		public static bool IsCommonImage(FileMime mime)
		{
			return mime.MimeType.ContainsAny(true, _ImageTypes);
		}

		public static bool Exists(string ext)
		{
			if (ext != null)
			{
				ext = ext.Trim('.');
				ext = ext.Trim();

				return _Registry.ContainsKey(ext);
			}

			return false;
		}

		public static void Register(string ext, string type)
		{
			if (ext != null)
			{
				ext = ext.Trim('.');
				ext = ext.Trim();

				_Registry[ext] = new FileMime(ext, type);
			}
		}

		public static bool Unregister(string ext)
		{
			if (ext == null)
			{
				return false;
			}

			ext = ext.Trim('.');
			ext = ext.Trim();

			return _Registry.Remove(ext);
		}

		public static FileMime ReverseLookup(string type)
		{
			ReverseLookup(type, out var mime);

			return mime;
		}

		public static bool ReverseLookup(string type, out FileMime mime)
		{
			type = type ?? String.Empty;
			type = type.Trim();

			if (type.Contains('/'))
			{
				var i = type.IndexOf(' ');

				if (i > -1)
				{
					foreach (var t in type.Split(' ').Where(s => s.Contains('/')))
					{
						if (ReverseLookup(t, out mime))
						{
							return true;
						}
					}
				}
				else
				{
					type = type.Trim();
					type = type.TrimEnd(';');

					foreach (var m in _Registry.Values.Where(m => Insensitive.Equals(m.MimeType, type)))
					{
						mime = m;
						return true;
					}
				}
			}

			mime = Default;
			return false;
		}

		public static FileMime Lookup(FileInfo file)
		{
			return Lookup(file.Extension);
		}

		public static FileMime Lookup(string path)
		{
			Lookup(path, out var mime);

			return mime;
		}

		public static bool Lookup(FileInfo file, out FileMime mime)
		{
			return Lookup(file.Extension, out mime);
		}

		public static bool Lookup(string path, out FileMime mime)
		{
			path = path ?? String.Empty;
			path = path.Trim();

			var ext = path;

			var i = ext.LastIndexOf('.');

			if (i > -1)
			{
				ext = ext.Substring(i + 1);
				ext = ext.Trim();
			}
			else
			{
				i = ext.IndexOf(' ');

				if (i > -1)
				{
					ext = ext.Substring(0, i);
				}
			}

			if (_Registry.TryGetValue(ext, out mime))
			{
				return true;
			}

			i = path.IndexOf(' ');

			if (i > -1)
			{
				path = path.Substring(0, i);
			}

			if (ReverseLookup(path, out mime))
			{
				return true;
			}

			mime = Default;
			return false;
		}

		private readonly string _Extension;
		private readonly string _MimeType;

		public string Extension => _Extension;
		public string MimeType => _MimeType;

		public bool IsDefault => _Extension == "*";

		private FileMime(string ext, string type)
		{
			_Extension = ext ?? "*";
			_MimeType = type ?? "application/octet-stream";
		}

		public bool IsCommonText()
		{
			return IsCommonText(this);
		}

		public bool IsCommonImage()
		{
			return IsCommonImage(this);
		}

		public bool IsMatch(FileInfo file)
		{
			return Lookup(file) == this;
		}

		public bool IsMatch(string path)
		{
			return Lookup(path) == this;
		}

		public override string ToString()
		{
			return MimeType;
		}

		public override int GetHashCode()
		{
			return Extension.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is FileMime && Equals((FileMime)obj);
		}

		public bool Equals(FileMime mime)
		{
			return Insensitive.Equals(Extension, mime.Extension);
		}

		public int CompareTo(FileMime mime)
		{
			return Insensitive.Compare(Extension, mime.Extension);
		}

		public static bool operator ==(FileMime l, FileMime r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(FileMime l, FileMime r)
		{
			return !l.Equals(r);
		}

		public static bool operator ==(FileMime l, string r)
		{
			return l.Equals(Lookup(r));
		}

		public static bool operator !=(FileMime l, string r)
		{
			return !l.Equals(Lookup(r));
		}

		public static bool operator ==(string l, FileMime r)
		{
			return Lookup(l).Equals(r);
		}

		public static bool operator !=(string l, FileMime r)
		{
			return !Lookup(l).Equals(r);
		}

		public static implicit operator string(FileMime mime)
		{
			return mime.MimeType;
		}

		public static implicit operator FileMime(string path)
		{
			return Lookup(path);
		}

		public static implicit operator FileMime(FileInfo file)
		{
			return Lookup(file);
		}
	}
}