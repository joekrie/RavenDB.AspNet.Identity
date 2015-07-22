using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Threading;
using Raven.Client;
using Raven.Abstractions.Commands;
using System.ComponentModel;

namespace RavenDB.AspNet.Identity
{
    public class RoleStore<TRole> : IRoleStore<TRole>
        where TRole : IdentityRole
    {
        private bool _disposed;
        private readonly Func<IAsyncDocumentSession> _getSessionFunc;
        private IAsyncDocumentSession _session;
        private IDocumentStore _documentStore;

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
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            await _session.StoreAsync(role, cancellationToken);
            await SaveChanges(cancellationToken);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            var loadedRole = await _session.LoadAsync<TRole>(role.Id, cancellationToken);

            if (loadedRole.SetUpdatedProperties(role))
            {
                try
                {
                    await SaveChanges(cancellationToken);
                }
                catch (Exception)
                {
                    return IdentityResult.Failed();
                }
            }
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            _session.Advanced.Defer(new DeleteCommandData { Key = role.Id });
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (Exception)
            {
                return IdentityResult.Failed();
            }
            return IdentityResult.Success;
        }

        private async Task SaveChanges(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _session.SaveChangesAsync(cancellationToken);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            return Task.FromResult(ConvertIdToString(role.Id));
        }

        public virtual string ConvertIdFromString(string id)
        {
            if (id == null)
            {
                return default(string);
            }
            return (string)TypeDescriptor.GetConverter(typeof(string)).ConvertFromInvariantString(id);
        }

        public virtual string ConvertIdToString(string id)
        {
            if (id.Equals(default(string)))
            {
                return null;
            }
            return id.ToString();
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            role.Name = roleName;
            return Task.FromResult(0);
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            role.NormalizedName = normalizedName;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Find a role by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TRole> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var roleId = ConvertIdFromString(id);
            return _session.LoadAsync<TRole>(id, cancellationToken);
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
