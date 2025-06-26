using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.DTO
{
    public class SaveMindMapDTO
    {
        public FileSystemItemDTO FileItem { get; set; }
        public List<MindMapNodeSaveDTO>Nodes { get; set; }
        public List<MindMapBranchSaveDTO> Branches { get; set; }
    }
}
