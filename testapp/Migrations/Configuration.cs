namespace testapp.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using testapp.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var manager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(
                    new ApplicationDbContext()));
            
            var newUsers = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName="Pushkin",
                    FirstName="А",
                    LastName="Пушкин",
                    Email="test@test.test",
                    Created=DateTime.Now,
                    Modify=DateTime.Now
                },
                new ApplicationUser
                {
                    UserName="Lermantov",
                    FirstName="М",
                    LastName="Лермантов",
                    Email="test2@test.test",
                    Created=DateTime.Now,
                    Modify=DateTime.Now
                }
            };
            newUsers.ForEach(ns => manager.Create(ns, ns.UserName== "Pushkin" ? "Natasha#17" : "Kirka#18"));
            //context.SaveChanges();

            var newGroups = new List<Group> {
                new Group{
                    GroupName="Administrator"
                },
                new Group{
                    GroupName="Archivarius"
                }
            };
            newGroups.ForEach(ns => context.Groups.AddOrUpdate(p => p.GroupName, ns));
            context.SaveChanges();

            if (context.Users.Where(u => u.UserName == "Pushkin").FirstOrDefault().Id != null && newGroups.FirstOrDefault(u => u.GroupName == "Administrator").Id != 0)
            {
                var GroupInUsersAdministratorDb = context.GroupInUsers.Where(
                        ns => ns.UserId == context.Users.Where(u => u.UserName == "Pushkin").FirstOrDefault().Id).SingleOrDefault();
                if (GroupInUsersAdministratorDb == null)
                {
                    context.GroupInUsers.Add(new GroupInUsers { GroupId = newGroups.FirstOrDefault(u => u.GroupName == "Administrator").Id, UserId = context.Users.Where(u => u.UserName == "Pushkin").FirstOrDefault().Id });
                }
            }

            if (context.Users.Where(u => u.UserName == "Lermantov").FirstOrDefault().Id != null && newGroups.FirstOrDefault(u => u.GroupName == "Archivarius").Id != 0)
            {
                var GroupInUsersArchivariusDb = context.GroupInUsers.Where(
                        ns => ns.UserId == context.Users.Where(u => u.UserName == "Lermantov").FirstOrDefault().Id).SingleOrDefault();
                if (GroupInUsersArchivariusDb == null)
                {
                    context.GroupInUsers.Add(new GroupInUsers { GroupId = newGroups.FirstOrDefault(u => u.GroupName == "Archivarius").Id, UserId = context.Users.Where(u => u.UserName == "Lermantov").FirstOrDefault().Id });
                }
            }

            context.SaveChanges();

            if (newGroups.FirstOrDefault(u => u.GroupName == "Administrator").Id != 0) {
                var newPermissions = new List<Permission>
                {
                    new Permission{ GroupId=newGroups.FirstOrDefault(u => u.GroupName == "Administrator").Id, AddContent=true, DeleteContent=true, EditContent=true, ViewContent=true }
                };
                newPermissions.ForEach(ns => context.Permissions.AddOrUpdate(p => p.GroupId, ns));
            }

            if (newGroups.FirstOrDefault(u => u.GroupName == "Archivarius").Id != 0)
            {
                var newPermissions = new List<Permission>
                {
                    new Permission{ GroupId=newGroups.FirstOrDefault(u => u.GroupName == "Administrator").Id, AddContent=true, DeleteContent=true, EditContent=true, ViewContent=true }
                };
                newPermissions.ForEach(ns => context.Permissions.AddOrUpdate(p => p.GroupId, ns));
            }

            context.SaveChanges();
        }
    }
}
