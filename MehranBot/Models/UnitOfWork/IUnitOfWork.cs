using MehranBot.Models.Entities;
using MehranBot.Models.Repository;

namespace MehranBot.Models.UnitOfWork;

public interface IUnitOfWork : IDisposable
{

    CrudGenericMethod<User> UserManagerUW { get; }
    CrudGenericMethod<Ads> AdsManagerUW { get; }
    CrudGenericMethod<Category> CategoryManagerUW { get; }
    CrudGenericMethod<UserActivity> UserActivityManagerUW { get; }
    CrudGenericMethod<Setting> SettingManagerUW { get; }
    CrudGenericMethod<UserPayment> UserPaymentManagerUW { get; }



    Task<int> saveAsync();

    void save();
}
