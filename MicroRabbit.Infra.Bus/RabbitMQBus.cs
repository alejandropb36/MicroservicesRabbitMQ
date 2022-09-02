using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MicroRabbit.Infra.Bus
{
    public sealed class RabbitMQBus : IEventBus
    {
        private readonly IMediator _mediator;
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventTypes;
        private RabbitMQSettings _rabbitMQSettings;

        public RabbitMQBus(IMediator mediator, IOptions<RabbitMQSettings> rabbitMQSettings)
        {
            _mediator = mediator;
            _handlers = new Dictionary<string, List<Type>>();
            _eventTypes = new List<Type>();
            _rabbitMQSettings = rabbitMQSettings.Value;
        }

        public void Publish<T>(T @event) where T : Event
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = _rabbitMQSettings.Hostname,
                UserName = _rabbitMQSettings.Username,
                Password = _rabbitMQSettings.Password
            };

            using(var connection = connectionFactory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                var eventName = @event.GetType().Name;
                channel.QueueDeclare(eventName, false, false, false, null);

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("", eventName, null, body);
            }
        }

        public Task SendCommand<T>(T command) where T : Command
        {
            return _mediator.Send(command);
        }

        public void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (!_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }

            if (!_handlers.ContainsKey(eventName))
            {
                _handlers.Add(eventName, new List<Type>());
            }

            if (_handlers[eventName].Any(s => s.GetType() == handlerType))
            {
                throw new ArgumentException($"El handler exception {handlerType.Name} ya fue registrado anteriormente por '{eventName}'", nameof(handlerType));
            }

            _handlers[eventName].Add(handlerType);

            StartBasicConsume<T>();
        }

        private void StartBasicConsume<T>() where T : Event
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = _rabbitMQSettings.Hostname,
                UserName = _rabbitMQSettings.Username,
                Password = _rabbitMQSettings.Password,
                DispatchConsumersAsync = true
            };

            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            var eventName = typeof(T).Name;
            channel.QueueDeclare(eventName, false, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += Consumer_Recieved;
            channel.BasicConsume(eventName, true, consumer);
        }

        private async Task Consumer_Recieved(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var message = Encoding.UTF8.GetString(e.Body.Span);

            try
            {
                await ProcessEvent(eventName, message).ConfigureAwait(false);
            }
            catch(Exception ex)
            {

            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_handlers.ContainsKey(eventName))
            {
                var subscriptions = _handlers[eventName];

                foreach(var subsciption in subscriptions)
                {
                    var handler = Activator.CreateInstance(subsciption);
                    if (handler == null) continue;

                    var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                    var @event = JsonConvert.DeserializeObject(message, eventType);
                    var concretType = typeof(IEventHandler<>).MakeGenericType(eventType);

                    await (Task)concretType.GetMethod("Handle").Invoke(handler, new object[] {@event});
                }
            }
        }
    }
}
