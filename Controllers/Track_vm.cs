using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment7.Controllers
{
    public class TrackAddForm
    {
        // Attention 03 - Data annotations are important on ALL Track classes

        [Required, StringLength(200)]
        [Display(Name = "Track name")]
        public string Name { get; set; }

        // Simple comma-separated string of all the track's composers
        [Required, StringLength(500)]
        [Display(Name = "Composer names (comma-separated)")]
        public string Composers { get; set; }

        [Required]
        [Display(Name = "Track genre")]
        public SelectList GenreList { get; set; }

        // Will be a hidden HTML Forms element
        public int AlbumId { get; set; }

        [Display(Name = "Album name")]
        public string AlbumName { get; set; }

        [Required]
        [Display(Name = "Sample clip")]
        [DataType(DataType.Upload)]
        public string AudioUpload { get; set; }
    }

    public class TrackAdd
    {
        [Required, StringLength(200)]
        [Display(Name = "Track name")]
        public string Name { get; set; }

        [Required, StringLength(500)]
        [Display(Name = "Composer name(s)")]
        public string Composers { get; set; }

        [Required]
        [Display(Name = "Track genre")]
        public string Genre { get; set; }

        // Associated item properties
        [Range(1, Int32.MaxValue)]
        public int AlbumId { get; set; }

        [Required]
        public HttpPostedFileBase AudioUpload { get; set; }
    }

    public class TrackBase 
    {
        public int Id { get; set; }

        [Display(Name = "Clerk who helps with album tasks")]
        public string Clerk { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Track name")]
        public string Name { get; set; }

        [Required, StringLength(500)]
        [Display(Name = "Composer name(s)")]
        public string Composers { get; set; }

        [Required]
        [Display(Name = "Track genre")]
        public string Genre { get; set; }

        // Associated item properties
        [Range(1, Int32.MaxValue)]
        public int AlbumId { get; set; }

        [Display(Name = "Sample clip")]
        public string AudioUrl
        {
            get
            {
                return $"/clip/{Id}";
            }
        }
    }

    public class TrackWithDetails : TrackBase
    {
        public TrackWithDetails()
        {
            AlbumNames = new List<string>();
        }

        [Display(Name = "Number of albums with this track")]
        public int AlbumsCount { get; set; }

        [Display(Name = "Albums with this track")]
        public IEnumerable<string> AlbumNames { get; set; }
    }

    public class TrackEditComposers
    {
        public int Id { get; set; }

        [Display(Name = "Track name")]
        public string Name { get; set; }

        [Required, StringLength(500)]
        [Display(Name = "Composer name(s)")]
        public string Composers { get; set; }

        [Display(Name = "Track genre")]
        public string Genre { get; set; }
    }

    public class TrackAudio
    {
        public int Id { get; set; }
        public string AudioContentType { get; set; }
        public byte[] Audio { get; set; }
    }

}
