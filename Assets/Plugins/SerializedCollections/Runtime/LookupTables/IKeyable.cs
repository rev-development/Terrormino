using System.Collections;
using System.Collections.Generic;

namespace AYellowpaper.SerializedCollections
{
    internal interface IKeyable
    {

        IEnumerable Keys { get; }

        void RecalculateOccurences();
        IReadOnlyList<int> GetOccurences(object key);

        void AddKey(object key);
        void RemoveKey(object key);
        void RemoveAt(int index);
        object GetKeyAt(int index);
        int GetCount();
        void RemoveDuplicates();

    }
}