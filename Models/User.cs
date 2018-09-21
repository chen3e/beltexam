using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
namespace BeltExam.Models
    {
        public class User
    	{
            [Key]
            public int UserId {get;set;}
            [Required(ErrorMessage = "First name required")]
            [MinLength(2, ErrorMessage = "First name must be at least 2 characters")]
            [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Invalid first name")]
            public string FName {get;set;}
            [Required(ErrorMessage = "Last name required")]
            [MinLength(2, ErrorMessage = "Last name must be at least 2 characters")]
            [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Invalid last name")]
            public string LName {get;set;}
            [Required(ErrorMessage = "You must enter an email address")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email {get;set;}
            [Required(ErrorMessage = "You must enter a password")]
            [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
            [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[\\]*+\\/|!\"£$%^&*()#[@~'?><,.=_-]).{6,}$", ErrorMessage = "Password must include at least one upper case character, lower case character, special character, and number")]
            public string Password {get;set;}
            [NotMapped]
            [Compare("Password", ErrorMessage = "Passwords do not match")]
            public string ConfirmPassword {get;set;}
            public DateTime CreatedAt {get;set;}
            public DateTime UpdatedAt {get;set;}
            public List<Event> CreatedEvents {get;set;}
            public List<Attending> AttendingEvents {get;set;}

            public User()
            {
                CreatedEvents = new List<Event>();
                AttendingEvents = new List<Attending>();
            }
    	}
    }