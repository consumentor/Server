using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MovieApp.Models
{
    [MetadataType(typeof(MovieMetadata))]
    [Bind(Include = "Id, Title, Director, DateReleased")]
    public partial class Movie
    {
    }

    public class MovieMetadata
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter the name of the director of this movie.")]
        public string Director { get; set; }

        [DisplayName("Date Released")]
        [Range(typeof(DateTime), "3/22/1895", "3/1/2010",
                ErrorMessage = "Value for {0} must be between {1} and {2}")] // probably want the upper bound to be Now+#ofdays instead
        public DateTime DateReleased { get; set; }
    }
}
