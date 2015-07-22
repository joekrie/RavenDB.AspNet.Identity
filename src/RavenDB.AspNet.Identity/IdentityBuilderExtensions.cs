using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.DependencyInjection;
using Raven.Client;

namespace RavenDB.AspNet.Identity
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddRavenStores(this IdentityBuilder builder)
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

        public static IDocumentStore ConfigureDocumentStoreForIdentity(this IDocumentStore documentStore)
        {
            documentStore.Conventions
                .RegisterAsyncIdConvention<IdentityUser>((dbname, commands, user) => Task.FromResult("IdentityUsers/" + user.Id))
                .RegisterAsyncIdConvention<IdentityRole>((dbname, commands, role) => Task.FromResult("IdentityRoles/" + role.Id));

            return documentStore;
        }
    }
}