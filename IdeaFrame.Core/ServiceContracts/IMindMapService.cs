using IdeaFrame.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.ServiceContracts
{
    public interface IMindMapService
    {
        public Task SaveMindMap(SaveMindMapDTO saveDto);

        public Task<List<MindMapNodeDTO>> GetMindMap(FileSystemItemDTO mindMapFileDTO);

    }
}
