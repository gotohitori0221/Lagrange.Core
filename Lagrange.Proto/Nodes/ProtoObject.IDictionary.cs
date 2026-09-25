using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Lagrange.Proto.Nodes;

public partial class ProtoObject : IDictionary<int, ProtoNode>
{
    private readonly Dictionary<int, ProtoNode> _fields = new();

    public int Count => _fields.Count;
    
    public bool ContainsKey(int key) => _fields.ContainsKey(key);
    
    public bool TryGetValue(int key, [NotNullWhen(true)] out ProtoNode? value) => _fields.TryGetValue(key, out value);
    
    public void Clear()
    {
        foreach (var node in _fields.Values) DetachParent(node);

        _fields.Clear();
    }
    
    public void Add(int field, ProtoNode? value)
    {
        
        if (value == null) return;
        
        if (_fields.TryGetValue(field, out var removed))
        {
            if (removed is ProtoArray array)
            {
                array.Add(value);
                DetachParent(value);
                value.AssignParent(array);
                return;
            }
            else
            {
                DetachParent(removed);
                array = new ProtoArray(removed.WireType, removed, value);
                value = array;
            }
        }
        
        _fields[field] = value;
        value.AssignParent(this);
    }
    
    public bool Remove(int field)
    {
        bool success = _fields.Remove(field, out var removedNode);
        if (success)
        {
            DetachParent(removedNode);
        }

        return success;
    }

    bool ICollection<KeyValuePair<int, ProtoNode>>.IsReadOnly => false;

    public ICollection<int> Keys => _fields.Keys;

    public ICollection<ProtoNode> Values => _fields.Values;
    
    public void Add(KeyValuePair<int, ProtoNode> property) => Add(property.Key, property.Value);
    
    bool ICollection<KeyValuePair<int, ProtoNode>>.Remove(KeyValuePair<int, ProtoNode> item) => Remove(item.Key);
    
    bool ICollection<KeyValuePair<int, ProtoNode>>.Contains(KeyValuePair<int, ProtoNode> item) => ((IDictionary<int, ProtoNode>)_fields).Contains(item);
    
    void ICollection<KeyValuePair<int, ProtoNode>>.CopyTo(KeyValuePair<int, ProtoNode>[] array, int index) => ((IDictionary<int, ProtoNode>)_fields).CopyTo(array, index);

    IEnumerator<KeyValuePair<int, ProtoNode>> IEnumerable<KeyValuePair<int, ProtoNode>>.GetEnumerator() => _fields.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_fields).GetEnumerator();
    
    private static void DetachParent(ProtoNode? item)
    {
        if (item != null) item.Parent = null;
    }
}