using S7Test.Core.Entity.Domain;
using System.Collections.Generic;

namespace S7Test.Web.Models
{
    public class PagingViewModel
    {
        public int PageCount { get; set; }
        public IEnumerable<Player> Data { get; set; }
    }
}
