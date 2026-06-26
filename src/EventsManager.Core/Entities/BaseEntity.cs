using System;
using System.Collections.Generic;
using System.Text;

namespace EventsManager.Core.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
