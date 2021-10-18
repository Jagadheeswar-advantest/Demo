using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace democloudapplication.Models
{
    public class Logins
    {
        [Required]
        public string ServerName { get; set; }
        [Required]
        public string LoginID { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Database { get; set; }
    }
    public class Pagingutility
    {
        public string Pages { get; set; }
        public string Record { get; set; }
        public int? TotalPages { get; set; }
        public int? PageNumber { get; set; }
        public int? RecordCount { get; set; }
    }
}