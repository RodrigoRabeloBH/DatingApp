using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Interfaces.Repository;
using API.Interfaces.Services;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _rep;

        private readonly IMapper _mapper;

        private readonly IPhotoService _photoService;

        private readonly ILikeServices _likeServices;

        public UserServices(IUserRepository rep, IMapper mapper, IPhotoService photoService, ILikeServices likeServices)
        {
            _rep = rep;
            _mapper = mapper;
            _photoService = photoService;
            _likeServices = likeServices;
        }

        public async Task<PhotoModel> AddPhoto(string username, IFormFile file)
        {
            var user = await _rep.GetUserByUsernameAsync(username);

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return null;

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            await _rep.Update(user);

            return _mapper.Map<PhotoModel>(photo);
        }

        public async Task<PagedList<MemberModel>> GetAllMembers(UserParams userParams, string username)
        {
            var currentUser = await _rep.GetUserByUsernameAsync(username);

            userParams.CurrentUsername = currentUser.UserName;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = currentUser.Gender.ToLower() == "male" ? "female" : "male";
            }

            var users = await _rep.GetMembersAsync(userParams);

            return users;
        }

        public async Task<MemberModel> GetByUsername(string username)
        {
            var user = await _rep.GetMemberAsync(username);

            if (user == null) return null;

            return user;
        }

        public async Task<bool> RemovePhoto(string username, int photoId)
        {
            var user = await _rep.GetUserByUsernameAsync(username);

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo.IsMain) return false;

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                if (result.Error != null) return false;
            }

            user.Photos.Remove(photo);

            await _rep.Update(user);

            return true;
        }

        public async Task<bool> SetMainPhoto(string username, int photoId)
        {
            var user = await _rep.GetUserByUsernameAsync(username);

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo.IsMain) return false;

            var currentMain = user.Photos.FirstOrDefault(p => p.IsMain);

            if (currentMain != null) currentMain.IsMain = false;

            photo.IsMain = true;

            await _rep.Update(user);

            return true;
        }

        public async Task<bool> UpdateUser(string username, MemberUpdateModel model)
        {
            var user = await _rep.GetUserByUsernameAsync(username);

            _mapper.Map(model, user);

            var result = await _rep.Update(user);

            return result;
        }

        public async Task<bool> RemoveUser(string username)
        {
            var user = await _rep.GetUserByUsernameAsync(username);

            await _likeServices.RemoveAllLikes(user.Id);

            var result = await _rep.Delete(user);

            return result;
        }
    }
}
