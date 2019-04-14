using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObserverPattern
{
    public abstract class Observer : MonoBehaviour
    {
        public abstract void OnNotify();
    }

    public abstract class Subject : MonoBehaviour
    {
        //A list with observers that are waiting for something to happen
        List<Observer> observers = new List<Observer>();

        //Send notifications if something has happened
        public void Notify()
        {
            foreach (var observer in observers)
                observer.OnNotify();
        }

        //Add observer to the list
        public void AddObserver(Observer observer)
        {
            observers.Add(observer);
        }

        //Remove observer from the list
        public void RemoveObserver(Observer observer)
        {
            observers.Remove(observer);
        }
    }

}



public interface IObserver<T>
{
    void OnCompleted();
    void OnError(System.Exception exception);
    void OnNext(T value);
}

public interface IObservable<T>
{
    System.IDisposable Subscribe(IObserver<T> observer);
}
