﻿using Microsoft.AspNetCore.Mvc;
using backend.Infrastucture;
using AutoMapper;
using backend.Domain.Cores.TargetAggregate;
using backend.Models.TargetDtos;
using backend.Models.SubTaskDtos;
using backend.Models.TokenDtos;
using backend.Domain.Cores.TokenAggregate;
using backend.Domain.Cores.SubTaskAggregate;
using backend.Models.Targets;
using backend.Models.TokenDtos;
using Microsoft.AspNetCore.Authorization;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Http.HttpResults;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PDPController : ControllerBase
    {
        private readonly exinDBContext _dBContext;
        private readonly IMapper _mapper;
        private readonly ITargetRepository _targetRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly ISubTaskRepository _subTaskRepository;
        public PDPController(exinDBContext dBContext, ITargetRepository targetRepository, IMapper mapper, ITokenRepository tokenRepository, ISubTaskRepository  subTaskRepository )
        {
            dBContext = _dBContext;
            targetRepository = _targetRepository;
            mapper = _mapper;
            tokenRepository = _tokenRepository;
            subTaskRepository = _subTaskRepository;
        }
        [HttpGet("{token}")]
        public async Task<ActionResult<TokenDto>> GetTokenAsync(Token token)
        {
            var _token = await _tokenRepository.GetTokenAsync(token);
            if (_token is null)
            {
                return NotFound($"targetId{token}not found");
            }
            var tokenDto = _mapper.Map<TokenDto>(_token);
            return Ok(tokenDto);
        }
        [Authorize]
        public async Task<ActionResult<TokenDto>> CheckTokenAsync(Token token)
        {
            var item=await _tokenRepository.CheckTokenAsync(token);
           
            if (item is null) { 
                return  NotFound($"UserId{token}notfound");
            }
            else if(item.Timeout > DateTime.Now.TimeOfDay){

                  _tokenRepository.DelTokenAsync(token);
                return NotFound($"UserId{token}notfound");
            }
            else {
                var tokenmodel = _mapper.Map<Token>(item);
                return Ok(tokenmodel);
            }
        }
        public async Task<ActionResult<TokenDto>> DelTokenAsync(Token token)
        {
            var _token = await _tokenRepository.GetTokenAsync(token);
            if (token is null)
            {
                return NotFound($"targetid{token}notfound");
            }
            await _tokenRepository.DelTokenAsync( _token);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TargetDto>> GetTargetByIdAsync( Guid id,Token token)
        {
            if (CheckTokenAsync(token)!=null)
            {
                var target=await _targetRepository.GetTargetByIdAsync(id,token);
                if (target is null)
                {
                    return NotFound($"targetId{id}not found");
                }
                var targetDto=_mapper.Map<TargetDto>(target);
                return Ok(targetDto);
            }
            return Unauthorized();
        }

        [HttpPost]
        public async Task<ActionResult<TargetDto>> AddTargetAsync(Target target, Token token,TargetForAddDto targetForAddDto)
        {
            if (CheckTokenAsync(token) != null)
            {
                var _target = _mapper.Map<Target>(targetForAddDto);
                await _targetRepository.AddTargetAsync(target,token);
                var targetDto=_mapper.Map<TargetDto>(_target);
                return Ok(targetDto);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPut("{targetId}")]
        public async Task<ActionResult<TargetDto>> EditeTargetAsync(Guid targetId, Token token,TargetForEditeDto targetForEditeDto)
        {
            if (CheckTokenAsync(token)!=null)
            {
                var target=await _targetRepository.GetTargetByIdAsync(targetId,token);
                if (target is null)
                {
                    return NotFound($"targetid{targetId}notfound");
                    
                }
                _mapper.Map(target,targetForEditeDto );
                return Ok(target);
            }
            return Unauthorized();
        }
        [HttpDelete("{targetId}")]
        public async Task<ActionResult<TargetDto>> DeleteTargetAsync(Guid targetId, Token token)
        {
            if (CheckTokenAsync(token) != null)
            {
                var target = await _targetRepository.GetTargetByIdAsync(targetId, token);
                if (target is null)
                {
                    return NotFound($"targetid{targetId}notfound");
                }
                await _targetRepository.DeleteTargetAsync(targetId, token);
                return Ok();
            }
            return Unauthorized();
        }
        
        public async Task<ActionResult<SubTaskDto>> GetSubTaskByIdAsync(Guid id, Token token)
        {
            if (CheckTokenAsync(token) != null)
            {
                var subtask = await _subTaskRepository.GetSubTaskByIdAsync(id, token);
                if (subtask is null)
                {
                    return NotFound($"subTaskId{id}not found");
                }
                var subtaskDto = _mapper.Map<SubTaskDto>(subtask);
                await _subTaskRepository.CheckStatusTaskAsync(id,);
                return Ok();
            }
            return Unauthorized();

        }

        [HttpPost]
        public async Task<ActionResult<SubTaskDto>> AddSubTaskAsync( SubTask subTask,Token token,SubTaskForAddDto subTaskForAdd)
        {
            if (CheckTokenAsync(token) != null)
            {
                var _subtask = _mapper.Map<SubTask>(subTaskForAdd);
                await _subTaskRepository.AddSubTaskAsync(subTask, token);
                var subtaskDto = _mapper.Map<SubTaskDto>(_subtask);
                return Ok(subtaskDto);
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPut("{guid}")]
        public async Task<ActionResult<SubTaskDto>> EditeSubTaskAsync(Guid guid, Token token,SubTaskForEditeDto subTaskForEditeDto)
        {
            if (CheckTokenAsync(token) != null)
            {
                var subtask = await _targetRepository.GetTargetByIdAsync(guid, token);
                if (subtask is null)
                {
                    return NotFound($"targetid{guid}notfound");

                }
                _mapper.Map(subtask, subTaskForEditeDto);
                return Ok(subtask);
            }
            return Unauthorized();
        }
        [HttpDelete("{subtaskId}")]
        public async Task<ActionResult<SubTaskDto>> DeleteSubTaskAsync(Guid  subtaskId, Token token)
        {
             if (CheckTokenAsync(token) != null)
            {
                var subtask = await _subTaskRepository.GetSubTaskByIdAsync(subtaskId, token);
                if (subtask is null)
                {
                    return NotFound($"targetid{subtaskId}notfound");
                }
                await _subTaskRepository.DeleteSubTaskAsync(subtaskId, token);
                return Ok();
            }
            return Unauthorized();
        }
        [HttpGet("{suntaskId}")]
        public async Task<ActionResult<SubTaskDto>> CheckStatusTaskAsync(Guid subtaskId, Target target,Token token)
        {
            if (CheckTokenAsync(token)!=null)
            {
                var subtask = await _subTaskRepository.GetSubTaskByIdAsync(subtaskId, token);
                if(subtask is null)
                {
                    return NotFound($"targetid{subtaskId}notfound");
                }
                var result = subtask.Status;
                return Ok(result);
            }
            return Unauthorized();
        }


    }
}
