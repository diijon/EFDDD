using System;
using System.Data.Entity;
using EFDDD.DataModel.EF;

namespace DomainDataRepository.Test
{
    public class ContextInitializer : DropCreateDatabaseAlways<Context>
    {
        private readonly Action<IContext> _onSeed;

        public ContextInitializer(Action<IContext> onSeed)
        {
            _onSeed = onSeed;
        }

        public ContextInitializer() : this(null)
        {
        }

        protected override void Seed(Context context)
        {
            if (_onSeed != null) { _onSeed(context); }
        }
    }
}
