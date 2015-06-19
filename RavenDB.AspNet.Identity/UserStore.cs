using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Raven.Client;

namespace RavenDB.AspNet.Identity
{
    public partial class UserStore<TUser> 
        where TUser : IdentityUser
    {
        private bool _disposed;
        private readonly Func<IAsyncDocumentSession> _getSessionFunc;
        private IAsyncDocumentSession _session;

        private IAsyncDocumentSession Session
        {
            get
            {
                if (_session == null)
                {
                    _session = _getSessionFunc();
                    _session.Advanced.DocumentStore.Conventions
                        .RegisterIdConvention<IdentityUser>((dbname, commands, user) => "IdentityUsers/" + user.Id);
                }
                return _session;
            }
        }

        public UserStore(Func<IAsyncDocumentSession> getSession)
        {
            _getSessionFunc = getSession;
        }

        public UserStore(IAsyncDocumentSession session)
        {
            _session = session;
        }

        public void Dispose()
        {
            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}