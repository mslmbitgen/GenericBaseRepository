using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericBaseRepository.Entities;

public interface IEntityTimestamps
{
    DateTime CreatedAt { get; set; }
    DateTime? ModifiedAt { get; set; }
    DateTime? DeletedAt { get; set; }
}
