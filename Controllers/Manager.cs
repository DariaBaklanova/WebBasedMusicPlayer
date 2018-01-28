using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// new...
using AutoMapper;
using Assignment7.Models;
using System.Security.Claims;

namespace Assignment7.Controllers
{
    public class Manager
    {
        // Reference to the data context
        private ApplicationDbContext ds = new ApplicationDbContext();

        // AutoMapper components
        MapperConfiguration config;
        public IMapper mapper;

        // Request user property...

        // Backing field for the property
        private RequestUser _user;

        // Getter only, no setter
        public RequestUser User
        {
            get
            {
                // On first use, it will be null, so set its value
                if (_user == null)
                {
                    _user = new RequestUser(HttpContext.Current.User as ClaimsPrincipal);
                }
                return _user;
            }
        }

        // Default constructor...
        public Manager()
        {
            // If necessary, add constructor code here

            // Configure the AutoMapper components
            config = new MapperConfiguration(cfg =>
            {
                // Define the mappings below, for example...
                // cfg.CreateMap<SourceType, DestinationType>();
                // cfg.CreateMap<Employee, EmployeeBase>();

                // Object mapper definitions

                cfg.CreateMap<Models.RegisterViewModel, Models.RegisterViewModelForm>();

                cfg.CreateMap<Models.Artist, Controllers.ArtistBase>();
                cfg.CreateMap<Models.Artist, Controllers.ArtistWithDetails>();
                cfg.CreateMap<Controllers.ArtistAdd, Models.Artist>();
                cfg.CreateMap<Controllers.ArtistAdd, Controllers.ArtistAddForm>();
                cfg.CreateMap<Models.Artist, Controllers.ArtistWithMediaInfo>();

                cfg.CreateMap<Models.Album, Controllers.AlbumBase>();
                cfg.CreateMap<Models.Album, Controllers.AlbumWithDetails>();
                cfg.CreateMap<Controllers.AlbumAdd, Models.Album>();
                cfg.CreateMap<Controllers.AlbumAdd, Controllers.AlbumAddForm>();

                cfg.CreateMap<Models.Track, Controllers.TrackBase>();
                cfg.CreateMap<Models.Track, Controllers.TrackWithDetails>();
                cfg.CreateMap<Models.Track, Controllers.TrackEditComposers>();
                cfg.CreateMap<Controllers.TrackWithDetails, Controllers.TrackEditComposers>();
                cfg.CreateMap<Controllers.TrackAdd, Models.Track>();
                cfg.CreateMap<Controllers.TrackAdd, Controllers.TrackAddForm>();

                cfg.CreateMap<Models.Track, Controllers.TrackAudio>();

                cfg.CreateMap<Models.MediaItem, Controllers.MediaItemBase>();
                cfg.CreateMap<Models.MediaItem, Controllers.MediaItemContent>();
                //cfg.CreateMap<Controllers.MediaItemAdd, Models.MediaItem>();
                //cfg.CreateMap<Controllers.MediaItemAdd, Controllers.MediaItemAddForm>();

                cfg.CreateMap<Models.Genre, Controllers.GenreBase>();
            });

            mapper = config.CreateMapper();

            // Turn off the Entity Framework (EF) proxy creation features
            // We do NOT want the EF to track changes - we'll do that ourselves
            ds.Configuration.ProxyCreationEnabled = false;

            // Also, turn off lazy loading...
            // We want to retain control over fetching related objects
            ds.Configuration.LazyLoadingEnabled = false;
        }

        // ############################################################
        // RoleClaim

        public List<string> RoleClaimGetAllStrings()
        {
            return ds.RoleClaims.OrderBy(r => r.Name).Select(r => r.Name).ToList();
        }

        // Add methods below
        // Controllers will call these methods
        // Ensure that the methods accept and deliver ONLY view model objects and collections
        // The collection return type is almost always IEnumerable<T>

        // Suggested naming convention: Entity + task/action
        // For example:
        // ProductGetAll()
        // ProductGetById()
        // ProductAdd()
        // ProductEdit()
        // ProductDelete()



        // ############################################################
        // Genre

        public IEnumerable<GenreBase> GenreGetAll()
        {
            return mapper.Map<IEnumerable<GenreBase>>(ds.Genres.OrderBy(g => g.Name));
        }

        // ############################################################
        // Artist

        public IEnumerable<ArtistBase> ArtistGetAll()
        {
            return mapper.Map<IEnumerable<ArtistBase>>(ds.Artists.OrderBy(a => a.Name));
        }

        public ArtistWithMediaInfo ArtistGetByIdWithAudioInfo(int id)
        {
            var o = ds.Artists.Include("MediaItems").SingleOrDefault(p => p.Id == id);

            return (o == null) ? null : mapper.Map<ArtistWithMediaInfo>(o);
        }

        public MediaItemContent ArtistAudioGetById(string stringId)
        {
            var o = ds.MediaItems.SingleOrDefault(p => p.StringId == stringId);

            return (o == null) ? null : mapper.Map<MediaItemContent>(o);
        }

        public ArtistWithDetails ArtistGetByIdWithDetail(int id)
        {
            var o = ds.Artists.Include("Albums").SingleOrDefault(a => a.Id == id);

            return (o == null) ? null : mapper.Map<ArtistWithDetails>(o);
        }


        public ArtistBase ArtistMediaAdd(MediaItemAdd newItem)
        {
            // Validate the associated item
            var a = ds.Artists.Find(newItem.ArtistId);

            if (a == null)
            {
                return null;
            }
            else
            {
                // Attempt to add the new item
                var addedItem = new MediaItem();
                ds.MediaItems.Add(addedItem);

                addedItem.Caption = newItem.Caption;
                //addedItem.Artist = a;

                byte[] photoBytes = new byte[newItem.MediaItemUpload.ContentLength];
                newItem.MediaItemUpload.InputStream.Read(photoBytes, 0, newItem.MediaItemUpload.ContentLength);

                addedItem.Content = photoBytes;
                addedItem.ContentType = newItem.MediaItemUpload.ContentType;

                ds.SaveChanges();

                return (addedItem == null) ? null : mapper.Map<ArtistBase>(a);
            }
        }





        public ArtistBase ArtistAdd(ArtistAdd newItem)
        {
            var addedItem = ds.Artists.Add(mapper.Map<Artist>(newItem));

            addedItem.Executive = User.Name;

            ds.SaveChanges();

            return (addedItem == null) ? null : mapper.Map<ArtistBase>(addedItem);
        }


        // ############################################################
        // Album

        public IEnumerable<AlbumBase> AlbumGetAll()
        {
            return mapper.Map<IEnumerable<AlbumBase>>(ds.Albums.OrderBy(a => a.Name));
        }

        public AlbumWithDetails AlbumGetByIdWithDetail(int id)
        {
            var o = ds.Albums.Include("Tracks").Include("Artists")
                .SingleOrDefault(a => a.Id == id);

            return (o == null) ? null : mapper.Map<AlbumWithDetails>(o);
        }

        public AlbumBase AlbumAdd(AlbumAdd newItem)
        {
            // Attention 14 - Manager method to add a new album for an artist

            // Validate each of the desired associated artists
            var artists = new List<Artist>();
            foreach (var artistId in newItem.ArtistIds)
            {
                var a = ds.Artists.Find(artistId);
                if (a != null)
                {
                    artists.Add(a);
                }
            }

            // Validate each of the desired associated tracks
            var tracks = new List<Track>();
            foreach (var trackId in newItem.TrackIds)
            {
                var a = ds.Tracks.Find(trackId);
                if (a != null)
                {
                    tracks.Add(a);
                }
            }

            // We only really want to continue if the album HAS at least one associated artist
            if (artists.Count == 0)
            {
                return null;
            }
            else
            {
                // Attempt to add the new item
                var addedItem = ds.Albums.Add(mapper.Map<Album>(newItem));

                // Set the association - artists
                foreach (var item in artists)
                {
                    addedItem.Artists.Add(item);
                }

                // Set the association - tracks
                foreach (var item in tracks)
                {
                    addedItem.Tracks.Add(item);
                }

                // Set the coordinator user name
                addedItem.Coordinator = User.Name;

                ds.SaveChanges();

                return (addedItem == null) ? null : mapper.Map<AlbumBase>(addedItem);
            }
        }

        // ############################################################
        // Track

        public TrackAudio TrackAudioGetById(int id)
        {
            var o = ds.Tracks.Find(id);

            return (o == null) ? null : mapper.Map<TrackAudio>(o);
        }

        public IEnumerable<TrackBase> TrackGetAll()
        {
            return mapper.Map<IEnumerable<TrackBase>>(ds.Tracks.OrderBy(t => t.Name));
        }

        public TrackBase TrackGetById(int id)
        {
            // Fetch the track object
            var o = ds.Tracks.Find(id);

            if (o == null)
            {
                return null;
            }
            else
            {
                return mapper.Map<TrackBase>(o);
            }
        }

        public TrackWithDetails TrackGetByIdWithDetail(int id)
        {
            // Fetch the track object
            var o = ds.Tracks.Include("Albums.Artists")
                .SingleOrDefault(t => t.Id == id);

            if (o == null)
            {
                return null;
            }
            else
            {
                // Create the result collection
                var result = mapper.Map<TrackWithDetails>(o);
                // Fill in the album names
                result.AlbumNames = o.Albums.Select(a => a.Name);

                return result;
            }
        }

        public IEnumerable<TrackBase> TrackGetAllByArtistId(int id)
        {
            // Fetch the artist
            var o = ds.Artists.Include("Albums.Tracks").SingleOrDefault(a => a.Id == id);

            // Continue?
            if (o == null) { return null; }

            // Create a collection to hold the results
            var c = new List<Track>();

            // Go through each album, and get the tracks
            foreach (var album in o.Albums)
            {
                c.AddRange(album.Tracks);
            }

            // Remove duplicates
            c = c.Distinct().ToList();

            return mapper.Map<IEnumerable<TrackBase>>(c.OrderBy(t => t.Name));
        }

        public TrackWithDetails TrackAdd(TrackAdd newItem)
        {
            // Attention 18 - Manager method to add a new track for an album

            // Validate the associated item
            var a = ds.Albums.Include("Artists")
                .SingleOrDefault(al => al.Id == newItem.AlbumId);

            if (a == null)
            {
                return null;
            }
            else
            {
                // Attempt to add the new item
                var addedItem = ds.Tracks.Add(mapper.Map<Track>(newItem));
                // Set the association
                addedItem.Albums.Add(a);
                // Set the clerk user name
                addedItem.Clerk = User.Name;
                byte[] photoBytes = new byte[newItem.AudioUpload.ContentLength];
                newItem.AudioUpload.InputStream.Read(photoBytes, 0, newItem.AudioUpload.ContentLength);

                addedItem.Audio = photoBytes;
                addedItem.AudioContentType = newItem.AudioUpload.ContentType;

                ds.SaveChanges();

                return (addedItem == null) ? null : mapper.Map<TrackWithDetails>(addedItem);
            }
        }


        public TrackBase TrackEdit(TrackEditComposers newItem)
        {
            // Attention 22 - Track edit code in the Manager class

            // Attempt to fetch the object
            var o = ds.Tracks.Find(newItem.Id);

            if (o == null)
            {
                // Problem - item was not found, so return
                return null;
            }
            else
            {
                // Update the object with the incoming values
                ds.Entry(o).CurrentValues.SetValues(newItem);
                ds.SaveChanges();

                // Prepare and return the object
                return mapper.Map<TrackBase>(o);
            }
        }

        public bool TrackDelete(int id)
        {
            // Attention 27 - Track delete code in the Manager class

            // Attempt to fetch the object to be deleted
            var itemToDelete = ds.Tracks.Find(id);

            if (itemToDelete == null)
            {
                return false;
            }
            else
            {
                // Remove the object
                ds.Tracks.Remove(itemToDelete);
                ds.SaveChanges();

                return true;
            }
        }





        // Add some programmatically-generated objects to the data store
        // Can write one method, or many methods - your decision
        // The important idea is that you check for existing data first
        // Call this method from a controller action/method

        public bool LoadData()
        {
            // User name
            var user = User.Name;

            // Monitor the progress
            bool done = false;

            // ############################################################
            // Role claims

            if (ds.RoleClaims.Count() == 0)
            {
                // Add role claims here

                ds.RoleClaims.Add(new RoleClaim { Name = "Executive" });
                ds.RoleClaims.Add(new RoleClaim { Name = "Coordinator" });
                ds.RoleClaims.Add(new RoleClaim { Name = "Clerk" });
                ds.RoleClaims.Add(new RoleClaim { Name = "Staff" });

                ds.SaveChanges();
                done = true;
            }

            // ############################################################
            // Genre

            if (ds.Genres.Count() == 0)
            {
                // Add genres

                ds.Genres.Add(new Genre { Name = "Alternative" });
                ds.Genres.Add(new Genre { Name = "Classical" });
                ds.Genres.Add(new Genre { Name = "Country" });
                ds.Genres.Add(new Genre { Name = "Easy Listening" });
                ds.Genres.Add(new Genre { Name = "Hip-Hop/Rap" });
                ds.Genres.Add(new Genre { Name = "Jazz" });
                ds.Genres.Add(new Genre { Name = "Pop" });
                ds.Genres.Add(new Genre { Name = "R&B" });
                ds.Genres.Add(new Genre { Name = "Rock" });
                ds.Genres.Add(new Genre { Name = "Soundtrack" });

                ds.SaveChanges();
                done = true;
            }

            // ############################################################
            // Artist

            if (ds.Artists.Count() == 0)
            {
                // Add artists

                ds.Artists.Add(new Artist
                {
                    Name = "The Beatles",
                    BirthOrStartDate = new DateTime(1962, 8, 15),
                    Executive = user,
                    Genre = "Pop",
                    UrlArtist = "https://upload.wikimedia.org/wikipedia/commons/9/9f/Beatles_ad_1965_just_the_beatles_crop.jpg"
                });

                ds.Artists.Add(new Artist
                {
                    Name = "Adele",
                    BirthName = "Adele Adkins",
                    BirthOrStartDate = new DateTime(1988, 5, 5),
                    Executive = user,
                    Genre = "Pop",
                    UrlArtist = "http://www.billboard.com/files/styles/article_main_image/public/media/Adele-2015-close-up-XL_Columbia-billboard-650.jpg"
                });

                ds.Artists.Add(new Artist
                {
                    Name = "Bryan Adams",
                    BirthOrStartDate = new DateTime(1959, 11, 5),
                    Executive = user,
                    Genre = "Rock",
                    UrlArtist = "https://upload.wikimedia.org/wikipedia/commons/7/7e/Bryan_Adams_Hamburg_MG_0631_flickr.jpg"
                });

                ds.SaveChanges();
                done = true;
            }

            // ############################################################
            // Album

            if (ds.Albums.Count() == 0)
            {
                // Add albums

                // For Bryan Adams
                var bryan = ds.Artists.SingleOrDefault(a => a.Name == "Bryan Adams");

                ds.Albums.Add(new Album
                {
                    Artists = new List<Artist> { bryan },
                    Name = "Reckless",
                    ReleaseDate = new DateTime(1984, 11, 5),
                    Coordinator = user,
                    Genre = "Rock",
                    UrlAlbum = "https://upload.wikimedia.org/wikipedia/en/5/56/Bryan_Adams_-_Reckless.jpg"
                });

                ds.Albums.Add(new Album
                {
                    Artists = new List<Artist> { bryan },
                    Name = "So Far So Good",
                    ReleaseDate = new DateTime(1993, 11, 2),
                    Coordinator = user,
                    Genre = "Rock",
                    UrlAlbum = "https://upload.wikimedia.org/wikipedia/pt/a/ab/So_Far_so_Good_capa.jpg"
                });

                ds.SaveChanges();
                done = true;
            }

            // ############################################################
            // Track

            if (ds.Tracks.Count() == 0)
            {
                // Add tracks

                // For Reckless
                var reck = ds.Albums.SingleOrDefault(a => a.Name == "Reckless");

                ds.Tracks.Add(new Track
                {
                    Albums = new List<Album> { reck },
                    Name = "Run To You",
                    Composers = "Bryan Adams, Jim Vallance",
                    Clerk = user,
                    Genre = "Rock"
                });

                ds.Tracks.Add(new Track
                {
                    Albums = new List<Album> { reck },
                    Name = "Heaven",
                    Composers = "Bryan Adams, Jim Vallance",
                    Clerk = user,
                    Genre = "Rock"
                });

                ds.Tracks.Add(new Track
                {
                    Albums = new List<Album> { reck },
                    Name = "Somebody",
                    Composers = "Bryan Adams, Jim Vallance",
                    Clerk = user,
                    Genre = "Rock"
                });

                ds.Tracks.Add(new Track
                {
                    Albums = new List<Album> { reck },
                    Name = "Summer of '69",
                    Composers = "Bryan Adams, Jim Vallance",
                    Clerk = user,
                    Genre = "Rock"
                });

                ds.Tracks.Add(new Track
                {
                    Albums = new List<Album> { reck },
                    Name = "Kids Wanna Rock",
                    Composers = "Bryan Adams, Jim Vallance",
                    Clerk = user,
                    Genre = "Rock"
                });

                // For Reckless
                var so = ds.Albums.SingleOrDefault(a => a.Name == "So Far So Good");

                ds.Tracks.Add(new Track
                {
                    Albums = new List<Album> { so },
                    Name = "Straight from the Heart",
                    Composers = "Bryan Adams, Eric Kagna",
                    Clerk = user,
                    Genre = "Rock"
                });

                ds.Tracks.Add(new Track
                {
                    Albums = new List<Album> { so },
                    Name = "It's Only Love",
                    Composers = "Bryan Adams, Jim Vallance",
                    Clerk = user,
                    Genre = "Rock"
                });

                ds.Tracks.Add(new Track
                {
                    Albums = new List<Album> { so },
                    Name = "This Time",
                    Composers = "Bryan Adams, Jim Vallance",
                    Clerk = user,
                    Genre = "Rock"
                });

                ds.Tracks.Add(new Track
                {
                    Albums = new List<Album> { so },
                    Name = "(Everything I Do) I Do It for You",
                    Composers = "Bryan Adams, Jim Vallance",
                    Clerk = user,
                    Genre = "Rock"
                });

                ds.Tracks.Add(new Track
                {
                    Albums = new List<Album> { so },
                    Name = "Heat of the Night",
                    Composers = "Bryan Adams, Jim Vallance",
                    Clerk = user,
                    Genre = "Rock"
                });

                ds.SaveChanges();
                done = true;
            }

            return done;
        }

        public bool RemoveData()
        {
            try
            {
                foreach (var e in ds.RoleClaims)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                ds.SaveChanges();

                foreach (var e in ds.Tracks)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                ds.SaveChanges();

                foreach (var e in ds.Albums)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                ds.SaveChanges();

                foreach (var e in ds.Artists)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                ds.SaveChanges();

                foreach (var e in ds.Genres)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                ds.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveDatabase()
        {
            try
            {
                return ds.Database.Delete();
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

    // New "RequestUser" class for the authenticated user
    // Includes many convenient members to make it easier to render user account info
    // Study the properties and methods, and think about how you could use it

    // How to use...

    // In the Manager class, declare a new property named User
    //public RequestUser User { get; private set; }

    // Then in the constructor of the Manager class, initialize its value
    //User = new RequestUser(HttpContext.Current.User as ClaimsPrincipal);

    public class RequestUser
    {
        // Constructor, pass in the security principal
        public RequestUser(ClaimsPrincipal user)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                Principal = user;

                // Extract the role claims
                RoleClaims = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

                // User name
                Name = user.Identity.Name;

                // Extract the given name(s); if null or empty, then set an initial value
                string gn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.GivenName).Value;
                if (string.IsNullOrEmpty(gn)) { gn = "(empty given name)"; }
                GivenName = gn;

                // Extract the surname; if null or empty, then set an initial value
                string sn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Surname).Value;
                if (string.IsNullOrEmpty(sn)) { sn = "(empty surname)"; }
                Surname = sn;

                IsAuthenticated = true;
                // You can change the string value in your app to match your app domain logic
                IsAdmin = user.HasClaim(ClaimTypes.Role, "Admin") ? true : false;
            }
            else
            {
                RoleClaims = new List<string>();
                Name = "anonymous";
                GivenName = "Unauthenticated";
                Surname = "Anonymous";
                IsAuthenticated = false;
                IsAdmin = false;
            }

            // Compose the nicely-formatted full names
            NamesFirstLast = $"{GivenName} {Surname}";
            NamesLastFirst = $"{Surname}, {GivenName}";
        }

        // Public properties
        public ClaimsPrincipal Principal { get; private set; }
        public IEnumerable<string> RoleClaims { get; private set; }

        public string Name { get; set; }

        public string GivenName { get; private set; }
        public string Surname { get; private set; }

        public string NamesFirstLast { get; private set; }
        public string NamesLastFirst { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public bool IsAdmin { get; private set; }

        public bool HasRoleClaim(string value)
        {
            if (!IsAuthenticated) { return false; }
            return Principal.HasClaim(ClaimTypes.Role, value) ? true : false;
        }

        public bool HasClaim(string type, string value)
        {
            if (!IsAuthenticated) { return false; }
            return Principal.HasClaim(type, value) ? true : false;
        }
    }

}