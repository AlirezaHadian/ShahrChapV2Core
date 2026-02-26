using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShahrChap.DataLayer.Entities.Order
{
    public class OrderDetailService
    {
        [Key]
        public int OrderDetailServiceID { get; set; }
        public int OrderDetailID { get; set; }
        public int ServiceID { get; set; }
        public string ServiceTitle { get; set; }
        public long ServicePrice { get; set; }
        #region Relations
        public virtual OrderDetail OrderDetail { get; set; }
        #endregion
    }
}
