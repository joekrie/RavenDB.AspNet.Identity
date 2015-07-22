using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Raven.Abstractions.Exceptions;

namespace RavenDB.AspNet.Identity
{
    public partial class UserStore<TUser>: IUserClaimStore<TUser>
    {
        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IList<Claim> result = user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
            return Task.FromResult(result);
        }

        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            foreach (var claim in claims)
            {
                if (!user.Claims.Any(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value))
                {
                    user.Claims.Add(new IdentityUserClaim
                    {
                        ClaimType = claim.Type,
                        ClaimValue = claim.Value
                    });
                }
            }

            return Task.FromResult(0);
        }

        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            throw new NotImplementedException();
        }

        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            foreach (var claim in claims)
            {
                user.Claims.RemoveAll(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
            }
        
            return Task.FromResult(0);
        }

        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrEmpty(user.Id))
            {
                throw new InvalidOperationException("user.Id property must be specified before calling CreateAsync");
            }

            using (var session = _documentStore.OpenAsyncSession())
            {
                await session.StoreAsync(user, cancellationToken);

                // This model allows us to lookup a user by name in order to get the id
                var userByName = new IdentityUserByUserName(user.Id, user.UserName);
                await session.StoreAsync(userByName, Util.GetIdentityUserByUserNameId(user.UserName), cancellationToken);

                try
                {
                    await session.SaveChangesAsync(cancellationToken);
                    return IdentityResult.Success;
                }
                catch (ConcurrencyException)
                {
                    return IdentityResult.Failed(_errorDescriber.ConcurrencyFailure());
                }
            }
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            using (var session = _documentStore.OpenAsyncSession())
            {
                try
                {
                    await session.SaveChangesAsync(cancellationToken);
                    return IdentityResult.Success;
                }
                catch (ConcurrencyException)
                {
                    return IdentityResult.Failed(_errorDescriber.ConcurrencyFailure());
                }
            }
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            using (var session = _documentStore.OpenAsyncSession())
            {
                var userByName = await session.LoadAsync<IdentityUserByUserName>(Util.GetIdentityUserByUserNameId(user.UserName),
                    cancellationToken);

                if (userByName != null)
                {
                    session.Delete(userByName);
                }

                session.Delete(user);

                try
                {
                    await session.SaveChangesAsync(cancellationToken);
                    return IdentityResult.Success;
                }
                catch (ConcurrencyException)
                {
                    return IdentityResult.Failed(_errorDescriber.ConcurrencyFailure());
                }
            }
        }

        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            using (var session = _documentStore.OpenAsyncSession())
            {
                return await session.LoadAsync<TUser>(userId, cancellationToken);
            }
        }

        public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            using (var session = _documentStore.OpenAsyncSession())
            {
                var userByName = await session.LoadAsync<IdentityUserByUserName>(
                    Util.GetIdentityUserByUserNameId(normalizedUserName), cancellationToken);

                if (userByName == null)
                {
                    return default(TUser);
                }

                return await FindByIdAsync(userByName.UserId, cancellationToken);
            }
        }
    }
}