using System.Collections.Generic;

namespace S7Test.Core.Entity.Domain
{
    public class Team : IEntity<int>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        //navigation

        public ICollection<Player> Players { get; set; }
    }
}
