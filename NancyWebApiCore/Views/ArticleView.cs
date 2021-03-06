﻿using System;
using Newtonsoft.Json;

namespace NancyWebApiCore.Views
{
    public class ArticleView
    {
        [JsonProperty("heading")] public string Heading { get; set; }

        [JsonProperty("updated")] public DateTime Updated { get; set; }

        [JsonProperty("link")] public string Link { get; set; }
    }
}