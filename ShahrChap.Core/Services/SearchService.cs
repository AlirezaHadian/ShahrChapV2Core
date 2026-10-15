using Microsoft.EntityFrameworkCore;
using ShahrChap.Core.DTOs.Products;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ShahrChap.Core.Services
{
    public class SearchService : ISearchService
    {
        private const int PageSize = 12;
        private const int MaxQueryLength = 100;
        private const string ThumbFolder = "/product/thumb";

        private readonly ShahrChapContext _context;
        public SearchService(ShahrChapContext context) => _context = context;

        private sealed class Row
        {
            public int ProductId { get; set; }
            public string Title { get; set; } = "";
            public string? Image { get; set; }
            public int? GroupId { get; set; }
            public string GroupTitle { get; set; } = "";
            public long? Price { get; set; }
        }

        private IQueryable<Row> BaseQuery()
        {
            return from p in _context.Products.AsNoTracking()
                   where p.ParentId == null && !p.IsDelete
                   join g in _context.ProductGroups.AsNoTracking()
                        on (int?)p.SubGroupId equals (int?)g.GroupId into gj
                   from g in gj.DefaultIfEmpty()
                   select new Row
                   {
                       ProductId = p.ProductId,
                       Title = p.ProductTitle,
                       Image = p.Image,
                       GroupId = p.SubGroupId,
                       GroupTitle = g != null ? g.GroupTitle : "",
                       Price = null
                   };
        }

        private static string Normalize(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var s = input.Replace('ي', 'ی').Replace('ك', 'ک').Replace('\u200c', ' ');
            s = Regex.Replace(s, @"\s+", " ").Trim();
            return s.Length > MaxQueryLength ? s.Substring(0, MaxQueryLength) : s;
        }

        private static string[] Words(string q) =>
            q.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(5).ToArray();

        private static IQueryable<Row> Filter(IQueryable<Row> source, string[] words)
        {
            foreach (var w in words)
            {
                var word = w;
                source = source.Where(x => x.Title.Contains(word) || x.GroupTitle.Contains(word));
            }
            return source;
        }

        private static string? ImageUrl(string? file) =>
            string.IsNullOrWhiteSpace(file) ? null : ThumbFolder + file;

        public SearchResultViewModel Search(string? q, int? groupId, string? sort, int page)
        {
            q = Normalize(q);
            var sortKey = (sort == "cheap" || sort == "expensive") ? sort : "new";

            var vm = new SearchResultViewModel
            {
                Query = q,
                GroupId = groupId,
                Sort = sortKey!,
                Page = Math.Max(page, 1)
            };

            var words = Words(q);
            if (words.Length == 0) return vm;

            var matched = Filter(BaseQuery(), words);


            vm.Groups = matched
                .Where(x => x.GroupId != null)
                .GroupBy(x => new { x.GroupId, x.GroupTitle })
                .Select(g => new SearchGroupFilter
                {
                    GroupId = g.Key.GroupId.GetValueOrDefault(),
                    GroupTitle = g.Key.GroupTitle,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .ToList();

            if (groupId.HasValue)
                matched = matched.Where(x => x.GroupId == groupId);


            matched = sortKey switch
            {
                "cheap" => matched.OrderBy(x => x.Price == null).ThenBy(x => x.Price).ThenByDescending(x => x.ProductId),
                "expensive" => matched.OrderByDescending(x => x.Price).ThenByDescending(x => x.ProductId),
                _ => matched.OrderByDescending(x => x.ProductId)
            };

            vm.TotalCount = matched.Count();
            vm.PageCount = Math.Max(1, (int)Math.Ceiling(vm.TotalCount / (double)PageSize));
            vm.Page = Math.Min(vm.Page, vm.PageCount);

            vm.Products = matched
                .Skip((vm.Page - 1) * PageSize)
                .Take(PageSize)
                .ToList()
                .Select(x => new SearchProductItem
                {
                    ProductId = x.ProductId,
                    ProductTitle = x.Title,
                    GroupTitle = x.GroupTitle,
                    ImageName = x.Image,
                    Price = x.Price
                })
                .ToList();

            return vm;
        }

        public SuggestResultDto Suggest(string? q)
        {
            q = Normalize(q);
            var result = new SuggestResultDto { Query = q };

            var words = Words(q);
            if (q.Length < 2 || words.Length == 0) return result;

            var matched = Filter(BaseQuery(), words);
            result.Total = matched.Count();
            if (result.Total == 0) return result;

            var first = words[0];
            result.Products = matched
                .OrderBy(x => x.Title.StartsWith(first) ? 0 : 1)
                .ThenByDescending(x => x.ProductId)
                .Take(5)
                .ToList()
                .Select(x => new SuggestProductDto
                {
                    Id = x.ProductId,
                    Title = x.Title,
                    Group = x.GroupTitle,
                    Image = ImageUrl(x.Image),
                    Price = x.Price
                })
                .ToList();

            result.Groups = matched
                .Where(x => x.GroupId != null)
                .GroupBy(x => new { x.GroupId, x.GroupTitle })
                .Select(g => new SuggestGroupDto
                {
                    Id = g.Key.GroupId.GetValueOrDefault(),
                    Title = g.Key.GroupTitle,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .Take(3)
                .ToList();

            return result;
        }
    }
}
