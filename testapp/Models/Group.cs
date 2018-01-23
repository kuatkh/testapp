using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace testapp.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
    }
}