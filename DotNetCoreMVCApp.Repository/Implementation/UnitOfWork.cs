using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCoreMVCApp.Models.Repository;

namespace DotNetCoreMVCApp.Repository.Implementation
{
    public class UnitOfWork : IDisposable
    {
        private ApplicationDbContext _context;
        private GenericRepository<Country> _countryRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

        }
        public GenericRepository<Country> CountryRepository
        {
            get
            {

                if (_countryRepository == null)
                {
                    _countryRepository = new GenericRepository<Country>(_context);
                }
                return _countryRepository;
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
