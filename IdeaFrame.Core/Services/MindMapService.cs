using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.Domain.RepositoryContracts;
using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.Services
{
    public class MindMapService : IMindMapService
    {
        IDirectoryService _directoryService;
        IMindMapRepository _mindMapRepository;
        public MindMapService(IDirectoryService directoryService,IMindMapRepository mindMapRepository)
        {
            _directoryService = directoryService;
            _mindMapRepository = mindMapRepository;
        }
        public async Task SaveMindMap(SaveMindMapDTO saveDto)
        {
            FileSystemItem mindMapFile = await _directoryService.GetFileItem(saveDto.FileItem);
            List<MindMapNode> mindMapNodesToAdd = getNodesToAdd(saveDto, mindMapFile);
            await _mindMapRepository.AddNewNodes(mindMapNodesToAdd);
            List<MindMapNode> mindMapNodesToUpdate = getNodesToUpdate(saveDto, mindMapFile);
            await _mindMapRepository.UpdateNodes(mindMapNodesToUpdate);
        }

        private List<MindMapNode> getNodesToAdd(SaveMindMapDTO saveDto, FileSystemItem mindMapFile)
        {
            List<MindMapNode> mindMapNodesToAdd = new List<MindMapNode>();
            foreach (var nodeDTO in saveDto.Nodes)
            {
                if (nodeDTO.Id.Length > 0)
                    continue;
                MindMapNode newMindMapNode = new MindMapNode()
                {

                    Name = nodeDTO.Name,
                    MindMapFile = mindMapFile,
                    positionX = nodeDTO.Coordinates.X,
                    positionY = nodeDTO.Coordinates.Y,
                    FileId = mindMapFile.Id,
                };
                mindMapNodesToAdd.Add(newMindMapNode);

            }

            return mindMapNodesToAdd;
        }

        private List<MindMapNode> getNodesToUpdate(SaveMindMapDTO saveDto, FileSystemItem mindMapFile)
        {
            List<MindMapNode> mindMapNodesToUpdate = new List<MindMapNode>();
            foreach (var nodeDTO in saveDto.Nodes)
            {
                if (!nodeDTO.WasEdited)
                    continue;
                MindMapNode newMindMapNode = new MindMapNode()
                {

                    Name = nodeDTO.Name,
                    MindMapFile = mindMapFile,
                    positionX = nodeDTO.Coordinates.X,
                    positionY = nodeDTO.Coordinates.Y,
                    Id = Guid.Parse(nodeDTO.Id)
                };
                mindMapNodesToUpdate.Add(newMindMapNode);

            }

            return mindMapNodesToUpdate;
        }

    }


}

