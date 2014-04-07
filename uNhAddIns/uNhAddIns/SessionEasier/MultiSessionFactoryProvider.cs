using System;
using System.Collections;
using System.Collections.Generic;
using log4net;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Engine;

namespace uNhAddIns.SessionEasier
{
	[Serializable]
	public class MultiSessionFactoryProvider : ISessionFactoryProvider
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(MultiSessionFactoryProvider));

		[NonSerialized]
		private IConfigurationProvider mfc;
		private string defaultSessionFactoryName;
		private Dictionary<string, ISessionFactory> sfs = new Dictionary<string, ISessionFactory>(4);

		public MultiSessionFactoryProvider() : this(new DefaultMultiFactoryConfigurationProvider()) { }

		public MultiSessionFactoryProvider(IConfigurationProvider configurationProvider)
		{
			if (configurationProvider == null)
			{
				throw new ArgumentNullException("configurationProvider");
			}
			mfc = configurationProvider;
		}

		#region ISessionFactoryProvider Members

		public ISessionFactory GetFactory(string factoryId)
		{
			Initialize();
			return string.IsNullOrEmpty(factoryId)
							? InternalGetFactory(defaultSessionFactoryName)
							: InternalGetFactory(factoryId);
		}

		public event EventHandler<EventArgs> BeforeCloseSessionFactory;

		#endregion

		public void Initialize()
		{
			if (sfs.Count != 0)
			{
				return;
			}
			log.Debug("Initialize new session factories reading the configuration.");
			foreach (Configuration cfg in mfc.Configure())
			{
				var sf = (ISessionFactoryImplementor)cfg.BuildSessionFactory();
				string sessionFactoryName = sf.Settings.SessionFactoryName;
				if (!string.IsNullOrEmpty(sessionFactoryName))
				{
					sessionFactoryName = sessionFactoryName.Trim();
				}
				else
				{
					throw new ArgumentException("The session-factory-id was not register; you must assign the name of the factory, example: <session-factory name='HereTheFactoryName'>");
				}
				if (string.IsNullOrEmpty(defaultSessionFactoryName))
				{
					defaultSessionFactoryName = sessionFactoryName;
				}
				sfs.Add(sessionFactoryName, sf);
			}
			mfc = null; // after built the SessionFactories the configuration is not needed
		}

		private ISessionFactory InternalGetFactory(string factoryId)
		{
			try
			{
				return sfs[factoryId];
			}
			catch (KeyNotFoundException)
			{
				throw new ArgumentException("The session-factory-id was not register", "factoryId");
			}
		}

		private void DoBeforeCloseSessionFactory()
		{
			if (BeforeCloseSessionFactory != null)
			{
				BeforeCloseSessionFactory(this, new EventArgs());
			}
		}

		#region Implementation of IEnumerable

		public IEnumerator<ISessionFactory> GetEnumerator()
		{
			Initialize();
			return sfs.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Implementation of IDisposable

		private bool disposed;

		~MultiSessionFactoryProvider()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					foreach (ISessionFactory sessionFactory in sfs.Values)
					{
						if (sessionFactory != null)
						{
							DoBeforeCloseSessionFactory();
							sessionFactory.Close();
						}
					}
					sfs = new Dictionary<string, ISessionFactory>(4);
				}
				disposed = true;
			}
		}

		#endregion
	}
}