using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment7.Controllers
{
    public class ArtistAddForm
    {
        public ArtistAddForm()
        {
            BirthName = "";
            BirthOrStartDate = DateTime.Now.AddYears(-20);
        }
        [Required, StringLength(100)]
        [Display(Name = "Artist name or stage name")]
        public string Name { get; set; }

        [StringLength(100)]
        [Display(Name = "If applicable, artist's birth name")]
        public string BirthName { get; set; }

        [Required]
        [Display(Name = "Birth date, or start date")]
        [DataType(DataType.Date)]
        public DateTime BirthOrStartDate { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "URL to artist photo")]
        [DataType(DataType.Url)]
        public string UrlArtist { get; set; }

        [Required]
        [Display(Name = "Artist's primary genre")]
        public SelectList GenreList { get; set; }

        [StringLength(10000)]
        [Display(Name = "Artist profile")]
        [DataType(DataType.MultilineText)]
        public string Profile { get; set; }


    }

    public class ArtistAdd
    {
        public ArtistAdd()
        {
            BirthName = "";
        }

        [Required, StringLength(100)]
        [Display(Name = "Artist name or stage name")]
        public string Name { get; set; }

        [StringLength(100)]
        [Display(Name = "If applicable, artist's birth name")]
        public string BirthName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Birth date, or start date")]
        public DateTime BirthOrStartDate { get; set; }

        [Required, StringLength(200)]
        [DataType(DataType.Url)]
        [Display(Name = "Artist photo")]
        public string UrlArtist { get; set; }

        [Required]
        [Display(Name = "Artist's primary genre")]
        public string Genre { get; set; }

        [Required, StringLength(10000)]
        [Display(Name = "Artist profile")]
        [DataType(DataType.MultilineText)]
        public string Profile { get; set; }
    }

    public class ArtistBase : ArtistAdd
    {
        public int Id { get; set; }

        [Display(Name = "Executive who looks after this artist")]
        public string Executive { get; set; }
    }

    public class ArtistWithDetails : ArtistBase
    {
        [Display(Name = "Number of albums")]
        public int AlbumsCount { get; set; }
    }

    public class ArtistWithMediaInfo : ArtistBase
    {
        public ArtistWithMediaInfo()
        {
            MediaItems = new List<MediaItemBase>();
        }

        public IEnumerable<MediaItemBase> MediaItems { get; set; }
    }
}