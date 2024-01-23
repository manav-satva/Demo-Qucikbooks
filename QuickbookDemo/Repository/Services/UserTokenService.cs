using DatabaseContextLibrary.DatabaseContext;
using DemoQuickBooks.Repository.Interfaces;
using ModelDataLibrary.Models;

namespace DemoQuickBooks.Repository.Services
{
    public class UserTokenService : IUserToken
    {
        private readonly DatabaseContextInfo _context;
        public UserTokenService(DatabaseContextInfo contextInfo)
        {
            _context = contextInfo;
        }

        public UserToken GetByRealMId(string realmId)
        {
            return _context.UserTokens.FirstOrDefault(x => x.RealMId == realmId);
        }
        public void AddUserToken(UserToken Obj)
        {
            _context.UserTokens.Add(Obj);

        }
        public void UpdateUserToken(UserToken Obj)
        {
            _context.UserTokens.Update(Obj);
        }
        public void SaveUserTokenChanges()
        {
            _context.SaveChanges();
        }

    }
}
