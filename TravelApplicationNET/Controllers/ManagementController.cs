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
using TravelApplicationNET.Travel.Management;
using TravelApplicationNET.Models;

namespace TravelApplicationNET.Controllers
{
    public class ManagementController : Controller
    {
        private readonly IManagementService mgmt;

        public ManagementController(IManagementService managementComponent)
        {
            this.mgmt = managementComponent;
        }

        //
        // GET: /Management/
        // show list of hotels

        public ActionResult Index()
        {
            ViewBag.Message = "Hotels";
            return View(mgmt.GetHotels());
        }

        //
        // GET: /Management/Rooms/Rilton
        // show list of rooms in given hotel

        public ActionResult Rooms(string hotelName)
        {
            ViewBag.Message = "Rooms in " + hotelName;
            return View(mgmt.GetRooms(hotelName));
        }

        //
        // GET: /Management/Bookings/Rilton
        // show list of bookings in given hotel and room

        public ActionResult Bookings(string hotelName, int? roomNb)
        {
            ViewBag.Message = "Bookings";
            if (hotelName == null)
                return View(mgmt.GetBookings());
            if (roomNb == null)
            {
                ViewBag.Message += " in " + hotelName;
                return View(mgmt.GetBookings(hotelName));
            }
            ViewBag.Message += " in " + hotelName + ", room " + roomNb;
            return View(mgmt.GetBookings(hotelName, roomNb));
        }

        //
        // GET: /Management/AddHotel

        public ActionResult AddHotel()
        {
            ViewBag.Message = "Add hotel";
            return View();
        }

        //
        // POST: /Management/AddHotel

        [HttpPost]
        public ActionResult AddHotel(HotelInfo hotelInfo)
        {
            try
            {
                mgmt.AddNewHotel(hotelInfo.HotelName, hotelInfo.Address);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
                return View();
            }
        }

        ////
        //// GET: /Management/RemoveHotel/Rilton

        //public ActionResult RemoveHotel(string hotelName)
        //{
        //    TODO
        //    return View();
        //}

        ////
        //// POST: /Management/RemoveHotel/Rilton

        //[HttpPost]
        //public ActionResult RemoveHotel(string hotelName, FormCollection collection)
        //{
        //    try
        //    {
        //        TODO
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //
        // GET: /Config/AddRoom

        public ActionResult AddRoom(string hotelName)
        {
            ViewBag.Message = "Add room to " + hotelName;
            return View();
        }

        //
        // POST: /Config/AddRoom

        [HttpPost]
        public ActionResult AddRoom(string hotelName, RoomDetails roomDetails)
        {
            try
            {
                mgmt.AddNewRoomToHotel(hotelName, roomDetails);
                return RedirectToAction("Rooms", "Management", new { hotelName = hotelName });
            }
            catch (Exception e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
                return View();
            }
        }

        //
        // POST: /Config/Cleanup

        public ActionResult Cleanup()
        {
            ViewBag.Message = "Clean up all bookings";
            return View(mgmt.GetBookings());
        }

        //
        // POST: /Config/Cleanup

        [HttpPost]
        public ActionResult Cleanup(FormCollection collection)
        {
            try
            {
                mgmt.RemoveAllBookings();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(String.Empty, e.Message);
                return View();
            }
        }
    }
}
