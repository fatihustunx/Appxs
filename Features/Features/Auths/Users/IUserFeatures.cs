using App.Appxs.eSecurities.Usings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Features.Features.Auths.Users
{
    public interface IUserFeatures
    {
        public Task<bool> Add(User user);
        public Task<User> GetByEmail(string email);
        public Task<bool> isExists(string email);

        public Task<List<OperationClaim>> GetClaims(User user);
    }
}