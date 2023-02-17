using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScrumPoker.StandaloneServer
{
    public class UserSession
    {
        [Obsolete("Use HttpListenerContext.User")]
        public const string CookieName_AuthToken = "AuthToken";
        private readonly HttpListenerResponse _response;
        public HttpListenerResponse Response { get { return _response; } }

        private readonly ApplicationSession _appSession;
        public ApplicationSession AppSession { get { return _appSession; } }
        
        private readonly ScrumPoker.DataContracts.TeamMember _user;
        public ScrumPoker.DataContracts.TeamMember User { get { return _user; } }
        
        private readonly string _localPath;
        public string LocalPath { get { return _localPath; } }
        
        private readonly string _httpMethod;
        public string HttpMethod { get { return _httpMethod; } }
        
        private readonly NameValueCollection _query = new NameValueCollection(ApplicationSession.DefaultComparer);
        public NameValueCollection Query { get { return _query; } }
        
        private readonly string _fragment;
        public string Fragment { get { return _fragment; } }
        
        private readonly NameValueCollection _formData = new NameValueCollection(ApplicationSession.DefaultComparer);
        public NameValueCollection FormData { get { return _formData; } }

        private static void DecodeUriFormData(string uriEncodedData, NameValueCollection target)
        {
            if (uriEncodedData != null && (uriEncodedData = uriEncodedData.Trim()).Length > 0)
                foreach (string pair in uriEncodedData.Split('&'))
                {
                    int i = pair.IndexOf('=');
                    if (i < 0)
                        target.Add(pair, null);
                    else
                        target.Add(pair.Substring(0, i), pair.Substring(i + 1));
                }
        }

        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

        public bool TryGetFileInfo(DirectoryInfo webRootDirectory, out FileInfo result)
        {
            if (webRootDirectory == null || !webRootDirectory.Exists || _localPath.Length < 2)
                result = null;
            else
            {
                string[] segments = _localPath.Split('/').Where(s => s.Length > 0).ToArray();
                if (!segments.Any(s => s == "." || s == ".." || InvalidFileNameChars.Any(c => s.Contains(c))))
                    try
                    {
                        string fullPath = webRootDirectory + new string(new char[] { Path.DirectorySeparatorChar });
                        string rootPath = fullPath;
                        foreach (string s in segments)
                            if (!(fullPath = Path.Combine(fullPath, s)).StartsWith(rootPath))
                            {
                                result = null;
                                return false;
                            }
                        return (result = new FileInfo(fullPath)).Exists;
                    }
                    catch { result = null; }
                else
                    result = null;
            }
            return false;
        }
        
        public UserSession(ApplicationSession appSession, HttpListenerContext context)
        {
            _response = context.Response;
            _appSession = appSession;
            HttpListenerRequest request = context.Request;
            IPrincipal user = context.User;
            if (user != null)
            {
                IIdentity identity = user.Identity;
                if (identity != null && identity.IsAuthenticated)
                    _user = appSession.SessionData.ScrumPokerUsers.FirstOrDefault(d => string.Equals(d.UserName, identity.Name, StringComparison.InvariantCultureIgnoreCase));
            }
            Uri url = request.Url;
            string s = url.LocalPath;
            _localPath = (s.EndsWith("/") && s.Length > 1) ? s.Substring(0, s.Length - 1) : s;
            _httpMethod = request.HttpMethod;
            DecodeUriFormData((s = url.Query).StartsWith("?") ? s.Substring(1) : s, _query);
            _fragment = (s = url.Fragment).StartsWith("#") ? s.Substring(1) : s;
            if (ApplicationSession.DefaultComparer.Equals(_httpMethod, "GET"))
                try
                {
                    using (StreamReader reader = new StreamReader(request.InputStream, ApplicationSession.DefaultEncoding, true))
                        DecodeUriFormData(reader.ReadToEnd(), _formData);
                }
                catch { /* okay to ignore */ }
        }

        public static string ToStatusDescription(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.BadGateway:
                    return "Bad Gateway";
                case HttpStatusCode.BadRequest:
                    return "Bad Request";
                case HttpStatusCode.ExpectationFailed:
                    return "Expectation Failed";
                case HttpStatusCode.GatewayTimeout:
                    return "Gateway Timeout";
                case HttpStatusCode.HttpVersionNotSupported:
                    return "Http Version Not Supported";
                case HttpStatusCode.InternalServerError:
                    return "Internal ServerError";
                case HttpStatusCode.LengthRequired:
                    return "Length Required";
                case HttpStatusCode.MethodNotAllowed:
                    return "Method Not Allowed";
                case HttpStatusCode.NoContent:
                    return "No Content";
                case HttpStatusCode.NonAuthoritativeInformation:
                    return "Non-Authoritative Information";
                case HttpStatusCode.NotAcceptable:
                    return "Not Acceptable";
                case HttpStatusCode.NotFound:
                    return "Not Found";
                case HttpStatusCode.NotImplemented:
                    return "Not Implemented";
                case HttpStatusCode.NotModified:
                    return "Not Modified";
                case HttpStatusCode.PartialContent:
                    return "Partial Content";
                case HttpStatusCode.PaymentRequired:
                    return "Payment Required";
                case HttpStatusCode.PreconditionFailed:
                    return "Precondition Failed";
                case HttpStatusCode.ProxyAuthenticationRequired:
                    return "Proxy Authentication Required";
                case HttpStatusCode.RedirectKeepVerb:
                    return "Redirect Keep Verb";
                case HttpStatusCode.RedirectMethod:
                    return "Redirect Method";
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                    return "Requested Range Not Satisfiable";
                case HttpStatusCode.RequestEntityTooLarge:
                    return "Request Entity Too Large";
                case HttpStatusCode.RequestTimeout:
                    return "Request Timeout";
                case HttpStatusCode.RequestUriTooLong:
                    return "Request Uri Too Long";
                case HttpStatusCode.ResetContent:
                    return "Reset Content";
                case HttpStatusCode.ServiceUnavailable:
                    return "Service Unavailable";
                case HttpStatusCode.SwitchingProtocols:
                    return "Switching Protocols";
                case HttpStatusCode.UnsupportedMediaType:
                    return "Unsupported Media Type";
                case HttpStatusCode.UpgradeRequired:
                    return "Upgrade Required";
                case HttpStatusCode.UseProxy:
                    return "Use Proxy";
                default:
                    return statusCode.ToString("F");
            }
        }
        
        public void SendPlainText(string content, HttpStatusCode statusCode = HttpStatusCode.OK, string statusDescription = null)
        {
            _response.StatusCode = (int)statusCode;
            _response.StatusDescription = (statusDescription != null && (statusDescription = statusDescription.Trim()).Length > 0) ? statusDescription : ToStatusDescription(statusCode);
            byte[] bytes = ApplicationSession.DefaultEncoding.GetBytes(content);
            _response.ContentType = MediaTypeNames.Text.Plain;
            _response.ContentLength64 = bytes.Length;
            _response.ContentEncoding = ApplicationSession.DefaultEncoding;
            Stream outputStream = _response.OutputStream;
            outputStream.Write(bytes, 0, bytes.Length);
            outputStream.Close();
        }

        public static XDocument ToXHtmlDocumentWithTitle(string title, params object[] bodyContent)
        {
            string titleText = "Scrum Poker";
            if (title != null && (title = title.Trim()).Length > 0)
                titleText += " - " + title;
            return new XDocument(new XhtmlElementBuilder("html",
                new XhtmlElementBuilder("head",
                    new XhtmlElementBuilder("meta").SetAttribute("charset", ApplicationSession.DefaultEncoding.WebName),
                    new XhtmlElementBuilder("meta").SetAttribute("http-equiv", "X-UA-Compatible").SetAttribute("content", "IE=edge"),
                    new XhtmlElementBuilder("title", titleText),
                    new XhtmlElementBuilder("base").SetAttribute("href", "/"),
                    new XhtmlElementBuilder("meta").SetAttribute("name", "viewport").SetAttribute("content", "width=device-width, initial-scale=1.0"),
                    new XhtmlElementBuilder("link").SetAttribute("rel", "icon").SetAttribute("type", "image/x-icon").SetAttribute("href", "favicon.ico")
                ),
                new XhtmlElementBuilder("body", bodyContent)
            ).SetAttribute("lang", "en").Element);
        }

        public static XDocument ToXHtmlDocument(params object[] bodyContent) { return ToXHtmlDocumentWithTitle(null, bodyContent); }

        public void SendHtml(XDocument content, HttpStatusCode statusCode = HttpStatusCode.OK, string statusDescription = null)
        {
            _response.StatusCode = (int)statusCode;
            _response.StatusDescription = (statusDescription != null && (statusDescription = statusDescription.Trim()).Length > 0) ? statusDescription : ToStatusDescription(statusCode);
            byte[] bytes = ApplicationSession.DefaultEncoding.GetBytes(content.ToString(SaveOptions.None));
            _response.ContentType = MediaTypeNames.Text.Html;
            _response.ContentLength64 = bytes.Length;
            _response.ContentEncoding = ApplicationSession.DefaultEncoding;
            Stream outputStream = _response.OutputStream;
            outputStream.Write(bytes, 0, bytes.Length);
            outputStream.Close();
        }

        public void SendNotFoundResponse(string url)
        {
            SendHtml(ToXHtmlDocumentWithTitle("Resource not found", new XhtmlElementBuilder("h1", "Resource not found"), url + " was not found."), HttpStatusCode.NotFound);
        }

        public void SendRedirect(string url)
        {
            string absUrl = new Uri(_appSession.BaseUrl, url).AbsoluteUri;
            byte[] bytes = ApplicationSession.DefaultEncoding.GetBytes(ToXHtmlDocumentWithTitle("Redirect",
                new XhtmlElementBuilder("h1", "Redirect"),
                new XhtmlElementBuilder("a", url).SetAttribute("href", absUrl)
            ).ToString(SaveOptions.None));
            _response.ContentType = MediaTypeNames.Text.Html;
            _response.ContentLength64 = bytes.Length;
            _response.ContentEncoding = ApplicationSession.DefaultEncoding;
            Stream outputStream = _response.OutputStream;
            outputStream.Write(bytes, 0, bytes.Length);
            outputStream.Close();   
            _response.Redirect(absUrl);
        }
    }
}