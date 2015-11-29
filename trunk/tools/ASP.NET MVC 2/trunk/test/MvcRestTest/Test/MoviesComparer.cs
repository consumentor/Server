using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using MovieApp.Models;

namespace MovieApp.Test
{
    public class MoviesComparer : EqualityComparer<Movie>
    {
        public bool EqualsIgnoreTimeSkew(Movie x, Movie y)
        {
            if (x.Director == y.Director &&
                x.Title == y.Title)
            {
                if (x.DateReleased == y.DateReleased)
                {
                    return true;
                }
                else if ((x.DateReleased - y.DateReleased).TotalDays < 1.0d)
                {
                    Console.WriteLine("WARNING: ignored time delta: " + x.DateReleased.ToString("G") + " <=> " + y.DateReleased.ToString("G"));
                    return true;
                }
            }
            return false;
        }

        public override bool Equals(Movie x, Movie y)
        {
            if (x.Director == y.Director &&
                x.Title == y.Title &&
                x.DateReleased == y.DateReleased)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode(Movie obj)
        {
            return obj.Director.GetHashCode() ^ obj.Title.GetHashCode() ^ obj.DateReleased.GetHashCode();
        }
    }
}
