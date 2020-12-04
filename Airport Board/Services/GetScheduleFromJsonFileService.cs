using Airport_Board.Models;
using Airport_Board.Services.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Airport_Board.Services
{
    internal class GetScheduleFromJsonFileService : IGetScheduleFromFileService
    {
        public IEnumerable<ScheduleRow> GetScheduleFromFile(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            var schedule = JsonSerializer.Deserialize<List<ScheduleRow>>(jsonString);
            return schedule; // List!!!
        }
    }
}
