using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get;set;}

        [Required]
        public string WedderOne {get;set;}

        [Required]
        public string WedderTwo {get;set;}

        [Required]
        [NoPastDateTime]
        public DateTime Date {get;set;}

        [Required]
        public string Address {get;set;}

        [Required]
        public int CreatorId {get;set;}

        public User Creator {get;set;}

        public List<Rsvp> Rsvp {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }

    public class NoPastDateTime : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is DateTime)
            {
                DateTime checkMe = (DateTime)value;

                //acctual logic of determing validity
                if(checkMe < DateTime.Now)
                {
                    return new ValidationResult("Hey wait a sec thats in the Past!");
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return new ValidationResult("Thats not Even a Date Time!");
            }
        }
    }
}