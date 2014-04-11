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
using TravelApplicationNET.Travel.Features.Price;
using System.Transactions;
using System.Data.Objects;

namespace TravelApplicationNET.Travel
{
    public interface IBookingService
    {
        ICollection<HotelInfo> FindHotelsInCity(string city);
        ICollection<HotelInfo> FindAllHotelsWithFreeRoomsInPeriod(DateTime startDate, DateTime endDate);

        Booking CreateBooking(string hotelName, BookingConstraints constraints, string guestName);
        ICollection<Booking> FinalizeBookings(ICollection<Booking> tentativeBookings);
    }

    class BookingService : IBookingService
    {
        private HotelContext db;
        private readonly IPriceCalculation calc;

        public BookingService(IPriceCalculation calculation)
        {
            db = new HotelContext();
            calc = calculation;
        }

        #region Lookup

        /**********
         * LOOKUP *
         **********/

        public ICollection<HotelInfo> FindHotelsInCity(string city)
        {
            var hotelNames = from hotel in db.Hotels
                             where hotel.Address.City.Equals(city)
                             select hotel.Name;
            return GetHotelInfos(hotelNames.ToList());
        }

        public ICollection<HotelInfo> FindAllHotelsWithFreeRoomsInPeriod(DateTime startDate, DateTime endDate)
        {
            var hotelNames = (from room in db.Rooms
                              where (!room.Bookings.Any() ||
                                 !(from booking in room.Bookings
                                   where (EntityFunctions.TruncateTime(booking.StartDate) >= startDate && EntityFunctions.TruncateTime(booking.StartDate) <= endDate) ||
                                   (EntityFunctions.TruncateTime(booking.EndDate) >= startDate && EntityFunctions.TruncateTime(booking.EndDate) <= endDate)
                                   select booking).Any())
                              select room.HotelName).Distinct();
            return GetHotelInfos(hotelNames.ToList());
        }

        private ICollection<HotelInfo> GetHotelInfos(List<string> hotelNames)
        {
            var hotelInfos = new List<HotelInfo>();
            hotelNames.ForEach(n => hotelInfos.Add(CreateHotelInfo(n)));
            return hotelInfos;
        }

        private HotelInfo CreateHotelInfo(string hotelName)
        {
            return new HotelInfo { HotelName = hotelName, Address = GetHotel(hotelName).Address };
        }

        private Hotel GetHotel(string name)
        {
            return db.Hotels.Find(name);
        }
        #endregion

        #region Bookings
        /************
         * BOOKINGS *
         ************/

        public Booking CreateBooking(string hotelName, BookingConstraints constraints, string guestName)
        {
            var hotel = GetHotel(hotelName);
            if (hotel == null)
                throw new ArgumentException("There exists no hotel with the given name.");
            foreach (RoomDetails rd in hotel.GetFreeRooms(constraints.StartDate, constraints.EndDate))
            {
                if (rd.SmokingAllowed.Equals(constraints.SmokingAllowed)
                    && constraints.NbOfBeds <= rd.NbOfBeds
                    && constraints.MaxPricePerNight >= rd.PricePerNight)
                {
                    return new Booking
                    {
                        Guest = guestName,
                        StartDate = constraints.StartDate,
                        EndDate = constraints.EndDate,
                        RoomNb = rd.RoomNb,
                        HotelName = hotel.Name,
                        Price = calc.CalculatePrice(rd.PricePerNight, constraints.StartDate, constraints.EndDate)
                    };
                }
            }
            throw new BookingException("No room to satisfy the given booking constraints.");
        }

        public ICollection<Booking> FinalizeBookings(ICollection<Booking> tentativeBookings)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                foreach (Booking b in tentativeBookings)
                {
                    FinalizeBooking(b);
                }
                scope.Complete();
            }
            return tentativeBookings.ToList().AsReadOnly();
        }

        private void FinalizeBooking(Booking booking)
        {
            var hotel = GetHotel(booking.HotelName);
            if (hotel == null)
                throw new ArgumentException("There exists no hotel with the given name.");
            var room = hotel.GetRoom(booking.RoomNb);
            if (room == null)
                throw new ArgumentException("There exists no room with the given number.");
            room.AddBooking(booking);
            db.SaveChanges();
        }
        #endregion
    }
}
