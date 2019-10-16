using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayerServices {
    public interface IWebcastRepository {

        Webcast Find(int ID);

        List<Webcast> GetAll();

        String Save(Webcast webcast);

        Webcast Delete(int ID);
    }
}
