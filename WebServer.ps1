Param(
    [int]$PortNumber = 8080,
    [string]$AdminUserName = 'admin'
)

<#
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
#>
Add-Type -TypeDefinition @'
namespace WebServer
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mime;
    using System.Security.Cryptography;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    public class ApplicationSession
    {
        public const int AUTH_TOKEN_LENGTH = 64;
        public const int COMBINED_TOKEN_LENGTH = 128;
        public static readonly StringComparer DefaultComparer = StringComparer.InvariantCultureIgnoreCase;
        public static readonly Encoding DefaultEncoding = new UTF8Encoding(false, false);
        private readonly RNGCryptoServiceProvider _csp = new RNGCryptoServiceProvider();
        private readonly byte[] _authToken = new byte[AUTH_TOKEN_LENGTH];

        private readonly Uri _baseUrl;
        public Uri BaseUrl { get { return _baseUrl; } }

        private readonly User _adminUser;
        public User AdminUser { get { return _adminUser; } }

        private readonly Collection<User> _backingUsers = new Collection<User>();
        private readonly ReadOnlyCollection<User> _users;
        public ReadOnlyCollection<User> Users { get { return _users; } }

        public ApplicationSession(int portNumber, string userName, string displayName = null)
        {
            UriBuilder ub = new UriBuilder("http://localhost");
            ub.Port = portNumber;
            _baseUrl = ub.Uri;
            _users = new ReadOnlyCollection<User>(_backingUsers);
            _csp.GetBytes(_authToken);
            User adminUser;
            User.TryAdd(this, userName, displayName, out adminUser);
            _adminUser = adminUser;
        }

        public class User
        {
            private readonly byte[] _authToken = new byte[AUTH_TOKEN_LENGTH];

            private readonly string _userName;
            public string UserName { get { return _userName; } }

            private string _displayName;
            public string DisplayName
            {
                get { return _displayName; }
                set
                {
                    string displayName = value;
                    _displayName = (displayName != null && (displayName = displayName.Trim()).Length > 0) ? displayName : _userName;
                }
            }
            
            private User(string userName, string displayName)
            {
                _userName = userName;
                _displayName = (displayName != null && (displayName = displayName.Trim()).Length > 0) ? displayName : userName;
            }

            public string GetTokenString(ApplicationSession appSession)
            {
                if (appSession == null)
                    throw new ArgumentNullException("appSession");
                byte[] combined = new byte[COMBINED_TOKEN_LENGTH];
                Array.Copy(_authToken, combined, AUTH_TOKEN_LENGTH);
                Array.Copy(appSession._authToken, 0, combined, AUTH_TOKEN_LENGTH, AUTH_TOKEN_LENGTH);
                return _userName + ":" + Convert.ToBase64String(ProtectedData.Protect(combined, DefaultEncoding.GetBytes(_userName), DataProtectionScope.CurrentUser));
            }

            public static bool TryFindFromTokenString(ApplicationSession appSession, string tokenString, out User user)
            {
                if (tokenString != null && (tokenString = tokenString.Trim()).Length > 0)
                {
                    int i = tokenString.IndexOf(':');
                    if (i > 0 && i < tokenString.Length - 1)
                    {
                        try
                        {
                            string userName = tokenString.Substring(0, i);
                            byte[] combined = ProtectedData.Unprotect(Convert.FromBase64String(tokenString.Substring(i + 1)), DefaultEncoding.GetBytes(userName), DataProtectionScope.CurrentUser);
                            if (combined != null && combined.Length == COMBINED_TOKEN_LENGTH)
                            {
                                if ((user = appSession._users.FirstOrDefault(u => DefaultComparer.Equals(userName, u._userName))) != null &&
                                        combined.Take(AUTH_TOKEN_LENGTH).SequenceEqual(user._authToken) && combined.Skip(AUTH_TOKEN_LENGTH).SequenceEqual(appSession._authToken))
                                    return true;
                            }
                        }
                        catch { /* okay to ignore */ }
                    }
                }
                user = null;
                return false;
            }

            public static bool TryAdd(ApplicationSession appSession, string userName, out User result) { return TryAdd(appSession, userName, null, out result); }

            public static bool TryAdd(ApplicationSession appSession, string userName, string displayName, out User result)
            {
                if (appSession == null || userName == null || (userName = userName.Trim()).Length == 0)
                {
                    result = null;
                    return false;
                }
                lock (appSession._backingUsers)
                {
                    if (appSession._backingUsers.Any(u => DefaultComparer.Equals(userName, u._userName)))
                    {
                        result = null;
                        return false;
                    }
                    result = new User(userName, displayName);
                    appSession._csp.GetBytes(result._authToken);
                    appSession._backingUsers.Add(result);
                }
                return true;
            }
        }
    }

    public class UserSession
    {
        public const string CookieName_AuthToken = "AuthToken";
        private readonly HttpListenerResponse _response;
        public HttpListenerResponse Response { get { return _response; } }

        private readonly ApplicationSession _appSession;
        public ApplicationSession AppSession { get { return _appSession; } }
        
        private readonly ApplicationSession.User _user;
        public ApplicationSession.User User { get { return _user; } }
        
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

        public UserSession(ApplicationSession appSession, HttpListenerContext context)
        {
            _response = context.Response;
            _appSession = appSession;
            HttpListenerRequest request = context.Request;
            Cookie cookie = request.Cookies[CookieName_AuthToken];
            ApplicationSession.User user;
            if (cookie != null && ApplicationSession.User.TryFindFromTokenString(appSession, cookie.Value, out user))
            {
                _user = user;
                _response.SetCookie(new Cookie(CookieName_AuthToken, cookie.Value));
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

    public class XhtmlElementBuilder
    {
        private readonly XElement _element;
        public XElement Element { get { return _element; } }

        private XmlDateTimeSerializationMode _dateTimeOption = XmlDateTimeSerializationMode.RoundtripKind;
        public XmlDateTimeSerializationMode DateTimeOption
        {
            get { return _dateTimeOption; }
            set { _dateTimeOption = value; }
        }
        

        public XhtmlElementBuilder(string localName, params object[] content)
        {
            _element = new XElement(XNamespace.None.GetName(localName));
            Add(content);
        }

        public XhtmlElementBuilder WithDateTimeOption(XmlDateTimeSerializationMode dateTimeOption)
        {
            _dateTimeOption = dateTimeOption;
            return this;
        }

        private string ToStringValue(object obj)
        {
            if (obj == null)
                return "";
            if (obj is string)
                return obj as string;
            if (obj is char)
                return XmlConvert.ToString((char)obj);
            if (obj is bool)
                return XmlConvert.ToString((bool)obj);
            if (obj is byte)
                return XmlConvert.ToString((byte)obj);
            if (obj is sbyte)
                return XmlConvert.ToString((sbyte)obj);
            if (obj is short)
                return XmlConvert.ToString((short)obj);
            if (obj is ushort)
                return XmlConvert.ToString((ushort)obj);
            if (obj is int)
                return XmlConvert.ToString((int)obj);
            if (obj is uint)
                return XmlConvert.ToString((uint)obj);
            if (obj is long)
                return XmlConvert.ToString((long)obj);
            if (obj is ulong)
                return XmlConvert.ToString((ulong)obj);
            if (obj is float)
                return XmlConvert.ToString((float)obj);
            if (obj is double)
                return XmlConvert.ToString((double)obj);
            if (obj is decimal)
                return XmlConvert.ToString((decimal)obj);
            if (obj is TimeSpan)
                return XmlConvert.ToString((TimeSpan)obj);
            if (obj is Guid)
                return XmlConvert.ToString((Guid)obj);
            if (obj is Uri)
            {
                Uri uri = obj as Uri;
                return uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString;
            }
            if (obj is byte[])
                return string.Join("", ((byte[])obj).Select(b => b.ToString("x2")));
            if (obj is char[])
                return new string((char[])obj);
            if (obj is DateTime)
                return XmlConvert.ToString((DateTime)obj, _dateTimeOption);
            Type t = obj.GetType();
            if (t.IsEnum)
                return Enum.GetName(t, obj);
            return Convert.ToString(obj) ?? "";
        }

        public XhtmlElementBuilder AddComment(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                XNode lastNode = _element.LastNode;
                if (lastNode is XComment)
                    ((XComment)lastNode).Value += text;
                else
                    _element.Add(new XComment(text));
            }
            return this;
        }

        public XhtmlElementBuilder SetAttribute(string localName, object value)
        {
            XName name = XNamespace.None.GetName(localName);
            XAttribute attribute = _element.Attribute(name);
            if (attribute == null)
            {
                if (value != null)
                    _element.Add(new XAttribute(name, ToStringValue(value)));
            }
            else if (value == null)
                attribute.Remove();
            else
                attribute.Value = ToStringValue(value);
            return this;
        }

        public XhtmlElementBuilder Add(params object[] content)
        {
            if (content != null)
                foreach (object obj in content)
                {
                    if (obj == null)
                        continue;
                    if (obj is XAttribute || obj is XNode)
                        _element.Add(obj);
                    else if (obj is XhtmlElementBuilder)
                        _element.Add(((XhtmlElementBuilder)obj)._element);
                    else
                    {
                        string value = ToStringValue(obj);
                        XNode lastNode = _element.LastNode;
                        if (lastNode is XText)
                        {
                            if (value.Length > 0)
                                ((XText)lastNode).Value += value;
                        }
                        else
                            _element.Add(new XText(value));
                    }
                }
            return this;
        }
    }
}
'@ -ReferencedAssemblies 'System.Xml', 'System.Xml.Linq', 'System.Security' -ErrorAction Stop;

Function Get-LocalFilePath {
    [CmdletBinding()]
    [OutputType([string])]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $LocalPath = "MyPowerShellSite:$Path";
    if (Test-Path -LiteralPath $LocalPath -PathType Leaf) { return $LocalPath }
    if (Test-Path -LiteralPath $LocalPath -PathType Container) {
        foreach ($FileName in @('index.html', 'index.htm', 'default.html', 'default.htm')) {
            $LocalPath = "MyPowerShellSite:$Path" | Join-Path -ChildPath $FileName;
            if (Test-Path -LiteralPath $LocalPath -PathType Leaf) { return $LocalPath }
        }
    }
}

Function Send-LocalFileContents {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [System.Net.HttpListenerResponse]$Response,
        
        [Parameter(Mandatory = $true)]
        [string]$Path,
        
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [SessionUser]$User
    )

    $LocalPath = Get-LocalFilePath -Path $Path;
    if ($null -eq $LocalPath) {
        Send-NotFoundResponse -Response $Response -Uri $Path;
    } else {
        $Content = Get-Content -Encoding Byte -LiteralPath $LocalPath;
        $Response.ContentType = [System.Web.MimeMapping]::GetMimeMapping($LocalPath);
        $Response.ContentLength64 = $Content.Length;
        Write-Information -MessageData "Sending $($Response.ContentLength64) bytes ($($Response.ContentType)) from $LocalPath";
        $Response.OutputStream.Write($Content, 0, $Content.Length);
        $Response.Close();
    }
}

Set-Variable -Name 'ApplicationSession' -Option Constant -Scope 'Script' -Value ([WebServer.ApplicationSession]::new($PortNumber, $AdminUserName));
$HttpListener = [System.Net.HttpListener]::new();
$HttpListener.Prefixes.Add($Script:ApplicationSession.BaseUrl.AbsoluteUri);
if ($null -eq (Get-PSDrive -LiteralName 'ScrumPokerSite' -ErrorAction SilentlyContinue)) {
    (New-PSDrive -Name 'ScrumPokerSite' -PSProvider FileSystem -Root ($PSScriptRoot | Join-Path -ChildPath 'wwwRoot')) | Out-Null;
}
$HttpListener.Start();
"Listening on $($Script:ApplicationSession.BaseUrl.AbsoluteUri)" | Write-Host;

$AdminTokenString = $Script:ApplicationSession.AdminUser.GetTokenString($Script:ApplicationSession);
"Administrative token is $([Uri]::EscapeDataString($AdminTokenString))" | Write-Host;
$UriBuilder = [UriBuilder]::new($Script:ApplicationSession.BaseUrl);
$UriBuilder.Path = '/login';
$UriBuilder.Query = "?userName=$([Uri]::EscapeDataString($AdminUserName))&adminToken=$([Uri]::EscapeDataString($AdminTokenString))";
$BrowserUrl = $UriBuilder.Uri.AbsoluteUri;
[System.IO.File]::WriteAllLines(($PSScriptRoot | Join-Path -ChildPath 'SessionInfo.txt'), ([string[]]@(
    $UriBuilder.Uri.AbsoluteUri,
    $AdminTokenString
)), $Script:UTF8Encoding);

start $BrowserUrl;
try {
    $IsRunnning = $true;
    do {
        $UserSession = [WebServer.UserSession]::new($Script:ApplicationSession, $HttpListener.GetContext());
        switch ($UserSession.LocalPath) {
            '/quit' {
                $IsRunnning = $false;
                $UserSession.SendPlainText('Shutting down');
                break;
            }
        }
    } while ($IsRunnning);
} finally { $HttpListener.Stop(); }
