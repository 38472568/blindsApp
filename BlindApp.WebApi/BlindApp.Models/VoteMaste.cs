using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlindApp.Models
{
    public class VoteMaste
    {
        public VoteMaste()
        {
            VoteArts = new List<VoteArt>();

            VoteUsers = new List<VoteUser>();
        }

        public Int32 ID { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public Int32 Hits { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public Int32 UserCount { get; set; }

        public List<VoteArt> VoteArts { get; set; }

        public List<VoteUser> VoteUsers { get; set; }

        public string Status
        {
            get
            {
                if (DateTime.Now < StartTime)
                {
                    return "报名中";
                }
                if (DateTime.Now >= StartTime && DateTime.Now <= EndTime)
                {
                    return "投票中";
                }
                return "结束";
            }
        }
    }
}
