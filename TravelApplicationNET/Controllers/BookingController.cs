﻿//
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
using System.Web.Mvc;
using TravelApplicationNET.Models;
using TravelApplicationNET.Travel;

namespace TravelApplicationNET.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService bk;

        public BookingController(IBookingService bookingComponent)
        {
            this.bk = bookingComponent;
        }

        //
        // GET: /Booking/

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome!";
            PlannedTrip trip = Session["trip"] as PlannedTrip ?? new PlannedTrip();
            return View(trip.TentativeBookings.ToList());
        }

        //
        // GET: /Booking/Search

        public ActionResult Search()
        {
            ViewBag.Message = "Search for available hotels";
            return View();
        }

        //
        // POST: /Booking/Search

        [HttpPost]
        public ActionResult Search(DateTime startDate, DateTime endDate)
        {
            try
            {
                return RedirectToAction("FreeHotels", new { startDate = startDate, endDate = endDate });
            }
            catch (Exception e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
                return View();
            }
        }

        public ActionResult FreeHotels(DateTime startDate, DateTime endDate)
        {
            ViewBag.Message = "Available hotels";
            ICollection<HotelInfo> freeHotels = bk.FindAllHotelsWithFreeRoomsInPeriod(startDate, endDate);
            return View(freeHotels);
        }

        //
        // GET: /Booking/CreateBooking

        public ActionResult CreateBooking()
        {
            ViewBag.Message = "Booking constraints submission form";
            return View();
        }

        //
        // POST: /Booking/CreateBooking

        [HttpPost]
        public ActionResult CreateBooking(string hotelName, BookingConstraints constraints, string guest)
        {
            try
            {
                Booking b = bk.CreateBooking(hotelName, constraints, guest);
                PlannedTrip trip = Session["trip"] as PlannedTrip ?? new PlannedTrip();
                trip.TentativeBookings.Add(b);
                Session["trip"] = trip;
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
                return View();
            }
        }

        //
        // GET: /Booking/Delete/5

        public ActionResult Delete(int booking)
        {
            ViewBag.Message = "Delete booking";
            PlannedTrip trip = Session["trip"] as PlannedTrip ?? new PlannedTrip();
            return View(FindBooking(trip, booking));
        }

        //
        // POST: /Booking/Delete/5

        [HttpPost]
        public ActionResult Delete(int booking, FormCollection collection)
        {
            try
            {
                PlannedTrip trip = Session["trip"] as PlannedTrip ?? new PlannedTrip();
                trip.TentativeBookings.Remove(FindBooking(trip, booking));
                Session["trip"] = trip;
                return RedirectToAction("Index");
            }
            catch
            {
                return View(booking);
            }
        }

        private Booking FindBooking(PlannedTrip trip, int hashcode)
        {
            return (from booking in trip.TentativeBookings
                    where booking.GetHashCode() == hashcode
                    select booking).First();
        }

        //
        // GET: /Booking/Finalize
        public ActionResult Finalize()
        {
            try
            {
                ViewBag.Message = "The following bookings have been finalized:";
                PlannedTrip trip = Session["trip"] as PlannedTrip ?? new PlannedTrip();
                if (trip.TentativeBookings.Count() == 0)
                    throw new BookingException("There are no tentative bookings to finalize.");
                ICollection<Booking> finalized = bk.FinalizeBookings(trip.TentativeBookings);
                trip.TentativeBookings.Clear();
                Session["trip"] = new PlannedTrip();
                return View(finalized);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
                return View();
            }
        }
    }

    [Serializable]
    public class PlannedTrip
    {
        public ICollection<Booking> TentativeBookings { get; set; }

        public PlannedTrip()
        {
            TentativeBookings = new List<Booking>();
        }
    }
}
