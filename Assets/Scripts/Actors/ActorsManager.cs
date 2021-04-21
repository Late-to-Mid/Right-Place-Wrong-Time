using System.Collections.Generic;
using UnityEngine;

public class ActorsManager : MonoBehaviour
{
    public List<Actor> actors { get; private set; }

    private void Awake()
    {
        actors = new List<Actor>();
    }

    public void RegisterActor(Actor actor)
    {
        if (!actors.Contains(actor))
        {
            actors.Add(actor);
        }

    }

    public void UnregisterActor(Actor actor)
    {
        actors.Remove(actor);
    }
}
