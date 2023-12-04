﻿using AutoMapper;
using Core_Layer.DTOs;
using Core_Layer.Enums;
using Core_Layer.Models;
using Core_Layer.Services;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service_Layer.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.Services
{
    public class AppUserServices : IAppUserServices
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        public AppUserServices(IUnitOfWork UnitOfWork, IMapper mapper)
        {
            _unitOfWork = UnitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> ADD_USER(UserDto userDto, UserRole userRole)
        {
            ApplicationUser newUser = _mapper.Map<ApplicationUser>(userDto);
            string? NewUserRole = Enum.GetName(userRole);
            if (NewUserRole == null)
            {
                return new NotFoundObjectResult("Role is not found");
            }

            try
            {
                 
                IdentityResult result = await _unitOfWork._userRepository.AddUser(newUser);
                if (result.Succeeded)
                {
                    await _unitOfWork._userRepository.AssignRole(newUser,NewUserRole);
                    try
                    {
                        await _unitOfWork._userRepository.AddSignInCookie(newUser,userDto.RememberMe);
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork._userRepository.DeleteUser(newUser);
                        return new ObjectResult($"An error occurred while Creating cookie \n: {ex.Message}")
                        {
                            StatusCode = 500
                        };
                    }
                    return new OkObjectResult(newUser);
                }
                else
                {
                    return new BadRequestResult();
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork._userRepository.DeleteUser(newUser);
                return new BadRequestObjectResult($"{ex.Message}\n {ex.InnerException?.Message}");
            }

        }
      
        public async Task<IActionResult> GetUserImage(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return  new NotFoundResult();
            }

            //return PhysicalFile
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            var fileStream = new MemoryStream(fileBytes);
            string fileName = Path.GetFileName(path);
            var formFile = new FormFile(fileStream, 0, fileStream.Length, null, fileName);

            return new OkObjectResult(formFile);

        }

        public async Task<IActionResult> SignInUser(SignInDto signInDto)
        {
            ApplicationUser user = await _unitOfWork._userRepository.FindByEmail(signInDto.Email);
            if(user == null)
            {
                return new NotFoundResult();
            }
             await _unitOfWork._userRepository.SignIn(user, signInDto.RememberMe);
            return new OkObjectResult(user);
        }
        public async Task<IActionResult> LogOut()
        {
          await  _unitOfWork._userRepository.SignOut();
            return new OkResult();
        }
    }
}

