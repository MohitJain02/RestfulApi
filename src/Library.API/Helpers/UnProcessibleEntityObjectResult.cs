﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Helpers
{
    public class UnProcessibleEntityObjectResult : ObjectResult
    {
        public UnProcessibleEntityObjectResult(ModelStateDictionary modelState)
            : base(new SerializableError(modelState))
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }


            StatusCode = 422;
        }
    }
}
