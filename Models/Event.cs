using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
namespace BeltExam.Models
    {
        public class Event
        {
            [Key]
            public int EventId {get;set;}
            [Required(ErrorMessage = "This field is required")]
            [MinLength(2, ErrorMessage = "Activity name must be at least 2 characters")]
            public string EventName {get;set;}
            [Required(ErrorMessage = "This field is required")]
            [MinLength(10, ErrorMessage = "Description must be at least 10 characters")]
            public string Description {get;set;}
            [Required(ErrorMessage = "This field is required")]
            public DateTime StartingTime {get;set;}
            [NotMapped]
            public DateTime Date {get;set;}
            [Required(ErrorMessage = "This field is required")]
            [NotMapped]
            public TimeSpan Time {get;set;}
            [Required(ErrorMessage = "This field is required")]
            [Range(1, 4294967295, ErrorMessage = "This number must be at least 0")]
            public Int32 DurationNumber {get;set;}
            [Required(ErrorMessage = "This field is required")]
            public String DurationType {get;set;}

            public DateTime EndingTime {get;set;}
            public int CoordinatorId {get;set;}
            public User Coordinator {get;set;}
            public List<Attending> AttendingUsers {get;set;}

            public Event(){
                AttendingUsers = new List<Attending>();
            }
        }
    }