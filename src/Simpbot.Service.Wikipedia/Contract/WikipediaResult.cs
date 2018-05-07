using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Simpbot.Service.Wikipedia.Contract
{
    public class WikipediaResult
    {
        [JsonProperty("batchcomplete")]
        public string Batchcomplete { get; set; }

        [JsonProperty("continue")]
        public Continue Continue { get; set; }

        [JsonProperty("query")]
        public Query Query { get; set; }
    }

    public class Continue
    {
        [JsonProperty("sroffset")]
        public long Sroffset { get; set; }

        [JsonProperty("continue")]
        public string ContinueContinue { get; set; }
    }

    public class Query
    {
        [JsonProperty("searchinfo")]
        public Searchinfo Searchinfo { get; set; }

        [JsonProperty("search")]
        public List<Search> Search { get; set; }
    }

    public class Search
    {
        [JsonProperty("ns")]
        public long Ns { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("pageid")]
        public long Pageid { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("wordcount")]
        public long Wordcount { get; set; }

        [JsonProperty("snippet")]
        public string Snippet { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    public class Searchinfo
    {
        [JsonProperty("totalhits")]
        public long Totalhits { get; set; }
    }
}
