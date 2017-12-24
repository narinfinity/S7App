using S7Test.Core.Entity.Domain;
using S7Test.Core.Model;
using System.Collections.Generic;

namespace S7Test.Core.Interface.Service.Domain
{
    public interface IPlayerService
    {

        IEnumerable<Team> GetTeamOrList(int teamId);
        bool DeletePlayerById(int playerId);
        IEnumerable<Player> GetPlayersByFilterModel(PlayerFilterModel model, out int pageCount);
        int AddOrUpdatePlayer(Player player);
    }
}
