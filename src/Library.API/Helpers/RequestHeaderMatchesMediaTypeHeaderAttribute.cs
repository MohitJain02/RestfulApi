using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Helpers
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class RequestHeaderMatchesMediaTypeHeaderAttribute : Attribute, IActionConstraint
    {
        private string _requestHeaderToMatch;

        private string[] _mediaTypes;

        public RequestHeaderMatchesMediaTypeHeaderAttribute(string requestHeaderToMatch, string[] mediaTypes)
        {
            _requestHeaderToMatch = requestHeaderToMatch;
            _mediaTypes = mediaTypes;

        }

        public int Order
        {
            get
            {
                return 0;
            }
        }

        public bool Accept(ActionConstraintContext context)
        {
            var requestHeader = context.RouteContext.HttpContext.Request.Headers;

            if(!requestHeader.ContainsKey(_requestHeaderToMatch))
            {
                return false;
            }

            // if one of media type matches return true
            foreach(var mediaType in _mediaTypes)
            {
                var mediaTypeMatches = string.Equals(requestHeader[_requestHeaderToMatch],
                                                        mediaType,
                                                        StringComparison.OrdinalIgnoreCase);

                if (mediaTypeMatches)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
