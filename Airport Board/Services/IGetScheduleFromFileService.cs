using Airport_Board.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airport_Board.Services
{
    internal interface IGetScheduleFromFileService
    {
        IEnumerable<ScheduleRow> GetScheduleFromFile(string filePath);
    }
}
