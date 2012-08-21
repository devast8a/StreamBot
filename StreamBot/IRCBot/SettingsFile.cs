using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

namespace StreamBot.IRCBot
{
    internal class SettingsFile
    {
        private readonly string _filename;
        private readonly XDocument _document;
        private readonly List<SettingsInstance> _instances;

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public SettingsFile(string filename)
        {
            _filename = filename;
            _document = XDocument.Load(_filename);
            _instances = new List<SettingsInstance>();

            if (_document.Root == null) return;
            var instances = _document.Root.Element("Instances");
            if (instances == null) return;

            foreach (var inst in instances.Elements("Instance"))
            {
                _instances.Add(new SettingsInstance(inst));
            }
        }

        // TODO: Make this totally threadsafe.
        // Child instances need to have synchronized access to the document tree.
        public void Save()
        {
            _lock.EnterWriteLock();
            _document.Save(_filename);
            _lock.ExitWriteLock();
        }

        public IEnumerable<SettingsInstance> Instances
        {
            get { return _instances; }
        }
    }
}