using OOD_Project.WorldGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine;

public class SoundManager : ISubject<SoundEvent>
{
    private readonly List<IObserver<SoundEvent>> _observers = new List<IObserver<SoundEvent>>();

    private Map _map;

    public SoundManager(Map map)
    {
        _map = map;
    }

    public void Attach(IObserver<SoundEvent> observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void Detach(IObserver<SoundEvent> observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(SoundEvent eventData)
    {

        foreach (var observer in _observers)
        {
            observer.OnNotify(eventData);
        }
    }
}
