using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.DTO
{
    public class MindMapNodeDTO
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public String Color { get; set; }
        public CoordinatesDTO Coordinates { get; set; }
        public bool WasEdited { get; set; }
    }
}
