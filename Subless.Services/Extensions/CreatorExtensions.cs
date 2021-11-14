﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subless.Models;

namespace Subless.Services.Extensions
{
    public static class CreatorExtensions
    {
        public static PartnerViewCreator ToPartnerView(this Creator creator)
        {
            return new PartnerViewCreator()
            {
                PartnerId = creator.PartnerId,
                Active = creator.Active,
                Email = creator.Email,
                Id = creator.Id,
                Username = creator.Username
            };
        }
    }
}
