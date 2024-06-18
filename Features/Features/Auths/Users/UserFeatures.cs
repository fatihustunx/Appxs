using App.Appxs.Exceptions;
using Microsoft.EntityFrameworkCore;
using App.Appxs.eSecurities.Usings;
using App.xContexts.Apps;
using System;

namespace Features.Features.Auths.Users
{
    public class UserFeatures : IUserFeatures
    {
        private readonly AppDbContext _appDbContext;

        public UserFeatures(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<bool> Add(User user)
        {
            await _appDbContext.AddAsync(user);
            await _appDbContext.SaveChangesAsync();

            var userRol = await _appDbContext.Set<OperationClaim>().
                FirstOrDefaultAsync(c => c.Name.Equals("User"));

            if (userRol==null) 
            {
                await _appDbContext.AddAsync(new OperationClaim { Name = "User" }); await _appDbContext.SaveChangesAsync();

                userRol = await _appDbContext.Set<OperationClaim>().FirstOrDefaultAsync(c => c.Name.Equals("User"));
            }

            var useR = await _appDbContext.Set<User>().FirstOrDefaultAsync(c=>c.Email.Equals(user.Email));

            await _appDbContext.AddAsync(new UserOperationClaim
            { UserId = useR.Id, OperationClaimId = userRol.Id });

            await _appDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<User> GetByEmail(string email)
        {
            var user = await _appDbContext.Set<User>().
                FirstOrDefaultAsync(x => x.Email.Equals(email));

            if (user == null)
            {
                throw new BusinessException("Böyle bir user yok.");
            }

            return user;
        }

        public async Task<bool> isExists(string email)
        {
            var isExists = await _appDbContext.Set<User>().AnyAsync(x=>x.Email.Equals(email));

            if(!isExists) { return false; }
            else { return true; }
        }

        public async Task<List<OperationClaim>> GetClaims(User user)
        {
            var result = from operationClaim in _appDbContext.OperationClaims
                         join userOperationClaim in _appDbContext.UserOperationClaims
                             on operationClaim.Id equals userOperationClaim.OperationClaimId
                         where userOperationClaim.UserId == user.Id
                         select new OperationClaim { Id = operationClaim.Id, Name = operationClaim.Name };

            return await result.ToListAsync();
        }
    }
}