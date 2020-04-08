# Observr
Library that simplifies the use of the Observer pattern. Easily send notifications and subscribe to these notifications.

A notification can be of any type. A new value that has been created, a simple message, a progress update, ..
## Usage
You can instantiate the `Broker` class and use this to subscribe to or publish changes. Since this class will hold the references to the subscribers, it is best to only have one instance of this class.
```csharp
IBroker brk = new Broker();
```
Or add it as a service in a .NET core application

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddObservr();
    ...
}
```
Then you can inject IBroker in your classes.

### Subscribe
To subscribe to notifications, you need to subscribe instances of classes that implements the IObserver&lt;TE&gt; interface, where TE is the type of the notification that has been published.

The `Subscribe` method returns an `IDisposable`. By calling the `Dispose` method, the observer will get unsubscribed.
```csharp
public class SimpleObserver : IObserver<int>
{
    public Task Handle(int value, CancellationToken cancellationToken)
    {
        Console.WriteLine($"A new Value of {value} has been published");
        return Task.CompletedTask;
    }
}

public static async Task Main(string[] args)
{
    var broker = new Broker();
    var simpleObserver = new SimpleObserver();
    
    var subscription = broker.Subscribe(simpleObserver);
    await broker.Publish(1);
    await broker.Publish(2);
    await broker.Publish(3);
    await broker.Publish(4);
    
    subscription.Dispose();
}

```
Output:
```
A new Value of 1 has been published
A new Value of 2 has been published
A new Value of 3 has been published
A new Value of 4 has been published
```

### Publish
To send a notification you call the `Publish` method on the broker and pass in your notification.
The `Publish` method returns a `Task` which you can await.
```csharp
Broker.Publish(10);
```
Observers which handle `int` will now be notified.
#### Example

```csharp
public class Item
{
    public string Name{get;set;}
}

public class ViewWithItemDataSource : IObserver<Item>
{
    public ViewWithItemDataSource(IBroker broker)
    {
        broker.Subscribe(this)
    }
        
    public List<Item> Items{get;set;}

    public Task Handle(Item value, CancellationToken cancellationToken)
    {
        Items.Add(item);
        return Task.CompletedTask;
    }
}

public class SomeBackgroundTask
{
    private IBroker _broker;
    private SomeService _someService

    public SomeBackgroundTask(IBroker broker, SomeService someService)
    {
        _broker = broker;
        _someService = someService;
    }


    public async Task Run()
    {
        while(true)
        {
            var newItem = await _someService.ReceiveAsync();
            await _broker.Publish(newItem);
            await Task.Delay(1000);
        }
    }
}

```