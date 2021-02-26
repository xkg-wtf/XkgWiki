using System;
using System.ComponentModel.DataAnnotations;
using Att.Domain.Shared;

namespace XkgWiki.Domain
{
    public class Page : EntityBase
    {
        [Required]
        public string Url { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }
    }
}