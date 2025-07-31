

using ASP.NETcoreMVC.Models;

namespace ASP.NETcoreMVC.Services.Interface
{
    public interface IpgAdoNetService
    {
        Task<IEnumerable<string>> GetNamesAsync();

        Task<IEnumerable<User>> GetUsersAsync();

    }
}
