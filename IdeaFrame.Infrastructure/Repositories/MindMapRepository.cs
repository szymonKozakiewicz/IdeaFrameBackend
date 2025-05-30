using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.Domain.RepositoryContracts;
using IdeaFrame.Infrastructure.DbContextCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Infrastructure.Repositories
{
    public class MindMapRepository: IMindMapRepository
    {
        private MyDbContexSqlServer _dbContext;
        public MindMapRepository(MyDbContexSqlServer dbContext)
        {
            _dbContext = dbContext; 
        }

        public async Task AddNewNodes(List<MindMapNode> nodes)
        {
            foreach (var node in nodes)
            {
                _dbContext.MindMapNodes.Add(node);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateNodes(List<MindMapNode> nodes)
        {
            _dbContext.MindMapNodes.UpdateRange(nodes);
            await _dbContext.SaveChangesAsync();

        }
    }    
}
