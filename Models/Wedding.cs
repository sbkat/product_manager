using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace wedding_planner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId { get; set; }
        [Display (Name="Wedder One: ")]
        [Required (ErrorMessage = "Enter name of Wedder One.")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters.")]
        public string WedderOne { get; set; }
        [Display (Name="Wedder Two: ")]
        [Required (ErrorMessage = "Enter name of Wedder Two.")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters.")]
        public string WedderTwo { get; set; }
        [Display (Name="Date: ")]
        [Required (ErrorMessage = "Enter date of wedding.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Display (Name="Wedding Address: ")]
        [Required (ErrorMessage = "Enter address of wedding.")]
        public string Address { get; set; }
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}