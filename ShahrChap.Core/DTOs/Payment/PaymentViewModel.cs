using System;
using System.Collections.Generic;
using System.Text;

namespace ShahrChap.Core.DTOs.Payment
{
    public class PaymentGatewayViewModel
    {
        public int CartId { get; set; }
        public long Amount { get; set; }
    }
}
