using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace testapp.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public bool ViewContent { get; set; }
        public bool EditContent { get; set; }
        public bool AddContent { get; set; }
        public bool DeleteContent { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        public static Permission GetCurrentUserPermissions(string userName, string[] groupNames)
        {
            using (var context = new ApplicationDbContext())
            {
                var currentUser = context.Users.Where(u => u.UserName == userName).Include(i => i.GroupInUsers).FirstOrDefault();
                if (currentUser != null)
                {
                    var permissions = context.Permissions.ToList();
                    if (permissions == null || !(permissions.Count() > 0) || currentUser.GroupInUsers == null || !(currentUser.GroupInUsers.Count() > 0)) return null;

                    permissions = permissions.Where(p => currentUser.GroupInUsers.Where(u => groupNames.Contains(u.Group.GroupName)).Select(g => g.GroupId).Contains(p.GroupId)).ToList();
                    if (permissions.Count() == 0) return null;
                    return new Permission
                    {
                        ViewContent = permissions.Any(p => p.ViewContent),
                        EditContent = permissions.Any(p => p.EditContent),
                        AddContent = permissions.Any(p => p.AddContent),
                        DeleteContent = permissions.Any(p => p.DeleteContent)
                    };
                }
                else
                {
                    return null;
                }
            }
        }
    }
}