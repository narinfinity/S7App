using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using S7Test.Core.Entity.Domain;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace S7Test.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AppDbContext context)
        {
            // Look for any students.
            if (context.Players.Any())
            {
                return;   // DB has been seeded
            }

            await context.AddRangeAsync(
                new IdentityRole<string>("User") { NormalizedName = "USER" },
                new IdentityRole<string>("Admin") { NormalizedName = "ADMIN" }
            );
            
            var stream = typeof(DbInitializer).GetTypeInfo().Assembly.GetManifestResourceStream("S7Test.Infrastructure.Data.players.json");
            using (var reader = new StreamReader(stream))
            {
                var players = JsonConvert.DeserializeObject<List<Player>>(reader.ReadToEnd());

                var teams = players.Select(p => p.Team).GroupBy(e => e.Id).Select(g => g.First()).ToList();
                teams.ForEach(t => { t.Id = 0; });
                context.Teams.AddRange(teams);
                context.SaveChanges();
                teams = context.Teams.ToList();

                players.ForEach(p => {
                    p.Id = 0;
                    p.Team = teams.First(t => t.Name == p.Team.Name);
                });

                context.Players.AddRange(players);
                context.SaveChanges();
            }

        }
    }
}
