using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ScrumPokerServer
{
    public partial class ApplicationSession : IDisposable, DataContracts.TitleAndIdentifier
    {
        private readonly SessionMessage.Queue _messages = new SessionMessage.Queue();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _isDisposed = false;
        private IAsyncResult _currentListenerResult = null;
        private ManualResetEvent _applicationTerminatedEvent = new ManualResetEvent();
        private readonly DataContracts.SessionEntity _session = new DataContracts.SessionEntity();

        public static readonly StringComparer DefaultComparer = StringComparer.InvariantCultureIgnoreCase;
        public static readonly Encoding DefaultEncoding = new UTF8Encoding(false, false);

        private readonly DirectoryInfo _webRootDirectory;

        private readonly Uri _baseUrl;
        public Uri BaseUrl { get { return _baseUrl; } }

        private readonly DataContracts.SessionEntity _sessionData = new readonly DataContracts.SessionEntity();
        
        // TODO: Make this obsolete
        [Obsolete("Use _sessionData, instead")]
        private readonly AdminUsery _adminUser;
        [Obsolete("Use _sessionData, instead")]
        public AdminUser AdminUser { get { return _adminUser; } }

        // TODO: Make this obsolete
        [Obsolete("Use _sessionData, instead")]
        private readonly Collection<WebAppUser> _backingUsers = new Collection<WebAppUser>();
        [Obsolete("Use _sessionData, instead")]
        private readonly ReadOnlyCollection<WebAppUser> _users;
        [Obsolete("Use _sessionData, instead")]
        public ReadOnlyCollection<WebAppUser> Users { get { return _users; } }

        private readonly HttpListener _listener;

        public static ApplicationSession CreateDigestAuthSession(string webRootPath, int portNumber, string adminUserName, SecureString adminPassword, string displayName = null)
        {
            if (string.IsNullOrWhiteSpace(webRootPath))
                throw new ArgumentException("'webRootPath' cannot be null or whitespace.", "webRootPath");
            if (string.IsNullOrWhiteSpace(adminUserName))
                throw new ArgumentException("'adminUserName' cannot be null or whitespace.", "adminUserName");
            if (adminPassword == null || adminPassword.Length == 0)
                throw new ArgumentException("'adminPassword' cannot be null or empty.", "adminPassword");
            return new ApplicationSession(webRootPath, portNumber, AuthenticationSchemes.Digest, adminUserName, adminPassword, displayName);
        }

        public static ApplicationSession CreateIntegratedWindowsAuthSession(string webRootPath, int portNumber, string adminUserName, string displayName = null)
        {
            if (string.IsNullOrWhiteSpace(webRootPath))
                throw new ArgumentException("'webRootPath' cannot be null or whitespace.", "webRootPath");
            if (string.IsNullOrWhiteSpace(adminUserName))
                throw new ArgumentException("'adminUserName' cannot be null or whitespace.", "adminUserName");
            return new ApplicationSession(webRootPath, portNumber, AuthenticationSchemes.IntegratedWindowsAuthentication, adminUserName, null, displayName);
        }

        public static ApplicationSession CreateIntegratedWindowsCurrentUserSession(string webRootPath, int portNumber)
        {
            if (string.IsNullOrWhiteSpace(webRootPath))
                throw new ArgumentException("'webRootPath' cannot be null or whitespace.", "webRootPath");
            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
            if (windowsIdentity == null || !windowsIdentity.IsAuthenticated || windowsIdentity.IsAnonymous || windowsIdentity.IsGuest || windowsIdentity.IsSystem || string.IsNullOrWhiteSpace(windowsIdentity.Name))
                throw new InvalidOperationException("Cannot determine current user identity.");
            return new ApplicationSession(webRootPath, portNumber, AuthenticationSchemes.IntegratedWindowsAuthentication, windowsIdentity.Name);
        }

        public static ApplicationSession CreateNegotiateAuthSession(string webRootPath, int portNumber, string adminUserName, string displayName = null)
        {
            if (string.IsNullOrWhiteSpace(webRootPath))
                throw new ArgumentException("'webRootPath' cannot be null or whitespace.", "webRootPath");
            if (string.IsNullOrWhiteSpace(adminUserName))
                throw new ArgumentException("'adminUserName' cannot be null or whitespace.", "adminUserName");
            return new ApplicationSession(webRootPath, portNumber, AuthenticationSchemes.Negotiate, adminUserName, null, displayName);
        }

        public static ApplicationSession CreateNtlmCurrentUserSession(string webRootPath, int portNumber)
        {
            if (string.IsNullOrWhiteSpace(webRootPath))
                throw new ArgumentException("'webRootPath' cannot be null or whitespace.", "webRootPath");
            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
            if (windowsIdentity == null || !windowsIdentity.IsAuthenticated || windowsIdentity.IsAnonymous || windowsIdentity.IsGuest || windowsIdentity.IsSystem || string.IsNullOrWhiteSpace(windowsIdentity.Name))
                throw new InvalidOperationException("Cannot determine current user identity.");
            return new ApplicationSession(webRootPath, portNumber, AuthenticationSchemes.Ntlm, windowsIdentity.Name);
        }

        public static ApplicationSession CreateNtlmAuthSession(string webRootPath, int portNumber, string adminUserName, string displayName = null)
        {
            if (string.IsNullOrWhiteSpace(webRootPath))
                throw new ArgumentException("'webRootPath' cannot be null or whitespace.", "webRootPath");
            if (string.IsNullOrWhiteSpace(adminUserName))
                throw new ArgumentException("'adminUserName' cannot be null or whitespace.", "adminUserName");
            return new ApplicationSession(webRootPath, portNumber, AuthenticationSchemes.Ntlm, adminUserName, null, displayName);
        }

        public ApplicationSession(DataContracts.HostSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            if (string.IsNullOrWhiteSpace(settings.webRootPath))
                throw new ArgumentException("webRootPath is not specified.", "settings");
            _webRootDirectory = new DirectoryInfo(settings.webRootPath);
            if (!_webRootDirectory.Exists)
                throw new ArgumentException("Path specified by 'webRootPath' not found.", "settings");
            if (settings.portNumber < 1 || settings.portNumber > 65535)
                throw new ArgumentException("Invalid port number.", "settings");
            string s;
            if (settings.participants == null || settings.participants.Length == 0 || (settings.participants = settings.participants.Where(s => s != null).ToArray()).Length == 0)
                throw new ArgumentException("Not participants were specified.", "settings");
            User u;
            foreach (DataContracts.WebAppUser w in settings.participants)
            {
                if (u = User.Create(w, out s) == null)
                    throw new ArgumentException(s, "settings");
                _backingUsers.Add(u);
            }
            s = settings.authentication;
            AuthenticationSchemes authenticationScheme;
            if (s != null && (s = s.Trim()).Length > 0)
            {
                if (Enum.TryParse<AuthenticationSchemes>(s, true, out authenticationScheme))
                    switch (authenticationScheme)
                    {
                        case AuthenticationSchemes.None:
                        case AuthenticationSchemes.Basic:
                            throw new ArgumentException("Unsupported authentication value.", "settings");
                    }
                else
                    throw new ArgumentException("Invalid authentication value.", "settings");
            }
            else
                authenticationScheme = (settings.adminUser != null && !(string.IsNullOrWhiteSpace(settings.adminUser.password) || string.IsNullOrWhiteSpace(settings.adminUser.userName))) ?
                    AuthenticationSchemes.Negotiate : AuthenticationSchemes.Digest;
            if (settings.adminUser == null)
                settings.adminUser = new DataContracts.AdminUser();
            if (authenticationScheme == AuthenticationSchemes.Digest)
            {
                if (string.IsNullOrWhiteSpace(settings.adminUser.userName) || string.IsNullOrWhiteSpace(settings.adminUser.password))
                    throw new ArgumentException("'adminUser' must specify both a username and password for digest authentication.", "settings");
                DataContracts.WebAppUser w = settings.participants.FirstOrDefault(p => string.IsNullOrWhiteSpace(p.password));
                if (w != null)
                    throw new ArgumentException("Participant '" + w.userName + "' must also specify a password for digest authentication.", "settings");
                // TODO: Figure out how to handle Digest
            }
            else if (string.IsNullOrWhiteSpace(settings.adminUser.userName))
            {
                WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
                if (windowsIdentity == null || !windowsIdentity.IsAuthenticated || windowsIdentity.IsAnonymous || windowsIdentity.IsGuest || windowsIdentity.IsSystem || string.IsNullOrWhiteSpace(windowsIdentity.Name))
                    throw new InvalidOperationException("Cannot determine current user identity.");
                settings.adminUser.userName = windowsIdentity.Name;
            }
            if (_adminUser = User.Create(settings.adminUser, out s) == null)
                throw new ArgumentException("Admin user error: " + s, "settings");
            if (settings.adminUser.isParticipant)
                _backingUsers.Add(_adminUser);
            UriBuilder ub = new UriBuilder("http://localhost");
            ub.Port = settings.portNumber;
            _baseUrl = ub.Uri;
            _users = new ReadOnlyCollection<User>(_backingUsers);
            _listener = new HttpListener();
            _listener.Prefixes.Add(_baseUrl.AbsoluteUri);
        }

        internal bool TryRaiseAppSessionMessage(TraceLevel level, string message) { return _messages.TryAdd(level, message); }
        
        internal bool TryRaiseAppSessionMessage(TraceLevel level, string message, Guid concurrencyId) { return _messages.TryAdd(level, message, concurrencyId); }
        
        public void Start()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("ApplicationSession");
            if (_listener.IsListening)
                return;
            _listener.Start();
            _currentListenerResult = _listener.BeginGetContext(ListenerCallback, _cancellationTokenSource.Token);
        }

        public void Stop()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("ApplicationSession");
            if (_listener.IsListening)
                _listener.Stop();
        }

        public static bool TryGetSessionTerminated(int millisecondsTimeout) { return _applicationTerminatedEvent.WaitOne(millisecondsTimeout); }

        public static bool TryGetSessionTerminated(TimeSpan timeout) { return _applicationTerminatedEvent.WaitOne(timeout); }

        private void ListenerCallback(IAsyncResult result)
        {
            CancellationToken token = (CancellationToken)result.AsyncState;
            token.ThrowIfCancellationRequested();
            try
            {
                UserSession userSession = new UserSession(this, _listener.EndGetContext(result));
                if (userSession.User == null)
                {
                    // TODO: Send unauthorized
                }
                switch (userSession.LocalPath)
                {
                    case "/userStory/new":
                        // TODO: Add user story
                        break;
                    default:
                        if (userSession.HttpMethod == "GET")
                        {
                            FileInfo fileInfo;
                            if (userSession.HttpMethod == "GET" && userSession.TryGetFileInfo(_webRootDirectory, out fileInfo))
                            {
                                // TODO: Send file
                            }
                            else
                            {
                                // TODO: Send Not Found
                            }
                        }
                        else if (userSession.HttpMethod == "POST")
                        {
                            // TODO: Send Not Found
                        }
                        else
                        {
                            // Send denied
                        }

                }
            }
            catch (Exception exception)
            {
                _messages.TryAdd(TraceLevel.Error, string.IsNullOrWhiteSpace(exception.Message) ? "Unexpected " + exception.GetType().Name + " while processing request." : "Unexpected exception while processing request: " + exception.Message);
            }
            finally
            {
                if (_listener.IsListening)
                    _currentListenerResult = _listener.BeginGetContext(ListenerCallback, token);
            }
        }

        public bool TryGetMessage(int millisecondsTimeout, out SessionMessage item) { return _messages.TryTake(millisecondsTimeout, out item); }

        public bool TryGetMessage(TimeSpan timeout, out SessionMessage item) { return _messages.TryTake(timeout, out item); }

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;
            _isDisposed = true;
            if (disposing)
                try
                {
                    if (!_cancellationTokenSource.IsCancellationRequested)
                        _cancellationTokenSource.Cancel();
                }
                finally
                {
                    try
                    {
                        if (_listener.IsListening)
                            _listener.Stop();
                    }
                    finally
                    {
                        try { _applicationTerminatedEvent.Set(); }
                        finally
                        {
                            try
                            {
                                IAsyncResult listenerResult = _currentListenerResult;
                                if (listenerResult != null && !listenerResult.IsCompleted)
                                    listenerResult.AsyncWaitHandle.WaitOne(1000);
                            }
                            finally
                            {
                                try { _applicationTerminatedEvent.Dispose(); }
                                finally
                                {
                                    try { _messages.Dispose(); }
                                    finally { _cancellationTokenSource.Dispose(); }
                                }
                            }
                        }
                    }
                }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
    }
}