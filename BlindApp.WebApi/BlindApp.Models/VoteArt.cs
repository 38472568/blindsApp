using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlindApp.Models
{
    public class VoteArt
    {
        public int id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int Hits { get; set; }

        public int Ord { get; set; }
    }
}
