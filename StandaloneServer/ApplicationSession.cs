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

namespace ScrumPoker.StandaloneServer
{
    public partial class ApplicationSession : IDisposable
    {
        private readonly SessionMessage.Queue _messages = new SessionMessage.Queue();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _isDisposed = false;
        private IAsyncResult _currentListenerResult = null;
        private readonly ManualResetEvent _applicationTerminatedEvent = new ManualResetEvent(false);

        public static readonly StringComparer DefaultComparer = StringComparer.InvariantCultureIgnoreCase;
        public static readonly Encoding DefaultEncoding = new UTF8Encoding(false, false);

        private readonly DirectoryInfo _webRootDirectory;

        private readonly Uri _baseUrl;
        public Uri BaseUrl { get { return _baseUrl; } }

        private readonly DataContracts.ScrumSession _sessionData = new DataContracts.ScrumSession();
        
        internal DataContracts.ScrumSession SessionData { get { return _sessionData; } }

        private readonly HttpListener _listener;

        private static bool TryGetCurrentAdminUser(out DataContracts.MemberCredentials result)
        {
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            if (wi == null || !wi.IsAuthenticated || wi.IsAnonymous || wi.IsGuest || wi.IsSystem)
            {
                result = null;
                return false;
            }
            int index = wi.Name.IndexOf('/');
            result = new DataContracts.MemberCredentials()
            {
                DisplayName = ((index < 0) ? wi.Name : wi.Name.Substring(index + 1)).Trim(),
                UserName = wi.Name
            };
            if (result.DisplayName.Length == 0)
                result.DisplayName = wi.Name;
            return true;
        }
        public ApplicationSession(DataContracts.HostSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            if (string.IsNullOrWhiteSpace((settings = settings.Clone()).WebRootPath))
                throw new ArgumentException("webRootPath is not specified.", "settings");
            _webRootDirectory = new DirectoryInfo(settings.WebRootPath);
            if (!_webRootDirectory.Exists)
                throw new ArgumentException("Path specified by 'webRootPath' not found.", "settings");
            DataContracts.MemberCredentials adminUser;
            if (settings.AdminUser == null)
            {
                if (!settings.UseIntegratedWindowsAuthentication)
                    throw new ArgumentException("Admin user must be specified when not using integrated windows authentication.", "settings");
                if (!TryGetCurrentAdminUser(out adminUser))
                    throw new ArgumentException("Unable to detect current user identity.", "settings");
                settings.AdminUser = adminUser;
            }
            else
            {
                if (settings.AdminUser.UserName.Length == 0)
                {
                    if (settings.UseIntegratedWindowsAuthentication)
                    {
                        if (!TryGetCurrentAdminUser(out adminUser))
                            throw new ArgumentException("Unable to detect current user identity.", "settings");
                        settings.AdminUser.UserName = adminUser.UserName;
                    }
                    else
                        throw new ArgumentException("Admin user name must be specified when not using integrated windows authentication.", "settings");
                }
                if (!settings.UseIntegratedWindowsAuthentication && settings.AdminUser.Password == null)
                    throw new ArgumentException("Password for admin user must be specified when not using integrated windows authentication.", "settings");
            }
            if (settings.ScrumPokerUsers.Any(d => d.IsParticipant && d.UserName.Length == 0))
                throw new ArgumentException("All participants must have a user name specified.", "settings");
            if (!settings.UseIntegratedWindowsAuthentication && settings.ScrumPokerUsers.Any(p => p.Password == null))
                throw new ArgumentException("Passwords for members must be specified when not using integrated windows authentication.", "settings");
            _baseUrl = new UriBuilder("http://localhost")
            {
                Port = settings.PortNumber
            }.Uri;
            (_listener = new HttpListener()
            {
                AuthenticationSchemes = settings.UseIntegratedWindowsAuthentication ? AuthenticationSchemes.IntegratedWindowsAuthentication : AuthenticationSchemes.Basic
            }).Prefixes.Add(_baseUrl.AbsoluteUri);
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

        public bool TryGetSessionTerminated(int millisecondsTimeout) { return _applicationTerminatedEvent.WaitOne(millisecondsTimeout); }

        public bool TryGetSessionTerminated(TimeSpan timeout) { return _applicationTerminatedEvent.WaitOne(timeout); }

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
                        if (userSession.HttpMethod == WebRequestMethods.Http.Get)
                        {
                            FileInfo fileInfo;
                            if (userSession.TryGetFileInfo(_webRootDirectory, out fileInfo))
                            {
                                // TODO: Send file
                            }
                            else
                            {
                                // TODO: Send Not Found
                            }
                        }
                        else if (userSession.HttpMethod == WebRequestMethods.Http.Post)
                        {
                            // TODO: Send Not Found
                        }
                        else if (userSession.HttpMethod == WebRequestMethods.Http.Put)
                        {
                            // TODO: Send Not Found
                        }
                        else
                        {
                            // Send denied
                        }
                    break;
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

        protected virtual void Dispose(bool disposing)
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