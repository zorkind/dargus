using System;

namespace WMIT.Framework.Test.Repository
{
    public class RepositoryBase<A, B, C, D> : BLL.BLLBase, IDisposable
        where A : Framework.VO.IVO, new()
        where B : Framework.VO.IBitwise, new()
        where C : Framework.VO.BaseOrdinal<B>, new()
        where D : Framework.VO.IParametros, new()
    {

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool pDisposing)
        {
            if (pDisposing)
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}
