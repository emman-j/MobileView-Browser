using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WV2Service.Tab
{
    public class WV2TabCollection : IList<WV2Tab>, ICollection<WV2Tab>, IEnumerable<WV2Tab>, IEnumerable
    {
        private List<WV2Tab> Tabs = new List<WV2Tab>();

        public WV2Tab this[int index] 
        { 
            get => Tabs[index]; 
            set => Tabs[index] = value; 
        }

        public int Count => Tabs.Count;

        public bool IsReadOnly => false;

        public void Add(WV2Tab item)
        {
            Tabs.Add(item);
        }

        public void Clear()
        {
            Tabs.Clear();
        }

        public bool Contains(WV2Tab item)
        {
            return Tabs.Contains(item);
        }

        public void CopyTo(WV2Tab[] array, int arrayIndex)
        {
            Tabs.CopyTo(array, arrayIndex);
        }

        public IEnumerator<WV2Tab> GetEnumerator()
        {
            return Tabs.GetEnumerator();
        }

        public int IndexOf(WV2Tab item)
        {
            return Tabs.IndexOf(item);
        }

        public void Insert(int index, WV2Tab item)
        {
            Tabs.Insert(index, item);
        }

        public bool Remove(WV2Tab item)
        {
            return Tabs.Remove(item);
        }

        public void RemoveAt(int index)
        {
            Tabs.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Tabs.GetEnumerator();
        }
    }
}