using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.DTO
{
    public class CoordinatesDTO
    {
        public int X { get; set; }
        public int Y { get; set; }

        public CoordinatesDTO() { }

        public CoordinatesDTO(int x, int y)
        {
            X = x;
            Y = y;
        }

        
    }
}
