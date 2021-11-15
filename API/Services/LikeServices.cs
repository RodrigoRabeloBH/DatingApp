using API.Entities;
using API.Helpers;
using API.Interfaces.Repository;
using API.Interfaces.Services;
using API.Models;
using System.Threading.Tasks;

namespace API.Services
{
    public class LikeServices : ILikeServices
    {
        private readonly IUserRepository _userRepository;

        private readonly ILikesRepository _rep;

        public LikeServices(IUserRepository userRepository, ILikesRepository rep)
        {
            _userRepository = userRepository;
            _rep = rep;
        }

        public async Task AddLike(int sourceUserId, string likedUsername)
        {
            var likedUser = await _userRepository.GetUserByUsernameAsync(likedUsername);

            var sourceUser = await _rep.GetUserWithLikes(sourceUserId);

            var userLike = await _rep.GetUserLike(sourceUser.Id, likedUser.Id);

            if (userLike != null)
            {
                await _rep.RemoveLike(userLike);

                await _userRepository.Update(sourceUser);

                return;
            }

            userLike = new UserLike
            {
                SourceUserId = sourceUser.Id,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            await _userRepository.Update(sourceUser);
        }

        public async Task<PagedList<LikeModel>> GetUserLikes(LikesParams likesParams)
        {
            var users = await _rep.GetUserLikes(likesParams);

            return users;
        }
    }
}
