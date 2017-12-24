using S7Test.Core.Entity.Domain;
using S7Test.Core.Interface.Common;
using S7Test.Core.Interface.Service.Domain;
using S7Test.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace S7Test.Core.Service
{
    public class PlayerService : IPlayerService, IDisposable
    {
        IUnitOfWork _unitOfWork;
        public PlayerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            //Clean up managable resources
            if (disposing)
            {
                if (_unitOfWork != null)
                {
                    _unitOfWork.Dispose();
                    _unitOfWork = null;
                }
            }
            //Clean up unmanagable resources

        }
        

        public IEnumerable<Player> GetPlayersByFilterModel(PlayerFilterModel model, out int pageCount)
        {
            Func<IQueryable<Player>, IOrderedQueryable<Player>> orderBy(OrderBy ob)
            {
                if (ob == null) return null;

                if (ob.Prop == "name" && ob.Asc == true) return o => o.OrderBy(e => e.Name);
                else if (ob.Prop == "name" && ob.Asc == false) return o => o.OrderByDescending(e => e.Name);

                else if (ob.Prop == "isActive" && ob.Asc == true) return o => o.OrderBy(e => e.IsActive);
                else if (ob.Prop == "isActive" && ob.Asc == false) return o => o.OrderByDescending(e => e.IsActive);

                else if (ob.Prop == "age" && ob.Asc == true) return o => o.OrderBy(e => e.Age);
                else if (ob.Prop == "age" && ob.Asc == false) return o => o.OrderByDescending(e => e.Age);

                else if (ob.Prop == "yellowCards" && ob.Asc == true) return o => o.OrderBy(e => e.YellowCards);
                else if (ob.Prop == "yellowCards" && ob.Asc == false) return o => o.OrderByDescending(e => e.YellowCards);

                else if (ob.Prop == "redCards" && ob.Asc == true) return o => o.OrderBy(e => e.RedCards);
                else if (ob.Prop == "redCards" && ob.Asc == false) return o => o.OrderByDescending(e => e.RedCards);

                else if (ob.Prop == "goals" && ob.Asc == true) return o => o.OrderBy(e => e.Goals);
                else if (ob.Prop == "goals" && ob.Asc == false) return o => o.OrderByDescending(e => e.Goals);

                else if (ob.Prop == "appearances" && ob.Asc == true) return o => o.OrderBy(e => e.Appearances);
                else if (ob.Prop == "appearances" && ob.Asc == false) return o => o.OrderByDescending(e => e.Appearances);

                else if (ob.Prop == "gender" && ob.Asc == true) return o => o.OrderBy(e => e.Gender);
                else if (ob.Prop == "gender" && ob.Asc == false) return o => o.OrderByDescending(e => e.Gender);

                else if (ob.Prop == "team" && ob.Asc == true) return o => o.OrderBy(e => e.Team.Name);
                else if (ob.Prop == "team" && ob.Asc == false) return o => o.OrderByDescending(e => e.Team.Name);

                else return null;
            }
            Expression<Func<Player, bool>> filter(string keyword)
            {
                if (model.Keyword == null) return null;

                return e => e.Name.StartsWith(model.Keyword, StringComparison.InvariantCultureIgnoreCase)
                               || e.Team.Name.StartsWith(model.Keyword, StringComparison.InvariantCultureIgnoreCase);
            };
            var includeUsefulProps = new List<Expression<Func<Player, object>>> { e => e.Team };

            pageCount = _unitOfWork.Repository<Player, int>().GetList(filter(model.Keyword), tracking: false).Count() / model.pageSize;
            return _unitOfWork.Repository<Player, int>().GetList(
                            filter(model.Keyword),
                            orderBy(model.OrderBy),
                            includeUsefulProps,
                            page: model.Page, pageSize: model.pageSize, tracking: false);
        }
        public IEnumerable<Team> GetTeamOrList(int teamId)
        {
            //var includeUsefulProps = new List<Expression<Func<Team, object>>> { t => t.Players };
            return _unitOfWork.Repository<Team, int>()
                .GetList(e => teamId == 0 || e.Id == teamId, o => o.OrderBy(e => e.Name), tracking: false);
        }
        public bool DeletePlayerById(int playerId)
        {
            _unitOfWork.Repository<Player, int>().Delete(playerId);
            return _unitOfWork.Save();
        }

        public int AddOrUpdatePlayer(Player player)
        {
            if (player.Id > 0) _unitOfWork.Repository<Player, int>().Update(player);
            else if(player.Id == 0) _unitOfWork.Repository<Player, int>().Create(player);

            return _unitOfWork.Save() ? player.Id : 0;
        }
    }
}
