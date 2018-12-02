using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Services
{
    public class PropertyMappingValue
    {
        public bool Revert { get; private set; }

        public IEnumerable<string> DestinationProperties { get; private set; }

        public PropertyMappingValue(
            IEnumerable<string> destinationProperties,
            bool revert = false)
        {
            Revert = revert;
            DestinationProperties = destinationProperties;
        }


    }
}
