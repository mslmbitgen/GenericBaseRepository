using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericBaseRepository.Entities;

public class PagedResult<TEntity>
{
    public int TotalCount { get; set; } // Toplam kayıt sayısı
    public List<TEntity> Items { get; set; } = new List<TEntity>(); // Sayfalanmış kayıtlar
}
