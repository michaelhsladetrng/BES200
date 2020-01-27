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

        public async Task<bool> UpdateGenreFor(int id, string genre)
        {
            var book = await GetBooksInInventory().SingleOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return false;
            }
            else
            {
                book.Genre = genre;
                await Context.SaveChangesAsync();

                return true;
            }

        }

        public async Task Remove(int id)
        {
            var book = await GetBooksInInventory().SingleOrDefaultAsync(b => b.Id == id);
            if (book != null)
            {
                book.InInventory = false;
                await Context.SaveChangesAsync();
            }

        }

        public async Task<GetBookDetailsResponse> Add(PostBooksRequest bookToAdd)
        {
            // Add it to the domain.
            //  - PostBooksRequest -> Book
            //var book = new Book
            //{
            //    Title = bookToAdd.Title,
            //    Author = bookToAdd.Author,
            //    Genre = bookToAdd.Genre,
            //    NumberOfPages = bookToAdd.NumberOfPages,
            //    InInventory = true
            //};
            var book = Mapper.Map<Book>(bookToAdd);

            //  - Add it to the Context.
            Context.Books.Add(book);

            //  - Have the context save everything.
            await Context.SaveChangesAsync();


            // Return a 201 Created Status Code.
            //  - Add a location header on the response e.g. Location: http://server/books/8
            //  - Add the entity
            //  - Book -> Get BooksDetailResponse

            //var response = new GetBookDetailsResponse
            //{
            //    Id = book.Id,
            //    Title = book.Title,
            //    Author = book.Author,
            //    Genre = book.Genre,
            //    NumberOfPages = book.NumberOfPages
            //};

            return Mapper.Map<GetBookDetailsResponse>(book);

           // return response;
        }

        public async Task<GetBooksResponse> GetBooks(string genre)
        {
            var books = GetBooksInInventory();

            if (genre != "all")
            {
                books = books.Where(b => b.Genre == genre);
            }

            var booksListItems = await books.Select(b =>
            Mapper.Map<BookSummaryItem>(b))
                .AsNoTracking()
                .ToListAsync();

            //new BookSummaryItem
            //{
            //    Id = b.Id,
            //    Title = b.Title,
            //    Author = b.Author,
            //    Genre = b.Genre
            //}).ToListAsync();

            var response = new GetBooksResponse
            {
                Data = booksListItems,
                Genre = genre,
                Count = booksListItems.Count()
            };

            return response;
        }
    }
}
