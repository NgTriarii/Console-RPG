using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_Project;
public class Species : ISubject<DeathEvent>
{
    private readonly List<IObserver<DeathEvent>> _members = new List<IObserver<DeathEvent>>();

    public void Attach(IObserver<DeathEvent> observer)
    {
        if (!_members.Contains(observer))
        {
            _members.Add(observer);
        }
    }

    public void Detach(IObserver<DeathEvent> observer)
    {
        _members.Remove(observer);
    }

    public void Notify(DeathEvent eventData)
    {
        foreach (var member in _members)
        {
            if (member != eventData.DeceasedEnemy)
            {
                member.OnNotify(eventData);
            }
        }
    }
}
