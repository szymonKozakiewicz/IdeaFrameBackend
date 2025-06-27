using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.DTO
{
    public class MindMapNodeSaveDTO: MindMapNodeDTO
    {

        public bool WasEdited { get; set; }
        public String UiId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
