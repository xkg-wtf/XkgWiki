using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XkgWiki.Models.Pages
{
    public class CreatePageRequestModel
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}
