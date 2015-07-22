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
    public partial class UserStore<TUser> : IQueryableUserStore<TUser> where TUser : IdentityUser
    {
        private bool _disposed;
            
        private readonly IdentityErrorDescriber _errorDescriber;  // allows for custom error messages if injected
        private readonly IDocumentStore _documentStore;
        
        public UserStore(IDocumentStore documentStore, IdentityErrorDescriber errorDescriber = null)
        {
            _errorDescriber = errorDescriber ?? new IdentityErrorDescriber();
            _documentStore = documentStore;
        }

        public bool AutoSaveChanges { get; set; } = true;

        public IQueryable<TUser> Users
        {
            get
            {
                throw new NotImplementedException();
            }
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