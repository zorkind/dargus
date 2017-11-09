
namespace WMIT.Framework.VO
{
    public abstract class BaseOrdinal : IOrdinal
    {
        public int Codigo { get; set; }
        public int Ativo { get; set; }
        public int CodigoUsuarioCadastro { get; set; }
        public int Cadastro { get; set; }
        public int CodigoUsuarioAtualizacao { get; set; }
        public int Atualizacao { get; set; }
        private BitwiseSellection lFields;
        public BitwiseSellection Fields
        {
            get { return lFields ?? (lFields = new BitwiseSellection()); }
            set { lFields = value; }
        }

        private BitwiseSellection lParameters;
        public BitwiseSellection Parameters
        {
            get { return lParameters ?? (lParameters = new BitwiseSellection()); }
            set { lParameters = value; }
        }

        private BitwiseSellection lSiblings;
        public BitwiseSellection Siblings
        {
            get { return lSiblings ?? (lSiblings = new BitwiseSellection()); }
            set { lSiblings = value; }
        }

        public virtual IBitwise BaseField { get; set; }

        public abstract bool SiblingsBitwise();

        public bool Bitwise()
        {
            return Fields.Any();
        }

    }


    public abstract class BaseOrdinal<A> : BaseOrdinal, IOrdinal<A>
        where A : IBitwise, new()
    {
        private A lField;
        public A Field
        {
            get
            {
                if (lField == null)
                    lField = new A();

                return lField;
            }
            set
            {
                lField = value;
            }
        }

        public override IBitwise BaseField
        {
            get
            {
                return (IBitwise)Field;
            }
            set
            {
                Field = (A)value;
            }
        }
    }
}
