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
    public enum Discount { None = 0, Bronze = 5, Silver = 10, Gold = 20 }

    public class DiscountWrapper
    {
        private Discount _d;

        [Required]
        public int Value
        {
            get
            {
                return (int)_d;
            }
            set
            {
                _d = (Discount)value;
            }
        }

        public Discount EnumValue
        {
            get
            {
                return _d;
            }
            set
            {
                _d = value;
            }
        }

        public static implicit operator DiscountWrapper(Discount d)
        {
            return new DiscountWrapper { EnumValue = d };
        }

        public static implicit operator Discount(DiscountWrapper dw)
        {
            if (dw == null)
                return Discount.None;
            else
                return dw.EnumValue;
        }
    }

    public class UserProfile
    {
        [Key]
        public string UserProfileID { get; set; }
        public string UserName { get; set; }
        public virtual DiscountWrapper Discount { get; set; }
    }
}
