using System.Collections.Generic;
using UnityEngine;

public abstract class EntityStateManager : MonoBehaviour
{
}

public abstract class EntityStateManager<T> : EntityStateManager where T : Entity<T>
{
    protected abstract List<EntityState<T>> GetStateList();
}