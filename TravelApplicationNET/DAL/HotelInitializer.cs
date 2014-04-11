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
using System.Data.Entity;
using System.Web;
using TravelApplicationNET.Models;

namespace TravelApplicationNET.DAL
{
    public class HotelInitializer : DropCreateDatabaseAlways<HotelContext>
    {
        protected override void Seed(HotelContext context)
        {
            var hotels = new List<Hotel>
            {
                new Hotel { Name = "Fawlty Towers", Address = new Address {Street = "Celestijnenlaan", Number = 200, PostalCode = 3001, City = "Heverlee", Country = "Belgium" } },
                new Hotel { Name = "Rilton", Address = new Address {Street = "Naamsestraat", Number = 80, PostalCode = 3000, City = "Leuven", Country = "Belgium" } }
            };

            var roomDetails = new List<RoomDetails>
            {
                new RoomDetails { RoomNb = 1, NbOfBeds = 1, PricePerNight = 70.0, SmokingAllowed = false },
                new RoomDetails { RoomNb = 2, NbOfBeds = 2, PricePerNight = 90.0, SmokingAllowed = true },
                new RoomDetails { RoomNb = 3, NbOfBeds = 3, PricePerNight = 100.0, SmokingAllowed = false },
                new RoomDetails { RoomNb = 4, NbOfBeds = 4, PricePerNight = 150.0, SmokingAllowed = false }
            };

            foreach (Hotel h in hotels)
            {
                roomDetails.ForEach(rd => h.AddRoom(new Room { RoomDetails = rd }));
            }

            hotels.ForEach(h => context.Hotels.Add(h));
            context.SaveChanges();
        }
    }
}
