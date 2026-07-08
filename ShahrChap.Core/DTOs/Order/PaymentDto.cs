using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.DTOs.Order
{
    public class PaymentResultDto
    {
        public bool IsSuccessful { get; set; }
        public string Authority { get; set; }
        public string RefId { get; set; }
        public long Amount { get; set; }
    }
}
