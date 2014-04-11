﻿﻿//
// Copyright (c) KU Leuven Research and Development - iMinds-DistriNet
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Administrative Contact: dnet-project-office@cs.kuleuven.be
// Technical Contact: stefan.walraven@cs.kuleuven.be
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelApplicationNET.Models;

namespace TravelApplicationNET.Travel.Features.Price
{
    public interface IPriceCalculation
    {
        double CalculatePrice(double pricePerNight, DateTime startDate, DateTime endDate);
    }

    class DefaultPriceCalculation : IPriceCalculation
    {
        public double CalculatePrice(double pricePerNight, DateTime startDate, DateTime endDate)
        {
            return pricePerNight * endDate.Subtract(startDate).Days;
        }
    }

    class UserProfileBasedPriceCalculation : IPriceCalculation
    {
        public double CalculatePrice(double pricePerNight, DateTime startDate, DateTime endDate)
        {
            using (var db = new HotelContext())
            {
                var normalPrice = pricePerNight * endDate.Subtract(startDate).Days;
                // TODO use authentication service
                // User user =
                string userId = null;
                if (userId == null)
                    return normalPrice;
                else
                {
                    UserProfile up = db.UserProfiles.Find(userId);
                    if (up == null)
                        return normalPrice;
                    else
                    {
                        int discountPercentage = up.Discount.Value;
                        return normalPrice * (1.0 - discountPercentage / 100);
                    }
                }
            }
        }
    }
}
