using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ShahrChap.Core.DTOs.Products
{
    // ===== مدل ویوی صفحه‌ی نتایج (/Search) =====
    public class SearchResultViewModel
    {
        public string Query { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageCount { get; set; } = 1;
        public string Sort { get; set; } = "relevant";
        public int? GroupId { get; set; }
        public List<SearchGroupFilter> Groups { get; set; } = new();
        public List<SearchProductItem> Products { get; set; } = new();
    }

    public class SearchGroupFilter
    {
        public int GroupId { get; set; }
        public string GroupTitle { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class SearchProductItem
    {
        public int ProductId { get; set; }
        public string ProductTitle { get; set; } = string.Empty;
        public string GroupTitle { get; set; } = string.Empty;
        public string? ImageName { get; set; }
    }

    // ===== خروجی JSON اتوکامپلیت (/Search/Suggest) =====
    public class SuggestResultDto
    {
        [JsonPropertyName("query")] public string Query { get; set; } = string.Empty;
        [JsonPropertyName("total")] public int Total { get; set; }
        [JsonPropertyName("groups")] public List<SuggestGroupDto> Groups { get; set; } = new();
        [JsonPropertyName("products")] public List<SuggestProductDto> Products { get; set; } = new();
    }

    public class SuggestGroupDto
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; } = string.Empty;
        [JsonPropertyName("count")] public int Count { get; set; }
        [JsonPropertyName("url")] public string Url { get; set; } = "#";
    }

    public class SuggestProductDto
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; } = string.Empty;
        [JsonPropertyName("group")] public string Group { get; set; } = string.Empty;
        [JsonPropertyName("image")] public string? Image { get; set; }
        [JsonPropertyName("url")] public string Url { get; set; } = "#";
    }
}