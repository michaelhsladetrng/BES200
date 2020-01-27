﻿using LibraryApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApi.Mappers
{
    public interface IMapBooks
    {
        Task<GetBookDetailsResponse> GetBookById(int id);
    }
}
