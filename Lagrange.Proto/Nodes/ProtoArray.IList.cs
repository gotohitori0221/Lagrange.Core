using System.Collections;

namespace Lagrange.Proto.Nodes;

public partial class ProtoArray : IList<ProtoNode>
{
    public int Count => _list.Count;
    
    public void Add(ProtoNode item) => _list.Add(item);
    
    public bool Remove(ProtoNode item)
    {
        if (!_list.Remove(item)) return false;

        DetachParent(item);
        return true;
    }
    
    public void Clear()
    {
        foreach (var node in _list) DetachParent(node);

        _list.Clear();
    }
    
    public bool Contains(ProtoNode item) => _list.Contains(item);
    
    public int IndexOf(ProtoNode item) => _list.IndexOf(item);
    
    public void Insert(int index, ProtoNode item)
    {
        _list.Insert(index, item);
        item.AssignParent(this);
    }
    
    public void RemoveAt(int index)
    {
        var item = _list[index];
        _list.RemoveAt(index);
        DetachParent(item);
    }
    
    void ICollection<ProtoNode>.CopyTo(ProtoNode[] array, int index) => _list.CopyTo(array, index);
    
    public IEnumerator<ProtoNode> GetEnumerator() => _list.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_list).GetEnumerator();
    
    bool ICollection<ProtoNode>.IsReadOnly => false;
    
    private static void DetachParent(ProtoNode item) => item.Parent = null;
}