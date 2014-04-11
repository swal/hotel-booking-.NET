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
using System.Web;
using TravelApplicationNET.Models;
using System.Transactions;

namespace TravelApplicationNET.Travel.Management
{
    public interface IManagementService
    {
        void AddNewHotel(string hotelName, Address address);
        void AddNewRoomToHotel(string hotelName, RoomDetails roomDetails);

        ICollection<string> GetHotelNames();
        Address GetHotelAddress(string hotelName);
        ICollection<HotelInfo> GetHotels();
        ICollection<RoomDetails> GetRooms(string hotelName);
        ICollection<Booking> GetBookings(string hotelName, int? roomNb);
        ICollection<Booking> GetBookings(string hotelName);
        ICollection<Booking> GetBookings();

        void RemoveAllBookings();
    }

    class ManagementService : IManagementService
    {
        private HotelContext db;

        public ManagementService()
        {
            db = new HotelContext();
        }

        #region HotelManagement

        /**********************
         * ADD HOTELS & ROOMS *
         **********************/

        public void AddNewHotel(string hotelName, Address address)
        {
            try
            {
                db.Hotels.Add(new Hotel { Name = hotelName, Address = address });
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw new ManagementException("Adding a new hotel failed.");
            }
        }

        public void AddNewRoomToHotel(string hotelName, RoomDetails roomDetails)
        {
            try
            {
                var hotel = GetHotel(hotelName);
                hotel.AddRoom(new Room { RoomDetails = roomDetails });
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw new ManagementException("Adding a new room failed: "+e.Message);
            }
        }
        #endregion

        #region Lookup

        /***********
         * LOOK UP *
         ***********/

        public ICollection<string> GetHotelNames()
        {
            return (from hotel in db.Hotels select hotel.Name).ToList();
        }

        public Address GetHotelAddress(string hotelName)
        {
            return GetHotel(hotelName).Address;
        }

        public ICollection<HotelInfo> GetHotels()
        {
            //Console.WriteLine(db.Hotels.First().Name);
            var hotelInfos = new List<HotelInfo>();
            db.Hotels.ToList().ForEach(h => hotelInfos.Add(new HotelInfo { HotelName = h.Name, Address = h.Address }));
            return hotelInfos;
        }

        private Hotel GetHotel(string name)
        {
            return db.Hotels.Find(name);
        }

        public ICollection<RoomDetails> GetRooms(string hotelName)
        {
            return GetHotel(hotelName).Rooms.Select(r => r.RoomDetails).ToList();
            //return (from room in db.Rooms
            //        where room.HotelName.Equals(hotelName)
            //        select room.RoomDetails).ToList();
        }

        public ICollection<Booking> GetBookings(string hotelName, int? roomNb)
        {
            if (roomNb == null)
                return GetBookings(hotelName);
            return (from booking in db.Bookings
                    where booking.HotelName.Equals(hotelName) && booking.RoomNb == roomNb
                    select booking).ToList();
        }

        public ICollection<Booking> GetBookings(string hotelName)
        {
            return (from booking in db.Bookings
                    where booking.HotelName.Equals(hotelName)
                    select booking).ToList();
        }

        public ICollection<Booking> GetBookings()
        {
            return db.Bookings.ToList();
        }
        #endregion

        #region Cleanup

        /************
         * CLEAN UP *
         ************/

        public void RemoveAllBookings()
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    foreach (var b in db.Bookings)
                        db.Bookings.Remove(b);
                    db.SaveChanges();
                    scope.Complete();
                }
            }
            catch (Exception)
            {
                throw new ManagementException("Removing all bookings failed.");
            }
        }
        #endregion
    }
}
