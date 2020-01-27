using LibraryApi.Domain;
using LibraryApi.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace LibraryApi.Mappers
{
    public class EfBookMapper : IMapBooks
    {
        LibraryDataContext Context;
        IMapper Mapper;

        public EfBookMapper(LibraryDataContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }
        
        private IQueryable<Book> GetBooksInInventory()
        {
            return Context.Books.Where(b => b.InInventory);
        }

        public async Task<GetBookDetailsResponse> GetBookById(int id)
        {
            //return await GetBooksInInventory()
            //    .Where(b => b.Id == id)
            //    .Select(b => new GetBookDetailsResponse
            //    {
            //        Id = b.Id,
            //        Title = b.Title,
            //        Author = b.Author,
            //        Genre = b.Genre,
            //        NumberOfPages = b.NumberOfPages
            //    }).SingleOrDefaultAsync();

            return await GetBooksInInventory()
                .Where(b => b.Id == id)
                //.AsNoTracking()
                .Select(b => Mapper.Map<GetBookDetailsResponse>(b)).SingleOrDefaultAsync();

        }
    }
}
