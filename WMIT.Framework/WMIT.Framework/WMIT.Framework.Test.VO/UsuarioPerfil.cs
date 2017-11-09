using WMIT.Framework.VO;

namespace WMIT.Framework.Test.VO
{
    public class UsuarioPerfil : VOBase
    {
        public string Descricao { get; set; }

        public class Bitwise : BaseBitwise
        {
            public int Descricao { get { return 1 << 6; } }
            public override int All { get { return (1 << 7) - 1; } }
        }
    }
}
