using full_Project.Models.Domain;
using full_Project.Models.DTO;
using System.Linq; // Ensure System.Linq is imported for IQueryable

namespace full_Project.Repositories.Abstract
{
    public interface IGenreService
    {
        bool Add(Genre model);
        bool Update(Genre model);
        Genre GetById(int id);
        bool Delete(int id);
        IQueryable<Genre> List();
    }
}
