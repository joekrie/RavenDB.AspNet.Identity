using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.DependencyInjection;
using IServiceCollection = Microsoft.Framework.DependencyInjection.IServiceCollection;

namespace RavenDB.AspNet.Identity
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddRavenDBStores(this IdentityBuilder builder)
        {
            var userStoreType = typeof (UserStore<>).MakeGenericType(builder.UserType);
            var iUserStoreType = typeof (IUserStore<>).MakeGenericType(builder.UserType);

            var roleStoreType = typeof (RoleStore<>).MakeGenericType(builder.RoleType);
            var iRoleStoreType = typeof (IRoleStore<>).MakeGenericType(builder.RoleType);

            builder.Services
                .AddScoped(iUserStoreType, userStoreType)
                .AddScoped(iRoleStoreType, roleStoreType);

            return builder;
        }
    }
}
