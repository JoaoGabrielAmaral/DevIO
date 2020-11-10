using DevIO.Business.Notifications;
using System.Collections.Generic;

namespace DevIO.Business.Interfaces
{
    public interface INotifier
    {
        bool IsValid();
        List<Notification> Get();
        void Handle(Notification notification);
    }
}
