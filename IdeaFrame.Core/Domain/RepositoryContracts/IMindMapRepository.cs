using IdeaFrame.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.Domain.RepositoryContracts
{
    public interface IMindMapRepository
    {
        Task AddNewNodes(List<MindMapNode>nodes);

        Task UpdateNodes(List<MindMapNode> nodes);

        Task AddNewBranches(List<MindMapBranch> branches);

        Task UpdateBranches(List<MindMapBranch> branches);


        public Task<List<MindMapNode>> GetNodesByFileId(Guid fileId);
        public Task RemoveNodes(List<MindMapNode> nodes);
        public Task RemoveBranches(List<MindMapBranch> branches);
        Task<List<MindMapBranch>> GetBranchesByFileId(Guid id);
    }
}
