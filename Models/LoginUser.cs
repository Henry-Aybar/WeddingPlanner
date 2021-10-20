using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace WeddingPlanner.Models
{
    public class LoginUser
    {
        [Key]
        public int UserId {get;set;}

        [Required]
        public string LoginEmail {get;set;}

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="You Need at least 8 charaters for the Password!")]
        public string LoginPassword {get;set;}


        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}