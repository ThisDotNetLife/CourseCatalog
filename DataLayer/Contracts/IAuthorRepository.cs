using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayerServices {
    public interface IAuthorRepository {

        DataLayer.Entities.Author Find(int ID);

        List<DataLayer.Entities.Author> GetAll();

        DataLayer.Entities.Author Add(DataLayer.Entities.Webcast webcast);

        DataLayer.Entities.Author Update(DataLayer.Entities.Webcast webcast);

        DataLayer.Entities.Author Remove(int ID);
    }
}