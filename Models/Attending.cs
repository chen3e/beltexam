using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
namespace BeltExam.Models
    {
        public class Attending
        {
            [Key]
            public int AttendingId {get;set;}
            public int UserId {get;set;}
            public User User {get;set;}
            public int EventId {get;set;}
            public Event Event {get;set;}
        }
    }