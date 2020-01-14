using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Retrospective_Core.Models;

namespace Retrospective_EFSQLRetrospectiveDbImpl.Seeds
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            await using var context = new RetroSpectiveDbContext(

                serviceProvider.GetRequiredService<DbContextOptions<RetroSpectiveDbContext>>());
            if (context.Retrospectives.Any(x => x.Title == "Training 1"))
            {
                return;   // DB has been seeded
            }

            var user = new RetroUser
            {
                Email = "henk@mail.com",
                UserName = "henk@mail.com",
                LockoutEnabled = false
            };

            var password = "Superhelder123!";

            var usrMgr = serviceProvider.GetRequiredService<UserManager<RetroUser>>();

            await usrMgr.CreateAsync(user, password);

            context.SaveChanges();

            var result = usrMgr.Users.FirstOrDefault(u => u.Email == user.Email);

            Retrospective retrospective = new Retrospective
            {
                CreatedDate = new DateTime(),
                Description = "Welkom bij de eerste retro training",
                Title = "Training 1",
                RetroUser = result,
                RetroUserId = result.Id
            };

            RetroColumn retroColumn1 = new RetroColumn { Title = "Dit is collom 1" };
            RetroColumn retroColumn2 = new RetroColumn { Title = "Dit is collom 2" };
            RetroColumn retroColumn3 = new RetroColumn { Title = "Dit is collom 3" };

            RetroFamily retroFamily1 = new RetroFamily { Content = "Dit is familie 1" };

            RetroCard retroCard1 = new RetroCard { Content = "Ik sta in collom 1 als het goed is" };

            RetroCard retroCard2 = new RetroCard { Content = "Ik sta in collom 2 als het goed is" };

            RetroCard retroCard3 = new RetroCard { Content = "Ik sta in collom 3 als het goed is" };

            RetroCard retroCard4 = new RetroCard { Content = "Ik sta in collom 3 als het goed is", Position = 1};

            retroColumn1.RetroCards.Add(retroCard1);

            retrospective.RetroColumns.Add(retroColumn1);
            retrospective.RetroColumns.Add(retroColumn2);
            retrospective.RetroColumns.Add(retroColumn3);

            context.Retrospectives.Add(retrospective);

            context.SaveChanges();
        }
    }
}