//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Markup;
//using System.Windows.Media.Animation;

//namespace CommunityManagerLib
//{
//  public interface IFilterable
//  {
//    public bool IsAccepted(string filter);
//  }

//  public class OrderedFilteredBindingList<T, K> : Collection<T>, IBindingList where T : IFilterable where K : IComparable
//  {
//    private readonly List<T> inner;
//    private readonly Func<T, K> orderCriteriaProducer;

//    public OrderedFilteredBindingList(IEnumerable<T> values, Func<T, K> orderCriteria)
//    {
//      this.inner = values.OrderBy(q => q).ToList();
//      this.orderCriteriaProducer = orderCriteria;
//    }

//    public bool AllowEdit => throw new NotImplementedException();

//    public bool AllowNew => throw new NotImplementedException();

//    public bool AllowRemove => throw new NotImplementedException();

//    public bool IsSorted => throw new NotImplementedException();

//    public ListSortDirection SortDirection => throw new NotImplementedException();

//    public PropertyDescriptor? SortProperty => throw new NotImplementedException();

//    public bool SupportsChangeNotification => throw new NotImplementedException();

//    public bool SupportsSearching => throw new NotImplementedException();

//    public bool SupportsSorting => throw new NotImplementedException();

//    public event ListChangedEventHandler ListChanged;

//    public void AddIndex(PropertyDescriptor property)
//    {
//      throw new NotImplementedException();
//    }

//    public object? AddNew()
//    {
//      throw new NotImplementedException();
//    }

//    public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
//    {
//      throw new NotImplementedException();
//    }

//    public int Find(PropertyDescriptor property, object key)
//    {
//      throw new NotImplementedException();
//    }

//    public void RemoveIndex(PropertyDescriptor property)
//    {
//      throw new NotImplementedException();
//    }

//    public void RemoveSort()
//    {
//      throw new NotImplementedException();
//    }
//  }
//}
