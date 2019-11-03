using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayerServices {
    public interface ITagRepository {

        string Get();

        void Update(DataLayer.Entities.Tag tag);
    }
}