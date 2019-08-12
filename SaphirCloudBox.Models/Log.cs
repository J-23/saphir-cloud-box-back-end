using SaphirCloudBox.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Models
{
    public class Log
    {
        public int Id { get; set; }

        public LogType LogType { get; set; }

        public string Text { get; set; }
    }
}
