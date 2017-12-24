using S7Test.Core.Model.Enum;

namespace S7Test.Core.Entity.Domain
{
   public class Player : IEntity<int>
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string Picture { get; set; }
        public int Age { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
        public int Goals { get; set; }
        public int Appearances { get; set; }
        public string Name { get; set; }
        public Gender? Gender { get; set; }

        //Navigation
        public int? TeamId { get; set; }
        public Team Team { get; set; }
    }
}
