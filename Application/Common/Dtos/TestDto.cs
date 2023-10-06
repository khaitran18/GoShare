using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class TestDto
    {
        public bool IsSuccess { get; set; }
        public User? user { get; set; }
    }
}
