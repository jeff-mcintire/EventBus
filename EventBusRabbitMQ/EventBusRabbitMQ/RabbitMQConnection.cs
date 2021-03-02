﻿using System;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace EventBusRabbitMQ
{
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private bool _disposed;

        public RabbitMQConnection(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentException((nameof(connectionFactory)));

            if (!IsConnected)
                TryConnect();
        }

        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_disposed;
            }
        }

        public bool TryConnect()
        {
            try
            {
                _connection = _connectionFactory.CreateConnection();
            }
            catch (BrokerUnreachableException e)
            {
                Thread.Sleep((2000));
                _connection = _connectionFactory.CreateConnection();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                throw;
            }

            if (IsConnected)
                return true;
            else
                return false;
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
                throw new InvalidOperationException("No rabbit connection");

            return _connection.CreateModel();
        }
        
        public void Dispose()
        {
            if (_disposed)
                return;

            try
            {
                _connection.Dispose();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}