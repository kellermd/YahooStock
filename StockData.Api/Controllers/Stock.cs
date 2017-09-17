using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using StockData.Library.DataContext;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StockData.Api.Controllers
{
    [Route("api/[controller]")]
    public class StockController : Controller
    {
        private StockDisconnectedData _data;

        public StockController(StockDisconnectedData data)
        {
            _data = data;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<KeyValuePair<int, string>> Get()
        {
            var list = _data.GetStockReferenceList();
            return list;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Stock Get(int id)
        {
            return _data.LoadStockGraph(id);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]Stock value)
        {
            _data.SaveStockGraph(value);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Stock value)
        {
            _data.SaveStockGraph(value);
        }

        // DELETE api/values/50
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _data.DeleteStockGraph(id);
        }

        #region Private types

        public class ClientChangeTracker : INotifyPropertyChanged
        {
            private bool _isDirty;

            public bool IsDirty
            {
                get { return _isDirty; }
                set { SetWithNotify(value, ref _isDirty); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void SetWithNotify<T>
              (T value, ref T field, [CallerMemberName] string propertyName = "")
            {
                if (!Equals(field, value))
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public class DisconnectedData<T> where T : DbContext
        {
            protected T _context;

            public DisconnectedData(T context) 
            {
                _context = context;
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }


            protected static void ApplyStateUsingIsKeySet(EntityEntry entry)
            {
                if (entry.IsKeySet)
                {
                    if (((ClientChangeTracker)entry.Entity).IsDirty)
                    {
                        entry.State = EntityState.Modified;
                    }
                    else
                    {
                        entry.State = EntityState.Unchanged;
                    }
                }
                else
                {
                    entry.State = EntityState.Added;
                }
            }
        }

        public class StockDisconnectedData : DisconnectedData<StockDataContext>
        {
            public StockDisconnectedData() : base(new StockDataContext())
            {

            }
            public List<KeyValuePair<int, string>> GetStockReferenceList()
            {
                var stocks = _context.Stock.OrderBy(s => s.StockExchangeCode)
                  .Select(s => new { s.StockID, s.Name })
                  .ToDictionary(t => t.StockID, t => t.Name).ToList();
                return stocks;
            }


            public Stock LoadStockGraph(int id)
            {
                var stock =
                  _context.Stock
                  .FirstOrDefault(s => s.StockID == id);
                return stock;
            }


            public void SaveStockGraph(Stock stock)
            {
                _context.ChangeTracker.TrackGraph
                  (stock, e => DisconnectedData<StockDataContext>.ApplyStateUsingIsKeySet(e.Entry));
                _context.SaveChanges();
            }

            public void DeleteStockGraph(int id)
            {
                //goal:  delete samurai , quotes and secret identity
                //       also delete any joins with battles
                //EF Core supports Cascade delete by convention
                //Even if full graph is not in memory, db is defined to delete
                //But always double check!
                var stock = _context.Stock.Find(id); //NOT TRACKING !!
                _context.Entry(stock).State = EntityState.Deleted; //TRACKING
                _context.SaveChanges();
            }
        }
    #endregion Private types
}
}
