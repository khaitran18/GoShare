using Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DataModels
{
    public partial class Setting
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = null!;
        public double Value { get; set; }
        public SettingDataUnit DataUnit { get; set; }
    }
}
