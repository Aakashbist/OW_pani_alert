﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HT.Overwatch.Application.ResponseModels.QuickLinkResponse
{
    public class QuickLinkResponse
    {
        public int Id { get; set; }
        public int? SiteId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
