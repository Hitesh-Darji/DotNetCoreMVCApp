using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCoreMVCApp.Models.Repository;
using DotNetCoreMVCApp.Models.Web;

namespace DotNetCoreMVCApp.Service.Abstraction
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryViewModel>> GetAllAsync();
        Task<CountryViewModel> GetByIdAsync(int id);
        Task<ErrorStateModel> ValidateAsync(CountryViewModel country);
        Task<bool> CreateAsync(CountryViewModel country, string userId);
        Task<bool> UpdateAsync(CountryViewModel country, string userId);
        Task<bool> DeleteAsync(int id, string userId);
    }
}
