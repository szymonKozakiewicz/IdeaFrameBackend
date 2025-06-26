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

        public async Task<LoadMindMapDTO> GetMindMap(FileSystemItemDTO mindMapFileDTO)
        {
            FileSystemItem mindMapFile = await _directoryService.GetFileItem(mindMapFileDTO);
            List<MindMapNode>nodes=await _mindMapRepository.GetNodesByFileId(mindMapFile.Id);
            List<MindMapBranch> branches = await _mindMapRepository.GetBranchesByFileId(mindMapFile.Id);

            List<MindMapNodeDTO>nodesDTO=nodes.Select(x=>x.ConvertToMindMapDTO()).ToList();
            List<BranchLoadDTO> branchesDTO = branches.Select(x => x.ConvertToBranchLoadDTO()).ToList();
            LoadMindMapDTO loadMindMapDTO = new LoadMindMapDTO()
            {
                Nodes = nodesDTO,
                Branches = branchesDTO
            };
            return loadMindMapDTO;
        }

        public async Task SaveMindMap(SaveMindMapDTO saveDto)
        {
            FileSystemItem mindMapFile = await _directoryService.GetFileItem(saveDto.FileItem);
            List<MindMapNode> addedNodes = await updateNodes(saveDto, mindMapFile);
            updateNewNodesIdInNewBranchesDTO(saveDto, addedNodes);
            await updateBranches(saveDto, mindMapFile);
        }

        private static void updateNewNodesIdInNewBranchesDTO(SaveMindMapDTO saveDto, List<MindMapNode> addedNodes)
        {
            foreach (var addedNode in addedNodes)
            {
                updateNodeIdInAllNewBranches(saveDto, addedNode);
            }
        }

        private static void updateNodeIdInAllNewBranches(SaveMindMapDTO saveDto, MindMapNode addedNode)
        {
            foreach (var branchDTO in saveDto.Branches)
            {
                if (branchDTO.Source.UiId == addedNode.UiId.ToString())
                {
                    branchDTO.Source.Id = addedNode.Id.ToString();
                }
                if (branchDTO.Target.UiId == addedNode.UiId.ToString())
                {
                    branchDTO.Target.Id = addedNode.Id.ToString();
                }
            }
        }


        private async Task<List<MindMapNode>> updateNodes(SaveMindMapDTO saveDto, FileSystemItem mindMapFile)
        {
            List<MindMapNode> mindMapNodesToAdd = getNodesToAdd(saveDto, mindMapFile);
            await _mindMapRepository.AddNewNodes(mindMapNodesToAdd);
            List<MindMapNode> mindMapNodesToUpdate = getNodesToUpdate(saveDto, mindMapFile);
            await _mindMapRepository.UpdateNodes(mindMapNodesToUpdate);
            return mindMapNodesToAdd;
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
                    PositionX = nodeDTO.Coordinates.X,
                    PositionY = nodeDTO.Coordinates.Y,
                    FileId = mindMapFile.Id,
                    Color = nodeDTO.Color,
                    UiId = Guid.Parse(nodeDTO.UiId)
                };
                mindMapNodesToAdd.Add(newMindMapNode);

            }

            return mindMapNodesToAdd;
        }


        private List<MindMapBranch> getBranchesToAdd(SaveMindMapDTO saveDto, FileSystemItem mindMapFile)
        {
            List<MindMapBranch> mindMapBranchesToAdd = new List<MindMapBranch>();
            foreach (var branchDTO in saveDto.Branches)
            {
                if (branchDTO.Id.Length > 0)
                    continue;
                MindMapBranch newMindMapBranch = new MindMapBranch()
                {
                    SourceId = Guid.Parse(branchDTO.Source.Id),
                    TargetId = Guid.Parse(branchDTO.Target.Id),

                };
                mindMapBranchesToAdd.Add(newMindMapBranch);

            }

            return mindMapBranchesToAdd;
        }

        private async Task updateBranches(SaveMindMapDTO saveDto, FileSystemItem mindMapFile)
        {
   
            List<MindMapBranch> mindBranchesToAdd = getBranchesToAdd(saveDto, mindMapFile);

            branchValidation(mindBranchesToAdd);
            await _mindMapRepository.AddNewBranches(mindBranchesToAdd);

        }

        private static void branchValidation(List<MindMapBranch> mindBranchesToAdd)
        {
            foreach (var branch in mindBranchesToAdd)
            {
                if (branch.TargetId == branch.SourceId)
                    throw new Exception("Branch cannot connect to itself");

            }
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
                    PositionX = nodeDTO.Coordinates.X,
                    PositionY = nodeDTO.Coordinates.Y,
                    Id = Guid.Parse(nodeDTO.Id),
                    Color= nodeDTO.Color
                };
                mindMapNodesToUpdate.Add(newMindMapNode);

            }

            return mindMapNodesToUpdate;
        }


    }


}

