using System;
using EFDDD.Core;

namespace EFDDD.DataModel
{
    public class Home : IDataEntity
    {
        public Guid Id { get; set; }
        public DateTime? DateBuilt { get; set; }
        public string Address { get; set; }
        public Neighborhood Neighborhood { get; set; }
        
        /// <summary>
        /// Serialized to/from List&lt;Home&gt;
        /// </summary>
        public string Rooms { get; set; }

        public Guid HomeOwnerId { get; set; }

        public Home()
        {
            Neighborhood = new Neighborhood();
        }
    }
}
