
using UnityEngine;

public interface IDragable
{
    void OnStartDrag(Transform dragger);
    void OnDrag();
    void OnEndDrag();
}

