using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Engine;
using log4net;

namespace uNhAddIns.SessionEasier.Conversations
{
    [Serializable]
    public class NhConversation : AbstractConversation, ISupportOutsidePersistentCall
    {
        // TODO : use System.Transaction to enclose multi DB conversation
        // NOTE : NH2.1 are supporting ambient transactions (even if the implementation is not complete)

        private const string SessionsContextKey = "uNhAddIns.Conversations.NHSessions";
        [NonSerialized] protected static readonly ILog Log = LogManager.GetLogger(typeof (NhConversation));
        [NonSerialized] private readonly ISessionFactoryProvider _factoriesProvider;

        public NhConversation(ISessionFactoryProvider factoriesProvider, ISessionWrapper wrapper)
        {
            if (factoriesProvider == null)
            {
                throw new ArgumentNullException("factoriesProvider");
            }
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }
            _factoriesProvider = factoriesProvider;
            Wrapper = wrapper;
        }

        public NhConversation(ISessionFactoryProvider factoriesProvider, ISessionWrapper wrapper, string id)
            : base(id)
        {
            if (factoriesProvider == null)
            {
                throw new ArgumentNullException("factoriesProvider");
            }
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }
            _factoriesProvider = factoriesProvider;
            Wrapper = wrapper;
        }

        public ISessionWrapper Wrapper { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                IDictionary<ISessionFactory, ISession> contextS = GetFromContext();
                var toDispose = new Dictionary<ISessionFactory, ISession>(contextS);
                foreach (var pair in toDispose)
                {
                    pair.Value.Dispose();
                }
                contextS.Clear();
            }
        }

        protected override void DoStart()
        {
            DoResume();
        }

        protected override void DoPause()
        {
            IDictionary<ISessionFactory, ISession> sessions = GetFromContext();
            foreach (var pair in sessions)
            {
                Commit(pair.Value);
            }
        }

        protected override void DoFlushAndPause()
        {
            IDictionary<ISessionFactory, ISession> sessions = GetFromContext();
            foreach (var pair in sessions)
            {
                ISession session = pair.Value;
                if (session != null && session.IsOpen)
                {
                    FlushAndCommit(session);
                }
            }
        }

        private static void Commit(ISession session)
        {
            if (session.Transaction != null && session.Transaction.IsActive)
            {
                session.Transaction.Commit();
            }
        }

        private static void FlushAndCommit(ISession session)
        {
            if (session.Transaction != null && session.Transaction.IsActive)
            {
                session.Flush();
                session.Transaction.Commit();
            }
        }

        protected override void DoResume()
        {
            IDictionary<ISessionFactory, ISession> sessions = GetFromContext();
            if (sessions.Count > 0)
            {
                var factoriesToRebind = new List<ISessionFactory>(5);
                foreach (var pair in sessions)
                {
                    ISession s = pair.Value;
                    if (s != null && s.IsOpen)
                    {
                        if (!s.IsConnected)
                        {
                            s.Reconnect();
                        }
                    }
                    else
                    {
                        factoriesToRebind.Add(pair.Key);
                    }
                }
                foreach (ISessionFactory factory in factoriesToRebind)
                {
                    Bind(BuildSession((ISessionFactoryImplementor) factory));
                }
            }
            else
            {
                // Bind a session for each SessionFactory
                foreach (ISessionFactory factory in _factoriesProvider)
                {
                    Bind(BuildSession((ISessionFactoryImplementor) factory));
                }
            }
            foreach (var pair in sessions)
            {
                pair.Value.BeginTransaction();
            }
        }

        private static void EnsureFlushMode(ISession session)
        {
            if (session.FlushMode != FlushMode.Never)
            {
                Log.Debug("Disabling automatic flushing of the Session");
                session.FlushMode = FlushMode.Never;
            }
        }

        protected virtual ISession Wrap(ISession session)
        {
            if (UseSupportForOutsidePersistentCall)
            {
                return Wrapper.WrapWithAutoTransaction(session, null, Unbind);
            }
            return Wrapper.Wrap(session, null, Unbind);
        }

        protected virtual ISession BuildSession(ISessionFactoryImplementor factory)
        {
            return factory.OpenSession(null, false, false, factory.Settings.ConnectionReleaseMode);
        }

        protected override void DoEnd()
        {
            IDictionary<ISessionFactory, ISession> sessions = GetFromContext();
            foreach (var pair in sessions)
            {
                ISession session = pair.Value;
                if (session != null && session.IsOpen)
                {
                    FlushAndCommit(session);
                    session.Close();
                }
            }
        }

        protected override void DoAbort()
        {
            IDictionary<ISessionFactory, ISession> sessions = GetFromContext();
            foreach (var pair in sessions)
            {
                ISession session = pair.Value;
                if (session != null && session.IsOpen)
                {
                    session.Close();
                }
            }
        }

        public bool UseSupportForOutsidePersistentCall { get; set; }

        protected IDictionary<ISessionFactory, ISession> GetFromContext()
        {
            object result;
            if (!Context.TryGetValue(SessionsContextKey, out result))
            {
                result = new Dictionary<ISessionFactory, ISession>(2);
                Context[SessionsContextKey] = result;
            }
            return (IDictionary<ISessionFactory, ISession>) result;
        }

        public virtual ISession GetSession(ISessionFactory sessionFactory)
        {
            IDictionary<ISessionFactory, ISession> sessions = GetFromContext();
            ISession result;
            if (!sessions.TryGetValue(sessionFactory, out result))
            {
                throw new ConversationException(
                    "The conversation was not started or was disposed or the SessionFactoryProvider don't manage it.");
            }
            return result;
        }

        public void Bind(ISession session)
        {
            ISessionFactory factory = session.SessionFactory;
            CleanupAnyOrphanedSession(factory);
            EnsureFlushMode(session);
            // wrap the session in the transaction-protection proxy
            ISession sessionToBind = Wrap(session);

            DoBind(sessionToBind, factory);
        }

        private void CleanupAnyOrphanedSession(ISessionFactory factory)
        {
            ISession orphan = DoUnbind(factory);
            if (orphan != null && orphan.IsOpen)
            {
                Log.Warn("Already session bound on call to Bind(); make sure you clean up your sessions!");
                try
                {
                    if (orphan.Transaction != null && orphan.Transaction.IsActive)
                    {
                        try
                        {
                            orphan.Transaction.Rollback();
                        }
                        catch (Exception t)
                        {
                            Log.Debug("Unable to rollback transaction for orphaned session", t);
                        }
                    }
                    orphan.Close();
                }
                catch (Exception t)
                {
                    Log.Debug("Unable to close orphaned session", t);
                }
            }
        }

        private ISession DoUnbind(ISessionFactory factory)
        {
            IDictionary<ISessionFactory, ISession> sessionDic = GetFromContext();
            ISession session = null;
            if (sessionDic != null)
            {
                sessionDic.TryGetValue(factory, out session);
                sessionDic[factory] = null;
            }
            return session;
        }

        public void Unbind(ISession session)
        {
            DoUnbind(session.SessionFactory);
        }

        private void DoBind(ISession session, ISessionFactory factory)
        {
            IDictionary<ISessionFactory, ISession> sessions = GetFromContext();
            sessions[factory] = session;
        }
    }
}