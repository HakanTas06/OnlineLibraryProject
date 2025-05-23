﻿using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface ICategoryDal
    {
        List<Category> GetAll();

        void Add(Category category);
        void Update(Category category);
        void Delete(int categoryId);
        Category GetById(int id);
    }
}
