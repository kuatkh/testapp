using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace testapp.Models
{
    public class GroupInUsers
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string UserId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }
    }
}