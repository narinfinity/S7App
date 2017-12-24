using System;
using System.Linq;
using System.Linq.Expressions;
using S7Test.Core.Entity.Domain;
using S7Test.Core.Interface.Common;

namespace S7Test.Core.Interface.Repository
{
    public interface IPlayerRepository : IRepository<Player, int>
    {        
        IQueryable<Player> GetListByPredicate(Expression<Func<Player, bool>> predicate = null);
    }
}
