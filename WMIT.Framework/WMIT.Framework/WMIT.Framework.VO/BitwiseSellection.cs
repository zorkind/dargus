
namespace WMIT.Framework.VO
{
    public class BitwiseSellection
    {
        private int lFields;

        public bool Contains(int pBitwise)
        {
            return (lFields & pBitwise) != 0;
        }

        public bool Any()
        {
            return lFields != 0;
        }

        public int All()
        {
            return lFields;
        }

        public BitwiseSellection Add(int pBitwise)
        {
            if (!Contains(pBitwise))
                lFields = lFields | pBitwise;

            return this;
        }

        public override string ToString()
        {
            return lFields.ToString();
        }
    }
}
