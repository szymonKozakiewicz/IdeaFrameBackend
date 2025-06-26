using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.DTO
{
    public class MindMapBranchDTO
    {
        public String Id { get; set; }

        public NodeForBranchSaveDTO Target { get; set; }

        public NodeForBranchSaveDTO Source { get; set; }



    }
}
