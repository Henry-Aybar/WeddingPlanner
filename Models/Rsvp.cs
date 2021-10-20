using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace WeddingPlanner.Models
{
    public class Rsvp
    {
        [Key]
        public int RsvpId {get;set;}

        [Required]
        public int UserId {get;set;}

        public User User {get;set;}

        [Required]
        public int WeddingId {get; set;}

        public Wedding Wedding {get;set;}


        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}