using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace CalwayPest.Domain.Data
{
    public class AdminUserDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<AdminUser, Guid> _adminUserRepository;

        public AdminUserDataSeederContributor(IRepository<AdminUser, Guid> adminUserRepository)
        {
            _adminUserRepository = adminUserRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _adminUserRepository.GetCountAsync() > 0)
            {
                return;
            }

            await _adminUserRepository.InsertAsync(
                new AdminUser(
                    id: Guid.NewGuid(),
                    username: "admin",
                    password: "Shamail_1985"
                ),
                autoSave: true
            );
        }
    }
}
