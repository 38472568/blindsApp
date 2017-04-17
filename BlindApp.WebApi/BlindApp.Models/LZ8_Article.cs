using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlindApp.Models
{
    [Table(Name = "LZ8_Article")]
    public class Article
    {
        [Key]
        public int ID { get; set; }

        public string Title { get; set; }

        public string Intro { get; set; }

        public int Hits { get; set; }

        public int IsGood { get; set; }

        public string Content { get; set; }

        public string IndexPicUrl { get; set; }

        public string AuthorName { get; set; }
    }
}
