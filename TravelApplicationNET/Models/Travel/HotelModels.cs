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
using System.ComponentModel.DataAnnotations;

namespace TravelApplicationNET.Models
{
    public class Hotel
    {
        [Key, StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        public virtual Address Address { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }

        public Hotel()
        {
            Rooms = new List<Room>();
        }

        public Room GetRoom(int roomNb)
        {
            foreach (Room r in Rooms)
            {
                if (r.RoomDetails.RoomNb.Equals(roomNb))
                    return r;
            }
            return null;
        }

        public void AddRoom(Room room)
        {
            if (GetRoom(room.RoomDetails.RoomNb) == null)
            {
                Rooms.Add(room);
                room.HotelName = Name;
            }
            else
                throw new ArgumentException("There is already a room with the same room number.");
        }

        public bool HasFreeRooms(DateTime start, DateTime end)
        {
            foreach (Room room in Rooms)
                if (room.IsFree(start, end))
                    return true;
            return false;
        }

        public IList<RoomDetails> GetFreeRooms(DateTime start, DateTime end)
        {
            var freeRooms = new List<RoomDetails>();
            foreach (Room r in Rooms)
                if (r.IsFree(start, end))
                    freeRooms.Add(r.RoomDetails);
            return freeRooms;
        }
    }

    [Serializable]
    public class HotelInfo
    {
        [Required, Display(Name = "Hotel name")]
        public string HotelName { get; set; }
        [Required]
        public virtual Address Address { get; set; }
    }

    public class Room
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomID { get; set; }
        [Required]
        public virtual RoomDetails RoomDetails { get; set; }
        [Display(Name = "Hotel name")]
        public string HotelName { get; set; }
        public virtual ICollection<Booking> Bookings { get; private set; }

        public bool IsFree(DateTime start, DateTime end)
        {
            if (Bookings == null)
                return false;
            foreach (Booking b in Bookings)
            {
                if (b.EndDate.CompareTo(start) == -1)
                    continue;
                if (b.StartDate.CompareTo(end) == 1)
                    continue;
                return false;
            }
            return true;
        }

        public void AddBooking(Booking booking)
        {
            if (IsFree(booking.StartDate, booking.EndDate))
                Bookings.Add(booking);
            else
                throw new BookingException("Room " + RoomDetails.RoomNb + " in hotel " + booking.HotelName + " not available.");
        }
    }

    [Serializable]
    public class RoomDetails
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomDetailsID { get; set; }
        [Required, Display(Name = "Room number")]
        public int RoomNb { get; set; }
        [Required, Display(Name = "Number of beds")]
        public int NbOfBeds { get; set; }
        [Required, Display(Name = "Price per night")]
        public double PricePerNight { get; set; }
        [Required, Display(Name = "Smoking allowed")]
        public bool SmokingAllowed { get; set; }

        public override string ToString()
        {
            string r = "Room " + RoomNb + ", " + NbOfBeds + " beds, " +
                PricePerNight + " euros per night, ";
            if (SmokingAllowed)
                r += "smoking allowed.";
            else
                r += "non-smoking.";
            return r;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            RoomDetails other = obj as RoomDetails;
            return (other.RoomNb == RoomNb && other.NbOfBeds == NbOfBeds &&
                other.PricePerNight.Equals(PricePerNight) && other.SmokingAllowed == SmokingAllowed);
        }

        public override int GetHashCode()
        {
            int hash = 5;
            hash = 79 * hash + RoomNb;
            hash = 79 * hash + NbOfBeds;
            hash = 79 * hash + Convert.ToInt32(PricePerNight);
            hash = 79 * hash + Convert.ToInt32(SmokingAllowed);
            return hash;
        }
    }

    [Serializable]
    public class Booking
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingID { get; set; }
        [Required]
        public string Guest { get; set; }
        [Required, Display(Name = "Start date")]
        public DateTime StartDate { get; set; }
        [Required, Display(Name = "End date")]
        public DateTime EndDate { get; set; }
        [Required]
        public double Price { get; set; }
        [Required, Display(Name = "Room number")]
        public int RoomNb { get; set; }
        [Required, Display(Name = "Hotel name")]
        public string HotelName { get; set; }

        public override string ToString()
        {
            return "Booking for " + Guest + "\r\n" +
                "Hotel: " + HotelName + ", Room: " + RoomNb + "\r\n" +
                "From " + StartDate + " till " + EndDate + "\r\n" +
                "Total price: " + Price;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            Booking other = obj as Booking;
            return (other.Guest.Equals(Guest) && other.StartDate.Equals(StartDate) &&
                other.EndDate.Equals(EndDate) && other.Price.Equals(Price) &&
                other.RoomNb == RoomNb && other.HotelName.Equals(HotelName));
        }

        public override int GetHashCode()
        {
            int hash = 5;
            hash = 43 * hash + (Guest != null ? Guest.GetHashCode() : 0);
            hash = 43 * hash + (StartDate != null ? StartDate.GetHashCode() : 0);
            hash = 43 * hash + (EndDate != null ? EndDate.GetHashCode() : 0);
            hash = 43 * hash + Convert.ToInt32(Price);
            hash = 43 * hash + RoomNb;
            hash = 43 * hash + (HotelName != null ? HotelName.GetHashCode() : 0);
            return hash;
        }
    }

    [Serializable]
    public class BookingConstraints
    {
        [Required, DataType(DataType.Date), Display(Name = "Start date"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [Required, DataType(DataType.Date), Display(Name = "End date"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        [Required, Display(Name = "Maximum price per night")]
        public double MaxPricePerNight { get; set; }
        [Required, Display(Name = "Number of beds")]
        public int NbOfBeds { get; set; }
        [Required, Display(Name = "Smoking allowed")]
        public bool SmokingAllowed { get; set; }

        public override string ToString()
        {
            return "Booking constraints:\r\n" +
                "From " + StartDate + " till " + EndDate + "\r\n" +
                "# beds: " + NbOfBeds + "\t" + (SmokingAllowed ? "Smoking" : "Non-smoking") + "\r\n" +
                "Max price: " + MaxPricePerNight;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            BookingConstraints other = obj as BookingConstraints;
            return (other.StartDate.Equals(StartDate) && other.EndDate.Equals(EndDate) &&
                other.MaxPricePerNight.Equals(MaxPricePerNight) && other.NbOfBeds == NbOfBeds &&
                other.SmokingAllowed == SmokingAllowed);
        }

        public override int GetHashCode()
        {
            int hash = 5;
            hash = 59 * hash + (StartDate != null ? StartDate.GetHashCode() : 0);
            hash = 59 * hash + (EndDate != null ? EndDate.GetHashCode() : 0);
            hash = 59 * hash + Convert.ToInt32(MaxPricePerNight);
            hash = 59 * hash + NbOfBeds;
            hash = 59 * hash + Convert.ToInt32(SmokingAllowed); ;
            return hash;
        }
    }

    public class BookingException : Exception
    {
        public BookingException(string msg)
            : base(msg)
        { }
    }

    public class ManagementException : Exception
    {
        public ManagementException(string msg)
            : base(msg)
        { }
    }
}
