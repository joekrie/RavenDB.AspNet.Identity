using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Raven.Client;

namespace RavenDB.AspNet.Identity
{
    public class IdentityRole
    {
        public string Id { get; set; }
    }

    public class RoleStore<TRole> : IRoleStore<TRole>
        where TRole : IdentityRole
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
                        .RegisterIdConvention<IdentityRole>((dbname, commands, role) => "IdentityRoles/" + role.Id);
                }

                return _session;
            }
        }

        public RoleStore(Func<IAsyncDocumentSession> getSession)
        {
            _getSessionFunc = getSession;
        }

        public RoleStore(IAsyncDocumentSession session)
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

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (string.IsNullOrEmpty(role.Id))
            {
                throw new InvalidOperationException("role.Id property must be specified before calling CreateAsync");
            }

            await _session.StoreAsync(role, cancellationToken);
            return IdentityResult.Success;
        }

        public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
