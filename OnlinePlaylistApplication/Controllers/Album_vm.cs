using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment7.Controllers
{
    public class AlbumAddForm
    {
        // Attention 02 - Data annotations are important on ALL Album classes

        public AlbumAddForm()
        {
            ReleaseDate = DateTime.Now;
        }

        [Required, StringLength(100)]
        [Display(Name = "Album name")]
        public string Name { get; set; }

        // Attention 11 - Note the DataType data annotation

        [Required]
        [Display(Name = "Release date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        // Get from Apple iTunes Preview, Amazon, or Wikipedia
        [Required, StringLength(200)]
        [Display(Name = "URL to album image (cover art)")]
        [DataType(DataType.Url)]
        public string UrlAlbum { get; set; }

        [Required]
        [Display(Name = "Album's primary genre")]
        public SelectList GenreList { get; set; }

        // Associated artist info
        // Display only
        public string ArtistName { get; set; }

        // Will pre-select at the associated artist
        [Display(Name = "Album's artist(s)")]
        public MultiSelectList ArtistList { get; set; }

        // Allow user to select tracks
        [Display(Name = "Tracks on this album")]
        public MultiSelectList TrackList { get; set; }
    }

    public class AlbumAdd
    {
        public AlbumAdd()
        {
            ReleaseDate = DateTime.Now;
            ArtistIds = new List<int>();
            TrackIds = new List<int>();
        }

        [Required, StringLength(100)]
        [Display(Name = "Album name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Release date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Album image (cover art)")]
        [DataType(DataType.Url)]
        public string UrlAlbum { get; set; }

        [Required]
        [Display(Name = "Album's primary genre")]
        public string Genre { get; set; }

        // Associated item properties
        public List<int> ArtistIds { get; set; }
        public List<int> TrackIds { get; set; }
    }

    public class AlbumBase : AlbumAdd
    {
        public int Id { get; set; }

        // User name who looks after the album
        [Display(Name = "Coordinator who looks after the album")]
        public string Coordinator { get; set; }
    }

    public class AlbumWithDetails : AlbumBase
    {
        [Display(Name = "Number of tracks on this album")]
        public int TracksCount { get; set; }

        [Display(Name = "Number of artists on this album")]
        public int ArtistsCount { get; set; }
    }

}
