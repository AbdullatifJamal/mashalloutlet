using Ilibrary.DataAccess.Data;
using Ilibrary.DataAccess.Repository.IRepository;
using Ilibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ilibrary.DataAccess.Repository
{
    public class BrandRepository :Repository<Section>, IBrandRepository
    {


        private ApplicationDbContext _db;
    public BrandRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }




    public void Update(Section obj)
    {
        _db.Categories.Update(obj);
    }
}
}
