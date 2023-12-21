using MehranBot.Models.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MehranBot.Models.Repository;

public class CrudGenericMethod<Tentity> where Tentity : class
{
    private readonly MehranBotDbContext _context;

    private DbSet<Tentity> _table;


    public CrudGenericMethod(MehranBotDbContext context)
    {
        _context = context;
        _table = context.Set<Tentity>();

    }


    public virtual void Create(Tentity entity)
    {
        _table.Add(entity);
    }

    public virtual void Update(Tentity entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _table.Attach(entity);
        }
        _context.Entry(entity).State = EntityState.Modified;
    }


    public virtual void Delete(Tentity entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _table.Attach(entity);
        }
        _table.Remove(entity);

    }

    public virtual void DeleteAll(IEnumerable<Tentity> entity)
    {
        _table.RemoveRange(entity);

    }

    public virtual async Task DeleteById(object id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
            Delete(entity);
    }


    public virtual async Task<IEnumerable<Tentity>> GetAsync(Expression<Func<Tentity, bool>> where = null, Func<IQueryable<Tentity>,
       IOrderedQueryable<Tentity>> orderbyVariable = null, string joinString = "")
    {
        IQueryable<Tentity> query = _table;

        if (where != null)
        {
            query = query.Where(where);
        }

        if (orderbyVariable != null)
        {
            query = orderbyVariable(query);
        }

        if (joinString != "")
        {
            foreach (string joins in joinString.Split(','))
            {
                query = query.Include(joins);
            }
        }

        return await query.AsNoTracking().ToListAsync();

    }


    public virtual async Task<IEnumerable<Tentity>> GetAsync(Expression<Func<Tentity, bool>> where = null)
    {
        return await _table.Where(where).AsNoTracking().ToListAsync();

    }

    public virtual async Task<IEnumerable<Tentity>> GetSkipAsync(Expression<Func<Tentity, bool>> where = null, Func<IQueryable<Tentity>,
      IOrderedQueryable<Tentity>> orderbyVariable = null, string joinString = "", int skip = 0, int take = 0)
    {
        IQueryable<Tentity> query = _table;

        if (where != null)
        {
            query = query.Where(where);
        }

        if (orderbyVariable != null)
        {
            query = orderbyVariable(query);
        }

        if (joinString != "" && !string.IsNullOrEmpty(joinString))
        {
            foreach (string joins in joinString.Split(','))
            {
                query = query.Include(joins);
            }
        }

        return await query.Skip(skip).Take(take).AsNoTracking().ToListAsync();

    }



    public virtual async Task<int> GetCountAsync(Expression<Func<Tentity, bool>> where = null)
    {
        if (where != null)
            return await _table.Where(where).CountAsync();
        return await _table.CountAsync();

    }


    public virtual async Task<IEnumerable<Tentity>> GetAllAsync()
    {
        return await _table.AsNoTracking().ToListAsync();
    }


    public virtual async Task<Tentity> GetSingleAsync(Expression<Func<Tentity, bool>> where = null)
    {
        if (where != null)
            return await _table.Where(where).FirstOrDefaultAsync();
        return await _table.FirstOrDefaultAsync();
    }


    public virtual Tentity GetSingle(Expression<Func<Tentity, bool>> where = null)
    {
        if (where != null)
            return  _table.Where(where).FirstOrDefault();
        return  _table.FirstOrDefault();
    }


    public virtual async Task<Tentity> GetSingleAsync(Expression<Func<Tentity, bool>> where= null, string join="")
    {
       
        IQueryable<Tentity> query = _table;

        if (where != null)
        {
            query = query.Where(where);
        }

        if (join != "" && !string.IsNullOrEmpty(join))
        {
            foreach (string joins in join.Split(','))
            {
                query = query.Include(joins);
            }
        }

        return await _table.FirstOrDefaultAsync();


    }



    public virtual async Task<Tentity> GetSingleByOrderAsync(Expression<Func<Tentity, bool>> where = null, Func<IQueryable<Tentity>,
       IOrderedQueryable<Tentity>> orderbyVariable = null)
    {
        IQueryable<Tentity> query = _table;

        if (where != null)
        {
            query = query.Where(where);
        }

        if (orderbyVariable != null)
        {
            query = orderbyVariable(query);
        }

        return await query.FirstOrDefaultAsync();
    }



 

    public virtual async Task<Tentity> GetAsync(Expression<Func<Tentity, bool>> where, string joinstring = "")
    {

        IQueryable<Tentity> query = _table;

        query = query.Where(where);

        foreach (var item in joinstring.Split(','))
        {
            query = query.Include(item);
        }

        return await query.FirstOrDefaultAsync();
    }



    public virtual async Task<Tentity> GetByIdAsync(object Id)
    {
        return await _table.FindAsync(Id);
    }



    public virtual async Task<IEnumerable<Tentity>> GetManyAsync(Expression<Func<Tentity, bool>> where)
    {
        return await _table.Where(where).AsNoTracking().ToListAsync();
    }







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
