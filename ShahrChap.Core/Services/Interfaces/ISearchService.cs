using ShahrChap.Core.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface ISearchService
    {
        SearchResultViewModel Search(string? q, int? groupId, string? sort, int page);
        SuggestResultDto Suggest(string? q);
    }
}
