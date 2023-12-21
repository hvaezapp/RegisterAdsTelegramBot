using MehranBot.Models.Context;
using MehranBot.Models.Entities;
using MehranBot.Models.Repository;

namespace MehranBot.Models.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly MehranBotDbContext _context;

    public UnitOfWork(MehranBotDbContext context)
    {
        _context = this._context ?? context;


    }


    #region Objects

    private CrudGenericMethod<User> _userManager;
    private CrudGenericMethod<Ads> _adsManager;
    private CrudGenericMethod<Category> _categoryManager;
    private CrudGenericMethod<Setting> _settingManager;
    private CrudGenericMethod<UserActivity> _userActivityManager;
    private CrudGenericMethod<UserPayment> _userPaymentManager;

    #endregion Objects


    #region Prop
    public CrudGenericMethod<User> UserManagerUW
    {
        get
        {
            if (_userManager == null)
            {
                _userManager = new CrudGenericMethod<User>(_context);
            }

            return _userManager;
        }
    }


    public CrudGenericMethod<Category> CategoryUW
    {
        get
        {
            if (_categoryManager == null)
            {
                _categoryManager = new CrudGenericMethod<Category>(_context);
            }

            return _categoryManager;
        }
    }




    public CrudGenericMethod<Setting> SettingUW
    {
        get
        {
            if (_settingManager == null)
            {
                _settingManager = new CrudGenericMethod<Setting>(_context);
            }

            return _settingManager;
        }
    }


    public CrudGenericMethod<Ads> AdsManagerUW
    {
        get
        {
            if (_adsManager == null)
            {
                _adsManager = new CrudGenericMethod<Ads>(_context);
            }

            return _adsManager;
        }
    }

    public CrudGenericMethod<Category> CategoryManagerUW
    {
        get
        {
            if (_categoryManager == null)
            {
                _categoryManager = new CrudGenericMethod<Category>(_context);
            }

            return _categoryManager;
        }
    }

    public CrudGenericMethod<UserActivity> UserActivityManagerUW
    {
        get
        {
            if (_userActivityManager == null)
            {
                _userActivityManager = new CrudGenericMethod<UserActivity>(_context);
            }

            return _userActivityManager;
        }
    }


    public CrudGenericMethod<Setting> SettingManagerUW
    {
        get
        {
            if (_settingManager == null)
            {
                _settingManager = new CrudGenericMethod<Setting>(_context);
            }

            return _settingManager;
        }
    }


    public CrudGenericMethod<UserPayment> UserPaymentManagerUW
    {
        get
        {
            if (_userPaymentManager == null)
            {
                _userPaymentManager = new CrudGenericMethod<UserPayment>(_context);
            }

            return _userPaymentManager;
        }
    }

    #endregion props


    #region Save
    public async Task<int> saveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void save()
    {
        _context.SaveChanges();
    }

    #endregion Save


    #region Dispose
    protected bool isDisposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            _context.Dispose();
        }

        isDisposed = true;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion


}
