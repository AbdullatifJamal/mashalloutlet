using Ilibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ilibrary.DataAccess.Repository.IRepository
{
    public interface IBrandRepository : IRepository<Section>
    {
        void Update(Section obj );
       
    }
}
