using AutoMapper;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCoreMVCApp.Models.Repository;
using DotNetCoreMVCApp.Models.Web;
using DotNetCoreMVCApp.Repository.Implementation;
using DotNetCoreMVCApp.Service.Abstraction;

namespace DotNetCoreMVCApp.Service.Implementation
{
    public class CountryService : ICountryService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILog _logger;

        public CountryService(UnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = LogManager.GetLogger(typeof(CountryService));
        }

        public async Task<IEnumerable<CountryViewModel>> GetAllAsync()
        {
            return _mapper.Map<List<CountryViewModel>>(await _unitOfWork.CountryRepository.GetAsync(filter: (c => c.IsDeleted == false), (c => c.OrderBy(a => a.Name))));
        }

        public async Task<CountryViewModel> GetByIdAsync(int id)
        {
            return _mapper.Map<CountryViewModel>(await _unitOfWork.CountryRepository.GetByIdAsync(id));
        }

        public async Task<bool> CreateAsync(CountryViewModel countryModel, string userId)
        {
            _logger.Info($"Country create request by user: {userId} : {JsonConvert.SerializeObject(countryModel)}");
            var country = _mapper.Map<Country>(countryModel);
            country.CreatedBy = userId;
            country.CreatedOn = DateTime.Now;
            await _unitOfWork.CountryRepository.InsertAsync(country);
            await _unitOfWork.SaveAsync();
            _logger.Info($"Created country");
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            _logger.Info($"Country delete request by user: {userId} : Country Id: {id}");
            var country = await _unitOfWork.CountryRepository.GetByIdAsync(id);
            country.IsDeleted = true;
            country.DeletedBy = userId;
            country.DeletedOn = DateTime.Now;

            _unitOfWork.CountryRepository.Update(country);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(CountryViewModel countryModel, string userId)
        {
            _logger.Info($"Country update request by user: {userId} : {JsonConvert.SerializeObject(countryModel)}");
            var country = await _unitOfWork.CountryRepository.GetByIdAsync(countryModel.Id);
            country.Code = countryModel.Code;
            country.Name = countryModel.Name;
            country.UpdatedBy = userId;
            country.UpdatedOn = DateTime.Now;
            _unitOfWork.CountryRepository.Update(country);
            await _unitOfWork.SaveAsync();
            _logger.Info($"Created country");
            return true;
        }

        public async Task<ErrorStateModel> ValidateAsync(CountryViewModel countryModel)
        {
            //Check if country with same name or code exists
            ErrorStateModel errorStateModel = new();

            errorStateModel.IsValid = !(await _unitOfWork.CountryRepository.GetAsync(filter: (c => c.Id != countryModel.Id && c.IsDeleted == false && (c.Name == countryModel.Name || c.Code == countryModel.Code)))).Any();
            if (!errorStateModel.IsValid)
            {
                errorStateModel.Errors.Add("Country", "Country with same code or name exists.");
            }
            return errorStateModel;
        }
    }
}
