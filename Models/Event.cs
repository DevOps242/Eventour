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

        public enum Status
        {
            // Add items
            Active,
            Inactive
        }

        //FK for Event Details
        public int EventDetailId { get; set; }
        public EventDetail? EventDetail { get; set; }

        //FK for User
        public int UserId { get; set; }
        public User? User { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime createdOn { get; set; }

    }
}

