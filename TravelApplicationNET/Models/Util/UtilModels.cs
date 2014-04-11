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
    public class Address
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressID { get; set; }
        [StringLength(30, MinimumLength = 3)]
        public string Street { get; set; }
        [Range(1, 9999)]
        public int Number { get; set; }
        [Range(1, 999999), Display(Name = "Postal code")]
        public int PostalCode { get; set; }
        [StringLength(25, MinimumLength = 3)]
        public string City { get; set; }
        [StringLength(15, MinimumLength = 3)]
        public string Country { get; set; }

        public override string ToString()
        {
            return Street + "  " + Number + "\r\n"
                + PostalCode + "  " + City.ToUpper() + "\r\n"
                + Country;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            Address other = obj as Address;
            return (other.Street.Equals(Street) && other.Number == Number &&
                other.PostalCode == PostalCode && other.City.Equals(City) && other.Country.Equals(Country));
        }

        public override int GetHashCode()
        {
            int hash = 5;
            hash = 53 * hash + Number;
            hash = 53 * hash + (Street != null ? Street.GetHashCode() : 0);
            hash = 53 * hash + PostalCode;
            hash = 53 * hash + (City != null ? City.GetHashCode() : 0);
            hash = 53 * hash + (Country != null ? Country.GetHashCode() : 0);
            return hash;
        }
    }
}
