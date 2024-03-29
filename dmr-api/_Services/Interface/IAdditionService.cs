﻿using DMR_API.DTO;
using DMR_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMR_API._Services.Interface
{
    public interface IAdditionService : IECService<AdditionDto>
    {
        Task<List<Building>> GetLinesByBuildingID(int buildingID);
        Task<object> GetAllByBuildingID(int buildingID);
        Task<List<BPFCStatusDto>> GetBPFCSchedulesByApprovalStatus();
        Task<List<IngredientDto>> GetAllChemical();
        Task<List<RemarkDto>> GetAllRemark();
        Task<bool> AddRange(List<AdditionDto> model);
        Task<bool> UpdateRange(AdditionDto model);
        Task<bool> UpdateRemark(RemarkDto model);
        Task<bool> AddRemark(RemarkDto model);
        Task<bool> DeleteRemark(int id);

        Task<bool> DeleteRange(List<int> model, int deleteBy);
    }
}
