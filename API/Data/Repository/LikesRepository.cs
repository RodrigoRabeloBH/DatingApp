using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces.Repository;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data.Repository
{
    public class LikesRepository : ILikesRepository
    {
        private DataContext _context;

        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task RemoveLike(UserLike userLike)
        {
            _context.Likes.Remove(userLike);

            await _context.SaveChangesAsync();
        }

        public async Task<PagedList<LikeModel>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();

            var likes = _context.Likes.AsQueryable();

            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(l => l.SourceUserId == likesParams.UserId);
                users = likes.Select(l => l.LikedUser);
            }

            if (likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(l => l.LikedUserId == likesParams.UserId);
                users = likes.Select(l => l.SourceUser);
            }

            var liskdUsers = users.Select(user => new LikeModel
            {
                Age = user.DateOfBirth.CalculateAge(),
                City = user.City,
                Id = user.Id,
                KnownAs = user.KnownAs,
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                UserName = user.UserName
            });

            return await PagedList<LikeModel>.CreateAsync(liskdUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(u => u.LikedUsers)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<UserLike>> GetLikes(int sourceUserId)
        {
            var likes = await _context.Likes
                .Where(l => l.SourceUserId == sourceUserId || l.LikedUserId == sourceUserId)
                .ToListAsync();

            return likes;
        }

        public async Task<bool> RemoveAllLikes(IEnumerable<UserLike> likes)
        {
            _context.Likes.RemoveRange(likes);

            var result = await _context.SaveChangesAsync();

            return result != 0;
        }
    }
}
