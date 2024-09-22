﻿
using AutoMapper;
using Backend.Core.Dtos.Role;
using Backend.Core.Entities;
using Backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private IRoleRepository _roleRepository { get; }
        private IMapper _mapper { get; }

        public RoleController(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDto dto)
        {
            Role role = _mapper.Map<Role>(dto);
            await _roleRepository.CreateRoleAsync(role);
            return Ok(new { message = $"Successfully created role: {dto.Rolename}" });
        }

        [HttpGet]
        [Route("read")]
        public async Task<ActionResult<IEnumerable<Role>>> ReadRole()
        {
            var roles = await _roleRepository.ReadRoleAsync();
            var convertedRoles = _mapper.Map<IEnumerable<RoleReadDto>>(roles);
            return Ok(convertedRoles);

        }

        [HttpPut]
        [Route("update/{roleId}")]
        public async Task<IActionResult> UpdateRole(string roleId, [FromBody] RoleUpdateDto dto)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleId);
            if (role == null)
            {
                return NotFound($"SellConfig with ID {roleId} not exist");
            }
            _mapper.Map(dto, role);
            await _roleRepository.UpdateRoleAsync(role);
            return Ok(new { message = $"Successfully update sell config: {roleId}" });
        }

        [HttpDelete]
        [Route("delete/{roleId}")]
        public async Task<ActionResult<Role>> DeleteSellConfig(string roleId)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleId);
            if (role == null)
            {
                return NotFound($"SellConfig with ID {roleId} not exist");
            }
            await _roleRepository.DeleteRoleAsync(role);
            return Ok(role);
        }

    }
}