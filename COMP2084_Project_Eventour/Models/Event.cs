using System;
using System.ComponentModel.DataAnnotations;

namespace COMP2084_Project_Eventour.Models
{
    public class Event
    {
        public int EventId {get; set;}

        [Required]
        [MaxLength(70, ErrorMessage = "Event Title must be less than 70 characters.")]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        public string? Photo { get; set; }

        //public enum EventStatus
        //{
        //    // Add items
        //    Active,
        //    Inactive
        //}

        //public EventStatus Status { get; set; }

        //FK for Event Details
        public int EventDetailId { get; set; }
        public EventDetail? EventDetail { get; set; }

        //FK for User
        public int UserId { get; set; }
        public User? User { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created")]
        [DisplayFormat(DataFormatString = "{0:dd MMMM, yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime createdOn { get; set; }

    }
}

