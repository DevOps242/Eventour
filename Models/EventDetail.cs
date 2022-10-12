using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP2084_Project_Eventour.Models
{
    public class EventDetail
    {

        public int EventDetailId {get; set;}

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Range(0, 10000, ErrorMessage = "Price of an event can be 0 (free) - 10,000")]
        public double Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMMM, yyyy hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMMM, yyyy hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime EndDate { get; set; }
        
        [Required]
        // FK for Category item
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        // FK for EventVenue item
        public int EventVenueId { get; set; }
        public EventVenue? EventVenue { get; set; }

        // add nullable child ref to Event model
        public List<Event>? Events { get; set; }

    }
}

