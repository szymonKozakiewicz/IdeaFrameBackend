using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.DTO
{
    public class LoadMindMapDTO
    {
        public List<MindMapNodeDTO> Nodes { get; set; }
        public List<BranchLoadDTO> Branches { get; set; }
    }
}
