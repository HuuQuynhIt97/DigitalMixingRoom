﻿using DMR_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMR_API.DTO
{
    public class IngredientInfoDto
    {
        public IngredientInfoDto()
        {
            this.CreatedTime = DateTime.Now;
            this.CreatedDate = DateTime.Now;
        }
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ManufacturingDate { get; set; }
        public string SupplierName { get; set; }
        public string Batch { get; set; }
        public DateTime ExpiredTime { get; set; }
        public double Qty { get; set; }
        public string Consumption { get; set; }
        public bool Status { get; set; }

    }
}
