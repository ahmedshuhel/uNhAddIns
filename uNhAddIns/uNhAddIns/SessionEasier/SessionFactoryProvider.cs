using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Util;
using log4net;

namespace uNhAddIns.SessionEasier
{
    [Serializable]
    public class SessionFactoryProvider : ISessionFactoryProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (SessionFactoryProvider));
        private bool _disposed;

        private IEnumerable<ISessionFactory> _esf;
        private IConfigurationProvider _fcp;
        private ISessionFactory _sf;


        public SessionFactoryProvider() : this(new DefaultSessionFactoryConfigurationProvider())
        {
        }

        public SessionFactoryProvider(IConfigurationProvider configurationProvider)
        {
            if (configurationProvider == null)
            {
                throw new ArgumentNullException("configurationProvider");
            }
            _fcp = configurationProvider;
        }


        public ISessionFactory GetFactory(string factoryId)
        {
            Initialize();
            return _sf;
        }

        public event EventHandler<EventArgs> BeforeCloseSessionFactory;


        public IEnumerator<ISessionFactory> GetEnumerator()
        {
            Initialize();
            return _esf.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Initialize()
        {
            if (_sf == null)
            {
                Log.Debug("Initialize a new session factory reading the configuration.");
                IEnumerator<Configuration> conf = _fcp.Configure().GetEnumerator();
                if (conf.MoveNext())
                {
                    _sf = conf.Current.BuildSessionFactory();
                    _esf = new SingletonEnumerable<ISessionFactory>(_sf);
                }
                _fcp = null; // after built the SessionFactory the configuration is not needed
                if (conf.MoveNext())
                {
                    Log.Warn(
                        @"More than one configurations are available but only the first was used.
Check your configuration or use a Multi-RDBS session-factory provider.");
                }
            }
        }

        private void DoBeforeCloseSessionFactory()
        {
            if (BeforeCloseSessionFactory != null)
            {
                BeforeCloseSessionFactory(this, new EventArgs());
            }
        }

        ~SessionFactoryProvider()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_sf != null)
                    {
                        DoBeforeCloseSessionFactory();
                        _sf.Close();
                        _sf = null;
                    }
                }
                _disposed = true;
            }
        }
    }
}