Param(
    [int]$PortNumber = 8080,
    [string]$AdminUserName = 'admin'
)

$DebugPreference = [System.Management.Automation.ActionPreference]::Continue;
$InformationPreference = [System.Management.Automation.ActionPreference]::Continue;

<#
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
#>
Add-Type -TypeDefinition @'
namespace WebServer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    public class ApplicationSession
    {
        public const int TOKEN_LENGTH = 64;
        internal static readonly Encoding DefaultEncoding = new UTF8Encoding(false, false);
        private readonly RNGCryptoServiceProvider _csp = new RNGCryptoServiceProvider();
        private readonly byte[] _token;
        private readonly User _adminUser;
        private readonly Uri _baseUri;
        private readonly Collection<User> _users = new Collection<User>();

        public User AdminUser { get { return _adminUser; } }

        public Collection<User> Users { get { return _users; } }

        public ApplicationSession(int portNumber, string userName, string displayName)
        {
            _adminUser = new User(_csp, userName, displayName);
            _users.Add(_adminUser);
            _token = new byte[TOKEN_LENGTH];
            csp.GetBytes(_token);
            UriBuilder uriBuilder = new UriBuilder("http://localhost");
            uriBuilder.Port = portNumber;
            _baseUri = uriBuilder.Uri;
        }
            
        public static ToStatusDescription(HttpStatusCode satusCode)
        {
            switch (satusCode) {
                case HttpStatusCode.Accepted:
                    return "Request has been accepted for further processing.";
                case HttpStatusCode.BadGateway:
                    return "Bad Gateway";
                case HttpStatusCode.BadRequest:
                    return "Bad Request";
                case HttpStatusCode.Created:
                    return "Resource Created";
                case HttpStatusCode.ExpectationFailed:
                    return "Expectation given in the Expect header could not be met by the server.";
                case HttpStatusCode.GatewayTimeout:
                    return "Gateway Timeout";
                case HttpStatusCode.HttpVersionNotSupported:
                    return "HTTP Version Not Supported";
                case HttpStatusCode.InternalServerError:
                    return "Internal Server Error";
                case HttpStatusCode.LengthRequired:
                    return "Length Required";
                case HttpStatusCode.MethodNotAllowed:
                    return "Method Not Allowed";
                case HttpStatusCode.MovedPermanently:
                    return "Moved Permanently";
                case HttpStatusCode.MultipleChoices:
                    return "Multiple Choices";
                case HttpStatusCode.NoContent:
                    return "No Content";
                case HttpStatusCode.NonAuthoritativeInformation:
                    return "Non-Authoritative Information";
                case HttpStatusCode.NotAcceptable:
                    return "Not Acceptable";
                case HttpStatusCode.NotFound:
                    return "Resource Not Found";
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
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                    return "Requested Range Not Satisfiable";
                case HttpStatusCode.RequestEntityTooLarge:
                    return "Request Entity To oLarge";
                case HttpStatusCode.RequestTimeout:
                    return "Request Timeout";
                case HttpStatusCode.RequestUriTooLong:
                    return "Request Uri Too Long";
                case HttpStatusCode.ResetContent:
                    return "Reset Content";
                case HttpStatusCode.SeeOther:
                    return "See Other";
                case HttpStatusCode.ServiceUnavailable:
                    return "Service Unavailable";
                case HttpStatusCode.SwitchingProtocols:
                    return "Switching Protocols";
                case HttpStatusCode.TemporaryRedirect:
                    return "Temporary Redirect";
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

        public void SendPlainText(string content, HttpStatusCode statusCode = HttpStatusCode.OK, string description = null)
        {
            byte[] buffer = DefaultEncoding.GetBytes(message);
            _response.ContentType = "text/plain";
            _response.ContentLength64 = buffer.Length;
            _response.StatusCode = statusCode;
            _response.StatusDescription = string.IsNullOrWhiteSpace(description) ? ToStatusDescription(statusCode) : description;
            Stream outputStream = _response.OutputStream;
            outputStream.Write(buffer, 0, buffer.Length);
            outputStream.Flush();
            outputStream.Close();
        }
        
        public static XDocument ToXHTMLDocument(params object[] content) { return ToXHTMLDocumentWithTitle(null, content); }

        public static XDocument ToXHTMLDocumentWithTitle(string title, params object[] content)
        {
            string titleText = "Scrum Poker";
            if (title != null && (title = title.Trim()).Length > 0)
                titleText += " - " + title;
            return new XDocument(new ElementBuilder("html",
                new ElementBuilder("head",
                    new ElementBuilder("meta").SetAttribute("charset", "utf-8"),
                    new ElementBuilder("title", titleText),
                    new ElementBuilder("base").SetAttribute("href", "/"),
                    new ElementBuilder("meta").SetAttribute("name", "viewport").SetAttribute("content", "width=1024, initial-scale=1"),
                    new ElementBuilder("link").SetAttribute("rel", "icon").SetAttribute("type", "image/x-icon").SetAttribute("href", "favicon.ico")
                ),
                new ElementBuilder("body", content)
            ).SetAttribute("lang", "en").Element);
        }

        public void SendXhtmlResponse(XDocument content, HttpStatusCode statusCode = HttpStatusCode.OK, string description = null)
        {
            byte[] buffer = DefaultEncoding.GetBytes(content.ToString(SaveOptions.None));
            _response.ContentType = "text/html";
            _response.ContentLength64 = buffer.Length;
            _response.StatusCode = statusCode;
            _response.StatusDescription = string.IsNullOrWhiteSpace(description) ? ToStatusDescription(statusCode) : description;
            Stream outputStream = _response.OutputStream;
            outputStream.Write(buffer, 0, buffer.Length);
            outputStream.Flush();
            outputStream.Close();
        }
        
        public SendRedirectResponse(string url)
        {
            string absoluteUri = newUri(_baseUri, url).AbsoluteUri;
            _response.Headers.Add("Location", absoluteUri);
            SendXhtmlResponse(ToXHTMLDocumentWithTitle("Redirect", new ElementBuilder("a", "Redirecting to " + url).SetAttribute("href", absoluteUri)), HttpStatusCode.Redirect);
        }


        public SendNotFoundResponse(string url)
        {
            SendXhtmlResponse(ToXHTMLDocumentWithTitle("Resource not found", new ElementBuilder("h1", "Resource not found"), url + " was not found."), HttpStatusCode.NotFound);
        }

        public class User
        {
            private readonly string _userName;
            private string _displayName;
            private readonly byte[] _token;

            public string UserName { get { return _userName; } }

            public string DisplayName
            {
                get { return _displayName; }
                set
                {
                    string displayName = value;
                    _displayName = (displayName != null && (displayName = displayName.Trim()).Length > 0) ? displayName : _userName;
                }
            }

            public User(RNGCryptoServiceProvider csp, string userName, string displayName)
            {
                if (csp == null)
                    throw new ArgumentNullException("csp");
                if (userName == null || (userName = userName.Trim()).Length == 0)
                    throw new ArgumentException("User name cannot be null or whitespace.", "userName");
                _userName = userName;
                _displayName = (displayName != null && (displayName = displayName.Trim()).Length > 0) ? displayName : _userName;
                _token = new byte[TOKEN_LENGTH];
                csp.GetBytes(_token);
            }

            public bool IsMatch(string tokenString, ApplicationSession appSession)
            {
                if (appSession == null || tokenString == null || (tokenString = tokenString.Trim()).Length == 0)
                    return false;
                byte[] data;
                try
                {
                    if ((data = ProtectedData.Unprotect(Convert.FromBase64String(tokenString), DefaultEncoding.GetBytes(_userName), DataProtectionScope.CurrentUser)) == null)
                        return false;
                }
                catch { return false; }
                int n1 = _token.Length;
                int n2 = n1 + appSession._token.Length;
                if (data.Length != n2)
                    return false;
                for (int i = 0; i < n1; i++)
                    if (data[i] != _token[i])
                        return false;
                for (int i = n1; i < n2; i++)
                    if (data[i] != appSession._token[i - n1])
                        return false;
                return true;
            }
            public string ToTokenString(ApplicationSession appSession)
            {
                if (appSession == null)
                    throw new ArgumentNullException("appSession");
                return Convert.ToBase64String(ProtectedData.Protect(_token.Concat(appSession._token).ToArray(), DefaultEncoding.GetBytes(_userName), DataProtectionScope.CurrentUser));
            }
        }
    }

    public class UserSession
    {
        public const string CookieName_AuthenticationToken = "AuthenticationToken";
        private static readonly StringComparer _defaultStringComparer = StringComparer.InvariantCultureIgnoreCase;
        private readonly ApplicationSession _appSession;
        private readonly ApplicationSession.User _user;
        private readonly string _localPath;
        private readonly NameValueCollection _query = new NameValueCollection(_defaultStringComparer);
        private readonly NameValueCollection _formData = new NameValueCollection(_defaultStringComparer);
        private readonly string _fragment;
        private readonly string _httpMethod;
        private readonly HttpListenerResponse _response;

        public ApplicationSession AppSession { get { return _appSession; } }
        public ApplicationSession.User User { get { return _user; } }
        public string LocalPath { get { return _localPath; } }
        public NameValueCollection Query { get { return _query; } }
        public NameValueCollection FormData { get { return _formData; } }
        public string Fragment { get { return _httpMethod; } }
        public string HttpMethod { get { return _fragment; } }
        public HttpListenerResponse Response { get { return _response; } }
        private void InitializeNameValueCollection(NameValueCollection target, string text)
        {
            if (text == null || (text = text.Trim()).Length == 0)
                return;
            foreach (string pair in text.Split('&'))
            {
                int i = pair.IndexOf('=');
                if (i < 0)
                    target.Add(pair, "");
                else
                    target.Add(pair.Substring(0, i), pair.Substring(i + 1));
            }
        }
        public UserSession(ApplicationSession appSession, HttpListenerContext context)
        {
            if (appSession == null)
                throw new ArgumentNullException("appSession");
            if (context == null)
                throw new ArgumentNullException("context");
            _appSession = appSession;
            HttpListenerRequest request = context.Request;
            _localPath = string.IsNullOrEmpty(request.Url.LocalPath) ? "/" : (request.Url.LocalPath.EndsWith("/") && request.Url.LocalPath.Length > 0) ? request.Url.LocalPath.Substring(0, request.Url.LocalPath.Length - 1) : request.Url.LocalPath;
            InitializeNameValueCollection(_query, request.Url.Query.StartsWith("?") ? request.Url.Query.Substring(1) : request.Url.Query);
            _fragment = request.Url.Fragment.StartsWith("#") ? request.Url.Fragment.Substring(1) : request.Url.Fragment;
            _httpMethod = request.HttpMethod;
            _response = context.Response;
            if (_defaultStringComparer.Equals(_httpMethod, "POST"))
                using (StreamReader reader = new StreamReader(request.InputStream, ApplicationSession.DefaultEncoding, true))
                    InitializeNameValueCollection(_formData, reader.ReadToEnd());
            Cookie cookie = request.Cookies.OfType<Cookie>().FirstOrDefault(c => c.Name == CookieName_AuthenticationToken);
            if (cookie != null)
            {
                string pair = cookie.Value;
                int i = pair.IndexOf(' ');
                if (i > 0)
                {
                    string userName = pair.Substring(0, i);
                    ApplicationSession.User user = appSession.Users.FirstOrDefault(u => _defaultStringComparer.Equals(u.UserName, userName));
                    if (user.IsMatch(pair.Substring(i + 1), appSession))
                    {
                        _user = user;
                        _response.Cookies.Add(new Cookie(CookieName_AuthenticationToken, pair));
                    }
                }
            }
        }
    }

    public class ElementBuilder
    {
        private XmlDateTimeSerializationMode _dateTimeSerializationMode = XmlDateTimeSerializationMode.RoundtripKind;
        private readonly XElement _element;
        
        public XmlDateTimeSerializationMode DateTimeSerializationMode { get { return _dateTimeSerializationMode; } set { _dateTimeSerializationMode = value; } }

        public XElement Element { get { return _element; } }
        
        private ElementBuilder(XElement element)
        {
            _element = element;
        }

        public ElementBuilder(string name, params object[] content) : this(new XElement(XNamespace.None.GetName(name)))
        {
            if (content != null)
                foreach (object obj in content)
                    Add(obj);
        }

        public static ElementBuilder Create(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return new ElementBuilder(element);
        }

        public ElementBuilder SetAttribute(string name, object obj)
        {
            XName xName = XNamespace.None.GetName(name);
            XAttribute attribute = _element.Attribute(xName);
            if (obj == null)
            {
                if (attribute != null)
                    attribute.Remove();
            }
            else
            {
                string value;
                if (obj is string)
                    value = obj as string;
                else if (obj is bool)
                    value = XmlConvert.ToString((bool)obj);
                else if (obj is char)
                    value = XmlConvert.ToString((char)obj);
                else if (obj is byte)
                    value = XmlConvert.ToString((byte)obj);
                else if (obj is sbyte)
                    value = XmlConvert.ToString((sbyte)obj);
                else if (obj is short)
                    value = XmlConvert.ToString((short)obj);
                else if (obj is ushort)
                    value = XmlConvert.ToString((ushort)obj);
                else if (obj is int)
                    value = XmlConvert.ToString((int)obj);
                else if (obj is uint)
                    value = XmlConvert.ToString((uint)obj);
                else if (obj is long)
                    value = XmlConvert.ToString((long)obj);
                else if (obj is ulong)
                    value = XmlConvert.ToString((ulong)obj);
                else if (obj is float)
                    value = XmlConvert.ToString((float)obj);
                else if (obj is double)
                    value = XmlConvert.ToString((double)obj);
                else if (obj is decimal)
                    value = XmlConvert.ToString((decimal)obj);
                else if (obj is Guid)
                    value = XmlConvert.ToString((Guid)obj);
                else if (obj is TimeSpan)
                    value = XmlConvert.ToString((TimeSpan)obj);
                else if (obj is DateTime)
                    value = XmlConvert.ToString((DateTime)obj, _dateTimeSerializationMode);
                else if ((value = obj.ToString()) == null)
                    value = "";
                if (attribute != null)
                    attribute.Value = value;
                else
                    _element.Add(new XAttribute(xName, value));
            }
            return this;
        }

        public ElementBuilder Add(params object[] content)
        {
            if (content == null)
                return this;
            foreach (object obj in content)
            {
                if (obj == null)
                    continue;
                if (obj is XNode)
                    _element.Add(obj as XNode);
                else if (obj is ElementBuilder)
                    _element.Add((obj as ElementBuilder).Element);
                else
                {
                    string value;
                    if (obj is string)
                        value = obj as string;
                    else if (obj is bool)
                        value = XmlConvert.ToString((bool)obj);
                    else if (obj is char)
                        value = XmlConvert.ToString((char)obj);
                    else if (obj is byte)
                        value = XmlConvert.ToString((byte)obj);
                    else if (obj is sbyte)
                        value = XmlConvert.ToString((sbyte)obj);
                    else if (obj is short)
                        value = XmlConvert.ToString((short)obj);
                    else if (obj is ushort)
                        value = XmlConvert.ToString((ushort)obj);
                    else if (obj is int)
                        value = XmlConvert.ToString((int)obj);
                    else if (obj is uint)
                        value = XmlConvert.ToString((uint)obj);
                    else if (obj is long)
                        value = XmlConvert.ToString((long)obj);
                    else if (obj is ulong)
                        value = XmlConvert.ToString((ulong)obj);
                    else if (obj is float)
                        value = XmlConvert.ToString((float)obj);
                    else if (obj is double)
                        value = XmlConvert.ToString((double)obj);
                    else if (obj is decimal)
                        value = XmlConvert.ToString((decimal)obj);
                    else if (obj is Guid)
                        value = XmlConvert.ToString((Guid)obj);
                    else if (obj is TimeSpan)
                        value = XmlConvert.ToString((TimeSpan)obj);
                    else if (obj is DateTime)
                        value = XmlConvert.ToString((DateTime)obj, _dateTimeSerializationMode);
                    else if ((value = obj.ToString()) == null)
                        return this;
                    XNode lastNode = _element.LastNode;
                    if (lastNode is XText)
                    {
                        if (value.Length > 0)
                            (lastNode as XText).Value += value;
                    }
                    else
                        _element.Add(new XText(value));
                }
            }
            return this;
        }

        public string ToString(SaveOptions options) { return _element.ToString(options); }

        public override string ToString() { return ToString(SaveOptions.DisableFormatting); }
    }
}
'@ -ReferencedAssemblies 'System.Xml', 'System.Xml.Linq', 'System.Security' -ErrorAction Stop;

Function New-XHTMLDocument {
    <#
    .SYNOPSIS
        Creates a new XHTML document object.
    .DESCRIPTION
        Creates a neww XHTML document object for text/html responses.
    #>
    [CmdletBinding()]
    Param(
        # The optional page title which is appended to 'Scrum Poker - '.
        [string]$Title, 
        
        # Body content nodes.
        [Parameter(ValueFromPipeline = $true)]
        [ValidateScript({ $_.Name.LocalName -eq 'body' -and $_.Name.NamespaceName.Length -eq 0 })]
        [System.Xml.Linq.XElement]$Body
    )

    $TitleText = 'Scrum Poker';
    if ($PSBoundParameters.ContainsKey('Title')) { $TitleText = "$TitleText = $Title" }
    Write-Output -InputObject ([System.Xml.Linq.XDocument]::new(([WebServer.ElementBuilder]::new('html',
            [WebServer.ElementBuilder]::new('head',
                [WebServer.ElementBuilder]::new('meta').SetAttribute('charset', 'utf-8'),
                [WebServer.ElementBuilder]::new('title', $TitleText),
                [WebServer.ElementBuilder]::new('base').SetAttribute('href', '/'),
                [WebServer.ElementBuilder]::new('meta').SetAttribute('name', 'viewport').SetAttribute('content', 'width=1024, initial-scale=1'),
                [WebServer.ElementBuilder]::new('link').SetAttribute('rel', 'icon').SetAttribute('type', 'image/x-icon').SetAttribute('href', 'favicon.ico')
            ),
            $Body
        ).SetAttribute('lang', 'en').Element))) -NoEnumerate;
}

Function New-XHtmlForm {
    <#
    .SYNOPSIS
        A short one-line action-based description, e.g. 'Tests if a function is valid'
    .DESCRIPTION
        A longer description of the function, its purpose, common use cases, etc.
    .NOTES
        Information or caveats about the function e.g. 'This function is not supported in Linux'
    .LINK
        Specify a URI to a help page, this will show when Get-Help -Online is used.
    .EXAMPLE
        Test-MyTestFunction -Verbose
        Explanation of the function or its result. You can include multiple examples with additional .EXAMPLE lines
    #>
    [CmdletBinding(DefaultParameterSetName = 'get')]
    Param(
        [Parameter(Mandatory = $true)]
        [ValidateScript({
            return [Uri]::IsWellFormedUriString($_, [System.UriKind]::RelativeOrAbsolute) -and -not ($_.Contains('?') -or $_.Contains('#'));
        })]
        [string]$Action,
        
        [string]$ID,

        [string]$Name,

        [Parameter(ParameterSetName = 'post')]
        [ValidateSet('application/x-www-form-urlencoded', 'multipart/form-data')]
        [string]$EncType,
        
        [ValidateSet('on', 'off')]
        [string]$AutoComplete,

        [ValidateSet('_blank', '_self', '_parent', '_top')]
        [string]$Target,

        [Parameter(ParameterSetName = 'get')]
        [switch]$Get,

        [Parameter(Mandatory = $true, ParameterSetName = 'post')]
        [switch]$Post,

        [Parameter(ValueFromPipeline = $true)]
        [object[]]$Content,

        [switch]$AsBuilder
    )

    Begin {
        $Builder = [WebServer.ElementBuilder]::new('form').SetAttribute('action', $Action).SetAttribute('method', $PSCmdlet.ParameterSetName);
        if ($PSBoundParameters.ContainsKey('Target')) {
            $Builder.SetAttribute('target', $Target);
        }
        if ($PSBoundParameters.ContainsKey('EncType')) {
            $Builder.SetAttribute('enctype', $EncType);
        }
        if ($PSBoundParameters.ContainsKey('AutoComplete')) {
            $Builder.SetAttribute('autocomplete', $AutoComplete);
        }
        if ($PSBoundParameters.ContainsKey('Name')) {
            $Builder.SetAttribute('name', $Name);
        }
        if ($PSBoundParameters.ContainsKey('ID')) {
            $Builder.SetAttribute('id', $ID);
        }
    }
    Process {
        if ($PSBoundParameters.ContainsKey('Content')) {
           $Builder.Add($Content);
        }
    }
    End {
        if ($AsBuilder.IsPresent) {
            Write-Output -InputObject $Builder -NoEnumerate;
        } else {
            Write-Output -InputObject $Builder.Element -NoEnumerate;
        }
    }
}

Function Send-LoginForm {
    [CmdletBinding()]
    [OutputType([SessionUser])]
    Param(
        [Parameter(Mandatory = $true)]
        [System.Net.HttpListenerResponse]$Response,

        [string]$ReturnUrl,

        [string]$LoginFailMessage,
        
        [AllowNull()]
        [SessionUser]$User
    )

    $Builder = (
        [WebServer.ElementBuilder]::new('div', 'User Name').SetAttribute('style', 'font-weight: bold'),
        [WebServer.ElementBuilder]::new('div',
            [WebServer.ElementBuilder]::new('input').SetAttribute('type', 'text').SetAttribute('name', 'UserNameTextBox').SetAttribute('id', 'UserNameTextBox').SetAttribute('style', 'width: 250px')
        ),
        [WebServer.ElementBuilder]::new('div', 'Token String').SetAttribute('style', 'font-weight: bold'),
        [WebServer.ElementBuilder]::new('div',
            [WebServer.ElementBuilder]::new('input').SetAttribute('type', 'password').SetAttribute('name', 'TokenTextBox').SetAttribute('id', 'TokenTextBox').SetAttribute('style', 'width: 100%')
        )
    ) | New-XHtmlForm -Action '/login' -Post -AsBuilder;
    if ($PSBoundParameters.ContainsKey('ReturnUrl')) {
        $Builder.Add([WebServer.ElementBuilder]::new('input').SetAttribute('type', 'hidden').SetAttribute('name', 'ReturnUrl').SetAttribute('value', $ReturnUrl));
    }
    $Builder.Add([WebServer.ElementBuilder]::new('div',
        [WebServer.ElementBuilder]::new('input').SetAttribute('type', 'submit').SetAttribute('value', 'Log In')
    ));
    if ($PSBoundParameters.ContainsKey('LoginFailMessage')) {
        (
            [WebServer.ElementBuilder]::new('h1', 'Scrum Poker Login'),
            [WebServer.ElementBuilder]::new('h2', 'Login Error'),
            [WebServer.ElementBuilder]::new('em', $LoginFailMessage),
            $Builder
        ) | Send-XhtmlResponse -Response $Response -Title 'Login';
    } else {
        (
            [WebServer.ElementBuilder]::new('h1', 'Scrum Poker Login'),
            $Builder
        ) | Send-XhtmlResponse -Response $Response -Title 'Login';
    }
}

Function Receive-LoginRequest {
    Param(
        [Parameter(Mandatory = $true)]
        [Hashtable]$FormData,
        
        [Parameter(Mandatory = $true)]
        [System.Net.HttpListenerResponse]$Response,
        
        [AllowNull()]
        [SessionUser]$User
    )
    $ReturnUrl = '';
    if ($FormData['ReturnUrl'] -is [string]) { $ReturnUrl = $FormData['ReturnUrl'].Trim() }
    if ($FormData['UserNameTextBox'] -is [string] -and $FormData['TokenTextBox'] -is [string]) {
        $UserName = $FormData['UserNameTextBox'].Trim();
        $TokenText = $FormData['TokenTextBox'].Trim();
        [SessionUser]$User = $null;
        [byte[]]$Data = New-Object -TypeName 'System.Byte[]' -ArgumentList 0;
        if ($TokenText.Length -eq 0) {
            if ($UserName.Length -eq 0) {
                if ($ReturnUrl.Length -eq 0) {
                    Send-LoginForm -Response $Response -LoginFailMessage 'User Name and authentication token not provided.';
                } else {
                    Send-LoginForm -Response $Response -ReturnUrl $ReturnUrl -LoginFailMessage 'User Name and authentication token not provided.';
                }
            } else {
                if ($ReturnUrl.Length -eq 0) {
                    Send-LoginForm -Response $Response -LoginFailMessage 'Authentication token not provided.';
                } else {
                    Send-LoginForm -Response $Response -ReturnUrl $ReturnUrl -LoginFailMessage 'Authentication token not provided.';
                }
            }
        } else {
            if ($UserName.Length -eq 0) {
                if ($ReturnUrl.Length -eq 0) {
                    Send-LoginForm -Response $Response -LoginFailMessage 'User Name not provided.';
                } else {
                    Send-LoginForm -Response $Response -ReturnUrl $ReturnUrl -LoginFailMessage 'User Name not provided.';
                }
            } else {
                $User = $null;
                [byte[]]$Data = $null;
                try {
                    if ($null -ne ($Data = [System.Convert]::FromBase64String($TokenString)) -and $Data.Length -gt 0) {
                        $Data = [System.Security.Cryptography.ProtectedData]::Unprotect($Data, [SessionUser]::Encoding.GetBytes($User.UserName), [System.Security.Cryptography.DataProtectionScope]::CurrentUser)
                    }
                }
                catch { $Data = $null; }
                if ($null -ne $Data -and $Data.Length -gt 0) {
                    [SessionUser]$User = Get-SessionUser -UserName $UserName;
                    if ($null -ne $User) {
                        $l1 = $Script:SessionToken.Length;
                        $l2 = $l1 = $User.Token.Length;
                        if ($Data.Length -ne $l2) {
                            $User = $null;
                        } else {
                            for ($i = 0; $i -lt $l1; $i++) {
                                if ($Data[$i] -ne $Script:SessionToken[$i]) {
                                    $User = $null;
                                    break;
                                }
                            }
                            if ($null -ne $User) {
                                for ($i = $l1; $i -lt $l2; $i++) {
                                    if ($Data[$i] -ne $User.Token[$i - $l1]) {
                                        $User = $null;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if ($null -ne $User) {
                     if ($ReturnUrl.Length -eq 0) {
                        Send-RedirectResponse -Response $Response -Url '/' -User $User;
                    } else {
                        Send-RedirectResponse -Response $Response -Url $ReturnUrl -User $User;
                    }
                } else {
                    if ($ReturnUrl.Length -eq 0) {
                        Send-LoginForm -Response $Response -LoginFailMessage 'Invalid user name or authentication token';
                    } else {
                        Send-LoginForm -Response $Response -ReturnUrl $ReturnUrl -LoginFailMessage 'Invalid user name or authentication token';
                    }
                }
            }
        }
    } else {
        if ($ReturnUrl.Length -gt 0) {
            Send-LoginForm -Response $Response -ReturnUrl $ReturnUrl -LoginFailMessage "Invalid HTTP POST";
        } else {
            Send-LoginForm -Response $Response -LoginFailMessage "Invalid HTTP POST";
        }
    }
}

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

Function Receive-GetRequest {
    Param(
        [System.Net.HttpListenerResponse]$Response,
        [SessionUser]$User
    )
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

Function Import-UrlEncodedFormData {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [AllowNull()]
        [string]$Text
    )
    $FormData = @{};
    if (-not [string]::IsNullOrWhiteSpace($Text)) {
        $Text.Split('&') | ForEach-Object {
            ($Key, $Value) = $_.Split('=', 2);
            $Key = [Uri]::UnescapeDataString($Key);
            if ($FormData.ContainsKey($Key)) {
                if ($FormData[$Key] -is [bool]) {
                    if ($null -ne $Value) {
                        $FormData[$Key] = ([object[]]@($FormData[$Key], [Uri]::UnescapeDataString($Value)));
                    }
                } else {
                    if ($FormData[$Key] -is [string]) {
                        if ($null -eq $Value) {
                            $FormData[$Key] = ([object[]]@($FormData[$Key], $true));
                        } else {
                            $FormData[$Key] = ([object[]]@($FormData[$Key], [Uri]::UnescapeDataString($Value)));
                        }
                    } else {
                        if ($null -eq $Value) {
                            $FormData[$Key] = ([object[]](@($FormData[$Key]) + @($true)));
                        } else {
                            $FormData[$Key] = ([object[]](@($FormData[$Key]) + @([Uri]::UnescapeDataString($Value))));
                        }
                    }
                }
            } else {
                if ($null -eq $Value) {
                    $FormData[$Key] = $true;
                } else {
                    $FormData[$Key] = [Uri]::UnescapeDataString($Value);
                }
            }
        }
    }

    return $FormData;
}

Function Receive-GetRequest {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$LocalPath,

        [Parameter(Mandatory = $true)]
        [Hashtable]$Query,
        
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Fragment,
        
        [Parameter(Mandatory = $true)]
        [System.Net.HttpListenerResponse]$Response,
        
        [AllowNull()]
        [SessionUser]$User
    )
}

Function Receive-PostRequest {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [string]$LocalPath,
        
        [Parameter(Mandatory = $true)]
        [Hashtable]$Query,
        
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Fragment,
        
        [Parameter(Mandatory = $true)]
        [Hashtable]$FormData,
        
        [Parameter(Mandatory = $true)]
        [System.Net.HttpListenerResponse]$Response,
        
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [SessionUser]$User
    )
}

Function Receive-Request {
    [CmdletBinding()]
    Param(
        # https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistenerrequest
        [Parameter(Mandatory = $true)]
        [System.Net.HttpListenerRequest]$Request,
        
        # https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistenerresponse
        [Parameter(Mandatory = $true)]
        [System.Net.HttpListenerResponse]$Response
    )
    $LocalPath = $Request.Url.LocalPath -replace '/+$', '';
    [SessionUser]$User = $null;
    [System.Net.Cookie]$Cookie = $null;
    foreach ($c in $Request.Cookies) {
        if ($c.Name -eq $Script:CookieName_AuthenticationToken) {
            $Cookie = $c;
            break;
        }
    }
    $Request.HttpMethod
    if ($null -eq $c) { return $null }
    ($UserName, $TokenString) = $Cookie.Value.Split(' ', 2);
    if ([string]::IsNullOrWhiteSpace($UserName) -or [string]::IsNullOrWhiteSpace($TokenString)) { return $null }
    [SessionUser]$User = Get-SessionUser -UserName $UserName;
    if ($null -ne $User) {
        [byte[]]$Data = New-Object -TypeName 'System.Byte[]' -ArgumentList 0;
        try {
            if ($null -eq ($Data = [System.Convert]::FromBase64String($TokenString)) -or $Data.Length -eq 0 -or $null -eq ($Data = [System.Security.Cryptography.ProtectedData]::Unprotect($Data, [SessionUser]::Encoding.GetBytes($User.UserName), [System.Security.Cryptography.DataProtectionScope]::CurrentUser)) -or $Data.Length -eq 0) {
                $User = $null;
            }
        }
        catch { $User = $null }
    }
    if ($null -ne $User) {
        $l1 = $Script:SessionToken.Length;
        $l2 = $l1 = $User.Token.Length;
        if ($Data.Length -ne $l2) { return $null }
        for ($i = 0; $i -lt $l1; $i++) {
            if ($Data[$i] -ne $Script:SessionToken[$i]) {
                $User = $null;
                break;
            }
        }
        if ($null -ne $User) {
            for ($i = $l1; $i -lt $l2; $i++) {
                if ($Data[$i] -ne $User.Token[$i - $l1]) {
                    $User = $null;
                    break;
                }
            }
        }
    }
    if ($null -ne $User) {
        $Cookie = [System.Net.Cookie]::new($Script:CookieName_AuthenticationToken, (Get-TokenString -User $User));
        $Response.Cookies.Add($Cookie);
    }

    $Query = Import-UrlEncodedFormData -Text $Request.Url.Query;
    switch ($Request.HttpMethod) {
        'GET' {
            Receive-GetRequest -LocalPath $LocalPath -Query $Query -Fragment $Request.Url.Fragment -Response $Response -User $User;
            break;
        }
        'POST' {
            Write-Debug -Message "LocalPath=`"$LocalPath`"; ContentEncoding=`"$($Request.ContentEncoding)`"; ContentLength64=$($Request.ContentLength64); ContentType=`"$($Request.ContentType)`"" -Debug Continue;
            $Text = '';
            try {
                $Reader = [System.IO.StreamReader]::new($Request.InputStream, $Script:UTF8Encoding, $true);
                try { $Text = $Reader.ReadToEnd() }
                finally { $Reader.Close() }
            } finally { $Request.InputStream.Close() }
            Write-Debug -Message "Read form data: $Text" -Debug Continue;
            $FormData = Import-UrlEncodedFormData -Text $Text -Debug Continue -InformationAction Continue;
            Receive-PostRequest -LocalPath $LocalPath -Query $Query -Fragment $Request.Url.Fragment -FormData $FormData -Response $Response -User $User;
            break;
        }
        default {
            
        }
    }
}

<#
$HttpListener = [System.Net.HttpListener]::new();
$HttpListener.Prefixes.Add($UriBuilder.Uri.AbsoluteUri);
if ($null -eq (Get-PSDrive -LiteralName 'ScrumPokerSite' -ErrorAction SilentlyContinue)) {
    (New-PSDrive -Name 'ScrumPokerSite' -PSProvider FileSystem -Root ($PSScriptRoot | Join-Path -ChildPath 'WebRoot')) | Out-Null;
}
$HttpListener.Start();
"Listening on $($UriBuilder.Uri.AbsoluteUri)" | Write-Host;
$AdminTokenString = Get-TokenString -User $Script:AdminUse -Debug Continue -InformationAction Continue;
"Administrative token is $([Uri]::EscapeDataString($AdminTokenString))" | Write-Host;
[System.IO.File]::WriteAllLines(($PSScriptRoot | Join-Path -ChildPath 'SessionInfo.txt'), ([string[]]@(
    $UriBuilder.Uri.AbsoluteUri,
    $AdminUserName,
    $AdminTokenString
)), $Script:UTF8Encoding)
try {
    # https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistenercontext
    $IsRunnning = $true;
    [System.Net.HttpListenerContext]$Context = $null;
    
    do {
        if ($null -eq ($Context = $HttpListener.GetContext())) { break }
        # https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistenerrequest
        [System.Net.HttpListenerRequest]$Request = $Context.Request;
        # https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistenerresponse
        [System.Net.HttpListenerResponse]$Response = $Context.Response;
        $LocalPath = $Request.Url.LocalPath -replace '/+$', '';
        switch ($Request.HttpMethod) {
            'GET' {
                switch ($LocalPath) {
                    '/quit' {
                        [SessionUser]$User = $null;
                        $User = Get-AuthenticatedUser -Request $Request -Debug Continue -InformationAction Continue;
                        if ($null -eq $User) {
                            Send-LoginForm -Response $Context.Response -ReturnUrl $Url.PathAndQuery -Debug Continue -InformationAction Continue;
                        } else {
                            if ([object]::ReferenceEquals($Script:AdminUser, $User)) {
                                (
                                    [System.Xml.Linq.XElement]::new('h1', 'Exit'),
                                    'Exiting Web Application'
                                ) | Send-XhtmlResponse -Response $Response -Title 'Exiting Web Application' -Debug Continue -InformationAction Continue;
                                $IsRunnning = $false;
                            } else {
                                Send-NotFoundResponse -Response $Response -Uri $Request.Url -Debug Continue -InformationAction Continue;
                            }
                        }
                        break;
                    }
                    '/login' {
                        Send-LoginForm -Response $Context.Response;
                        break;
                    }
                    default {
                        [SessionUser]$User = $null;
                        $User = Get-AuthenticatedUser -Request $Request -Debug Continue -InformationAction Continue;
                        if ($null -eq $User) {
                            Send-LoginForm -Response $Context.Response -ReturnUrl = $Url.PathAndQuery -Debug Continue -InformationAction Continue;
                        } else {
                            Send-LocalFileContents -Response $Response -Path $LocalPath -Debug Continue -InformationAction Continue;
                        }
                        break;
                    }
                }
                break;
            }
            'POST' {
                Write-Debug -Message "LocalPath=`"$LocalPath`"; ContentEncoding=`"$($Request.ContentEncoding)`"; ContentLength64=$($Request.ContentLength64); ContentType=`"$($Request.ContentType)`"" -Debug Continue;
                $Text = '';
                try {
                    $Reader = [System.IO.StreamReader]::new($Request.InputStream, $Script:UTF8Encoding, $true);
                    try { $Text = $Reader.ReadToEnd() }
                    finally { $Reader.Close() }
                } finally { $Request.InputStream.Close() }
                Write-Debug -Message "Read form data: $Text" -Debug Continue;
                $FormData = Import-UrlEncodedFormData -Text $Text -Debug Continue -InformationAction Continue;
                if ($LocalPath -eq '/login') {
                    Receive-LoginRequest -FormData $FormData -Response $Response -Debug Continue -InformationAction Continue;
                    $Request.InputStream
                    $Text = '';
                    try {  }
                    finally { $Request.InputStream.Close() }
                } else {
                    [SessionUser]$User = $null;
                    $User = Get-AuthenticatedUser -Request $Request -Debug Continue -InformationAction Continue;
                    if ($null -eq $User) {
                        Send-LoginForm -Response $Context.Response -Debug Continue -InformationAction Continue;
                    } else {
                        Receive-PostRequest -Path $LocalPath -Query (Import-UrlEncodedFormData -Text $Request.Url.Query -Debug Continue -InformationAction Continue) -Fragment $Request.Url.Fragment -FormData $FormData -Response $Response -User $User -Debug Continue -InformationAction Continue;
                    }
                }
                break;
            }
        }
    } while ($IsRunnning);
} finally { $HttpListener.Stop(); }

#>