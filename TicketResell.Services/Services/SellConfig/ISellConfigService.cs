﻿using Repositories.Core.Dtos.SellConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketResell.Services.Services
{
    public interface ISellConfigService
    {
        public Task<ResponseModel> CreateSellConfigAsync(SellConfigCreateDto dto, bool saveAll = true);
        public Task<ResponseModel> GetSellConfigByIdAsync(string id);
        public Task<ResponseModel> GetAllSellConfigAsync();
        public Task<ResponseModel> UpdateSellConfigAsync(string id, SellConfigUpdateDto dto, bool saveAll = true);
        public Task<ResponseModel> DeleteSellConfigAsync(string id, bool saveAll = true);
    }
}
