using ModelDataLibrary.Models;

namespace DemoQuickBooks.Repository.Interfaces
{
    public interface IUserToken
    {
        UserToken GetByRealMId(string realmId);
        void AddUserToken(UserToken Obj);
        void UpdateUserToken(UserToken Obj);
        void SaveUserTokenChanges();
    }
}
